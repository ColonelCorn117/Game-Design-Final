using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class DescriptionScript : MonoBehaviour {

	//public DescriptionTester container;
	// Use this for initialization
	void Start () {
		LoadDescription ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LoadDescription () {
		//var path = "Assets/Descriptions/TestDescription.xml";

		//var serializer = new XmlSerializer(typeof(DescriptionTester));
		//var stream = new FileStream(path, FileMode.Open);
		//container = serializer.Deserialize(stream) as DescriptionTester;

		//Debug.Log (container.sceneName);

		//this.GetComponentInChildren<Text> ().text = container.description;
		//stream.Close();
	}
}
