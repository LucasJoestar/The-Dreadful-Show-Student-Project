using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    #region Fields / Properties
    /// <summary>
    /// Indicates if this trigger should automatically starts when entering object trigger.
    /// </summary>
    [SerializeField] private bool doAutoTriggerOnEnter = true;

    /// <summary>
    /// Indicates if this object trigger should be desactivated when starting events.
    /// </summary>
    [SerializeField] private bool doDesactivateTriggerOnStart = true;

    /// <summary>
    /// Indicates if this object should be destroyed when finished.
    /// </summary>
    [SerializeField] private bool doDestroyOnFinish = false;

    /// <summary>
    /// Indicates if the event system should loop or not.
    /// </summary>
    [SerializeField] private bool doLoop = false;

    /// <summary>
    /// Is this event system activated and in process or not ?
    /// </summary>
    [SerializeField] private bool isActivated = false;

    /// <summary>
    /// Boolean used when waiting for other players.
    /// </summary>
    [SerializeField] private bool isWaitingForOthers = true;

    /// <summary>
    /// Current event of the system.
    /// </summary>
    private Coroutine currentEvent = null;

    /// <summary>
    /// All events to trigger in this events system.
    /// </summary>
    [SerializeField] private TDS_Event[] events = new TDS_Event[] { };

    /// <summary>
    /// This object photon view.
    /// </summary>
    [SerializeField] private PhotonView photonView = null;

    /// <summary>
    /// Players who are waiting at an event.
    /// </summary>
    private List<TDS_Player> waitingPlayers = new List<TDS_Player>();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Check if every players is waiting. If so, pass the event
    /// </summary>
    private void CheckPlayersWaiting()
    {
        if (!waitingPlayers.Union(TDS_LevelManager.Instance.AllPlayers).Any())
        {
            isWaitingForOthers = false;
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "PassEvent"), new object[] { });
        }
    }

    /// <summary>
    /// Starts this event system !
    /// </summary>
    public void StartEvents()
    {
        if (isActivated) return;

        isActivated = true;
        StartCoroutine(EventsSystem());
    }

    /// <summary>
    /// Starts this event system !
    /// </summary>
    public void StopEvents()
    {
        if (!isActivated) return;

        if (currentEvent != null) StopCoroutine(currentEvent);
        StopAllCoroutines();

        isActivated = false;

        if (doDestroyOnFinish) Destroy(this);
    }

    /// <summary>
    /// Main coroutine managing the whole system and triggering the events one after the other.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EventsSystem()
    {
        for (int _i = 0; _i < events.Length; _i++)
        {
            // If this event is to wait other players, do not start a coroutine
            if (events[_i].EventType == CustomEventType.WaitOthers)
            {
                if (PhotonNetwork.isMasterClient)
                {
                    waitingPlayers.Add(TDS_LevelManager.Instance.LocalPlayer);
                    CheckPlayersWaiting();
                }
                else
                {
                    TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, GetType(), "SetPlayerWaiting"), new object[] { TDS_LevelManager.Instance.LocalPlayer.PhotonID });
                }

                while (isWaitingForOthers)
                {
                    yield return null;
                }
                isWaitingForOthers = true;
            }
            else
            {
                // Starts the coroutine
                currentEvent = StartCoroutine(events[_i].Trigger());

                // If next one wait the previous, wait
                if ((_i < events.Length - 1) && events[_i + 1].DoWaitPreviousOne) yield return currentEvent;
            }
        }

        if (doLoop) StartCoroutine(EventsSystem());
        else
        {
            isActivated = false;
            if (doDestroyOnFinish) Destroy(this);
        }

        yield break;
    }

    /// <summary>
    /// Pass the event where waiting for others.
    /// </summary>
    public void PassWaitingEvent()
    {
        isWaitingForOthers = false;
    }

    /// <summary>
    /// Set a player as waiting.
    /// </summary>
    /// <param name="_playerID">ID of the player who is waiting.</param>
    public void SetPlayerWaiting(int _playerID)
    {
        waitingPlayers.Add(PhotonView.Find(_playerID).GetComponent<TDS_Player>());
        CheckPlayersWaiting();
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!photonView)
        {
            photonView = GetComponent<PhotonView>();
            if (!photonView) Debug.LogWarning($"Missing Photon View on the Events System \"{name}\"");
        }
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (doAutoTriggerOnEnter && !isActivated)
        {
            StartEvents();

            if (doDesactivateTriggerOnStart)
            {
                BoxCollider _trigger = GetComponent<BoxCollider>();
                if (_trigger) _trigger.enabled = false;
            }
        }
    }
    #endregion

    #endregion
}
