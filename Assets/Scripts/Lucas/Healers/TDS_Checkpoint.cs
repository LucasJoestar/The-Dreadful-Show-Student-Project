﻿using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_Checkpoint : MonoBehaviour 
{
    /* TDS_Checkpoint :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Checkpoint used to heal players when get activated, and serve as respawn point
     *	for dead players.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
     *	• Set animations.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[01 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Players now dodge when resurrected to get out of the Checkpoint box.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[26 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Added the spawn system.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[25 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the Checkpoint class.
     *	
     *	    Added the base system of the class, with activation and heal.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Animator of the Checkpoint object.
    /// </summary>
    [SerializeField] private Animator animator = null;

    /// <summary>
    /// Trigger of the checkpoint.
    /// </summary>
    [SerializeField] private BoxCollider trigger = null;

    /// <summary>Backing field for <see cref="IsActivated"/>.</summary>
    [SerializeField] private bool isActivated = false;

    /// <summary>
    /// Indicates if the checkpoint has already been activated or not.
    /// </summary>
    public bool IsActivated
    {
        get { return isActivated; }
        private set
        {
            isActivated = value;
        }
    }

    /// <summary>
    /// Position where to spawn on this checkpoint (local space).
    /// </summary>
    [SerializeField] private Vector3 spawnPosition = Vector3.zero;
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Activate this checkpoint.
    /// </summary>
    private void Activate()
    {
        isActivated = true;
        TDS_LevelManager.Instance.SetCheckpoint(this);

        trigger.enabled = false;

        // Resurrect dead players
        Respawn();
    }

    /// <summary>
    /// Starts the coroutine to make dead players respawn.
    /// </summary>
    public void Respawn() => StartCoroutine(RespawnCoroutine());

    /// <summary>
    /// Make all dead players respawn to this point.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnCoroutine()
    {
        TDS_Player[] _deadPlayers = TDS_LevelManager.Instance.AllPlayers.Where(p => p.IsDead).ToArray();

        if (_deadPlayers.Length == 0) yield break;

        yield return new WaitForSeconds(3);

        // Trigger curtain animation

        TDS_Player _player = null;

        for (int _i = 0; _i < _deadPlayers.Length; _i++)
        {
            _player = _deadPlayers[_i];

            // Make player disappear in smoke

            _player.gameObject.SetActive(false);
            _player.ActivePlayer(false);
            _player.HealthCurrent = _player.HealthMax;

            yield return new WaitForSeconds(1);

            if (!_player.IsFacingRight) _player.Flip();
            _player.transform.position = transform.position + spawnPosition;
            _player.gameObject.SetActive(true);
            _player.StartDodge();

            _player.OnStopDodgeOneShot += () => _player.ActivePlayer(true);

            yield return new WaitForSeconds(.5f);
        }

        // Trigger curtain animation
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Try to get missing components
        if (!animator)
        {
            animator = GetComponent<Animator>();
            if (!animator) Debug.LogWarning("The Animator of \"" + name + "\" for script TDS_Checkpoint is missing !");
        }
        if (!trigger)
        {
            trigger = GetComponents<BoxCollider>().Where(b => b.isTrigger).FirstOrDefault();
            if (!trigger) Debug.LogWarning("The Trigger of \"" + name + "\" for script TDS_Checkpoint is missing !");
        }
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        // Draw gizmos indicating if the point is activated or not
        Gizmos.color = isActivated ? (TDS_LevelManager.Instance.Checkpoint == this ? Color.green : Color.blue) : Color.red;
        Gizmos.DrawCube(transform.position + (Vector3.up * 1.5f), Vector3.one * .25f);

        // Draw spawn point gizmo
        Gizmos.DrawIcon(transform.position + spawnPosition, "Spawn");
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // On trigger enter, heal the player and activate the checkpoint
        if (other.gameObject.HasTag("Player"))
        {
            other.GetComponent<TDS_Player>().Heal(999);
            Activate();
        }
    }
    #endregion

    #endregion
}
