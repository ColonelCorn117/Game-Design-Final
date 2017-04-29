using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class SceneScript : MonoBehaviour {
	Text locationText;
	Text descriptionText;
	GameObject optionsBox;

	public static SceneScript sceneScript;
	string currentSceneName = "";
	public string startingSceneName = "TestDescription";

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
		locationText = GameObject.Find("Location Text").GetComponent<Text>();
		descriptionText = GameObject.Find ("Description Text").GetComponent<Text>();
		optionsBox = GameObject.Find ("Options Box");

		//BuildDictionaries ();


		//LoadScene ("Intro01-01");
		LoadScene(startingSceneName);
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

	//----------------------------------------------------------------------------------------------------

	public void examineObject(GenericGameObject o) {
		examineObject (o, o.name);
	}

	public void examineObject(GenericGameObject o, string name) {
		// rebuild description w/o changing the xml object
		descriptionText.text = o.description;
		//string desc = GameObject.Find ("Description Text").GetComponentInChildren<Text> ().text;

		if (o is Problem) {
			Problem p = (Problem)o;



			if (p is Mess) {
				foreach (Condition c in p.conditions) {
					//Debug.Log (c.description);
					//Debug.Log (c.Satisfied ());
				}
			}
				
			LoadButtons (p.conditions, new Condition ("Back", new Action ("Back", currentSceneName, "")));
			if (p is NPC) {
				LoadDetailSprite (name, "npc");
			} else {
				LoadDetailSprite (name, "mess");
			}

		} else if (o is Item) {
			Item i = (Item)o;

			List<Condition> l = new List<Condition> ();
			if (Controller_Game.ctrl_game.itemList.Contains (i.name)) {

				l.Add (new Condition ("Back", new Action ("Back", currentSceneName, "")));
				LoadButtons (l);

			} else {
				if (i.conditions != null) {
					if (i.conditions.Count == 0 || i.conditions [0].description != "Take") {
						l.Add (new Condition ("Take", new Action ("takeBack", currentSceneName, "", name)));
					}
					if (i.conditions.Count == 0 || i.conditions [i.conditions.Count - 1].description != "Back") {
						l.Add (new Condition ("Back", new Action ("Back", currentSceneName, "")));
					}
				}

				if (l.Count > 0) {
					LoadButtons (i.conditions, l);
				} else {
					LoadButtons (i.conditions);
				}
			}


			LoadDetailSprite (name);
		}
	}

	//----------------------------------------------------------------------------------------------------

	void BuildDescription () {
		BuildDescription ("","");
	}

	//----------------------------------------------------------------------------------------------------

	void BuildDescription (string additionalTextFront, string additionalTextBack) {
		string desc = xml.description;
		string additionalDesc = "";

		foreach (Condition c in xml.optionList) {
			if (c.Satisfied ()) {
				additionalDesc += c.additionalDescription + "\n\n";
			}
		}

		desc += additionalDesc;

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
		/* // Show Inventory in text pane
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
		*/

		descriptionText.text = desc;
	}

	//----------------------------------------------------------------------------------------------------

	void LoadSceneXML (string path) {
		var serializer = new XmlSerializer(typeof(SceneDescription));
		var stream = new FileStream(path, FileMode.Open);
		xml = serializer.Deserialize(stream) as SceneDescription;
		stream.Close();
	}

	//----------------------------------------------------------------------------------------------------

	public void LoadScene(string sceneName) {
		if (sceneName == "reload") {
			sceneName = currentSceneName;
		} 
		string path = "Assets/Rooms/" + sceneName + ".xml";
		if (File.Exists (path)) {
			//if (sceneName != currentSceneName) {
			currentSceneName = sceneName;
			LoadSceneXML (path);
			locationText.text = xml.name;
			//}

			BuildDescription ();
			LoadButtons ();
			LoadBackgroundImage (xml.background);
			LoadDetailSprite ("Default");
		} else if ((sceneName.Length > 8) && (sceneName.Substring (0, 8) == "dialogue")) {
			LoadDialogue (sceneName.Substring (8));
		} else {
			LoadScene ("Default");
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void LoadScene() {
		BuildDescription ();
		LoadButtons ();
		LoadBackgroundImage ("black");
		LoadDetailSprite ("Default");
	}

	//----------------------------------------------------------------------------------------------------

	public void LoadSimpleScene(string text, string nextScence) {
		// rebuild description w/o changing the xml object
		descriptionText.text = text;
		Condition continueCondition = new Condition ("Continue", new Action ("continue", nextScence, ""));
		List<Condition> l = new List<Condition> ();
		l.Add (continueCondition);
		LoadButtons (l);
	}

	//----------------------------------------------------------------------------------------------------

	public void LoadBackgroundImage(string imageName) {

		string imagePath = "Assets/Rooms/Images/" + imageName + ".png";

		if (imageName.Length > 3 && imageName.Substring (imageName.Length - 4, 1) == ".") {
			imagePath = "Assets/Rooms/Images/" + imageName;
		}

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

	//----------------------------------------------------------------------------------------------------

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

	//----------------------------------------------------------------------------------------------------

	public bool LoadDetailSprite(string name) {
		string npcPath = "Assets/NPCs/Images/"  + name + ".png";
		string messPath = "Assets/Messes/Images/" + name + ".png";
		string itemPath = "Assets/Items/Images/" + name + ".png";
		string path = "";
		if (File.Exists (npcPath)) {
			path = npcPath;
		} else if (File.Exists (messPath)) {
			path = messPath;
		} else if (File.Exists (itemPath)) {
			path = itemPath;
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

	//----------------------------------------------------------------------------------------------------

	void LoadButtons() {
		Debug.Log ("LoadButtons()");
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;

		foreach (Condition c in xml.optionList) {
			if (c.Satisfied ()) {
				addButtonAction (c.action, i);
				optionsText [i - 1].text = c.description;
				++i;
			}
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

	//----------------------------------------------------------------------------------------------------

	void LoadButtons (List<Condition> conditions) {
		Debug.Log ("LoadButtons(conditions)");
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		Debug.Log ("Conditions : " + conditions.Count);
		foreach (Condition c in conditions) {
			Debug.Log ("Condition: requirement: count; " + c.requirement.Count);
			if (c.Satisfied ()) {
				addButtonAction (c.action, i);
				optionsText [i - 1].text = c.description;
				++i;
			}
		}
		while(i < optionsText.Length) {
			addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";
			++i;
		}
	}

	//----------------------------------------------------------------------------------------------------

	void LoadButtons(List<Condition> conditions, Condition extraCondition) {
		Debug.Log ("LoadButtons(condtions, extra)");
		List<Condition> l = new List<Condition> ();
		l.Add (extraCondition);
		LoadButtons (conditions, l);
	}
	//----------------------------------------------------------------------------------------------------
	void LoadButtons(List<Condition> conditions, List<Condition> extraConditions) {
		Debug.Log ("LoadButtons(conditions, extraConditions)");
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Condition c in conditions) {
			//Debug.Log (c.description + ", " + c.Satisfied());

			if (c.description == null || c.description == "") {
				continue;
			}
			if (c.Satisfied()) {
				addButtonAction (c.action, i);
				optionsText [i - 1].text = c.description;
				++i;
			}
		}

		foreach (Condition c in extraConditions) {
			//Debug.Log (c.description + ", " + c.Satisfied());

			if (c.description == null || c.description == "") {
				continue;
			}
			if (c.Satisfied()) {
				addButtonAction (c.action, i);
				optionsText [i - 1].text = c.description;
				++i;
			}
		}

		while(i < optionsText.Length) {
			// The following line isn't strictly needed. Disabling the buttons would also work
			addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";
			++i;
		}
	}
	//----------------------------------------------------------------------------------------------------


	/*
	void LoadButtons(List<Condition> conditions, Option extraOption) {
		Debug.Log ("LoadButtons(conditions, extra)");
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Condition c in conditions) {
			Debug.Log (c.description + ", " + c.Satisfied());

			if (c.description == null || c.description == "") {
				continue;
			}
			if (c.Satisfied()) {
				addButtonAction (c.action, i);
				optionsText [i - 1].text = c.description;
				++i;
			}
		}

		addButtonAction (extraOption.action,i);
		optionsText [i - 1].text = extraOption.description;
		++i;

		while(i < optionsText.Length) {
			// The following line isn't strictly needed. Disabling the buttons would also work
			addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";

			++i;
		}
	}
	*/
	//----------------------------------------------------------------------------------------------------

	void addButtonAction(Action a, int i) {
		Controller_Game.ctrl_game.addButtonAction (a, i);
	}

	//----------------------------------------------------------------------------------------------------

	/*
	void LoadButtons(List<Option> options, Option extraOption) {
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Option o in options) {
			//Debug.Log ("Adding option " + o.action.name + " to button " + i);

			//if (o is Condition && (!((Condition)o).Satisfied())) {
			//	continue;
			//}
			addButtonAction (o.action,i);
			optionsText [i - 1].text = o.description;
			++i;
		}

		addButtonAction (extraOption.action,i);
		optionsText [i - 1].text = extraOption.description;
		++i;

		while(i < optionsText.Length) {
			// The following line isn't strictly needed. Disabling the buttons would also work
			addButtonAction (new Action(),i);
			optionsText [i - 1].text = "";

			++i;
		}
	}
	*/
	//----------------------------------------------------------------------------------------------------

	public void LoadDialogue(string dialogueName) {
		//Note: the dialogue name should be formatted as "[NPCname][integer value between 0 and 9]"
		string path = "Assets/Dialogue/" + dialogueName + ".xml";

		if (File.Exists (path)) {
			var serializer = new XmlSerializer (typeof(Dialogue));
			var stream = new FileStream (path, FileMode.Open);
			Dialogue d = serializer.Deserialize (stream) as Dialogue;
			stream.Close ();

			descriptionText.text = d.description;
			NpcLookup (dialogueName.Substring (0, dialogueName.Length - 1)).setDialogueLocation (
				int.Parse(dialogueName.Substring (dialogueName.Length - 1))); // sets the dialogue location on the NPC

			this.LoadButtons(d.conditions, new Condition ("Back", new Action ("Back", currentSceneName, "")));

			foreach(Condition c in d.conditions) {
				if (c.Satisfied ()) {
					AddToDescription ("\n\n" + c.additionalDescription);
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void AddToDescription(string additionalDescription) {
		string desc = descriptionText.text;
		descriptionText.text = desc + additionalDescription;
	}

	//----------------------------------------------------------------------------------------------------

	// Update is called once per frame
	void Update () {
		
	}
}
