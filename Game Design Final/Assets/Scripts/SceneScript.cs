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
	public string currentSceneName = "";
	string pastSceneName = "";
	public string startingSceneName = "TestDescription";

	SceneDescription xml;

	//----------------------------------------------------------------------------------------------------

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

	//----------------------------------------------------------------------------------------------------

	// Use this for initialization
	void Start () {
		locationText = GameObject.Find("Location Text").GetComponent<Text>();
		descriptionText = GameObject.Find ("Description Text").GetComponent<Text>();
		optionsBox = GameObject.Find ("Options Box");

		//BuildDictionaries ();


		//LoadScene ("Intro01-01");
		LoadScene(startingSceneName);
	}

	//----------------------------------------------------------------------------------------------------

	public void NewGame() {
		pastSceneName = "";
		currentSceneName = "";
		LoadScene(startingSceneName);
	}

	//----------------------------------------------------------------------------------------------------

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

	public string GetSceneID() {
		return xml.id;
	}

	//----------------------------------------------------------------------------------------------------

	public void examineObjectInInventory(Item i) {
		List<Condition> l = new List<Condition> ();
		l.Add (new Condition ("Back", new Action ("Back", "reload", "", "", 0f)));

		LoadSimpleScene (i.description, l);
	}

	//----------------------------------------------------------------------------------------------------

	public void examineObject(GenericGameObject o) {
		// rebuild description w/o changing the xml object
		descriptionText.text = o.description;
		//string desc = GameObject.Find ("Description Text").GetComponentInChildren<Text> ().text;

		if (o is Problem) {
			Problem p = (Problem)o;
				
			LoadButtons (p.conditions, new Condition ("Back", new Action ("Back", currentSceneName, "","",0f)));
			if (p is NPC) {
				LoadDetailSprite (o.id, "npc");
			} else {
				LoadDetailSprite (o.id, "mess");
			}

		} else if (o is Item) {
			Item i = (Item)o;

			List<Condition> l = new List<Condition> ();
			if (i.isClaimed()) {
				if (i.conditions == null || i.conditions.Count == 0) {
					l.Add (new Condition ("Back", new Action ("Back", currentSceneName, "", "", 0f)));
					LoadButtons (l);
				} else {
					LoadButtons (i.conditions);
				}

			} else {
				if (i.conditions != null) {
					/*
					if (i.conditions.Count == 0 || i.conditions [0].description != "Take") {
						l.Add (new Condition ("Take", new Action ("Take", currentSceneName, "", name)));
					}
					if (i.conditions.Count == 0 || i.conditions [i.conditions.Count - 1].description != "Back") {
						l.Add (new Condition ("Back", new Action ("Back", currentSceneName, "")));
					}
					*/
					if (i.conditions.Count == 0) {
						Debug.Log (i.id + " taken");
						l.Add (new Condition ("Take", new Action ("Take", currentSceneName, "", i.id,0f)));
						l.Add (new Condition ("Back", new Action ("Back", currentSceneName, "","",0f)));
					}
				}


				if (l.Count > 0) {
					LoadButtons (i.conditions, l);
				} else {
					LoadButtons (i.conditions);
				}
			}


			LoadDetailSprite (o.id);
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
				additionalDesc += "\n" + c.additionalDescription;
			}
		}

		desc += additionalDesc;
		/*
		if (xml.npcList.Count > 0) {

			int i = 0;

			foreach(string s in xml.npcList) {
				NPC n = NpcLookup (s);
				/*
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
		*/
		/*
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
		*/

		/*
		if (xml.itemList.Count > 0) {

			int i = 0;
			foreach(string s in xml.itemList) {
				Item it = ItemLookup (s);
				/*
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
		*/
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
		desc += BodyDescriptions ();
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
			if (sceneName != currentSceneName) {
				pastSceneName = currentSceneName;
				currentSceneName = sceneName;
			}
			LoadSceneXML (path);
			locationText.text = xml.name;
			xml.id = sceneName;
			//}

			BuildDescription ();
			Controller_Game.ctrl_game.UpdateMessCount ();
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
		Condition continueCondition = new Condition ("Continue", new Action ("continue", nextScence, "","",0f));
		List<Condition> l = new List<Condition> ();
		l.Add (continueCondition);
		LoadButtons (l);
	}

	public void LoadSimpleScene(string text, List<Condition> conditions) {
				descriptionText.text = text;
				//Condition continueCondition = new Condition ("Continue", new Action ("continue", nextScence, "","",0f));
				//List<Condition> l = new List<Condition> ();

				LoadButtons (conditions);
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

	string BodyDescriptions() {
		string textToAdd = "";
		
		if (xml.id.Contains ("Closet") || (xml.id == "LaundryRoom")) {
			int bodyCount = Controller_Game.ctrl_game.BodiesVisible (xml.id);
			int corpseCount = Controller_Game.ctrl_game.CorpsesVisible (xml.id);
			if (bodyCount + corpseCount == 0) {
				textToAdd += "Assassin: I could probably squeeze a few bodies in here if I needed to hide them.";
			} else {
				switch (bodyCount) {
				case 0:
					break;
				case 1:
					textToAdd += "A maid lies";
					break;
				case 2:
					textToAdd += "Two maids lie";
					break;
				case 3:
					textToAdd += "Three maids lie";
					break;
				default:
					textToAdd += "Several maids lie";
					break;
				}
				if (bodyCount > 0) {
					textToAdd += " unconscious on the floor.";
				}

				switch (corpseCount) {
				case 0:
					break;
				case 1:
					textToAdd += "\nA maid lies";
					break;
				case 2:
					textToAdd += "\nTwo maids lie";
					break;
				case 3:
					textToAdd += "\nThree maids lie";
					break;
				default:
					textToAdd += "\nSeveral maids lie";
					break;
				}
				if (corpseCount > 0) {
					textToAdd += " dead on the floor.";
				}

				if ((bodyCount + corpseCount) > 4) {
					textToAdd += "\nAssassin: How did I manage to get all of these idiots into here?";
				}
			}
		}
		return textToAdd;
	}

	//----------------------------------------------------------------------------------------------------

	void LoadButtons() {
		//Debug.Log ("LoadButtons()");
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;

		foreach (Condition c in xml.optionList) {
			if (c.Satisfied ()) {
				addButtonAction (c.action, i);
				optionsText [i - 1].text = c.description;
				if (c.action.nextScene == pastSceneName)
				{
					optionsText [i - 1].text += "*";
				}
				++i;
			}
		}
		foreach (string npcName in xml.npcList) {
			NPC n = NpcLookup (npcName);
			if (n.exists == 1) {
				Action a = new Action ("examine" + npcName, "", npcName, "",0f);
				addButtonAction (a,i);
				optionsText [i - 1].text = "Approach " + n.name;
				++i;
			}
		}
		foreach (string messName in xml.messList) {
			Mess m = MessLookup (messName);
			if (m.exists == 1) {
				Action a = new Action ("examine" + messName, "", messName,"",0f);
				addButtonAction (a,i);
				optionsText [i - 1].text = "Examine " + m.name;
				++i;
			}
		}
		//TODO: Add item options here.
		foreach (string itemName in xml.itemList) {
			Item it = ItemLookup (itemName);
			if (it.isViewableInRoom(xml.id)) {
				Action a = new Action ("examine" + itemName, "", itemName,"",0f);
				addButtonAction (a,i);
				optionsText [i - 1].text = "Examine " + it.name;
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
		//Debug.Log ("LoadButtons(conditions)");
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Condition c in conditions) {
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
		//Debug.Log ("LoadButtons(condtions, extra)");
		List<Condition> l = new List<Condition> ();
		l.Add (extraCondition);
		LoadButtons (conditions, l);
	}
	//----------------------------------------------------------------------------------------------------
	void LoadButtons(List<Condition> conditions, List<Condition> extraConditions) {
		//Debug.Log ("LoadButtons(conditions, extraConditions)");
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Condition c in conditions) {

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
		
		var optionsText = optionsBox.GetComponentsInChildren<Text>();
		int i = 1;
		foreach (Condition c in conditions) {

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

			this.LoadButtons(d.conditions, new Condition ("Back", new Action ("Back", currentSceneName, "","",0f)));

			foreach(Condition c in d.conditions) {
				if (c.Satisfied ()) {
					AddToDescription ("\n" + c.additionalDescription);
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
