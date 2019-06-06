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
