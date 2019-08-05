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
    BringCloser,
    Knockback,
    Project
}

/// <summary>
/// All axis used by game controllers.
/// </summary>
public enum AxisType
{
    Horizontal,
    Vertical,
    HorizontalAim,
    VerticalAim
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
/// All buttons used by game controllers.
/// </summary>
public enum ButtonType
{
    Confirm,
    Cancel,
    Up,
    Down,
    Left,
    Right,
    Jump,
    LightAttack,
    HeavyAttack,
    Interact,
    Dodge,
    Parry,
    Pause,
    Aim,
    Shoot,
    Snack
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
/// All types of events used in the game.
/// </summary>
public enum CustomEventType
{
    // Local Events
    DisplayInfoBox,
    DesactiveInfoBox,
    InstantiatePhoton,
    WaitForAction,
    WaitForEveryone,
    UnityEventLocal,
    WaitForObjectDeath,
    
    // Online Events
    Narrator = 21,
    Instantiate,
    CameraMovement,
    UnityEventOnline,
    FreezePlayerForCutscene,
    UnfreezePlayerFromCutscene
}

/// <summary>
/// All available animation states for the destructibles.
/// </summary>
public enum DestructibleAnimState
{
    Hit,
    Destruction
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
    Death, 
    Taunt, 
    Brought, 
    BringTargetCloser, 
    EndBringingTargetCloser, 
    StopDashing, 
    LightHit
}

/// <summary>
/// States of the enemies 
/// </summary>
public enum EnemyState
{
    None,
    Searching, 
    MakingDecision, 
    ComputingPath, 
    GettingInRange, 
    Attacking, 
    PickingUpObject, 
    ThrowingObject, 
    Wandering, 
    Waiting
}

/// <summary>
/// All animations states related to the Fat Lady.
/// </summary>
public enum FatLadyAnimState
{
    Angry,
    Cool,
    Snack,
    PrepareAttack
}

/// <summary>
/// All animation states specific to the Fire Eater.
/// </summary>
public enum FireEaterAnimState
{
    Sober,
    Drunk,
    MiniGame,
    Spit,
    DoNotSpit,
    Fire,
    Puke
}

/// <summary>
/// Enum referencing all kinds of FX to instantiate.
/// </summary>
public enum FXType
{
    Kaboom,
    Poof,
    MagicAppear,
    MagicDisappear,
    RabbitPoof,
    BeardGrowsUp,
    BeardDamaged,
}

/// <summary>
/// All animation states used for the Juggler's aim target UI feedback.
/// </summary>
public enum JugglerAimTargetAnimState
{
    Disabled = -1,
    Neutral,
    UnderTarget,
    Locked
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
    NotParrying,
    Sliding,
    NotSliding,
    Down
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
/// All the Id for the differents rooms 
/// </summary>
public enum RoomId
{
    FirstRoom = 15,
    SecondRoom = 25,
    ThirdRoom = 35,
    FourthRoom = 45,
    FifthRoom = 55,
    WaitForIt = 99
}

/// <summary>
/// All triggers activation modes.
/// </summary>
public enum TriggerActivationMode
{
    Enter,
    Exit,
    Traverse,
    Other
}

/// <summary>
/// All states of the UI during the game
/// </summary>
public enum UIState
{
    InMainMenu, 
    InRoomSelection,
    InCharacterSelection,
    InGame, 
    InPause, 
    InGameOver
}

/// <summary>
/// Type of action to wait for in event system.
/// </summary>
public enum WaitForAction
{
    UseRabbit
}