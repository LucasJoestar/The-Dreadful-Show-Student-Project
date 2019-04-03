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

    #endregion

    #region Fields / Properties
    private bool goRight = true; 
    [SerializeField, Range(1, 10)] private int healingValueMax;
    [SerializeField, Range(1,10)] private int healingValueMin;
    private int passingCountCurrent = 0;
    [SerializeField] int passingCountMax = 5;
    private float boundLeft;
    private float boundRight; 
    [SerializeField, Range(1,10)] private float speed;
    private CustomNavMeshAgent agent; 
    #endregion

    #region Methods

    #region Original Methods
    protected override void Use(TDS_Player _player)
    {
        int _healingValue = UnityEngine.Random.Range(healingValueMin, healingValueMax);
        _player.Heal(_healingValue);
        PhotonView.Destroy(gameObject);
    }

    private void Run()
    {
        float _x = goRight ? boundRight : boundLeft; 
        Vector3 _targetPosition = new Vector3(_x, transform.position.y, transform.position.z);
        goRight = !goRight; 
        agent.SetDestination(_targetPosition); 
    }

    private void IncreasePassingCount()
    {
        Debug.Log("IN"); 
        passingCountCurrent++;
        if(passingCountCurrent > passingCountMax)
        {
            PhotonView.Destroy(gameObject); 
        }
        Run(); 
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
        boundLeft = TDS_Camera.Instance.CurrentBounds.XMin;
        boundRight = TDS_Camera.Instance.CurrentBounds.XMax;
        Run(); 
    }
    #endregion

    #endregion
}
