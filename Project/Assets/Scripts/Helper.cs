using UnityEngine;
using System;
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
}

