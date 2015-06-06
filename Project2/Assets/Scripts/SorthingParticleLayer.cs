using UnityEngine;
using System.Collections;

public class SorthingParticleLayer : MonoBehaviour {

    public string sortingLayer;

    public void Start() {
        if (particleSystem != null)
            particleSystem.renderer.sortingLayerName = sortingLayer;
        
        if (renderer != null)
            renderer.sortingLayerName = sortingLayer;
    }

    public void OnDrawGizmosSelected() {
        if (particleSystem != null)
            particleSystem.renderer.sortingLayerName = sortingLayer;

        if (renderer != null)
            renderer.sortingLayerName = sortingLayer;
    }
}
