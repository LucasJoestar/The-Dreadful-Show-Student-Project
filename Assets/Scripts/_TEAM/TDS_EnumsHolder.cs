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
 *  Date :			[21 / 02 / 2019]
 *	Author :		[THIEBAUT Alexis]
 *
 *	Changes :
 *
 *  Adding more enums.
 *  	- Added the UIState enum.
 *  	
 *  -----------------------------------
 *  Date :			[13 / 02 / 2019]
 *	Author :		[THIEBAUT Alexis]
 *
 *	Changes :
 *
 *  Adding more enums.
 *  	- Added the MinionAttackType enum.
 *  	
 *  -----------------------------------
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
/// All possible attack effects of the game.
/// </summary>
public enum AttackEffectType
{
    None,
    Burn,
    PutOnTheGround,
    BringCloser
}

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

/// <summary>
/// All different states of the Beard Lady's beard.
/// </summary>
public enum BeardState
{
    Short,
    Normal,
    Long,
    VeryVeryLongDude
}

/// <summary>
/// All different states for the checkpoint animator.
/// </summary>
public enum CheckpointAnimState
{
    Desactivated,
    Resurrect,
    Activated
}

/// <summary>
/// Animation States of the enemies
/// </summary>
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
/// States of the enemies 
/// </summary>
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

/// <summary>
/// All types of events used in the game.
/// </summary>
public enum CustomEventType
{
    Narrator,
    DisplayInfoBox,
    DesactiveInfoBox,
    Instantiate,
    Wait,
    WaitForAction,
    WaitOthers,
    CameraMovement,
    UnityEvent
}

/// <summary>
/// All animation states specific to the Fire Eater.
/// </summary>
public enum FireEaterAnimState
{
    Sober,
    Drunk
}

/// <summary>
/// All animation states shared by all players.
/// </summary>
public enum PlayerAnimState
{
    Idle,
    Run,
    Hit,
    Die,
    Dodge,
    Throw,
    Catch,
    LightAttack,
    HeavyAttack,
    ComboBreaker,
    Super,
    Grounded,
    Jumping,
    Falling,
    HasObject,
    LostObject,
    Parrying,
    NotParrying
}

/// <summary>
/// All types of player available in the game.
/// </summary>
public enum PlayerType
{
    Unknown,
    BeardLady,
    FatLady,
    FireEater,
    Juggler,
}

/// <summary>
/// All states of the UI during the game
/// </summary>
public enum UIState
{
    InMenu, 
    InGame, 
    InPause
}

/// <summary>
/// Type of player action to wait for in event system.
/// </summary>
public enum WaitForPlayerAction
{
    Jump,
    Dodge,
    Grab,
    Throw
}
