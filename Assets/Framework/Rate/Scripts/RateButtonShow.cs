using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RateButtonShow : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		Button btn = gameObject.GetComponent<Button>();
		btn.onClick.AddListener(ShowRateButton);
	}

	public void ShowRateButton()
	{
		RateController.Instance.Show();
		PlayerPrefs.SetInt("Count", 0);
	}
}
