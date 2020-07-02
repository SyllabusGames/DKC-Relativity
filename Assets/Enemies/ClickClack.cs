using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickClack : MonoBehaviour {

	int direction = -1;
	float realPos;
	float offset = 0;
	public float leftLimit = -1;
	public float rightLimit = 1;
	Vector3 defaultPosition;
	AudioSource soundEffect;

	void Start(){
		realPos = transform.position.x;
		defaultPosition = transform.position;
		soundEffect = GetComponent<AudioSource>();
	}


	void OnCollisionEnter2D(Collision2D collision){
		if(collision.gameObject.GetComponent<Player>() != null){
			if(Player.cartWheelTimer > 0){
				StartCoroutine(die());			
				return;
			}
			collision.gameObject.SendMessage("reset" , true);
		}
	}

	//		Jumped on
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.GetComponent<Player>() != null){
			StartCoroutine(die());		
		}
	}

	IEnumerator die(){
		GetComponent<Animator>().SetBool("Dead" , true);
		//GetComponent<AudioSource>().loop = false;
		//GetComponent<AudioSource>().clip = Menu.menu.clickClackDie;
		soundEffect.PlayOneShot(Menu.menu.clickClackDie);
		offset = 0;//		don't hit any walls
		Player.ppp.bounce();
		//		turn off the player damage collider and take damage trigger
		GetComponents<BoxCollider2D>()[0].enabled = false;
		GetComponents<BoxCollider2D>()[1].enabled = false;
		yield return new WaitForSeconds(0.83f);
		gameObject.SetActive(false);
		RelativityController.deadGO.Add(gameObject);//		add to the list of dead enemies for revive if the player moves backwards far enough in the level
		RelativityController.deadX.Add(defaultPosition.x);
	}

	void Update(){
		if(Menu.showTimeWarp){//							↓Time dilation↓
			realPos += direction*Time.deltaTime * 1/(Mathf.Sqrt(1-Mathf.Abs(Player.lorentz)));
			offset +=  direction*Time.deltaTime * 1/(Mathf.Sqrt(1-Mathf.Abs(Player.lorentz)));
		}else{
			realPos += direction*Time.deltaTime;
			offset +=  direction*Time.deltaTime;
		}

		if(Menu.showLightWarp){//						↓Effects of light taking time to reach player↓
			transform.position = new Vector3(realPos + Player.lorentz*Mathf.Abs(realPos-Player.posx) , transform.position.y , transform.position.z);
		}else{
			transform.position = new Vector3(realPos , transform.position.y , transform.position.z);
		}

		if(offset > rightLimit)
			direction = -1;
		if(offset < leftLimit)
			direction = 1;

		if(Menu.showLightWarp)
			transform.localScale = new Vector3(-direction*(1 + Player.lorentz*Mathf.Abs(realPos+0.5f-Player.posx) - Player.lorentz*Mathf.Abs(realPos-0.5f-Player.posx)) , 1 , 1);
		else
			transform.localScale = new Vector3(-direction , 1 , 1);
	}

	void OnDrawGizmosSelected(){//		show path of travel in edditor
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position - transform.up*0.2f + transform.right * leftLimit , transform.position - transform.up*0.2f + transform.right * rightLimit);
		
    }
	
	
	public void respawn(){
		transform.position = defaultPosition;
		GetComponents<BoxCollider2D>()[0].enabled = true;
		GetComponents<BoxCollider2D>()[1].enabled = true;
		realPos = defaultPosition.x;
		offset = 0;
		direction = -1;
		transform.localScale = Vector3.one;
	}
}
