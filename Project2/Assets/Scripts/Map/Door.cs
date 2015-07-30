using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using X_UniTMX;
using DG.Tweening;

namespace MapResources {

    public class Door : MonoBehaviour {

        public static Dictionary<string, Door> doors = new Dictionary<string, Door>();

        public Direction Out;
        public Direction In;
        public float ForceOut;
        public float ForceIn;
        public string GoTo;
        public string Scene;

        CollisionLevel collisionLevel;

        private Player _player = null;

        void Awake() {
            if (!string.IsNullOrEmpty(name)) doors.Add(name, this);
        }

        void Start() {
            collisionLevel = GetComponent<CollisionLevel>();
        }

        public void Init(MapObject door) {
            if (door.HasProperty("name")) name = door.GetPropertyAsString("name");

            Out = Helper.TranslateDirection(door.GetPropertyAsString("out"));
            In = Helper.TranslateDirection(door.GetPropertyAsString("in"));

            ForceOut = door.HasProperty("force out") ? door.GetPropertyAsFloat("force out") : 1;
            ForceIn = door.HasProperty("force in") ? door.GetPropertyAsFloat("force in") : 1;

            GoTo = door.GetPropertyAsString("go to");
            Scene = door.GetPropertyAsString("scene");
        }

        public void OnDrawGizmos() {
            Collider2D coll = GetComponent<Collider2D>();
            Gizmos.DrawSphere(coll.bounds.center, 0.05f);
        }

        public void OnTriggerEnter2D(Collider2D other) {
			if (other.tag == "NimFeet" && _player == null && !string.IsNullOrEmpty(GoTo)) {
				GetIn();
				Debug.Log("Entered");
            }
        }

        public void AfterEnter() {
            if (!string.IsNullOrEmpty(GoTo))
                doors[GoTo].GetOut(_player);

            _player = null;
        }

        public void AfterOut() {
            //if (string.IsNullOrEmpty(goTo)) collider2D.isTrigger = false;
            _player.GetComponent<SpriteRenderer>().sortingOrder = collisionLevel.Level;
            _player = null;
        }

		public void GetIn() {
            Camera.main.GetComponent<SmoothFollow>().target = transform;

			_player = GameController.self.player;
			//_player.DisableColliders();

            var direction = CalcInDirection("in");

            _player.StartFixedMove(direction, direction, Game.DOOR_ANIMATION_TIME, new Color(0, 0, 0, 0.0f));

            Invoke("AfterEnter", Game.DOOR_ANIMATION_TIME + 0.1f);
        }

        public void GetOut(Player player) {
            player.collisionLevel.Level = collisionLevel.Level;
            Camera.main.GetComponent<SmoothFollow>().target = player.transform;

			_player = GameController.self.player;
            var direction = CalcInDirection("out");

            _player.StartFixedMove(direction, direction, Game.DOOR_ANIMATION_TIME, new Color(1, 1, 1, 1));
            _player.DeadDirection = direction;

            Vector3 colliderCenter = (Vector3)(_player.FeetCollider as CircleCollider2D).offset;
            _player.transform.position = GetComponent<Collider2D>().bounds.center - colliderCenter;

            Invoke("AfterOut", Game.DOOR_ANIMATION_TIME + 0.1f);
        }

        public Vector2 CalcInDirection(string state) {
            Direction /*direction*/ d = Direction.CC;
            float /*force*/ f = 1;

            if (state.CompareTo("in") == 0) {
                d = In;
                f = ForceIn;
            } else if (state.CompareTo("out") == 0) {
                d = Out;
                f = ForceOut;
            }

            if (d == Direction.CC) {
                Vector2 boundCenter = (Vector2)GetComponent<Collider2D>().bounds.center;
                Vector2 playerPosition = (Vector2)_player.GetComponent<Collider2D>().bounds.center; //Mudar para player feet collider

                return (boundCenter - playerPosition).normalized / 2;
            } else {
                return Helper.GetDirectionVector(d) * f;
            }
        }
    }
}
