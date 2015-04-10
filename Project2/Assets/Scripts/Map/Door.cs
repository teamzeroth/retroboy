﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using X_UniTMX;
using DG.Tweening;

namespace MapResources {

    public class MapUtils{
        public enum Direction { Left, Right, Up, Down };

        static public Direction TranslateDirection(string value) {
            switch (value) {
                case "NO": return MapUtils.Direction.Left;
                case "SE": return MapUtils.Direction.Right;
                case "NE": return MapUtils.Direction.Up;
                case "SO": return MapUtils.Direction.Down;
            }

            return MapUtils.Direction.Down;
        }

        static public Vector2 GetDirectionVector(Direction d){
            switch (d){
		        case Direction.Left: return new Vector2(-1, 1).normalized;
                case Direction.Right: return new Vector2(1, -1).normalized;
                case Direction.Up: return new Vector2(1, 1).normalized;
                case Direction.Down: return new Vector2(-1, -1).normalized;
            }

            return Vector2.zero;
        }
    }

    public class Door : MonoBehaviour {

        public static Dictionary<string, Door> doors = new Dictionary<string, Door>();

        public MapUtils.Direction Out;
        public MapUtils.Direction In;

        public string id;
        public string goTo;
        public bool anotherMap = false;

        private AnimationController _player = null;

        void Start(){
            if (!string.IsNullOrEmpty(id)) doors.Add(id, this);
        }

        public void Init(MapObject door) {
            if (door.HasProperty("name")){
                name = door.GetPropertyAsString("name");
            }

            if (door.HasProperty("out"))
                Out = MapUtils.TranslateDirection(door.GetPropertyAsString("out"));

            if (door.HasProperty("in"))
                In = MapUtils.TranslateDirection(door.GetPropertyAsString("in"));

            if (door.HasProperty("go to"))
                goTo = door.GetPropertyAsString("go to");

            if (door.HasProperty("another map"))
                anotherMap = door.GetPropertyAsBoolean("another map");
            else
                anotherMap = false;
        }

        public void OnDrawGizmos() {
            Collider2D coll = GetComponent<Collider2D>();
            Gizmos.DrawSphere(coll.bounds.center, 0.05f);
        }

        public void OnTriggerEnter2D(Collider2D other) {
            if (other.tag == "Player" && _player == null) {
                _player = other.gameObject.GetComponent<AnimationController>();
                GetIn();
            }
        }

        public void AfterEnter() {
            doors[this.goTo].GetOut(_player);
            _player = null;
        }

        public void AfterOut() {
            _player = null;
        }


        public void GetIn() {
            _player.MakeFixedMove(CalcInDirection(), 1, new Color(0, 0, 0, 0.3f));
            Invoke("AfterEnter", 1.1f);
        }

        public void GetOut(AnimationController player) {
            _player = player;
            _player.MakeFixedMove(MapUtils.GetDirectionVector(Out), 1, new Color(1, 1, 1, 1));
            _player.DeadVector = MapUtils.GetDirectionVector(Out);

            Vector3 colliderCenter = (Vector3) (_player.collider2D as CircleCollider2D).center;
            _player.transform.position = GetComponent<Collider2D>().bounds.center - colliderCenter;

            Invoke("AfterOut", 1.1f);
        }

        public Vector2 CalcInDirection(){
            Vector2 boundCenter = (Vector2) GetComponent<Collider2D>().bounds.center;
            Vector2 playerPosition = (Vector2)_player.collider2D.bounds.center;

            return (boundCenter - playerPosition).normalized / 2;
	    }

    }

}
