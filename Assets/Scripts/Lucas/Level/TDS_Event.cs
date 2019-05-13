using System;
using System.Collections;
using System.Linq;
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
    /// Transform where to instantiate the prefab.
    /// </summary>
    [SerializeField] private Transform prefabTransform = null;

    /// <summary>
    /// Unity event to invoke.
    /// </summary>
    [SerializeField] private UnityEvent unityEvent;

    /// <summary>
    /// Action to wait player to perform.
    /// </summary>
    [SerializeField] private WaitForPlayerAction actionType = WaitForPlayerAction.Dodge;
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
                string[] _quotes = TDS_GameManager.GetDialog(textID).Split('\n').Select(s => s.Trim()).Where(s => s != string.Empty).ToArray();

                foreach (string _quote in _quotes)
                {
                    int _time = _quote.Length / 20;
                    TDS_UIManager.Instance.ActivateNarratorBox(_quote);

                    // If not local, activate narrator in other players too
                    if (isOnline)
                    {
                        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "ActivateNarratorBox"), new object[] { _quote });
                    }

                    yield return new WaitForSeconds(_time);
                }

                TDS_UIManager.Instance.DesactivateNarratorBox();

                // If not local, desactivate narrator in other players too
                if (isOnline)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(TDS_UIManager.Instance.photonView, TDS_UIManager.Instance.GetType(), "DesactivateNarratorBox"), new object[] { });
                }

                break;

            // Display a message in an information box
            case CustomEventType.DisplayInfoBox:
                TDS_UIManager.Instance.ActivateDialogBox(TDS_GameManager.GetDialog(textID));
                break;

            // Desactivate the current information box
            case CustomEventType.DesactiveInfoBox:
                TDS_UIManager.Instance.DesactivateDialogBox();
                break;

            // Instantiate a prefab
            case CustomEventType.Instantiate:
                Object.Instantiate(prefab, prefabTransform.position, prefabTransform.rotation);

                // If not local, instantiate for other players too
                if (isOnline)
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, $"{TDS_LevelManager.Instance.phID}#{TDS_UIManager.Instance.GetType()}#Instantiate", new object[] { prefab, prefabTransform.position, prefabTransform.rotation });
                }
                break;

            // Wait during a certain time
            case CustomEventType.Wait:
                yield return new WaitForSeconds(waitTime);
                break;

            // Wait for an action of the local player
            case CustomEventType.WaitForAction:
                bool _isReady = false;

                switch (actionType)
                {
                    case WaitForPlayerAction.Jump:
                        TDS_LevelManager.Instance.LocalPlayer.OnStartJumpOneShot += () => _isReady = true;
                        break;

                    case WaitForPlayerAction.Dodge:
                        TDS_LevelManager.Instance.LocalPlayer.OnStartDodgeOneShot += () => _isReady = true;
                        break;

                    case WaitForPlayerAction.Grab:
                        TDS_LevelManager.Instance.LocalPlayer.OnGrabObjectOneShot += () => _isReady = true;
                        break;

                    case WaitForPlayerAction.Throw:
                        TDS_LevelManager.Instance.LocalPlayer.OnThrowOneShot += () => _isReady = true;
                        break;

                    default:
                        break;
                }

                while (!_isReady)
                {
                    yield return null;
                }
                break;

            // Wait until other players reach this event (Events System manage this)
            case CustomEventType.WaitOthers:
                while (true)
                {
                    yield return null;
                }
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
