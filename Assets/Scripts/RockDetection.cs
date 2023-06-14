using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockDetection : MonoBehaviour {
    public GameObject collisionParticle;
    bool isDetect = false;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(delayDetectOn());
    }

    // Update is called once per frame
    //void Update () {

    //}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Block" && isDetect)
        {
            Debug.Log("Hit");
            GameObject particles = Instantiate(collisionParticle, transform.position, Quaternion.identity) as GameObject;
            particles.GetComponent<ParticleSystem>().Play();
            Destroy(other.gameObject);
            //Destroy(gameObject);
        }
    }

    IEnumerator delayDetectOn()
    {
        yield return new WaitForSeconds(3);
        isDetect = true;
    }
}
