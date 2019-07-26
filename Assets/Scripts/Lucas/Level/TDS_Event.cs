using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 0649
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
    /// Should the event system wait for all players performing an action or not.
    /// </summary>
    [SerializeField] private bool doWaitForAllPlayers = false;

    /// <summary>
    /// Public accessor for <see cref="doWaitForAllPlayers"/>.
    /// </summary>
    public bool DoWaitForAllPlayers { get { return doWaitForAllPlayers; } }

    /// <summary>
    /// Indicates if this event should wait the end of the previous one before starting.
    /// </summary>
    [SerializeField] private bool doWaitPreviousEvent = true;

    /// <summary>Public accessor for <see cref="doWaitPreviousEvent"/>.</summary>
    public bool DoWaitPreviousOne { get { return doWaitPreviousEvent; } }

    /// <summary>
    /// Indicates if the waiting action is complete or not.
    /// </summary>
    public bool IsActionComplete { get; private set; }

    /// <summary>
    /// Type of this event.
    /// </summary>
    [SerializeField] private CustomEventType eventType = CustomEventType.UnityEventLocal;

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
    /// ID of this event associated Event System photon view.
    /// </summary>
    public int EventSystemID = 0;

    /// <summary>
    /// Name of the prefab to instantiate.
    /// </summary>
    [SerializeField] private string prefabName = null;

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
    /// Stop to wait for the action.
    /// </summary>
    public void StopWaitingAction() => IsActionComplete = true;

    /// <summary>
    /// Triggers this event.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Trigger()
    {
        switch (eventType)
        {
            // Triggers a particular quote of the Narrator
            case CustomEventType.Narrator:

                // Activate narrator in other players too
                if (doNeedSpecificPlayerType && TDS_LevelManager.Instance.AllPlayers.Any(p => p.PlayerType == playerType))
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", TDS_LevelManager.Instance.AllPlayers.ToList().Where(p => p.PlayerType == playerType).Select(p => p.photonView.owner).First(), TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateNarratorBox")
                      , new object[] { TDS_GameManager.GetDialog(textID).Skip(1).ToArray() });
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateNarratorBox")
                      , new object[] { TDS_GameManager.GetDialog(textID).Skip(1).ToArray() });
                }
                break;

            // Display a message in an information box
            case CustomEventType.DisplayInfoBox:
                // Activate narrator in other players too
                if (doNeedSpecificPlayerType && TDS_LevelManager.Instance.AllPlayers.Any(p => p.PlayerType == playerType))
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", TDS_LevelManager.Instance.AllPlayers.ToList().Where(p => p.PlayerType == playerType).Select(p => p.photonView.owner).First(), TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateDialogBox")
                      , new object[] { TDS_GameManager.GetDialog(textID)[1] });
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateDialogBox")
                      , new object[] { TDS_GameManager.GetDialog(textID)[1] });
                }
                break;

            // Desactivate the current information box
            case CustomEventType.DesactiveInfoBox:
                if (doNeedSpecificPlayerType && TDS_LevelManager.Instance.AllPlayers.Any(p => p.PlayerType == playerType))
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", TDS_LevelManager.Instance.AllPlayers.ToList().Where(p => p.PlayerType == playerType).Select(p => p.photonView.owner).First(), TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "DesactivateDialogBox")
                      , new object[] {  });
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "DesactivateDialogBox")
                      , new object[] {  });
                }
                break;

            // Instantiate a prefab
            case CustomEventType.Instantiate:
                PhotonNetwork.Instantiate(prefabName, eventTransform.position, eventTransform.rotation, 0);
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
                        TDS_WhiteRabbit.OnLoseRabbit += StopWaitingAction;
                        break;

                    default:
                        break;
                }

                while (!IsActionComplete)
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
                        TDS_WhiteRabbit.OnLoseRabbit -= StopWaitingAction;
                        break;

                    default:
                        break;
                }

                IsActionComplete = false;

                TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(PhotonView.Find(EventSystemID), typeof(TDS_EventsSystem), "SetPlayerWaiting")
                      , new object[] { TDS_LevelManager.Instance.LocalPlayer.PhotonID });
                break;

            // Make a camera movement
            case CustomEventType.CameraMovement:
                yield return TDS_Camera.Instance.LookTarget(eventTransform.position.x, eventTransform.position.y, eventTransform.position.z, waitTime, cameraSpeedCoef);
                break;

            // Just invoke a Unity Event, that's it
            case CustomEventType.UnityEventLocal:
                unityEvent.Invoke();
                break;

            // Just invoke a Unity Event, that's it
            case CustomEventType.UnityEventForAll:
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
