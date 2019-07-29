using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Throwing_Attack", menuName = "Attacks/Throwing Attack", order = 3), Serializable]
public class TDS_ThrowingAttackBehaviour : TDS_EnemyAttack 
{
    /* TDS_ThrowingAttackBehaviour :
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
    [SerializeField] private string thrownObjectName = string.Empty;
    #endregion

    #region Methods

    #region Original Methods
    public override void ApplyAttackBehaviour(TDS_Enemy _caster)
    {
        if (thrownObjectName == string.Empty) return;
        GameObject _thrownObject = PhotonNetwork.Instantiate(thrownObjectName, _caster.HandsTransform.position, _caster.transform.rotation, 0);
        if (!_thrownObject) return; 
        if(_thrownObject.GetComponent<TDS_Throwable>())
        {
            TDS_Throwable _throwable = _thrownObject.GetComponent<TDS_Throwable>();
             _caster.GrabObject(_throwable);
             if(_throwable.ThrowableAttackType == AttackEffectType.BringCloser)
             {
                 _throwable.ObjectDurability = 1; 
                 _throwable.HitBox.OnTouch += () => _caster.SetAnimationState((int)EnemyAnimationState.BringTargetCloser);
                 _throwable.HitBox.OnStopAttack += () => _caster.SetAnimationState((int)EnemyAnimationState.EndBringingTargetCloser);
            }
            _caster.ThrowObject(_caster.PlayerTarget.transform.position);
            if (!_caster.IsFacingRight) _thrownObject.transform.Rotate(Vector3.up, 180);
        }
        else if(_thrownObject.GetComponent<TDS_Projectile>())
        {
            if (!_caster.IsFacingRight) _thrownObject.transform.Rotate(Vector3.up, 180);
            Vector3 _dir = _caster.IsFacingRight ? Vector3.right : Vector3.left;
            TDS_Projectile _proj = _thrownObject.GetComponent<TDS_Projectile>();
            _proj.HitBox.Activate(this, _caster);
            _proj.StartProjectileMovement(_dir, MaxRange); 
        }
    }
    #endregion

    #endregion
}
