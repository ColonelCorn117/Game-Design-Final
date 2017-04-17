using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller_GUI : MonoBehaviour
{
	//====================================================================================================
	//Variables
	//====================================================================================================

	public static Controller_GUI ctrl_gui;

	public Image bgImage;
	public Text descrText;

	//List of every image used. Values set in editor
	public Sprite test;


	//====================================================================================================
	//Start Stuff and Updaters
	//====================================================================================================

	void Awake()
	{
		if (ctrl_gui == null)
		{
			DontDestroyOnLoad(gameObject);
			ctrl_gui = this;
		}
		else if (ctrl_gui != this)
		{
			Destroy(gameObject);
		}
	}

	//----------------------------------------------------------------------------------------------------

	void Start()
	{
		bgImage = GetComponentInChildren<Image>();
		SetBackgroundImage(test);
	}


	//====================================================================================================
	//Setters
	//====================================================================================================

	public void SetBackgroundImage(Sprite imageSource)
	{
		bgImage.sprite = imageSource;	
	}

	//----------------------------------------------------------------------------------------------------

	public void SetDescriptionText(string text)
	{
		descrText.text = text;
	}
}
