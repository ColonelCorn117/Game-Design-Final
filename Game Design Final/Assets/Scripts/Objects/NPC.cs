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

	public NPC copy() {
		NPC p = new NPC ();
		p.exists = this.exists;
		p.id = this.id;
		p.name = this.name;
		p.description = this.description;
		p.conditions = this.conditions;
		p.dialogueLocation = this.dialogueLocation;
		return p;
	}

	public bool compare(NPC n) {
		return base.compare(n) && (this.dialogueLocation == n.getDialogueLocation ());
	}

	public void overwrite(NPC n) {
		base.overwrite (n);
		this.dialogueLocation = n.getDialogueLocation ();
	}
}
