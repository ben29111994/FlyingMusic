using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveTop : MonoBehaviour {

	public GameObject content;
	RectTransform rectContent;
	RectTransform rect;
	public float posY;
	public float currentPosY;
	public float currentMyPosY;
	// Use this for initialization
	void Start () {
		rectContent = content.GetComponent<RectTransform> ();
		rect = GetComponent<RectTransform> ();
		currentPosY = rectContent.anchoredPosition.y;
		currentMyPosY = rect.anchoredPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
		posY = rectContent.anchoredPosition.y - currentPosY;
		if (posY >= 0 && posY < 825) {
			rect.anchoredPosition = new Vector2 (rect.anchoredPosition.x, currentMyPosY - posY*0.5f);
		}

	}
}
