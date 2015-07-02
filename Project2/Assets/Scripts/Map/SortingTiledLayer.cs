using UnityEngine;
using System.Collections;

public class SortingTiledLayer : MonoBehaviour {

    void Start() {
        float minY = float.PositiveInfinity, minX = float.PositiveInfinity, maxX = float.NegativeInfinity;

        GameObject feets = new GameObject("Feets");

        foreach (Transform tile in transform) {
            if (tile.renderer.bounds.min.y < minY) minY = tile.renderer.bounds.min.y;
            if (tile.renderer.bounds.min.x < minX) minX = tile.renderer.bounds.min.x;
            if (tile.renderer.bounds.max.x > maxX) maxX = tile.renderer.bounds.max.x;

            addSortingMoveableLayer(tile.gameObject, feets.transform);
        }

        feets.transform.parent = transform;
        feets.transform.position = new Vector3((minX + maxX) / 2, minY, 0);
    }

    void addSortingMoveableLayer(GameObject tile, Transform feets){
        SortingMoveableLayer sML = tile.AddComponent<SortingMoveableLayer>();
        SpriteRenderer tileRenderer = (SpriteRenderer)tile.renderer;

        sML.positionPoint = feets;
        sML.InitialOrder = 0;
        tileRenderer.sortingLayerName = "Game";
        
    }

}
