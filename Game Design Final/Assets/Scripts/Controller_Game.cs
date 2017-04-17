using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Game : MonoBehaviour
{
	//====================================================================================================
	//Variables
	//====================================================================================================

	public static Controller_Game ctrl_game;
	public static Controller_GUI ctrl_gui;


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
		}
		else if (ctrl_game != this)
		{
			Destroy(gameObject);
		}
	}


}
