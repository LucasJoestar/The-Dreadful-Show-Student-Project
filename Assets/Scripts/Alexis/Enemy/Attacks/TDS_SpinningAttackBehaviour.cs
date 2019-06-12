        using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spinning_Attack", menuName = "Attacks/Spinning Attack", order = 4), Serializable]
public class TDS_SpinningAttackBehaviour : TDS_EnemyAttack
{
    /* TDS_SpinningAttackBehaviour :
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

    private TDS_Enemy caster = null;

    private float casterInitialRadius = 0;

    [SerializeField, Range(1, 10)] private float spinningRadius = 3.6f; 
    #endregion

    #region Methods

    #region Original Methods

    public override void ApplyAttackBehaviour(TDS_Enemy _caster)
    {
        caster = _caster;
        caster.Agent.Speed = spinningSpeed;
        casterInitialRadius = caster.Agent.Radius;
        caster.Agent.Radius = spinningRadius;
        TDS_Bounds _bounds = TDS_Camera.Instance.CurrentBounds;
        spinningPositions = new Vector3[5] {new Vector3(_bounds.XMin, 0, _bounds.ZMax ),
                                            new Vector3(_bounds.XMin, 0, _bounds.ZMin ),
                                            new Vector3(_bounds.XMax, 0, _bounds.ZMax ),
                                            new Vector3(_bounds.XMax, 0, _bounds.ZMin ),
                                            new Vector3((_bounds.XMin + _bounds.XMax) / 2, 0, (_bounds.ZMin + _bounds.ZMax) / 2)};
        /*
        spinningPositions = new Vector3[5] {new Vector3(UnityEngine.Random.Range(_bounds.XMin, _bounds.XMax), 0, _bounds.ZMax ),
                                            new Vector3(UnityEngine.Random.Range(_bounds.XMin, _bounds.XMax), 0, _bounds.ZMin ),
                                            new Vector3(UnityEngine.Random.Range(_bounds.XMin, _bounds.XMax), 0, _bounds.ZMax ),
                                            new Vector3(UnityEngine.Random.Range(_bounds.XMin, _bounds.XMax), 0, _bounds.ZMin ),
                                            new Vector3((_bounds.XMin + _bounds.XMax) / 2, 0, (_bounds.ZMin + _bounds.ZMax) / 2)};
        */
        caster.Agent.OnDestinationReached += GoNextSpinningPosition;
        GoNextSpinningPosition();
    }


    /// <summary>
    /// Set the new destination as the next position in the spinningPositions array
    /// If there is no more positions in the array, reset all settings and call the Method StopSpinning
    /// </summary>
    private void GoNextSpinningPosition()
    {
        if (spinningIndex > 4)
        {
            caster.Agent.OnDestinationReached -= GoNextSpinningPosition;
            spinningIndex = 0;
            StopSpinning();
            caster.Agent.Speed = caster.SpeedInitial;
            caster.Agent.Radius = casterInitialRadius;
            return;
        }
        caster.Agent.SetDestination(spinningPositions[spinningIndex]);
        spinningIndex++;
    }


    /// <summary>
    /// Stop the spinning Animation
    /// -> Stop the attack in the animation
    /// </summary>
    private void StopSpinning()
    {
        caster.SetAnimationTrigger("StopSpinning"); 
    }
    #endregion

    #endregion
}
