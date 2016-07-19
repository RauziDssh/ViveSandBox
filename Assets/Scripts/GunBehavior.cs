using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class GunBehavior : MonoBehaviour,IShootable,IGrippable {

    //銃系オブジェクトの基底クラス

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

    Transform _GripPos;
    public Transform GripPos
    {
        get
        {
            return _GripPos;
        }

        set
        {
            _GripPos = value;
        }
    }

    public Quaternion rotationOffset { get; set; }

    public void Grip()
    {
        rotationOffset = Quaternion.Euler(50.0f, 0.0f, 0.0f);
        GripPos = GripObj.transform;
        Grabbed = true;
    }

    public virtual void Reload()
    {
        Debug.Log("Reloading!");
    }

    public virtual void Shot(float pow)
    {
        Debug.Log("Shot! at" + pow);
    }

    public void UnGrip()
    {
        Grabbed = false;
    }

    public GameObject GripObj;

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
	}
}
