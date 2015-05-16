using UnityEngine;
using System.Collections;

public class EnemiesRadar : MonoBehaviour {
    
    public void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemies")) return;
		if (other.GetComponent<BaseEnemy>() == null) return;

        other.GetComponent<BaseEnemy>().OnDistanceWithPlayer(
            transform,
            Vector3.Distance(transform.parent.position, other.transform.position)
        );
    }
}
