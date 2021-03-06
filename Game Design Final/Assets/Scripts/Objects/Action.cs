﻿using System;

public class Action : GenericGameObject {
	public string nextScene;
	public string objectExamined;
	public string itemGained;
	public string itemUsed;
	public string itemCreated;
	public string npcSubdued;
	public string npcCreated;
	public int kill;
	public string messResolved;
	public string messCreated;
	public int startTimer;
	public int stopTimer;
	public float timeUsed = -1.0f;		//Negative because I doubt we'll ever add time to the clock and we want some actions (such as dialogue) to not take up time, so we can't use 0.0f to denote "unassigned time".


	public Action() {
		timeUsed = -1.0f;
	}

	public Action(string name) {
		this.name = name;
	}

	public Action(string name, string next, string objectToExamine) {
		this.name = name;
		this.nextScene = next;
		this.objectExamined = objectToExamine;
	}

	public Action(string name, string next, string objectToExamine, string gainItem) {
		this.name = name;
		this.nextScene = next;
		this.objectExamined = objectToExamine;
		this.itemGained = gainItem;
	}

	public Action(string name, string next, string objectToExamine, string gainItem, float timeUsed) {
		this.name = name;
		this.nextScene = next;
		this.objectExamined = objectToExamine;
		this.itemGained = gainItem;
		this.timeUsed = timeUsed;
	}

	public Action(string name, float timeUsed)
	{
		this.name = name;
		this.timeUsed = timeUsed;
	}
}
