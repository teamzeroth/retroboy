using UnityEngine;
using System.Collections;

public class SortingTiledLayer : MonoBehaviour {

    public int Level;

    void Start() {
        float minY = float.PositiveInfinity, minX = float.PositiveInfinity, maxX = float.NegativeInfinity;

        GameObject feets = new GameObject("Feets");

        foreach (Transform tile in transform) {
            if (tile.GetComponent<Renderer>().bounds.min.y < minY) minY = tile.GetComponent<Renderer>().bounds.min.y;
            if (tile.GetComponent<Renderer>().bounds.min.x < minX) minX = tile.GetComponent<Renderer>().bounds.min.x;
            if (tile.GetComponent<Renderer>().bounds.max.x > maxX) maxX = tile.GetComponent<Renderer>().bounds.max.x;

            addSortingMoveableLayer(tile.gameObject, feets.transform);
        }

        feets.transform.parent = transform;
        feets.transform.position = new Vector3((minX + maxX) / 2, minY, 0);
    }

    void addSortingMoveableLayer(GameObject tile, Transform feets){
        SortingOrder sML = tile.AddComponent<SortingOrder>();
        SpriteRenderer tileRenderer = (SpriteRenderer)tile.GetComponent<Renderer>();

        sML.positionPoint = feets;
        sML.InitialOrder = 0;
        tileRenderer.sortingLayerName = "Game";
        tileRenderer.sortingOrder = Level;
        
    }

}
