using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class Helper {

    /// <summary>
    /// Return the easing Circular starting with b for d seconds
    /// </summary>
    /// <param name="t">Current in time</param>
    /// <param name="b">Start value</param>
    /// <param name="c">Change in value</param>
    /// <param name="d">Duration</param>

    public static float EaseCirc(float t, float b, float c, float d) {
        t /= d;
        return -c * (Mathf.Sqrt(1 - t * t) - 1) + 0;
    }

    /// <summary>
    /// Return the current animation state name
    /// </summary>
    /// <param name="anim">this paramether of the Animation</param>

    public static string CurrentAnimState(this Animator _anim){
        try {
            return _anim.GetCurrentAnimationClipState(0)[0].clip.name;
        } catch (System.Exception) {
            return "";
        }
    }

    /// <summary>
    /// Return the current animation state name
    /// </summary>
    /// <param name="_particles">this paramether of the ParticleSystem</param>
    /// <param name="velocity">the new Velocity that the particles will be</param>

    private static void SetParticlesVelocity(this ParticleSystem _particles, Vector3 velocity) {
        ParticleSystem.Particle[] p = new ParticleSystem.Particle[_particles.particleCount + 1];
        int l = _particles.GetParticles(p);

        for (int i = 0; i < l; i++)
            p[i].velocity = velocity * (1 + 0.3f * Random.value);

        _particles.SetParticles(p, l);
    }
}

