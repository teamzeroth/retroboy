using UnityEngine;
using System;

class PlayerInput {

    public Vector2 deltaDirection = new Vector2(1, 0);
    public Vector2 deadDirection = new Vector2(1, 0);

    public Vector2 Direction;

    public Vector2 LockDirection;

    public bool InMoving;
    public bool InDashing;
    public bool InCharging;
    public bool InShooting;
	public bool InClimbing;

    public float TimeInCharge;
    public float LastTimeInCharge;

    float lastTimeWithMouse;

    /// <summary>
    /// Update all the variables of this class according the Game Input and Player States
    /// </summary>
    /// <param name="player">The player</param>
    /// <returns>A delta direction of the player</returns>
    public Vector3 Update(Player player) {

		if (!player.onCutscene) {

			if(!player.climbing){
				setupShoot (player);
			}

			setupDirection (player);

			if(!player.climbing){
				setupDash (player);
			}
		}

        return deltaDirection;
    }

    /// <summary>
    /// Define the variables that control the diraction of the player
    /// </summary>
    /// <param name="player">The player</param>
    public void setupDirection(Player player) {
        if (Input.GetKey(KeyCode.Mouse0)) {
            lastTimeWithMouse = Time.time;

            /// GET THE DIRECTION WITH THE MOUSE
            Camera camera = Camera.main;
            Vector3 pos = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camera.nearClipPlane));

            deltaDirection = pos - player.transform.position;
            deltaDirection.Normalize();

            deadDirection = deltaDirection;

        } else if (lastTimeWithMouse < Time.time - .3f) { // Check if passed 0.2 seconds after the last mouse input

            /// GET THE DIRECTION WITH THE AXIS
            deltaDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (deltaDirection.magnitude > 1)
                deltaDirection.Normalize();

            if (deltaDirection.magnitude > 0.2f)
                deadDirection = deltaDirection;
            else
                deltaDirection = Vector2.zero;

        } else {
            deltaDirection = Vector2.zero;
        }

        Direction = LockDirection != Vector2.zero ? LockDirection : deadDirection;

        InMoving = deltaDirection != Vector2.zero;
    }

    /// <summary>
    /// Define the variables to control the shoots stats
    /// </summary>
    /// <param name="player">The player</param>
    public void setupShoot(Player player) {
        InShooting = false;
        InCharging = false;

        InCharging = Input.GetButton("Action");
        InShooting = Input.GetButtonUp("Action");

        if (InCharging) LastTimeInCharge = TimeInCharge;
        TimeInCharge = InCharging ? TimeInCharge + Time.deltaTime : 0;
    }

    /// <summary>
    /// Check if the player press the dash button
    /// </summary>
    /// <param name="player"></param>
    public void setupDash(Player player) {
        InDashing = InDashing || Input.GetButtonDown("Dash");
    }
}
