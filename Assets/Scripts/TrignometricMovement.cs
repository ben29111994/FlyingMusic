using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrignometricMovement : MonoBehaviour
{
    public float randomDistance;
    public float randomFrequencyX;
    public float randomFrequencyY;
    public Vector3 Distance;
    public Vector3 MovementFrequency;
    Vector3 Moveposition;
    Vector3 startPosition;
    public bool isManual = false;
    void Start()
    {
        startPosition = transform.localPosition;
        if (!isManual)
        {
            Distance.x = Random.Range(-randomDistance, -randomDistance);
            Distance.y = Random.Range(0, randomDistance);
            MovementFrequency.x = Random.Range(0, randomFrequencyX);
            MovementFrequency.y = Random.Range(0, randomFrequencyY);
            //		AdMobManager._AdMobInstance.loadInterstitial ();
        }
    }
    void FixedUpdate()
    {
        Moveposition.x = startPosition.x + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.x) * Distance.x;
        Moveposition.y = startPosition.y + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.y) * Distance.y;
        Moveposition.z = startPosition.z + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.z) * Distance.z;
        transform.localPosition = new Vector3(Moveposition.x, Moveposition.y, Moveposition.z);
    }
}
