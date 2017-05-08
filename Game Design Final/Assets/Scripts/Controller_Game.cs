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

	//Used to keep references to the xml files so they are included in the build, but we're just dropping the assets folder next to the build, so we don't use them.
	//public List<TextAsset> dialogueXML = new List<TextAsset>();
	//public List<TextAsset> itemsXML = new List<TextAsset>();
	//public List<TextAsset> messesXML = new List<TextAsset>();
	//public List<TextAsset> npcsXML = new List<TextAsset>();
	//public List<TextAsset> roomsXML = new List<TextAsset>();

	public List<string> itemList = new List<string>();				//Every individual item in the inventory (no item stacks)
	public List<string> itemListCondensed = new List<string>();		//Abbreviated version of the above (1 stack = 1 item)
	Dictionary<string, NPC> npcs = new Dictionary<string, NPC>();
	Dictionary<string,Mess> messes = new Dictionary<string, Mess>();
	Dictionary<string,Item> items = new Dictionary<string, Item>();

	public float startTimeRemaining = 130.0f;

	public float timeRemaining = 130.0f;
	public int killCount = 0;
	public int unclaimedBodyCount = 0;
	public int breadConsumed = 0;		//How much bread the gorilla maid has consumed
	public int breadQuantity{ get; set; }
	public int breadSlot{ get; set;}	//Which slot in the inventory contains bread

	Dictionary<string,int> bodyInRoom = new Dictionary<string,int>();
	Dictionary<string,int> corpseInRoom = new Dictionary<string,int>();


	public bool encounterTimerRunning = false;
	public float encounterTimer = 0f;

	public bool endGame = false;

	public string savedSceneName = ""; // Only used in save states
	public List<string> saveitemList = new List<string>();				//Every individual item in the inventory (no item stacks)
	public List<string> saveitemListCondensed = new List<string>();		//Abbreviated version of the above (1 stack = 1 item)
	Dictionary<string, NPC> savenpcs = new Dictionary<string, NPC>();
	Dictionary<string,Mess> savemesses = new Dictionary<string, Mess>();
	Dictionary<string,Item> saveitems = new Dictionary<string, Item>();
	public float savetimeRemaining = 130.0f;
	public int savekillCount = 0;
	public int saveunclaimedBodyCount = 0;
	public int savebreadConsumed = 0;		//How much bread the gorilla maid has consumed
	public int savebreadQuantity{ get; set; }
	public int savebreadSlot{ get; set;}	//Which slot in the inventory contains bread
	public bool saveencounterTimerRunning = false;
	public float saveencounterTimer = 0f;
	public bool saveendGame = false;
	Dictionary<string,int> savebodyInRoom = new Dictionary<string,int>();
	Dictionary<string,int> savecorpseInRoom = new Dictionary<string,int>();

	float unassignedActionTime = 1f;	//how much time an action should take if the action has no assigned value

	public Action[] buttonActions = new Action[9]; // buttonActions[0] is empty. the action for btn1 is found in buttonActions[1].
	//int numButtonActions = 1; // first 'empty slot' that can be filled in buttonActions.

	GameObject titleLocation;
	GameObject descriptionText;
	GameObject optionsBox;
	//SceneDescription xml;

	//====================================================================================================
	//Start Stuff and Updaters
	//====================================================================================================

	void Awake()
	{
		if (ctrl_game == null)
		{
			DontDestroyOnLoad(gameObject);
			ctrl_game = this;
			breadQuantity = 0;
			breadSlot = -1;

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
		Controller_GUI.ctrl_gui.SetMessesText(GetMessCount());
		Controller_GUI.ctrl_gui.SetTimeText(timeRemaining);

		timeRemaining = startTimeRemaining;
	}

	//----------------------------------------------------------------------------------------------------

	void Update() {
		if (!Controller_GUI.ctrl_gui.itemsBox.gameObject.activeSelf)
		{
			if (Input.GetKeyDown (KeyCode.Space)) {
				this.performAction (1);
			} else if (Input.GetKeyDown (KeyCode.Alpha1)) {
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
	}

	//====================================================================================================
	//Other Functions
	//====================================================================================================

	public void AddItemToInv(string item)
	{
		itemList.Add(item);
		//Controller_GUI.ctrl_gui.SetItemsText(itemList);
		//ctrl_gui.ScaleItemsList();
		//Debug.Log("Item Added: " + item);
	}

	//----------------------------------------------------------------------------------------------------

	public void RemoveItemFromInv(string item)
	{
		itemList.Remove(item);
		//Controller_GUI.ctrl_gui.SetItemsText(itemList);
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
			RemoveItemFromInv(a.itemUsed); // needs to happen before consuming the item, due to duplicable item stuff
			this.ItemLookup (a.itemUsed).consume ();

		}
		if (a.startTimer != 0) {
			this.encounterTimerRunning = true;
		}
		if (a.stopTimer != 0) {
			this.encounterTimerRunning = false;
		}

		// if an action picks up an item, add that item to inventory and mark it as taken from the room
		if (a.itemGained != null && a.itemGained != "") {
			if (a.itemGained == "Bread") {
				if (breadQuantity < 1) {
					this.ItemLookup (a.itemGained).claim ();
					breadQuantity = 0;
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
				SceneScript.sceneScript.examineObject (n);
			} else {
				Mess m = MessLookup (a.objectExamined);
				if (m.exists == 1) {
					SceneScript.sceneScript.examineObject (m);
				} else {
					Item i = ItemLookup(a.objectExamined);
					if (i.exists == 1) {
						SceneScript.sceneScript.examineObject (i);
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

		Controller_GUI.ctrl_gui.SetMessesText(GetMessCount());
//		Debug.Log("pA timeUsed: " + a.timeUsed);
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
		if (i < itemListCondensed.Count)
		{
			Item item = ItemLookup (itemListCondensed [i-1]);
			SceneScript.sceneScript.examineObjectInInventory (item);
		}

	}

	//----------------------------------------------------------------------------------------------------

	NPC LoadNPC(string path) {
		//Debug.Log (path);
		var serializer = new XmlSerializer(typeof(NPC));
		var stream = new FileStream(path, FileMode.Open);
		var output = serializer.Deserialize(stream) as NPC;
		stream.Close();
		return output;
	}

	Mess LoadMess(string path) {
		//Debug.Log (path);
		var serializer = new XmlSerializer(typeof(Mess));
		var stream = new FileStream(path, FileMode.Open);
		var output = serializer.Deserialize(stream) as Mess;
		stream.Close();
		return output;
	}

	Item LoadItem(string path) {
		//Debug.Log (path);
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
		BuildItemDictionary();

		// Build Mess Dictionary
		BuildMessDictionary();
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
//		Debug.Log("CTR: " + timeRemaining);
		Controller_GUI.ctrl_gui.SetTimeText(timeRemaining);
		if (encounterTimerRunning) {
			encounterTimer += amount;
		}
		if (timeRemaining <= 0) {
			timeRemaining = 0;

			if (!endGame)
			{
				endGame = true;
				SceneScript.sceneScript.LoadScene ("EndGame");
			}

		} else if (GetMessCount () < 1) {
			endGame = true;
			SceneScript.sceneScript.LoadScene ("EndGame1");
		}
	}

	//----------------------------------------------------------------------------------------------------

	public void UpdateMessCount() {
		int count = 0;

		foreach (KeyValuePair<string, Mess> mess in messes)
		{
			if (mess.Value.exists == 1)
			{
				//Debug.Log (mess.Value.name);
				count++;
			}
		}
		Controller_GUI.ctrl_gui.SetMessesText (GetMessCount());
	}

	public int GetMessCount() {
		int count = 0;

		foreach (KeyValuePair<string, Mess> mess in messes)
		{
			if (mess.Value.exists == 1)
			{
				Debug.Log (mess.Value.name);
				++count;
			}
		}
		foreach (KeyValuePair<string, NPC> npc in npcs) {
			if (npc.Value.exists == 1) {
				if (!npc.Value.name.Contains ("Flag")) {
					if (npc.Value.name == "Maid3a") {
						
					} else if (npc.Value.name == "'Maid'") {

					} else {
						Debug.Log (npc.Value.name);
						++count;
					}

				}
			}
		}

		return count + this.unclaimedBodyCount;
	}

	//----------------------------------------------------------------------------------------------------

	public void QuitGame()
	{
		Application.Quit();
	}

	public void NewGame()
	{
		npcs.Clear ();
		messes.Clear ();
		items.Clear ();
		itemList.Clear ();
		itemListCondensed.Clear ();
		BuildDictionaries ();

		//SceneScript.sceneScript
		timeRemaining = startTimeRemaining;

		killCount = 0;
		unclaimedBodyCount = 0;
		breadConsumed = 0;
		bodyInRoom.Clear ();
		corpseInRoom.Clear ();
		encounterTimerRunning = false;
		encounterTimer = 0f;
		breadQuantity = 0;
		breadSlot = -1;


		SceneScript.sceneScript.NewGame();
	}

	public void SaveGame() {
		if (this.savedSceneName == null || this.savedSceneName == "") {
			//Make a new one
			savedSceneName = SceneScript.sceneScript.currentSceneName;

			foreach (string item in itemList) {
				saveitemList.Add (item);
			}

			foreach (string item in itemListCondensed) {
				saveitemListCondensed.Add (item);
			}

			foreach (KeyValuePair<string, NPC> k in npcs) {
				savenpcs.Add (k.Key, k.Value);
			}

			foreach (KeyValuePair<string, Mess> k in messes) {
				savemesses.Add (k.Key, k.Value);
			}

			foreach (KeyValuePair<string, Item> k in items) {
				saveitems.Add (k.Key, k.Value);
			}

			foreach (KeyValuePair<string, int> k in bodyInRoom) {
				savebodyInRoom.Add (k.Key, k.Value);
			}
			foreach (KeyValuePair<string, int> k in corpseInRoom) {
				savecorpseInRoom.Add (k.Key, k.Value);
			}
		} else {
			saveitemList.Clear ();
			foreach (string item in itemList) {
				
				saveitemList.Add (item);
			}

			saveitemListCondensed.Clear ();
			foreach (string item in itemListCondensed) {
				saveitemListCondensed.Add (item);
			}

			foreach (KeyValuePair<string, NPC> k in npcs) {
				NPC n;
				savenpcs.TryGetValue (k.Key, out n);
				if (n != null) {
					n.overwrite (k.Value);
				}
			}

			foreach (KeyValuePair<string, Mess> k in messes) {
				Mess m;
				savemesses.TryGetValue (k.Key, out m);
				if (m != null) {
					m.overwrite (k.Value);
				}

			}

			foreach (KeyValuePair<string, Item> k in items) {
				Item i;
				saveitems.TryGetValue (k.Key, out i);
				if (i != null) {
					i.overwrite (k.Value);
				}
			}

			savebodyInRoom.Clear ();
			foreach (KeyValuePair<string, int> k in bodyInRoom) {
				savebodyInRoom.Add (k.Key, k.Value);
			}
			savecorpseInRoom.Clear ();
			foreach (KeyValuePair<string, int> k in corpseInRoom) {
				savecorpseInRoom.Add (k.Key, k.Value);
			}
		}

		savetimeRemaining = ctrl_game.timeRemaining;
		Debug.Log ("Saved turn count: " + savetimeRemaining);
		savekillCount = ctrl_game.killCount;
		saveunclaimedBodyCount = ctrl_game.unclaimedBodyCount;
		savebreadConsumed = ctrl_game.breadConsumed;		//How much bread the gorilla maid has consumed
		savebreadQuantity = ctrl_game.breadQuantity;
		savebreadSlot = ctrl_game.breadSlot;	//Which slot in the inventory contains bread

		saveencounterTimerRunning = ctrl_game.encounterTimerRunning;
		saveencounterTimer = ctrl_game.encounterTimer;

		saveendGame = ctrl_game.endGame;
		Debug.Log ("Saved");
	}

	public void LoadGame() {
		if (this.savedSceneName == null) {

		} else {
			SceneScript.sceneScript.currentSceneName = savedSceneName;

			itemList.Clear ();
			foreach (string item in saveitemList) {
				itemList.Add (item);
			}

			itemListCondensed.Clear ();
			foreach (string item in saveitemListCondensed) {
				itemListCondensed.Add (item);
			}

			foreach (KeyValuePair<string, NPC> k in savenpcs) {
				NPC n;
				npcs.TryGetValue (k.Key, out n);
				n.overwrite (k.Value);
			}

			foreach (KeyValuePair<string, Mess> k in messes) {
				Mess m;
				messes.TryGetValue (k.Key, out m);
				m.overwrite (k.Value);
			}

			foreach (KeyValuePair<string, Item> k in items) {
				Item i;
				items.TryGetValue (k.Key, out i);
				i.overwrite (k.Value);
			}

			bodyInRoom.Clear ();
			foreach (KeyValuePair<string, int> k in savebodyInRoom) {
				bodyInRoom.Add (k.Key, k.Value);
			}
			corpseInRoom.Clear ();
			foreach (KeyValuePair<string, int> k in savecorpseInRoom) {
				corpseInRoom.Add (k.Key, k.Value);
			}

			this.timeRemaining = savetimeRemaining;
			this.killCount = savekillCount;
			this.unclaimedBodyCount = saveunclaimedBodyCount;
			this.breadConsumed = savebreadConsumed;		//How much bread the gorilla maid has consumed
			this.breadQuantity = savebreadQuantity;
			this.breadSlot = savebreadSlot;	//Which slot in the inventory contains bread

			this.encounterTimerRunning = saveencounterTimerRunning;
			this.encounterTimer = saveencounterTimer;

			this.endGame = saveendGame;
			Debug.Log ("Saved");
			Controller_GUI.ctrl_gui.SetMessesText(GetMessCount());
			Controller_GUI.ctrl_gui.SetTimeText(this.timeRemaining);
			SceneScript.sceneScript.LoadScene (this.savedSceneName);
		}
	}
}
