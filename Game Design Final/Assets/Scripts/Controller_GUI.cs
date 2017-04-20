﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller_GUI : MonoBehaviour
{
	//This script needs to be attached to the canvas to work properly.

	//====================================================================================================
	//Variables
	//====================================================================================================

	public static Controller_GUI ctrl_gui;

	Image bgImage;
	Text descrText;
	Button option1Btn;
	Button option2Btn;
	Button option3Btn;
	Button option4Btn;
	Button invBtn;
	Image itemsListImage;

	float invBtnWidth;		//Default width of the expandable item list
	float invBtnHeight;		//Default height "

	//Item button prefab. Value set in editor
	public GameObject itemBtn;

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
			invBtnWidth = 80.0f;
			invBtnHeight = 50.0f;

		}
		else if (ctrl_gui != this)
		{
			Destroy(gameObject);
		}
	}

	//----------------------------------------------------------------------------------------------------

	void Start()
	{
		foreach (Image image in GetComponentsInChildren<Image>())
		{
			switch (image.name)
			{
			case("Background Image"):
				bgImage = image;
				break;
			case("ItemsList Image"):
				itemsListImage = image;
				itemsListImage.gameObject.SetActive(false);
				break;
			default:
				//Debug.Log("Other image found: " + image.name);
				break;
			}
		}

		foreach (Button button in GetComponentsInChildren<Button>())
		{
			switch (button.name)
			{
			case("Option 1 Button"):
				option1Btn = button;
				break;
			case("Option 2 Button"):
				option2Btn = button;
				break;
			case("Option 3 Button"):
				option3Btn = button;
				break;
			case("Option 4 Button"):
				option4Btn = button;
				break;
			case("Inventory Button"):
				invBtn = button;
				break;
			default:
				//Debug.Log("Other button found: " + button.name);
				break;
			}
		}
		descrText = GetComponentInChildren<Text>();
		SetBackgroundImage(test);
		ctrl_gui.ScaleItemsList();
	}


	//====================================================================================================
	//Setters
	//====================================================================================================

	//Called from the OnClick of invBtn
	public void SetBackgroundImage(Sprite imageSource)
	{
		bgImage.sprite = imageSource;	
	}

	//----------------------------------------------------------------------------------------------------

	public void SetDescriptionText(string text)
	{
		descrText.text = text;
	}

	//----------------------------------------------------------------------------------------------------

	public void SetButtonText(Button button, string text)
	{
		button.GetComponent<Text>().text = text;
	}


	//====================================================================================================
	//Other Functions
	//====================================================================================================

	public void ToggleItemsList()
	{
		itemsListImage.gameObject.SetActive(!itemsListImage.gameObject.activeSelf);
	}

	//----------------------------------------------------------------------------------------------------

	//Scales and repositions expandable item list based on number of items in inventory
	public void ScaleItemsList()
	{
		int numItems = Controller_Game.ctrl_game.items.Count;
		itemsListImage.rectTransform.sizeDelta = new Vector2(invBtnWidth * numItems, invBtnHeight);					//Increases height for each item. Width stays the same
		itemsListImage.rectTransform.anchoredPosition = new Vector2(itemsListImage.rectTransform.sizeDelta.x / 2, 0);	//Moves expandable area up/down so that its bottom edge is near its anchor point (the "Items" button)

		for (int i=0; i<Controller_Game.ctrl_game.items.Count; i++)
		{
			GameObject button = AddBtnToItemsList(invBtnWidth, invBtnWidth);
			if (button.GetComponentInChildren<Text>())
			{
				//Debug.Log("Text Component Found");
				button.GetComponentInChildren<Text>().text = Controller_Game.ctrl_game.items[i];
			}

			RectTransform buttonRect = (RectTransform)button.transform;
			buttonRect.anchoredPosition = new Vector2((invBtnWidth / 2) + (i * invBtnWidth), 0);	//(invBtnHeight / 2) + (i * invBtnHeight)
		}
	}

	//----------------------------------------------------------------------------------------------------

	public GameObject AddBtnToItemsList(float width, float height)
	{
		return Instantiate(itemBtn, itemsListImage.rectTransform);
	}

	//----------------------------------------------------------------------------------------------------

	//It's easier to get rid of every inventory item button an remake them than it is to delete one and move the above ones down
	public void DestroyBtnsOnItemsList()
	{
		List<Button> buttons = new List<Button>(itemsListImage.GetComponentsInChildren<Button>());
		foreach(Button button in buttons)
		{
			Destroy(button.gameObject);
		}

		//Readd buttons for items we still have
		ScaleItemsList();
	}
}