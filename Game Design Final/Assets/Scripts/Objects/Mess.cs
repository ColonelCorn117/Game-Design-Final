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

	public bool Unclean(Action a) {
		this.exists = 1;

		return true;
	}
}
