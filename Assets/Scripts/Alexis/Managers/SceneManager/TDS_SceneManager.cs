using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon; 

public class TDS_SceneManager : PunBehaviour 
{
    /* TDS_SceneManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Manage all scene loading]
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
	 *	Date :			[06/06/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialise the TDS_SceneManager class]
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Event called each time a scene is loaded, with its build index as parameter.
    /// </summary>
    public static event Action<int> OnLoadScene = null;
    #endregion

    #region Fields / Properties
    public static TDS_SceneManager Instance;
    public static Dictionary<PhotonPlayer, bool> PlayerSceneLoaded = new Dictionary<PhotonPlayer, bool>();
    private bool sceneIsReady = false;

    private readonly WaitForSeconds waitBeforeChangeLevel = new WaitForSeconds(1.25f);
    #endregion

    #region Methods

    #region Original Methods

    #region Online 
    /// <summary>
    /// Call this method to load a scene with the loading screen
    /// </summary>
    /// <param name="_sceneIndex"></param>
    public void PrepareOnlineSceneLoading(int _sceneIndex, int _nextUIState)
    {
        if (_sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            TDS_NetworkManager.Instance.LeaveGame();
            return;
        }

        // Save health
        foreach (TDS_Player _player in TDS_LevelManager.Instance.AllPlayers)
        {
            foreach (TDS_PlayerInfo _info in TDS_GameManager.PlayersInfo)
            {
                if (_player.PlayerType == _info.PlayerType)
                {
                    _info.Health = _player.HealthCurrent;
                    break;
                }
            }
        }

        sceneIsReady = false;
        if (PhotonNetwork.isMasterClient && PlayerSceneLoaded.Count > 0)
        {
            List<PhotonPlayer> _players = new List<PhotonPlayer>(PlayerSceneLoaded.Keys);
            foreach (PhotonPlayer _player in _players)
            {
                PlayerSceneLoaded[_player] = false;
            }
        }

        StartCoroutine(LoadSceneOnline(_sceneIndex, _nextUIState));
    }

    /// <summary>
    /// Load the scene async and display the loading screen during the loading time
    /// </summary>
    /// <param name="_sceneIndex">Index of the scene in the build</param>
    /// <returns></returns>
    private IEnumerator LoadSceneOnline(int _sceneIndex, int _nextUIState)
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "PrepareOnlineSceneLoading", new object[] { _sceneIndex, _nextUIState });
        }
        TDS_UIManager.Instance.DisplayLoadingScreen(true);
        yield return waitBeforeChangeLevel;
        yield return SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Single);

        OnLoadScene?.Invoke(_sceneIndex);
        TDS_GameManager.CurrentSceneIndex = _sceneIndex;
        if (PhotonNetwork.connected && PhotonNetwork.isMasterClient && PlayerSceneLoaded.Count > 0)
        {
            while (PlayerSceneLoaded.ContainsValue(false))
            {
                yield return null;
            }

            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "OnEverySceneReady", new object[] { });
        }
        else if (PhotonNetwork.connected && !PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, photonView, GetType(), "PlayerHasLoadScene", new object[] { PhotonNetwork.player.ID });

            while (!sceneIsReady)
            {
                yield return null;
            }
        }

        if (_sceneIndex > 0)
            TDS_LevelManager.Instance.Spawn();

        TDS_UIManager.Instance.DisplayLoadingScreen(false);
        TDS_UIManager.Instance.ActivateMenu(_nextUIState);
    }

    /// <summary>
    /// Called from the master, set every client's sceneIsReady to true
    /// </summary>
    private void OnEverySceneReady() => sceneIsReady = true; 

    /// <summary>
    /// Called on Master cclient when a player has loaded its scene 
    /// </summary>
    /// <param name="_playerID"></param>
    private void PlayerHasLoadScene(int _playerID)
    {
        PhotonPlayer _player = PhotonPlayer.Find(_playerID);
        if (PlayerSceneLoaded.ContainsKey(_player))
        {
            PlayerSceneLoaded[_player] = true; 
        }
    }
    #endregion

    #region Local
    /// <summary>
    /// Prepare to load a scene locally
    /// </summary>
    /// <param name="_sceneName"></param>
    public void PrepareSceneLoading(int _sceneIndex, int _nextUIState)
    {
        StartCoroutine(LoadScene(_sceneIndex, _nextUIState));
    }

    /// <summary>
    /// Load a Scene locally
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <returns></returns>
    public IEnumerator LoadScene(int _sceneIndex, int _nextUIState)
    {
        TDS_UIManager.Instance.DisplayLoadingScreen(true);
        if (_sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            TDS_NetworkManager.Instance.LeaveGame();
            yield break;
        }
        else if (_sceneIndex > 0)
        {
            // Save health
            foreach (TDS_Player _player in TDS_LevelManager.Instance.AllPlayers)
            {
                foreach (TDS_PlayerInfo _info in TDS_GameManager.PlayersInfo)
                {
                    if (_player.PlayerType == _info.PlayerType)
                    {
                        _info.Health = _player.HealthCurrent;
                        break;
                    }
                }
            }
        }

        yield return waitBeforeChangeLevel;
        yield return SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Single);

        OnLoadScene?.Invoke(_sceneIndex);
        TDS_GameManager.CurrentSceneIndex = _sceneIndex;
        

        if (_sceneIndex > 0)
        {
            TDS_LevelManager.Instance.Spawn();
            TDS_UIManager.Instance.IsloadingNextScene = false;
        }
        else
            TDS_UIManager.Instance.IsloadingNextScene = true;

        TDS_UIManager.Instance.DisplayLoadingScreen(false);
        TDS_UIManager.Instance.ActivateMenu(_nextUIState);

        // Reset score when loading main menu
        if (_sceneIndex == 0)
        {
            foreach (TDS_PlayerInfo _info in TDS_GameManager.PlayersInfo)
            {
                _info.PlayerScore.Reset();
            }
        }
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if(!Instance)
        {
            Instance = this; 
        }
        else
        {
            Destroy(this);
            return; 
        }

        TDS_GameManager.CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.isMasterClient && !PlayerSceneLoaded.ContainsKey(newPlayer))
        {
            PlayerSceneLoaded.Add(newPlayer, false);
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (PhotonNetwork.isMasterClient && PlayerSceneLoaded.ContainsKey(otherPlayer))
        {
            PlayerSceneLoaded.Remove(otherPlayer);
        }
    }
    #endregion

    #endregion
}
