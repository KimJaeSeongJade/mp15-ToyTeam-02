using DG.Tweening.Plugins;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class MapLoader : MonoBehaviour
{
    [SerializeField] private GameMode _gameMode;
    [SerializeField] private TextAsset[] _files;

    /// <summary>
    /// 저장한 맵 / 샘플 맵
    /// </summary>
    public int[,] Map => _map;

    /// <summary>
    /// 저장한 초기 무한모드 맵
    /// </summary>
    public int[,] InfiniteInitialMap => _infiniteInitialMap;
    public int[,] QuickLastMap => _quickLastMap;

    private int[,] _map;
    private int[,] _infiniteInitialMap;
    private int[,] _quickLastMap;
    private string _docId = "1aNNeM5KLbdZ4hOK1AM0-LkXCTkBRzoJswdcpBvjRjKI";

    private Dictionary<GameMode, string> _docGids = new Dictionary<GameMode, string> {
        { GameMode.Quick, "1355842421" },
        { GameMode.Infinite, "1355842421" },
        { GameMode.Tutorial, "1277891633" },
        { GameMode.Time, "0" }
        };
    /// <summary>
    /// 외부에서 맵 정보를 읽어와서 배열로 저장한 여부
    /// </summary>
    public bool CanLoadMap { get; private set; }

    /// <summary>
    /// 현재 게임 모드
    /// </summary>
    public GameMode GameMode => _gameMode;

    /// <summary>
    /// 외부에서 맵 불러와서 MapLoader Map에 저장 완료
    /// </summary>
    public event Action OnMapSaved;

    private void Start()
    {
        _gameMode = GameManager.Instance.GMode;
        StartCoroutine(LoadMapDataRoutine(_docId, _gameMode));
    }    

    private IEnumerator LoadMapDataRoutine(string docId, GameMode gameMode)
    {
        UnityWebRequest www = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{docId}/export?format=csv&gid={_docGids[gameMode]}");
        www.timeout = 5;
        yield return www.SendWebRequest();

        string text;

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            text = _files[(int)gameMode].text;
        }
        else
        {
            text = www.downloadHandler.text;
        }

        ParseCSV(text);
        OnMapSaved?.Invoke();

        if (_gameMode == GameMode.Time || _gameMode == GameMode.Tutorial) CanLoadMap = true;
    }

    private void ParseCSV(string text)
    {
        string[] lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        int columnStartIndex = 1;
        int rowStartIndex = 2;

        int row;

        // 1. 월드맵 (샘플맵)
        if (lines.Length - rowStartIndex > ChunkManager.CHUNK_SIZE)
        {
            row = ChunkManager.CHUNK_SIZE;
        }
        else
        {
            row = lines.Length - rowStartIndex;
        }

        int column = lines[0].Split(',').Length - columnStartIndex;

        int totalChunk = (column - 1) % ChunkManager.CHUNK_SIZE;

        _map = new int[column, row];


        for (int i = 0; i < row; i++)
        {
            string[] values = lines[i + rowStartIndex].Split(',');

            int y = row - 1 - i;

            for (int x = columnStartIndex; x < values.Length; x++)
            {
                int.TryParse(values[x], out _map[x - columnStartIndex, y]);
            }
        }

        // 2. 무한 모드 초기 맵
        rowStartIndex = ChunkManager.CHUNK_SIZE + 5;

        if (lines.Length < rowStartIndex) return;

        int infinityInitialMapColumn = 0;

        string[] infinityIndices = lines[rowStartIndex - 1].Split(',');

        for (int x = columnStartIndex; x < columnStartIndex + ChunkManager.CHUNK_SIZE; x++)
        {
            int value;
            int.TryParse(infinityIndices[x], out value);

            if (infinityInitialMapColumn < value) infinityInitialMapColumn = value;
        }

        // index이므로 실제 개수는 1 추가
        infinityInitialMapColumn++;

        _infiniteInitialMap = new int[infinityInitialMapColumn, ChunkManager.CHUNK_SIZE];

        for (int i = 0; i < ChunkManager.CHUNK_SIZE; i++)
        {
            string[] values = lines[i + rowStartIndex].Split(',');

            int y = row - 1 - i;

            for (int x = columnStartIndex; x < columnStartIndex + infinityInitialMapColumn; x++)
            {
                int.TryParse(values[x], out _infiniteInitialMap[x - columnStartIndex, y]);
            }
        }

        // 3. 퀵 모드 마지막 맵
        rowStartIndex = ChunkManager.CHUNK_SIZE * 2 + 8;

        if (lines.Length < rowStartIndex) return;

        string[] quickIndices = lines[rowStartIndex - 1].Split(',');

        int quickLastMapColumn = ChunkManager.CHUNK_SIZE * 2 - int.Parse(quickIndices[columnStartIndex]);

        _quickLastMap = new int[quickLastMapColumn, ChunkManager.CHUNK_SIZE];

        for (int i = 0; i < ChunkManager.CHUNK_SIZE; i++)
        {
            string[] values = lines[i + rowStartIndex].Split(',');

            int y = row - 1 - i;

            for (int x = columnStartIndex; x < columnStartIndex + quickLastMapColumn; x++)
            {
                int.TryParse(values[x], out _quickLastMap[x - columnStartIndex, y]);
            }
        }
    }

    // 디버깅용
    private void PrintMapData(int[,] map)
    {
        for (int j = map.GetLength(1) - 1; j >= 0; j--)
        {
            string a = "";
            for (int i = 0; i < map.GetLength(0); i++)
            {
                a += $"{map[i, j]} ";
            }
            Debug.Log(a);
        }
    }
}