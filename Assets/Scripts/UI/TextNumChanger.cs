using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class TextNumChanger : MonoBehaviour
{
    private TextMeshProUGUI _text => GetComponent<TextMeshProUGUI>();

    //private void Update()
    //{
    //    ChangeNum();
    //}

    public void ChangeNum(float value)
    {
        _text.text = value.ToString();
    }
}
