using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotateByDrag : MonoBehaviour
{
    public float sensitivity = 1f;
    public float speedApprx = 1f;
    public float magneticPower = 1f;
    public AnimationCurve sensitivityEaseCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0.5f);

    const int QUEUE_SIZE = 5;

    float R;
    float cachedMouseX;
    Queue<float> angularSpeedQueue = new Queue<float>();
    float magneticMovementLimitTime;

    void Start()
    {
        InitContstants();
    }

    void InitContstants()
    {
        R = 1000f / Screen.width;
        magneticMovementLimitTime = 0.5f / magneticPower;
    }

    void Update()
    {
#if UNITY_EDITOR
        InitContstants();
#endif
        if (Input.GetMouseButtonDown(0))
        {
            cachedMouseX = Input.mousePosition.x;
            idleFirst = true;
        }

        if (Input.GetMouseButton(0))
        {
            HandleDelta(Input.mousePosition.x - cachedMouseX);
            cachedMouseX = Input.mousePosition.x;
        }
        else
        {
            HandleIdle();
        }
    }

    void HandleDelta(float dx)
    {
        angularSpeedQueue.Enqueue(dx);
        if (angularSpeedQueue.Count > QUEUE_SIZE)
        {
            angularSpeedQueue.Dequeue();
        }

        var yAngle = transform.localEulerAngles.y;
        var E = sensitivityEaseCurve.Evaluate(Mathf.Abs(45 - (yAngle % 90f)) / 45f);

        transform.RotateAround(Vector3.zero
            , Vector3.up
            , -sensitivity * dx * E * Time.deltaTime * R);
    }

    void HandleIdle()
    {
        var yAngle = transform.localEulerAngles.y;

        if (idleFirst)
        {
            cachedVel = avgAngularSpeed;
            var targetAngle = yAngle - cachedVel * speedApprx;
            var targetIDX = Mathf.FloorToInt((targetAngle + 45) / 90);
            normTargetAngle = targetIDX * 90f;
            idleFirst = false;
        }

        var calculatedYAngle = Mathf.SmoothDampAngle(yAngle
            , normTargetAngle
            , ref cachedVel
            , magneticMovementLimitTime);

        transform.RotateAround(Vector3.zero
            , Vector3.up
            , calculatedYAngle - yAngle);
    }

    float cachedVel;
    bool idleFirst = false;
    float normTargetAngle;

    float avgAngularSpeed
    {
        get
        {
            if (angularSpeedQueue.Count == 0)
            {
                return 0f;
            }

            var sum = 0f;

            foreach (var sample in angularSpeedQueue)
            {
                sum += sample;
            }

            return sum / angularSpeedQueue.Count;
        }
    }
}
