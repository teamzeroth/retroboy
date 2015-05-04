using UnityEngine;
using System.Collections;

public class SorthingParticleLayer : MonoBehaviour {

    public string sorthingLayer;

    public void Start() {
        particleSystem.renderer.sortingLayerName = sorthingLayer;
    }

    void Update() {
        //transform.localScale = transform.parent.localScale;
    }
}
