using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Splines;

public class SplineManager : MonoBehaviour
{
    [SerializeField] private DummyTrain _trainPrefab;
    [SerializeField] private SplineContainer _splineContainer;

    private LinkedList<Rail> _railLinkedList = new();
    private MapLoader map;
    private List<BezierKnot> _knots = new();
    private Spline _spline;

    private void Awake() => InitSpline();
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
    }
}
