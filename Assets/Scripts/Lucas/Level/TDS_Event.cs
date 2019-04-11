using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TDS_Event 
{
    /* TDS_Event :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Event object used in an event system, triggering something.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	Meh.
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
	 *	Creation of the TDS_Event class.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    /// <summary>
    /// Indicates if this event should wait the end of the previous one before starting.
    /// </summary>
    [SerializeField] private bool doWaitPreviousEvent = true;

    public bool DoWaitPreviousOne { get { return doWaitPreviousEvent; } }

    /// <summary>
    /// Indicates if this event is performed in local only. If not, a RPC will be executed.
    /// </summary>
    [SerializeField] private bool isLocal = true;

    /// <summary>
    /// Indicates if this event should only be called by the master.
    /// </summary>
    [SerializeField] private bool isMasterOnly = false;

    /// <summary>
    /// Boolean used to wait if a specific condition is required.
    /// </summary>
    private bool isReady = false;

    /// <summary>
    /// Type of this event.
    /// </summary>
    [SerializeField] private CustomEventType eventType = CustomEventType.UnityEvent;

    /// <summary>
    /// Time to wait.
    /// </summary>
    [SerializeField] private float waitTime = 0;

    /// <summary>
    /// ID of this event, shared by all players in the room.
    /// Used to identify this event.
    /// </summary>
    private int id = 0;

    /// <summary>
    /// Local player type required to execute this action.
    /// If unknown, then no specific player type is required.
    /// </summary>
    [SerializeField] private PlayerType playerType = PlayerType.Unknown;

    /// <summary>
    /// Text ID used to load a narrator quote or information box text.
    /// </summary>
    [SerializeField] private string textID = "ID";

    /// <summary>
    /// Unity event to invoke.
    /// </summary>
    [SerializeField] private UnityEvent unityEvent;
	#endregion

	#region Methods
    /// <summary>
    /// Triggers this event.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Trigger()
    {
        // If this event require a particular player type different than the local one, return
        if ((playerType != PlayerType.Unknown) && (TDS_LevelManager.Instance.LocalPlayer.PlayerType != playerType)) yield break;

        switch (eventType)
        {
            // Triggers a particular quote of the Narrator
            case CustomEventType.Narrator:
                break;

            // Display a message in an information box
            case CustomEventType.DisplayInfoBox:
                //TDS_UIManager.Instance.ActivateDialogBox();
                break;

            // Desactivate the current information box
            case CustomEventType.DesactiveInfoBox:
                TDS_UIManager.Instance.DesactivateDialogBox();
                break;

            case CustomEventType.Wait:
                yield return new WaitForSeconds(waitTime);
                break;

            case CustomEventType.WaitForAction:
                break;

            case CustomEventType.WaitOthers:
                break;

            // Just invoke a Unity Event, that's it
            case CustomEventType.UnityEvent:
                unityEvent.Invoke();
                break;

            // Nobody here but us chicken
            default:
                break;
        }
        
        yield break;
    }
	#endregion
}
