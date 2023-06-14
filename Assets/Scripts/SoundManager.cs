using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;

	public AudioSource audioSource;
	public AudioClip win, star, lose, collect, invalid;	



	void Awake(){
		instance = this;
		audioSource = GetComponent<AudioSource> ();
	}

	public void PlaySound(AudioClip ac){
		audioSource.PlayOneShot (ac);
	}
}
