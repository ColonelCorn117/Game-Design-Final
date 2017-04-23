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

	public static SceneScript sceneScript;

	SceneDescription xml;

	void Awake()
	{
		if (sceneScript == null)
		{
			DontDestroyOnLoad(gameObject);
			sceneScript = this;
		}
		else if (sceneScript != this)
		{
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		titleLocation = GameObject.Find ("Title-Location");
		descriptionText = GameObject.Find ("Description Text");
		optionsBox = GameObject.Find ("Options Box");

		buildDictionaries ();


		var sceneName = "TestDescription";

		LoadScene (sceneName);
	}

	void LoadSceneXML (string path) {
		var serializer = new XmlSerializer(typeof(SceneDescription));
		var stream = new FileStream(path, FileMode.Open);
		xml = serializer.Deserialize(stream) as SceneDescription;
		stream.Close();
	}

	public void LoadScene(string sceneName) {
		string path = "Assets/Descriptions/" + sceneName + ".xml";
		if (File.Exists (path)) {
			LoadSceneXML (path);
			titleLocation.GetComponentInChildren<Text> ().text = xml.name;
			BuildDescription ();

			loadButtons ();
		} else {
			LoadScene ("Default");
		}
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
		
		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/NPCs/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			Controller_Game.ctrl_game.npcs.Add (file.Name.Substring(0,file.Name.Length-4), LoadNPC ("Assets/NPCs/" + file.Name));
			// Note: file.Name includes the file extension.
			//Debug.Log (npcs [file.Name].exists);
		}
	}

	void buildMessDictionary() {

		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/messes/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			Controller_Game.ctrl_game.messes.Add (file.Name.Substring(0,file.Name.Length-4), LoadMess ("Assets/messes/" + file.Name));
			// Note: file.Name includes the file extension.
			//Debug.Log (npcs [file.Name].exists);
		}
	}
		

	void loadButtons() {
		
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;

		//Controller_GUI.ctrl_gui.buttonActions[];
		Controller_Game.ctrl_game.clearButtonActions();

		foreach (Option o in xml.optionList) {
			//Debug.Log ("Adding option " + o.action.name + " to button " + i);
			Controller_Game.ctrl_game.addButtonAction (o.action,i);
			optionsText [i - 1].text = o.optionDescription;
			++i;
		}
		foreach (string npcName in xml.npcList) {
			NPC n = Controller_Game.ctrl_game.npcLookup (npcName);
			if (n.exists == 1) {

				Action a = new Action ();
				a.objectExamined = npcName;

				a.name = "examine" + npcName;

				Controller_Game.ctrl_game.addButtonAction (a,i);
				optionsText [i - 1].text = n.name;
				++i;
			}
		}
		foreach (string messName in xml.messList) {
			Mess m = Controller_Game.ctrl_game.messLookup (messName);
			if (m.exists == 1) {

				Action a = new Action ();
				a.objectExamined = messName;

				a.name = "examine" + messName;

			
				Controller_Game.ctrl_game.addButtonAction (a,i);
				optionsText [i - 1].text = m.name;
				++i;
			}
		}

		//TODO: Add item options here.


		while(i < optionsText.Length) {
			Controller_Game.ctrl_game.addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";

			++i;
		}



	}

	void BuildDescription () {
		string desc = xml.description;


		if (xml.npcList.Count > 0) {
			
			int i = 0;

			foreach(string s in xml.npcList) {
				NPC n = Controller_Game.ctrl_game.npcLookup (s);

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
			
		if (xml.messList.Count > 0) {

			int i = 0;
			foreach(string s in xml.messList) {
				Mess m = Controller_Game.ctrl_game.messLookup (s);

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


		descriptionText.GetComponentInChildren<Text> ().text = desc;

	}

	void changeScene(string path) {
		LoadScene (path);
	}

	void performAction(Action a) {
		// if there's no target, I'm not sure what else could be going on besides a scene change?

		if (a.nextScene != null && a.nextScene != "") {
			LoadScene (a.nextScene);
		}
	}
		

	public void addToDescription(string textToAdd) {
		//Debug.Log ("Adding");
		string desc = descriptionText.GetComponentInChildren<Text> ().text;
		desc += textToAdd;
		descriptionText.GetComponentInChildren<Text> ().text = desc;
	}

	// Update is called once per frame
	void Update () {
		
	}
}
