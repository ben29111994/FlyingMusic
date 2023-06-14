using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SWS;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using UnityEngine.EventSystems;


namespace SonicBloom.Koreo
{
    public class SpawnItem : MonoBehaviour
    {
        public static SpawnItem SpawnItemInstance;
        public GameObject AudioBeat;
        public splineMove playerMove;
        public splineMove eventMove;
        public GameObject player;
        public GameObject[] environmentList;
        //public GameObject[] PoolObject;
        public GameObject Environment;
        public GameObject VolumetricLight;
        public List<GameObject> Obstacles;
        [EventID]
        public string eventID;
        public GameObject powerUp;
        public AudioSource audioCom;
        public AudioSource audioEvent;
        public static bool isMusicStart = false;
        float randomOffsetX = 0;
        float randomOffsetY = 0;
        public static bool isShake = false;
        public static float numberOfEvent = 0;
        public GameObject[] defaultShipPrefab;
        private GameObject shipModel;
        public GameObject[] pathList;
        string pathName;
        int tutorialStep = 0;
        bool isTutorial = false;
        public GameObject tutorialText;
        public Material[] rockCL;
        int randomTheme;
        public static int randomShip;
        int nextItem = 9;
        Color color;
        Color lightColor;
        public float maxSize = 1;
        public static bool isPlaying = false;
        public TrailRenderer line;
        private Koreography koreography;
        private KoreographyTrackBase koreographyTrackBase;
        public List<KoreographyEvent> listEvent = new List<KoreographyEvent>();

        private void Awake()
        {
            SpawnItemInstance = this;
            if (isPlaying)
            {
                //Register the beat callback function
                var id = PlayerPrefs.GetInt("id");
                //Debug.Log("ID: " + id);
                //if (id > 15)
                //{
                //    AudioBeat.GetComponent<BeatDetection>().CallBackFunction = MyCallbackEventHandler;
                //}

                // create the default vehicle model
                randomShip = Random.Range(0, 2);
                SetSpawnShipModel(defaultShipPrefab[randomShip]);

                playerMove = player.GetComponent<splineMove>();
                eventMove = GetComponent<splineMove>();
            }
        }

        //public void MyCallbackEventHandler(BeatDetection.EventInfo eventInfo)
        //{
        //    switch (eventInfo.messageInfo)
        //    {
        //        case BeatDetection.EventType.Energy:
        //            SpawnEvent();
        //            break;
        //        case BeatDetection.EventType.HitHat:
        //            SpawnEvent();
        //            break;
        //        case BeatDetection.EventType.Kick:
        //            SpawnEvent();
        //            break;
        //        case BeatDetection.EventType.Snare:
        //            SpawnEvent();
        //            break;
        //    }
        //}

        void SpawnEvent()
        {
            if (isTutorial)
            {
                randomOffsetX = -1;
                randomOffsetY = 1;
                StartCoroutine(waitForInput());
            }
            //var payloadValue = evt.GetIntValue();
            //Debug.Log(payloadValue);
            Vector3 spawnPosition = new Vector3(transform.position.x + randomOffsetX, transform.position.y + /*UnityEngine.Mathf.Abs(*/randomOffsetY/*)*/, transform.position.z);
            GameObject powerUpObject = Instantiate(powerUp, spawnPosition, transform.rotation) as GameObject;
            powerUpObject.transform.localScale = Vector3.zero;
            powerUpObject.transform.DOScale(maxSize, 3f);

            Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-20, 20), transform.position.y + Random.Range(-40, 40), transform.position.z + 50); ;
            float rangeValue = 0;
            //if (PlayerControl.isInvincible)
            //{
            //    rangeValue = Random.Range(-10, 10);
            //}
            //else
            //    rangeValue = Random.Range(-20, 20);
            Vector3 rot = transform.eulerAngles;
            rot.z = Random.Range(0, 360);
            rot.y = Random.Range(0, 360);
            rot.x = Random.Range(0, 360);

