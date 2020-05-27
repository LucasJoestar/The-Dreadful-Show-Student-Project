﻿using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(PhotonView))]
public class TDS_Checkpoint : PunBehaviour 
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

    #region Events
    /// <summary>
    /// Event called when a player pass a checkpoint, on that player machine only.
    /// </summary>
    public static event Action OnPassCheckpoint = null;
    #endregion

    #region Fields / Properties

    #region Animator
    private static readonly int activated_Hash = Animator.StringToHash("Activated");
    private static readonly int resurrect_Hash = Animator.StringToHash("Resurrect");
    #endregion

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
    /// List referencing all healed players.
    /// </summary>
    private List<int> healedPlayers = new List<int>();

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
        IsActivated = true;
        SetAnimState(CheckpointAnimState.Activated);

        // Resurrect dead players
        StartCoroutine(RespawnCoroutine());

        // Plays activation sound for everyone
        PlayActivationSound();
        TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "PlayActivationSound", new object[] { });
    }

    /// <summary>
    /// Call the event OnPassCheckpoint ; this method is called on a player machine when this one pass a checkpoint.
    /// </summary>
    private void CallOnPassCheckpoint() => OnPassCheckpoint?.Invoke();

    /// <summary>
    /// Plays checkpoit activation sound.
    /// </summary>
    private void PlayActivationSound()
    {
        // Play activation sound
    }

    /// <summary>
    /// Make all dead players respawn to this point.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnCoroutine()
    {
        TDS_Player[] _deadPlayers = TDS_LevelManager.Instance.AllPlayers.Where(p => p.IsDead).ToArray();

        if (_deadPlayers.Length == 0) yield break;

        yield return new WaitForSeconds(1);

        TDS_Player _player = null;

        for (int _i = 0; _i < _deadPlayers.Length; _i++)
        {
            _player = _deadPlayers[_i];

            // Make player disappear in smoke
            TDS_VFXManager.Instance.SpawnEffect(FXType.MagicAppear, new Vector3(_player.transform.position.x, _player.transform.position.y + .25f, _player.transform.position.z));

            Vector3 _newPos = transform.position + spawnPosition;
            _player.DisappearBeforeRespawn(_newPos.x, _newPos.y, _newPos.z);

            yield return new WaitForSeconds(.5f);

            // Trigger curtain animation
            SetAnimState(CheckpointAnimState.Resurrect);

            if (!_player.IsFacingRight) _player.Flip();

            _player.RespawnPlayer();

            yield return new WaitForSeconds(.5f);
        }
    }

    /// <summary>
    /// Set this checkpoint animation state
    /// </summary>
    /// <param name="_state"></param>
    public void SetAnimState(CheckpointAnimState _state)
    {
        TDS_RPCManager.Instance.CallRPC(PhotonTargets.All, photonView, GetType(), "SetAnimState", new object[] { (int)_state });
    }

    /// <summary>
    /// Set this checkpoint animation state
    /// </summary>
    /// <param name="_state"></param>
    public void SetAnimState(int _state)
    {
        CheckpointAnimState _animState = (CheckpointAnimState)_state;

        switch (_animState)
        {
            case CheckpointAnimState.Desactivated:
                animator.SetBool(activated_Hash, false);
                break;

            case CheckpointAnimState.Resurrect:
                animator.SetTrigger(resurrect_Hash);
                break;

            case CheckpointAnimState.Activated:
                animator.SetBool(activated_Hash, true);
                break;

            default:
                break;
        }
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
        }
        if (!trigger)
        {
            trigger = GetComponents<BoxCollider>().Where(b => b.isTrigger).FirstOrDefault();
        }
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        // Draw gizmos indicating if the point is activated or not
        Gizmos.color = isActivated ? Color.green : Color.red;
        Gizmos.DrawCube(transform.position + (Vector3.up * 1.5f), Vector3.one * .25f);

        // Draw spawn point gizmo
        Gizmos.DrawIcon(transform.position + spawnPosition, "Spawn");
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // On trigger enter, heal the player and activate the checkpoint
        if (other.gameObject.HasTag("Player") && !healedPlayers.Contains(other.GetInstanceID()))
        {
            healedPlayers.Add(other.GetInstanceID());

            TDS_Player _player = other.GetComponent<TDS_Player>();
            _player.Heal(999);

            // Call the OnPassCheckpoint event on the player machine
            TDS_RPCManager.Instance.CallRPC(_player.photonView.owner, photonView, GetType(), "CallOnPassCheckpoint", new object[] { });

            if (!isActivated) Activate();

            // If the checkpoint healed all players, just disable its box trigger
            if (healedPlayers.Count == TDS_LevelManager.Instance.AllPlayers.Length)
            {
                trigger.enabled = false;
            }
        }
    }
    #endregion

    #endregion
}
