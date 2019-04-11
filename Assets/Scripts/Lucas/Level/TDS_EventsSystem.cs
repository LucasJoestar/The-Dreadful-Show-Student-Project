using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_EventsSystem : MonoBehaviour
{
    /* TDS_EventsSystem :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages multiple events to trigger one after the other
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	... Everything.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_EventsSystem class.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    /// <summary>
    /// Is this event system activated and in process or not ?
    /// </summary>
    [SerializeField] private bool isActivated = false;

    /// <summary>
    /// All events to trigger in this events system.
    /// </summary>
    [SerializeField] private TDS_Event[] events = new TDS_Event[] { };
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Starts this event system !
    /// </summary>
    public void StartEvents()
    {
        if (isActivated) return;

        isActivated = true;
    }

    /// <summary>
    /// Main coroutine managing the whole system and triggering the events one after the other.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EventsSystem()
    {
        for (int _i = 0; _i < events.Length; _i++)
        {
            yield return StartCoroutine(events[_i].Trigger());
        }

        isActivated = false;
        yield break;
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

    }

    // Update is called once per frame
    private void Update()
    {

    }
    #endregion

    #endregion
}
