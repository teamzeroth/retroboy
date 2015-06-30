using UnityEngine;
using System.Collections;

public class CollisionListener : MonoBehaviour {
    
    public int Layer;

    private GameObject father;
    public GameObject Father{
        get{return father;}
        set{
            father = value;
            enabled = true;
        }
    }
    
    void Start(){
        if(transform.parent != null)
            Father = transform.parent.gameObject;
        else
            enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == Layer)
            Father.BroadcastMessage("OnCollisionListener", other, SendMessageOptions.DontRequireReceiver);
    }

    public void OnCollisionEnter2D(Collision2D coll) {
        if (coll.gameObject.layer == Layer)
            Father.BroadcastMessage("OnCollisionListener", coll, SendMessageOptions.DontRequireReceiver);
    }
}
