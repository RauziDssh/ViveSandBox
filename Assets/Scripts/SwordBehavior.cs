using UnityEngine;
using System.Collections;
using System;

public class SwordBehavior : MonoBehaviour,IGrippable {

    public bool Grabbed { get; set; }

    public GameObject grip;
    public Transform GripPos { get; set; }

    public Quaternion rotationOffset { get; set; }

    public void Grip()
    {
        Grabbed = true;
    }

    public void UnGrip()
    {
        Grabbed = false;
    }

    // Use this for initialization
    void Start () {
        GripPos = grip.transform;
        rotationOffset = Quaternion.Euler(90.0f, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
