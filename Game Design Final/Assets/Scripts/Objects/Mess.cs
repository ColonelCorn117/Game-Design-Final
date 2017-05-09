using System;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

[XmlRoot("mess")]
public class Mess : Problem {


	public bool Cleanup(Action a) {
		this.exists = 0;
		return true;
	}

	public bool compare(Mess m) {
		//Debug.Log (m.name + " exists: " + m.exists + ", " + this.name + " exists: " + this.exists);
		return (this.id == m.id) && (this.exists == m.exists);
	}

	public new Mess copy() {
		Mess p = new Mess ();
		p.exists = this.exists;
		p.id = this.id;
		p.name = this.name;
		p.description = this.description;
		p.conditions = this.conditions;
		return p;
	}

	public bool Unclean(Action a) {
		this.exists = 1;

		return true;
	}
}
