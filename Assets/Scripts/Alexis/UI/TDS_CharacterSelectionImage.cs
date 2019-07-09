using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

[Serializable]
public class TDS_CharacterSelectionImage 
{
    /* TDS_CharacterSelectionImage :
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
    [SerializeField] private Image characterImage;
    public Image CharacterImage { get { return characterImage; } }
    [SerializeField] private PlayerType characterType = PlayerType.Unknown;
    public PlayerType CharacterType { get { return characterType; } }
    private bool canBeSelected = true; 
    public bool CanBeSelected
    {
        get { return canBeSelected; }
        set { canBeSelected = value; }
    }

	#endregion
}
