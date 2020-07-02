using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroscopeOutput : MonoBehaviour{

	TextMesh txt;

    void Start(){
        txt = GetComponent<TextMesh>();
    }

    void Update(){
        txt.text = "x=" + Mathf.Round(Input.acceleration.x*10)/10 + "\ny=" + Mathf.Round(Input.acceleration.y*10)/10 + "\nz=" + Mathf.Round(Input.acceleration.z*10)/10;
    }
}
