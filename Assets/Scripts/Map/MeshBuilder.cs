using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBuilder : MonoBehaviour
{
    private Mesh _mesh;
    private MeshRenderer _meshRenderer;
    private int[,] _worldMap;
    private int _chunkIndex;
    private float _textureSize;
    private float _atlasSize;

    private List<Vector3> _vertices = new();
    private List<int> _triangles = new();
    private List<Vector2> _uvs = new();

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
    public void CreateChunkMesh(int chunkIndex, int[,] worldMap, UnityEngine.Material material, float textureSize, float atlasSize)
    {
        _chunkIndex = chunkIndex;
        _worldMap = worldMap;
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
                Vector2Int worldCoord = new Vector2Int(_chunkIndex * ChunkManager.CHUNK_SIZE + x, y);
                if (worldCoord.x < 0 ||
                    worldCoord.y < 0 ||
                    worldCoord.x >= _worldMap.GetLength(0) ||
                    worldCoord.y >= _worldMap.GetLength(1)) continue;

                if (_worldMap[worldCoord.x, worldCoord.y] == 0) continue;

                Vector2Int localCoord = new Vector2Int(x, y);

                NeighborFlags neighborFlags = GetNeighborFlags(worldCoord);

                CreateBlockShape(worldCoord, localCoord, neighborFlags);
            }
        }
    }

    /// <summary>
    /// Block 하나의 shape를 그리는 함수
    /// </summary>
    /// <param name="worldCoord"> 청크의 절대 좌표 </param>
    /// <param name="localCoord"> 청크의 상대 좌표 </param>
    /// <param name="neighborFlags"> 블록의 이웃 </param>
    private void CreateBlockShape(Vector2Int worldCoord, Vector2Int localCoord, NeighborFlags neighborFlags)
    {
        BlockType blocktype = (BlockType)_worldMap[worldCoord.x, worldCoord.y];
        if (blocktype == BlockType.Water)
        {
            CreatePosYWater(localCoord, Vector3.one);

            if (IsBorder(worldCoord))
                CreateWaterFall(localCoord, neighborFlags);
            return;
        }

        CreatePosYShape(localCoord, blocktype, Vector3.one);
        CreateNegYShape(localCoord, blocktype, Vector3.one);

        if ((neighborFlags & NeighborFlags.Right) == 0) CreatePosXShape(localCoord, blocktype, Vector3.one);
        if ((neighborFlags & NeighborFlags.Left) == 0) CreateNegXShape(localCoord, blocktype, Vector3.one);
        if ((neighborFlags & NeighborFlags.Forward) == 0) CreatePosZShape(localCoord, blocktype, Vector3.one);
        if ((neighborFlags & NeighborFlags.Back) == 0) CreateNegZShape(localCoord, blocktype, Vector3.one);
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

        AddUVs(BlockTypeToColor(BlockType.Water));
    }

    private bool IsBorder(Vector2Int coord)
    {
        if (coord.x == 0 ||
            coord.y == 0 ||
            coord.x == _worldMap.GetLength(0) - 1 ||
            coord.y == _worldMap.GetLength(1) - 1)
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

        if (coord.y == _worldMap.GetLength(1) - 1)
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

        AddUVs(BlockTypeToColor(blockType));
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

        AddUVs(BlockTypeToColor(blockType));
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

        AddUVs(BlockTypeToColor(blockType));
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

        AddUVs(BlockTypeToColor(blockType));
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

        AddUVs(BlockTypeToColor(blockType));
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

        AddUVs(BlockTypeToColor(blockType));
    }

    private Vector2 BlockTypeToColor(BlockType blockType)
    {
        switch (blockType)
        {
            case BlockType.Grass:
                return new Vector2(0, 3);
            case BlockType.Tree:
                return new Vector2(2, 3);
            case BlockType.Rock:
                return new Vector2(1, 3);
            case BlockType.Water:
                return new Vector2(3, 3);
            case BlockType.Obstacle:
                return new Vector2(0, 2);
            default:
                return new Vector2(0, 3);
        }
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

    private NeighborFlags GetNeighborFlags(Vector2Int worldCoord)
    {
        NeighborFlags flags = NeighborFlags.None;

        for (int i = 0; i < _neighbors.Length; i++)
        {
            if ((worldCoord.y + _neighbors[i].y < 0) ||
                (worldCoord.y + _neighbors[i].y >= _worldMap.GetLength(1)) ||
                (worldCoord.x + _neighbors[i].x < 0) ||
                (worldCoord.x + _neighbors[i].x >= _worldMap.GetLength(0)))
            {
                continue;
            }

            // Neighbor가 있다면
            if (_worldMap[worldCoord.x + _neighbors[i].x, worldCoord.y + _neighbors[i].y] != 0 &&
                _worldMap[worldCoord.x + _neighbors[i].x, worldCoord.y + _neighbors[i].y] != 4)
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
