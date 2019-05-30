using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;
using TMPro;
using Photon; 

public class TDS_MainMenu : PunBehaviour 
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

    #region Fields and properties 

    #region Singleton
    public static TDS_MainMenu Instance;
    #endregion

    #region MenuParents
    [Header("Menu Parents")]
    [SerializeField] private GameObject mainMenuParent;
    [SerializeField] private GameObject roomMenuParent;
    [SerializeField] private GameObject characterSelectionMenuParent;
    #endregion

    #region CharacterSelectionMenus
    [Header("Character Selection Menu")]
    [SerializeField] private TDS_CharacterMenuSelection characterSelectionMenu;
    [SerializeField] private TMP_Text[] playersName; 
    #endregion

    #region TextField
    [Header("Player Name")]
    [SerializeField] private TMP_Text playerNameField;
    public TMP_Text PlayerNameField
    {
        get { return playerNameField;  }
    }
    [Header("Player counter")]
    [SerializeField] private TMP_Text playerCountText; 
    #endregion

    #region Buttons
    [Header("Launch Game Button")]
    [SerializeField] private Button launchGameButton; 
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Load the next level 
    /// If Master client, send the information to the other players
    /// </summary>
    public void LoadLevel()
    {
        if(PhotonNetwork.isMasterClient)
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "LoadLevel"), new object[] { });
        //TDS_GameManager.LocalPlayer = localPlayer; 
        SceneManager.LoadScene(1); 
    }

    /// <summary>
    /// Quit the Game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit(); 
    }

    /// <summary>
    /// Set the menu state according to its state
    /// </summary>
    /// <param name="_state"></param>
    public void SetMenuState(UIState _state)
    {
        switch (_state)
        {
            case UIState.InMainMenu:
                if(mainMenuParent) mainMenuParent.SetActive(true);
                if(roomMenuParent) roomMenuParent.SetActive(false);
                if(characterSelectionMenuParent) characterSelectionMenuParent.SetActive(false);
                break;
            case UIState.InRoomSelection:
                if (mainMenuParent) mainMenuParent.SetActive(false);
                if(roomMenuParent)roomMenuParent.SetActive(true);
                if (characterSelectionMenuParent) characterSelectionMenuParent.SetActive(false);
                break;
            case UIState.InCharacterSelection:
                if (mainMenuParent) mainMenuParent.SetActive(false);
                if(roomMenuParent)roomMenuParent.SetActive(false);
                if (characterSelectionMenuParent) characterSelectionMenuParent.SetActive(true);
                break;
            case UIState.InGame:
                break;
            case UIState.InPause:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Show the main menu (used in UnityEvent)
    /// </summary>
    public void ShowMainMenu()
    {
        SetMenuState(UIState.InMainMenu);
    }

    /// <summary>
    /// Show the room selection (used in UnityEvent)
    /// </summary>
    public void ShowRoomSelection()
    {
        SetMenuState(UIState.InRoomSelection);
    }

    /// <summary>
    /// Show the character selection (used in UnityEvent)
    /// </summary>
    public void ShowCharacterSelection()
    {
        SetMenuState(UIState.InCharacterSelection);
    }

    /// <summary>
    /// Display the number of players in the room and their names
    /// If the player is the master client, also display the launch button
    /// </summary>
    /// <param name="_playerCount">Number of players</param>
    /// <param name="_displayLaunchButton">LaunchButton</param>
    public void UpdatePlayerCount(int _playerCount, bool _displayLaunchButton, PhotonPlayer[] _players)
    {
        if (playerCountText) playerCountText.text = $"Players : {_playerCount}/4";
        for (int i = 0; i < _playerCount; i++)
        {
            playersName[i].text = _players[i].NickName;
            playersName[i].gameObject.SetActive(true);
        }
        for (int i = _playerCount; i < playersName.Length; i++)
        {
            playersName[i].gameObject.SetActive(false);
        }

        if (launchGameButton) launchGameButton.gameObject.SetActive(_displayLaunchButton);

    }

    /// <summary>
    /// Update the player selection informations 
    /// Updated each time a player select a character
    /// </summary>
    /// <param name="_previousPlayerType"></param>
    /// <param name="_nextPlayerType"></param>
    public void UpdatePlayerSelectionInfo(int _previousPlayerType, int _nextPlayerType)
    {
        characterSelectionMenu.UpdateMenu((PlayerType)_previousPlayerType, (PlayerType)_nextPlayerType); 
    }

    /// <summary>
    /// Select a new character (used in UnityEvent)
    /// </summary>
    /// <param name="_newPlayerType">Index of the enum PlayerType</param>
    public void SelectCharacter(int _newPlayerType)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdatePlayerSelectionInfo"), new object[] { (int)TDS_GameManager.LocalPlayer, _newPlayerType });
        TDS_GameManager.LocalPlayer = (PlayerType)_newPlayerType == TDS_GameManager.LocalPlayer ? PlayerType.Unknown : (PlayerType)_newPlayerType;
    }

    /// <summary>
    /// Set the new name of the player (Used in Unity Event)
    /// </summary>
    public void SetNewName()
    { 
        if(playerNameField)
            TDS_NetworkManager.Instance.PlayerNamePrefKey = playerNameField.text; 
    }
    #endregion

    #region UnityMethods
    public void Awake()
    {
        if(Instance)
        {
            Destroy(this); 
            return; 
        }
        Instance = this; 
    }
    #endregion 

    #endregion
}
