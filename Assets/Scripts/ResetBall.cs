using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class ResetBall : MonoBehaviour {

    public GameObject obj;

	// Use this for initialization
	void Start () {
        GameObject controller = this.transform.parent.gameObject;
        SteamVR_TrackedObject trackedObject = controller.GetComponent<SteamVR_TrackedObject>();
        var device = SteamVR_Controller.Input((int)trackedObject.index);

        this.UpdateAsObservable()
            .Where(v => device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            .Subscribe(v => 
                {
                    Instantiate(obj,new Vector3(0,1,0),Quaternion.identity);
                }
            );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
