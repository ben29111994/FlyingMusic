using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
using SRS.CrossPromotion;


[RequireComponent(typeof(MoreAppsScrollController))]
public class MoreAppsHandler : MonoBehaviour {

    #region Declarations and constants

    private const string KEY_VERSION        = "Version";
    private const string KEY_APPLIST        = "AppsList";
    private const string KEY_IOS            = "-iOS";
    private const string KEY_ANDROID        = "-Android";

    public class ItemContainer
    {
        public Sprite iconSprite;
        public Sprite screenshotSprite;
        public string appName;
        public string appDescription;
        public UnityEngine.Events.UnityAction btnAction;
    }

    private class AppItem
    {
        public string AppTitle  { get; set; }
        public string AppDescription { get; set; }
        public string IconUrl   { get; set; }
        public string ScreenshotUrl { get; set; }
        public string Id        { get; set; }
        public bool Visible { get; set; }
        public bool ShouldShowPopup { get; set; }

        public AppItem()
        {

        }

        public void Print()
        {
            Debug.Log("Title: " + AppTitle +  ", icon url: " + IconUrl + ", id: " + Id);
        }
    }

    private class MoreAppsDescriptor
    {
        public int              AppsCount   { get; set; }
        public int              Version     { get; set; }
        public List<AppItem>    Items       { get; set; }

        public MoreAppsDescriptor()
        {
            Items = new List<AppItem>();
        }

        public void VersionInfo()
        {
            Debug.Log("Version: " + Version + ", AppsCount: " + AppsCount);
        }
    }