            for (int i = 0; i < 3; i++)
            {
                if (i % 2 == 0)
                {
                    rangeValue = 20;
                }
                else
                    rangeValue = -20;
                GameObject environmentSpawn = Instantiate(Environment, spawnPos, Quaternion.Euler(rot)) as GameObject;
                environmentSpawn.GetComponent<TrignometricMovement>().randomDistance = rangeValue;
                float size = Random.Range(1, 10);
                environmentSpawn.transform.DOScale(size/*0.05f*/, 1/*0.5f*/);
                environmentSpawn.transform.DOScale(5f/*0.05f*/, 15/*0.5f*/);
                Obstacles.Add(environmentSpawn);
            }
            for (int i = 9; i < environmentList.Length; i++)
            {
                if (i % 2 == 0)
                {
                    rangeValue = 20;
                }
                else
                    rangeValue = -20;
                GameObject stuffSpawn = Instantiate(environmentList[i], spawnPos, Quaternion.Euler(rot)) as GameObject;
                stuffSpawn.transform.DOScale(0.1f/*0.05f*/, 1/*0.5f*/);
                stuffSpawn.GetComponent<TrignometricMovement>().randomDistance = rangeValue;
            }

            StartCoroutine(delayBump());
            numberOfEvent++;
            Debug.Log(numberOfEvent);
        }

        // Use this for initialization
        public void SetSpawnShipModel(GameObject shipPrefab)
        {
            // destroy the current model if it exists
            if (shipModel != null)
                Destroy(shipModel);

            // instantiate a new vehicle model, and parent it to this object
            Vector3 spawnPos = transform.position;
            spawnPos.y = -0.5f;
            shipModel = Instantiate(shipPrefab, spawnPos, transform.rotation) as GameObject;
            shipModel.transform.parent = transform;
        }

        void Start()
        {
            var id = PlayerPrefs.GetInt("id");
            if (MenuManager.isHard)
            {
                line.enabled = false;
            }
            if (isPlaying)
            {
                isShake = false;
                numberOfEvent = 0;
                switch (id)
                {
                    case 1:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if(i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[0];
                        }
                        ColorUtility.TryParseHtmlString("#022371", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#00baff", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[0];
                        break;
                    case 2:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[6];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#6a7790", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[6];
                        break;
                    case 3:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[0];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[8];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#ff7d29", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[8];
                        break;
                    case 4:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[2];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#a3bb00", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[2];
                        break;
                    case 5:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[7];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#9761ff", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[7];
                        break;
                    case 6:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[1];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#a00000", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[1];
                        break;
                    case 7:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                                environmentList[i].GetComponent<Renderer>().material = rockCL[0];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#00b6ff", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[0];
                        break;
                    case 8:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[6];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#b12b8f", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[6];
                        break;
                    case 9:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[1];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#008993", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[1];
                        break;
                    case 10:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[0];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#3E56C9", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[0];
                        break;
                    case 11:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[7];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[6];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#606060", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[6];
                        break;
                    case 12:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[0];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                        }
                        ColorUtility.TryParseHtmlString("#000000", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#000000", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[4];
                        break;
                    case 13:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[6];
                        }
                        ColorUtility.TryParseHtmlString("#181818", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#535456", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[6];
                        break;
                    case 14:
                        ;
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[6];
                        }
                        ColorUtility.TryParseHtmlString("#9196AB", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#9196AB", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[6];
                        break;
                    case 15:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[4];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[5];
                        }
                        ColorUtility.TryParseHtmlString("#181818", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#FFFFFF", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[5];
                        break;
                    default:
                        for (int i = 9; i < environmentList.Length; i++)
                        {
                            if (i > 10)
                                environmentList[i].GetComponent<Renderer>().material = rockCL[0];
                            else
                                environmentList[i].GetComponent<Renderer>().material = rockCL[0];
                        }
                        ColorUtility.TryParseHtmlString("#161616", out color);
                        Camera.main.backgroundColor = color;
                        ColorUtility.TryParseHtmlString("#00123A", out lightColor);
                        VolumetricLight.GetComponent<Light>().color = lightColor;
                        Environment = environmentList[0];
                        break;
                }
                Obstacles.Clear();
                isMusicStart = false;
                pathName = pathList[randomShip].name;
                Debug.Log(pathName);
                eventMove.SetPath(WaypointManager.Paths[pathName]);
                var currentLife = PlayerPrefs.GetInt("life");
                currentLife--;
                PlayerPrefs.SetInt("life", currentLife);
                eventMove.onStart = true;
                eventMove.StartMove();
                Koreographer.Instance.RegisterForEvents(eventID, AddPowerUp);
                audioEvent.Play();
                StartCoroutine(delayStartEvent());

                if (id < 16)
                {
                    koreography = Koreographer.Instance.GetKoreographyAtIndex(0);
                    koreographyTrackBase = koreography.GetTrackByID(eventID);
                    listEvent = koreographyTrackBase.GetAllEvents();
                }
            }
        }

        private void FixedUpdate()
        {
            //if (isTutorial && Input.GetMouseButtonDown(0))
            //{
            //    tutorialText.SetActive(false);
            //    Time.timeScale = 1;
            //    audioCom.UnPause();
            //}
            if(!audioCom.isPlaying && isMusicStart && !isTutorial)
            {
                PlayerControl.PlayerControlInstance.OnGameOver();
            }
            //Debug.Log(numberOfEvent);
            if (numberOfEvent >= 12)
            {
                isShake = true;
            }
            else
            {
                isShake = false;
            }
        }

        IEnumerator delaySpawnLight()
        {
            yield return new WaitForSeconds(1);
            float randomOffset = Random.Range(800, 1000);
            Vector3 spawnPosition = new Vector3(transform.position.x + randomOffset, transform.position.y, transform.position.z + randomOffset);
            GameObject lightObject = Instantiate(VolumetricLight, spawnPosition, VolumetricLight.transform.rotation) as GameObject;
            lightObject.transform.localScale = Vector3.zero;
            lightObject.transform.DOScale(1f, 1f);
            StartCoroutine(delaySpawnLight());
        }

        IEnumerator delayStartEvent()
        {
            yield return new WaitForSeconds(3f);
            playerMove.SetPath(WaypointManager.Paths[pathName]);
            playerMove.onStart = true;
            playerMove.StartMove();
            var id = PlayerPrefs.GetInt("id");
            audioCom.Play();
            if (id > 15)
            {
                MusicPlayer.musicPlayerInstance.PlayCurrent();
            }
            isMusicStart = true;
            StartCoroutine(changeOffSet());
            yield return new WaitForSeconds(1f);
            PlayerControl.isFollowPlayer = true;
        }

        IEnumerator changeSpeedOverTime()
        {
            yield return new WaitForSeconds(1f);
            if (eventMove.speed >= 17.5f)
            {
                eventMove.ChangeSpeed(eventMove.speed -= 0.04f);
                eventMove.speed -= 0.04f;
            }
            StartCoroutine(changeSpeedOverTime());
        }

        IEnumerator changeOffSet()
        {
            if (!isShake)
            {
                yield return new WaitForSeconds(0.5f);
                if (MenuManager.isHard)
                {
                    randomOffsetX = Random.Range(-2, 2);
                    randomOffsetY = Random.Range(-2, 2);
                }
                else
                {
                    if (randomOffsetX >= 1.5f)
                    {
                        randomOffsetX = Random.Range(randomOffsetX - 1.5f, randomOffsetX);
                    }
                    else if (randomOffsetX <= -1.5f)
                    {
                        randomOffsetX = Random.Range(randomOffsetX, randomOffsetX + 1.5f);
                    }
                    else
                    {
                        randomOffsetX = Random.Range(-1.5f, 1.5f);
                    }

                    if (randomOffsetY >= 1.5f)
                    {
                        randomOffsetY = Random.Range(randomOffsetY - 1.5f, randomOffsetY);
                    }
                    else if (randomOffsetY <= -1.5f)
                    {
                        randomOffsetY = Random.Range(randomOffsetY, randomOffsetY + 1.5f);
                    }
                    else
                    {
                        randomOffsetY = Random.Range(-1.5f, 1.5f);
                    }
                }
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
                if (MenuManager.isHard)
                {
                    randomOffsetX = Random.Range(-2, 2);
                    randomOffsetY = Random.Range(-2, 2);
                }
                else
                {
                    if (randomOffsetX >= 1)
                    {
                        randomOffsetX = Random.Range(randomOffsetX - 1f, randomOffsetX);
                    }
                    else if (randomOffsetX <= -1)
                    {
                        randomOffsetX = Random.Range(randomOffsetX, randomOffsetX + 1f);
                    }
                    else
                    {
                        randomOffsetX = Random.Range(randomOffsetX - 1f, randomOffsetX + 1f);
                    }

                    if (randomOffsetY >= 1)
                    {
                        randomOffsetY = Random.Range(randomOffsetY - 1f, randomOffsetY);
                    }
                    else if (randomOffsetY <= -1)
                    {
                        randomOffsetY = Random.Range(randomOffsetY, randomOffsetY + 1f);
                    }
                    else
                    {
                        randomOffsetY = Random.Range(randomOffsetY - 1f, randomOffsetY + 1f);
                    }
                }
            }
            StartCoroutine(changeOffSet());
        }

        //IEnumerator changeOffSetRotate()
        //{
        //    yield return new WaitForSeconds(0.5f);
        //    randomOffsetX = -0.5f;
        //    randomOffsetY = 0.5f;
        //    yield return new WaitForSeconds(0.5f);
        //    randomOffsetX = -0.5f;
        //    randomOffsetY = -0.5f;
        //    yield return new WaitForSeconds(0.5f);
        //    randomOffsetX = 0.5f;
        //    randomOffsetY = -0.5f;
        //    yield return new WaitForSeconds(0.5f);
        //    randomOffsetX = 0.5f;
        //    randomOffsetY = 0.5f;
        //    //isShake = false;
        //    StartCoroutine(changeOffSet());
        //}

        void AddPowerUp(KoreographyEvent evt)
        {
            //if (isTutorial)
            //{
            //    randomOffsetX = -1;
            //    randomOffsetY = 1;
            //    StartCoroutine(waitForInput());
            //}
            //var payloadValue = evt.GetIntValue();
            //Debug.Log(payloadValue);
            Vector3 spawnPosition = new Vector3(transform.position.x + randomOffsetX, transform.position.y + /*UnityEngine.Mathf.Abs(*/randomOffsetY/*)*/, transform.position.z);
            GameObject powerUpObject = Instantiate(powerUp, spawnPosition, transform.rotation) as GameObject;
            powerUpObject.transform.localScale = Vector3.zero;
            powerUpObject.transform.DOScale(maxSize, 3f);

            Vector3 spawnPos = new Vector3(transform.position.x + Random.Range(-5, 5), transform.position.y + Random.Range(-10, 10), transform.position.z + 50); ;
            float rangeValue = 0;

            Vector3 rot = transform.eulerAngles;
            rot.z = Random.Range(0, 360);
            rot.y = Random.Range(0, 360);
            rot.x = Random.Range(0, 360);
            for (int i = 0; i < 2; i++)
            {
                if (i % 2 == 0)
                {
                    rangeValue = 20;
                }
                else
                    rangeValue = -20;
                GameObject environmentSpawn = Instantiate(Environment, spawnPos, Quaternion.Euler(rot)) as GameObject;
                environmentSpawn.GetComponent<TrignometricMovement>().randomDistance = rangeValue;
                float size = Random.Range(1, 10);
                environmentSpawn.transform.DOScale(size/*0.05f*/, 1/*0.5f*/);
                environmentSpawn.transform.DOScale(5f/*0.05f*/, 15/*0.5f*/);
                Obstacles.Add(environmentSpawn);
            }
            for (int i = 9; i < environmentList.Length; i++)
            {
                if (i % 2 == 0)
                {
                    rangeValue = 20;
                }
                else
                    rangeValue = -20;
                GameObject stuffSpawn = Instantiate(environmentList[i], spawnPos, Quaternion.Euler(rot)) as GameObject;
                stuffSpawn.transform.DOScale(1f, 1);
                stuffSpawn.GetComponent<TrignometricMovement>().randomDistance = rangeValue;
            }
            StartCoroutine(delayBump());
            numberOfEvent++;
            //Debug.Log(numberOfEvent);
        }

        IEnumerator delayBump()
        {
            yield return new WaitForSeconds(3);
            foreach(var item in Obstacles)
            {
                if (item != null)
                {
                    item.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 0.5f);
                }
            }
            //Debug.Log(Obstacles.Count);
        }

        IEnumerator waitForInput()
        {
            yield return new WaitForSeconds(2.8f);
            if (tutorialStep < 2)
            {
                tutorialText.SetActive(true);
                if(tutorialStep == 1)
                {
                    randomOffsetX = 0;
                    randomOffsetY = 0;
                    tutorialText.GetComponent<Text>().text = "TO GET PERFECT PERFORM, HIT THE RED ITEM IN THE MIDDLE OF THE BLUE CIRCLE"; 
                }
                else
                    tutorialText.GetComponent<Text>().text = "SWIPE IN ANY DIRECTION TO MOVE AND HIT THE BLUE CIRCLE, YOU CAN ALSO HOLD AND SWIPE";
                tutorialStep++;
                audioCom.Pause();
                Time.timeScale = 0;
                audioCom.Pause();
            }
            else
                isTutorial = false;
        }

        public void ButtonTest()
        {
            var currentScale = maxSize;
            if (currentScale >= 1f)
            {
                currentScale = 0.2f;
            }
            else
            {
                currentScale += 0.1f;
            }
            maxSize = currentScale;
            var button = EventSystem.current.currentSelectedGameObject.gameObject;
            button.GetComponent<Text>().text = "Size:" + currentScale;
        }
    }
}
