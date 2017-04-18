using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MenuScript : MonoBehaviour {


	// Use this for initialization
	void Start () {
		this.GetComponent<Button>().onClick.AddListener(TaskOnClick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
		

	void TaskOnClick() {
		Debug.Log ("You clicked me!");
	}
}
