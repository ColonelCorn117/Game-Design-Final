using System.Xml;
using System.Xml.Serialization;


[XmlRoot("npc")]
public class NPC : Problem {


	public bool Subdue(Action a) {
		if (a.npcSubdued == this.name) {
			this.exists = 0;

			return true;
		}

		return false;
	}
}
