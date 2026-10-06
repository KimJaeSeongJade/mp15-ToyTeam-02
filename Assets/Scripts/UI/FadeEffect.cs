using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    private static FadeEffect _instance;

    public static FadeEffect Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = _instance = FindObjectOfType<FadeEffect>();
                DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }

    [SerializeField] [Range(0.01f, 10f)] private float _fadeTime;

    [SerializeField] private Image _image;

    private void Awake()
    {
        SetSingleton();
    }

    // Fade In. 화면이 밝아짐
    public void FadeIn()
    {
        StartCoroutine(Fade(1, 0));
    }

    // Fade Out. 화면이 어두워짐
    public void FadeOut()
    {
        StartCoroutine(Fade(0, 1));
    }

    private IEnumerator Fade(float start, float end)
    {
        float currentTime = 0.0f;
        float percent = 0.0f;

        while(percent < 1)
        {
            currentTime += Time.deltaTime;
            percent = currentTime / _fadeTime;

            Color color = _image.color;
            color.a = Mathf.Lerp(start, end, percent);
            _image.color = color;

            yield return null;
        }
    }

    private void SetSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = GetComponent<FadeEffect>();
        DontDestroyOnLoad(gameObject);
    }
}
