using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class TDS_SceneManager : MonoBehaviour 
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

    #endregion

    #region Fields / Properties
    public static TDS_SceneManager Instance; 
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Call this method to load a scene with the loading screen
    /// </summary>
    /// <param name="_sceneIndex"></param>
    public void PrepareSceneLoading(int _sceneIndex)
    {
        StartCoroutine(LoadScene(_sceneIndex));
    }

    /// <summary>
    /// Load the scene async and display the loading screen during the loading time
    /// </summary>
    /// <param name="_sceneIndex">Index of the scene in the build</param>
    /// <returns></returns>
    private IEnumerator LoadScene(int _sceneIndex)
    {
        AsyncOperation _async = SceneManager.LoadSceneAsync(_sceneIndex, LoadSceneMode.Single);
        TDS_UIManager.Instance?.DisplayLoadingScreen(true); 
        while (!_async.isDone)
        {
            yield return null; 
        }
        TDS_GameManager.CurrentSceneIndex = _sceneIndex ; 
        yield return new WaitForSeconds(1); 
        TDS_UIManager.Instance?.DisplayLoadingScreen(false);
    }

    /// <summary>
    /// Call this method to load a scene with the loading screen
    /// </summary>
    /// <param name="_sceneIndex"></param>
    public void PrepareSceneLoading(string _sceneName)
    {
        StartCoroutine(LoadScene(_sceneName));
    }

    /// <summary>
    /// Load the scene async and display the loading screen during the loading time
    /// </summary>
    /// <param name="_sceneIndex">Index of the scene in the build</param>
    /// <returns></returns>
    private IEnumerator LoadScene(string _sceneName)
    {
        AsyncOperation _async = SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Single);
        TDS_UIManager.Instance?.DisplayLoadingScreen(true);
        while (!_async.isDone)
        {
            yield return null;
        }
        TDS_GameManager.CurrentSceneIndex = SceneManager.GetSceneByName(_sceneName).buildIndex;
        yield return new WaitForSeconds(1);
        TDS_UIManager.Instance?.DisplayLoadingScreen(false);
    }

    /// <summary>
    /// Called when the scene is loaded
    /// </summary>
    /// <param name="_scene">Scene loaded</param>
    /// <param name="_mode">Loading mode of the scene</param>
    private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        if(TDS_GameManager.LocalPlayer != PlayerType.Unknown && PhotonNetwork.connected)
        {
            TDS_LevelManager.Instance.Spawn(); 
        }
    }
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
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    
	#endregion

	#endregion
}
