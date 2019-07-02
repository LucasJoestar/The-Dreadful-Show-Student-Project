using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class TDS_RoomSelectionElement : MonoBehaviour
{
    /* TDS_RoomSelectionElement :
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

    #region Fields / Properties
    [SerializeField] private Button roomSelectionButton;
    [SerializeField] private TMPro.TMP_Text playerCountText; 
    
    public Button RoomSelectionButton { get { return roomSelectionButton; } }

    public string RoomName
    {
        get
        {
            return roomSelectionButton.gameObject.name;
        }
    }

    private int playerCount = 0; 
    public int PlayerCount
    {
        get
        {
            return playerCount;
        }
        set
        {
            playerCount = value;
            if (playerCountText)
                playerCountText.text = $"{playerCount}/4"; 
        }
    }

	#endregion
}
