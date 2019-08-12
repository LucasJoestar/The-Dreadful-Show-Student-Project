using System.Collections;
using System.Linq;
using UnityEngine;
using Photon; 

public class TDS_RoomSelectionManager : PunBehaviour
{
    /* TDS_RoomSelectionManager :
    *
    *	#####################
    *	###### PURPOSE ######
    *	#####################
    *
    *	[Manager of the Room selection menu]
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

    #region Fields and Porperties

    #region Room Selection Menu
    [Header("RoomSelectionMenu")]
    [SerializeField] private TDS_RoomSelectionElement[] roomSelectionElements = new TDS_RoomSelectionElement[] { };
    #endregion

    #endregion

    #region Methods 

    #region Original Methods

    #region IEnumerator
    /// <summary>
    /// Update the player count when the player is in the room selection Menu
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdatePlayerCount()
    {
        while (!PhotonNetwork.connected)
        {
            Debug.Log("Not Connected");
            yield return new WaitForSeconds(1);
        }
        RoomInfo[] _infos = new RoomInfo[] { };
        while (TDS_UIManager.Instance.UIState == UIState.InRoomSelection)
        {
            if (!PhotonNetwork.connected) yield break;
            _infos = PhotonNetwork.GetRoomList();
            roomSelectionElements.ToList().ForEach(e => e.PlayerCount = 0);
            for (int i = 0; i < _infos.Length; i++)
            {
                for (int j = 0; j < roomSelectionElements.Length; j++)
                {
                    if (roomSelectionElements[j].RoomName == _infos[i].Name)
                    {
                        roomSelectionElements[j].PlayerCount = _infos[i].PlayerCount;
                    }
                }
            }
            yield return new WaitForSeconds(2);
        }
    }
    #endregion

    #region void
    public void SetRoomsInterractable(bool _areInterractable)
    {
        roomSelectionElements.ToList().ForEach(e => e.RoomSelectionButton.interactable = _areInterractable);
    }
    #endregion

    #endregion

    #region Unity Methods
    public override void OnJoinedLobby()
    {
        SetRoomsInterractable(true);
    }

    public override void OnDisconnectedFromPhoton()
    {
        SetRoomsInterractable(false); 
    }
    #endregion

    #endregion
}
