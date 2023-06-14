using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace SonicBloom.Koreo
{
    public class UIManager : MonoBehaviour {

        public static UIManager instance;

        [Header(" menu ")]
        public GameObject taskBar;
        public GameObject menuSong;
        public GameObject menuPlane;
        public GameObject menuStore;
        public GameObject menuMySong;
        public GameObject menuInfo;

        public Text gemText;
        public Text lifeText;
        public Text scoreText;
        public Text[] selectedText;

        bool isDelay;

        [Header(" button ")]
        public GameObject buttonSong;
        public GameObject buttonPlane;
        public GameObject buttonStore;
        public GameObject buttonMySong;
        public GameObject buttonInfo;
        //IAP
        public GameObject btnRemoveAds;
        public GameObject btnBuy120Gems;
        public GameObject btnBuy250Gems;
        public GameObject btnBuy400Gems;
        public GameObject btnBuy800Gems;

        public AudioSource bgm;
        public Sprite[] backgrounds;

        void Awake() {
            instance = this;
            if (PlayerPrefs.GetInt("isFirstTime") == 0)
            {
                PlayerPrefs.SetInt("life", 10);
                PlayerPrefs.SetInt("isFirstTime", 1);
            }
        }

        void Start()
        {
            bgm.Play();
            gemText.text = PlayerPrefs.GetInt("gem").ToString();
            scoreText.text = PlayerPrefs.GetInt("score").ToString();
            lifeText.text = PlayerPrefs.GetInt("life").ToString();
        }

        public void BtnPlane() {
            if (!bgm.isPlaying)
            {
                bgm.Play();
            }
            menuPlane.SetActive(true);
            menuSong.SetActive(false);
            menuStore.SetActive(false);
            menuMySong.SetActive(false);
            menuInfo.SetActive(false);

            iTween.ScaleTo(buttonSong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonStore, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonMySong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonInfo, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            iTween.ScaleTo(button, new Vector3(1, 1, 1), 0.3f);
        }

        public void BtnMySongs()
        {
            if (!bgm.isPlaying)
            {
                bgm.Play();
            }
            menuSong.SetActive(false);
            menuPlane.SetActive(false);
            menuStore.SetActive(false);
            menuMySong.SetActive(true);
            menuInfo.SetActive(false);

            iTween.ScaleTo(buttonSong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonPlane, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonStore, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonInfo, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            iTween.ScaleTo(button, new Vector3(1, 1, 1), 0.3f);
        }

        public void BtnSongs() {
            menuSong.SetActive(true);
            menuPlane.SetActive(false);
            menuStore.SetActive(false);
            menuMySong.SetActive(false);
            menuInfo.SetActive(false);

            iTween.ScaleTo(buttonPlane, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonStore, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonMySong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonInfo, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            iTween.ScaleTo(button, new Vector3(1, 1, 1), 0.3f);
        }

        public void BtnStores() {
            if (!bgm.isPlaying)
            {
                bgm.Play();
            }
            menuPlane.SetActive(false);
            menuSong.SetActive(false);
            menuStore.SetActive(true);
            menuMySong.SetActive(false);
            menuInfo.SetActive(false);

            iTween.ScaleTo(buttonSong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonPlane, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonMySong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonInfo, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            iTween.ScaleTo(button, new Vector3(1, 1, 1), 0.3f);
        }


        public void BtnInfomation() {
            if (!bgm.isPlaying)
            {
                bgm.Play();
            }
            menuPlane.SetActive(false);
            menuSong.SetActive(false);
            menuStore.SetActive(false);
            menuMySong.SetActive(false);
            menuInfo.SetActive(true);

            iTween.ScaleTo(buttonSong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonPlane, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonStore, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            iTween.ScaleTo(buttonMySong, new Vector3(0.8f, 0.8f, 0.8f), 0.3f);
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            iTween.ScaleTo(button, new Vector3(1, 1, 1), 0.3f);
        }

        public void BtnHelixHoop() {
#if UNITY_IOS
		Application.OpenURL("itms-apps://itunes.apple.com/app/id1327168260");
#endif
        }

        public void BtnKingBallIO() {
#if UNITY_IOS
		Application.OpenURL("itms-apps://itunes.apple.com/app/id1395136315");
#endif
        }

        public void BtnGetLife_ViewAds() {
            MenuManager.MenuManagerInstance.isGemOrLifeOrRevive = 1;
            //UnityAdsManager.Instance.ShowRewardAds();
        }

        public void BtnGetGem_ViewAds()
        {
            MenuManager.MenuManagerInstance.isGemOrLifeOrRevive = 0;
            //UnityAdsManager.Instance.ShowRewardAds();
        }

        public void BtnRevive_ViewAds()
        {
            MenuManager.MenuManagerInstance.isGemOrLifeOrRevive = 2;
            //UnityAdsManager.Instance.ShowRewardAds();
        }

        public void CompleteViewAds() {
            if (MenuManager.MenuManagerInstance.isGemOrLifeOrRevive == 0)
            {
                BtnGetGem();
            }
            else if (MenuManager.MenuManagerInstance.isGemOrLifeOrRevive == 1)
            {
                BtnGetLife();
            }
            else if (MenuManager.MenuManagerInstance.isGemOrLifeOrRevive == 2)
            {
                PlayerControl.PlayerControlInstance.OnResume();
            }
            SoundManager.instance.PlaySound(SoundManager.instance.collect);
        }

        public void BtnFullUnlock_IAP()
        {
            SoundManager.instance.PlaySound(SoundManager.instance.collect);

            PlayerPrefs.SetInt("RemoveAds", 1);
            PlayerPrefs.SetInt("FullUnlock", 1);
            for (int i = 1; i <= 15; i++)
            {
                PlayerPrefs.SetInt(i + "unlock", 1);
                if (i < 3)
                {
                    PlayerPrefs.SetInt("ship" + i, 1);
                }
            }
            Color color = new Color32(255, 255, 255, 255);
            MenuManager.MenuManagerInstance.flashPanel.GetComponent<Image>().DOColor(color, 1);
            SceneManager.LoadScene(1);
        }

        public void BtnRemoveAds_IAP() {
            SoundManager.instance.PlaySound(SoundManager.instance.collect);

            PlayerPrefs.SetInt("RemoveAds", 1);
        }
        public void BtnGetLife()
        {
            var life = PlayerPrefs.GetInt("life");
            life += 20;
            PlayerPrefs.SetInt("life", life);
            if (UIManager.instance.lifeText != null)
                UIManager.instance.lifeText.text = life.ToString();
        }

        public void BtnGetGem()
        {
            var gem = PlayerPrefs.GetInt("gem");
            gem += 20;
            PlayerPrefs.SetInt("gem", gem);
            if(UIManager.instance.gemText!=null)
                UIManager.instance.gemText.text = gem.ToString();
        }

        public void BtnBuy120Gems_IAP() {
            SoundManager.instance.PlaySound(SoundManager.instance.collect);
            PlusGem(120);
        }

        public void BtnBuy250Gems_IAP() {
            SoundManager.instance.PlaySound(SoundManager.instance.collect);
            PlusGem(250);
        }

        public void BtnBuy400Gems_IAP() {
            SoundManager.instance.PlaySound(SoundManager.instance.collect);
            PlusGem(400);
        }

        public void BtnBuy800Gems_IAP() {
            SoundManager.instance.PlaySound(SoundManager.instance.collect);
            PlusGem(800);
        }

        public void BtnSoundIAP() {

            SoundManager.instance.PlaySound(SoundManager.instance.star);

        }

        public void PlusGem(int num)
        {
            var gem = PlayerPrefs.GetInt("gem");
            gem += num;
            PlayerPrefs.SetInt("gem", gem);
            UIManager.instance.gemText.text = gem.ToString();
        }

        public void ButtonAbout()
        {
            Application.OpenURL(AppInfo.Instance.FACEBOOK_LINK);
        }

        public void ButtonRate()
        {
#if UNITY_ANDROID
            Application.OpenURL(AppInfo.Instance.PLAYSTORE_LINK);
#elif UNITY_IOS
        Application.OpenURL(AppInfo.Instance.APPSTORE_LINK);
#endif
            PlayerPrefs.SetInt("isRate", 1);
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            if (button.name == "BtnOK")
                button.GetComponentInParent<Transform>().gameObject.SetActive(false);
        }
    }
}
