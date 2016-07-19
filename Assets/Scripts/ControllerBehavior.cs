using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using Valve.VR;

//つかむことのできるオブジェクトにつけるインターフェース
//トリガーを引くと手から離れる
public interface IGrabbable
{
    bool Grabbed { get; set; }
    void Grab();
    void UnGrab();
}
//道具として握ることのできるオブジェクトにつけるインターフェース
//トリガーを放しても手から離れない
public interface IGrippable
{
    Transform GripPos { get; set; }
    Quaternion rotationOffset { get; set; }
    bool Grabbed { get; set; }
    void Grip();
    void UnGrip();
}
//トリガーを引いてアクションを起こすオブジェクトにつけるインターフェース
public interface IShootable
{
    void Shot(float pow);
    void Reload();
}

public class ControllerBehavior : MonoBehaviour {

    public GameObject grabbingObject;
    Vector3 oldPosition;
    Vector3 speed;

    Vector3 oldRotation;
    Quaternion angularVelocity;

    public SteamVR_Controller.Device device;

	// Use this for initialization
	void Start () {
        GameObject controller = this.transform.parent.gameObject;
        SteamVR_TrackedObject trackedObject = controller.GetComponent<SteamVR_TrackedObject>();
        device = SteamVR_Controller.Input((int)trackedObject.index);

        this.UpdateAsObservable()
            .Subscribe(v => {
                var currentPosition = this.transform.position;
                speed = (currentPosition - oldPosition) / Time.deltaTime;
                oldPosition = currentPosition;

                var currenRotaion = this.transform.forward;
                angularVelocity = Quaternion.FromToRotation(oldRotation,currenRotaion);
                oldRotation = currenRotaion;
            });

        /*
        this.OnCollisionEnterAsObservable()
            .Where(v => speed.magnitude > 0.5f)
            .Subscribe(v => 
                {
                    v.gameObject.GetComponent<Rigidbody>().AddForce(speed / 0.5f, ForceMode.Impulse);
                }
            );*/

        //つかむ動作をしたとき
        this.OnTriggerStayAsObservable()
            .Where(v => v.GetComponent<GrabbableObject>() != null)
            .Where(v => grabbingObject == null)
            .Where(v => device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            .Select(v => v.gameObject)
            .Where(v => v.GetComponent<IGrabbable>().Grabbed == false)
            .Subscribe(v => GrabAction(v));

        //手を放したとき
        this.UpdateAsObservable()
            .Where(_ => device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            .Where(_ => grabbingObject != null)
            .Where(_ => grabbingObject.GetComponent<IGrabbable>() != null)
            .Subscribe(_ => UnGrabAction(grabbingObject,true));

        //道具をつかんだとき
        this.OnTriggerStayAsObservable()
            .Where(v => v.GetComponent<IGrippable>() != null)
            .Where(v => grabbingObject == null)
            .Where(v => device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            .Select(v => v.gameObject)
            .Where(v => v.GetComponent<IGrippable>().Grabbed == false)
            .Subscribe(v => GripAction(v));

        //道具を放したとき
        this.UpdateAsObservable()
            .Where(_ => device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            .Where(_ => grabbingObject != null)
            .Where(_ => grabbingObject.GetComponent<IGrippable>() != null)
            .Select(_ => grabbingObject)
            .Subscribe(v => UnGripAction(v,true));

        //道具を切り替えたとき
        this.OnTriggerStayAsObservable()
            .Where(v => v.GetComponent<IGrippable>() != null)
            .Where(v => grabbingObject == null)
            .Where(v => device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            .Select(v => v.gameObject)
            .Where(v => v.GetComponent<IGrippable>().Grabbed == true)
            .Subscribe(v => SwichGripAction(v,v.transform.parent.gameObject));

        //道具のトリガーを引いたとき
        this.UpdateAsObservable()
            .Where(_ => grabbingObject != null)
            .Where(_ => grabbingObject.GetComponent<IShootable>() != null)
            .Where(_ => Mathf.Abs(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x) > 0.0f)
            .Select(_ => grabbingObject)
            .Subscribe(v => 
                {
                    grabbingObject.GetComponent<IShootable>().Shot(device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x);
                }
            );
    }

    public void GrabAction(GameObject v)
    {
        grabbingObject = v;
        //v.GetComponent<GrabbableObject>().grabbed = true;
        v.GetComponent<Rigidbody>().useGravity = false;
        v.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        v.transform.SetParent(this.gameObject.transform);
        v.GetComponent<IGrabbable>().Grab();
    }

    public void UnGrabAction(GameObject v,bool addForce)
    {
        //grabbingObject.GetComponent<GrabbableObject>().grabbed = false;
        v.GetComponent<Rigidbody>().useGravity = true;
        v.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (addForce)
        {
            //投げるときに力を加える
            v.GetComponent<Rigidbody>().AddForce(speed, ForceMode.Impulse);
        }

        v.transform.parent = null;
        v.GetComponent<IGrabbable>().UnGrab();
        v = null;
        grabbingObject = null;
    }

    public void GripAction(GameObject v)
    {
        var model = this.gameObject.transform.parent.transform.FindChild("Model");
        grabbingObject = v;
        v.GetComponent<Rigidbody>().useGravity = false;
        v.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        v.transform.SetParent(this.gameObject.transform);
        v.GetComponent<IGrippable>().Grip();
        v.transform.localRotation = v.GetComponent<IGrippable>().rotationOffset;
        v.transform.localPosition = Vector3.zero;
        var grip = v.GetComponent<IGrippable>().GripPos;
        var delta = v.transform.position - grip.position;
        v.transform.position = this.transform.position + delta;
    }

    public void UnGripAction(GameObject v,bool addForce)
    {
        //grabbingObject.GetComponent<GrabbableObject>().grabbed = false;
        v.GetComponent<Rigidbody>().useGravity = true;
        v.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

        if (addForce)
        {
            //投げるときに力を加える
            v.GetComponent<Rigidbody>().AddForce(speed, ForceMode.Impulse);
            v.GetComponent<Rigidbody>().AddTorque(angularVelocity * speed.normalized,ForceMode.Force);
        }

        v.GetComponent<IGrippable>().UnGrip();
        v.transform.parent = null;
        v = null;
        grabbingObject = null;
    }

    public void SwichGripAction(GameObject v,GameObject otherHand)
    {
        otherHand.GetComponent<ControllerBehavior>().UnGripAction(v,false);
        otherHand.GetComponent<ControllerBehavior>().grabbingObject = null;//なんか危険な気もする
        this.GripAction(v);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
