using System.Xml;
using System.Xml.Serialization;


[XmlRoot("npc")]
public class NPC : Problem {
	int dialogueLocation = 1;

	public bool Subdue(Action a) {
		if (a.npcSubdued == this.name) {
			this.exists = 0;

			return true;
		}

		return false;
	}

	public void setDialogueLocation(int i) {
		dialogueLocation = i;
	}

	public int getDialogueLocation() {
		return dialogueLocation;
	}
}
