using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raaCamera : MonoBehaviour {

    public float sensitivityX = 8F;
    public float sensitivityY = 8F;
    float mHdg = 0F;
    float mPitch = 0F;

//    bool mouse0 = false;
//    bool mouse1 = false;
    bool mouse2 = false;

    Vector3 mouseStart;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!mouse2 && Input.GetMouseButton(2))
        {
            mouse2 = true;
            mouseStart = Input.mousePosition;
        }

        if (mouse2 && !Input.GetMouseButton(2)) mouse2 = false;

        if (mouse2)
        {
            Vector3 m = Input.mousePosition;

            float dispX = m[0] - mouseStart[0];
            float dispY = m[1] - mouseStart[1];

            Vector3 v = new Vector3(dispY, dispX, 0.0f);

            float l = v.magnitude;
            v.Normalize();

            //			transform.RotateAround(Vector3.zero, v, l);
            transform.RotateAround(Vector3.zero, Vector3.up, dispX * 0.1f);
            //            transform.RotateAround(Vector3.zero, Vector3.right, dispY * 0.3f);

        }

        float d = Input.GetAxis("Mouse ScrollWheel");

        if (d > 0f)
        {
            MoveForwards(d);
        }
        else if (d < 0f)
        {
            MoveForwards(d);
        }
        /* 

                if (!(Input.GetMouseButton(0) || Input.GetMouseButton(1)))
                    return;
                float deltaX = Input.GetAxis("Mouse X") * sensitivityX;
                float deltaY = Input.GetAxis("Mouse Y") * sensitivityY;
                if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
                {
                    Strafe(deltaX);
                    ChangeHeight(deltaY);
                }
                else
                {
                    if (Input.GetMouseButton(0))
                    {
                        MoveForwards(deltaY);
                        ChangeHeading(deltaX);
                    }
                    else if (Input.GetMouseButton(1))
                    {
                        ChangeHeading(deltaX);
                        ChangePitch(-deltaY);
                    }
                }
                */
    }
    void MoveForwards(float aVal)
    {
        Vector3 fwd = transform.forward;
        fwd.y = 0;
        fwd.Normalize();
        transform.position += aVal * fwd;
    }
    void Strafe(float aVal)
    {
        transform.position += aVal * transform.right;
    }
    void ChangeHeight(float aVal)
    {
        transform.position += aVal * Vector3.up;
    }
    void ChangeHeading(float aVal)
    {
        mHdg += aVal;
        WrapAngle(ref mHdg);
        transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
    }
    void ChangePitch(float aVal)
    {
        mPitch += aVal;
        WrapAngle(ref mPitch);
        transform.localEulerAngles = new Vector3(mPitch, mHdg, 0);
    }
    public static void WrapAngle(ref float angle)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
    }
}
