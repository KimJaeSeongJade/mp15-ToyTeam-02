using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TextNumChanger : MonoBehaviour
{
    [SerializeField] private float _multiV;
    private TextMeshProUGUI _text => GetComponent<TextMeshProUGUI>();
     
    public void ChangeNum(float value)
    {
        value *= _multiV;
        _text.text = value.ToString("0.##");
    }

    public void ChangeScoreNum(float value)
    {
        int ivalue = (int)value;
        _text.text = ivalue.ToString("D4");
    }

    public void ChangeTimeNum(float value)
    {
        int m = ((int)value / 60);
        int s = ((int)value % 60);

        _text.text = ($"{m.ToString("D2")}:{s.ToString("D2")}");
    }
}
