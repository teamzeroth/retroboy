﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ControlMap : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        foreach (Transform g in transform)
            Destroy(g.gameObject);
        Destroy(gameObject);
    }	
}