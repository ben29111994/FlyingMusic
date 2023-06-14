using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

namespace SonicBloom.Koreo
{
    public class UnityAdsManager : MonoSingleton<UnityAdsManager>
    {
#if UNITY_ADS
        private string GooglePlayGameID = "2738984";
        private string AppStoreGameID = "2738985";
        private string gameID;

        private float timeLastAdsShow = 0;
        public float timeBetweenAds = 120;
        private bool firstTime = true;

        // Use this for initialization
        void Awake()
        {
#if UNITY_ANDROID
            gameID = GooglePlayGameID;
#elif UNITY_IOS
		gameID = AppStoreGameID;
#endif
            Advertisement.Initialize(gameID);
        }

        private bool CanShowAds()
        {
            if (firstTime)
            {
                firstTime = false;
                timeLastAdsShow = Time.realtimeSinceStartup;

                return true;
            }
            else
            {
                float currentTime = Time.realtimeSinceStartup;

                if (currentTime - timeLastAdsShow >= timeBetweenAds)
                {
                    timeLastAdsShow = currentTime;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void ShowAds(string placementID = "")
        {
            StartCoroutine(WaitForAd());

            if (string.Equals(placementID, ""))
                placementID = null;

            ShowOptions options = new ShowOptions();
            options.resultCallback = AdCallbackhandler;

            if (Advertisement.IsReady(placementID) && CanShowAds())
                Advertisement.Show(placementID);

        }

        void AdCallbackhandler(ShowResult result)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    Debug.Log("Ad Finished. Rewarding player...");
                    break;
                case ShowResult.Skipped:
                    Debug.Log("Ad skipped. Son, I am dissapointed in you");
                    break;
                case ShowResult.Failed:
                    Debug.Log("I swear this has never happened to me before");
                    break;
            }
        }

        public void ShowRewardAds()
        {
            ShowRewardVideo();
        }
        int number;
        void ShowRewardVideo(string placementID = "rewardedVideo")
        {
            StartCoroutine(WaitForAd());

            if (string.Equals(placementID, ""))
                placementID = null;
            ShowOptions options = new ShowOptions();
            options.resultCallback = RewardCallback;

            if (Advertisement.IsReady(placementID))
                Advertisement.Show(placementID, options);
        }
        void RewardCallback(ShowResult reward)
        {
            switch (reward)
            {
                case ShowResult.Finished:
                    Debug.Log("Ad Finished. Rewarding player...");
                    UIManager.instance.CompleteViewAds();
                    break;
                case ShowResult.Skipped:
                    Debug.Log("Ad skipped. Son, I am dissapointed in you");
                    break;
                case ShowResult.Failed:
                    Debug.Log("I swear this has never happened to me before");
                    break;
            }
        }
        IEnumerator WaitForAd()
        {
            //float currentTimeScale = Time.timeScale;
            //Time.timeScale = 0f;
            yield return null;

            while (Advertisement.isShowing)
                yield return null;

            //Time.timeScale = currentTimeScale;
        }
#endif
    }
}