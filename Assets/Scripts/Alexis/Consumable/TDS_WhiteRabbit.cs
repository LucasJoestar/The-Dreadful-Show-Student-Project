using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private bool isLooping = false;
    [SerializeField, Range(1, 10)] private int healingValueMax;
    [SerializeField, Range(1,10)] private int healingValueMin;
    private int passingCountCurrent = 0;
    [SerializeField] int passingCountMax = 5;
    private float boundLeft;
    private float boundRight; 
    [SerializeField, Range(1,10)] private float speed;
    private CustomNavMeshAgent agent;
    [SerializeField] private string particlesName;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Rotate the rabbit (Local and online)
    /// </summary>
    protected void Flip()
    {
        transform.Rotate(Vector3.up, 180);
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
        if (passingCountCurrent > passingCountMax && !isLooping)
        {
            PhotonNetwork.Destroy(gameObject);
            OnLoseRabbit?.Invoke();
            return; 
        }
        Run();
    }

    /// <summary>
    /// Called when a player catch the rabbit
    /// Heal the player and Destroy the rabbit
    /// </summary>
    /// <param name="_player"></param>
    public override void Use(TDS_Player _player)
    {
        if(!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "UseOnline"), new object[] { _player.PhotonID });
            return; 
        }
        int _healingValue = UnityEngine.Random.Range(healingValueMin, healingValueMax);
        _player.Heal(_healingValue);

        OnUseRabbit?.Invoke();
        TDS_VFXManager.Instance.SpawnEffect(FXType.RabbitPoof, transform.position + Vector3.up);
        PhotonNetwork.Destroy(gameObject);
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
        float _x = goRight ? boundRight : boundLeft; 
        Vector3 _targetPosition = new Vector3(_x, transform.position.y, transform.position.z);
        goRight = !goRight; 
        agent.SetDestination(_targetPosition);
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.All, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Flip"), new object[] { });
    }

    #endregion

    #region Unity Methods
    // Use this for initialization
    private void Start()
    {
        if (!PhotonNetwork.isMasterClient) return; 
        agent = GetComponent<CustomNavMeshAgent>();
        if (!agent) return;
        agent.Speed = speed;
        agent.OnDestinationReached += IncreasePassingCount;
        boundLeft = TDS_Camera.Instance.CurrentBounds.XMin - 1;
        boundRight =  TDS_Camera.Instance.CurrentBounds.XMax + 1;
        Run();
    }
    #endregion

    #endregion
}
