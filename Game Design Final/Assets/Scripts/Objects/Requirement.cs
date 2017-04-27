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



			String reqName = req.Substring(6);

			if (reqName == "Mop") {
				Item mop = Controller_Game.ctrl_game.ItemLookup (reqName);

				//Debug.Log (req + ": " + mop.possessed () + ", claimed: " + mop.isClaimed () + ",  consumed: " + mop.isConsumed ());
			}

			//Debug.Log ("req: " + req + ", name: " + reqName);
			if (req.StartsWith ("noreqs")) {
				//this prerequisiste is always satisfied
				return true;
			}
			if (req.StartsWith ("gained")) {
				//the player has an item with the name after "gained"
				if (!Controller_Game.ctrl_game.ItemLookup (reqName).possessed ()) {
					//Debug.Log ("item possessed: " + reqName);
					return false;
				}
			} else if (req.StartsWith ("nogain")) {
				//the player does not have an item with the name after "nogain"
				if (Controller_Game.ctrl_game.ItemLookup (reqName).possessed ()) {
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
				int numKilled = 0;
				if (reqKilled > numKilled) {
					return false;
				}
			} else if (req.StartsWith ("nokill")) {
				//the player has killed fewer than a certain number of people
				int reqKilled = int.Parse (reqName);
				//TODO: look up number of people killed
				int numKilled = 0;
				if (reqKilled < numKilled) {
					return false;
				}
			} else if (req.StartsWith ("remove")) {
				//the player has already removed a certain NPC
				if (Controller_Game.ctrl_game.NpcLookup (reqName).exists == 1) {
					return false;
				}
			} else if (req.StartsWith ("exists")) {
				//the player has not yet removed a certain NPC
				if (!(Controller_Game.ctrl_game.NpcLookup (reqName).exists == 1)) {
					return false;
				}

			} else if (req.StartsWith ("talked")) {
				string npcName = reqName.Substring (0, reqName.Length - 1);
				int dialogueReached = int.Parse (reqName.Substring (reqName.Length - 1));
				NPC n = Controller_Game.ctrl_game.NpcLookup (npcName);
				if (n.getDialogueLocation() < dialogueReached) {
					return false;
				}
			}
			else {
				//The prerequisite is not one of the predefined options.
				return false;
			}
		}

		return true;
	}
}
