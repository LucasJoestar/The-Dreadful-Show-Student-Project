using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

using Object = UnityEngine.Object;

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
	 *	    • Implement RPC methods if not local.
     *	    
     *	    • Wait for a specific action
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

    #region Fields / Properties
    /// <summary>
    /// Name of this event.
    /// </summary>
    public string Name = "New Event";

    /// <summary>
    /// Is this event triggered only if the local player is a specific character ?
    /// </summary>
    [SerializeField] private bool doNeedSpecificPlayerType = false;

    /// <summary>
    /// Indicates if this event should wait the end of the previous one before starting.
    /// </summary>
    [SerializeField] private bool doWaitPreviousEvent = true;

    /// <summary>Public accessor for <see cref="doWaitPreviousEvent"/>.</summary>
    public bool DoWaitPreviousOne { get { return doWaitPreviousEvent; } }

    /// <summary>
    /// Indicates if this event is performed in online or local only. If online, a RPC will be executed.
    /// </summary>
    [SerializeField] private bool isOnline = false;

    /// <summary>
    /// Indicates if this event should only be called by the master.
    /// </summary>
    [SerializeField] private bool isMasterOnly = false;

    /// <summary>
    /// Indicates if the waiting action is complete or not.
    /// </summary>
    private bool isActionComplete = false;

    /// <summary>
    /// Type of this event.
    /// </summary>
    [SerializeField] private CustomEventType eventType = CustomEventType.UnityEvent;

    /// <summary>Public accessor for <see cref="eventType"/>.</summary>
    public CustomEventType EventType { get { return eventType; } }

    /// <summary>
    /// Time to wait.
    /// </summary>
    [SerializeField] private float waitTime = 0;

    /// <summary>
    /// Speed coefficient applied to the camera movement.
    /// </summary>
    [SerializeField] private float cameraSpeedCoef = 1;

    /// <summary>
    /// Prefab to instantiate.
    /// </summary>
    [SerializeField] private GameObject prefab = null;

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
    /// Transform used for the event.
    /// </summary>
    [SerializeField] private Transform eventTransform = null;

    /// <summary>
    /// Unity event to invoke.
    /// </summary>
    [SerializeField] private UnityEvent unityEvent;

    /// <summary>
    /// Action to wait player to perform.
    /// </summary>
    [SerializeField] private WaitForAction actionType = WaitForAction.Dodge;
    #endregion

    #region Methods
    /// <summary>
    /// Triggers this event.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Trigger()
    {
        // If this event require a particular player type different than the local one, return
        if ((isMasterOnly && !PhotonNetwork.isMasterClient) || (doNeedSpecificPlayerType && (TDS_LevelManager.Instance.LocalPlayer.PlayerType != playerType))) yield break;

        switch (eventType)
        {
            // Triggers a particular quote of the Narrator
            case CustomEventType.Narrator:

                string[] _quotes = TDS_GameManager.GetDialog(textID);
                TDS_UIManager.Instance.ActivateNarratorBox(_quotes);

                // If not local, activate narrator in other players too
                if (isOnline)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateNarratorBox")
                        , new object[] { _quotes });
                }
                break;

            // Display a message in an information box
            case CustomEventType.DisplayInfoBox:
                TDS_UIManager.Instance.ActivateDialogBox(TDS_GameManager.GetDialog(textID)[0]);
                break;

            // Desactivate the current information box
            case CustomEventType.DesactiveInfoBox:
                TDS_UIManager.Instance.DesactivateDialogBox();
                break;

            // Instantiate a prefab
            case CustomEventType.Instantiate:
                Object.Instantiate(prefab, eventTransform.position, eventTransform.rotation);

                // TO CHANGE

                // If not local, instantiate for other players too
                if (isOnline)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(TDS_LevelManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "Instantiate"), new object[] { prefab, eventTransform.position, eventTransform.rotation });
                }
                break;

            // Wait during a certain time
            case CustomEventType.Wait:
                yield return new WaitForSeconds(waitTime);
                break;

            // Wait for an action of the local player
            case CustomEventType.WaitForAction:
                switch (actionType)
                {
                    case WaitForAction.Jump:
                        TDS_LevelManager.Instance.LocalPlayer.OnJump += StopWaitingAction;
                        break;

                    case WaitForAction.Dodge:
                        TDS_LevelManager.Instance.LocalPlayer.OnStartDodging += StopWaitingAction;
                        break;

                    case WaitForAction.Grab:
                        TDS_LevelManager.Instance.LocalPlayer.OnGrabObject += StopWaitingAction;
                        break;

                    case WaitForAction.Throw:
                        TDS_LevelManager.Instance.LocalPlayer.OnThrow += StopWaitingAction;
                        break;

                    case WaitForAction.UseRabbit:
                        TDS_WhiteRabbit.OnUseRabbit += StopWaitingAction;
                        break;

                    default:
                        break;
                }

                while (!isActionComplete)
                {
                    yield return null;
                }

                switch (actionType)
                {
                    case WaitForAction.Jump:
                        TDS_LevelManager.Instance.LocalPlayer.OnJump -= StopWaitingAction;
                        break;

                    case WaitForAction.Dodge:
                        TDS_LevelManager.Instance.LocalPlayer.OnStartDodging -= StopWaitingAction;
                        break;

                    case WaitForAction.Grab:
                        TDS_LevelManager.Instance.LocalPlayer.OnGrabObject -= StopWaitingAction;
                        break;

                    case WaitForAction.Throw:
                        TDS_LevelManager.Instance.LocalPlayer.OnThrow -= StopWaitingAction;
                        break;

                    case WaitForAction.UseRabbit:
                        TDS_WhiteRabbit.OnUseRabbit -= StopWaitingAction;
                        break;

                    default:
                        break;
                }

                isActionComplete = false;

                break;

            // Wait until other players reach this event (Events System manage this)
            case CustomEventType.WaitOthers:
                while (true)
                {
                    yield return null;
                }
                break;

            // Make a camera movement
            case CustomEventType.CameraMovement:
                yield return TDS_Camera.Instance.LookTarget(eventTransform, waitTime, cameraSpeedCoef);
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

    /// <summary>
    /// Stop to wait for the action.
    /// </summary>
    private void StopWaitingAction()
    {
        isActionComplete = true;
    }
	#endregion
}
