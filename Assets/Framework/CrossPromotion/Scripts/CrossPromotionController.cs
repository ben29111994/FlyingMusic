using SRS.CrossPromotion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CrossPromotionController : MonoSingleton<CrossPromotionController> {
    #region Const parameters
    #endregion

    #region Editor paramters
    [Header("UI Config")]
    [SerializeField]
    private float delayBeforeAutoShowPopup = 2f;
    [SerializeField]
    private bool useDefaultMoreGameButton = true;

    [Header("Object references")]
    [SerializeField]
    private GameObject popupUI;
    [SerializeField]
    private GameObject moreGamesUI;
    [SerializeField]
    private Button btnMoreGames;

    [SerializeField]
    private MoreAppsHandler moreAppsHandler;
    [SerializeField]
    private MoreAppsScrollController moreAppsScrollController;
    [SerializeField]
    private MoreAppsPopupController moreAppsPopupController;
    #endregion

    #region Normal paramters
    private bool canShow = true;
    private bool isCompleted = false;
    private WaitForSeconds waitOneSecond = new WaitForSeconds(1);
    #endregion

    #region Encapsulate
    public bool CanShow {
        get {
            return canShow;
        }

        set {
            canShow = value;
        }
    }

    public bool ShouldUseDefaultMoreGameButton {
        get { return useDefaultMoreGameButton; }
    }

    public bool IsCompleted {
        get {
            return isCompleted;
        }

        set {
            isCompleted = value;
        }
    }
    #endregion

    public void ShowPopupUI () {
        popupUI.SetActive(true);
        if (useDefaultMoreGameButton)
            btnMoreGames.gameObject.SetActive(false);
    }

    public void HidePopupUI () {
        popupUI.SetActive(false);

        if (moreAppsHandler.IsComplete)
            ShowMoreGameButton();
    }

    public void ShowMoreGameButton () {
        if (useDefaultMoreGameButton && canShow && !popupUI.activeSelf)
            btnMoreGames.gameObject.SetActive(true);
    }

    public void HideMoreGameButton () {
        if (useDefaultMoreGameButton)
            btnMoreGames.gameObject.SetActive(false);
    }

    public void ShowMoreGamesUI () {
        moreGamesUI.SetActive(true);
    }

    public void HideMoreGamesUI () {
        moreGamesUI.SetActive(false);
    }

    public IEnumerator C_CheckPopupUI () {
        yield return null;
        float curTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup - curTime < delayBeforeAutoShowPopup) {
            yield return null;
        }

        //force to show popup
        while (true) {
            yield return waitOneSecond;
            //only show popup for 1 time
            if (moreAppsPopupController.HasItem) {
                ShowPopupUI();
                break;
            }
        }
    }

    public void CheckPopupUI () {
        StartCoroutine(C_CheckPopupUI());
    }

    public void Awake () {
        StartCoroutine(C_CheckPopupUI());
    }

    public void Active () {
        if (isCompleted)
            if (useDefaultMoreGameButton)
                btnMoreGames.gameObject.SetActive(true);
        canShow = true;
    }

    public void Deactive () {
        if (useDefaultMoreGameButton)
            btnMoreGames.gameObject.SetActive(false);
        canShow = false;
    }

}
