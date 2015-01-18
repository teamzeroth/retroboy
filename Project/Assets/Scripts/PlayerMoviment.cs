using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoviment : MonoBehaviour {

    Animator anim;    

    public float speed = 10f;

	void Start(){
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        Vector2 /*moveVector*/ mv = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        Vector3 deltaMv = mv * speed * Time.deltaTime;
        if (!CheckWallCollider(transform.position + deltaMv, mv)) {
            var mainAxis = Mathf.Abs(Input.GetAxis("Horizontal")) > Mathf.Abs(Input.GetAxis("Vertical"));
            var animSpeed = 0.75f * Mathf.Max(Mathf.Abs(Input.GetAxis("Horizontal")), Mathf.Abs(Input.GetAxis("Vertical")));
            
            print("moving on");

            anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"));
            anim.SetFloat("Vertical", Input.GetAxis("Vertical"));
            anim.SetBool("MainAxis", mainAxis);
            anim.speed = 0.25f + animSpeed;

            transform.position = transform.position + deltaMv;
        } else {
            anim.SetFloat("Horizontal", 0);
            anim.SetFloat("Vertical", 0);
        }

        /// Collider Test
        
        /*Vector2 v = ((Vector2) transform.position) + new Vector2(0, -0.7f);
        Collider2D[] test = Physics2D.OverlapCircleAll(v, 0.4f);

        print(test[0]);*/
    }

    Vector2[] points = new Vector2[4];
    Vector2[] /*deltaMatrix*/ dm = new Vector2[]{
        Vector2.up, -Vector2.up, -Vector2.right, Vector2.right
    };

    public bool CheckWallCollider(Vector2 position, Vector2 moveVector) {
        Collider2D[] /*colliderMatrix*/ cm = new Collider2D[4];
        Vector2 center = position + new Vector2(0, -0.7f);

        Vector2 point = center + (moveVector * 0.4f);
        Collider2D collider = Physics2D.OverlapPoint(point, LayerMask.GetMask("Floor"));

        if(collider != null)
            return false;
        return true;

        /*for (int i = 0; i < 4; i++) {
            points[i] = center + (dm[i] * 0.4f);
            cm[i] = Physics2D.OverlapPoint(points[i], LayerMask.GetMask("Floor"));
        }

        if (System.Array.IndexOf(cm, null) == -1)
            return false;
        return true;
        */

    }

    void OnDrawGizmos() {
        Vector3 /*moveVector*/ mv = new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        Gizmos.color = Color.yellow;
        Vector3 v = transform.position + new Vector3(0, -0.7f) + (mv * 0.4f);
        //Vector3 v = transform.position + new Vector3(0, -0.7f, 0);
        //Gizmos.DrawWireSphere(v, 0.4f);
        Gizmos.DrawWireSphere(v, 0.1f);
    }
}
