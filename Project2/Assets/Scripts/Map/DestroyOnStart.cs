using UnityEngine;
using System.Collections;

public class DestroyOnStart : MonoBehaviour {

    public void Start() {
        Destroy(gameObject);
    }
}
