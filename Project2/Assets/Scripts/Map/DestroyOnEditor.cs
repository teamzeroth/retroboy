using UnityEngine;
using System.Collections;

public class DestroyOnEditor : MonoBehaviour {

    public void OnDrawGizmos() {
        Destroy(gameObject);
    }
}
