using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
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

    #region Fields / Properties
    public static TDS_SceneManager Instance;
    public static Dictionary<PhotonPlayer, bool> PlayerSceneLoaded = new Dictionary<PhotonPlayer, bool>();
    private bool sceneIsReady = false;
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
        sceneIsReady = false;
        if (PhotonNetwork.isMasterClient && PlayerSceneLoaded.Count > 0)
        {
            PlayerSceneLoaded = PlayerSceneLoaded.ToDictionary(p => p.Key, p => false);
        }
        StartCoroutine(LoadSceneOnline(_sceneIndex, _nextUIState));
    }

    /// <summary>
    /// Call this method to load a scene with the loading screen
    /// </summary>
    /// <param name="_sceneIndex"></param>
    public void PrepareOnlineSceneLoading(string _sceneName, int _nextUIState)
    {
        PrepareOnlineSceneLoading(SceneManager.GetSceneByName(_sceneName).buildIndex, _nextUIState);
    }

    /// <summary>
    /// Load the scene async and display the loading screen during the loading time
    /// </summary>
    /// <param name="_sceneIndex">Index of the scene in the build</param>
    /// <returns></returns>
    private IEnumerator LoadSceneOnline(int _sceneIndex, int _nextUIState)
    {
        AsyncOperation _async = SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Single);
        TDS_UIManager.Instance?.DisplayLoadingScreen(true);
        while (!_async.isDone)
        {
            yield return null;
        }
        TDS_GameManager.CurrentSceneIndex = _sceneIndex;
        if (PhotonNetwork.connected && PhotonNetwork.isMasterClient && PlayerSceneLoaded.Count > 0)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "PrepareOnlineSceneLoading"), new object[] { _sceneIndex, _nextUIState });
            while (PlayerSceneLoaded.Any(p => p.Value == false))
            {
                yield return null;
            }
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "OnEverySceneReady"), new object[] { });
        }
        else if (PhotonNetwork.connected && !PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "PlayerHasLoadScene"), new object[] { PhotonNetwork.player.ID });
            while (!sceneIsReady)
            {
                yield return null;
            }
        }
        //Call OnEveryOneSceneLoaded online
        OnEveryOneSceneLoaded();
        TDS_UIManager.Instance.ActivateMenu(_nextUIState);
    }

    /// <summary>
    /// Called when the scene is loaded
    /// </summary>
    /// <param name="_scene">Scene loaded</param>
    /// <param name="_mode">Loading mode of the scene</param>
    private void OnEveryOneSceneLoaded()
    {
        TDS_UIManager.Instance?.DisplayLoadingScreen(false);
        if (TDS_GameManager.LocalPlayer != PlayerType.Unknown && PhotonNetwork.connected)
        {
            TDS_LevelManager.Instance.Spawn(); 
        }
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
    public void PrepareSceneLoading(string _sceneName)
    {
        StartCoroutine(LoadScene(_sceneName)); 
    }

    /// <summary>
    /// Load a Scene locally
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <returns></returns>
    public IEnumerator LoadScene(string _sceneName)
    {
        AsyncOperation _async = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
        TDS_UIManager.Instance?.DisplayLoadingScreen(true);
        while (!_async.isDone)
        {
            yield return null;
        }
        TDS_GameManager.CurrentSceneIndex = SceneManager.GetSceneByName(_sceneName).buildIndex;
        TDS_UIManager.Instance?.DisplayLoadingScreen(false);
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
