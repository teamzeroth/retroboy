using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class NPCController : MonoBehaviour {

    public static List<NPCController> interactions = new List<NPCController>();
    
    public Vector2 defaltDirection;
    public bool flipAnimation = true;

    Collider2D _interactionArea;
    Animator _animation;

    bool flipped = false;

    #region MonoBehavior

    void Awake() {
        _interactionArea = Array.Find<Collider2D>(GetComponents<Collider2D>(), x => x.isTrigger);
        _animation = GetComponent<Animator>();

        changeAnimation(defaltDirection);
    }

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") interactions.Add(this);
    }

    public void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player") interactions.Remove(this);
    }


    public void OnDrawGizmos() {        
        if (_interactionArea == null) _interactionArea = Array.Find<Collider2D>(GetComponents<Collider2D>(), x => x.isTrigger);
        CircleCollider2D coll = _interactionArea as CircleCollider2D;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(
            transform.position + (Vector3) coll.center,
            coll.radius
        );

        Gizmos.DrawSphere(
            transform.position + (Vector3) (coll.center + defaltDirection),
            0.03f
        );
    }

    #endregion

    #region Messages

    public void Interact(Vector2 playerPos) {
        Vector2 direction = playerPos - (Vector2) transform.position;
        Fungus.FungusScript fungus = GameObject.FindWithTag("Fungus").GetComponent<Fungus.FungusScript>();

        print(name + "Say");

        fungus.SendFungusMessage(name + "Say");
        changeAnimation(direction);
    }

    public void Flip() {
        flipped = !flipped;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    #endregion

    private void changeAnimation(Vector2 vector) {
        Vector2 direction = vector.normalized;

        _animation.SetFloat("Horizontal", direction.x);
        _animation.SetFloat("Vertical", direction.y);

        if (flipAnimation) {

            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y) * 0.42f) {
                
                if (!flipped && direction.x < 0) Flip();
                if (flipped && direction.x > 0) Flip();
            
            } else {
                
                if (flipped) Flip();
            }
        }
    }
}
