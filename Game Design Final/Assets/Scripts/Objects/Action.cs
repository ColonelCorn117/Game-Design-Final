using System;

public class Action : GenericGameObject {
	public String nextScene;
	public String objectExamined;
	public String itemGained;
	public String itemUsed;
	public String npcSubdued;
	public String messResolved;


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
