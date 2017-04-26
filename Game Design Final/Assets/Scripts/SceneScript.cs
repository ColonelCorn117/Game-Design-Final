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

		//BuildDictionaries ();


		//LoadScene ("Intro01-01");
		LoadScene("TestDescription");
	}

	NPC NpcLookup(string name) {
		return Controller_Game.ctrl_game.NpcLookup (name);
	}

	Mess MessLookup(string name) {
		return Controller_Game.ctrl_game.MessLookup (name);
	}

	Item ItemLookup(string name) {
		return Controller_Game.ctrl_game.ItemLookup (name);
	}

	public void examineObject(GenericGameObject o) {
		examineObject (o, o.name);
	}

	public void examineObject(GenericGameObject o, string name) {
		// rebuild description w/o changing the xml object
		descriptionText.GetComponentInChildren<Text> ().text = o.description;


		if (o is Problem) {
			Problem p = (Problem)o;
			Option back = new Option ("Back", new Action ("backNow", currentSceneName, ""));
			LoadButtons (p.options, back);
			if (p is NPC) {
				LoadDetailSprite (name, "npc");
			} else {
				LoadDetailSprite (name, "mess");
			}

		} else if (o is Item) {
			Item i = (Item)o;
			Option pickup = new Option ("Take", new Action ("takeBack", currentSceneName, "", name));

			Option back = new Option ("Back", new Action ("backNow", currentSceneName, ""));

			List<Option> l = new List<Option> ();

			l.Add (pickup);
			l.Add (back);

			LoadButtons (l);
			LoadDetailSprite (name);
		}
	}

	void BuildDescription () {
		string desc = xml.description;

		if (xml.npcList.Count > 0) {

			int i = 0;

			foreach(string s in xml.npcList) {
				NPC n = NpcLookup (s);

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
				Mess m = MessLookup (s);

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

		if (xml.itemList.Count > 0) {

			int i = 0;
			foreach(string s in xml.itemList) {
				Item it = ItemLookup (s);

				if (it.exists == 1 && (!it.isClaimed())) {
					if (i == 0) {
						desc += "\n\nItems: ";
						++i;
					} else {
						desc += ", ";
					}
					desc += it.name;

				}
			}
		}

		if (Controller_Game.ctrl_game.itemList.Count > 0) {

			int i = 0;
			foreach(string s in Controller_Game.ctrl_game.itemList) {
				Item it = ItemLookup (s);

				if (it.exists == 1 && (it.isClaimed())) {
					if (i == 0) {
						desc += "\n\nInventory: ";
						++i;
					} else {
						desc += ", ";
					}
					desc += it.name;

				}
			}
		}

		descriptionText.GetComponentInChildren<Text> ().text = desc;

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
			//if (sceneName != currentSceneName) {
			currentSceneName = sceneName;
			LoadSceneXML (path);
			titleLocation.GetComponentInChildren<Text> ().text = xml.name;
			//}

			BuildDescription ();
			LoadButtons ();
			LoadBackgroundImage (xml.background);
			LoadDetailSprite ("Default");
		} else {
			LoadScene ("Default");
		}
	}

	public void LoadScene() {
		BuildDescription ();
		LoadButtons ();
		LoadBackgroundImage ("black");
		LoadDetailSprite ("Default");
	}

	public void LoadBackgroundImage(string imageName) {

		string imagePath = "Assets/Rooms/Images/" + imageName + ".png";

		if (File.Exists (imagePath)) {
			byte[] data = File.ReadAllBytes (imagePath);
			Texture2D texture = new Texture2D (64, 64, TextureFormat.ARGB32, false);
			texture.LoadImage (data);
			texture.name = Path.GetFileNameWithoutExtension (imageName);

			Controller_GUI.ctrl_gui.SetBackgroundImage (Sprite.Create (texture, 
				new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f)));
		} else {
			//Debug.Log("Image " + imageName + " not found");

			if (imageName != "black") {
				LoadBackgroundImage ("black");
			}
		}
	}

	public bool LoadDetailSprite(string name, string typeName) {
		string folder = "Rooms";
		switch (typeName) {
		case "npc":
			folder = "NPCs";
			break;
		case "mess":
			folder = "Messes";
			break;
		case "item":
			folder = "Items";
			break;
		}
		string path = "Assets/" + folder + "/Images/" + name + ".png";

		if (File.Exists (path)) {
			byte[] data = File.ReadAllBytes (path);
			Texture2D texture = new Texture2D (64, 64, TextureFormat.ARGB32, false);
			texture.LoadImage (data);
			texture.name = name;

			Controller_GUI.ctrl_gui.SetDetailImage (Sprite.Create (texture,
				new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f)));

			return true;
		} else {
			//Debug.Log("Image " + name + " not found");

			if (name != "Default") {		
				//return LoadDetailSprite ("Default");
			}
			return false;
		}
	}

	public bool LoadDetailSprite(string name) {
		string npcPath = "Assets/NPCs/Images/"  + name + ".png";
		string messPath = "Assets/Messes/Images/" + name + ".png";
		string itemPath = "Assets/Items/Images/" + name + ".png";
		string path = "";
		if (File.Exists (npcPath)) {
			path = npcPath;
		} else if (File.Exists (messPath)) {
			path = messPath;
		} else {
			//Debug.Log("Image " + name + " not found");

			if (name != "Default") {		
				//return LoadDetailSprite ("Default");
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



	void LoadButtons() {

		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;

		foreach (Option o in xml.optionList) {
			//Debug.Log ("Adding option " + o.action.name + " to button " + i);
			addButtonAction (o.action,i);
			optionsText [i - 1].text = o.optionDescription;
			++i;
		}
		foreach (string npcName in xml.npcList) {
			NPC n = NpcLookup (npcName);
			if (n.exists == 1) {
				Action a = new Action ("examine" + npcName, "", npcName);
				addButtonAction (a,i);
				optionsText [i - 1].text = n.name;
				++i;
			}
		}
		foreach (string messName in xml.messList) {
			Mess m = MessLookup (messName);
			if (m.exists == 1) {
				Action a = new Action ("examine" + messName, "", messName);
				addButtonAction (a,i);
				optionsText [i - 1].text = m.name;
				++i;
			}
		}
		//TODO: Add item options here.
		foreach (string itemName in xml.itemList) {
			Item it = ItemLookup (itemName);
			if (it.exists == 1 && !it.isClaimed()) {
				Action a = new Action ("examine" + itemName, "", itemName);
				addButtonAction (a,i);
				optionsText [i - 1].text = it.name;
				++i;
			}
		}


		while(i < optionsText.Length) {
			addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";
			++i;
		}
	}

	void LoadButtons (List<Option> options) {
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Option o in options) {
			//Debug.Log ("Adding option " + o.action.name + " to button " + i);
			addButtonAction (o.action,i);
			optionsText [i - 1].text = o.optionDescription;
			++i;
		}
		while(i < optionsText.Length) {
			addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";
			++i;
		}
	}

	void addButtonAction(Action a, int i) {
		Controller_Game.ctrl_game.addButtonAction (a, i);
	}

	void LoadButtons(List<Option> options, Option extraOption) {
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Option o in options) {
			//Debug.Log ("Adding option " + o.action.name + " to button " + i);
			addButtonAction (o.action,i);
			optionsText [i - 1].text = o.optionDescription;
			++i;
		}

		addButtonAction (extraOption.action,i);
		optionsText [i - 1].text = extraOption.optionDescription;
		++i;

		while(i < optionsText.Length) {
			// The following line isn't strictly needed. Disabling the buttons would also work
			addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";

			++i;
		}

	}


	// Update is called once per frame
	void Update () {
		
	}
}
