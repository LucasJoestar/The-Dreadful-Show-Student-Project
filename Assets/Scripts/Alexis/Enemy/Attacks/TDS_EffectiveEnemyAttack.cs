using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TDS_EffectiveEnemyAttack : TDS_EnemyAttack 
{
    /* TDS_MinionAttack :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	- Inherited class from TDS_EnemyAttack 
     *	- Contains complementary informations for the minions
     *      - AttackType to know what effect has to be applied when the attack is used
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[13/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the class]
	 *      - Creation of the enum attackType
	 *	-----------------------------------
	*/

    #region Fields / Properties
    [SerializeField] protected MinionAttackType attackType; 
    public MinionAttackType AttackType { get { return attackType; } }
    #endregion

}
