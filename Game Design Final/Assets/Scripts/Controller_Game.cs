using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Game : MonoBehaviour
{
	//This script needs to be attached to the canvas to work properly.

	//====================================================================================================
	//Variables
	//====================================================================================================

	public static Controller_Game ctrl_game;
	public static Controller_GUI ctrl_gui;

	public List<string> items = new List<string>();


	//====================================================================================================
	//Start Stuff and Updaters
	//====================================================================================================

	void Awake()
	{
		if (ctrl_game == null)
		{
			DontDestroyOnLoad(gameObject);
			ctrl_game = this;
			ctrl_gui = Controller_GUI.ctrl_gui;
			items.Add("Mop");
		}
		else if (ctrl_game != this)
		{
			Destroy(gameObject);
		}
	}


	//====================================================================================================
	//Other Functions
	//====================================================================================================

	public void AddItemToInv()//string item)
	{
		items.Add("item" + items.Count);
		ctrl_gui.ScaleItemsList();
		Debug.Log("Item Added: " + items[items.Count - 1]);
	}

	//----------------------------------------------------------------------------------------------------

	public void RemoveItemFromInv()//string item)
	{
		items.Remove("item" + (items.Count - 1));
		ctrl_gui.DestroyBtnsOnItemsList();
		if (items.Count > 0)
		{
			Debug.Log("Item Removed: " + items[items.Count - 1]);
		}

	}
}
