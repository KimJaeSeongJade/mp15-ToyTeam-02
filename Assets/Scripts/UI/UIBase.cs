using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI기본기능 추상 클래스
/// </summary>
public abstract class UIBase : MonoBehaviour
{
    // 활성화시 UI이미지
    private Image _onImage;
    // 비활성화시 UI이미지
    private Image _offImage;

    private bool _isAllExist;

    protected virtual void Awake()
    {
        Init();
    }

    private void Init()
    {
        Image[] images = GetComponentsInChildren<Image>();

        _isAllExist = false;

        if (images.Length > 1)
        {
            _onImage = images[0];
            _offImage = images[1];

            _isAllExist = true;

            _onImage.gameObject.SetActive(false);
            _offImage.gameObject.SetActive(true);
        }
        else if (images.Length == 1)
        {
            _offImage = images[0];

            _offImage.gameObject.SetActive(true);
        }
    }
    // UI에 Enter시 OnImage활성화
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6 && _isAllExist)
        {
            _onImage.gameObject.SetActive(true);
            _offImage.gameObject.SetActive(false);
        }
    }

    // UI에 Exit시 OnImage비활성화
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            _offImage.gameObject.SetActive(true);

            if (_isAllExist)
            {
                _onImage.gameObject.SetActive(false);
            }
        }
    }

    protected virtual void OnDisable()
    {
        if (_offImage != null)
        {
            _offImage.gameObject.SetActive(true);
        }

        if (_isAllExist && _onImage != null)
        {
            _onImage.gameObject.SetActive(false);
        }
    }

    // 각 UI별 기능 작동
    protected abstract void PlayUI();

    private void OnUI()
    {
        gameObject.SetActive(true);
    }
    private void OffUI()
    {
        gameObject.SetActive(false);
    }
}