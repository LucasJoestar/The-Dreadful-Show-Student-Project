﻿using System;
using UnityEngine;

#pragma warning disable 0649

[RequireComponent(typeof(BoxCollider), typeof(CustomNavMeshAgent), typeof(PhotonView))]
public class TDS_WhiteRabbit : TDS_Consumable 
{
    /* TDS_WhiteRabbit :
 *
 *	#####################
 *	###### PURPOSE ######
 *	#####################
 *
 *	[PURPOSE]
 *
 *	#####################
 *	####### TO DO #######
 *	#####################
 *
 *	[TO DO]
 *
 *	#####################
 *	### MODIFICATIONS ###
 *	#####################
 *
 *	Date :			[DATE]
 *	Author :		[NAME]
 *
 *	Changes :
 *
 *	[CHANGES]
 *
 *	-----------------------------------
*/

    #region Events
    /// <summary>
    /// Event called when loosing a rabbit.
    /// </summary>
    public static event Action OnLoseRabbit = null;

    /// <summary>
    /// Event called when using a rabbit.
    /// </summary>
    public static event Action OnUseRabbit = null;
    #endregion

    #region Fields / Properties
    private bool goRight = true;
    private bool isDesactivated = false;
    [SerializeField] private bool isLooping = false;
    [SerializeField, Range(1, 200)] private int healingValueMax;
    [SerializeField, Range(1, 200)] private int healingValueMin;
    [SerializeField] private int passingCountCurrent = 0;
    [SerializeField] int passingCountMax = 5;
    [SerializeField, Range(1,10)] private float speed;
    [SerializeField] private CustomNavMeshAgent agent;
    [SerializeField] private new BoxCollider collider;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject shadow;
    [SerializeField] private ParticleSystem feedbackFX;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Rotate the rabbit (Local and online)
    /// </summary>
    protected void Flip()
    {
        transform.Rotate(Vector3.up, 180);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z * -1);
    }

    /// <summary>
    /// Increase the passing count 
    /// if the rabbit has made enough passages, destroy itself
    /// else run again
    /// </summary>
    private void IncreasePassingCount()
    {
        if (!PhotonNetwork.isMasterClient) return; 

        passingCountCurrent++;

        Debug.Log("Pass => " + passingCountCurrent);

        if (!isLooping && (passingCountCurrent > passingCountMax))
        {
            Debug.Log("Loose => " + passingCountCurrent);
            PhotonNetwork.Destroy(gameObject);
            OnLoseRabbit?.Invoke();
            return; 
        }

        goRight = !goRight;
        Run();
    }

    /// <summary>
    /// Called when a player catch the rabbit
    /// Heal the player and Destroy the rabbit
    /// </summary>
    /// <param name="_player"></param>
    public override void Use(TDS_Player _player)
    {
        if (isDesactivated) return;
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, photonView, GetType(), "UseOnline", new object[] { _player.PhotonID}); 
            return; 
        }
        int _healingValue = UnityEngine.Random.Range(healingValueMin, healingValueMax);
        _player.Heal(_healingValue);

        OnUseRabbit?.Invoke();

        TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "UseFeedback", new object[] { });

        UseFeedback();
        Invoke("Destroy", 2);

        Debug.Log("Use");
    }

    /// <summary>
    /// Plays feedback for when using the rabbit
    /// </summary>
    private void UseFeedback()
    {
        isDesactivated = true;

        collider.enabled = false;
        sprite.enabled = false;
        if (shadow) shadow.SetActive(false);

        // Play rabbit poof

        if (!feedbackFX) return;

        feedbackFX.gameObject.SetActive(true);
    }

    /// <summary>
    /// Called from a non-master client
    /// Call the method use in the master
    /// </summary>
    /// <param name="_playerId"></param>
    private void UseOnline(int _playerId)
    {
        if (!PhotonNetwork.isMasterClient) return; 
        TDS_Player _player = PhotonView.Find(_playerId).GetComponent<TDS_Player>();
        Use(_player); 
    }

    /// <summary>
    /// Get the next destination of the rabbit and set it on the navmeshagent
    /// Flip the rabbit and invert its goright boolean
    /// </summary>
    private void Run()
    {
        if (!PhotonNetwork.isMasterClient) return;

        float _x = goRight ? TDS_Camera.Instance.CurrentBounds.XMax + 1 : TDS_Camera.Instance.CurrentBounds.XMin - 1; 
        Vector3 _targetPosition = new Vector3(_x, transform.position.y, transform.position.z);
        agent.SetDestination(_targetPosition);
        TDS_RPCManager.Instance.CallRPC(PhotonTargets.All, photonView, GetType(), "Flip", new object[] { });
    }

    private void RecalculatePath()
    {
        if (!PhotonNetwork.isMasterClient) return;

        float _x = goRight ? TDS_Camera.Instance.CurrentBounds.XMax + 1 : TDS_Camera.Instance.CurrentBounds.XMin - 1;
        Vector3 _targetPosition = new Vector3(_x, transform.position.y, transform.position.z);

        agent.SetDestination(_targetPosition);
    }
    #endregion

    #region Unity Methods
    // Use this for initialization
    private void Start()
    {
        if (!PhotonNetwork.isMasterClient) return;
        if (!collider) collider = GetComponent<BoxCollider>();
        if (!sprite) sprite = GetComponent<SpriteRenderer>();
        if (!agent)
        {
            agent = GetComponent<CustomNavMeshAgent>();
            if (!agent) return;
        }
        agent.Speed = speed;
        agent.OnDestinationReached += IncreasePassingCount;
        TDS_Camera.Instance.OnBoundFreeze += RecalculatePath;

        Run();
    }

    private void OnDestroy()
    {
        Debug.Log("Destroy");
    }
    #endregion

    #endregion
}
