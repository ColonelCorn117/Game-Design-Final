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

	Dictionary<string, NPC> npcs;
	Dictionary<string,Mess> messes;
	Dictionary<string,Item> items;


	SceneDescription xml;

	// Use this for initialization
	void Start () {
		titleLocation = GameObject.Find ("Title-Location");
		descriptionText = GameObject.Find ("Description Text");
		optionsBox = GameObject.Find ("Options Box");


		var path = "Assets/Descriptions/TestDescription.xml";
		LoadSceneXML (path);

		buildDictionaries ();
		LoadScene ();
	}

	void LoadSceneXML (string path) {
		
		var serializer = new XmlSerializer(typeof(SceneDescription));
		var stream = new FileStream(path, FileMode.Open);
		xml = serializer.Deserialize(stream) as SceneDescription;
		stream.Close();
	}

	void LoadScene() {
		titleLocation.GetComponentInChildren<Text> ().text = xml.name;


		BuildDescription ();


		//Options

	}

	NPC LoadNPC(string path) {
		var serializer = new XmlSerializer(typeof(NPC));
		var stream = new FileStream(path, FileMode.Open);
		var output = serializer.Deserialize(stream) as NPC;
		stream.Close();
		return output;
	}

	Mess LoadMess(string path) {
		var serializer = new XmlSerializer(typeof(Mess));
		var stream = new FileStream(path, FileMode.Open);
		var output = serializer.Deserialize(stream) as Mess;
		stream.Close();
		return output;
	}

	void buildDictionaries() {
		// Build NPC Dictionary
		buildNPCDictionary();

		// Build Item Dictionary

		// Build Mess Dictionary
		buildMessDictionary();
	}

	void buildNPCDictionary() {
		npcs = new Dictionary<string,NPC>();

		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/NPCs/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			npcs.Add (file.Name.Substring(0,file.Name.Length-4), LoadNPC ("Assets/NPCs/" + file.Name));
			// Note: file.Name includes the file extension.
			//Debug.Log (npcs [file.Name].exists);
		}
	}

	void buildMessDictionary() {
		messes = new Dictionary<string,Mess>();

		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/messes/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			messes.Add (file.Name.Substring(0,file.Name.Length-4), LoadMess ("Assets/messes/" + file.Name));
			// Note: file.Name includes the file extension.
			//Debug.Log (npcs [file.Name].exists);
		}
	}
		

	void loadButtons() {
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 0;
		//Debug.Log ("Options loaded: " + xml.optionList.Options.Count);
		if (xml.optionList.Count > 0) {
			foreach (Text t in optionsText) {
				t.text = xml.optionList[i].optionDescription;

				++i;

				if (i >= (xml.optionList.Count)) {
					break;
				}
			}
		}
	}

	void BuildDescription () {
		string desc = xml.description;


		if (xml.npcList.Count > 0) {
			
			int i = 0;

			foreach(string s in xml.npcList) {
				if (npcs.ContainsKey (s)) {
					NPC n = npcs [s];

					if (n.exists == 1) {
						if (i == 0) {
							desc += "\n\nNPC's: ";
							++i;
						} else {
							desc += ", ";
						}
						desc += n.name;

					}
				}
			}
		}


		if (xml.messList.Count > 0) {

			int i = 0;
			foreach(string s in xml.messList) {
				if (messes.ContainsKey (s)) {
					Mess m = messes [s];

					if (m.exists == 1) {
						if (i == 0) {
							desc += "\n\nMesses: ";
							++i;
						} else {
							desc += ", ";
						}
						desc += m.name;

					}
				}	
			}
		}


		descriptionText.GetComponentInChildren<Text> ().text = desc;

	}

	// Update is called once per frame
	void Update () {
		
	}
}
