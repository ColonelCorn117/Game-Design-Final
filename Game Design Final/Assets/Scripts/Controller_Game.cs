using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Controller_Game : MonoBehaviour
{
	//This script needs to be attached to the canvas to work properly.

	//====================================================================================================
	//Variables
	//====================================================================================================

	public static Controller_Game ctrl_game;

	//Used to keep references to the xml files so they are included in the build
	//public List<TextAsset> dialogueXML = new List<TextAsset>();
	//public List<TextAsset> itemsXML = new List<TextAsset>();
	//public List<TextAsset> messesXML = new List<TextAsset>();
	//public List<TextAsset> npcsXML = new List<TextAsset>();
	//public List<TextAsset> roomsXML = new List<TextAsset>();

	public List<string> itemList = new List<string>();
	Dictionary<string, NPC> npcs = new Dictionary<string, NPC>();
	Dictionary<string,Mess> messes = new Dictionary<string, Mess>();
	Dictionary<string,Item> items = new Dictionary<string, Item>();
	public float timeRemaining = 50.0f;
	public int killCount = 0;
	public int unclaimedBodyCount = 0;
	public int breadConsumed = 0;
	public int breadQuantity = 0;

	Dictionary<string,int> bodyInRoom = new Dictionary<string,int>();
	Dictionary<string,int> corpseInRoom = new Dictionary<string,int>();

	float unassignedActionTime = 0.2f;	//how much time an action should take if the action has no assigned value

	public Action[] buttonActions = new Action[9]; // buttonActions[0] is empty. the action for btn1 is found in buttonActions[1].
	//int numButtonActions = 1; // first 'empty slot' that can be filled in buttonActions.

	GameObject titleLocation;
	GameObject descriptionText;
	GameObject optionsBox;
	SceneDescription xml;

	//====================================================================================================
	//Start Stuff and Updaters
	//====================================================================================================

	void Awake()
	{
		if (ctrl_game == null)
		{
			DontDestroyOnLoad(gameObject);
			ctrl_game = this;

			BuildDictionaries ();
			//items.Add("Mop");
		}
		else if (ctrl_game != this)
		{
			Destroy(gameObject);
		}
	}

	//----------------------------------------------------------------------------------------------------

	void Start()
	{
		Controller_GUI.ctrl_gui.SetItemsText(itemList);
		Controller_GUI.ctrl_gui.SetTimeText(timeRemaining);
	}

	//----------------------------------------------------------------------------------------------------

	void Update() {

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			this.performAction (1);
		} else if (Input.GetKeyDown (KeyCode.Alpha2)) {
			this.performAction (2);
		} else if (Input.GetKeyDown (KeyCode.Alpha3)) {
			this.performAction (3);
		} else if (Input.GetKeyDown (KeyCode.Alpha4)) {
			this.performAction (4);
		} else if (Input.GetKeyDown (KeyCode.Alpha5)) {
			this.performAction (5);
		} else if (Input.GetKeyDown (KeyCode.Alpha6)) {
			this.performAction (6);
		} else if (Input.GetKeyDown (KeyCode.Alpha7)) {
			this.performAction (7);
		} else if (Input.GetKeyDown (KeyCode.Alpha8)) {
			this.performAction (8);
		}

	}

	//====================================================================================================
	//Other Functions
	//====================================================================================================

	public void AddItemToInv(string item)
	{
		itemList.Add(item);
		Controller_GUI.ctrl_gui.SetItemsText(itemList);
		//ctrl_gui.ScaleItemsList();
		//Debug.Log("Item Added: " + item);
	}

	//----------------------------------------------------------------------------------------------------

	public void RemoveItemFromInv(string item)
	{
		itemList.Remove(item);
		Controller_GUI.ctrl_gui.SetItemsText(itemList);
		//ctrl_gui.DestroyBtnsOnItemsList();
		//Debug.Log("Item Removed: " + item);
	}

	//----------------------------------------------------------------------------------------------------

	public NPC NpcLookup(string s) {
		NPC n;
		npcs.TryGetValue (s, out n);
		if (n != null) {
			return n;
		}
		return new NPC();
	}

	//----------------------------------------------------------------------------------------------------

	public Mess MessLookup(string s) {
		Mess m;
		messes.TryGetValue (s, out m);
		if (m != null) {
			return m;
		}
		return new Mess();
	}

	//----------------------------------------------------------------------------------------------------

	public Item ItemLookup(string s) {
		Item i;
		items.TryGetValue (s, out i);
		if (i != null) {
			return i;
		}
		return new Item();
	}
		
	//----------------------------------------------------------------------------------------------------

	//----------------------------------------------------------------------------------------------------

	public int BodiesVisible(string room) {
		if (!this.bodyInRoom.ContainsKey (room)) {
			return 0;
		}
		return bodyInRoom [room];
	}

	//----------------------------------------------------------------------------------------------------

	public int CorpsesVisible(string room) {
		if (!corpseInRoom.ContainsKey (room)) {
			return 0;
		}
		return corpseInRoom [room];
	}

	//----------------------------------------------------------------------------------------------------

	public void AddBody(string room) {
		AddBody (room, true);
	}

	public void AddBody(string room, bool addToCount) {
		if (room != "") {
			if (room == "LaundryChute3F") {
				AddBody ("LaundryChute1F", addToCount);
			} else {
				if (this.bodyInRoom.ContainsKey (room)) {
					++bodyInRoom [room];
				} else {
					bodyInRoom.Add (room, 1);
				}
				if (addToCount) {
					++this.unclaimedBodyCount;
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void AddCorpse(string room) {
		AddCorpse (room, true);
	}

	public void AddCorpse(string room, bool addToCount) {
		if (room != "") {
			if (room == "LaundryChute3F") {
				AddCorpse ("LaundryChute1F", addToCount);
			} else {
				if (corpseInRoom.ContainsKey (room)) {
					++corpseInRoom [room];
				} else {
					this.corpseInRoom.Add (room, 1);
				}
				if (addToCount) {
					++this.unclaimedBodyCount;
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void RemoveBody(string room) {
		if (this.bodyInRoom.ContainsKey (room)) {
			if (bodyInRoom [room] > 0) {
				bodyInRoom [room] -= 1;
				--this.unclaimedBodyCount;
			} else {
				Debug.Log ("Room " + room + " contains too few bodies");
			}
		} else {
			Debug.Log ("Room " + room + " not found");
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void RemoveCorpse(string room) {
		if (this.corpseInRoom.ContainsKey (room)) {
			if (corpseInRoom [room] > 0) {
				corpseInRoom [room] -= 1;
				--this.unclaimedBodyCount;
			} else {
				Debug.Log ("Room " + room + " contains too few corpses");
			}
		} else {
			Debug.Log ("Room " + room + " not found");
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void performAction(int btnNbr) {
		Action a = buttonActions [btnNbr];

		// if an action consumes an item, delete that item from inventory
		if (a.itemUsed != null && a.itemUsed != "") {
			if (a.itemUsed == "Bread") {
				if (breadQuantity < 2) {
					//this.ItemLookup (a.itemGained).consume ();
					breadQuantity = 1;
				}
				--breadQuantity;
				++breadConsumed;

			} else if (a.itemUsed == "Body") {
				Debug.Log ("Attempting to hide a body in : " + SceneScript.sceneScript.GetSceneID());
				AddBody (SceneScript.sceneScript.GetSceneID(), false);
			} else if (a.itemUsed == "Corpse") {
				Debug.Log ("Attempting to hide a corpse in : " + SceneScript.sceneScript.GetSceneID());
				AddCorpse (SceneScript.sceneScript.GetSceneID(),false);
			}
			//this.itemList.Remove (a.itemUsed);
			this.ItemLookup (a.itemUsed).consume ();
			RemoveItemFromInv(a.itemUsed);
		}


		// if an action picks up an item, add that item to inventory and mark it as taken from the room
		if (a.itemGained != null && a.itemGained != "") {
			if (a.itemGained == "Bread") {
				if (breadQuantity < 1) {
					this.ItemLookup (a.itemGained).claim ();
					breadQuantity = 1;
				}
				++breadQuantity;
			} else if (a.itemGained == "Body") {
				Debug.Log ("Attempting to remove a body from : " + SceneScript.sceneScript.GetSceneID());
				RemoveBody (SceneScript.sceneScript.GetSceneID());
			} else if (a.itemGained == "Corpse") {
				Debug.Log ("Attempting to remove a corpse from : " + SceneScript.sceneScript.GetSceneID());
				RemoveCorpse (SceneScript.sceneScript.GetSceneID());
			}
			this.ItemLookup (a.itemGained).claim ();
			AddItemToInv (a.itemGained);
		}

		if (a.itemCreated != null && a.itemCreated != "") {
			this.ItemLookup (a.itemCreated).create ();
			if (a.itemCreated == "Corpse") {
				Debug.Log ("Attempting to add a corpse to : " + SceneScript.sceneScript.GetSceneID());
				AddCorpse (SceneScript.sceneScript.GetSceneID());
			} else if (a.itemCreated == "Body") {
				Debug.Log ("Attempting to add a body to : " + SceneScript.sceneScript.GetSceneID());
				AddBody (SceneScript.sceneScript.GetSceneID());
			}
		}
		// if an action cleans up a mess, clean it up
		if (a.messResolved != null && a.messResolved != "") {
			MessLookup (a.messResolved).Cleanup (a);
			// Note: this method returns a bool regarding whether it succeeded
			// it currently isn't used for anything
		}
		if (a.messCreated != null && a.messCreated != "") {
			MessLookup (a.messCreated).Unclean (a);
			// Note: this method returns a bool regarding whether it succeeded
			// it currently isn't used for anything
		}

		// if an action removes an NPC, do that
		if (a.npcSubdued != null && a.npcSubdued != "") {
			
			NpcLookup (a.npcSubdued).Subdue (a);
			// Note: this method returns a bool regarding whether it succeeded
			// it currently isn't used for anything
		}
		if (a.kill > 0) {
			this.killCount += a.kill;
		}

		if (a.npcCreated != null && a.npcCreated != "") {
			NpcLookup (a.npcCreated).Create ();
		}

		// if the player is trying to examine an item, mess, or NPC, set up the screen to do that.
		if (a.objectExamined != null && a.objectExamined != "") {
			string addedText = "";

			NPC n = NpcLookup (a.objectExamined);
			if (n.exists == 1) {
				SceneScript.sceneScript.examineObject (n, a.objectExamined);
			} else {
				Mess m = MessLookup (a.objectExamined);
				if (m.exists == 1) {
					SceneScript.sceneScript.examineObject (m, a.objectExamined);
				} else {
					Item i = ItemLookup(a.objectExamined);
					if (i.exists == 1) {
						SceneScript.sceneScript.examineObject (i, a.objectExamined);
					}
				}
			}
			if (addedText != "") {
				//string desc = GameObject.Find ("Description Text").GetComponentInChildren<Text> ().text;
				//desc += addedText;
				//GameObject.Find ("Description Text").GetComponentInChildren<Text> ().text = desc;
			}
		}
		if(a.nextScene != null && a.nextScene != "") {
			if (a.description != null && a.description != "") {
				SceneScript.sceneScript.LoadSimpleScene (a.description, a.nextScene);
			} else {
				SceneScript.sceneScript.LoadScene (a.nextScene);
			}

		}

		if (a.timeUsed < 0.0f)
		{
			ChangeRemainingTime(unassignedActionTime);
		}
		else
		{
			ChangeRemainingTime(a.timeUsed);
		}
	}

	//----------------------------------------------------------------------------------------------------

	public bool addButtonAction(Action a, int index) {
		if (index < buttonActions.Length) {

			if (a.name != null && a.name != "") {
				
			}
			buttonActions [index] = a;
			return true;
		}

		return false;
	}

	//----------------------------------------------------------------------------------------------------

	public void examineItem(int i) {
		Item item = ItemLookup (itemList [i-1]);
		SceneScript.sceneScript.examineObject (item,item.name);
	}

	//----------------------------------------------------------------------------------------------------

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

	Item LoadItem(string path) {
		var serializer = new XmlSerializer(typeof(Item));
		var stream = new FileStream(path, FileMode.Open);
		var output = serializer.Deserialize(stream) as Item;
		output.id = path.Substring(13,path.Length - (13 + 4));// Items should be in Assets/Items/
		stream.Close();
		return output;
	}


	//----------------------------------------------------------------------------------------------------

	void BuildDictionaries() {
		// Build NPC Dictionary
		BuildNPCDictionary();

		// Build Item Dictionary

		// Build Mess Dictionary
		BuildMessDictionary();

		BuildItemDictionary();
	}

	//----------------------------------------------------------------------------------------------------

	void BuildNPCDictionary() {

		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/NPCs/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			npcs.Add (file.Name.Substring(0,file.Name.Length-4), LoadNPC ("Assets/NPCs/" + file.Name));
			// Note: file.Name includes the file extension.
		}
	}

	//----------------------------------------------------------------------------------------------------

	void BuildMessDictionary() {

		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/messes/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			messes.Add (file.Name.Substring(0,file.Name.Length-4), LoadMess ("Assets/Messes/" + file.Name));
			// Note: file.Name includes the file extension.
		}
	}

	//----------------------------------------------------------------------------------------------------

	void BuildItemDictionary() {
		DirectoryInfo levelDirectoryPath = new DirectoryInfo("Assets/Items/");
		FileInfo[] fileInfo = levelDirectoryPath.GetFiles("*.xml", SearchOption.AllDirectories);
		foreach (FileInfo file in fileInfo) {
			items.Add (file.Name.Substring(0,file.Name.Length-4), LoadItem ("Assets/Items/" + file.Name));
			// Note: file.Name includes the file extension.
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void ChangeRemainingTime(float amount)
	{
		timeRemaining -= amount;
		Controller_GUI.ctrl_gui.SetTimeText(timeRemaining);
	}




}
