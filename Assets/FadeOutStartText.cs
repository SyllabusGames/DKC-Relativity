using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutStartText : MonoBehaviour {


	void Update(){
		if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f)
			Destroy(gameObject , 1);
	}
}
