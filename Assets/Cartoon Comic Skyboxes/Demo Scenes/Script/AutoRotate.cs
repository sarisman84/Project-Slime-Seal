using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour {

    float angle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        angle += Time.deltaTime *30f; 

        Vector3 Angles = Camera.main.transform.eulerAngles;
        transform.eulerAngles = new Vector3(Angles.x, angle, Angles.z);
    }
}
