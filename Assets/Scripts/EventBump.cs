using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SonicBloom.Koreo
{
    public class EventBump : MonoBehaviour
    {
        public bool isDestroy = false;
        [EventID]
        public string eventID;
        // Use this for initialization
        private void Awake()
        {
            string name = PlayerPrefs.GetString("name");
            eventID = name;
        }

        void Start()
        {
            isDestroy = false;
            float BumpOrRoam = Random.Range(1, 10);
            if (BumpOrRoam > 5)
            {
                //GetComponent<TrignometricMovement>().enabled = false;
                //GetComponent<TrignometricRotation>().enabled = false;
                Koreographer.Instance.RegisterForEvents(eventID, Bump);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(isDestroy)
            {
                Koreographer.Instance.UnregisterForEvents(eventID, Bump);
                Destroy(this.gameObject);
            }
        }

        void Bump(KoreographyEvent evt)
        {
            StartCoroutine(delayBump());
        }

        IEnumerator delayBump()
        {
            yield return new WaitForSeconds(3);
            //transform.DOPunchScale(new Vector3(2f, 2f, 2f), 0.5f);
            iTween.PunchScale(gameObject, new Vector3(2f, 2f, 2f), 0.5f);
        }
    }
}