    private class CoroutineWithData
    {
        public Coroutine    coroutine { get; private set; }
        public object       result;
        private IEnumerator target;

        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            this.target     = target;
            this.coroutine  = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (target.MoveNext())
            {
                result = target.Current;
                yield return result;
            }
        }
    }

    #endregion

    #region Vars
    public bool                 logOn           = false;

    //[SerializeField]
    //private Button              _moreAppsBtn;
    [SerializeField]
    private float delayInit = 2;

    public string               configFileUrl;

    private MoreAppsDescriptor  _currentDesc    = null;
    private static bool         _updatesChecked = false;

    private bool isComplete = false;
    #endregion

    public bool IsComplete
    {
        get
        {
            return isComplete;
        }

        set
        {
            isComplete = value;
        }
    }

    #region Utils

    private static AppItem parseItem(JSONNode node)
    {
        AppItem result = new AppItem();

        result.AppTitle = node["AppTitle"];
        result.AppDescription = node["AppDescription"];
        result.IconUrl  = node["IconUrl"];
        result.Id       = node["Id"];
        result.ScreenshotUrl = node["ScreenshotUrl"];
        result.Visible = node["Visible"].AsBool;
        result.ShouldShowPopup = node["ShouldShowPopup"].AsBool;

        return result;
    }

    private static MoreAppsDescriptor parseResponse(string json)
    {
        print(json);
#if UNITY_IOS
        string platformPref = KEY_IOS;
#else
        string platformPref = KEY_ANDROID;
#endif

        try
        {
            MoreAppsDescriptor result   = new MoreAppsDescriptor();
            JSONNode rootNode           = JSON.Parse(json);
            result.Version              = rootNode[KEY_VERSION].AsInt;
            JSONArray appsArray         = rootNode[KEY_APPLIST + platformPref].AsArray;
            result.AppsCount            = appsArray.Count;

            for (int i = 0; i < appsArray.Count; ++i)
            {
                AppItem item = parseItem(appsArray[i]);
                result.Items.Add(item);
            }

            return result;

        } catch(System.Exception e)
        {
            Debug.LogError("MoreAppsPage: Json parse error");
        }

        return null;
    }

    private IEnumerator DownloadJsonDescriptor()
    {
        yield return null;
        WWW www = new WWW(configFileUrl);
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("MoreAppsPage: WWW error: " + www.error);
        } else
        {
            if (logOn)
            {
                Debug.Log("MoreAppsPage: Descriptor downloaded");
            }

            yield return StartCoroutine(updateMoreAppsPage(www.text));
        }
    }

    private UnityEngine.Events.UnityAction getBtnAction(string id)
    {
        return () => {
#if UNITY_EDITOR
            string link = "https://play.google.com/store/apps/details?id=" + id;
#elif UNITY_ANDROID
            string link = "market://details?id=" + id;
#elif UNITY_IOS
            string link = "itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?type=Purple+Software&id=" + id;
#endif
            Application.OpenURL(link);
        };
    }

    private void checkForUpdates()
    {
        _updatesChecked = true;
        StartCoroutine(DownloadJsonDescriptor());
    }

    private void loadConfig()
    {
        print("Loading setting from " + Utils.GetSettingsFilePath());
        string oldFile  = File.ReadAllText(Utils.GetSettingsFilePath());
        _currentDesc    = parseResponse(oldFile);
    }

    #endregion

    private IEnumerator updateMoreAppsPage(string responseJson)
    {
        yield return null;
        MoreAppsDescriptor desc = parseResponse(responseJson);

        if (desc == null)
        {
            Debug.LogError("MoreAppsPage: Error parsing downloaded descriptor file");
            yield break;
        }

        if (Utils.ConfigFileExists())
        {
           
            if (desc.Version != _currentDesc.Version)
            {
                // remove old images that wont be replaced
                for(int i = 0; i < _currentDesc.AppsCount; ++i)
                {
                    string imagePath = Utils.GetIconPath(i);
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }

                File.WriteAllText(Utils.GetSettingsFilePath(), responseJson);
                _currentDesc = desc;

                yield return StartCoroutine(updateUI(forceReplace: true));

            } else
            {
                if (logOn)
                {
                    Debug.LogFormat("MoreAppsPage: Downloaded version is not new. Current version: {0}, Downloaded Version: {1}", _currentDesc.Version, desc.Version);
                    isComplete = true;
                } 
            }

        } else
        {
            if (logOn)
            {
                Debug.Log("MoreAppsPage: First time file load");
            }

            File.WriteAllText(Utils.GetSettingsFilePath(), responseJson);
            _currentDesc = desc;

            yield return StartCoroutine(updateUI());
        }

    }

    private IEnumerator updateUI(bool forceReplace = false)
    {
        if (_currentDesc == null)
        {
            Debug.LogError("MoreAppsPage: desciptor is NULL");
            yield break;
        }

        bool listEmpty = false;

        yield return null;
        MoreAppsScrollController scrollViewController = GetComponent<MoreAppsScrollController>();
        MoreAppsPopupController popupController = GetComponent<MoreAppsPopupController>();

        yield return null;
        for (int i = 0; i < _currentDesc.AppsCount; ++i)
        {
            if (_currentDesc.Items[i].Visible) {
                if (!File.Exists(Utils.GetIconPath(i)) || forceReplace) {
                    CoroutineWithData cdIcon = new CoroutineWithData(this, downloadIcon(i));
                    yield return cdIcon.coroutine;

                    bool success = (bool)cdIcon.result;
                    if (!success) {
                        Debug.LogError("MoreAppsPage: Failed to load icon");
                        continue;
                    }
                }

                if (!File.Exists(Utils.GetScreenshotPath(i)) || forceReplace) {
                    CoroutineWithData cdScreenshot = new CoroutineWithData(this, downloadScreenshot(i));
                    yield return cdScreenshot.coroutine;

                    bool success = (bool)cdScreenshot.result;
                    if (!success) {
                        Debug.LogError("MoreAppsPage: Failed to load screenshot");
                        continue;
                    }
                }

                if (!listEmpty) {
                    // one time lazy clear
                    scrollViewController.ClearList();
                    listEmpty = true;
                }

                ItemContainer itemData = new ItemContainer();
                itemData.appName = _currentDesc.Items[i].AppTitle;
                itemData.appDescription = _currentDesc.Items[i].AppDescription;
                string id = _currentDesc.Items[i].Id;
                itemData.btnAction = getBtnAction(id);
                itemData.iconSprite = Utils.GetIconSprite(i);
                itemData.screenshotSprite = Utils.GetScreenshotSprite(i);

                scrollViewController.AddItem(itemData);

                if (!popupController.HasItem && _currentDesc.Items[i].ShouldShowPopup)
                    popupController.SetPopupItem(itemData);

                yield return null;
                yield return null;
            }
        }

        if (scrollViewController.ItemCount > 0)
        {
            CrossPromotionController.Instance.ShowMoreGameButton();
            CrossPromotionController.Instance.IsCompleted = true;
        }


        if (!forceReplace && !_updatesChecked)
        {
            checkForUpdates();
        }

        isComplete = forceReplace;
    }

    private void loadStaticItems()
    {
        ItemLoader loader = GetComponent<ItemLoader>();
        if (loader != null)
        {
            MoreAppsScrollController scrollViewController = GetComponent<MoreAppsScrollController>();
#if UNITY_IOS
            ItemLoader.ItemElement[] itemArray = loader.IosApps;
#else
            ItemLoader.ItemElement[] itemArray = loader.AndroidApps;
#endif

            foreach (ItemLoader.ItemElement e in itemArray)
            {
                ItemContainer itemData  = new ItemContainer();
                itemData.iconSprite         = e.AppIcon;
                itemData.appName        = e.AppName;
                itemData.btnAction      = getBtnAction(e.AppId);

                scrollViewController.AddItem(itemData);
            }

            if (scrollViewController.ItemCount > 0)
            {
                CrossPromotionController.Instance.ShowMoreGameButton();
            }

            Destroy(loader, 0.1f);
        }
    }

    private IEnumerator downloadIcon(int index)
    {
        WWW www = new WWW(_currentDesc.Items[index].IconUrl);
        yield return www;


        if (www.error != null)
        {
            Debug.LogError("MoreAppsPage: WWW error: " + www.error);
            yield return false;
        } else
        {
            File.WriteAllBytes(Utils.GetIconPath(index), www.bytes);
            yield return true;
        }
    }

    private IEnumerator downloadScreenshot (int index) {
        WWW www = new WWW(_currentDesc.Items[index].ScreenshotUrl);
        yield return www;


        if (www.error != null) {
            Debug.LogError("MoreAppsPage: WWW error: " + www.error);
            yield return false;
        }
        else {
            File.WriteAllBytes(Utils.GetScreenshotPath(index), www.bytes);
            yield return true;
        }
    }

    #region Unity methods

    IEnumerator Start ()
    {        
        //_moreAppsBtn.gameObject.SetActive(false);
        //print("H");
        yield return new WaitForSeconds(delayInit);
        //print("K");
        //print("T");
        //loadStaticItems();

        if (Utils.ConfigFileExists())
        {
            loadConfig();
            if (_currentDesc == null) {
                checkForUpdates();
            }
            else {
                StartCoroutine(updateUI());
            }
        } else
        {
            checkForUpdates();
        }

        if (logOn)
        {
            Debug.Log("MoreAppsPage: data path: " + Utils.GetSettingsFilePath());
        }
        
    }
	

    #endregion
}
