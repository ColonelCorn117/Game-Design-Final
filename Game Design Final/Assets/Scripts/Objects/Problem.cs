using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

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

	public void overwrite(Problem p) {
		this.id = p.id;
		this.name = p.name;
		this.description = p.description;
		this.exists = p.exists;

		this.clearingConditions = p.clearingConditions;
		this.conditions = p.conditions;
	}
}
