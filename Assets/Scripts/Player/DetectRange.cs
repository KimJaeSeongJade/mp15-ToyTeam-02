using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어의 감지 범위
/// </summary>
public class DetectRange : MonoBehaviour
{
    [SerializeField] private SphereCollider _sphereCollider;

    private List<IInteractable> _detecteds;

    /// <summary>
    /// 영역 안으로 들어온 오브젝트를 저장
    /// </summary>
    public List<IInteractable> Detecteds => _detecteds;

    /// <summary>
    /// 플레이어 감지 범위의 반경
    /// </summary>
    public float Range => _sphereCollider.radius;

    // ------------------------------
    private void Awake() => CacheComponents();
    private void Start() => Init();
    // ------------------------------

    private void OnTriggerEnter(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if(interactable != null)
        {
            _detecteds.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            _detecteds.Remove(interactable);
        }
    }

    private void CacheComponents()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    private void Init()
    {
        _detecteds = new();
    }
}
