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

    #endregion

    #region Methods

    #region Original Methods

    private void StopSpinning()
    {
        if (PhotonNetwork.isMasterClient)
        {
            agent.OnDestinationReached -= StopSpinning;
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "StopSpinning"), new object[] { });
        }
        if (animator) animator.SetTrigger("StopSpinning");
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

    public override void CastThirdEffect()
    {
        // THIRD ATTACK MOVE WITHIN THE BOUNDS OF THE CAMERA
        agent.OnDestinationReached += StopSpinning;
        TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds; 
        Vector3[] _positions = new Vector3[4] {new Vector3(_bounds.XMin + 2, 0, _bounds.ZMin - 2),
                                               new Vector3(_bounds.XMin + 2, 0, _bounds.ZMax + 2),
                                               new Vector3(_bounds.XMax - 2, 0, _bounds.ZMin - 2),
                                               new Vector3(_bounds.XMax - 2, 0, _bounds.ZMax + 2)};
        List<Vector3> _path = _positions.ToList(); 
        //_path.Add(new Vector3((_bounds.XMin + _bounds.XMax) / 2, 0, (_bounds.ZMin + _bounds.ZMax) / 2));
        agent.CurrentPath.SetPath(_path);
        agent.StartAgent(); 
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
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update(); 
	}
	#endregion

	#endregion
}
