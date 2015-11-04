using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum Direction { E = 0, NE = 1, N = 2, NW = 3, W = 4, SW = 5, S = 6, SE = 7, CC = 8 };

public static class Helper {

    /// <summary>
    /// Return the easing Circular starting with b for d seconds
    /// </summary>
    /// <param name="t">Current in time</param>
    /// <param name="b">Start value</param>
    /// <param name="c">Change in value</param>
    /// <param name="d">Duration</param>

    public static float EaseCirc(float currentTime, float startValue, float changeInValue, float totalTime) {
        currentTime /= totalTime;
        return -changeInValue * (Mathf.Sqrt(1 - currentTime * currentTime) - 1) + 0;
    }

    /// <summary>
    /// Return the easing Out Bounce starting with b for d seconds
    /// </summary>
    /// <param name="currentTime">Current in time</param>
    /// <param name="startValue">Start value</param>
    /// <param name="changeInValue">Change in value</param>
    /// <param name="totalTime">Duration</param>

    public static float EaseOutBounce(float currentTime, float startValue, float changeInValue, float totalTime) {
        float magic1 = 7.5625f, magic2 = 2.75f, magic3 = 1.5f, magic4 = 2.25f,
            magic5 = 2.625f, magic6 = 0.75f, magic7 = 0.9375f, magic8 = 0.984375f;

        if ((currentTime /= totalTime) < (1 / magic2)) { //0.36
            return changeInValue * (magic1 * currentTime * currentTime) + startValue;

        } else if (currentTime < (2 / magic2)) { //0.72
            return changeInValue * (magic1 * (currentTime -= (magic3 / magic2)) * currentTime + magic6) + startValue;

        } else if (currentTime < (2.5 / magic2)) { //0.91
            return changeInValue * (magic1 * (currentTime -= (magic4 / magic2)) * currentTime + magic7) + startValue;

        } else {
            return changeInValue * (magic1 * (currentTime -= (magic5 / magic2)) * currentTime + magic8) + startValue;
        }
    }

    /// <summary>
    /// Return the current animation state name
    /// </summary>
    /// <param name="anim">this paramether of the Animation</param>

    public static string CurrentAnimState(this Animator _anim) {
        try {
            return _anim.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        } catch (System.Exception) {
            return "";
        }
    }

    /// <summary>
    /// Return the current animation state name
    /// </summary>
    /// <param name="_particles">this paramether of the ParticleSystem</param>
    /// <param name="velocity">the new Velocity that the particles will be</param>

    public static void SetParticlesVelocity(this ParticleSystem _particles, Vector3 velocity) {
        ParticleSystem.Particle[] p = new ParticleSystem.Particle[_particles.particleCount + 1];
        int l = _particles.GetParticles(p);

        for (int i = 0; i < l; i++)
            p[i].velocity = velocity * (1 + 0.3f * UnityEngine.Random.value);

        _particles.SetParticles(p, l);
    }

    /// <sumary>
    /// Flip the Transform using the scale
    /// </sumary>
    /// <param name="_transform">this paramether of the Transform Object</param>
    /// <param name="flipped">the current flipped state of the element</param>

    public static void Flip(this Transform _transform, ref bool flipped) {
        flipped = !flipped;

        Vector3 localScale = _transform.localScale;
        localScale.x *= -1;
        _transform.localScale = localScale;
    }

    /// <sumary>
    /// Return a component of object, if this component don't exist so, create it
    /// </sumary>

    public static T TryAddComponent<T>(this GameObject _object) where T : Component {
        T component = _object.GetComponent<T>();

        if (component == null)
            return _object.AddComponent<T>();
        return component;
    }

    /// <sumary>
    /// Overwrite a vector3 with a Vecto2, ignoring the Z cood
    /// </sumary>
    /// <param name="cur">The current value</param>
    /// <param name="over">The overwrite value</param>

    public static Vector3 IgnoreZ(Vector3 cur, Vector2 over) {
        return new Vector3(over.x, over.y, cur.z);
    }

    /// <sumary>
    /// Return the current Direction of a vector in geographic directions
    /// </sumary>
    /// <param name="vector">The given vector</param>
    /// <param name="fliped">If the directions will be return fliped</param>

    public static int getGeoDirection(Vector2 vector, bool fliped = true) {
        Vector2 normal = vector.normalized;

        float radAngle = Mathf.Atan2(vector.y, vector.x);
        float angle = Mathf.Rad2Deg * radAngle;

        int index = Mathf.FloorToInt(((angle + 360 + 22.5f) % 360) / 45);
        Direction dir = (Direction)index;

        if (fliped) {
            if (dir == Direction.NW) dir = Direction.NE;
            else if (dir == Direction.SW) dir = Direction.SE;
            else if (dir == Direction.W) dir = Direction.E;
        }

        return (int)dir;
    }

    public static Direction TranslateDirection(string value) {
        return (Direction)Enum.Parse(typeof(Direction), value, true);
    }

    static public Vector2 GetDirectionVector(Direction d) {
        switch (d) {
            case Direction.E: return new Vector2(1, 0).normalized;
            case Direction.NE: return new Vector2(1, 1).normalized;
            case Direction.N: return new Vector2(0, 1).normalized;
            case Direction.NW: return new Vector2(-1, 1).normalized;
            case Direction.W: return new Vector2(-1, 0).normalized;
            case Direction.SW: return new Vector2(-1, -1).normalized;
            case Direction.S: return new Vector2(0, -1).normalized;
            case Direction.SE: return new Vector2(1, -1).normalized;
        }

        return Vector2.zero;
    }
}

