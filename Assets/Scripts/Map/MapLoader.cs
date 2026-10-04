using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapLoader : MonoBehaviour
{
    /// <summary>
    /// 저장한 월드맵
    /// </summary>
    public int[,] WorldMap => _worldMap;

    // _worldMap <청크의 인덱스, 청크의 로컬맵>
    private int[,] _worldMap; 

    /// <summary>
    /// 외부에서 맵 정보를 읽어와서 배열로 저장한 여부
    /// </summary>
    public bool CanLoadMap { get; private set; }

    private void Start()
    {
        string docId = "1aNNeM5KLbdZ4hOK1AM0-LkXCTkBRzoJswdcpBvjRjKI";
        string gid = "0";
        string testRailGid = "1935130282";
        StartCoroutine(LoadMapDataRoutine(docId, gid));
        // StartCoroutine(LoadMapDataRoutine(docId, testRailGid));
    }

    private IEnumerator LoadMapDataRoutine(string docId, string gid)
    {
        Debug.Log("Requesting Map Data...");
        UnityWebRequest www = UnityWebRequest.Get($"https://docs.google.com/spreadsheets/d/{docId}/export?format=tsv&gid={gid}");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ProtocolError || www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Web Request Error!");
            yield break;
        }

        ParseMapData(www.downloadHandler.text);
        Map.Instance.SetMapData(_worldMap);
        CanLoadMap = true;
    }

    private void ParseMapData(string text)
    {
        string[] lines = text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        int columnStartIndex = 1;
        int rowStartIndex = 2;

        int row = lines.Length - rowStartIndex;
        int column = lines[0].Split('\t').Length - columnStartIndex;

        int totalChunk = (column - 1) % ChunkManager.CHUNK_SIZE;

        _worldMap = new int[column, row];

        for (int i = 0; i < row; i++)
        {
            string[] values = lines[i + rowStartIndex].Split('\t');

            int y = row - 1 - i;

            for (int x = columnStartIndex; x < values.Length - columnStartIndex && x < column; x++)
            {
                int.TryParse(values[x], out _worldMap[x - columnStartIndex, y]);
            }
        }

        // Debug.Log($"Map Data loaded! Row : {row}, Column = {column}");
    }
}