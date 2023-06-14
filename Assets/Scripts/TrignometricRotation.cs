using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrignometricRotation : MonoBehaviour
{
    public float randomAngle;
    public float randomFrequency;
    public Vector3 AngleLimit;
    public Vector3 RotationFrequency;
    Vector3 FinalRotation;
    Vector3 StartRotation;
    public bool isManual = false;
    void Start()
    {
        StartRotation = transform.localEulerAngles;
        if (!isManual)
        {
            AngleLimit.x = Random.Range(0, randomAngle);
            AngleLimit.y = Random.Range(0, randomAngle);
            AngleLimit.z = Random.Range(0, randomAngle);
            RotationFrequency.x = Random.Range(0, randomFrequency);
            RotationFrequency.y = Random.Range(0, randomFrequency);
            RotationFrequency.z = Random.Range(0, randomFrequency);
        }
    }
    void FixedUpdate()
    {
        FinalRotation.x = StartRotation.x + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.x) * AngleLimit.x;
        FinalRotation.y = StartRotation.y + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.y) * AngleLimit.y;
        FinalRotation.z = StartRotation.z + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.z) * AngleLimit.z;
        transform.localEulerAngles = new Vector3(FinalRotation.x, FinalRotation.y, FinalRotation.z);
    }
}
