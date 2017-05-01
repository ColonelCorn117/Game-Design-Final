using System;

public class Action : GenericGameObject {
	public string nextScene;
	public string objectExamined;
	public string itemGained;
	public string itemUsed;
	public string itemCreated;
	public string npcSubdued;
	public int kill;
	public string messResolved;
	public string messCreated;


	public Action() {

	}

	public Action(string name) {
		this.name = name;
	}

	public Action(string name, string next, string objectToExamine) {
		this.name = name;
		this.nextScene = next;
		this.objectExamined = objectToExamine;
	}

	public Action(string name, string next, string objectToExamine,string gainItem) {
		this.name = name;
		this.nextScene = next;
		this.objectExamined = objectToExamine;
		this.itemGained = gainItem;
	}
}
