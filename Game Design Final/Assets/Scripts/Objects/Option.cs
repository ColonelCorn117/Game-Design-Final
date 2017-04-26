using System;


public class Option
{
	public string optionDescription;
	public Action action;

	public Option() {

	}

	public Option(string desc) {
		this.optionDescription = desc;
	}

	public Option(string desc, Action a) {
		this.optionDescription = desc;
		this.action = a;
	}
}


