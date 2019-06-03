using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private void Destroy()
    {
        if (!PhotonNetwork.isMasterClient) return;
        PhotonNetwork.Destroy(gameObject);
    }

    /// <summary>
    /// Rotate the rabbit (Local and online)
    /// </summary>
    protected void Flip()
    {
        transform.Rotate(Vector3.up, 180);
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Flip"), new object[] { });
    }

    /// <summary>
    /// Increase the passing count 
    /// if the rabbit has made enough passages, destroy itself
    /// else run again
    /// </summary>
    private void IncreasePassingCount()
    {
        passingCountCurrent++;
        if (passingCountCurrent > passingCountMax && !isLooping)
        {
            if (!PhotonNetwork.isMasterClient)
            {
                TDS_RPCManager.Instance.RPCPhotonView.RPC("Call MethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Destroy"));
            }
            else
                Destroy(); 
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
        int _healingValue = UnityEngine.Random.Range(healingValueMin, healingValueMax);
        _player.Heal(_healingValue);

        OnUseRabbit?.Invoke();

        PhotonNetwork.Instantiate(particlesName, transform.position + Vector3.up, Quaternion.identity, 0);
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("Call MethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Destroy")); 
            return;
        }

        Destroy();
    }

    /// <summary>
    /// Get the next destination of the rabbit and set it on the navmeshagent
    /// Flip the rabbit and invert its goright boolean
    /// </summary>
    private void Run()
    {
        float _x = goRight ? boundRight : boundLeft; 
        Vector3 _targetPosition = new Vector3(_x, transform.position.y, transform.position.z);
        goRight = !goRight; 
        agent.SetDestination(_targetPosition);
        Flip(); 
    }

    #endregion

    #region Unity Methods
    private void Awake()
    {
    }

    // Use this for initialization
    private void Start()
    {
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
