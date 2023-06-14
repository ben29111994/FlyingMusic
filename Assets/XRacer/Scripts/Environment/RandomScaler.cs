using UnityEngine;
using System.Collections;

/// <summary>
/// Give an object a random scale - used to quickly get a lot a variation out of the same models
/// </summary>
namespace SonicBloom.Koreo
{
    public class RandomScaler : MonoBehaviour
    {
        [Tooltip("The minimum scale value to use for this object")]
        public Vector3 minScale = Vector3.one;
        [Tooltip("The maximum scale value to use for this object")]
        public Vector3 maxScale = Vector3.one;

        [EventID]
        public string eventID;

        void Start()
        {
            // Register for Koreography Events.  This sets up the callback.
            Koreographer.Instance.RegisterForEvents(eventID, AddImpulse);

            Vector3 scale;
            scale.x = Mathf.Lerp(minScale.x, maxScale.x, Random.value);
            scale.y = Mathf.Lerp(minScale.y, maxScale.y, Random.value);
            scale.z = Mathf.Lerp(minScale.z, maxScale.z, Random.value);
            transform.localScale = scale;
        }

        void FixedUpdate()
        {
            transform.localScale -= new Vector3(0, 0, 0.05f);
        }

        void AddImpulse(KoreographyEvent evt)
        { 
            Vector3 scale;
            scale.x = Mathf.Lerp(minScale.x, maxScale.x, Random.value);
            scale.y = Mathf.Lerp(minScale.y, maxScale.y, Random.value);
            scale.z = Mathf.Lerp(minScale.z, maxScale.z, Random.value);
            transform.localScale = scale;
        }

        void OnDestroy()
        {
            // Sometimes the Koreographer Instance gets cleaned up before hand.
            //  No need to worry in that case.
            if (Koreographer.Instance != null)
            {
                Koreographer.Instance.UnregisterForAllEvents(this);
            }
        }
    }
}
