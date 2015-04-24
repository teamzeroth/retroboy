using UnityEngine;
using System.Collections;

using DG.Tweening;

public class GrowAndFade : MonoBehaviour {

    public Ease easeType;
    public float time;

    private Tweener currentScale;
    private Tweener currentColor;

    public void OnEnable() {
        SpriteRenderer r = (SpriteRenderer) renderer;

        Color c = r.color;
        r.color = new Color(c.r, c.g, c.b, 1);
        c.a = 0;

        currentScale = transform.DOScale(Vector3.zero, time).From().SetEase(easeType).OnComplete(disabling);
        currentColor = r.DOColor(c, time).SetEase(easeType);
    }

    private void disabling() {
        gameObject.SetActive(false);
    }
}
