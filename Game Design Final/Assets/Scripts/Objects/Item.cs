using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[XmlRoot("item")]
public class Item : GenericGameObject {

	public Action action;
	public int duplicable = 0;
	int unclaimed = 1;
	int consumed = 0;
	public int exists;

	[XmlArray("conditionList"),XmlArrayItem("condition")]
	public List<Condition> conditions = new List<Condition>();

	public Item() {
		action = new Action ();
		//singleton = 1;
	}

	public Item(string n) {
		this.name = n;
		action = new Action ();

		action.itemGained = n;
		exists = 1;
		//singleton = 1;
	}

	public void setName(string n) {
		this.name = n;
		action.itemGained = n;
	}

	public bool isClaimed() {
		//return (this.consumed == 1) || Controller_Game.ctrl_game.itemList.Contains (this.name);
		return unclaimed == 0;
	}

	// like claim(), but doesn't check corpse or body status
	public void forceClaim() {
		unclaimed = 0;
	}

	public void claim() {
		if (this.name == "Corpse") {
			unclaimed = 0;
			//Controller_Game.ctrl_game.ItemLookup("Body").forceClaim();

		} else if(this.name == "Body") {
			unclaimed = 0;
			//Controller_Game.ctrl_game.ItemLookup("Corpse").forceClaim();
		} else {
			unclaimed = 0;
		}

	}

	public void create() {
		exists = 1;
	}

	public void consume() {
		

		if (duplicable == 1) {
			consumed = 0;
			unclaimed = 1;
		} else {
			consumed = 1;
		}
	}

	public bool isConsumed() {
		return consumed == 1;
	}

	public bool possessed() {
		//return Controller_Game.ctrl_game.itemList.Contains (this.name);
		//Debug.Log("id: " + id + ", unclaimed: " + unclaimed + ", consumed: " + consumed);
		return (unclaimed == 0 && consumed == 0);
	}

	public bool isViewableInRoom() {
		return isViewableInRoom ("");
	}

	// Given that this item is listed as being in a given room, should it appear as viewable
	public bool isViewableInRoom(string roomName) {
		//Debug.Log("viewable item name: " + this.name);
		if (name == "Body") {
			return Controller_Game.ctrl_game.BodiesVisible (roomName) > 0;
		} else if(name == "Corpse") {
			return Controller_Game.ctrl_game.CorpsesVisible (roomName) > 0;
		} else {
			// case 0: the item doesn't exist
			if (exists == 0) {
				return false;
			}
			// case 1: it's in the room
			if (!(isClaimed ())) {
				return true;
			}

			// case 2: it has been in inventory at one point
			else if (isClaimed ()) {
				// case 2a: the object is a singleton (and therefore it can't be in the room because it's in inventory or it was consumed): return false
				// case 2b: the object is not a singleton (and therefore could exist in the room as well as inventory): return true
				//Debug.Log("claimed item name: " + this.name);
				return (duplicable == 1); 
			}
				
			// case 3: it's been consumed
			// This case shouldn't come up ever, since the item must have been in inventory to be consumed
			// if for some reason, the above assumption is incorrect, adjust the conditional statements in this function to make the below code work
			/*
			if (isConsumed()) {
				return false;
			}
			 */
		}
		return true;
	}
}
