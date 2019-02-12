/* TDS_EnumsHolder :
 *
 *	#####################
 *	###### PURPOSE ######
 *	#####################
 *
 *	References all enums used for the project "The Dreadful Show"
 *
 *	#####################
 *	### MODIFICATIONS ###
 *	#####################
 *
 *	Date :			[11 / 02 / 2019]
 *	Author :		[Guibert Lucas]
 *
 *	Changes :
 *
 *      - Added the AxisState enum.
 *
 *	-----------------------------------
 * 
 *	Date :			[21 / 01 / 2019]
 *	Author :		[Guibert Lucas]
 *
 *	Changes :
 *
 *  Initialization of the class.
 *	    - Added the PlayerType enum.
 *
 *	-----------------------------------
 *	
 *	Date :			[22 / 01 / 2019]
 *	Author :		[THIEBAUT Alexis]
 *
 *	Changes :
 *
 *  Adding more enums.
 *  	- Added the EnemyState enum.
 *  	
 *  -----------------------------------
 *	
 *	Date :			[04 / 02 / 2019]
 *	Author :		[THIEBAUT Alexis]
 *
 *	Changes :
 *
 *  Adding more enums.
 *  	- Added the EnemyAnimationState enum.
*/

/// <summary>
/// All possible states of the axis once converted into an input.
/// </summary>
public enum AxisState
{
    Key,
    KeyDown,
    KeyUp,
    None
}

public enum EnemyState
{
    Searching, 
    MakingDecision, 
    ComputingPath, 
    GettingInRange, 
    Attacking, 
    PickingUpObject, 
    ThrowingObject
}

public enum EnemyAnimationState
{
    Idle, 
    Run, 
    Hit, 
    Grounded, 
    GrabObject, 
    ThrowObject,
    AttackOne, 
    AttackTwo, 
    AttackThree, 
    SpecialAttack,
    Death
}

/// <summary>
/// All types of player available in the game.
/// </summary>
public enum PlayerType
{
    BeardLady,
    FatLady,
    FireEater,
    Juggler,
    Unknown
}
