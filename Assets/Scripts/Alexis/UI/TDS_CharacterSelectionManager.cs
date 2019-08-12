using System.Collections;
using System.Linq;
using UnityEngine;
using Photon;
using TMPro; 

public class TDS_CharacterSelectionManager : PunBehaviour
{
    /* TDS_CharacterSelectionManager :
    *
    *	#####################
    *	###### PURPOSE ######
    *	#####################
    *
    *	[Manager of the Character Selection In room]
    *
    *	#####################
    *	### MODIFICATIONS ###
    *	#####################
    *	
    *	Date :			[11/08/2019]
    *	Author :		[THIEBAUT Alexis]
    *
    *	Changes :
    *	
    *	
    */

    #region Fields and Properties
    [Header("Character Selection Menu")]
    [SerializeField] private TDS_CharacterMenuSelection characterSelectionMenu = null;
    public TDS_CharacterMenuSelection CharacterSelectionMenu { get { return characterSelectionMenu; } }
    #endregion


    #region Methods 

    #region Original Methods

    #region IEnumerator
    public IEnumerator PreapreLeavingRoom()
    {
        characterSelectionMenu.LocalElement.ClearToggle();
        while (characterSelectionMenu.LocalElement.ReadyToggle.animator.IsInTransition(0))
        {
            yield return null;
        }
        yield return new WaitForSeconds(.5f);

        characterSelectionMenu.ClearMenu();
        TDS_GameManager.LocalPlayer = PlayerType.Unknown;
        yield return null;

        TDS_NetworkManager.Instance?.LeaveRoom();
        TDS_UIManager.Instance?.ActivateMenu((int)UIState.InRoomSelection);
    }
    #endregion

    #region void 
    /// <summary>
    /// Action called when the player use the cancel button in the character selection menu 
    /// if the player is ready, unlock its character, else leave the room
    /// </summary>
    public void CancelInCharacterSelection()
    {
        if (TDS_UIManager.Instance.UIState != UIState.InCharacterSelection) return;
        if (TDS_GameManager.LocalIsReady)
        {
            SelectCharacter();
            characterSelectionMenu.LocalElement.TriggerToggle();
            return;
        }
        TDS_UIManager.Instance?.StartLeavingRoom();
    }

