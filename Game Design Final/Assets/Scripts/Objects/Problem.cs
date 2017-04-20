﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

public class Problem : GenericGameObject {

	[XmlArray("conditionList"),XmlArrayItem("condition")]
	public List<Condition> conditions = new List<Condition>();
	//public ConditionList conditionList;
	public int exists; // Works like a bool, technically
}
