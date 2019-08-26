using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_TraverseEventSystem : TDS_EventsSystem
{
    /* TDS_EventsSystem :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	#####################
     *	####### TO DO #######
     *	#####################
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *
     *	Date :			
     *	Author :		
     *
     *	Changes :
     *
     *	-----------------------------------
    */

    #region Fields / Properties
    /// <summary>
    /// Indicates if current events are entering ones or exiting ones.
    /// </summary>
    [SerializeField] private bool isInEnterEvents = true;

    /// <summary>
    /// Stored events system coroutine.
    /// </summary>
    [SerializeField] private Coroutine eventsSystemCoroutine = null;

    /// <summary>
    /// Events to trigger when exiting the collider.
    /// </summary>
    [SerializeField] private TDS_Event[] exitEvents = new TDS_Event[] { };

    /// <summary>
    /// All players currently in the collider.
    /// </summary>
    [SerializeField] private List<TDS_Player> playersIn = new List<TDS_Player>();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Main coroutine managing the whole system and triggering the events one after the other.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator EventsSystem()
    {
        isActivated = true;
        TDS_Event[] _events = isInEnterEvents ? events : exitEvents;

        for (int _i = 0; _i < _events.Length; _i++)
        {
            // Starts the event
            currentEventCoroutine = StartCoroutine(_events[_i].Trigger());

            // Wait end of event to start next one
            yield return currentEventCoroutine;
        }

        // Stop the system
        isActivated = false;
        currentEventCoroutine = null;
        eventsSystemCoroutine = null;

        if (!isInEnterEvents && doDesObjectOnFinish) enabled = false;
    }

    /// <summary>
    /// Removes a player from the stored ones in collider.
    /// </summary>
    /// <param name="_player">Player to remove.</param>
    private void RemovePlayer(TDS_Player _player)
    {
        if (_player == null) return;

        _player.OnPlayerDie -= RemovePlayer;
        playersIn.Remove(_player);

        if (playersIn.Count > 0) return;

        StartExitEvents();
    }

    /// <summary>
    /// Starts this event system with entering events.
    /// </summary>
    public override void StartEvents()
    {
        if (eventsSystemCoroutine != null) StopCoroutine(eventsSystemCoroutine);

        isActivated = true;
        isInEnterEvents = true;
        eventsSystemCoroutine = StartCoroutine(EventsSystem());
    }

    /// <summary>
    /// Starts this event system with exiting entering events.
    /// </summary>
    public void StartExitEvents()
    {
        if (eventsSystemCoroutine != null) StopCoroutine(eventsSystemCoroutine);

        isActivated = true;
        isInEnterEvents = false;
        eventsSystemCoroutine = StartCoroutine(EventsSystem());
    }
    #endregion

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.HasTag(detectedTags.ObjectTags)) return;

        TDS_Player _player = other.GetComponent<TDS_Player>();
        if (!_player) return;

        playersIn.Add(_player);
        _player.OnPlayerDie += RemovePlayer;

        if (playersIn.Count > 1) return;

        StartEvents();
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    protected override void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.HasTag(detectedTags.ObjectTags)) return;

        TDS_Player _player = other.GetComponent<TDS_Player>();
        if (!_player) return;

        playersIn.Remove(_player);
        _player.OnPlayerDie -= RemovePlayer;

        if (playersIn.Count > 0) return;

        StartExitEvents();
    }
    #endregion

    #endregion
}
