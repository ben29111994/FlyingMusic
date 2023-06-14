using UnityEngine;

[AddComponentMenu("More Apps/Item Loader")]
public class ItemLoader : MonoBehaviour
{

    [System.Serializable]
    public class ItemElement
    {
        public Sprite AppIcon;
        public Sprite AppScreenshot;
        public string AppDescription;
        public string AppName;
        public string AppId;
    }

    public ItemElement[] AndroidApps;
    public ItemElement[] IosApps;


}