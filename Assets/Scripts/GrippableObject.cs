using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;

public class GrippableObject : MonoBehaviour,IGrippable {

    public Transform GripPos;
    bool _Grabbed;

    public bool Grabbed
    {
        get
        {
            return _Grabbed;
        }

        set
        {
            _Grabbed = value;
        }
    }

    Transform IGrippable.GripPos { get; set; }

    public Quaternion rotationOffset { get; set; }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Grip()
    {
        Grabbed = true;
    }

    public void UnGrip()
    {
        Grabbed = false;
    }
}
