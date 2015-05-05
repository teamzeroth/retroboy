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
        float magic1 = 7.5625f, magic2 = 2.75f,  magic3 = 1.5f, magic4 = 2.25f, 
            magic5 = 2.625f, magic6 = 0.75f, magic7 = 0.9375f, magic8 = 0.984375f;

        if ((currentTime /= totalTime) < (1 / magic2)){ //0.36
            return changeInValue * (magic1 * currentTime * currentTime) + startValue;

        } else if (currentTime < (2 / magic2)){ //0.72
            return changeInValue * (magic1 * (currentTime -= (magic3 / magic2)) * currentTime + magic6) + startValue;

        } else if (currentTime < (2.5 / magic2)){ //0.91
            return changeInValue * (magic1 * (currentTime -= (magic4 / magic2)) * currentTime + magic7) + startValue;

        } else {
            return changeInValue * (magic1 * (currentTime -= (magic5 / magic2)) * currentTime + magic8) + startValue;
        }
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

    public static void SetParticlesVelocity(this ParticleSystem _particles, Vector3 velocity) {
        ParticleSystem.Particle[] p = new ParticleSystem.Particle[_particles.particleCount + 1];
        int l = _particles.GetParticles(p);

        for (int i = 0; i < l; i++)
            p[i].velocity = velocity * (1 + 0.3f * Random.value);

        _particles.SetParticles(p, l);
    }
}

