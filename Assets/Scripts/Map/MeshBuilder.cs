using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBuilder : MonoBehaviour
{
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private int[,] _chunkMap;
    private int _chunkIndex;
    private float _textureSize;
    private float _atlasSize;

    private List<Vector3> _vertices = new();
    private List<int> _triangles = new();
    private List<Vector2> _uvs = new();
    private Dictionary<BlockType, BlockTypeColor> _blockTypeColors;
    private Vector2Int _color;
    private float _waterHeight = 0.7f;
    private float _waterFallHeight = 10f;

    private Vector2Int[] _neighbors = {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
    };

    private NeighborFlags[] _neighborFlags = {
        NeighborFlags.Right,
        NeighborFlags.Left,
        NeighborFlags.Forward,
        NeighborFlags.Back
    };

    private void Awake() => CacheComponents();

    /// <summary>
    /// Chunk 하나의 mesh를 그리는 함수
    /// </summary>
    /// <param name="chunkIndex"> Chunk의 번호 </param>
    /// <param name="map"></param>
    /// <param name="material"></param>
    /// <param name="textureSize"></param>
    /// <param name="atlasSize"></param>
    public void CreateChunkMesh(int chunkIndex, int[,] chunkMap,
        Dictionary<BlockType, BlockTypeColor> blockTypeColors, UnityEngine.Material material, float textureSize, float atlasSize)
    {
        _chunkIndex = chunkIndex;
        _chunkMap = chunkMap;
        _blockTypeColors = blockTypeColors;
        _meshRenderer.material = material;
        _textureSize = textureSize;
        _atlasSize = atlasSize;

        transform.position = new Vector3(chunkIndex * ChunkManager.CHUNK_SIZE, -1, 0);

        ResetMesh();
        CreateChunkShape();
        UpdateMesh();
    }

    /// <summary>
    /// Chunk 하나의 shape를 그리는 함수 
    /// </summary>
    private void CreateChunkShape()
    {
        for (int y = 0; y < ChunkManager.CHUNK_SIZE; y++)
        {
            for (int x = 0; x < ChunkManager.CHUNK_SIZE; x++)
            {
                if (_chunkMap[x, y] == 0) continue;

                Vector2Int localCoord = new Vector2Int(x, y);

                NeighborFlags neighborFlags = GetNeighborFlags(localCoord);

                CreateBlockShape(localCoord, neighborFlags);
            }
        }
    }

    /// <summary>
    /// Block 하나의 shape를 그리는 함수
    /// </summary>
    /// <param name="chunkCoord"> 청크의 상대 좌표 </param>
    /// <param name="neighborFlags"> 블록의 이웃 </param>
    private void CreateBlockShape(Vector2Int chunkCoord, NeighborFlags neighborFlags)
    {
        BlockType blocktype = (BlockType)_chunkMap[chunkCoord.x, chunkCoord.y];
        _color = Vector2Int.zero;

        if (blocktype == BlockType.Water)
        {
            CreatePosYWater(chunkCoord, Vector3.one);

            if (IsBorder(chunkCoord))
                CreateWaterFall(chunkCoord, neighborFlags);
            return;
        }

        CreatePosYShape(chunkCoord, blocktype, Vector3.one);
        CreateNegYShape(chunkCoord, blocktype, Vector3.one);

        if ((neighborFlags & NeighborFlags.Right) == 0) CreatePosXShape(chunkCoord, blocktype, Vector3.one);
        if ((neighborFlags & NeighborFlags.Left) == 0) CreateNegXShape(chunkCoord, blocktype, Vector3.one);
        if ((neighborFlags & NeighborFlags.Forward) == 0) CreatePosZShape(chunkCoord, blocktype, Vector3.one);
        if ((neighborFlags & NeighborFlags.Back) == 0) CreateNegZShape(chunkCoord, blocktype, Vector3.one);
    }

    /// <summary>
    /// Mesh를 그리는 함수
    /// </summary>
    private void UpdateMesh()
    {
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.uv = _uvs.ToArray();

        _mesh.RecalculateNormals();
    }

    /*
        .            .
        5            7
    .            .
    4            6


        .            .
        1            3
    .            .
    0            2
    */

    private void CreatePosYWater(Vector2Int coord, Vector3 size, bool reverse = false)
    {
        int verticesOffset = _vertices.Count;

        _vertices.Add(new Vector3(coord.x + 0, _waterHeight, coord.y + 0));        // 4
        _vertices.Add(new Vector3(coord.x + 0, _waterHeight, coord.y + size.z));   // 5
        _vertices.Add(new Vector3(coord.x + size.x, _waterHeight, coord.y + 0));        // 6
        _vertices.Add(new Vector3(coord.x + size.x, _waterHeight, coord.y + size.z));   // 7

        if (reverse)
            AddTriangles(verticesOffset, false);
        else
            AddTriangles(verticesOffset, true);

        if (_color == Vector2Int.zero) _color = BlockTypeToColor(BlockType.Water);
            AddUVs(_color);
    }

    private bool IsBorder(Vector2Int coord)
    {
        if (coord.x == 0 ||
            coord.y == 0 ||
            coord.x == _chunkMap.GetLength(0) - 1 ||
            coord.y == _chunkMap.GetLength(1) - 1)
            return true;
        else return false;
    }

    private void CreateWaterFall(Vector2Int coord, NeighborFlags neighborFlags)
    {
        // 가장 우측인 경우에는 폭포를 생성하지 않도록
        /*
        if (coord.x == _worldMap.GetLength(0) - 1)
        {
            CreatePosXShape(coord, BlockType.Water, new Vector3(1, _waterHeight, 1));
            CreatePosXShape(coord, BlockType.Water, new Vector3(1, -_waterFallHeight, 1), true);
        }
        */

        if (coord.x == 0)
        {
            CreateNegXShape(coord, BlockType.Water, new Vector3(1, _waterHeight, 1));
            CreateNegXShape(coord, BlockType.Water, new Vector3(1, -_waterFallHeight, 1), true);
        }

        if (coord.y == _chunkMap.GetLength(1) - 1)
        {
            CreatePosZShape(coord, BlockType.Water, new Vector3(1, _waterFallHeight, 1), true);
        }

        if (coord.y == 0)
        {
            CreateNegZShape(coord, BlockType.Water, new Vector3(1, _waterHeight, 1));
            CreateNegZShape(coord, BlockType.Water, new Vector3(1, -_waterFallHeight, 1), true);
        }
    }

    private void CreatePosXShape(Vector2Int coord, BlockType blockType, Vector3 size, bool reverse = false)
    {
        int verticesOffset = _vertices.Count;

        _vertices.Add(new Vector3(coord.x + size.x, 0, coord.y + 0));        // 2
        _vertices.Add(new Vector3(coord.x + size.x, size.y, coord.y + 0));        // 6
        _vertices.Add(new Vector3(coord.x + size.x, 0, coord.y + size.z));   // 3
        _vertices.Add(new Vector3(coord.x + size.x, size.y, coord.y + size.z));   // 7

        if (reverse)
            AddTriangles(verticesOffset, false);
        else
            AddTriangles(verticesOffset, true);

        AddUVs(_color);
    }

    private void CreateNegXShape(Vector2Int coord, BlockType blockType, Vector3 size, bool reverse = false)
    {
        int verticesOffset = _vertices.Count;

        _vertices.Add(new Vector3(coord.x + 0, 0, coord.y + 0));        // 0
        _vertices.Add(new Vector3(coord.x + 0, size.y, coord.y + 0));        // 4
        _vertices.Add(new Vector3(coord.x + 0, 0, coord.y + size.z));   // 1
        _vertices.Add(new Vector3(coord.x + 0, size.y, coord.y + size.z));   // 5


        if (reverse)
            AddTriangles(verticesOffset, true);
        else
            AddTriangles(verticesOffset, false);

        AddUVs(_color);
    }

    private void CreatePosYShape(Vector2Int coord, BlockType blockType, Vector3 size, bool reverse = false)
    {
        int verticesOffset = _vertices.Count;

        _vertices.Add(new Vector3(coord.x + 0, size.y, coord.y + 0));        // 4
        _vertices.Add(new Vector3(coord.x + 0, size.y, coord.y + size.z));   // 5
        _vertices.Add(new Vector3(coord.x + size.x, size.y, coord.y + 0));        // 6
        _vertices.Add(new Vector3(coord.x + size.x, size.y, coord.y + size.z));   // 7


        if (reverse)
            AddTriangles(verticesOffset, false);
        else
            AddTriangles(verticesOffset, true);

        if (_color == Vector2Int.zero) _color = BlockTypeToColor(blockType);
        AddUVs(_color);
    }

    private void CreateNegYShape(Vector2Int coord, BlockType blockType, Vector3 size, bool reverse = false)
    {
        int verticesOffset = _vertices.Count;

        _vertices.Add(new Vector3(coord.x + 0, 0, coord.y + 0));        // 0
        _vertices.Add(new Vector3(coord.x + 0, 0, coord.y + size.z));   // 1
        _vertices.Add(new Vector3(coord.x + size.x, 0, coord.y + 0));        // 2
        _vertices.Add(new Vector3(coord.x + size.x, 0, coord.y + size.z));   // 3

        if (reverse)
            AddTriangles(verticesOffset, true);
        else
            AddTriangles(verticesOffset, false);

        AddUVs(_color);
    }

    private void CreatePosZShape(Vector2Int coord, BlockType blockType, Vector3 size, bool reverse = false)
    {
        int verticesOffset = _vertices.Count;

        _vertices.Add(new Vector3(coord.x + size.x, 0, coord.y + size.z));   // 3
        _vertices.Add(new Vector3(coord.x + size.x, size.y, coord.y + size.z));   // 7
        _vertices.Add(new Vector3(coord.x + 0, 0, coord.y + size.z));   // 1
        _vertices.Add(new Vector3(coord.x + 0, size.y, coord.y + size.z));   // 5


        if (reverse)
            AddTriangles(verticesOffset, false);
        else
            AddTriangles(verticesOffset, true);

        AddUVs(_color);
    }

    private void CreateNegZShape(Vector2Int coord, BlockType blockType, Vector3 size, bool reverse = false)
    {
        int verticesOffset = _vertices.Count;

        _vertices.Add(new Vector3(coord.x + size.x, 0, coord.y + 0));   // 2
        _vertices.Add(new Vector3(coord.x + size.x, size.y, coord.y + 0));   // 6
        _vertices.Add(new Vector3(coord.x + 0, 0, coord.y + 0));   // 0
        _vertices.Add(new Vector3(coord.x + 0, size.y, coord.y + 0));   // 4


        if (reverse)
            AddTriangles(verticesOffset, true);
        else
            AddTriangles(verticesOffset, false);

        AddUVs(_color);
    }

    private Vector2Int BlockTypeToColor(BlockType blockType)
    {
        BlockTypeColor blockTypeColor;

        if (!_blockTypeColors.ContainsKey(blockType)) blockTypeColor = _blockTypeColors[BlockType.Grass];
        else blockTypeColor = _blockTypeColors[blockType];

        float roll = UnityEngine.Random.Range(0f, 1f);
        float cumulative = 0f;


        Vector2Int defaultColor = Vector2Int.zero;

        foreach(KeyValuePair<Vector2Int, float> pair in blockTypeColor.BlockColors)
        {
            defaultColor = pair.Key;
            cumulative += pair.Value;
            if (roll <= cumulative) return pair.Key;
        }

        return defaultColor;
    }

    private void AddTriangles(int verticesOffset, bool isClockwise)
    {
        if (isClockwise)
        {
            _triangles.Add(verticesOffset);
            _triangles.Add(verticesOffset + 1);
            _triangles.Add(verticesOffset + 2);
            _triangles.Add(verticesOffset + 1);
            _triangles.Add(verticesOffset + 3);
            _triangles.Add(verticesOffset + 2);
        }
        else
        {
            _triangles.Add(verticesOffset);
            _triangles.Add(verticesOffset + 2);
            _triangles.Add(verticesOffset + 1);
            _triangles.Add(verticesOffset + 1);
            _triangles.Add(verticesOffset + 2);
            _triangles.Add(verticesOffset + 3);
        }
    }

    private void AddUVs(Vector2 atlasPosition)
    {
        float size = 1f / _atlasSize;
        float padding = 0.5f / _textureSize; // for mipmap(texture) bleeding

        float x = atlasPosition.x * size;
        float y = atlasPosition.y * size;


        _uvs.Add(new Vector2(x + padding, y + padding));
        _uvs.Add(new Vector2(x + size - padding, y + padding));
        _uvs.Add(new Vector2(x + padding, y + size - padding));
        _uvs.Add(new Vector2(x + size - padding, y + size - padding));
    }

    private NeighborFlags GetNeighborFlags(Vector2Int localCoord)
    {
        NeighborFlags flags = NeighborFlags.None;

        for (int i = 0; i < _neighbors.Length; i++)
        {
            if ((localCoord.y + _neighbors[i].y < 0) ||
                (localCoord.y + _neighbors[i].y >= _chunkMap.GetLength(1)) ||
                (localCoord.x + _neighbors[i].x < 0) ||
                (localCoord.x + _neighbors[i].x >= _chunkMap.GetLength(0)))
            {
                continue;
            }

            // Neighbor가 있다면
            if (_chunkMap[localCoord.x + _neighbors[i].x, localCoord.y + _neighbors[i].y] != 0 &&
                _chunkMap[localCoord.x + _neighbors[i].x, localCoord.y + _neighbors[i].y] != 4)
            {
                flags |= _neighborFlags[i];
            }
        }

        return flags;
    }

    private void ResetMesh()
    {
        _mesh.Clear();
        _vertices.Clear();
        _triangles.Clear();
        _uvs.Clear();
    }

    private void CacheComponents()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
    }
}
