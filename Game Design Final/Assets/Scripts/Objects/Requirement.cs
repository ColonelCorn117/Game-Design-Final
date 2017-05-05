using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Requirement {
	
	[XmlElement("prereq")]
	public List<String> prereqs = new List<String>();

	public bool Satisfied(Action a) {
		foreach (String req in prereqs) {
			if (req != "") {


				String reqName = req.Substring (6);


				//Debug.Log ("req: " + req + ", name: " + reqName);
				if (req.StartsWith ("noreqs")) {
					//this prerequisiste is always satisfied
					return true;
				}
				if (req.StartsWith ("gained")) {
					//Debug.Log ("Searching for " + reqName + " in inventory");
					//the player has an item with the name after "gained"
					//if (!Controller_Game.ctrl_game.itemList.Contains (reqName)) {
					//Debug.Log("id: " + Controller_Game.ctrl_game.ItemLookup(reqName).id);
					Item i = Controller_Game.ctrl_game.ItemLookup (reqName);
					if (i != null && i.possessed ()) {
						//Debug.Log (reqName + " found");
					} else {
						//Debug.Log (reqName + " not found");
						//Debug.Log ("item possessed: " + reqName);
						return false;
					}

				} else if (req.StartsWith ("nogain")) {
					//the player does not have an item with the name after "nogain"
					Item i = Controller_Game.ctrl_game.ItemLookup (reqName);
					if (i != null && i.possessed ()) {
						//Debug.Log ("item not possessed: " + reqName);
						return false;
					}
				} else if (req.StartsWith ("expire")) {
					//the player uses an item with the name after "gained"
					if (a.itemUsed != reqName) {
						return false;
					}
				} else if (req.StartsWith ("killed")) {
					//the player has killed more than a certain number of people
					int reqKilled = int.Parse (reqName);
					//TODO: look up number of people killed
					int numKilled = Controller_Game.ctrl_game.killCount;
					if (reqKilled > numKilled) {
						return false;
					}
				} else if (req.StartsWith ("nokill")) {
					//the player has killed fewer than a certain number of people
					int reqKilled = int.Parse (reqName);
					//TODO: look up number of people killed
					int numKilled = Controller_Game.ctrl_game.killCount;
					if (reqKilled < numKilled) {
						return false;
					}
				} else if (req.StartsWith ("remove")) {
					//the player has already removed a certain NPC
					if (!(Controller_Game.ctrl_game.NpcLookup (reqName).exists == 0)) {
						return false;
					}
				} else if (req.StartsWith ("exists")) {
					//the player has not yet removed a certain NPC
					if (!(Controller_Game.ctrl_game.NpcLookup (reqName).exists == 1)) {
						return false;
					}

				} else if (req.StartsWith ("timelo")) {
					//the encounter timer has reached at least the specified threshold.
					int reqTime = int.Parse (reqName);
					if (Controller_Game.ctrl_game.encounterTimer < reqTime) {
						return false;
					}

				} else if (req.StartsWith ("timehi")) {
					//the encounter timer has not exceeded the specified threshold.
					int reqTime = int.Parse (reqName);
					if (Controller_Game.ctrl_game.encounterTimer > reqTime) {
						return false;
					}

				} 
				else if (req.StartsWith ("talked")) {
					string npcName = reqName.Substring (0, reqName.Length - 1);
					int dialogueReached = int.Parse (reqName.Substring (reqName.Length - 1));
					NPC n = Controller_Game.ctrl_game.NpcLookup (npcName);
					if (n.getDialogueLocation () < dialogueReached) {
						return false;
					}
				} else if (req.StartsWith ("messed")) {
					//Debug.Log("Messed: " + reqName + " exists? " + Controller_Game.ctrl_game.MessLookup (reqName).exists);
					//the player has not yet removed a certain mess
					if (!(Controller_Game.ctrl_game.MessLookup (reqName).exists == 1)) {
						return false;
					}

				} else if (req.StartsWith ("nomess")) {
					//the player has removed a certain mess
					//Debug.Log("NoMess: " + reqName + " exists? " + Controller_Game.ctrl_game.MessLookup (reqName).exists);
					if ((Controller_Game.ctrl_game.MessLookup (reqName).exists == 1)) {
						return false;
					}

				}else if (req.StartsWith ("loaves")) {
					// the player has a certain amount of bread in inventory.
					int reqBread = int.Parse (reqName);
					if (Controller_Game.ctrl_game.breadQuantity < reqBread) {
						return false;
					}

				} else if (req.StartsWith ("invblk")) {
					// the player has bread in the first N slots of their inventory.
					int breadCount = int.Parse (reqName);
					if (Controller_Game.ctrl_game.itemList.Count > 0) {
						for (int i = 0; i < breadCount; ++i) {
							if (Controller_Game.ctrl_game.itemList [i] != "Bread") {
								Debug.Log ("Bread Count: " + i);
								return false;	
							}
						}
					} else {
						return false;
					}
				} else if (req.StartsWith ("curloc")) {
					// the player has fed at least N loaves of bread to the gorilla
					//Debug.Log ("Bread Needed: " + breadEaten + ", Bread Eaten: " + Controller_Game.ctrl_game.breadConsumed);
					if (SceneScript.sceneScript.currentSceneName != "reqName") {
						return false;
					}
				}else if (req.StartsWith ("devour")) {
					// the player has fed at least N loaves of bread to the gorilla
					int breadEaten = int.Parse (reqName);
					//Debug.Log ("Bread Needed: " + breadEaten + ", Bread Eaten: " + Controller_Game.ctrl_game.breadConsumed);
					if (Controller_Game.ctrl_game.breadConsumed < breadEaten) {
						return false;
					}
				} else if (req.StartsWith ("nofeed")) {
					// the player has fed at most N loaves of bread to the gorilla
					int breadEaten = int.Parse (reqName);
					//Debug.Log ("Bread Needed: " + breadEaten + ", Bread Eaten: " + Controller_Game.ctrl_game.breadConsumed);
					if (Controller_Game.ctrl_game.breadConsumed > breadEaten) {
						return false;
					}
				} else if (req.StartsWith ("breadn")) {
					// the player has acquired/used at most N loaves of bread
					int breadLimit = int.Parse (reqName);
					//Debug.Log ("Bread Needed: " + breadEaten + ", Bread Eaten: " + Controller_Game.ctrl_game.breadConsumed);
					if ((Controller_Game.ctrl_game.breadConsumed + Controller_Game.ctrl_game.breadQuantity) > breadLimit) {
						return false;
					}
				} else if (req.StartsWith ("breadm")) {
					// the player has acquired/used at least M loaves of bread
					int breadLimit = int.Parse (reqName);
					//Debug.Log ("Bread Needed: " + breadEaten + ", Bread Eaten: " + Controller_Game.ctrl_game.breadConsumed);
					if ((Controller_Game.ctrl_game.breadConsumed + Controller_Game.ctrl_game.breadQuantity) < breadLimit) {
						return false;
					}
				} else {
					//The prerequisite is not one of the predefined options.
					return false;
				}
			}
		}

		return true;
	}
}
