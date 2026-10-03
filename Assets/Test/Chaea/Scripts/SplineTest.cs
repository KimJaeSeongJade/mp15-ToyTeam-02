using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineTest : MonoBehaviour
{
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] private Rail _railPrefab;

    private Camera _cam;

    private void Awake() => CacheComponents();

    private void Update()
    {
        if (!_splineManager.IsInitialized) return;
        ReadMouseInput();
    }

    private void ReadMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector2Int coord = hit.point.WorldToCoord();
                AddRail(coord);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject)
                {

                }
            }
        }
        
    }

    private void AddRail(Vector2Int coord)
    {
        Debug.Log($"{coord.x} {coord.y}");
        Rail newRail = Instantiate(_railPrefab);
        newRail.transform.position = coord.CoordToWorld();
        RailManager.Instance.AddRailWay(newRail);

        _splineManager.AddSplineKnot(coord);
    }

    private void CacheComponents()
    {
        _cam = Camera.main;
    }
}
