using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChestController : MonoBehaviour
{
    GameSceneDirector sceneDirector;

    // èâä˙âª
    public void Init(GameSceneDirector sceneDirector)
    {
        this.sceneDirector = sceneDirector;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ÉvÉåÉCÉÑÅ[Ç∂Ç·Ç»Ç¢
        if (!collision.gameObject.TryGetComponent<PlayerController>(out var player)) return;

        sceneDirector.DispPanelTreasureChest();
        Destroy(gameObject);
    }
}
