using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;

public class TDS_Siamese : TDS_Boss 
{
    /* TDS_Siamese :
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
    /// <summary>
    /// Movement speed of the Siamese during the third attack
    /// </summary>
    [SerializeField, Range(2, 10)] private float spinningSpeed = 8; 
    /// <summary>
    /// Index of the spinning position
    /// </summary>
    private int spinningIndex = 0; 
    /// <summary>
    /// Positions within the bounds of the camera
    /// </summary>
    private Vector3[] spinningPositions = null;

    [SerializeField] private string[] splitingEnemiesNames = new string[] { };
    [SerializeField] private Vector3[] splitingPosition = new Vector3[] { }; 
    #endregion

    #region Methods

    #region Original Methods

    /// <summary>
    /// Stop the spinning Animation
    /// -> Stop the attack in the animation
    /// </summary>
    private void StopSpinning()
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "StopSpinning"), new object[] { });
        }
        if (animator) animator.SetTrigger("StopSpinning");
    }

    /// <summary>
    /// Set the new destination as the next position in the spinningPositions array
    /// If there is no more positions in the array, reset all settings and call the Method StopSpinning
    /// </summary>
    private void GoNextSpinningPosition()
    {
        if(spinningIndex > 4)
        {
            agent.OnDestinationReached -= GoNextSpinningPosition;
            spinningIndex = 0;
            StopSpinning();
            agent.Speed = SpeedInitial; 
            return; 
        }
        agent.SetDestination(spinningPositions[spinningIndex]);
        spinningIndex++; 
    }

    /// <summary>
    /// Instantiate the splitting enemies at their splitting position
    /// </summary>
    private void SplitSiamese()
    {
        for (int i = 0; i < splitingEnemiesNames.Length; i++)
        {
            PhotonNetwork.Instantiate(splitingEnemiesNames[i], transform.position + splitingPosition[i], Quaternion.identity, 0); 
        }
        PhotonNetwork.Destroy(this.gameObject); 
    }
    #region Overriden Methods

    public override void CastFirstEffect()
    {
        // FIRST ATTACK HAS NO EFFECT
    }

    public override void CastSecondEffect()
    {
        // SECOND ATTACK HAS NO EFFECT
    }

    /// <summary>
    /// THIRD ATTACK MOVE WITHIN THE BOUNDS OF THE CAMERA
    /// Set the positions within the bounds and call the method do make the enemy move
    /// </summary>
    public override void CastThirdEffect()
    {
        if(spinningPositions == null)
        {
            TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds;
            spinningPositions = new Vector3[5] {new Vector3(_bounds.XMin + 2, 0, _bounds.ZMax + 2 ),
                                            new Vector3(_bounds.XMin + 2, 0, _bounds.ZMin - 2 ),
                                            new Vector3(_bounds.XMax - 2, 0, _bounds.ZMax + 2 ),
                                            new Vector3(_bounds.XMax - 2, 0, _bounds.ZMin - 2 ),
                                            new Vector3((_bounds.XMin + _bounds.XMax) / 2, 0, (_bounds.ZMin + _bounds.ZMax) / 2)};
        }
        agent.Speed = spinningSpeed; 
        agent.OnDestinationReached += GoNextSpinningPosition;
        GoNextSpinningPosition(); 
    }

    public override void CastSpecialEffect()
    {
        // SPECIAL ATTACK THROW A PROJECTILE THAT EXPLODES AFTER A CERTAIN AMOUNT OF TIME
        throwable = PhotonNetwork.Instantiate("ExplosiveGift", handsTransform.position, Quaternion.identity, 0).GetComponent<TDS_ExplosiveThrowable>();
        throwable.PickUp(this, handsTransform); 
        ThrowObject(playerTarget.transform.position); 

    }

    /// <summary>
    /// If the Attack is of the second Type, return true
    /// else check as <see cref="TDS_Boss.AttackCanBeCasted(float)"/>
    /// </summary>
    /// <param name="_distance">Distance between the enemy and the target</param>
    /// <returns></returns>
    protected override bool AttackCanBeCasted()
    {
        if (castedAttack.AttackType == EnemyEffectiveAttackType.TypeThree)
            return true; 
        return base.AttackCanBeCasted();
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake(); 
    }

	// Use this for initialization
    protected override void Start()
    {
        base.Start();
        TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds;
        spinningPositions = new Vector3[5] {new Vector3(_bounds.XMin + 2, 0, _bounds.ZMax + 2 ),
                                            new Vector3(_bounds.XMin + 2, 0, _bounds.ZMin - 2 ),
                                            new Vector3(_bounds.XMax - 2, 0, _bounds.ZMax + 2 ),
                                            new Vector3(_bounds.XMax - 2, 0, _bounds.ZMin - 2 ),
                                            new Vector3((_bounds.XMin + _bounds.XMax) / 2, 0, (_bounds.ZMin + _bounds.ZMax) / 2)};
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update(); 
	}

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < splitingPosition.Length; i++)
        {
            Gizmos.DrawLine(transform.position, transform.position + splitingPosition[i]); 
            Gizmos.DrawSphere(transform.position + splitingPosition[i], .1f); 

        }
    }
    #endregion

    #endregion
}
