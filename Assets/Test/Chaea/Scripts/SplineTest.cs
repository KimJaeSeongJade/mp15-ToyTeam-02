using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineTest : MonoBehaviour
{
    [SerializeField] private SplineManager _splineManager;
    [SerializeField] private GameObject[] _trains;

    private Camera _cam;

    private void Awake() => CacheComponents();

    private void Update()
    {
        ReadMouseInput();
    }

    private void ReadMouseInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (GameObject gameObject in _trains)
            {
                Train train = gameObject.GetComponent<Train>();

                if (train.gameObject.activeSelf == false)
                {
                    train.gameObject.SetActive(true);
                }
                else
                {
                    train.gameObject.SetActive(false);
                }
            }
        }
    }

    private void CacheComponents()
    {
        _cam = Camera.main;
    }
}
