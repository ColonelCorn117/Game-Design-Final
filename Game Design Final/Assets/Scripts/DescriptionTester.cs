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
		public String nextScene;
	}

	//[XmlArray("OptionList"),XmlArrayItem("Option")]
	public class OptionList 
	{
		public List<Option> Options = new List<Option>();



		/*public Option1 option1;
			public Option2 option2;
			public Option3 option3;
			public Option4 option4;*/
	}

	/*
	public Option1 option1;
	public Option2 option2;
	public Option3 option3;
	public Option4 option4;
	*/



	/*public class Option1 : Option {
	public String optionDescription;
	public String nextScene;
	}
	public class Option2 : Option {
		public String optionDescription;
		public String nextScene;
	}
	public class Option3 : Option {
		public String optionDescription;
		public String nextScene;
	}
	public class Option4 : Option {
		public String optionDescription;
		public String nextScene;
	}*/


}




