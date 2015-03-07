using UnityEngine;
using System.Collections;
[RequireComponent(typeof(SpriteRenderer))]
public class RepeatSpriteBoundary : MonoBehaviour {
    SpriteRenderer sprite;
    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        /*if (!SpritePivotAlignment.GetSpriteAlignment(gameObject).Equals(SpriteAlignment.TopRight)) {
            Debug.LogError("You forgot change the sprite pivot to Top Right.");
        }*/
        Vector2 spriteSize = new Vector2(sprite.bounds.size.x / transform.localScale.x, sprite.bounds.size.y / transform.localScale.y);
        GameObject childPrefab = new GameObject();
        SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
        childPrefab.transform.position = transform.position;
        childSprite.sprite = sprite.sprite;
        childSprite.sortingLayerID = sprite.sortingLayerID;

        GameObject child;
        for (int i = 0, h = (int)Mathf.Round(sprite.bounds.size.y); i * spriteSize.y < h; i++) {
            for (int j = 0, w = (int)Mathf.Round(sprite.bounds.size.x); j * spriteSize.x < w; j++) {
                child = Instantiate(childPrefab) as GameObject;
                child.transform.position = transform.position - (new Vector3(spriteSize.x * j, spriteSize.y * i, 0));
                child.transform.parent = transform;
            }
        }
        Destroy(childPrefab);
        sprite.enabled = false;
    }
}