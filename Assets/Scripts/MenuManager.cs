using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

namespace SonicBloom.Koreo
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager MenuManagerInstance;
        public List<Koreography> koreographies;
        public AudioClip[] songs;
        public GameObject spawnItem;
        public GameObject MusicPlayer;
        public GameObject SongPanel1;
        public GameObject SongPanel2;
        public GameObject GridView;
        public GameObject Menu;
        public BlurOptimized screenBlur;
        public SimpleMusicPlayer simpleMusicPlayer;
        public AudioSource UIAudio;
        public GameObject flashPanel;
        public static bool isHard;
        public GameObject PauseMenu;
        public AudioSource MainMusic;
        public Text ProgressPercent;
        public GameObject ContinueMenu;
        public GameObject GameOverMenu;
        public Text songName;
        public Image star1;
        public Image star2;
        public Image star3;
        public Text nameScoreBoard;
        public Text numberScoreBoard;
        public int isGemOrLifeOrRevive = 2;

        // Use this for initialization
        void Awake()
        {
            //PlayerPrefs.DeleteAll();
            simpleMusicPlayer = MusicPlayer.GetComponent<SimpleMusicPlayer>();
            MenuManagerInstance = this;
            Color color = new Color32(0, 0, 0, 0);
            flashPanel.GetComponent<Image>().DOColor(color, 1);

            //For auto generate songs list
            if(SpawnItem.isPlaying)
            {
                Menu.SetActive(false);
            }
            int i = 0;
            foreach (var item in koreographies)
            {
                i++;
                GameObject songPanel;
                if (i%2!=0)
                {
                    songPanel = Instantiate(SongPanel1) as GameObject;
                }
                else
                    songPanel = Instantiate(SongPanel2) as GameObject;

                songPanel.transform.SetParent(GridView.transform);
                songPanel.transform.localScale = new Vector3(1, 1, 1);
                songPanel.GetComponent<SongInfo>().IDText.text = i.ToString();
                songPanel.GetComponent<SongInfo>().NameText.text = songs[i].ToString().Replace(" (UnityEngine.AudioClip)", "");
            }
;
            screenBlur = GameObject.FindObjectOfType<BlurOptimized>();
            var id = PlayerPrefs.GetInt("id");
            var name = PlayerPrefs.GetString("name");
            if (id == 0)
            {
                id = 1;
            }
            if (name == null)
            {
                name = "Dubstep";
            }
            //Debug.Log(name);
            var music = spawnItem.GetComponent<AudioSource>();
            music.clip = songs[id];
            spawnItem.GetComponent<SpawnItem>().eventID = name;
            simpleMusicPlayer.LoadSong(koreographies[id - 1]);
        }

        public void OnPauseButton()
        {
            SpawnItem.isMusicStart = false;
            OnButtonPress();
            PauseMenu.SetActive(true);
            screenBlur.enabled = true;
            simpleMusicPlayer.Pause();
            MainMusic.Pause();
            Time.timeScale = 0f;
        }

        public void OnResumeButton()
        {
            Time.timeScale = 1;
            MainMusic.UnPause();
            simpleMusicPlayer.Play();
            OnButtonPress();
            PauseMenu.SetActive(false);
            screenBlur.enabled = false;
            if (SpawnItem.SpawnItemInstance.audioCom.isPlaying)
            {
                SpawnItem.isMusicStart = true;
            }
        }

        public void OnRestartButton()
        {
            Time.timeScale = 1;
            OnButtonPress();
            PauseMenu.SetActive(false);
            screenBlur.enabled = false;
            Color color = new Color32(255, 255, 255, 255);
            flashPanel.GetComponent<Image>().DOColor(color, 1);
            SpawnItem.isPlaying = true;
            SceneManager.LoadScene(1);
        }

        public void OnHomeButton()
        {
            Time.timeScale = 1;
            OnButtonPress();
            Color color = new Color32(255, 255, 255, 255);
            flashPanel.GetComponent<Image>().DOColor(color, 1);
            SpawnItem.isPlaying = false;
            SceneManager.LoadScene(1);
            screenBlur.enabled = true;
        }

        public void OnChooseSongButton(GameObject button)
        {
            Color color = new Color32(255, 255, 255, 255);
            flashPanel.GetComponent<Image>().DOColor(color, 1);
            SpawnItem.isPlaying = true;
            SceneManager.LoadScene(1);

            var id = int.Parse(button.GetComponent<SongInfo>().IDText.text.ToString());
            string name = button.GetComponent<SongInfo>().NameText.text.ToString();
            PlayerPrefs.SetInt("id", id);
            PlayerPrefs.SetString("name", name);
        }

        public void OnButtonPress()
        {
            UIAudio.Play();
        }
    }
}
