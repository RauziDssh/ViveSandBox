using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System;

public class GrabbableObject : MonoBehaviour,IGrabbable {

    private bool _Grabbed;
    public bool Grabbed
    {
        get
        {
            return _Grabbed;
        }

        set
        {
            _Grabbed = true;
        }
    }

    public void Grab()
    {
        Grabbed = true;
    }

    public void UnGrab()
    {
        Grabbed = false;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
