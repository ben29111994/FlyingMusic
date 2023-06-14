using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    #region Const parameters
    #endregion

    #region Editor paramters
    [Header("Object references")]
    [SerializeField]
    private GameObject emptyStar;
    [SerializeField]
    private GameObject fillStar;
    #endregion

    #region Normal paramters
    #endregion

    #region Encapsulate
    #endregion

    public void Fill()
    {
        fillStar.SetActive(true);
    }

    public void Empty()
    {
        fillStar.SetActive(false);
    }
}
