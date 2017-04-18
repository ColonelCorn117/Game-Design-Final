using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

[XmlRoot("SceneDescription")]
public class SceneDescription
{
	public String sceneName;
	public String description;

	public OptionList optionList;


	public class Option
	{
		public String optionDescription;
		public Action action;
	}

	public class Action {
		public String nextScene;
	}
		
	public class OptionList 
	{
		[XmlElement("option")]
		public List<Option> Options = new List<Option>();




	}
}




