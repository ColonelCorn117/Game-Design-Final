using System.Xml;
using System.Xml.Serialization;


[XmlRoot("npc")]
public class NPC : Problem {
	int dialogueLocation = 1;

	public bool Subdue(Action a) {
		this.exists = 0;
		return true;
	}

	public void Create() {
		this.exists = 1;
	}

	public void setDialogueLocation(int i) {
		dialogueLocation = i;
	}

	public int getDialogueLocation() {
		return dialogueLocation;
	}
}
