using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro; 
using UnityEngine;

public class TDS_CharacterMenuSelection : MonoBehaviour 
{
    /* TDS_CharacterMenuSelection :
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

    #region Events

    #endregion

    #region Fields / Properties
    [SerializeField] private Button beardLadyButton;
    [SerializeField] private Button fatLadyButton;
    [SerializeField] private Button jugglerButton;
    [SerializeField] private Button fireEaterButton;
    [SerializeField] private Button resetButton;
    #endregion


    #region Methods
    public void UpdateMenu(PlayerType _previousType, PlayerType _newType)
    {
        if(_previousType == _newType)
        {
            switch (_previousType)
            {
                case PlayerType.Unknown:
                    break;
                case PlayerType.BeardLady:
                    beardLadyButton.interactable = true;
                    break;
                case PlayerType.FatLady:
                    fatLadyButton.interactable = true;
                    break;
                case PlayerType.FireEater:
                    fireEaterButton.interactable = true;
                    break;
                case PlayerType.Juggler:
                    jugglerButton.interactable = true;
                    break;
                default:
                    break;
            }
            return; 
        }
        switch (_previousType)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                beardLadyButton.interactable = true; 
                break;
            case PlayerType.FatLady:
                fatLadyButton.interactable = true; 
                break;
            case PlayerType.FireEater:
                fireEaterButton.interactable = true; 
                break;
            case PlayerType.Juggler:
                jugglerButton.interactable = true; 
                break;
            default:
                break;
        }

        switch (_newType)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                beardLadyButton.interactable = false;
                break;
            case PlayerType.FatLady:
                fatLadyButton.interactable = false;
                break;
            case PlayerType.FireEater:
                fireEaterButton.interactable = false;
                break;
            case PlayerType.Juggler:
                jugglerButton.interactable = false;
                break;
            default:
                break;
        }
    }
    #endregion
}
