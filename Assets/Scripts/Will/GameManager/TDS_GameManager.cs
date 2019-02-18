using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_GameManager : MonoBehaviour 
{
    /* TDS_GameManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
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
    bool isPaused = false;
    bool isReadyToQuit = true;
    public static TDS_GameManager Instance;
    #endregion

    #region Methods
    #region Original Methods
    IEnumerator Quit()
    {
        yield return new WaitForEndOfFrame();
        isReadyToQuit = true;
        Application.Quit();
    }
    #endregion

    #region Unity Methods
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Already a GameManager in the Scene !");
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start ()
    {
		
    }
	
	void Update ()
    {
        
	}
	#endregion
	#endregion
}
