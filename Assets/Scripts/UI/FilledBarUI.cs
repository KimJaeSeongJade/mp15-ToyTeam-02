using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilledBarUI : MonoBehaviour
{
    [SerializeField] private float _delayTime;
    [SerializeField] private Image _bar;
    [SerializeField] private float _elapsedTime;


    private void Awake()
    {
        _elapsedTime = 0;
    }

    private void Update()
    {
        CurrentTime();
        _bar.fillAmount = _elapsedTime / _delayTime;        
    }   
    
    public void CurrentTime()
    {
        _elapsedTime += Time.deltaTime;
    }
}