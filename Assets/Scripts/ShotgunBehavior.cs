using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using System.Linq;
using System;

public class ShotgunBehavior : GunBehavior {

    float firingRate = 10.0f;
    float BulletSpeed = 5000.0f;
    int bulletNum = 12;
    public GameObject bullet;
    Subject<float> TriggerStream;

    public override void Shot(float pow)
    {
        TriggerStream.OnNext(pow);
    }

    public override void Reload()
    {

    }

    // Use this for initialization
    void Start () {

        var random = new System.Random();

        TriggerStream = new Subject<float>();
        TriggerStream
            .Where(v => Grabbed)
            .SkipWhile(v => v >= 0.6f)
            .Where(v => v > 0.6f)
            .Take(1)
            .RepeatUntilDestroy(this.gameObject)
            .Subscribe(v =>
            {
                var device = this.transform.parent.gameObject.GetComponent<ControllerBehavior>().device;
                FeedBack(device, 3999, 500.0f);

                var randoms =
                    Enumerable.Range(0, bulletNum)
                    .Select(s => new Vector3(random.Next(-50, 50)/100.0f, random.Next(-50, 50)/100.0f, 1.0f).normalized);

                foreach (var e in randoms)
                {
                    var go_bullet = Instantiate(bullet, this.transform.position + Quaternion.FromToRotation(e,Vector3.forward) * this.transform.forward, Quaternion.identity) as GameObject;
                    //go_bullet.transform.localRotation *= Quaternion.Euler(new Vector3(0,0,90.0f));
                    go_bullet.GetComponent<Rigidbody>().AddForce(BulletSpeed * (go_bullet.transform.position - this.transform.position).normalized, ForceMode.Acceleration);
                    Destroy(go_bullet, 5);
                }
            }
            );
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FeedBack(SteamVR_Controller.Device device, int power, float duration)
    {
        this.UpdateAsObservable()
            .TakeUntil(Observable.Timer(TimeSpan.FromMilliseconds(duration)))
            .Subscribe(v =>
            {
                device.TriggerHapticPulse((ushort)power);
            }
            );
    }
}
