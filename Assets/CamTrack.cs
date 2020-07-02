using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrack : MonoBehaviour {

	public Transform target;
	public Transform background;
	public Transform forground;
	public float leftLimit = -101.6f;
	public float rightLimit = 102.7f;
	static public float camSize = 4;
	Transform player;

	void Start(){
		player = Player.ppp.transform;
		if(leftLimit > rightLimit){
			Debug.Log("CAMERA LIMITS ARE REVERSED.\nClick Challenge Mode for game lockup.");
		}
	}

	void Update () {
		//		close camera must also change Y coordinate. 100% camera only changes X.
		if(Menu.closeCamera){
			transform.position = new Vector3(transform.position.x , Mathf.Clamp(transform.position.y + (player.position.y - transform.position.y)*Time.deltaTime*6 , camSize-4 , 4-camSize) , transform.position.z);
		}

		//		center just to the right of the character and shift to show where they are moving to
		transform.position += transform.right * (target.position.x - transform.position.x + Mathf.Clamp(2 , -6 , 6)) * Time.deltaTime*6;
		if(transform.position.x > rightLimit + camSize)
			transform.position =  new Vector3(rightLimit + camSize , transform.position.y , transform.position.z);
		if(transform.position.x < leftLimit - camSize)
			transform.position =  new Vector3(leftLimit - camSize , transform.position.y , transform.position.z);
		
		//		move background and forground with camera for paralax effect
		background.position = new Vector3(target.transform.position.x*0.4f , background.position.y , background.position.z);
		//		move the background in the opposite direction of the camera
		forground.position = new Vector3(-target.transform.position.x*0.4f , background.position.y , background.position.z);
		//		move the forground in the same direction as the camera
	}
}
