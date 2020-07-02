using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativityController : MonoBehaviour {

	public static RelativityController RC;

	float resetX = -100;//		this is the x coordinate of the right edge of the screen. Anything that goes beyond it respawns.
	Transform camt;

	Mesh msh;
	Vector3[] vertices;
	Vector3[] vmoved;
	float posx;

	int vertexCount;
	int i;
	float tmp;

	//		these lists track every edgeCollider platform and deform it relativistically
	public Transform[] AllStaticObjects;
	EdgeCollider2D[] allStaticColliders;
	float[] staticStartPos;
	float[] staticStartLeft;
	float[] staticStartRight;

	//		this list track every dead enemy to be respawned if the player moves backwards enough
	static public List<float> deadX = new List<float>();//		characters will add themselves to these lists when they die
	static public List<GameObject> deadGO = new List<GameObject>();
	float timeToRespawn = 0.3f;

	void Start () {
		RC = this;
		//		reset both of these when a new level is loaded
		deadGO.Clear();
		deadX.Clear();
		msh = GetComponent<MeshFilter>().mesh;
		vertices = msh.vertices;
		vertexCount = vertices.Length-1;
		vmoved = new Vector3[vertexCount+1];
		staticStartPos   = new float[AllStaticObjects.Length];
		staticStartLeft  = new float[AllStaticObjects.Length];
		staticStartRight = new float[AllStaticObjects.Length];
		allStaticColliders = new EdgeCollider2D[AllStaticObjects.Length];
		for(i = AllStaticObjects.Length - 1 ; i > -1 ; i--){
			staticStartPos[i] = AllStaticObjects[i].position.x;
			allStaticColliders[i] =  AllStaticObjects[i].GetComponent<EdgeCollider2D>();
			staticStartLeft[i]  = allStaticColliders[i].points[0].x;
			staticStartRight[i] = allStaticColliders[i].points[1].x;
		}
		camt = Camera.main.transform;
	}
	
	void Update(){
		if(Menu.showLightWarp){
			posx = Player.posx;
			//		loop through all verticies moving each to it's relativistic position
			for(i = vertexCount ; i > -1 ; i--){
				//		calculate position of each vertex based on compression of space due to movement
				//		Speed of light = 20m/s or when velx*0.05f = 1
				//vmoved[i] = new Vector3((vertices[i].x - Mathf.Abs(velx*0.05f) * (vertices[i].x-posx))/Mathf.Sqrt(1-Mathf.Pow(velx*0.05f , 2)/1) , vertices[i].y , vertices[i].z);
				//vmoved[i] = new Vector3(vertices[i].x + ((vmoved[i].x + velx*0.05f*Mathf.Abs(vmoved[i].x-posx))/20) / (vertices[i].x/20) , vertices[i].y , vertices[i].z);

				//vmoved[i] = new Vector3((vertices[i].x + Mathf.Sign(velx) * Mathf.Pow(velx*0.05f*Mathf.Abs(vertices[i].x-posx) , 2)) , vertices[i].y , vertices[i].z);
				//		just shows the effects of light taking longer to reach you due to running away from / twards light

				//		linear version
				//vmoved[i] = new Vector3(vertices[i].x + (velx/Player.C*Mathf.Abs(vertices[i].x-posx)) , vertices[i].y , vertices[i].z);
				//		more realistic version
				vmoved[i] = new Vector3(vertices[i].x + Player.lorentz*Mathf.Abs(vertices[i].x-posx) , vertices[i].y , vertices[i].z);

				//vmoved[i] = new Vector3(vmoved[i].x + (velx*0.05f*Mathf.Abs(vmoved[i].x-posx)) , vmoved[i].y , vmoved[i].z);

			}
			msh.vertices = vmoved;
			for(i = AllStaticObjects.Length - 1 ; i > -1 ; i--){
				//AllStaticObjects[i].position = new Vector3(staticStartPos[i] + velx/Player.C*Mathf.Abs(staticStartPos[i]-posx) , AllStaticObjects[i].position.y , AllStaticObjects[i].position.z);
				//		move the left and right ends of the edgeCollider seperatly so that each is in the righ relativistic position.

				//		linear version
			/*	allStaticColliders[i].points = new Vector2[]{new Vector2(staticStartLeft[i] + velx/Player.C*Mathf.Abs(staticStartLeft[i]+staticStartPos[i]-posx) , allStaticColliders[i].points[0].y),
															new Vector2(staticStartRight[i] + velx/Player.C*Mathf.Abs(staticStartRight[i]+staticStartPos[i]-posx) , allStaticColliders[i].points[1].y)};*/
				//		more realistic version
				allStaticColliders[i].points = new Vector2[]{new Vector2(staticStartLeft[i] + Player.lorentz*Mathf.Abs(staticStartLeft[i]+staticStartPos[i]-posx) , allStaticColliders[i].points[0].y),
															new Vector2(staticStartRight[i] + Player.lorentz*Mathf.Abs(staticStartRight[i]+staticStartPos[i]-posx) , allStaticColliders[i].points[1].y)};
			}
		}


		//		invert relativistic effects to get point 8m to the right of the camera
		//		every 200ms, check what enemies should respawn.
		if (timeToRespawn < 0){
			tmp = Camera.main.orthographicSize;
			if(Menu.showLightWarp)
				resetX = camt.position.x + tmp*2 - Player.lorentz*Mathf.Abs(camt.position.x + tmp*2 - posx);
			else
				resetX = camt.position.x + tmp*2;
			for(i = 0 ; i < deadX.Count ; i++){
				if(deadX[i] > resetX){
					deadGO[i].SetActive(true);
					deadGO[i].SendMessage("respawn");
					deadX.RemoveAt(i);
					deadGO.RemoveAt(i);
					i--;//		check the same i value again since it will be a different Game Object this time
				}
			}
			timeToRespawn = 0.2f;
		}else{
			timeToRespawn -= Time.deltaTime;
		}
	}

	//		reset colliders and background mesh to default
	public void turnOffLightWarp(){
		for(i = vertexCount ; i > -1 ; i--){
			vmoved[i] = vertices[i];
		}
		msh.vertices = vmoved;
		for(i = AllStaticObjects.Length - 1 ; i > -1 ; i--){
			allStaticColliders[i].points = new Vector2[]{new Vector2(staticStartLeft[i] , allStaticColliders[i].points[0].y),
														new Vector2(staticStartRight[i] , allStaticColliders[i].points[1].y)};
		}
	}

}
