using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine.UI;
using TMPro; 
using UnityEngine;

#pragma warning disable 0649

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

    #region Fields / Properties
    [Header("Character Selection Elements")]
    [SerializeField] private TDS_CharacterSelectionElement[] characterSelectionElements = new TDS_CharacterSelectionElement[] { };
    private TDS_CharacterSelectionElement localElement = null;
    public TDS_CharacterSelectionElement LocalElement
    {
        get { return localElement;  }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Add a new player within the CharacterSelectionElements
    /// If the added player is the local player, set the element as the local element
    /// </summary>
    /// <param name="_newPlayer">Id of the added player</param>
    public void AddNewPlayer(PhotonPlayer _newPlayer)
    {
        characterSelectionElements.Where(e => e.PhotonPlayer == null).First().SetPhotonPlayer(_newPlayer);
        if (_newPlayer.ID == PhotonNetwork.player.ID)
        {
            localElement = characterSelectionElements.Where(e => e.PhotonPlayer == _newPlayer).First();
            localElement.ActivateToggle(); 
        }
    }

    /// <summary>
    /// Remove the player from the character selection Elements
    /// If the player is'nt connected anymore, clear all the character selection elements
    /// </summary>
    /// <param name="_removedPlayer"></param>
    public void RemovePlayer(PhotonPlayer _removedPlayer)
    {
        if (PhotonNetwork.room == null || !PhotonNetwork.connected || PhotonNetwork.player == null)
        {
            ClearMenu(); 
            return; 
        }
        if (_removedPlayer == PhotonNetwork.player) return;
        TDS_CharacterSelectionElement _cleanedElement = characterSelectionElements.Where(e => e.PhotonPlayer == _removedPlayer).FirstOrDefault();
        if (_cleanedElement) _cleanedElement.DisconnectPlayer(); 
    }

    /// <summary>
    /// Clear all the character Selection elements 
    /// </summary>
    public void ClearMenu()
    {
        characterSelectionElements.ToList().ForEach(e => e.DisconnectPlayer()); 
    }

    /// <summary>
    /// Lock a particulary element 
    /// </summary>
    /// <param name="_playerID">Id of the player to lock</param>
    /// <param name="_playerIsLocked">Does the element has to be locked or unlocked</param>
    public void LockPlayer(int _playerID, bool _playerIsLocked)
    {
        // SET THE TOGGLE
        characterSelectionElements.Where(e => e.PhotonPlayer.ID == _playerID).First().LockElement(_playerIsLocked);
    }

    /// <summary>
    /// Make the elements selectable or not if a player select it
    /// If the local can't be selected, display the next selectable element
    /// </summary>
    /// <param name="_previousType">Previous Type of the player</param>
    /// <param name="_newType">new Type of the player</param>
    public void UpdateOnlineSelection(PlayerType _previousType, PlayerType _newType)
    {
        characterSelectionElements.ToList().ForEach(e => e.CharacterSelectionImages.Where(i => i.CharacterType == _previousType).ToList().ForEach(i => i.CanBeSelected = true));
        characterSelectionElements.ToList().ForEach(e => e.CharacterSelectionImages.Where(i => i.CharacterType == _newType).ToList().ForEach(i => i.CanBeSelected = false));
        if (!TDS_UIManager.Instance.LocalIsReady && !localElement.CurrentSelection.CanBeSelected) localElement.DisplayNextImage(); 
    }

    /// <summary>
    /// Update the selection element relative to a player. 
    /// Used online
    /// </summary>
    /// <param name="_player"></param>
    /// <param name="_newIndex"></param>
    public void UpdateMenuOnline(int _playerID, int _newIndex)
    {
        characterSelectionElements.Where(e => e.PhotonPlayer.ID == _playerID).FirstOrDefault().DisplayImageAtIndex(_newIndex); 
    }

    private void Update()
    {
        if (TDS_UIManager.Instance.UIState != UIState.InCharacterSelection) return;
    }
    #endregion

}
