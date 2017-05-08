using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Xml;

public class Controller_GUI : MonoBehaviour
{
	//This script needs to be attached to the canvas to work properly.

	//====================================================================================================
	//Variables
	//====================================================================================================

	public static Controller_GUI ctrl_gui;

	Image bgImage;
//	Text descrText;		//changed in SceneScript instead

	Text messesText;
	Text timeText;
	/*Button option1Btn;
	Button option2Btn;
	Button option3Btn;
	Button option4Btn;*/
	Image itemsListImage;
	Image detailImage;
	Image mapImage;
	public Image itemsBox;
	List<Text> itemsText = new List<Text>();	//Text components of the item slots



	//Values for old Items button implementation
//	Button invBtn;
//	float invBtnWidth;		//Default width of the expandable item list
//	float invBtnHeight;		//Default height "
//	public GameObject itemBtn;	//Item button prefab. Value set in editor

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
//			invBtnWidth = 80.0f;
//			invBtnHeight = 50.0f;

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
			case("Detail Image"):
				detailImage = image;
				break;
			case("Map Image"):
				mapImage = image;
				break;
			case("Items Box"):
				itemsBox = image;
				ToggleInventory();	//Hide inventory at start
				break;
			case("Titles Box"):
					List<Transform> children = new List<Transform>(GetComponentsInChildren<Transform>());	//gets child objects
					foreach (Transform child in children)
					{
						if (child.name == "Messes-Text")
						{
							messesText = child.GetComponent<Text>();
						}
						else if (child.name == "Time-Text")
						{
							timeText = child.GetComponent<Text>();
						}
					}
				break;
//			case("ItemsList Image"):				//Old Items button
//				itemsListImage = image;
//				itemsListImage.gameObject.SetActive(false);
//				break;
			default:
				//Debug.Log("Other image found: " + image.name);
				break;
			}
		}

		foreach (Text textComp in itemsBox.GetComponentsInChildren<Text>())
		{
			itemsText.Add(textComp);
		}
		itemsText.RemoveAt(0);	//Get rid of the first element since that one contains the "Items" title which we don't want to be changed
		itemsText.RemoveAt(itemsText.Count - 1);	//Same for the last element, since that's the "Hide Inventory" button

		/*
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
		}*/

		//descrText = GetComponentInChildren<Text>();



		//SetBackgroundImage(sp);
		//ScaleItemsList();
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

	public void SetMapImage(Sprite imageSource)
	{
		mapImage.sprite = imageSource;	
	}

	//----------------------------------------------------------------------------------------------------

