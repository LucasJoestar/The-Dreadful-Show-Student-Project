﻿using Photon;
using System.Collections;
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
	 *	
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
    [SerializeField] private bool doDesTriggerOnActiv = true;

    /// <summary>
    /// Indicates if this object should be destroyed when finished.
    /// </summary>
    [SerializeField] protected bool doDesObjectOnFinish = false;

    /// <summary>
    /// Indicates if the event system should loop or not.
    /// </summary>
    [SerializeField] private bool doLoop = false;

    /// <summary>
    /// Is this event system activated and in process or not ?
    /// </summary>
    [SerializeField] protected bool isActivated = false;

    /// <summary>
    /// Is this event system local based or not ?
    /// </summary>
    [SerializeField] private bool isLocal = false;

    /// <summary>
    /// When entering this event, if activation is on traverse, indicates if it was from right or left.
    /// </summary>
    [SerializeField] private bool wasActivatedFromRight = false;

    /// <summary>
    /// BoxCollider of the object.
    /// </summary>
    [SerializeField] protected new BoxCollider collider = null;

    /// <summary>
    /// Current event of the system.
    /// </summary>
    protected Coroutine currentEventCoroutine = null;

    /// <summary>
    /// Current event processing.
    /// </summary>
    [SerializeField] protected TDS_Event currentEvent = null;

    /// <summary>
    /// All events to trigger in this events system.
    /// </summary>
    [SerializeField] protected TDS_Event[] events = new TDS_Event[] { };

    /// <summary>
    /// Tags detected used to activate this event system.
    /// </summary>
    [SerializeField] protected Tags detectedTags = new Tags(new Tag[] { new Tag("Player") });

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
        if ((isLocal || PhotonNetwork.isMasterClient) && !isActivated && other.gameObject.HasTag(detectedTags.ObjectTags)) StartEvents();
    }

    /// <summary>
    /// Main coroutine managing the whole system and triggering the events one after the other.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator EventsSystem()
    {
        isActivated = true;

        for (int _i = 0; _i < events.Length; _i++)
        {
            // Starts the event
            currentEventCoroutine = StartEventCoroutine(_i);

            if (!isLocal && ((int)events[_i].EventType > 20))
            {
                TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "StartEventCoroutine", new object[] { _i });
            }

            // Wait end of event to start next one
            yield return currentEventCoroutine;
        }

        // Loop if needed, or just stop the system
        if (doLoop) StartCoroutine(EventsSystem());
        else
        {
            isActivated = false;
            currentEventCoroutine = null;
            if (doDesObjectOnFinish) enabled = false;
        }
        
        yield break;
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
    public virtual void StartEvents()
    {
        if (isActivated) return;

        if (doDesTriggerOnActiv && collider)
        {
            collider.enabled = false;
        }

        StartCoroutine(EventsSystem());
    }

    /// <summary>
    /// Starts this event system !
    /// </summary>
    public void StopEvents()
    {
        if (!isActivated) return;

        StopAllCoroutines();
        if (currentEventCoroutine != null)
        {
            StopCoroutine(currentEventCoroutine);
            currentEventCoroutine = null;
        }

        isActivated = false;

        if (doDesObjectOnFinish) enabled = false;
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        if (!collider) collider = GetComponent<BoxCollider>();
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (activationMode == TriggerActivationMode.Enter) CheckTriggerValidation(other);
        else if (activationMode == TriggerActivationMode.Traverse) wasActivatedFromRight = other.bounds.center.x > collider.bounds.center.x;
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    protected virtual void OnTriggerExit(Collider other)
    {
        if ((activationMode == TriggerActivationMode.Exit) ||
           ((activationMode == TriggerActivationMode.Traverse) &&
           ((other.bounds.center.x > collider.bounds.center.x) != wasActivatedFromRight))) CheckTriggerValidation(other);
    }
    #endregion

    #endregion
}
