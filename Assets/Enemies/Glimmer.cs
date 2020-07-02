using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glimmer : MonoBehaviour {

	int direction = -1;
	public float realPos;
	float offset = 0;
	
	Material mat;
	float x;

	void Start(){
		realPos = transform.position.x;
		offset = Random.Range(-1.9f , 1.9f);
		direction = Random.Range(0,1)*2-1;//		randomly either -1 or 1
		mat = GetComponent<SpriteRenderer>().material;
	}
	
	void Update(){
		//		↓effects of light travel times↓			↓Time dilation↓
		realPos += direction*Time.deltaTime*0.35f * 1/(Mathf.Sqrt(1-Player.lorentz));
		offset +=  direction*Time.deltaTime*0.35f * 1/(Mathf.Sqrt(1-Player.lorentz));
		//realPos = current position + velocity * 1/(sqrt(1-(v/c)^2))
		if(offset > 2){
			direction = -1;
			transform.localScale = new Vector3(-direction , 1 , 1);
		}
		if(offset < -2){
			direction = 1;
			transform.localScale = new Vector3(-direction , 1 , 1);
		}

		if(Menu.showLightWarp)
			transform.position = new Vector3(realPos + Player.lorentz*Mathf.Abs(realPos-Player.posx) , transform.position.y + Mathf.Sin(Time.time)*0.1f*Time.deltaTime , transform.position.z);
		else
			transform.position = new Vector3(realPos , transform.position.y + Mathf.Sin(Time.time)*0.1f*Time.deltaTime , transform.position.z);
		
		x = Mathf.Sign(transform.position.x - Player.posx) * Player.lorentz;
		mat.color = new Color(1-Mathf.Max(x,0) , 1 - Mathf.Abs(x) , 1-Mathf.Max(-x,0) ,1);
	}
}
