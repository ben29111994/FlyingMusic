using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// The player controller
/// </summary>
namespace SonicBloom.Koreo
{
    public class PlayerControl : MonoBehaviour
    {
        public static PlayerControl PlayerControlInstance;
        [Tooltip("The starting speed of the vehicle - in m/s")]
        public float speed = 80.0f;
        [Tooltip("The steering speed of the vehicle")]
        public float steerSpeed = 8.0f;
        [Tooltip("The hoveringing speed of the vehicle")]
        public float hoverSpeed = 8.0f;
        [Tooltip("The amount the vehicle tilts when turning - in degrees")]
        public float tiltAngle = 30.0f;
        [Tooltip("The speed at which the vehicle turns")]
        public float tiltSpeed = 10.0f;

        [Tooltip("A prefab to instantiate when the vehicle crashes")]
        public GameObject collisionParticles;
        [Tooltip("The default prefab to use for the vehicle model")]
        public GameObject[] defaultShipPrefab;

        [Tooltip("Make the player invincible - useful for testing")]
        public static bool isInvincible = false;

        private float steer = 0.0f;             // current steering value
        private float hover = 0.0f;
        private float tilt = 0.0f;              // current tilt angle
        private float tiltHover = 0.0f;
        private float tiltAuto = 0.0f;
        private float tiltHoverAuto = 0.0f;
        private float speedMultiplier = 1.0f;   // speed multiplier for speedboosts/slow motion effects
        private bool crashed = false;           // have we crashed?
        private float startingSpeed;            // initial speed at the start of the game

        private GameObject shipModel;           // our vehicle model

        private int powerupLayer;               // layer that powerup collectables are on

        /// <summary>
        /// Get the current steering value of the vehicle (between -1 and 1)
        /// </summary>
        /// <value>The current steering value</value>
        public float Steer { get { return steer; } }
        public float Hover { get { return hover; } }

        /// <summary>
        /// Get the current speed of the vehicle in m/s
        /// </summary>
        /// <value>The current speed</value>
        public float Speed { get { return crashed ? 0.0f : speed * speedMultiplier; } }

        public string horizontalAxis = "Horizontal";
        public string verticalAxis = "Vertical";
        public GameObject playerController;
        public static bool isFollowPlayer = false;
        public GameObject Enemy;

        public Text ScoreText;
        int Score;
        public GameObject VolumetricLight;
        static float steerValue;
        static float hoverValue;
        Vector2 secondPressPos;
        Vector2 firstPressPos;
        public AudioSource soundEffect;
        public AudioClip[] soundEffects;
        public GameObject FadeParticles;
        public ParticleSystem cameraEffect;
        float limitValue = 0.25f;
        public Text perfectText;
        public Text statusText;
        int combo = 1;
        int highestCombo = 0;
        int good = 0;
        int perfect = 0;
        float tempX = 0;
        float tempY = 0;
        Color color;
        int totalEvent = 0;
        public GameObject ratePanel;

        /// <summary>
        /// Get or set the current speed multiplier.
        /// </summary>
        /// <value>The current speed multiplier</value>
        public float SpeedMultiplier
        {
            get { return speedMultiplier; }
            set { speedMultiplier = value; }
        }

        void Awake()
        {
            PlayerControlInstance = this;
            powerupLayer = LayerMask.NameToLayer("Powerup");

            // remember our starting speed.
            startingSpeed = speed;

            // create the default vehicle model
            var currentShip = PlayerPrefs.GetInt("ship");
            SetShipModel(defaultShipPrefab[currentShip]);

            Reset();
        }

