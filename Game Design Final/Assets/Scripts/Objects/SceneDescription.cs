using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("SceneDescription")]
public class SceneDescription : GenericGameObject
{
	public string background;
	//public OptionList optionList;

	[XmlArray("conditionList"),XmlArrayItem("condition")]
	public List<Condition> optionList = new List<Condition>();

	[XmlArray("npcList"),XmlArrayItem("npc")]
	public List<String> npcList = new List<String>();

	[XmlArray("messList"),XmlArrayItem("mess")]
	public List<String> messList = new List<String>();

	[XmlArray("itemList"),XmlArrayItem("item")]
	public List<String> itemList = new List<String>();


}
