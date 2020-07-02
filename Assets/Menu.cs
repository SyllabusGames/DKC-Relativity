using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour{

	//			most functions here are called from the Canvas / event manager


	//		for use in other scripts
	static public Menu menu;

	//		changing the speed of light
	public Text CText;
	public Slider CSpeedSlider;
	
	//		changing camera size
	public Slider cameraSlider;
	public Text cameraSizeText;
	static public bool closeCamera = true;

	//		chaning how the speed of light is seen (C / instant)
	public Text visualSpeedText;
	public Toggle lightWarpToggle;
	static public bool showLightWarp = true;

	//		show effects of relativity on time
	public Toggle timeWarpToggle;
	static public bool showTimeWarp = true;

	//		color shift
	public Toggle colorShiftToggle;
	[HideInInspector]
	public bool colorShiftActive = true;
	ColorWarp[] allColorWarps;

	//		challenge mode
	public GameObject glimmer;
	Transform glimmerParent;
	Transform[] glimmers = new Transform[50];
	Glimmer[] glimmersAi = new Glimmer[50];

	bool paused = false;
	GameObject canvas;


	public Slider gameSpeedSlider;
	public Text gameSpeedText;


	//		temporary functisets
	int i;

	//		audio clips stored here so each instance does not have to keep up with its own refrence
	public AudioClip clickClackDie;

	void Start(){
		menu = this;
		canvas = transform.GetChild(0).gameObject;
		canvas.SetActive(false);
		//		set camera size slider to what is was in the last level
		cameraSlider.value = (CamTrack.camSize - 2) * 10 / 2;
	}

	void Update(){
		if(Input.GetButtonDown("Submit")){
			toggleMenu();
		}
	}

	public void setGameSpeed(float sss){
		gameSpeedText.text = "Game Speed (difficulty): " + Mathf.Round(gameSpeedSlider.value*100).ToString() + "%";
	}

	public void toggleMenu(){
		paused = !paused;
		canvas.SetActive(paused);
		if(paused){
			Time.timeScale = 0;
		}else{
			Time.timeScale = gameSpeedSlider.value;
		}
	}

	public void setLightSpeed(float c){
		CText.text = "Speed of light = " + c.ToString("0.0") + "m/s";
		Player.C = c;
	}

	public void cameraSize(float size){
		cameraSizeText.text = "Cameara Size: " + (50+size*5).ToString() + "% background height";
		if(size > 95){
			Camera.main.orthographicSize = 4;
			closeCamera = false;
		}else{
			Camera.main.orthographicSize = 2 + 2*size/10;
			closeCamera = true;
		}
		CamTrack.camSize = Camera.main.orthographicSize;
	}

	//		turn all colorWarp compsetents set or off.
	public void colorShift(bool csa){
		colorShiftActive = csa;
		if(allColorWarps.Length == 0){
			allColorWarps = GameObject.FindObjectsOfType<ColorWarp>();
		}
		
		for(i = allColorWarps.Length-1 ; i > -1 ; i--){
			allColorWarps[i].enabled = colorShiftActive;
		}
	}

	public void timeDilation(bool set){
		showTimeWarp = set;
	}
	
	public void musicToggle(bool set){
		if(set)
			GetComponent<AudioSource>().Play();
		else
			GetComponent<AudioSource>().Pause();
	}

	public void lightWarp(bool set){
		//		this just sets the players speed to always appear to be 0 to remove the effects of slow light travel
		//Player.lightTravelsAtC = set;
		showLightWarp = set;
		if(set){
			visualSpeedText.text = "Visual light travels: at the speed of light";
		}else{
			visualSpeedText.text = "Visual light travels: instantly";
			RelativityController.RC.turnOffLightWarp();
		}
	}

	public void challengeMode(bool set){
		Player.ppp.GetComponent<SpriteRenderer>().enabled = !set;
		if(glimmerParent == null){
			glimmerParent = new GameObject("GlimmerArmy").transform;
			float left = Player.posx - 12;
			for(i = 0 ; i < 50 ; i++){//		spawn 50 glimmers
				left += Random.Range(0.3f , 0.7f);
				//			spawn glimmers randomly allong the bottom of the screen		↓50/50 chance they spawn in the sky↓
				glimmers[i] = Instantiate(glimmer , new Vector3(left , Random.Range(-2.5f , -3.3f) + ((Random.value>0.5f)?0:5) , -2) , Quaternion.identity).transform;
				glimmersAi[i] = glimmers[i].GetComponent<Glimmer>();
				glimmers[i].parent = glimmerParent;
			}
		}else{
			glimmerParent.gameObject.SetActive(set);
		}
		
		if(set){
			colorShiftToggle.isOn = true;
			lightWarpToggle.isOn = false;
			timeWarpToggle.isOn = true;
			CSpeedSlider.value = 5.1f;
			cameraSlider.value = 10;
			gameSpeedSlider.value = 1;
			StartCoroutine("moveGlimmers");
		}else{
			StopCoroutine("moveGlimmers");
		}
	}

	IEnumerator moveGlimmers(){
		float px;
		int ppp;
		while(true){
			px = Player.posx;
			for(ppp = 0 ; ppp < 50 ; ppp++){
				if(glimmers[ppp].position.x - 12 > px){
					glimmersAi[ppp].realPos -= 22;
				}else if(glimmers[ppp].position.x + 12 < px){
					glimmersAi[ppp].realPos += 22;
				}
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void quit(){
		Application.Quit();
	}
}
