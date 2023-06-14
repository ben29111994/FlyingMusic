using UnityEngine;
using System.Collections;

/// <summary>
/// Class in charge to destroy all the platform who entered in
/// 
/// This script is attached to the GameObject destroyer (child of the GameObject "PlayerParent")
/// </summary>
namespace SonicBloom.Koreo
{
    public class Destroyer : MonoBehaviour
    {
        /// <summary>
        /// Will desatcivate (= despawn) the platform who is triggered with
        /// </summary>
        public static float progress = 0;
        int id;
        int totalEvent;

        void Start()
        {
            progress = 0;
            id = PlayerPrefs.GetInt("id");
            totalEvent = SpawnItem.SpawnItemInstance.listEvent.Count;
        }

        void LateUpdate()
        {
            if (PlayerControl.isFollowPlayer)
            {
                MenuManager.MenuManagerInstance.ProgressPercent.text = ((int)(progress * 100 / totalEvent)).ToString() + "%";
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Progress")
            {
                progress++;
                SpawnItem.numberOfEvent--;
            }
            Destroy(other.gameObject);
        }
    }
}
