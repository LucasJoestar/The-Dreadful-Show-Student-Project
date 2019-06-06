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
    [Header("Selection Buttons ")]
    [SerializeField] private Button beardLadyButton;
    public Button BeardLadyButton { get { return beardLadyButton; } }
    [SerializeField] private Button fatLadyButton;
    public Button FatLadyButton { get { return fatLadyButton; } }
    [SerializeField] private Button jugglerButton;
    public Button JugglerButton { get { return jugglerButton; } }
    [SerializeField] private Button fireEaterButton;
    public Button FireEaterButton { get { return fireEaterButton; } }
    [Space(5)]
    [Header("Selection Images ")]
    [SerializeField] private Image beardLadySelectionImage;
    [SerializeField] private Image fatLadySelectionImage;
    [SerializeField] private Image fireEaterSelectionImage;
    [SerializeField] private Image jugglerSelectionImage;

    #endregion


    #region Methods
    /// <summary>
    /// Make the buttons enabled or not if the online player has selected or deselected them 
    /// </summary>
    /// <param name="_previousType">Previous Type of the player</param>
    /// <param name="_newType">new Type of the player</param>
    public void UpdateMenuOnline(PlayerType _previousType, PlayerType _newType)
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

    /// <summary>
    /// Display a selection image on the locally selected player
    /// </summary>
    public void UpdateLocalSelection()
    {
        beardLadySelectionImage.gameObject.SetActive(false);
        fatLadySelectionImage.gameObject.SetActive(false);
        fireEaterSelectionImage.gameObject.SetActive(false);
        jugglerSelectionImage.gameObject.SetActive(false);

        switch (TDS_GameManager.LocalPlayer)
        {
            case PlayerType.Unknown:
                break;
            case PlayerType.BeardLady:
                beardLadySelectionImage.gameObject.SetActive(true);
                break;
            case PlayerType.FatLady:
                fatLadySelectionImage.gameObject.SetActive(true);
                break;
            case PlayerType.FireEater:
                fireEaterSelectionImage.gameObject.SetActive(true);
                break;
            case PlayerType.Juggler:
                jugglerSelectionImage.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
    #endregion
}
