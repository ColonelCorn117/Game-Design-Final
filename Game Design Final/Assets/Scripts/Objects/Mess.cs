using System;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("mess")]
public class Mess : Problem {


	public bool Cleanup(Action a) {
		if (a.messResolved == this.name) {
			this.exists = 0;

			return true;
		}

		return false;
	}
}
