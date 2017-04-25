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
	string currentSceneName = "";

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

		BuildDictionaries ();


		var sceneName = "Intro01-01";

		LoadScene (sceneName);
	}

	void LoadSceneXML (string path) {
		var serializer = new XmlSerializer(typeof(SceneDescription));
		var stream = new FileStream(path, FileMode.Open);
		xml = serializer.Deserialize(stream) as SceneDescription;
		stream.Close();
	}

	public void LoadScene(string sceneName) {
		string path = "Assets/Rooms/" + sceneName + ".xml";
		if (File.Exists (path)) {
			if (sceneName != currentSceneName) {
				currentSceneName = sceneName;
				LoadSceneXML (path);
				titleLocation.GetComponentInChildren<Text> ().text = xml.name;
			}

			BuildDescription ();
			LoadButtons ();
			LoadBackgroundImage (xml.background);
			LoadDetailSprite ("Default");
		} else {
			LoadScene ("Default");
		}
	}

	// loads the scene, but does not rebuild the XML object.
	public void LoadScene() {
		BuildDescription ();
		LoadButtons ();
		LoadBackgroundImage ("black.png");
		LoadDetailSprite ("Default");
	}

	public void LoadBackgroundImage(string imageName) {
		string imagePath = "Assets/Rooms/Images/" + imageName;


		if (File.Exists (imagePath)) {
			byte[] data = File.ReadAllBytes (imagePath);
			Texture2D texture = new Texture2D (64, 64, TextureFormat.ARGB32, false);
			texture.LoadImage (data);
			texture.name = Path.GetFileNameWithoutExtension (imageName);

			Controller_GUI.ctrl_gui.SetBackgroundImage (Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f)));
		} else {
			Debug.Log("Image " + imageName + " not found");

			if (imageName != "black.png") {		
				LoadBackgroundImage ("black.png");
			}
		}
	}

	public bool LoadDetailSprite(string name) {
		string npcPath = "Assets/NPCs/Images/"  + name + ".png";
		string messPath = "Assets/Messes/Images/" + name + ".png";
		string path = "";
		if (File.Exists (npcPath)) {
			path = npcPath;
		} else if (File.Exists (messPath)) {
			path = messPath;
		} else {
			Debug.Log("Image " + name + " not found");

			if (name != "Default") {		
				return LoadDetailSprite ("Default");
			}

			return false;
		}

		byte[] data = File.ReadAllBytes (path);
		Texture2D texture = new Texture2D (64, 64, TextureFormat.ARGB32, false);
		texture.LoadImage (data);
		texture.name = name;

		Controller_GUI.ctrl_gui.SetDetailImage (Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f)));

		return true;
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

	void BuildDictionaries() {
		// Build NPC Dictionary
		BuildNPCDictionary();

		// Build Item Dictionary

		// Build Mess Dictionary
		BuildMessDictionary();
	}

	void BuildNPCDictionary() {
		
		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/NPCs/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			Controller_Game.ctrl_game.npcs.Add (file.Name.Substring(0,file.Name.Length-4), LoadNPC ("Assets/NPCs/" + file.Name));
			// Note: file.Name includes the file extension.
			//Debug.Log (npcs [file.Name].exists);
		}
	}

	void BuildMessDictionary() {

		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/messes/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			Controller_Game.ctrl_game.messes.Add (file.Name.Substring(0,file.Name.Length-4), LoadMess ("Assets/messes/" + file.Name));
			// Note: file.Name includes the file extension.
			//Debug.Log (npcs [file.Name].exists);
		}
	}
		

	void LoadButtons() {
		
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

	void loadButtons (List<Option> options) {
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Option o in options) {
			//Debug.Log ("Adding option " + o.action.name + " to button " + i);
			Controller_Game.ctrl_game.addButtonAction (o.action,i);
			optionsText [i - 1].text = o.optionDescription;
			++i;
		}

		while(i < optionsText.Length) {
			Controller_Game.ctrl_game.addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";

			++i;
		}
	}

	void loadButtons(List<Option> options, Option extraOption) {
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Option o in options) {
			//Debug.Log ("Adding option " + o.action.name + " to button " + i);
			Controller_Game.ctrl_game.addButtonAction (o.action,i);
			optionsText [i - 1].text = o.optionDescription;
			++i;
		}

		Controller_Game.ctrl_game.addButtonAction (extraOption.action,i);
		optionsText [i - 1].text = extraOption.optionDescription;
		++i;

		while(i < optionsText.Length) {
			// The following line isn't strictly needed. Disabling the buttons would also work
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

	public void examineObject(GenericGameObject o) {
		examineObject (o, o.name);
	}

	public void examineObject(GenericGameObject o, string name) {
		// rebuild description w/o changing the xml object
		descriptionText.GetComponentInChildren<Text> ().text = o.description;


		if (o is Problem) {
			Problem p = (Problem)o;

			Option back = new Option ();
			back.optionDescription = "Back";
			//back.name = "goingBack";
			Action a = new Action ();
			a.nextScene = currentSceneName;
			a.name = "backNow";
			back.action = a;
			loadButtons (p.options, back);
			LoadDetailSprite (name);
		}


		//TODO: make this work for items

	}

	void changeScene(string path) {
		LoadScene (path);
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