        void Start()
        {
            Application.targetFrameRate = 60;
            steerValue = 0;
            hoverValue = 0;
            Score = 0;
            if (SpawnItem.isPlaying)
            {
                soundEffect.Play();
            }
            isFollowPlayer = false;
            isInvincible = false;
            totalEvent = SpawnItem.SpawnItemInstance.listEvent.Count;
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

        public void Reset()
        {
            steer = 0.0f;
            hover = 0.0f;
            tilt = 0.0f;
            tiltHover = 0.0f;
            tiltAuto = 0.0f;
            tiltHoverAuto = 0.0f;
            speedMultiplier = 1.0f;
            crashed = false;
            speed = startingSpeed;
        }

        public void SetShipModel(GameObject shipPrefab)
        {
            // destroy the current model if it exists
            if (shipModel != null)
                Destroy(shipModel);

            // instantiate a new vehicle model, and parent it to this object
            shipModel = Instantiate(shipPrefab, transform.position, transform.rotation) as GameObject;
            shipModel.transform.parent = transform;
        }

        private void Update()
        {
            transform.localPosition = new Vector3(Mathf.Clamp(transform.localPosition.x, -5f, 5f), Mathf.Clamp(transform.localPosition.y, -5f, 5f), transform.localPosition.z);
            VolumetricLight.transform.position = new Vector3(VolumetricLight.transform.position.x, VolumetricLight.transform.position.y, transform.position.z + 100);
            if (isFollowPlayer)
            {
                Camera.main.transform.localPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f/*5*/, transform.localPosition.z - 5/*10*/), 1f);
                Camera.main.transform.localRotation = Quaternion.RotateTowards(Camera.main.transform.localRotation, transform.localRotation, 0.5f);
            }
            else
            {
                Camera.main.transform.localPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, new Vector3(transform.localPosition.x, transform.localPosition.y + 0.5f, transform.localPosition.z - 5), 0.15f);
                Camera.main.transform.localRotation = Quaternion.RotateTowards(Camera.main.transform.localRotation, transform.localRotation, 0.15f);
            }
        }

        void FixedUpdate()
        {
            if (Joystick.joystickHeld && isFollowPlayer)
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), 5);

                steerValue = SimpleInput.GetAxis(horizontalAxis);
                hoverValue = SimpleInput.GetAxis(verticalAxis);
                if (steerValue > limitValue)
                    steerValue = limitValue;
                if (steerValue < -limitValue)
                    steerValue = -limitValue;
                if (hoverValue > limitValue)
                    hoverValue = limitValue;
                if (hoverValue < -limitValue)
                    hoverValue = -limitValue;

                float targetTiltX = -steerValue * tiltAngle;
                float targetTiltY = -hoverValue * tiltAngle;
                tilt = Mathf.Lerp(tilt, targetTiltX * 2, tiltSpeed * Time.deltaTime);
                tiltHover = Mathf.Lerp(tiltHover, targetTiltY / 2, tiltSpeed * Time.deltaTime);
                Vector3 rot = transform.localEulerAngles;
                rot.z = tilt;
                rot.x = tiltHover;
                transform.localEulerAngles = rot;


                Vector3 posMove = new Vector3(steerValue / steerSpeed, hoverValue / hoverSpeed, 0);
                this.transform.localPosition += posMove * Time.deltaTime;

                tempX = playerController.transform.localPosition.x;
                tempY = playerController.transform.localPosition.y;
            }
            else
            {
                transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(new Vector3(0, 0, 0)), 5);

                var moveX = playerController.transform.localPosition.x - tempX;
                var moveY = playerController.transform.localPosition.y - tempY;
                tempX = playerController.transform.localPosition.x;
                tempY = playerController.transform.localPosition.y;
                float targetTiltX = 0;
                float targetTiltY = 0;
                targetTiltX = -moveX / 5f * tiltAngle;
                targetTiltY = -moveY / 7f * tiltAngle;
                tilt = Mathf.Lerp(tilt, targetTiltX * 2, tiltSpeed * Time.deltaTime);
                tiltHover = Mathf.Lerp(tiltHover, targetTiltY / 2, tiltSpeed * Time.deltaTime);
                Vector3 rot = transform.localEulerAngles;
                rot.z = tilt;
                rot.x = tiltHover;
                transform.localEulerAngles = rot;
            }
        }

        IEnumerator delayFade(GameObject other)
        {
            yield return new WaitForSeconds(2);
            Destroy(other.gameObject);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Progress")
            {
                Score++;
                ScoreText.text = Score.ToString();
            }
            if (other.gameObject.tag == "Powerup")
            {
                bool isTouch = false;
                if (SpawnItem.isShake)
                {
                    Camera.main.transform.DOShakeRotation(0.1f, 2, 2, 2);
                }
                if (other.name == "Loop")
                {
                    isTouch = true;
                    var colList = other.GetComponentsInChildren<BoxCollider>();
                    foreach (var item in colList)
                    {
                        item.enabled = false;
                    }
                    other.GetComponent<BoxCollider>().enabled = false;
                    if (combo >= 2)
                    {
                        perfectText.text = "PERFECT x " + combo.ToString();
                    }
                    else
                        perfectText.text = "PERFECT";
                    perfectText.color = new Color32(255, 0, 255, 255);
                    perfect++;
                    combo++;
                    if (combo > highestCombo)
                    {
                        highestCombo = combo;
                    }
                }
                else
                {
                    if (!isTouch)
                    {
                        var colList = other.GetComponents<BoxCollider>();
                        foreach (var item in colList)
                        {
                            item.enabled = false;
                        }
                        perfectText.text = "GOOD";
                        perfectText.color = new Color32(0, 255, 0, 255);
                        good++;
                        combo = 1;
                    }
                }
                iTween.PunchScale(perfectText.gameObject, new Vector3(1f, 1f, 1f), 0.3f);
                var powerRing = other.transform.parent;
                iTween.ScaleTo(powerRing.gameObject, new Vector3(0.01f, 0.01f, 0.01f), 0.2f);
                Vector3 otherPos = other.transform.position;
                otherPos.z += 2;
                FadeParticles.transform.position = otherPos;
                FadeParticles.transform.rotation = Quaternion.identity;
                FadeParticles.SetActive(true);
                FadeParticles.GetComponent<ParticleSystem>().Play();
                cameraEffect.Play();
                StartCoroutine(delayFade(other.gameObject));
            }
            else if (other.gameObject.tag == "Progress")
            { }
            else
            {
                // We've crashed!
                // instantiate crash particles
                Handheld.Vibrate();
                Camera.main.transform.DOShakeRotation(0.1f, 2, 2, 2);
                if (isFollowPlayer)
                {
                    Vector3 otherPos = other.transform.position;
                    otherPos.z += 2;
                    collisionParticles.transform.position = otherPos;
                    collisionParticles.transform.rotation = Quaternion.identity;
                    collisionParticles.SetActive(true);
                    collisionParticles.GetComponent<ParticleSystem>().Play();
                }

                soundEffect.clip = soundEffects[0];
                soundEffect.Play();

                // come to a complete stop and hide the player model
                crashed = true;
                steer = 0.0f;
                hover = 0.0f;
                shipModel.SetActive(false);
                this.GetComponent<BoxCollider>().enabled = false;
                isFollowPlayer = false;
                StartCoroutine(delayContinueMenu());
            }
        }

        IEnumerator delayContinueMenu()
        {
            yield return new WaitForSeconds(2);
            if (PlayerPrefs.GetInt("RemoveAds") == 0)
            {
                //UnityAdsManager.Instance.ShowAds();
            }
            LoadResult();
            SpawnItem.isMusicStart = false;
            SpawnItem.SpawnItemInstance.audioCom.Pause();
            MenuManager.MenuManagerInstance.ContinueMenu.SetActive(true);
            MenuManager.MenuManagerInstance.simpleMusicPlayer.Pause();
            SoundManager.instance.PlaySound(SoundManager.instance.invalid);
            Time.timeScale = 0f;
        }

        public void LoadResult()
        {
            var id = PlayerPrefs.GetInt("id");
            var highestScore = PlayerPrefs.GetFloat(id.ToString() + "score");
            if (!MenuManager.isHard)
            {
                if (Score >= highestScore)
                {
                    PlayerPrefs.SetFloat(id.ToString() + "score", Score);
                    PlayerPrefs.SetInt("score", Score);
                }
                var highestProgress = PlayerPrefs.GetFloat(id.ToString() + "progress");
                if (Destroyer.progress >= highestProgress)
                {
                    PlayerPrefs.SetFloat(id.ToString() + "progress", Destroyer.progress);
                }
                var gem = PlayerPrefs.GetInt("gem");
                gem += perfect;
                PlayerPrefs.SetInt("gem", gem);
            }
            else
            {
                var gem = PlayerPrefs.GetInt("gem");
                gem += Score / 4 + perfect;
                PlayerPrefs.SetInt("gem", gem);
            }
        }


        public void OnGameOver()
        {
            SpawnItem.isMusicStart = false;
            SoundManager.instance.PlaySound(SoundManager.instance.collect);
            MenuManager.MenuManagerInstance.GameOverMenu.SetActive(true);
            Time.timeScale = 1;
            MenuManager.MenuManagerInstance.songName.text = PlayerPrefs.GetString("name").Replace(" (UnityEngine.AudioClip)", "");
            float scoreProgress = (float)Score / (float)totalEvent;
            Debug.Log("ScoreProgress: " + scoreProgress);
            if (scoreProgress > 0.66f)
            {
                var id = PlayerPrefs.GetInt("id");
                id++;
                PlayerPrefs.SetInt(id + "unlock", 1);
                MenuManager.MenuManagerInstance.star1.DOFillAmount(1, 1f);
                MenuManager.MenuManagerInstance.star2.DOFillAmount(1, 2f);
                MenuManager.MenuManagerInstance.star3.DOFillAmount(scoreProgress - 0.66f * 100 / 33.3f, 3f);
            }
            else if (scoreProgress > 0.33f)
            {
                MenuManager.MenuManagerInstance.star1.DOFillAmount(1, 1f);
                MenuManager.MenuManagerInstance.star2.DOFillAmount((scoreProgress - 0.33f) * 100 / 33.3f, 2f);
            }
            else if (scoreProgress < 0.33f)
            {
                MenuManager.MenuManagerInstance.star1.DOFillAmount(scoreProgress * 100 / 33.3f, 1f);
            }
            string progressPercent = MenuManager.MenuManagerInstance.ProgressPercent.text.ToString();
            var miss = SpawnItem.SpawnItemInstance.listEvent.Count - Score;
            MenuManager.MenuManagerInstance.numberScoreBoard.text = progressPercent + "\n"
                + perfect + "\n" + good + "\n"
                + miss.ToString() + "\n" + highestCombo + "\n" + "\n" + Score;
            MenuManager.MenuManagerInstance.nameScoreBoard.text = "DISTANCE" + "\n" + "PERFECT" + "\n" + "GOOD" + "\n" + "MISS" + "\n" + "COMBO" + "\n" + "\n"
                + "TOTAL SCORE";
            var life = PlayerPrefs.GetInt("life");
            life++;
            PlayerPrefs.SetInt("life", life);
            if (int.Parse(progressPercent) == 100)
            {
                StartCoroutine(delayShowRatePanel());
            }
            LoadResult();
        }

        IEnumerator delayShowRatePanel()
        {
            yield return new WaitForSeconds(2);
            var rateTime = PlayerPrefs.GetInt("RateTime");
            if (PlayerPrefs.GetInt("isRate") == 0 && (rateTime % 3 == 0))
            {
                rateTime++;
                ratePanel.SetActive(true);
            }
        }

        public void OnResume()
        {
            Time.timeScale = 1;
            crashed = false;
            shipModel.SetActive(true);
            this.GetComponent<BoxCollider>().enabled = true;
            isFollowPlayer = true;
            SpawnItem.isMusicStart = true;
            MenuManager.MenuManagerInstance.ContinueMenu.SetActive(false);
            MenuManager.MenuManagerInstance.simpleMusicPlayer.Play();
            SpawnItem.SpawnItemInstance.audioCom.UnPause();
            SoundManager.instance.audioSource.Stop();
        }

        public void OnUseGemToRevive()
        {
            var currentGem = PlayerPrefs.GetInt("gem");
            if (currentGem >= 200)
            {
                MenuManager.MenuManagerInstance.OnButtonPress();
                currentGem -= 200;
                PlayerPrefs.SetInt("gem", currentGem);
                OnResume();
            }
            else
                SoundManager.instance.PlaySound(SoundManager.instance.invalid);
        }  
    }
}
