using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class TDS_MainMenu : MonoBehaviour 
{
	/* TDS_MainMenu :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

	#region Methods

	#region Original Methods
    public void LoadLevel()
    {
        SceneManager.LoadScene(1); 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
	#endregion

	#endregion
}
