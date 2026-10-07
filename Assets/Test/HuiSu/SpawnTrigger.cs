using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    private MaterialBase _material;

    // ------------------------------
    private void Awake() => CacheComponent();
    private void OnEnable()
    {
        Spawned().Forget();
    }
    // ------------------------------

    private async UniTaskVoid Spawned()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.3f));
        OnBlockSpawned();
    }

    private void OnBlockSpawned()
    {
        TutorialManager manager = FindObjectOfType<TutorialManager>();
        manager.OnBlockSpawned(_material.BlockType);
    }

    private void CacheComponent()
    {
        _material = GetComponent<MaterialBase>();
    }
}