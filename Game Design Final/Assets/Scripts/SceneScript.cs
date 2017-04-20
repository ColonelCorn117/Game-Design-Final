using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SceneScript : MonoBehaviour {
	GameObject titleLocation;
	GameObject descriptionText;
	GameObject optionsBox;
	public SceneDescription xml;

	// Use this for initialization
	void Start () {
		titleLocation = GameObject.Find ("Title-Location");
		descriptionText = GameObject.Find ("Description-Text");
		optionsBox = GameObject.Find ("Options Box");

		LoadXML ();
		loadScene ();
	}

	void LoadXML () {
		var path = "Assets/Descriptions/TestDescription.xml";

		var serializer = new XmlSerializer(typeof(SceneDescription));
		var stream = new FileStream(path, FileMode.Open);
		xml = serializer.Deserialize(stream) as SceneDescription;
		stream.Close();
	}

	void loadScene() {
		titleLocation.GetComponentInChildren<Text> ().text = xml.sceneName;
		descriptionText.GetComponentInChildren<Text> ().text = xml.description;

		//Options
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 0;
		//Debug.Log ("Options loaded: " + xml.optionList.Options.Count);
		if (xml.optionList.Options.Count > 0) {
			foreach (Text t in optionsText) {
				t.text = xml.optionList.Options [i].optionDescription;

				++i;

				if (i >= (xml.optionList.Options.Count)) {
					break;
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
