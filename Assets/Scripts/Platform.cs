﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Platform : MonoBehaviour {

  public int objectID;
  public int instanceID;

  [Header("Level")]
  public int start_lvl;
  public int main_lvl;
  public int priority = 1;


  // Use this for initialization
  public void Initialize () {
    Vector3 localScale = transform.localScale;
    localScale.x *= Random.Range(1.2f, 2.5f);
    transform.localScale = localScale;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
