using Photon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class TDS_EventsSystem : PunBehaviour
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
    /// Indicates if this object trigger should be desactivated when starting events.
    /// </summary>
    [SerializeField] private bool doDesactivateTriggerOnActivation = true;

    /// <summary>
    /// Indicates if this object should be destroyed when finished.
    /// </summary>
    [SerializeField] private bool doDesactivateOnFinish = false;

    /// <summary>
    /// Indicates if the event system should loop or not.
    /// </summary>
    [SerializeField] private bool doLoop = false;

    /// <summary>
    /// Is this event system activated and in process or not ?
    /// </summary>
    [SerializeField] private bool isActivated = false;

    /// <summary>
    /// Is this event system local based or not ?
    /// </summary>
    [SerializeField] private bool isLocal = false;

    /// <summary>
    /// Boolean used when waiting for other players.
    /// </summary>
    [SerializeField] private bool isWaitingForOthers = false;

    /// <summary>
    /// When entering this event, if activation is on traverse, indicates if it was from right or left.
    /// </summary>
    [SerializeField] private bool wasActivatedFromRight = false;

    /// <summary>
    /// BoxCollider of the objecT.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>
    /// Current event of the system.
    /// </summary>
    private Coroutine currentEventCoroutine = null;

    /// <summary>
    /// Current event processing.
    /// </summary>
    [SerializeField] private TDS_Event currentEvent = null;

    /// <summary>
    /// All events to trigger in this events system.
    /// </summary>
    [SerializeField] private TDS_Event[] events = new TDS_Event[] { };

    /// <summary>
    /// Players who are waiting at an event.
    /// </summary>
    private List<TDS_Player> waitingPlayers = new List<TDS_Player>();

    /// <summary>
    /// Tags detected used to activate this event system.
    /// </summary>
    [SerializeField] private Tags detectedTags = new Tags(new Tag[] { new Tag("Player") });

    /// <summary>
    /// Activation mode used for this event system.
    /// </summary>
    [SerializeField] private TriggerActivationMode activationMode = TriggerActivationMode.Enter;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Check a trigger activation validation, and start events if succeed.
    /// </summary>
    /// <param name="other"></param>
    private void CheckTriggerValidation(Collider other)
    {
        if (((isLocal && other.GetComponent<PhotonView>() && other.GetComponent<PhotonView>().isMine) || PhotonNetwork.isMasterClient) && !isActivated && other.gameObject.HasTag(detectedTags.ObjectTags)) StartEvents();
    }

    /// <summary>
    /// Main coroutine managing the whole system and triggering the events one after the other.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EventsSystem()
    {
        for (int _i = 0; _i < events.Length; _i++)
        {
            // Starts the coroutine
            if (events[_i].EventType == CustomEventType.WaitForAction)
            {
                isWaitingForOthers = true;
            }

            currentEventCoroutine = StartEventCoroutine(_i);

            if (currentEvent.EventType == CustomEventType.WaitOthers)
            {
                if (collider.enabled)
                {
                    collider.enabled = false;
                    yield return null;
                }
                collider.enabled = true;
            }

            if (((events[_i].EventType == CustomEventType.CameraMovement) && !isLocal) ||
                (events[_i].EventType == CustomEventType.WaitForAction) || (events[_i].EventType == CustomEventType.UnityEventForAll))
            {
                TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "StartEventCoroutine"), new object[] { _i });
            }

            // If next one wait the previous, wait
            if (((_i < events.Length - 1) && events[_i + 1].DoWaitPreviousOne) || (currentEvent.EventType == CustomEventType.WaitOthers)) yield return currentEventCoroutine;

            if (currentEvent.EventType == CustomEventType.WaitOthers) collider.enabled = !doDesactivateTriggerOnActivation;

            while (isWaitingForOthers) yield return null;
        }

        if (doLoop) StartCoroutine(EventsSystem());
        else
        {
            isActivated = false;
            if (doDesactivateOnFinish) enabled = false;
        }

        yield break;
    }

    /// <summary>
    /// Set a player as waiting.
    /// </summary>
    /// <param name="_playerID">ID of the player who is waiting.</param>
    public void SetPlayerWaiting(int _playerID)
    {
        if (!isWaitingForOthers) return;

        if (currentEvent.DoWaitForAllPlayers)
        {
            waitingPlayers.Add(PhotonView.Find(_playerID).GetComponent<TDS_Player>());

            if (!waitingPlayers.Union(TDS_LevelManager.Instance.AllPlayers).Any())
            {
                isWaitingForOthers = false;
                waitingPlayers = new List<TDS_Player>();
            }
        }
        else
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "StopWaitingForEvent"), new object[] { });

            isWaitingForOthers = false;
        }
    }

    /// <summary>
    /// Starts an event coroutine at a given index.
    /// </summary>
    /// <param name="_id">Index of the event to start.</param>
    /// <returns></returns>
    public Coroutine StartEventCoroutine(int _id)
    {
        currentEvent = events[_id];
        return StartCoroutine(events[_id].Trigger());
    }

    /// <summary>
    /// Starts this event system !
    /// </summary>
    public void StartEvents()
    {
        if (isActivated) return;

        if (doDesactivateTriggerOnActivation && collider)
        {
            collider.enabled = false;
        }

        isActivated = true;
        StartCoroutine(EventsSystem());
    }

    /// <summary>
    /// Starts this event system !
    /// </summary>
    public void StopEvents()
    {
        if (!isActivated) return;

        if (currentEventCoroutine != null) StopCoroutine(currentEventCoroutine);
        StopAllCoroutines();

        isActivated = false;

        if (doDesactivateOnFinish) Destroy(this);
    }

    /// <summary>
    /// Stop waiting for an event.
    /// </summary>
    public void StopWaitingForEvent()
    {
        currentEvent.StopWaitingAction();
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!collider)
        {
            collider = GetComponent<BoxCollider>();
            if (!collider)
            {
                Debug.LogWarning("BoxCollider on Event System \"" + name + "\" is missing !");
                return;
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        // Set photon view ID for each event
        foreach (TDS_Event _event in events)
        {
            _event.EventSystemID = photonView.viewID;
        }
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (isActivated && (currentEvent.EventType == CustomEventType.WaitOthers) && other.gameObject.HasTag("Player"))
        {
            waitingPlayers.Add(other.GetComponent<TDS_Player>());
            if (!TDS_LevelManager.Instance.AllPlayers.Except(waitingPlayers).Any())
            {
                waitingPlayers = new List<TDS_Player>();
                currentEvent.StopWaitingAction();
            }
        }

        if (activationMode == TriggerActivationMode.Enter) CheckTriggerValidation(other);
        else if (activationMode == TriggerActivationMode.Traverse) wasActivatedFromRight = other.bounds.center.x > collider.bounds.center.x;
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        if (isActivated && (currentEvent.EventType == CustomEventType.WaitOthers) && other.gameObject.HasTag("Player"))
        {
            waitingPlayers.Remove(other.GetComponent<TDS_Player>());
        }

        if ((activationMode == TriggerActivationMode.Exit) ||
           ((activationMode == TriggerActivationMode.Traverse) &&
           ((other.bounds.center.x > collider.bounds.center.x) != wasActivatedFromRight))) CheckTriggerValidation(other);
    }
    #endregion

    #endregion
}
