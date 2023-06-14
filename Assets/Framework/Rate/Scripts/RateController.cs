using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RateController : MonoSingleton<RateController>
{
    #region Const parameters
    #endregion

    #region Editor paramters
    [Header("Object references")]
    [SerializeField]
    private GameObject starsUI;
    [SerializeField]
    private GameObject issuesUI;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private List<Star> stars;

    private string countStr = "Count";
    private int startCount = 0;
    [SerializeField]
    private int showRateButtonAfter = 5;
    #endregion

    #region Normal paramters
    private int curID = -1;
    private bool isGoodRate = true;
    #endregion

    #region Encapsulate
    #endregion

    public void ShowStarsUI()
    {
        starsUI.SetActive(true);
    }

    public void HideStarsUI()
    {
        starsUI.SetActive(false);
    }

    public void ShowIssuesUI()
    {
        issuesUI.SetActive(true);
    }

    public void HideIssuesUI()
    {
        issuesUI.SetActive(false);
    }

    public void Show()
    {
        ShowStarsUI();
        HideIssuesUI();
        animator.SetTrigger("Show");

        // Reset last session
        for (int i = 0; i < stars.Count; i++)
        {
            stars[i].Empty();
        }
        curID = -1;
        isGoodRate = true;
    }

    public void Hide()
    {
        animator.SetTrigger("Hide");
        //Destroy(this.gameObject, 2.0f);
    }

    public void OnStarClick(int nextID)
    {
        if (curID == nextID)
            return;
        else
        {
            StopAllCoroutines();
            StartCoroutine(C_OnStarClick(curID, nextID));
        }
    }

    private IEnumerator C_OnStarClick(int from, int to)
    {
        curID = to;

        WaitForSeconds wait = new WaitForSeconds(0.1f);
        if (from > to)
        {
            for (int i = from; i > to; i--)
            {
                if (i < 0)
                    continue;
                stars[i].Empty();
                yield return wait;
            }
        }
        else
        {
            for (int i = from; i <= to; i++)
            {
                if (i < 0)
                    continue;
                stars[i].Fill();
                yield return wait;
            }
        }

        if (to > 2)
        {
            if (!isGoodRate)
            {
                animator.SetTrigger("GoodRate");
                isGoodRate = true;
            }

            yield return new WaitForSeconds(1.0f);

#if UNITY_ANDROID
            Application.OpenURL(AppInfo.Instance.PLAYSTORE_SHARE_LINK);
#elif UNITY_IOS
            Application.OpenURL(AppInfo.Instance.APPSTORE_SHARE_LINK);
#endif
            Hide();
        }
        else
        {
            if (isGoodRate)
            {
                animator.SetTrigger("BadRate");
                isGoodRate = false;
            }
        }
    }


    //#if UNITY_EDITOR
    public void Start()
    {
        startCount = PlayerPrefs.GetInt(countStr);
        if (PlayerPrefs.GetInt(countStr) >= showRateButtonAfter - 1)
        {
            PlayerPrefs.SetInt(countStr, 0);
            Show();
        }
        else
        {
            PlayerPrefs.SetInt(countStr, ++startCount);
            Hide();
        }
    }
    //#endif
}
