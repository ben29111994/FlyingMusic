using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBaseDreamIO : MonoBehaviour {

	private const string DreamloURL    = "http://dreamlo.com/lb/";
	public const string PrivateCodeURL = "b0d29lGFqUONn69YGOTtYw5brOS_Avu0ahMCoomyMqsw";
	public const string PublicCodeURL  = "5b35f9cd191a8a0bccd02ac6";

	public int score;
	public string scoretostring;
	public string myname;
	void Start(){
		GetScore (myname);
	}

	void Update(){
		if (Input.GetMouseButtonDown (1)) {
			score++;
		
			SetScore (myname, score);
		}
	}

	void GetScore(string name){
		StartCoroutine (WaitForGetScore (name));

	}

	void SetScore(string name,int totalScore){
		StartCoroutine (WaitForSetScore (name,totalScore));
	}

	IEnumerator WaitForGetScore(string name){
		WWW www = new WWW(DreamloURL +  PublicCodeURL  + "/pipe-get/" + WWW.EscapeURL(name));
		yield return www;
		scoretostring = www.text;
		string[] array = scoretostring.Split ('|');
		score = int.Parse(array [1]);
	}

	IEnumerator WaitForSetScore(string name,int totalScore){
		WWW www = new WWW(DreamloURL + PrivateCodeURL + "/add-pipe/" + WWW.EscapeURL(name) + "/" + totalScore.ToString());
		yield return www;
		scoretostring = www.text;


	}
}
