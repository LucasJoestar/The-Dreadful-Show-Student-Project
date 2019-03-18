using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_BeardLady : TDS_Player 
{
    /* TDS_BeardLady :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[18 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_BeardLady class.
     *	
     *	    
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks
    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public override void Attack(bool _isLight)
    {
        base.Attack(_isLight);

        // Triggers the right actions
        switch (comboCurrent.Count)
        {
            case 1:
                if (_isLight)
                {
                    currentAttack = attacks[0];
                    //SetAnimLightAttack();
                }
                else
                {
                    currentAttack = attacks[1];
                    //SetAnimHeavyAttack();
                }
                break;

            case 2:
                if (_isLight)
                {
                    currentAttack = attacks[0];
                    //SetAnimLightAttack();
                }
                else
                {
                    currentAttack = attacks[1];
                    //SetAnimHeavyAttack();
                }
                break;

            case 3:
                if (_isLight)
                {
                    currentAttack = attacks[0];
                    //SetAnimLightAttack();
                }
                else
                {
                    currentAttack = attacks[1];
                    //SetAnimHeavyAttack();
                }
                break;

            default:
                Debug.Log($"There should not be more than 3 attacks for the Beard Lady, so here's a problem.");
                break;
        }
    }
    #endregion

    #region Animations

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
