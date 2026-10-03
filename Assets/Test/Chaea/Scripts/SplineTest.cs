using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] private Rail _railPrefab;
    [SerializeField] private DummyTrain _dummyTrain;

    private Camera _cam;

    private void Awake() => CacheComponents();

    private void Update()
    {
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
                RailManager.Instance.TryRailwayPlace(coord);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector2Int coord = hit.point.WorldToCoord();

                IInteractable interactable = Map.Instance.GetHoldable(coord);

                Rail rail = interactable as Rail;
                if (rail != null)
                {
                    rail.ButtonInteract(null);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_dummyTrain.gameObject.activeSelf == false)
            {
                _dummyTrain.gameObject.SetActive(true);
            }
            else
            {
                _dummyTrain.gameObject.SetActive(false);
            }
        }
    }

    private void CacheComponents()
    {
        _cam = Camera.main;
    }
}
