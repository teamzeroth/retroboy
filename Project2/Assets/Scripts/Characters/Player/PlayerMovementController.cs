using UnityEngine;
using System.Collections;
using System;

public class PlayerMovementController : MonoBehaviour {

    private Animator _anim;

    public bool ForcingMove {
        get {
            return (Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical"))) > 0.2f;
        }
    }

    public bool NormalState {
        get {
            string[] prefixes = { "Nim-idle", "Nim-walk", "Nim-friction" };
            return Array.Exists(prefixes, prefix => _anim.CurrentAnimState().StartsWith(prefix));
        }
    }

    public bool ShootState {
        get {
            string[] prefixes = { "Nim-draw", "Nim-charge", "Nim-shoot" };
            return Array.Exists(prefixes, prefix => _anim.CurrentAnimState().StartsWith(prefix));
        }
    }

    public bool OnMoving { get { return NormalState && (Input.GetButton("Horizontal") || Input.GetButton("Vertical")); } }
    public bool OnDraw { get { return Input.GetButtonDown("Fire1") && !ShootState; } }
    public bool OnCharge { get { return Input.GetButton("Fire1"); } }
    public bool OnShoot { get { return Input.GetButtonUp("Fire1"); } }

    //public Vector2 DeadVector { set { deadMoveVec = value; } }

    public void Start() {
        _anim = GetComponent<Animator>();
    }

    public void Update() {
    }
}
