using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorWarp : MonoBehaviour {

	Material mat;
	float x;

	void Start(){
		mat = GetComponent<SpriteRenderer>().material;
		//mat = GetComponent<MeshRenderer>().material;
	}
	
	void Update(){
		//		0 = not moving
		//		1 = player moving twards object at light speed
		//		-1 = player away from object at light speed
		x = Mathf.Sign(transform.position.x - Player.posx) * Player.lorentz;
		mat.color = new Color(1-Mathf.Max(x,0) , 1 - Mathf.Abs(x) , 1-Mathf.Max(-x,0) ,1);
	}
}
