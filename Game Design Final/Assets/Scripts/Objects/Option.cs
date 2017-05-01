using System;
using System.Xml;
using System.Xml.Serialization;


public class Option : GenericGameObject
{
	[XmlElement("description")]
	public new string description;

	public Action action;

	public Option() {

	}

	public Option(string desc) {
		this.description = desc;
	}

	public Option(string desc, Action a) {
		this.description = desc;
		this.action = a;
	}
}


