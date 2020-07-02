using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBox : MonoBehaviour {

	void OnTriggerEnter2D(){
		Player.ppp.reset(false);
		GetComponent<AudioSource>().Play();
	}
}