//	public void SetDescriptionText(string text)
//	{
//		descrText.text = text;
//	}

	//----------------------------------------------------------------------------------------------------

	public void SetButtonText(Button button, string text)
	{
		button.GetComponent<Text>().text = text;
	}

	//----------------------------------------------------------------------------------------------------

	public void SetItemsText(List<string> itemList)
	{
		List<string> itemListCondensed = new List<string>();
		int breadSlot = Controller_Game.ctrl_game.breadSlot;
		int assignedSlots = 0;		//How many individual slots have been assigned
//		Debug.Log("breadSlot: " + breadSlot);
		for (int i=0; i<itemList.Count; i++)		//Iterate through our obtained items
		{
			if (i < itemsText.Count)	//Dont try to assign names to more slots than we have
			{
				if (Controller_Game.ctrl_game.ItemLookup(itemList[i]).name == "Bread")
				{
					if (breadSlot == -1)		//breadSlot gets reset to -1 every time the inventory is hidden (i.e. disabled) in this version of unity, so there isn't a need to reset breadSlot if breadQuantity == 0.
					{
						breadSlot = i;
						itemListCondensed.Add(itemList[i]);
						assignedSlots++;
					}
					itemsText[breadSlot].text = "Bread x" + Controller_Game.ctrl_game.breadQuantity;

				}
				else
				{
					itemsText[assignedSlots].text = Controller_Game.ctrl_game.ItemLookup(itemList[i]).name;
					itemListCondensed.Add(itemList[i]);
					assignedSlots++;
				}
			}
			else
			{
				break;
			}

		}
//		foreach (string item in itemListCondensed)	//Debug lines to test if the item list is shortened properly
//		{
//			Debug.Log("ILC: " + item);
//		}

		Controller_Game.ctrl_game.itemListCondensed = itemListCondensed;

		if (assignedSlots < itemsText.Count)	//We still have open inventory slots after going through our items. Gives slots blank name
		{
			for (int i=assignedSlots; i<itemsText.Count; i++)
			{
				itemsText[i].text = "";	
			}
		}
	}


	//----------------------------------------------------------------------------------------------------

	public void SetDetailImage(Sprite s) {
		//GameObject.Find ("Detail Image").GetComponent<Image> ().sprite = s;
		detailImage.sprite = s;
	}

	//----------------------------------------------------------------------------------------------------

	public void SetMessesText(int messCount) {
		messesText.text = (messCount).ToString ();
	}

	public void SetMessesText(Dictionary<string, Mess> messes)
	{
		int count = 0;
		foreach (KeyValuePair<string, Mess> mess in messes)
		{
			if (mess.Value.exists == 1)
			{
				count++;
				Debug.Log(mess.Value);
			}
		}
		messesText.text = (count + Controller_Game.ctrl_game.unclaimedBodyCount).ToString();
	}

	//----------------------------------------------------------------------------------------------------

	public void SetTimeText(float amount)
	{
		timeText.text = Mathf.Ceil(amount).ToString();
	}

	//----------------------------------------------------------------------------------------------------

	public void ToggleInventory()
	{
		itemsBox.gameObject.SetActive(!itemsBox.gameObject.activeSelf);
		if (itemsBox.gameObject.activeSelf)
		{
			this.SetItemsText (Controller_Game.ctrl_game.itemList);
		}
	}

	//----------------------------------------------------------------------------------------------------

	#region: Old Items button implementation. Button itself is still offscreen just in case
//	public void ToggleItemsList()
//	{
//		itemsListImage.gameObject.SetActive(!itemsListImage.gameObject.activeSelf);
//	}
//
//	//----------------------------------------------------------------------------------------------------
//
//	//Scales and repositions expandable item list based on number of items in inventory
//	public void ScaleItemsList()
//	{
//		int numItems = Controller_Game.ctrl_game.itemList.Count;
//		itemsListImage.rectTransform.sizeDelta = new Vector2(invBtnWidth * numItems, invBtnHeight);					//Increases height for each item. Width stays the same
//		itemsListImage.rectTransform.anchoredPosition = new Vector2(itemsListImage.rectTransform.sizeDelta.x / 2, 0);	//Moves expandable area up/down so that its bottom edge is near its anchor point (the "Items" button)
//
//		for (int i=0; i<Controller_Game.ctrl_game.itemList.Count; i++)
//		{
//			GameObject button = AddBtnToItemsList(invBtnWidth, invBtnWidth);
//			if (button.GetComponentInChildren<Text>())
//			{
//				//Debug.Log("Text Component Found");
//				button.GetComponentInChildren<Text>().text = Controller_Game.ctrl_game.itemList[i];
//			}
//
//			RectTransform buttonRect = (RectTransform)button.transform;
//			buttonRect.anchoredPosition = new Vector2((invBtnWidth / 2) + (i * invBtnWidth), 0);	//(invBtnHeight / 2) + (i * invBtnHeight)
//		}
//	}
//
//	//----------------------------------------------------------------------------------------------------
//
//	public GameObject AddBtnToItemsList(float width, float height)
//	{
//		return Instantiate(itemBtn, itemsListImage.rectTransform);
//	}
//
//	//----------------------------------------------------------------------------------------------------
//
//	//It's easier to get rid of every inventory item button an remake them than it is to delete one and move the above ones down
//	public void DestroyBtnsOnItemsList()
//	{
//		List<Button> buttons = new List<Button>(itemsListImage.GetComponentsInChildren<Button>());
//		foreach(Button button in buttons)
//		{
//			Destroy(button.gameObject);
//		}
//
//		//Readd buttons for items we still have
//		ScaleItemsList();
//	}
	#endregion

	//----------------------------------------------------------------------------------------------------






}
