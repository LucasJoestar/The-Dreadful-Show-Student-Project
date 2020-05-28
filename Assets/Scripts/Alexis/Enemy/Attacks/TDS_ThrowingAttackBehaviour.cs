﻿using System;
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
        if (thrownObjectName == string.Empty || !_caster.PlayerTarget) return;
        GameObject _thrownObject = PhotonNetwork.Instantiate(thrownObjectName, _caster.HandsTransform.position, _caster.transform.rotation, 0);
        //Debug.LogError("Stop!");
        if (!_thrownObject) return;

        TDS_Throwable _throwable = _thrownObject.GetComponent<TDS_Throwable>();
        if (_throwable)
        {
            _throwable.HitBox.HittableTags = new Tags(_caster.HitBox.HittableTags.ObjectTags); 
             _caster.GrabObject(_throwable);
            if (_throwable.ThrowableAttackEffectType == AttackEffectType.BringCloser)
             {
                _throwable.ObjectDurability = 1;
                //_throwable.HitBox.OnTouch += () => _caster.SetAnimationState((int)EnemyAnimationState.BringTargetCloser);
                _throwable.HitBox.OnStopAttack += _caster.NoTargetToBrought;
            }
            _caster.ThrowObject(_caster.PlayerTarget.transform.position);
            if (!_caster.IsFacingRight)
            {
                _thrownObject.transform.Rotate(Vector3.up, 180);

                _thrownObject.transform.localScale = new Vector3(_thrownObject.transform.localScale.x, _thrownObject.transform.localScale.y, _thrownObject.transform.localScale.z * -1);
            }
        }
        else if(_thrownObject.GetComponent<TDS_Projectile>())
        {
            Vector3 _dir = _caster.IsFacingRight ? Vector3.right : Vector3.left;
            TDS_Projectile _proj = _thrownObject.GetComponent<TDS_Projectile>();
            _proj.HitBox.HittableTags = new Tags(_caster.HitBox.HittableTags.ObjectTags); 
            _proj.HitBox.Activate(this, _caster);
            _proj.StartProjectileMovement(_dir, MaxRange);

            // Play sound
            PlaySound(_thrownObject);
        }
    }
    #endregion

    #endregion
}
