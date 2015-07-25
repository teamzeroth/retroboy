using UnityEngine;
using System.Collections;

public class SorthingParticleLayer : MonoBehaviour {

    public string sortingLayer;

    public void Start() {
        if (GetComponent<ParticleSystem>() != null)
            GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayer;
        
        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().sortingLayerName = sortingLayer;
    }

    public void OnDrawGizmosSelected() {
        if (GetComponent<ParticleSystem>() != null)
            GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = sortingLayer;

        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().sortingLayerName = sortingLayer;
    }
}
