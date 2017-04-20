using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("SceneDescription")]
public class SceneDescription : GenericGameObject
{
	public String background;
	//public OptionList optionList;

	[XmlArray("optionList"),XmlArrayItem("option")]
	public List<Option> optionList = new List<Option>();

	[XmlArray("npcList"),XmlArrayItem("npc")]
	public List<String> npcList = new List<String>();

	[XmlArray("messList"),XmlArrayItem("mess")]
	public List<String> messList = new List<String>();


}
