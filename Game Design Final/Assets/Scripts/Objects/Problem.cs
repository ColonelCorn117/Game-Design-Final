using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class Problem : GenericGameObject {

	[XmlArray("clearList"),XmlArrayItem("condition")]
	public List<Condition> clearingConditions = new List<Condition>();

	[XmlArray("conditionList"),XmlArrayItem("condition")]
	public List<Condition> conditions = new List<Condition>();
	//public ConditionList conditionList;
	public int exists; // Works like a bool, technically




	public Problem() {
		this.exists = 0;
		this.name = "";
		this.description = "";
	}

	public bool compare(Problem p) {
		//Debug.Log (p.name + " exists: " + p.exists + ", " + this.name + " exists: " + this.exists);
		return (this.id == p.id) && (this.exists == p.exists);
	}

	public Problem copy() {
		Problem p = new Problem ();
		p.exists = this.exists;
		p.id = this.id;
		p.name = this.name;
		p.description = this.description;
		p.conditions = this.conditions;
		return p;
	}

	public void overwrite(Problem p) {
		this.id = p.id;
		this.name = p.name;
		this.description = p.description;
		this.exists = p.exists;

		this.clearingConditions = p.clearingConditions;
		this.conditions = p.conditions;
	}
}
