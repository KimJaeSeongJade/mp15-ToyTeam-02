using UnityEngine;

public class SpawnTrigger : MonoBehaviour
{
    private void Start()
    {
        MaterialBase material = GetComponent<MaterialBase>();

        if (material != null)
        {
            // TutorialManager를 찾아 BlockType 전달
            TutorialManager manager = FindObjectOfType<TutorialManager>();
            if (manager != null)
            {
                manager.OnBlockSpawned(material.BlockType);
            }
        }
    }
}