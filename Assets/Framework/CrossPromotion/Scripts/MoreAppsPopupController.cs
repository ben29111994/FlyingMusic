using SRS.CrossPromotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreAppsPopupController : MonoBehaviour
{
    [SerializeField]
    private ItemView popupItemView;
    [SerializeField]
    private bool hasItem = false;

    public bool HasItem
    {
        get
        {
            return hasItem;
        }

        set
        {
            hasItem = value;
        }
    }

    public void SetPopupItem(MoreAppsHandler.ItemContainer itemData)
    {
        popupItemView.btn.onClick.AddListener(itemData.btnAction);
        popupItemView.appName.text = itemData.appName;
        popupItemView.appDescription.text = itemData.appDescription;
        popupItemView.icon.sprite = itemData.iconSprite;
        popupItemView.screenshot.sprite = itemData.screenshotSprite;
        popupItemView.screenshot.preserveAspect = true;

        hasItem = true;
    }

}
