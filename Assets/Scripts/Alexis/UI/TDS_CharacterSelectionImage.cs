using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
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
    [SerializeField] private Image characterImage = null;
    public Image CharacterImage { get { return characterImage; } }
    [SerializeField] private PlayerType characterType = PlayerType.Unknown;
    public PlayerType CharacterType { get { return characterType; } }
    public bool CanBeSelected
    {
        get 
        {
            return !TDS_GameManager.PlayersInfo.Any(p => p.PlayerType == characterType);
        }
    }

    #endregion

    #region Void 
    #endregion
}
