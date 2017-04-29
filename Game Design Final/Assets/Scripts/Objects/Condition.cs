using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Condition : GenericGameObject {

	[XmlElement("requirement")]
	public List<Requirement> requirement = new List<Requirement> ();

	public string additionalDescription;

	[XmlElement("action")]
	public Action action = new Action();

	public Condition() {

	}

	public Condition(string desc) {
		this.description = desc;
	}

	public Condition(string desc, Action a) {
		this.description = desc;
		this.action = a;
	}

	public bool Satisfied(Action a) {
		Debug.Log ("Requirement length: " + requirement.Count);
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
		Debug.Log ("Requirement length: " + requirement.Count);
		if ((requirement == null) || (requirement.Count < 1)) {
			//Debug.Log ("no requirement object");
			return true;
		}

		Action a = new Action ();

		foreach (Requirement r in requirement) {
			//Debug.Log (r.prereqs);
			if (r.Satisfied (a)) {
				return true;
			}
		}
		return false;
	}
}
