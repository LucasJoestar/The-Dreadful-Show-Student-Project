using Photon;
using System;
using System.Collections;
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
    /// Event called when activating any checkpoint.
    /// </summary>
    public static event Action OnCheckpointActivated = null;

    /// <summary>
    /// Called when making a player respawn, with that player photon ID as parameter.
    /// </summary>
    public static event Action<int> OnRespawnPlayer = null;
    #endregion

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
        IsActivated = true;
        TDS_LevelManager.Instance.SetCheckpoint(this);

        trigger.enabled = false;

        SetAnimState(CheckpointAnimState.Activated);

        // Resurrect dead players
        Respawn();

        OnCheckpointActivated?.Invoke();
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

        TDS_Player _player = null;

        for (int _i = 0; _i < _deadPlayers.Length; _i++)
        {
            _player = _deadPlayers[_i];

            OnRespawnPlayer?.Invoke(_player.PhotonID);

            // Make player disappear in smoke
            TDS_VFXManager.Instance.SpawnEffect(FXType.MagicAppear, new Vector3(_player.transform.position.x, _player.transform.position.y + .25f, _player.transform.position.z));

            _player.gameObject.SetActive(false);
            _player.ActivePlayer(false);
            _player.HealthCurrent = _player.HealthMax;

            yield return new WaitForSeconds(1);

            // Trigger curtain animation
            SetAnimState(CheckpointAnimState.Resurrect);

            if (!_player.IsFacingRight) _player.Flip();
            _player.transform.position = transform.position + spawnPosition + (Vector3.right * 1);
            _player.gameObject.SetActive(true);
            _player.StartDodge();

            _player.OnStopDodgeOneShot += () => _player.ActivePlayer(true);

            yield return new WaitForSeconds(.5f);
        }
    }

    /// <summary>
    /// Set this checkpoint animation state
    /// </summary>
    /// <param name="_state"></param>
    public void SetAnimState(CheckpointAnimState _state)
    {
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, GetType(), "SetAnimState"), new object[] { (int)_state });
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
                animator.SetBool("Activated", false);
                break;

            case CheckpointAnimState.Resurrect:
                animator.SetTrigger("Resurrect");
                break;

            case CheckpointAnimState.Activated:
                animator.SetBool("Activated", true);
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
            if (!isActivated) Activate();
        }
    }
    #endregion

    #endregion
}
