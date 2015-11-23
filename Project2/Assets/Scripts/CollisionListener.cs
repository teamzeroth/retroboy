using UnityEngine;
using System.Collections;

public class CollisionListener : MonoBehaviour {

    public GameObject father;
    public string layer;

    void Start() {
        if (father == null) enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (string.IsNullOrEmpty(layer) || other.gameObject.layer == LayerMask.NameToLayer(layer))
            father.SendMessage("OnTriggerListener", other, SendMessageOptions.DontRequireReceiver);
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        if (string.IsNullOrEmpty(layer) || coll.gameObject.layer == LayerMask.NameToLayer(layer))
            father.SendMessage("OnCollisionListener", coll, SendMessageOptions.DontRequireReceiver);
    }
}
