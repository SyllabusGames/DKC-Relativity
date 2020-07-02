using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	Rigidbody2D rig;
	static public float posx;//		player position.x used by many other scripts for calculating relativistic effects
	static public float velx;//		velocity allong x, updated every frame
	static public float lorentz;//		Lorentz transformation used to offset things to show relativity
	static public float delx;//		=velx except when time or light dilation are activated
	static public Player ppp;//		used by other scripts to get player
	float direction = 0;

	Animator anim;
	//	NOTE: in the animator, I set all the Transition Durations to 0 so every state can be switched to instantly. Not doing this produced a delay before an animation would start. This took a while to figure out.
	AudioSource soundEffect;

	static public float cartWheelTimer = -1;
	static public float C = 7;
	static public bool lightTravelsAtC = true;

	RaycastHit2D respawnPoint;
	static public bool respawining = false;//		when miniNeckys see this true, they stop their attach and reset
	bool grounded = false;
	float startXPos;//		the starting x position. The player should never spawn left of this.

	float horizontalInput;

	void Awake () {
		rig = GetComponent<Rigidbody2D>();//		get rigidbody to make things more efficient
		ppp = this;//		other scripts will need to access this one
		anim = GetComponent<Animator>();
		soundEffect = GetComponent<AudioSource>();
		startXPos = transform.position.x;
	}


	void Update(){
		if(respawining)
			return;
		if(cartWheelTimer > 0 || Physics2D.OverlapBox((Vector2)transform.position + Vector2.down*0.23f , new Vector2(0.6f , 0.13f) , 0 , 1)){//		on ground or doing cartWheel
			if(rig.velocity.y < 0.1f)//		if you are falling and about to hit the ground
				anim.SetBool("Jump" , false);
			if(Input.GetButtonDown("Run") && cartWheelTimer < 0.2f){//		start a cartWheel if on ground and not already doing one
				anim.SetBool("Cartwheel" , true);
				cartWheelTimer = 0.7f;
			}
			if(Input.GetButtonDown("Jump")){
				rig.velocity = new Vector2(rig.velocity.x , 5);
				anim.SetBool("Jump" , true);
				anim.SetBool("Cartwheel" , false);
				cartWheelTimer = -1;
			}
			grounded = true;
		}else
			grounded = false;

		horizontalInput = Input.GetAxis("Horizontal") + Mathf.Clamp(Mathf.Round(Input.acceleration.y/3) , -1 , 1);//		Input.acceleration is the gyroscope and it ranges from -10 to 10

		//		when in a cartWheel, you have to move at a fixed speed and the only way to cancel is to jump.
		if(cartWheelTimer > 0){
			cartWheelTimer -= Time.deltaTime;
			if(Mathf.Abs(horizontalInput) > 0.1f){//		pushing in a direction
				direction = Mathf.Sign(horizontalInput) * 5;
			}else{
				direction = Mathf.Sign(direction) * 5;
			}
			rig.velocity = new Vector2(direction , rig.velocity.y);
		}else{
			anim.SetBool("Cartwheel" , false);
			//		use running speed if holding down the run button and are on the ground or have not let go of run button since leaving ground
			if(Input.GetButton("Run") && grounded){
				direction = Mathf.Clamp(direction + horizontalInput*Time.deltaTime*40 , -4.7f , 4.7f);
				rig.velocity = new Vector2(direction , rig.velocity.y);
				if(Mathf.Abs(rig.velocity.x) < 0.1f){
					anim.SetBool("Walk" , false);
					anim.SetBool("Run" , false);
				}else{
					anim.SetBool("Run" , true);
				}
			}else{
				//		this will give a very quick, but not instant ramp up to max speed (3m/s), allowing for fine controll when on small platforms but negligable build up time when sprinting
				direction = Mathf.Clamp(direction + horizontalInput*Time.deltaTime*30 , -2.7f , 2.7f);
				rig.velocity = new Vector2(direction , rig.velocity.y);
				anim.SetBool("Run" , false);
				if(Mathf.Abs(rig.velocity.x) < 0.5f){
					anim.SetBool("Walk" , false);
				}else{
					anim.SetBool("Walk" , true);
				}
			}
		}

		posx = transform.position.x;
		velx = rig.velocity.x;
		lorentz = Mathf.Sign(velx) * (1 - Mathf.Sqrt(1 - (velx * velx) / (C * C)));
		lorentz = velx / C;
		//		face the direction you are moving
		if(Mathf.Abs(horizontalInput) > 0.1f){
			transform.localScale = new Vector3(Mathf.Sign(horizontalInput) , 1 , 1);
		}else{//		not trying to move, slow to a stop
			if(grounded)
				direction *= 0.7f;
			else
				direction *= 0.92f;
		}
	}

	public void bounce(){
		if(Input.GetButton("Jump")){//	if holding jump, bounce high
			rig.velocity = new Vector2(rig.velocity.x , 5.5f);
		}else if(rig.velocity.y < 3.8f){//	if not already traveling up faster, do small bounce
			rig.velocity = new Vector2(rig.velocity.x , 3.8f);
		}
		//		continue the cartwheel if you are killing things with it
		if(cartWheelTimer > 0)
			cartWheelTimer = Mathf.Min( 0.7f , cartWheelTimer + 0.15f);
	}

	public void reset(bool wasHit){
		if(wasHit)//		if this was false, the thing that killed you will play a sound
			soundEffect.Play();
		rig.velocity = new Vector2(0 , 8.5f);
		//		circle cast from above so you will be most likely to respawn 7m back or at the highest platform near that position (the docks)
		if(Menu.showLightWarp)//																					↓the offset from the player is 7. No need to calculate it.
			respawnPoint = Physics2D.CircleCast((Vector2)transform.position + Vector2.left*7 + Vector2.right*lorentz*7 + Vector2.up*30 , 14 , Vector2.down , 50 , 1);
		else
			respawnPoint = Physics2D.CircleCast((Vector2)transform.position + Vector2.left*7 + Vector2.up*30 , 14 , Vector2.down , 50 , 1);

		if(transform.position.x > -100){
			//		if there is land < 16m behind me (there alwasys is) spawn right beneath it, jumping out of the bog
			if(respawnPoint != null){
				//		invert relativistic effects to spawn on the platform in the location it will be when you get there instead of where it is now
				transform.position = new Vector3(Mathf.Max(respawnPoint.point.x - lorentz*Mathf.Abs(respawnPoint.point.x-posx) , startXPos) , -3f , transform.position.z);
				respawining = true;
			}else{
				transform.position = new Vector3(Mathf.Max(transform.position.x - 10  , startXPos) , -2.5f , transform.position.z);
			}
		}else{//		if you fall off the back of the level, reset to -106 (your spawn point)
			transform.position = new Vector3(startXPos , -2.5f , transform.position.z);
		}
		//		the variable (respawining) stops these from being updated in addition to stopping play for 1 second
		velx = 0;
		posx = transform.position.x;
		cartWheelTimer = 1;
		Invoke("notRespawning" , 0.5f);
	}

	void notRespawning(){
		respawining = false;
		cartWheelTimer = -1;
	}
}
