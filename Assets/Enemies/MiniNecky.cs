using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniNecky : MonoBehaviour {

	float realPos;
	float offset = 0;
	bool attack = false;
	bool dead = false;
	Vector3 defaultPosition;
	AudioSource soundEffect;

	void Start(){
		realPos = transform.position.x;
		defaultPosition = transform.position;
		soundEffect = GetComponent<AudioSource>();
	}

	private void OnCollisionEnter2D(Collision2D collision){
		if(collision.gameObject.GetComponent<Player>() != null){
			if(Player.cartWheelTimer > 0){
				StartCoroutine(die());			
				Player.ppp.bounce();
				soundEffect.Play();
				return;
			}
			collision.gameObject.SendMessage("reset" , true);
		}
	}

	//		player has arrive. Swooping attack.
	void triggerActive(){
		if(!attack){
			GetComponent<Animator>().SetBool("Attack" , true);
			soundEffect.pitch = 0.8f;
			soundEffect.Play();
			attack = true;
		}
	}

	//		Jumped on
	void OnTriggerEnter2D(Collider2D collider){
		if(collider.gameObject.GetComponent<Player>() != null){
			StartCoroutine(die());		
			Player.ppp.bounce();
			soundEffect.pitch = 1.1f;
			soundEffect.Play();
		}
	}

	IEnumerator die(){
		GetComponent<Animator>().SetBool("Dead" , true);
		//direction = 0;//		stop moving
		dead = true;
		offset = 0;//		offset is used to control the fall after dieing when not in an attack.
		GetComponents<BoxCollider2D>()[0].enabled = false;
		GetComponents<BoxCollider2D>()[1].enabled = false;
		yield return new WaitForSeconds(0.8f);
		gameObject.SetActive(false);
		RelativityController.deadGO.Add(gameObject);//		add to the list of dead enemies for revive if the player moves backwards far enough in the level
		RelativityController.deadX.Add(defaultPosition.x);
	}

	void Update(){
		if(attack || dead){//		offset is time since attack was started or death. It is used in two seperate equations to offset X and Y.
			if(Menu.showTimeWarp){//				↓ Time dilation ↓
				offset += Time.deltaTime*0.8f / (Mathf.Sqrt(1-Mathf.Abs(Player.lorentz)));
			}else{
				offset += Time.deltaTime*0.8f;
			}
		}

		if(Menu.showLightWarp){//						↓Effects of light taking time to reach player↓
			//		all movement animations are done through the code below instead of through the animator so they can be effected by relativaty.
			if(attack){//		swooping in an attack
				//	X += 1-1/(1+offset).	Y += 0.1/(1+offset)
				realPos -= (1-1/(1 + offset)) * Time.deltaTime*4;
				//		↓effects of light travel times↓
				transform.position = new Vector3(realPos + Player.lorentz*Mathf.Abs(realPos-Player.posx) , transform.position.y - (1.5f / (1 + offset)) * Time.deltaTime, transform.position.z);
				if(realPos + 8 < Player.posx){
					StartCoroutine("die");			
				}
				if(Player.respawining){//	player died, reset to start postion
					respawn();
				}
			}else if(dead){//		fall after dieing while not in an attack
				transform.position = new Vector3(realPos + Player.lorentz*Mathf.Abs(realPos-Player.posx) , transform.position.y - Mathf.Pow(offset*2 , 2)*Time.deltaTime  , transform.position.z);
			}else{//		hovering
				transform.position = new Vector3(realPos + Player.lorentz*Mathf.Abs(realPos-Player.posx) , transform.position.y , transform.position.z);
			}
			transform.localScale = new Vector3(1 + Player.lorentz*Mathf.Abs(realPos+0.5f-Player.posx) - Player.lorentz*Mathf.Abs(realPos-0.5f-Player.posx) , 1 , 1);
		}else{
			if(attack){//		swooping in an attack
				realPos -= (1-1/(1 + offset)) * Time.deltaTime*4;
				transform.position = new Vector3(realPos , transform.position.y - (1.5f / (1 + offset)) * Time.deltaTime, transform.position.z);
				if(realPos + 7 < Player.posx){
					StartCoroutine("die");			
				}
			}else if(dead){
				transform.position = new Vector3(realPos , transform.position.y , transform.position.z);
			}else{
				transform.position = new Vector3(realPos , transform.position.y , transform.position.z);
			}

			transform.localScale = Vector3.one;
		}
	}

	public void respawn(){
		StopCoroutine("die");
		transform.position = defaultPosition;
		GetComponents<BoxCollider2D>()[0].enabled = true;
		GetComponents<BoxCollider2D>()[1].enabled = true;
		GetComponent<Animator>().SetBool("Attack" , false);
		GetComponent<Animator>().SetBool("Dead" , false);
		realPos = defaultPosition.x;
		attack = false;
		dead = false;
		offset = 0;
		transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;//		object only has 1 child (the attack trigger)
	}
}
