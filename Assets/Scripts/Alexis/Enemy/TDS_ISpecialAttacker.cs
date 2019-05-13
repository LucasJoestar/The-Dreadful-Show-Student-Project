using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TDS_ISpecialAttacker 
{
    /* TDS_ISpecialAttacker :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Interface for the enemies that apply effects on their attacks]
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
	 *	Date :			[13/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the interface]
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    TDS_EffectiveEnemyAttack[] Attacks { get;  set; }
    #endregion

    #region Methods

    #region Original Methods
    TDS_EffectiveEnemyAttack GetAttack(float _distance = 0);
    void ApplyAttackEffect(MinionAttackType _type);
    void CastFirstEffect();
    void CastSecondEffect();
    void CastThirdEffect();
    void CastSpecialEffect();
    #endregion

    #endregion
}
