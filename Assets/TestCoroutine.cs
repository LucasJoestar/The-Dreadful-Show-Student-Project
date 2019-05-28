using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutine : MonoBehaviour 
{
    /* TestCoroutine :
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

    #endregion

    #region Methods

    Coroutine coroutineStoped = null; 

	#region Original Methods
    IEnumerator Coroutine1()
    {
        yield return StartCoroutine(Coroutine2()); 
    }

    IEnumerator Coroutine2()
    {
        while (true)
        {
            Debug.Log("Wait"); 
            yield return new WaitForSeconds(.5f); 
        }
    }
	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    private void Awake()
    {

    }

	// Use this for initialization
    private void Start()
    {
        coroutineStoped = StartCoroutine(Coroutine1()); 
    }
	
	// Update is called once per frame
	private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StopCoroutine(coroutineStoped);
            Debug.Log("STOP"); 
        }
    }
	#endregion

	#endregion
}
