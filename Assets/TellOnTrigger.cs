using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TellOnTrigger : MonoBehaviour {

	private void OnTriggerEnter2D(){
		transform.parent.SendMessage("triggerActive");
		GetComponent<BoxCollider2D>().enabled = false;
	}
}
