using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowInstructions : MonoBehaviour {

	public GameObject showThis;
	public GameObject hideThis;

	private void OnTriggerEnter2D(){
		showThis.SetActive(true);
		hideThis.SetActive(false);
	}

	private void OnTriggerExit2D(){
		showThis.SetActive(false);
		hideThis.SetActive(true);
	}

	float realPos;

	void Start(){
		realPos = transform.position.x;
	}
	
	void Update(){
		if(Menu.showLightWarp)
			transform.position = new Vector3(realPos + Player.lorentz*Mathf.Abs(realPos-Player.posx) , transform.position.y , transform.position.z);
		else
			transform.position = new Vector3(realPos , transform.position.y  , transform.position.z);
	}
}
