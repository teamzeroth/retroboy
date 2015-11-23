using UnityEngine;
using System.Collections;

public class ActivateChildren : MonoBehaviour {

    public void OnEnable() {
        foreach (Transform child in transform) {
            child.gameObject.SetActive(true);
        }
    }
}
