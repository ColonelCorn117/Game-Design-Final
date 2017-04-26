using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Condition : GenericGameObject {

	[XmlElement("requirement")]
	public List<Requirement> requirement = new List<Requirement> ();

	[XmlElement("action")]
	public Action action = new Action();

	public bool Satisfied(Action a) {
		if ((requirement == null) || (requirement.Count < 1)) {
			return true;
		}
		foreach (Requirement r in requirement) {
			if (r.Satisfied (a)) {
				return true;
			}
		}
		return false;
	}

	public bool Satisfied() {
		if ((requirement == null) || (requirement.Count < 1)) {
			Debug.Log ("no requirement object");
			return true;
		}

		Action a = new Action ();

		foreach (Requirement r in requirement) {
			if (r.Satisfied (a)) {
				return true;
			}
		}
		return false;
	}
}
