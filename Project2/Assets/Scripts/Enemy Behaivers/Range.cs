using UnityEngine;
using System.Collections;

public class Range : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D trigger)
    {
        //print(this.gameObject.name);
        MachineDog m = this.gameObject.transform.parent.gameObject.GetComponent<MachineDog>();        
        m.onTriggerExternal(this.gameObject,trigger);
    }  
}
