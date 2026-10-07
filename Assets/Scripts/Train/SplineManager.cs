using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SplineManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _trains;
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private FilledBarUI _filledBarUI;

    public event Action<int> OnRailwayChanged;
    public int SplineCount => _spline.Count;

    private LinkedList<Rail> _railLinkedList = new();
    private MapLoader map;
    private List<BezierKnot> _knots = new();
    private Spline _spline;

    private void Awake() => InitSpline();
    private void OnEnable() => BindBarLoadingEvents();
    private void OnDisable() => UnbindBarLoadingEvents();

    /// <summary>
    /// 초기 rail을 추가하며 Spline 초기화
    /// </summary>
    /// <param name="initialRailway"> 초기 rail 좌표 리스트 </param>
    public void InitSpline()
    {
        _spline = _splineContainer.AddSpline();
    }

    /// <summary>
    /// 열차가 지나가는 railway(spline knot)를 생성
    /// </summary>
    /// <param name="coord"> spline knot의 좌표 </param>
    public void AddSplineKnot(Vector2Int coord)
    {
        _knots.Add(new(coord.CoordToWorld()));
        _spline.Knots = _knots;

        SplineRange all = new SplineRange(0, _spline.Count);
        _spline.SetTangentMode(all, TangentMode.AutoSmooth);
        OnRailwayChanged?.Invoke(_spline.Count);
    }

    /// <summary>
    /// 마지막에 배치한 railway(spline knot) 삭제
    /// </summary>
    public void RemoveLastSplineKnot()
    {
        _knots.RemoveAt(_spline.Count - 1);
        _spline.Knots = _knots;

        SplineRange all = new SplineRange(0, _spline.Count);
        _spline.SetTangentMode(all, TangentMode.AutoSmooth);
        OnRailwayChanged?.Invoke(_spline.Count);
    }

    private void LoadTrain()
    {
        foreach (GameObject gameObject in _trains)
        {
            Train train = gameObject.GetComponent<Train>();

            if (train.gameObject.activeSelf == false)
            {
                train.gameObject.SetActive(true);

                if (GameManager.Instance.GMode != GameMode.Tutorial)
                    train.StartTrainDepartRoutine();
            }
            else
            {
                train.gameObject.SetActive(false);
            }
        }

        _playerController.gameObject.SetActive(true);
        FadeEffect.Instance.FadeIn();
    }

    private void BindBarLoadingEvents()
    {
        _filledBarUI.OnBarLoaded += LoadTrain;
    }

    private void UnbindBarLoadingEvents()
    {
        _filledBarUI.OnBarLoaded -= LoadTrain;
    }
}
