using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextStage : MonoBehaviour {

	public int sceneNumber = 1;

	private void OnTriggerEnter2D(){
		if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))//		curently on menu
			CamTrack.camSize = 3;//		set default camera size to 75%
		SceneManager.LoadScene(sceneNumber , LoadSceneMode.Single);
		//SceneManager.SetActiveScene(SceneManager.GetSceneByName("MudholeMarsh"));
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