    /// <summary>
    /// Called when the toggle is pressed
    /// Update the ready settings
    /// If the player has an Unknown PlayerType, the game cannot start
    /// </summary>
    public void OnLocalPlayerReady(bool _isReady)
    {
        TDS_GameManager.LocalIsReady = _isReady;

        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetPlayerReady"), new object[] { PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady });
        if(!_isReady)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UnlockPlayerType"), new object[] { (int)characterSelectionMenu.LocalElement.CurrentSelection.CharacterType });
        }
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, typeof(TDS_UIManager), "UpdateReadySettings"), new object[] { PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady });
            return;
        }
        TDS_UIManager.Instance?.UpdateReadySettings(PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady);
    }

    /// <summary>
    /// Select a new character (used in UnityEvent)
    /// </summary>
    /// <param name="_newPlayerType">Index of the enum PlayerType</param>
    public void SelectCharacter()
    {
        if(TDS_GameManager.LocalIsReady)
        {
            OnLocalPlayerReady(false);
            return; 
        }
        int _newPlayerType = (int)characterSelectionMenu.LocalElement.CurrentSelection.CharacterType;
        TDS_GameManager.LocalPlayer = (PlayerType)_newPlayerType; 
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UpdatePlayerSelectionInfo"), new object[] {_newPlayerType, PhotonNetwork.player.ID });
        OnLocalPlayerReady(true); 
    }

    public void SetPlayerReady(int _playerID, bool _isReady)
    {
        characterSelectionMenu.LockPlayer(_playerID, _isReady);
    }

    /// <summary>
    /// Action called when the player use the submit button in the character selection menu 
    /// Lock the character when the player is not ready 
    /// When the player is master client and everyone is ready, launch the game
    /// </summary>
    public void SubmitInCharacterSelection()
    {
        if (TDS_UIManager.Instance.UIState != UIState.InCharacterSelection) return;
        if (!TDS_GameManager.LocalIsReady)
        {
            SelectCharacter();
            characterSelectionMenu.LocalElement.TriggerToggle();
            return;
        }
        if (PhotonNetwork.isMasterClient && TDS_UIManager.Instance.LaunchGameButton && !TDS_GameManager.PlayerListReady.Any(p => p.Value == false) && TDS_GameManager.LocalIsReady)
        {
            TDS_NetworkManager.Instance.LockRoom();
            TDS_UIManager.Instance?.LoadLevel();
        }
    }

    /// <summary>
    /// Unlock a player on the local client
    /// Called when a player leave the room
    /// </summary>
    /// <param name="_playerType">Type of the player to disconnect</param>
    public void UnlockPlayerType(PlayerType _playerType)
    {
        Debug.LogError("Unlock!");
        characterSelectionMenu.UnlockCharacterOnline(_playerType);
    }

    /// <summary>
    /// Unlock a player on the local client
    /// Called when a player leave the room
    /// </summary>
    /// <param name="_playerType">Type of the player to disconnect</param>
    public void UnlockPlayerType(int _playerType)
    {
        Debug.LogError("Unlock BIS!");
        characterSelectionMenu.UnlockCharacterOnline(_playerType);
    }

    /// <summary>
    /// Used Online, update the selection index on others players
    /// </summary>
    /// <param name="_player">id of the updated player</param>
    /// <param name="_newCharacterSelectionIndex">new Index</param>
    public void UpdateOnlineCharacterIndex(int _player, int _newCharacterSelectionIndex) => characterSelectionMenu.UpdateMenuOnline(_player, _newCharacterSelectionIndex);

    public void UpdateOnlineCharacterType(int _player, int _newCharacterSelectionType)
    {
        characterSelectionMenu.UpdateMenuOnline(_player, (PlayerType)_newCharacterSelectionType);
    }

    /// <summary>
    /// Update the player selection informations 
    /// Updated each time a player select a character
    /// </summary>
    /// <param name="_previousPlayerType"></param>
    /// <param name="_nextPlayerType"></param>
    public void UpdatePlayerSelectionInfo(int _nextPlayerType, int _playerID)
    {
        characterSelectionMenu.UpdateOnlineSelection((PlayerType)_nextPlayerType, _playerID);
    }

    public void ReceiveOnConnectionInfo(int _playerID, bool _isReady, int _playerType)
    {
        Debug.LogError(PhotonPlayer.Find(_playerID).NickName + ": Is ready = " + _isReady + " with character of type " + (PlayerType)_playerType);
        ///Update the displayed Image of the player
        UpdateOnlineCharacterType(_playerID, _playerType);

        if (_isReady)
        {       
            //Lock the player type if the player is ready
            SetPlayerReady(_playerID, _isReady);
            characterSelectionMenu.LocalElement.CharacterSelectionImages.Where(i => i.CharacterType == (PlayerType)_playerType).FirstOrDefault().CanBeSelected = false;
        }
    }

    private void SendInfoToNewPlayer(PhotonPlayer _newPlayer)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", _newPlayer, TDS_RPCManager.GetInfo(photonView, this.GetType(), "ReceiveOnConnectionInfo"), new object[] { PhotonNetwork.player.ID, TDS_GameManager.LocalIsReady, (int)characterSelectionMenu.LocalElement.CurrentSelection.CharacterType });
    }
    #endregion

    #endregion

    #region Unity Methods
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        characterSelectionMenu.RemovePlayer(otherPlayer);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.playerList.ToList().ForEach(p => characterSelectionMenu.AddNewPlayer(p));
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
        characterSelectionMenu.AddNewPlayer(newPlayer);
        SendInfoToNewPlayer(newPlayer);
    }
    #endregion

    #endregion


}
