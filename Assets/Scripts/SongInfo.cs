using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace SonicBloom.Koreo
{
    public class SongInfo : MonoBehaviour
    {
        public static SongInfo songInfoInstance;
        MenuManager uiManager;
        public Text IDText;
        public Text NameText;
        public Text numberProgress;
        public Image fillProgress;
        public Image star1;
        public Image star2;
        public Image star3;
        public float score;
        public float progress;
        public  float eventCount;
        public Koreography koreography;
        private KoreographyTrackBase koreographyTrackBase;
        List<KoreographyEvent> listEvent = new List<KoreographyEvent>();
        public GameObject buttonNormal;
        public GameObject buttonHard;
        public GameObject buttonBuySong;
        public Image buttonTrialPlay;
        AudioSource trialMusic;
        Tween myTween;
        public Image background;
        Color color;
        int price;
        // Use this for initialization
        void Start()
        {
            songInfoInstance = this;
            trialMusic = GetComponent<AudioSource>();
            int id = int.Parse(IDText.text.ToString());
            var unlockStatus = PlayerPrefs.GetInt(IDText.text.ToString() + "unlock");
            if (unlockStatus == 1 || id == 1)
            {
                buttonNormal.SetActive(true);
                buttonHard.SetActive(true);
            }
            else
            {
                buttonBuySong.SetActive(true);
                fillProgress.transform.parent.gameObject.SetActive(false);
                star1.transform.parent.gameObject.SetActive(false);
                star2.transform.parent.gameObject.SetActive(false);
                star3.transform.parent.gameObject.SetActive(false);
                price = id * 100;
                buttonBuySong.GetComponentInChildren<Text>().text = price.ToString();
            }

            //For auto generate songs list
            uiManager = FindObjectOfType<MenuManager>();
            koreography = uiManager.koreographies[int.Parse(IDText.text.ToString()) - 1];
            koreographyTrackBase = koreography.GetTrackByID(NameText.text.ToString());
            listEvent = koreographyTrackBase.GetAllEvents();
            eventCount = listEvent.Count;
            score = PlayerPrefs.GetFloat(IDText.text.ToString()+"score");
            progress = PlayerPrefs.GetFloat(IDText.text.ToString()+"progress");
            numberProgress.text = ((int)(progress * 100 / eventCount)).ToString() + "%";
            fillProgress.fillAmount = progress / eventCount;
            var scoreProgress = score / eventCount;
            if(scoreProgress > 0.66f)
            {
                star1.fillAmount = 1;
                star2.fillAmount = 1;
                star3.fillAmount = scoreProgress - 0.66f * 100 / 33.3f;
            }
            else if (scoreProgress > 0.33f)
            {
                star1.fillAmount = 1;
                star2.fillAmount = (scoreProgress - 0.33f) * 100 / 33.3f;
            }
            else if (scoreProgress < 0.33f)
            {
                star1.fillAmount = scoreProgress * 100 / 33.3f;
            }

            background.sprite = UIManager.instance.backgrounds[int.Parse(IDText.text.ToString()) - 1];
            //line.sprite = UIManager.instance.lines[int.Parse(IDText.text.ToString()) - 1];
            //string stringColor = UIManager.instance.colors[int.Parse(IDText.text.ToString()) - 1];
            //foreach (var item in cubes)
            //{
            //    ColorUtility.TryParseHtmlString("#" + stringColor, out color);
            //    item.color = color;
            //}
        }

        public void OnButtonClick()
        {
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            if (button.name == "BtnHard")
            {
                MenuManager.isHard = true;
            }
            else
            {
                MenuManager.isHard = false;
            }
            int currentlife = PlayerPrefs.GetInt("life");
            if (currentlife > 0)
            {
                uiManager.OnChooseSongButton(this.gameObject);
            }
            else
            {
                UIManager.instance.BtnGetLife_ViewAds();
            }
                //SoundManager.instance.PlaySound(SoundManager.instance.invalid);

            MenuManager.MenuManagerInstance.OnButtonPress();
        }

        public void OnButtonBuySong()
        {
            var currentGem = PlayerPrefs.GetInt("gem");
            if(currentGem >= price)
            {
                MenuManager.MenuManagerInstance.OnButtonPress();
                currentGem -= price;
                PlayerPrefs.SetInt("gem", currentGem);
                PlayerPrefs.SetInt(IDText.text.ToString() + "unlock", 1);
                UIManager.instance.gemText.text = currentGem.ToString();
                buttonNormal.SetActive(true);
                buttonHard.SetActive(true);
                buttonBuySong.SetActive(false);
                fillProgress.transform.parent.gameObject.SetActive(true);
                star1.transform.parent.gameObject.SetActive(true);
                star2.transform.parent.gameObject.SetActive(true);
                star3.transform.parent.gameObject.SetActive(true);
            }
            else
                SoundManager.instance.PlaySound(SoundManager.instance.invalid);
        }

        public void OnButtonTrialPlay()
        {
            if (!trialMusic.isPlaying)
            {
                trialMusic.clip = MenuManager.MenuManagerInstance.songs[int.Parse(IDText.text.ToString())];
                AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
                foreach (var item in allAudioSources)
                {
                    item.Stop();
                }
                trialMusic.Play();
                var color1 = new Color32(246, 193, 34, 255);
                buttonTrialPlay.color = color1;
                var color2 = new Color32(246, 193, 34, 50);
                myTween = buttonTrialPlay.DOColor(color2, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
            else
            {
                trialMusic.Stop();
                UIManager.instance.bgm.Play();
                var color = new Color32(246, 193, 34, 255);
                myTween.Kill();
                buttonTrialPlay.color = color;
            }
        }

        private void FixedUpdate()
        {
            if(trialMusic.time > 10)
            {
                trialMusic.Stop();
                UIManager.instance.bgm.Play();
                var color = new Color32(246, 193, 34, 255);
                myTween.Kill();
                buttonTrialPlay.color = color;
            }
            if(!trialMusic.isPlaying)
            {
                var color = new Color32(246, 193, 34, 255);
                myTween.Kill();
                buttonTrialPlay.color = color;
            }
        }
    }
}
