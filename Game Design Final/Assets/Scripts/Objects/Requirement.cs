using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Requirement {
	[XmlElement("prereq")]
	public List<String> prereqs = new List<String>();

	public bool Satisfied(Action a) {

		foreach (String req in prereqs) {
			String reqName = req.Substring(6);

			if(req.StartsWith("gained")) {
				//the player has an item with the name after "gained"
				if (!Controller_Game.ctrl_game.items.Contains (reqName)) {
					return false;
				}
			}
			else if (req.StartsWith("expire")) {
				//the player uses an item with the name after "gained"
				if (a.itemUsed != reqName) {
					return false;
				}
			}
			else if(req.StartsWith("killed")) {
				//the player has killed more than a certain number of people
				int reqKilled = int.Parse(reqName);
				//TODO: look up number of people killed
				int numKilled = 0;
				if(reqKilled > numKilled) {
					return false;
				}
			}
			else if(req.StartsWith("nokill")) {
				//the player has killed fewer than a certain number of people
				int reqKilled = int.Parse(reqName);
				//TODO: look up number of people killed
				int numKilled = 0;
				if(reqKilled < numKilled) {
					return false;
				}
			}
			else if(req.StartsWith("remove")) {
				//the player has already removed a certain NPC
				//TODO: this


			}
			else if(req.StartsWith("exists")) {
				//the player has not yet removed a certain NPC
				//TODO: this

			}
			else {
				return false;
			}
		}

		return true;
	}
}
