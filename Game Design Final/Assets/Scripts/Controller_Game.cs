using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller_Game : MonoBehaviour
{
	//This script needs to be attached to the canvas to work properly.

	//====================================================================================================
	//Variables
	//====================================================================================================

	public static Controller_Game ctrl_game;
	public static Controller_GUI ctrl_gui;

	public List<string> items = new List<string>();
	public Dictionary<string, NPC> npcs = new Dictionary<string, NPC>();
	public Dictionary<string,Mess> messes = new Dictionary<string, Mess>();

	Action[] buttonActions = new Action[9]; // buttonActions[0] is empty. the action for btn1 is found in buttonActions[1].
	//int numButtonActions = 1; // first 'empty slot' that can be filled in buttonActions.

	//====================================================================================================
	//Start Stuff and Updaters
	//====================================================================================================

	void Awake()
	{
		if (ctrl_game == null)
		{
			DontDestroyOnLoad(gameObject);
			ctrl_game = this;
			ctrl_gui = Controller_GUI.ctrl_gui;
			items.Add("Mop");
		}
		else if (ctrl_game != this)
		{
			Destroy(gameObject);
		}
	}


	//====================================================================================================
	//Other Functions
	//====================================================================================================

	public void AddItemToInv()//string item)
	{
		items.Add("item" + items.Count);
		ctrl_gui.ScaleItemsList();
		Debug.Log("Item Added: " + items[items.Count - 1]);
	}

	//----------------------------------------------------------------------------------------------------

	public void RemoveItemFromInv()//string item)
	{
		items.Remove("item" + (items.Count - 1));
		ctrl_gui.DestroyBtnsOnItemsList();
		if (items.Count > 0)
		{
			Debug.Log("Item Removed: " + items[items.Count - 1]);
		}

	}


	public NPC npcLookup(string s) {
		NPC n;

		npcs.TryGetValue (s, out n);

		if (n != null) {
			return n;
		}
		return new NPC();
	}

	public Mess messLookup(string s) {
		Mess m;

		messes.TryGetValue (s, out m);

		if (m != null) {
			return m;
		}
		return new Mess();
	}

	public void performAction(Action a) {

		//Debug.Log ("action performing: " + a.name);
		// if an action consumes an item, delete that item from inventory
		//TODO: this
		if (a.itemUsed != null && a.itemUsed != "") {

		}

		// if an action cleans up a mess, clean it up
		if (a.messResolved != null && a.messResolved != "") {
			messLookup (a.messResolved).Cleanup (a);
			// Note: this method returns a bool regarding whether it succeeded
			// it currently isn't used for anything
		}

		// if an action removes an NPC, do that
		if (a.npcSubdued != null && a.npcSubdued != "") {
			npcLookup (a.npcSubdued).Subdue (a);
			// Note: this method returns a bool regarding whether it succeeded
			// it currently isn't used for anything
		}


		// if an action picks up an item, add that item to inventory and mark it as taken from the room
		//TODO: this
		if (a.itemGained != null && a.itemGained != "") {

		}

		// if the player is trying to examine an item, mess, or NPC, set up the screen to do that.
		if (a.objectExamined != null && a.objectExamined != "") {
			string addedText = "";


			//NPC
			NPC n = npcLookup (a.objectExamined);
			if (n.exists == 1) {
				//Debug.Log ("examining " + n.name);


				addedText = "\n\n" + n.description;
			} else {
				//Debug.Log ("No such NPC");
				//Mess
				Mess m = messLookup (a.objectExamined);
				if (m.exists == 1) {
					//Debug.Log ("examining " + m.name);

					addedText = "\n\n" + m.description;
				} else {

					//Item
					//TODO: This
				}
			}

			if (addedText != "") {
				string desc = GameObject.Find ("Description Text").GetComponentInChildren<Text> ().text;
				desc += addedText;
				GameObject.Find ("Description Text").GetComponentInChildren<Text> ().text = desc;
			}
		}

		if(a.nextScene != null && a.nextScene != "") {
			SceneScript.sceneScript.LoadScene (a.nextScene);

		}

	}

	public void performAction(int btnNbr) {
		//Debug.Log ("Action: " + buttonActions [btnNbr].name);
		performAction (buttonActions [btnNbr]);
	}

	public bool addButtonAction(Action a, int index) {
		if (index < buttonActions.Length) {

			if (a.name != null && a.name != "") {
				
			}
			buttonActions [index] = a;
			return true;
		}

		return false;
	}

	public void clearButtonActions() {
		
	}


}
