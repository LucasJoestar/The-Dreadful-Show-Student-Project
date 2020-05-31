using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public class TDS_Player : TDS_Character, IPunObservable
{
    /* TDS_Player :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class to inherit from for all players types.
     *	    
     *	    Contains everything needed to create a new player by implementing and overriding methods.
     *	    
     *	    The TDS_BeardLady, TDS_FatLady, TDS_FireEater & TDS_Juggler classes inherit from this.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     *	
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     *	
     *	Date :			[21 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    - Removed aim system from Player class, and set it only in the Juggler one.
	 *
	 *	-----------------------------------
     *	
     *  Date :			[28 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Players now hold the throw button to aim, and throw with another button.
     *	That's better, for sure.
     * 
     *  -----------------------------------
     *	
     *  Date :			[27 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Made the player constantly move the same way when dodging.
     * 
     *  -----------------------------------
     *	
     *  Date :			[21 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the DPadXAxis, DPadYAxis, RightStickXAxis, RightStickYAxis & handsTransformMemoryLocalPosition fields ; and the HandTransformLocalPosition property.
     *	    - Added the AimFlip method.
     * 
     *  -----------------------------------
     * 
     *  Date :			[19 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the whatIsAllButThis field.
     * 
     *  -----------------------------------
     * 
     *  Date :			[12 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the ProjectilePreviewEndZone & ProjectilePreviewArrow fields.
     * 
     *  -----------------------------------
     * 
     *  Date :			[07 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the OnGetOnGround & OnGetOffGround events.
     *	    - Added the previousColliderPosition field ; and the isDodging & isParrying fields & properties.
     *	    - Added the StartAttack, SetAnimCatchSetAnimDie, SetAnimDodge, SetAnimHit, SetAnimThrow, SetAnimGroundState, SetAnimHasObject, SetAnimIsParrying, SetAnimIsMoving, SetCurrentAttack, ActiveAttack, EndAttack & CheckGrounded methods.
     * 
     *  -----------------------------------
     * 
     *  Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Moved the throwAimingPoint field ; and the aimAngle field & property to the TDS_Character class.
     *	    - Added the lineRenderer, ParryButton, throwVelocity & throwTrajectoryMotionPoints fields ; the ThrowAimingPoint property ; and the throwPreviewPosition field & property.
     *	    - Added the DrawPreviewTrajectory & Parry methods.
     * 
     *  -----------------------------------
     * 
     *  Date :			[04 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the cancelThrowButton, aimCoroutine  & throwAimingPoint fields ; and the isAiming & aimAngle fields & properties.
     *	    - Added the Aim, CancelThrow, PrepareThrow & UseObject methods.
     * 
     *  -----------------------------------
     * 
     *  Date :			[29 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the isGrounded, isJumping & playerType fields ; and the JumpMaximumTime property.
     * 
     *  -----------------------------------
     * 
     *  Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the attacks field & property.
     * 
     *  -----------------------------------
     * 
     *  Date :			[22 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the previousPosition field.
     *	    - Fulfilled the AdjustPositionOnRigidbody method.
     * 
     *  -----------------------------------
     * 
     *  Date :			[21 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Fulfilled the Move & Jump methods.
     *	    - Added the groundDetection & WhatIsObstacle fields.
     *	    - Added the AdjustPositionOnRigidbody method.
     *	    - Removed the groundDetector field.
     * 
     *  -----------------------------------
     * 
     *  Date :			[17 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    Made the first version of the controller.
     *	
     *	    - Added the CatchButton, DodgeButton, HeavyAttackButton, HorizontalAxis, InteractButton, JumpButton, LightAttackButton, SuperAttackButton, ThrowButton, VerticalAxis, IsJumping, JumpForce, JumpMaximumTime, jumpCoroutine, interactionDetector & groundDetector fields.
     *	    - Added the CheckActionsInputs & CheckMovementsInputs methods ; Jump coroutine ; Move methods with one overload.
     *	    - Removed the CheckInputs method.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[16 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	Creation of the TDS_Player class.
     *	
     *	    - Added the summoner field ; IsGrounded property ; comboCurrent, comboMax & comboResetTime fields & properties.
     *	    - Added the Attack, Catch, CheckInputs, Dodge, Interact, ResetCombo, StopAttack, StopDodge & SuperAttack method to fulfill later.
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Event called when the player gets back on the ground.
    /// </summary>
    public event Action OnGetOnGround = null;

    /// <summary>
    /// Event called when the player gets off the ground.
    /// </summary>
    public event Action OnGetOffGround = null;

    /// <summary>
    /// Event called when starting an attack.
    /// </summary>
    public event Action OnStartAttack = null;

    /// <summary>
    /// Event called when starting a dodge.
    /// </summary>
    public event Action OnStartDodging = null;

    /// <summary>
    /// Event called when starting parrying.
    /// </summary>
    public event Action OnStartParry = null;

    /// <summary>
    /// Event called when stopping an attack.
    /// </summary>
    public event Action OnStopAttack = null;

    /// <summary>
    /// Event called when stopping a dodge.
    /// </summary>
    public event Action OnStopDodge = null;

    /// <summary>
    /// Event called when stopping parrying.
    /// </summary>
    public event Action OnStopParry = null;


    /// <summary>
    /// Event called when stopping a dodge. It is cleaned once called.
    /// </summary>
    public event Action OnStopDodgeOneShot = null;


    /// <summary>
    /// Event called when starting to jump.
    /// </summary>
    public event Action OnJump = null;


    /// <summary>
    /// Event called when the player grab an object or loose it.
    /// </summary>
    public event Action<bool> OnHasObject = null;

    /// <summary>
    /// Event called when the player press the button indicating how to play.
    /// </summary>
    public event Action OnTriggerHowToPlay = null;

    /// <summary>
    /// Event called when this player dies, with this script as parameter.
    /// </summary>
    public event Action<TDS_Player> OnPlayerDie = null;
    #endregion

    #region Fields / Properties

    #region Constants
    /// <summary>
    /// Minimum time for a dodge before the player can cancel it and attack.
    /// </summary>
    public const float DODGE_MINIMUM_TIMER = .25f;

    /// <summary>
    /// Time to wait to drop object instead of throwing it.
    /// </summary>
    public const float DROP_OBJECT_TIME = .35f;

    /// <summary>
    /// Time during which the player is invulnerable after being hit.
    /// </summary>
    public const float INVULNERABILITY_TIME = .5f;

    /// <summary>
    /// Minimum movement value of the character when moving on an axis.
    /// </summary>
    public const float MOVEMENT_MINIMUM_VALUE = .5f;
    #endregion

    #region Components & References
    /// <summary>
    /// Controller linked to this player.
    /// </summary>
    [SerializeField] protected TDS_Controller controller = new TDS_Controller();

    /// <summary>Public accessor for <see cref="Controller"/>.</summary>
    public TDS_Controller Controller { get { return controller; } }

    /// <summary>
    /// The summoner this player is currently carrying.
    /// </summary>
    public TDS_Summoner Summoner = null;

    /// <summary>
    /// <see cref="TDS_PlayerInteractionBox"/> used to detect when possible interactions with the environment are availables.
    /// </summary>
    [SerializeField] protected TDS_PlayerInteractionBox interactionBox = null;

    /// <summary>Public accessor for <see cref="interactionBox"/>.</summary>
    public TDS_PlayerInteractionBox InteractionBox { get { return interactionBox; } }

    /// <summary>
    /// Virtual box used to detect if the player is grounded or not.
    /// </summary>
    [SerializeField] protected TDS_VirtualBox groundDetectionBox = new TDS_VirtualBox();

    /// <summary>
    /// Sprite holder, used to display informations relatives to the player sprite.
    /// </summary>
    [SerializeField] protected TDS_PlayerSpriteHolder spriteHolder;

    /// <summary>
    /// PhotonView of the transform used to spawn fx.
    /// </summary>
    [SerializeField] protected PhotonView fxTransformPV = null;
    #endregion

    #region Coroutines
    /// <summary>
    /// Reference of the current coroutine of the dodge method.
    /// </summary>
    protected Coroutine dodgeCoroutine = null;

    /// <summary>
    /// Reference of the current coroutine making the player going around a certain position.
    /// </summary>
    protected Coroutine goAroundCoroutine = null;

    /// <summary>
    /// References the coroutine setting player invulnerability after being hit.
    /// </summary>
    protected Coroutine invulnerabilityCoroutine = null;

    /// <summary>
    /// References the current coroutine used for the jump attack.
    /// </summary>
    protected Coroutine jumpAttackCoroutine = null;

    /// <summary>
    /// References the current coroutine of the jump method. Null if none is actually running.
    /// </summary>
    protected Coroutine jumpCoroutine = null;

    /// <summary>
    /// References the current coroutine used to move the player inside the visible zone.
    /// </summary>
    protected Coroutine movePlayerInViewCoroutine = null;

    /// <summary>Backing field for <see cref="PreparingAttackCoroutine"/>.</summary>
    protected Coroutine preparingAttackCoroutine = null;
    #endregion

    #region Variables
    /// <summary>Backing field for <see cref="Attacks"/></summary>
    [SerializeField] protected TDS_Attack[] attacks = new TDS_Attack[] { };

    /// <summary>
    /// All attacks this player can perform.
    /// Contains informations about their animation, damages, effect and others.
    /// </summary>
    public TDS_Attack[] Attacks
    {
        get { return attacks; }
        protected set { attacks = value; }
    }

    /// <summary>
    /// Indicates if the player can flip.
    /// </summary>
    public bool CanFlip = true;

    /// <summary>Backing field for <see cref="IsDodging"/>.</summary>
    [SerializeField] protected bool isDodging = false;

    /// <summary>
    /// Is the player currently dodging ?
    /// </summary>
    public bool IsDodging
    {
        get { return isDodging; }
        protected set
        {
            isDodging = value;
        }
    }

    /// <summary>Backing field for <see cref="IsGrounded"/></summary>
    [SerializeField] protected bool isGrounded = true;

    /// <summary>
    /// Is the player touching the ground ?
    /// If true, jump is enabled.
    /// </summary>
    public bool IsGrounded
    {
        get { return isGrounded; }
        protected set
        {
            isGrounded = value;
        }
    }

    /// <summary>
    /// Get if the invulnerability coroutine is in process.
    /// </summary>
    public bool IsInInvulnerabilityCoroutine { get { return invulnerabilityCoroutine != null; } }

    /// <summary>Backing field for <see cref="IsJumping"/>.</summary>
    [SerializeField] protected bool isJumping = false;

    /// <summary>
    /// Is the player actually performing a jump ?
    /// </summary>
    public bool IsJumping
    {
        get { return isJumping; }
        protected set
        {
            isJumping = value;
        }
    }

    /// <summary>Backing field for <see cref="IsMoving"/>.</summary>
    [SerializeField] protected bool isMoving = false;

    /// <summary>
    /// Is the player actually moving ?
    /// </summary>
    public bool IsMoving
    {
        get { return isMoving; }
        protected set
        {
            isMoving = value;
        }
    }

    /// <summary>Backing field for <see cref="IsParrying"/>.</summary>
    [SerializeField] protected bool isParrying = false;

    /// <summary>
    /// Is the player currently parrying ?
    /// </summary>
    public bool IsParrying
    {
        get { return isParrying; }
        protected set
        {
            isParrying = value;
        }
    }

    /// <summary>
    /// Indicates if the character is under the player control or not.
    /// </summary>
    public bool IsPlayable = true;

    /// <summary>
    /// Indicates if the player is preparing an attack.
    /// </summary>
    [SerializeField] protected bool isPreparingAttack = false;

    /// <summary>Backing field for <see cref="ComboCurrent"/>.</summary>
    [SerializeField] protected List<bool> comboCurrent = new List<bool>();

    /// <summary>
    /// The state of the current combo.
    /// Index determines the position of an attack in the combo order.
    /// Bool value indicates if the attack was a light one ; true for light and false for heavy.
    /// </summary>
    public List<bool> ComboCurrent
    {
        get { return comboCurrent; }
        protected set
        {
            if (value.Count > comboMax)
            {
                value.RemoveRange(comboMax, value.Count - comboMax);
            }

            comboCurrent = value;
        }
    }

    /// <summary>Backing field for <see cref="ComboResetTime"/>.</summary>
    [SerializeField] protected float comboResetTime = 2;

    /// <summary>
    /// Time after which the current combo resets if no attack has been performed.
    /// </summary>
    public float ComboResetTime
    {
        get { return comboResetTime; }
        set
        {
            if (value < 0) value = 0;
            comboResetTime = value;
        }
    }

    /// <summary>
    /// Force given to the rigidbody velocity in Y of this player to perform a jump.
    /// </summary>
    public float JumpForce = 1;

    /// <summary>Backing field for <see cref="JumpMaximumTime"/></summary>
    [SerializeField] protected float jumpMaximumTime = 1.5f;

    /// <summary>
    /// Maximum time length of a jump.
    /// </summary>
    public float JumpMaximumTime
    {
        get { return jumpMaximumTime; }
        set
        {
            if (value < 0) value = 0;
            jumpMaximumTime = value;
        }
    }

    /// <summary>Backing field for <see cref="ComboMax"/>.</summary>
    [SerializeField] protected int comboMax = 3;

    /// <summary>
    /// The maximum length of the player's combos.
    /// When <see cref="ComboCurrent"/> reach this limit, the combo ends and it is reset.
    /// </summary>
    public int ComboMax
    {
        get { return comboMax; }
        set
        {
            if (value < 1) value = 1;
            comboMax = value;
        }
    }

    /// <summary>Backing field for <see cref="NextAction"/>.</summary>
    [SerializeField] private int nextAction = 0;

    /// <summary>
    /// Used as a buffer for the next player action ; 1 if light attack, 2 if heavy one, -1 if dodge, and of course, 0 for nothing.
    /// </summary>
    public int NextAction
    {
        get { return nextAction; }
        set
        {
            nextAction = value;
            CancelInvoke("ResetNextAttack");

            if (value != 0)
            {
                Invoke("ResetNextAttack", .5f);
            }
        }
    }

    /// <summary>
    /// LayerMask used to detect what is an obstacle for the player movements.
    /// </summary>
    public LayerMask WhatIsObstacle = new LayerMask();

    /// <summary>Backing field for <see cref="PlayerType"/>.</summary>
    [SerializeField] private PlayerType playerType = PlayerType.Unknown;

    /// <summary>
    /// What character type this player is ?
    /// </summary>
    public PlayerType PlayerType
    {
        get { return playerType; }
        protected set
        {
            playerType = value;
        }
    }
    #endregion

    #region Debug & Script memory Variables
    /// <summary>
    /// Timer used during dodge, to known for how it long it is processing.
    /// </summary>
    protected float dodgeTimer = 0;

    /// <summary>
    /// The position of the player collider at the previous frame
    /// </summary>
    private Vector3 previousColliderPosition = Vector3.zero;

    /// <summary>
    /// The position of the player at the previous frame
    /// </summary>
    private Vector3 previousPosition = Vector3.zero;

    /// <summary>
    /// Velocity used to throw an object.
    /// </summary>
    protected Vector3 throwVelocity = Vector3.zero;
    #endregion

    #region Animator
    private static readonly int comboBreaker_Hash = Animator.StringToHash("ComboBreaker");
    private static readonly int die_Hash = Animator.StringToHash("Die");
    private static readonly int dodge_Hash = Animator.StringToHash("Dodge");
    private static readonly int down_Hash = Animator.StringToHash("Down");
    private static readonly int groundState_Hash = Animator.StringToHash("GroundState");
    private static readonly int hasObject_Hash = Animator.StringToHash("HasObject");
    private static readonly int heavyAttack_Hash = Animator.StringToHash("HeavyAttack");
    private static readonly int hit_Hash = Animator.StringToHash("Hit");
    private static readonly int isMoving_Hash = Animator.StringToHash("IsMoving");
    private static readonly int isParrying_Hash = Animator.StringToHash("IsParrying");
    private static readonly int isSliding_Hash = Animator.StringToHash("IsSliding");
    private static readonly int jumpAttack_Hash = Animator.StringToHash("JumpAttack");
    private static readonly int lightAttack_Hash = Animator.StringToHash("LightAttack");
    private static readonly int revive_Hash = Animator.StringToHash("REVIVE");
    private static readonly int throw_Hash = Animator.StringToHash("Throw");
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions

    #region Aim & Throwables
    /// <summary>
    /// Start a coroutine to drop object if the button is maintained or to throw it.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator DropObjectCoroutine()
    {
        float _timer = DROP_OBJECT_TIME;

        while (controller.GetButton(ButtonType.Interact))
        {
            yield return null;
            _timer -= Time.deltaTime;

            // If no throwable anymore, break
            if (!throwable) yield break;

            if (_timer > 0) continue;

            DropObject();
            yield break;
        }

        // If no throwable anymore, break
        if (!throwable || (playerType == PlayerType.Juggler)) yield break;

        // Throw the object
        IsPlayable = false;
        SetAnimOnline(PlayerAnimState.Throw);

        yield break;
    }

    /// <summary>
    /// Removes the throwable from the character.
    /// </summary>
    /// <returns>Returns true if successfully removed the throwable, false otherwise.</returns>
    public override bool RemoveThrowable()
    {
        if (!base.RemoveThrowable())
            return false;

        // Set animation
        SetAnim(PlayerAnimState.LostObject);

        // Activates the detection box
        interactionBox.DisplayInteractionFeedback(true);

        // Triggers event
        if (photonView.isMine)
            OnHasObject?.Invoke(false);

        return true;
    }

    /// <summary>
    /// Set this character throwable.
    /// </summary>
    /// <param name="_throwable">Throwable to set.</param>
    /// <returns>Returns true if successfully set the throwable, false otherwise.</returns>
    public override bool SetThrowable(TDS_Throwable _throwable)
    {
        if (!base.SetThrowable(_throwable))
            return false;

        // Set animation
        SetAnim(PlayerAnimState.HasObject);

        // Desactivates the detection box
        interactionBox.DisplayInteractionFeedback(false);

        // Triggers event
        if (photonView.isMine)
            OnHasObject?.Invoke(true);

        return true;
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Makes the player active its planned attack.
    /// </summary>
    /// <param name="_attackIndex">Index of the attack to activate from <see cref="attacks"/>.</param>
    /// <returns>Returns true if the attack as correctly been activated, false otherwise.</returns>
    public virtual bool ActiveAttack(int _attackIndex)
    {
        #if UNITY_EDITOR
        // If index is out of range, debug it
        if ((_attackIndex < 0) || (_attackIndex >= attacks.Length))
        {
            Debug.LogWarning($"The Player \"{name}\" has no selected attack to perform");
            return false;
        }
        #endif

        // Set jump attack bonus damages
        if (!isGrounded || isJumping)
        {
            if (transform.position.y < 2) SetBonusDamages((int)(attacks[_attackIndex].DamagesMin * ((transform.position.y / 2f) - 1)));
            else SetBonusDamages((attacks[_attackIndex].DamagesMax - attacks[_attackIndex].DamagesMin) / 2);
        }
      
        // Activate the hit box
        hitBox.Activate(attacks[_attackIndex]);

        // Play sound
        attacks[_attackIndex].PlaySound(gameObject);

        return true;
    }

    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public virtual void Attack(bool _isLight)
    {
        IsAttacking = true;
        OnStartAttack?.Invoke();

        // Adds the current combo to the list
        ComboCurrent.Add(_isLight);

        if (!isGrounded || isJumping)
        {
            SetAnimOnline(PlayerAnimState.JumpAttack);
            jumpAttackCoroutine = StartCoroutine(JumpAttackMove());
            return;
        }

        // Set animator
        if (_isLight) SetAnimOnline(PlayerAnimState.LightAttack);
        else SetAnimOnline(PlayerAnimState.HeavyAttack);

        #if UNITY_EDITOR
        if (comboCurrent.Count > comboMax) Debug.LogError($"Player \"{name}\" should not have a combo of {comboCurrent.Count} !");
        #endif
    }

    /// <summary>
    /// Breaks an on going combo, with animation set.
    /// </summary>
    public virtual void BreakCombo()
    {
        if (ComboCurrent.Count > 0) SetAnimOnline(PlayerAnimState.ComboBreaker);

        ComboCurrent = new List<bool>();
        CancelInvoke("BreakCombo");
    }

    /// <summary>
    /// Desactivate the hit box of the character.
    /// </summary>
    public virtual void DesactiveHitBox() => hitBox.Desactivate();

    /// <summary>
    /// Ends definitively the current attack and enables back the capacity to attack once more.
    /// </summary>
    protected virtual void EndAttack()
    {
        IsAttacking = false;
        OnStopAttack?.Invoke();
        
        // If haven't yet reached the end of the combo, plan to reset it in X seconds if  not attacking before
        if (comboCurrent.Count < comboMax)
        {
            if ((nextAction < 1) && (comboCurrent.Count > 0)) Invoke("BreakCombo", comboResetTime);
        }
        else
        {
            // Reset the combo when reaching its end
            BreakCombo();
        }

        // Activates the detection box
        interactionBox.DisplayInteractionFeedback(true);

        // Executes next planned action
        ExecuteNextAction();
    }

    /// <summary>
    /// Makes the player move forward during the jump attack.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator JumpAttackMove()
    {
        rigidbody.isKinematic = true;

        float _movement = 2f;
        while (!isGrounded)
        {
            MoveInDirection(new Vector3(transform.position.x, transform.position.y - _movement, transform.position.z));
            if (transform.position.y < 0)
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                break;
            }

            _movement *= 1.2f;
            yield return null;
        }

        // Screen shake
        float _force = Mathf.Clamp(_movement / 500, .01f, .025f);
        TDS_Camera.Instance.StartScreenShake(_force, _force * 10);

        // Make the player bounce
        for (int _i = 0; _i < 5; _i++)
        {
            MoveInDirection(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));
            yield return null;
        }
        for (int _i = 0; _i < 5; _i++)
        {
            MoveInDirection(new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z));
            yield return null;
        }

        StopAttack();
        BreakCombo();
        rigidbody.isKinematic = false;
        jumpAttackCoroutine = null;
    }

    /// <summary>
    /// Makes the player prepare an attack.
    /// By default, the player is supposed to just directly attack ; but for certain situations, the attack might not played directly : that's the goal of this method, to be override to rewrite a pre-attack behaviour.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    protected virtual IEnumerator PrepareAttack(bool _isLight)
    {
        if (isDodging)
        {
            while (isDodging)
            {
                yield return null;

                if (dodgeTimer > DODGE_MINIMUM_TIMER)
                {
                    StopDodge();
                    //SetAnimOnline(PlayerAnimState.Dodge);
                    break;
                }
            }

            yield return null;
        }

        Attack(_isLight);

        preparingAttackCoroutine = null;
        isPreparingAttack = false;
        yield break;
    }

    /// <summary>
    /// Resets the on going combo.
    /// </summary>
    public virtual void ResetCombo()
    {
        ComboCurrent = new List<bool>();
        CancelInvoke("BreakCombo");
    }

    /// <summary>
    /// Makes the player start preparing an attack. This is the method called just before calling the <see cref="PrepareAttack(bool)"/> coroutine.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public virtual void StartPreparingAttack(bool _isLight)
    {
        if (isPreparingAttack || (comboCurrent.Count == comboMax) || ((!isGrounded || isJumping) && isAttacking)) return;

        // If already attacking, just stock this attack as the next one
        if (isAttacking)
        {
            NextAction = _isLight ? 1 : 2;
            return;
        }

        CancelInvoke("BreakCombo");

        // Desactivates the detection box
        interactionBox.DisplayInteractionFeedback(false);

        SetBonusDamages(0);

        isPreparingAttack = true;
        preparingAttackCoroutine = StartCoroutine(PrepareAttack(_isLight));
    }

    /// <summary>
    /// Stops this player's current attack if attacking.
    /// </summary>
    public override void StopAttack()
    {
        // Stop it, please
        if (hitBox.IsActive) DesactiveHitBox();

        if (isAttacking) Invoke("EndAttack", .1f);
    }

    /// <summary>
    /// Stops the movement for the jump attack.
    /// </summary>
    public virtual void StopJumpAttackMovement()
    {
        if (jumpAttackCoroutine == null) return;

        StopCoroutine(jumpAttackCoroutine);

        StopAttack();
        BreakCombo();
        rigidbody.isKinematic = false;
        jumpAttackCoroutine = null;
    }

    /// <summary>
    /// Stops the player from preparing an attack.
    /// </summary>
    /// <returns>Returns true if successfully stopped preparing an attack, false if none was in preparation.</returns>
    public virtual bool StopPreparingAttack()
    {
        if (!isPreparingAttack) return false;

        StopCoroutine(preparingAttackCoroutine);
        preparingAttackCoroutine = null;
        isPreparingAttack = false;

        // Activates the detection box
        interactionBox.DisplayInteractionFeedback(true);

        return true;
    }

    /// <summary>
    /// Performs the Super attack if the gauge is filled enough.
    /// </summary>
    public virtual void SuperAttack()
    {
        // SUPER attack
        Debug.Log("Super Attack !!");
    }
    #endregion

    #region Actions
    /// <summary>
    /// Performs the catch attack of this player.
    /// </summary>
    public virtual void Catch()
    {
        // Catch

        // Triggers the associated animation
        SetAnimOnline(PlayerAnimState.Catch);
    }

    /// <summary>
    /// Performs a dodge.
    /// While dodging, the player cannot take damage or attack.
    /// </summary>
    protected virtual IEnumerator Dodge()
    {
        // Dodge !
        SetInvulnerable(true);
        isDodging = true;
        dodgeTimer = 0;

        // Desactivates the detection box
        interactionBox.DisplayInteractionFeedback(false);

        OnStartDodging?.Invoke();

        // Get player movement
        Vector3 _movement = new Vector3(Mathf.RoundToInt(controller.GetAxis(AxisType.Horizontal)), 0, Mathf.RoundToInt(controller.GetAxis(AxisType.Vertical)));
        if ((_movement == Vector3.zero) || ((_movement.x != 0) && (_movement.z != 0))) _movement = Vector3.right * isFacingRight.ToSign();
        _movement = _movement.normalized;
        _movement *= speedMax * Mathf.Clamp(speedCoef, 0f, 1f);

        // Adds an little force at the start of the dodge
        rigidbody.AddForce(_movement * Mathf.Clamp(speedCurrent, speedInitial, speedMax) * (isGrounded ? 10 : 2));

        // Triggers the associated animation
        SetAnimOnline(PlayerAnimState.Dodge);

        // Play sound
        AkSoundEngine.PostEvent("Play_DODGE", gameObject); 

        // Get new constant movement
        Vector3 _movementDirection = new Vector3(_movement.x != 0 ? Mathf.Sign(_movement.x) : 0, 0, _movement.z != 0 ? Mathf.Sign(_movement.z) : 0);

        // Adds a little force to the player to move him along while dodging
        while (true)
        {
            Vector3 _thisMovement = _movement * (isGrounded ? 7 : 2.5f);
            _thisMovement.y = isGrounded ? 0 : -.35f;

            rigidbody.AddForce(_thisMovement);
            MoveInDirection(transform.position + _movementDirection);

            yield return null;
            dodgeTimer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Executes the next action.
    /// </summary>
    protected void ExecuteNextAction()
    {
        if (nextAction == 0) return;

        if (nextAction < 0) StartDodge();
        else StartPreparingAttack(nextAction == 1);

        NextAction = 0;
    }

    /// <summary>
    /// Set the player in parry position.
    /// While parrying, the player avoid to take damages.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Parry()
    {
        // Parry
        SetInvulnerable(true);
        isParrying = true;

        // Desactivates the detection box
        interactionBox.DisplayInteractionFeedback(false);

        SetAnimOnline(PlayerAnimState.Parrying);

        OnStartParry?.Invoke();

        // While holding the parry button, parry attacks
        while (controller.GetButton(ButtonType.Parry))
        {
            yield return null;
        }

        StopParry();
    }

    /// <summary>
    /// Resets the player planned next action.
    /// </summary>
    public void ResetNextAction() => NextAction = 0;

    /// <summary>
    /// Make the player dodge.
    /// </summary>
    public virtual void StartDodge()
    {
        if (isAttacking)
        {
            // And if in combo, reset it
            if (comboCurrent.Count > 0) BreakCombo();
            if (IsAttacking) StopAttack();

            // What's better ??

            //NextAction = -1;
            //return;
        }

        dodgeCoroutine = StartCoroutine(Dodge());
    }

    /// <summary>
    /// Stops the current dodge if dodging.
    /// </summary>
    public virtual void StopDodge()
    {
        if (!isDodging) return;

        // If dodge coroutine still active, disable it
        if (dodgeCoroutine != null)
        {
            StopDodgeMove();
        }

        // Stop dodging
        SetInvulnerable(false);
        isDodging = false;
        dodgeTimer = 0;

        // Activates the detection box
        interactionBox.DisplayInteractionFeedback(true);

        // Call events
        OnStopDodge?.Invoke();

        OnStopDodgeOneShot?.Invoke();
        OnStopDodgeOneShot = null;

        // Executes next planned action
        ExecuteNextAction();
    }

    /// <summary>
    /// Stops the automatic movement when dodging.
    /// </summary>
    public void StopDodgeMove()
    {
        if (dodgeCoroutine != null)
        {
            StopCoroutine(dodgeCoroutine);

            Vector3 _velocity = Vector3.zero;

            if (!isGrounded)
            {
                _velocity.x = rigidbody.velocity.x * .8f;
                _velocity.y = rigidbody.velocity.y * 1.5f;
                _velocity.z = rigidbody.velocity.z * .8f;
            }

            rigidbody.velocity = _velocity;
        }
    }

    /// <summary>
    /// Stops the player from parrying.
    /// </summary>
    public void StopParry()
    {
        if (!isParrying) return;

        // Stop parrying
        SetAnimOnline(PlayerAnimState.NotParrying);
        isParrying = false;
        SetInvulnerable(false);

        // Activates the detection box
        interactionBox.DisplayInteractionFeedback(true);

        OnStopParry?.Invoke();
    }

    /// <summary>
    /// Use the selected object in the inventory.
    /// </summary>
    public virtual void UseObject()
    {
        // Use
    }
    #endregion

    #region Effects
    /// <summary>
    /// Bring this damageable closer from a certain distance.
    /// </summary>
    /// <param name="_distance">Distance to browse.</param>
    public override bool BringCloser(float _distance)
    {
        if (!base.BringCloser(_distance)) return false;

        FreezePlayer();
        SetInvulnerable(true);

        // Set Animation
        SetAnimOnline(PlayerAnimState.Sliding);

        return true;
    }

    /// <summary>
    /// Tells the Character that he's getting up.
    /// </summary>
    public override void GetUp()
    {
        base.GetUp();

        UnfreezePlayer();
        SetInvulnerable(false);
    }

    /// <summary>
    /// Put the character on the ground.
    /// </summary>
    public override bool PutOnTheGround()
    {
        if (!base.PutOnTheGround()) return false;

        FreezePlayer();
        SetInvulnerable(true);

        // Set animation
        SetAnimOnline(PlayerAnimState.Down);

        return true;
    }

    /// <summary>
    /// Method called when stopped being bringed closer.
    /// </summary>
    public override void StopBringingCloser()
    {
        base.StopBringingCloser();

        SetInvulnerable(false);
        UnfreezePlayer();

        // Set animation
        SetAnimOnline(PlayerAnimState.NotSliding);
    }
    #endregion

    #endregion

    #region Health
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected override void Die()
    {
        base.Die();

        OnPlayerDie?.Invoke(this);

        if (PhotonNetwork.isMasterClient)
            DropObject();

        if (!photonView.isMine) return;

        // Removes the player to follow for the camera if offline mode
        if (PhotonNetwork.offlineMode)
        {
            TDS_Camera.Instance.RemoveLocalPlayer(this);
            if (movePlayerInViewCoroutine != null) StopMovingPlayerInView();
        }

        AkSoundEngine.PostEvent("Stop_APROACHING_DEATH", gameObject);

        // Desactivates the detection box
        interactionBox.DisplayInteractionFeedback(false);

        // Triggers associated animations
        SetAnimOnline(PlayerAnimState.Die);
    }

    /// <summary>
    /// Makes this object be healed and restore its health.
    /// </summary>
    /// <param name="_heal">Amount of health point to restore.</param>
    public override void Heal(int _heal)
    {
        float _healthBefore = healthCurrent;

        base.Heal(_heal);

        if (photonView.isMine)
        {
            TDS_VFXManager.Instance.SpawnEffect(FXType.Heal, fxTransformPV);

            // Stop feedback sound
            float _treshold = healthMax / 4f;

            if ((_healthBefore <= _treshold) && (healthCurrent > _treshold))
            {
                AkSoundEngine.PostEvent("Stop_APROACHING_DEATH", TDS_GameManager.MainAudio);
            }
        }
    }

    /// <summary>
    /// Method called when this character hit something.
    /// Override it to implement feedback and other things.
    /// </summary>
    /// <param name="_opponentXCenter">X value of the opponent collider center position.</param>
    /// <param name="_opponentYMax">Y value of the opponent collider max position.</param>
    /// <param name="_opponentZ">Z value of the opponent position.</param>
    public override void HitCallback(float _opponentXCenter, float _opponentYMax, float _opponentZ)
    {
        base.HitCallback(_opponentXCenter, _opponentYMax, _opponentZ);

        // Instantiate cool FX
        TDS_VFXManager.Instance.SpawnOpponentHitEffect(new Vector3(_opponentXCenter, _opponentYMax + .25f, _opponentZ) + ((Vector3)Random.insideUnitCircle * .5f));

        // Screen shake guy !
        if ((comboCurrent.Count > 0) && comboCurrent.Last())
        {
            TDS_Camera.Instance.StartScreenShake(.012f, .25f);
        }
        else TDS_Camera.Instance.StartScreenShake(.015f, .35f);
    }

    /// <summary>
    /// Set invulnerability during a certain time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Invulnerability()
    {
        yield return null;

        // If down of bringed closer, just return
        if (IsDown || (bringingCloserCoroutine != null)) yield break;

        SetInvulnerable(true);

        FreezePlayer();
        Invoke("UnfreezePlayer", INVULNERABILITY_TIME / 2f);

        float _timer = INVULNERABILITY_TIME;
        while (_timer > 0)
        {
            yield return new WaitForSeconds(INVULNERABILITY_TIME / 7);
            _timer -= INVULNERABILITY_TIME / 7;
            sprite.gameObject.SetActive(!sprite.gameObject.activeInHierarchy);
        }

        sprite.gameObject.SetActive(true);
        SetInvulnerable(false);

        invulnerabilityCoroutine = null;
        yield break;
    }

    /// <summary>
    /// Method called when this object gets back from the deads.
    /// </summary>
    protected override void Revive()
    {
        base.Revive();

        if (!photonView.isMine) return;

        // Adds the player to follow for the camera if offline mode
        if (PhotonNetwork.offlineMode)
        {
            TDS_Camera.Instance.AddLocalPlayer(this);
        }

        // Triggers associated animations
        SetAnimOnline(PlayerAnimState.BackFromTheDeads);
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        // Cannot hit the player while in cutscene !
        if (TDS_GameManager.IsInCutscene) return false;

        float _healthBefore = healthCurrent;

        // Executes base method
        if (!base.TakeDamage(_damage))
        {
            if (photonView.isMine) TDS_Camera.Instance.StartScreenShake(.02f, .2f);

            // Play parry sound
            AkSoundEngine.PostEvent("Play_PARRY", gameObject);
            return false;
        }

        if (photonView.isMine)
        {
            // Spawn hit effect
            TDS_VFXManager.Instance.SpawnPlayerHitEffect(new Vector3(collider.bounds.center.x, collider.bounds.max.y + .25f, transform.position.z) + ((Vector3)Random.insideUnitCircle * .5f));

            // If preparing an attack, stop it
            if (isPreparingAttack) StopPreparingAttack();

            // And if in combo, reset it
            if (comboCurrent.Count > 0) BreakCombo();
            if (IsAttacking) StopAttack();
            if (isDodging) StopDodge();

            // Play feedback sound
            float _treshold = healthMax / 4f;

            if ((_healthBefore > _treshold) && (healthCurrent <= _treshold))
            {
                AkSoundEngine.PostEvent("Play_APROACHING_DEATH", gameObject);
            }
        }

        // If not dead, be just hit
        if (!isDead)
        {
            invulnerabilityCoroutine = StartCoroutine(Invulnerability());

            if (photonView.isMine)
            {
                TDS_Camera.Instance.StartScreenShake(.02f, .5f);

                // Triggers associated animation
                if (!IsDown) SetAnimOnline(PlayerAnimState.Hit);
            }
        }
        else if (photonView.isMine)
        {
            TDS_Camera.Instance.StartScreenShake(.05f, .5f);
        }
        
        return true;
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <param name="_position">Position in world space from where the hit come from.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage, Vector3 _position)
    {
        if (!base.TakeDamage(_damage, _position))
        {
            if (photonView.isMine)
            {
                transform.position += new Vector3(.025f * (_position.x < transform.position.x ? 1 : 1), 0, 0);
            }
            return false;
        }
        return true;
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Interacts with the nearest available object in range.
    /// </summary>
    /// <returns>Returns true if interacted with something. False if nothing was found.</returns>
    public virtual bool Interact()
    {
        // Interact !
        if (throwable && (playerType != PlayerType.Juggler))
        {
            StartCoroutine(DropObjectCoroutine());
            return true;
        }

        // Get the nearest object in range ; if null, cannot interact, so return false
        GameObject _nearestObject = interactionBox.NearestObject;

        if (!_nearestObject) return false;

        TDS_Throwable _throwable = null;

        // Interact now with the object depending on its type
        if (_throwable = _nearestObject.GetComponent<TDS_Throwable>())
        {
            GrabObject(_throwable);
            return true;
        }

        TDS_Consumable _consumable = null;
        if (_consumable = _nearestObject.GetComponent<TDS_Consumable>())
        {
            _consumable.Use(this);
            return true;
        }

        return false;
    }
    #endregion

    #region Movements
    static Collider[] touchedColliders = new Collider[4];
    static int touchedCollidersAmount = 0;

    /// <summary>
    /// Adjusts the position of the player on the axis where a force is exercised on the rigidbody velocity.
    /// </summary>
    private void AdjustPositionOnRigidbody()
    {
        if (!photonView.isMine) return; 
        // If the player rigidbody velocity is null, return
        if (rigidbody.velocity == Vector3.zero) return;

        if ((touchedCollidersAmount = Physics.OverlapBoxNonAlloc(collider.bounds.center, collider.bounds.extents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore)) > 0)
        {
            for (int _i = 0; _i < touchedCollidersAmount; _i++)
            {
                if (Physics.ComputePenetration(collider, rigidbody.position, transform.rotation,
                                               touchedColliders[_i], touchedColliders[_i].transform.position, touchedColliders[_i].transform.rotation,
                                               out Vector3 _direction, out float _distance))
                {
                    rigidbody.position += _direction * _distance;
                    rigidbody.velocity -= Vector3.Scale(rigidbody.velocity, _direction);
                }
            }
        }
        return;

        // Old fashion way,
        // kept for history purpose.

        //// Get all touching colliders ; if none, return
        //if (Physics.OverlapBoxNonAlloc(collider.bounds.center, collider.bounds.extents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore) == 0)
        //    return;

        //// For each axis where the player rigidbody velocity is non null, adjust the player position if it is in another collider
        //// To do this, use the previous position and overlap from this in the actual position for each axis where the velocity is not null

        //Vector3 _newPosition = transform.position;
        //Vector3 _newVelocity = rigidbody.velocity;
        //Vector3 _movementVector = transform.position - previousPosition;
        //Vector3 _colliderCenter = Vector3.Scale(collider.center, collider.transform.lossyScale);
        //Vector3 _colliderWorldPosition = collider.bounds.center;
        //Vector3 _colliderExtents = collider.bounds.extents - (Vector3.one * .0001f);
        //Vector3 _overlapCenter = Vector3.zero;
        //Vector3 _overlapExtents = Vector3.one;

        //// X axis adjustment
        //if (rigidbody.velocity.x != 0)
        //{
        //    // Get the extents & center position for the overlap
        //    _overlapExtents = new Vector3(Mathf.Abs(_movementVector.x) / 2, _colliderExtents.y, _colliderExtents.z);

        //    _overlapCenter = new Vector3(previousColliderPosition.x + ((_colliderExtents.x + _overlapExtents.x) * Mathf.Sign(rigidbody.velocity.x)), previousColliderPosition.y, previousColliderPosition.z);

        //    // Overlap in the zone where the player would be from the previous position after the movement on the X axis.
        //    // If something is touched, then adjust the position of the player against it
        //    if ((touchedCollidersAmount = Physics.OverlapBoxNonAlloc(_overlapCenter, _overlapExtents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore)) > 0)
        //    {
        //        float _xLimit = 0;

        //        // Get the X position of the nearest collider limit, and set the position of the player against it
        //        if (_movementVector.x > 0)
        //        {
        //            _xLimit = touchedColliders[0].bounds.center.x - touchedColliders[0].bounds.extents.x;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _xLimit = Mathf.Min(_xLimit, touchedColliders[_i].bounds.center.x - touchedColliders[_i].bounds.extents.x);
        //            }

        //            _newPosition.x = _xLimit - (_colliderExtents.x - _colliderCenter.x) - .001f;
        //        }
        //        else
        //        {
        //            _xLimit = touchedColliders[0].bounds.center.x + touchedColliders[0].bounds.extents.x;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _xLimit = Mathf.Max(_xLimit, touchedColliders[_i].bounds.center.x + touchedColliders[_i].bounds.extents.x);
        //            }

        //            _newPosition.x = _xLimit + (_colliderExtents.x - _colliderCenter.x) + .001f;
        //        }

        //        _movementVector.x = _newPosition.x - previousPosition.x;

        //        // Reset the X velocity
        //        _newVelocity.x = 0;
        //    }
        //}

        //// Y axis adjustment
        //if (rigidbody.velocity.y != 0)
        //{
        //    // Get the extents & center position for the overlap
        //    _overlapExtents = new Vector3(_colliderExtents.x, Mathf.Abs(_movementVector.y) / 2, _colliderExtents.z);

        //    _overlapCenter = new Vector3(previousColliderPosition.x, previousColliderPosition.y + ((_colliderExtents.y + _overlapExtents.y) * Mathf.Sign(rigidbody.velocity.y)), previousColliderPosition.z);

        //    // Overlap in the zone where the player would be from the previous position after the movement on the Y axis.
        //    // If something is touched, then adjust the position of the player against it
        //    if ((touchedCollidersAmount = Physics.OverlapBoxNonAlloc(_overlapCenter, _overlapExtents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore)) > 0)
        //    {
        //        float _yLimit = 0;

        //        // Get the Y position of the nearest collider limit, and set the position of the player against it
        //        if (_movementVector.y > 0)
        //        {
        //            _yLimit = touchedColliders[0].bounds.center.y - touchedColliders[0].bounds.extents.y;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _yLimit = Mathf.Min(_yLimit, touchedColliders[_i].bounds.center.y - touchedColliders[_i].bounds.extents.y);
        //            }

        //            _newPosition.y = _yLimit - (_colliderExtents.y - _colliderCenter.y) - .001f;
        //        }
        //        else
        //        {
        //            _yLimit = touchedColliders[0].bounds.center.y + touchedColliders[0].bounds.extents.y;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _yLimit = Mathf.Max(_yLimit, touchedColliders[_i].bounds.center.y + touchedColliders[_i].bounds.extents.y);
        //            }

        //            _newPosition.y = _yLimit + (_colliderExtents.y - _colliderCenter.y) + .001f;
        //        }

        //        _movementVector.y = _newPosition.y - previousPosition.y;

        //        // Reset the Y velocity
        //        _newVelocity.y = 0;
        //    }
        //}

        //// Z axis adjustment
        //if (rigidbody.velocity.z != 0)
        //{
        //    // Get the extents & center position for the overlap
        //    _overlapExtents = new Vector3(_colliderExtents.x, _colliderExtents.y, Mathf.Abs(_movementVector.z) / 2);

        //    _overlapCenter = new Vector3(previousColliderPosition.x, previousColliderPosition.y, previousColliderPosition.z + ((_colliderExtents.z + _overlapExtents.z) * Mathf.Sign(rigidbody.velocity.z)));

        //    // Overlap in the zone where the player would be from the previous position after the movement on the Z axis.
        //    // If something is touched, then adjust the position of the player against it
        //    if ((touchedCollidersAmount = Physics.OverlapBoxNonAlloc(_overlapCenter, _overlapExtents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore)) > 0)
        //    {
        //        float _zLimit = 0;

        //        // Get the Z position of the nearest collider limit, and set the position of the player against it
        //        if (_movementVector.z > 0)
        //        {
        //            _zLimit = touchedColliders[0].bounds.center.z - touchedColliders[0].bounds.extents.z;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _zLimit = Mathf.Min(_zLimit, touchedColliders[_i].bounds.center.z - touchedColliders[_i].bounds.extents.z);
        //            }

        //            _newPosition.z = _zLimit - (_colliderExtents.z - _colliderCenter.z) - .001f;
        //        }
        //        else
        //        {
        //            _zLimit = touchedColliders[0].bounds.center.z + touchedColliders[0].bounds.extents.z;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _zLimit = Mathf.Max(_zLimit, touchedColliders[_i].bounds.center.z + touchedColliders[_i].bounds.extents.z);
        //            }

        //            _newPosition.z = _zLimit + (_colliderExtents.z - _colliderCenter.z) + .001f;
        //        }

        //        _movementVector.z = _newPosition.z - previousPosition.z;

        //        // Reset the Z velocity
        //        _newVelocity.z = 0;
        //    }
        //}

        //// Set the position of the player as the new calculated one, and reset the velocity for the recalculated axis
        //transform.position = _newPosition;
        //rigidbody.velocity = _newVelocity;
    }

    /// <summary>
    /// Checks if the player is grounded and updates related elements and parameters.
    /// </summary>
    private void CheckGrounded()
    {
        // Set the player as grounded if something is detected in the ground detection box
        bool _isGrounded = groundDetectionBox.DoOverlap(transform.position);

        // If grounded value changed, updates all necessary things
        if (_isGrounded != IsGrounded)
        {
            // Updates value
            IsGrounded = _isGrounded;

            // Updates the ground state information in the animator

            if (!_isGrounded)
            {
                SpeedCoef *= .7f;

                // Activates event
                OnGetOffGround?.Invoke();
            }
            else
            {
                SpeedCoef /= .7f;
                rigidbody.velocity = Vector3.zero;

                // Activates event
                OnGetOnGround?.Invoke();

                // Plays land sound
                AkSoundEngine.PostEvent("Play_LAND_jump", gameObject);
            }
        }

        // Updates animator grounded informations
        if (!_isGrounded && !isDodging && !isAttacking)
        {
            if (rigidbody.velocity.y < 0)
            {
                if (animator.GetInteger(groundState_Hash) > -1)
                {
                    SetAnimOnline(PlayerAnimState.Falling);
                }
            }
            else if (animator.GetInteger(groundState_Hash) < 1)
            {
                SetAnimOnline(PlayerAnimState.Jumping);
            }
        }
        else if (animator.GetInteger(groundState_Hash) != 0)
        {
            SetAnimOnline(PlayerAnimState.Grounded);
        }
    }

    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        base.Flip();

        interactionBox.RotateText();
    }

    /// <summary>
    /// Freezes the player's movements and actions.
    /// </summary>
    public virtual void FreezePlayer()
    {
        if (!photonView.isMine)
        {
            if (PhotonNetwork.isMasterClient)
            {
                TDS_RPCManager.Instance.CallRPC(photonView.owner, photonView, GetType(), "FreezePlayer", new object[] { });
            }

            return;
        }

        IsPlayable = false;
        if (isDodging) StopDodge();
        if (isPreparingAttack) StopPreparingAttack();
        if (isAttacking) StopAttack();
        if (isMoving)
        {
            isMoving = false;
            SetAnimOnline(PlayerAnimState.Idle);
        }
        if (!isGrounded) SetAnimOnline(PlayerAnimState.Grounded);
    }

    /// <summary>
    /// Makes the player go around a certain position.
    /// </summary>
    /// <param name="_position">Where to go.</param>
    /// <param name="_unfreezeAfter">Should the player be unfreezed after move.</param>
    public void GoAround(Vector3 _position, bool _unfreezeAfter = true)
    {
        if (goAroundCoroutine != null) StopCoroutine(goAroundCoroutine);
        goAroundCoroutine = StartCoroutine(GoAroundCoroutine(_position, _unfreezeAfter));
    }

    /// <summary>
    /// Coroutine making the player going around a certain position.
    /// </summary>
    /// <param name="_position">Where to go.</param>
    /// <param name="_unfreezeAfter">Should the player be unfreezed after move.</param>
    /// <returns></returns>
    private IEnumerator GoAroundCoroutine(Vector3 _position, bool _unfreezeAfter)
    {
        FreezePlayer();
        SpeedCoef *= .25f;

        if (Mathf.Sign(_position.x - transform.position.x) != isFacingRight.ToSign()) Flip();

        // Moves around point while not in range enough
        while (Mathf.Abs((transform.position - _position).magnitude) > 2f)
        {
            yield return null;
            MoveInDirection(_position);
        }

        if (isMoving)
        {
            isMoving = false;
            SetAnimOnline(PlayerAnimState.Idle);
            SetAnimOnline(PlayerAnimState.Idle);
        }

        if (!isFacingRight) Flip();

        SpeedCoef /= .25f;
        if (_unfreezeAfter) UnfreezePlayer();
        goAroundCoroutine = null;
        yield break;
    }

    /// <summary>
    /// Starts a jump.
    /// Jump higher while maintaining the jump button.
    /// When releasing the button, stop adding force to the jump.
    /// <see cref="JumpMaximumTime"/> determines the maximum time of a jump.
    /// </summary>
    /// <returns>Returns the world.</returns>
    private IEnumerator Jump()
    {
        // Creates a float to use as timer
        float _timer = 0;

        IsJumping = true;

        // Call one shot event
        OnJump?.Invoke();

        // Adds a base vertical force to the rigidbody to expels the player in the air
        rigidbody.AddForce(Vector3.up * JumpForce);

        // Plays jump sound
        AkSoundEngine.PostEvent("Play_START_jump", gameObject);

        while (controller.GetButton(ButtonType.Jump) && _timer < JumpMaximumTime)
        {
            rigidbody.AddForce(Vector3.up * (JumpForce / JumpMaximumTime) * Time.deltaTime);
            yield return null;

            _timer += Time.deltaTime;
        }

        IsJumping = false;
    }

    /// <summary>
    /// Moves the player in a direction according to a position.
    /// </summary>
    /// <param name="_newPosition">Position where to move the player. (World space)</param>
    public void MoveInDirection(Vector3 _newPosition)
    {
        if (!photonView.isMine) return; 

        // Increases speed if needed
        if (speedCurrent < SpeedMax)
        {
            IncreaseSpeed();
        }

        float _speed = speedCurrent * speedCoef;

        // Adjust future position by checking possible collisions
        _newPosition = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _speed);

        // Move the player
        MoveTo(_newPosition);
    }

    private bool isMovingPlayerInView = false;

    /// <summary>
    /// Moves the player inside the visible zone.
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePlayerInView()
    {
        float _timer = 3;
        while (_timer > 0)
        {
            yield return null;
            _timer -= Time.deltaTime;
        }

        isMovingPlayerInView = true;
        FreezePlayer();

        rigidbody.isKinematic = true;
        enabled = false;
        isMoving = true;
        SetAnim(PlayerAnimState.Run);

        TDS_Player _destinationPlayer = TDS_LevelManager.Instance.AllPlayers.FirstOrDefault(p => p.sprite.isVisible && p.enabled);

        if (!_destinationPlayer)
        {
            StopMovingPlayerInView();
            yield break;
        }

        if (((transform.position.x < _destinationPlayer.transform.position.x) && !isFacingRight) || ((transform.position.x > _destinationPlayer.transform.position.x) && isFacingRight))
        {
            Flip();
        }

        while (true)
        {
            if (_destinationPlayer.sprite.isVisible || _destinationPlayer.enabled)
            {
                _destinationPlayer = TDS_LevelManager.Instance.AllPlayers.FirstOrDefault(p => p.sprite.isVisible && p.enabled);

                if (!_destinationPlayer)
                {
                    StopMovingPlayerInView();
                    yield break;
                }
            }

            transform.position = Vector3.Lerp(transform.position, _destinationPlayer.transform.position, Time.deltaTime * speedMax);

            yield return null;
        }
    }

    /// <summary>
    /// Move directly the player to a new position.
    /// </summary>
    /// <param name="_newPosition">New position to move to.</param>
    public void MoveTo(Vector3 _newPosition)
    {
        rigidbody.position = _newPosition;

        if ((touchedCollidersAmount = Physics.OverlapBoxNonAlloc(collider.bounds.center, collider.bounds.extents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore)) > 0)
        {
            for (int _i = 0; _i < touchedCollidersAmount; _i++)
            {
                if (Physics.ComputePenetration(collider, rigidbody.position, transform.rotation,
                                               touchedColliders[_i], touchedColliders[_i].transform.position, touchedColliders[_i].transform.rotation,
                                               out Vector3 _direction, out float _distance))
                {
                    rigidbody.position += _direction * _distance;
                }
            }
        }

        // Move the player
        if (transform.position != rigidbody.position)
        {
            transform.position = rigidbody.position;

            // If starting moving, update informations
            if (!isMoving)
            {
                isMoving = true;
                SetAnimOnline(PlayerAnimState.Run);
            }
        }
        else if (isMoving)
        {
            isMoving = false;
            SetAnimOnline(PlayerAnimState.Idle);
        }
        return;

        // Old fashion way,
        // kept for history purpose.

        // For X & Z axis, overlap in the zone between this position and the future one ; priority order is X, & Z.
        // If something is touched, use the bounds of the collider to set the position of the player against the obstacle.
        //Vector3 _movementVector = _newPosition - transform.position;
        //Vector3 _colliderCenter = Vector3.Scale(collider.center, collider.transform.lossyScale);
        //Vector3 _colliderWorldPosition = collider.bounds.center;
        //Vector3 _colliderExtents = collider.bounds.extents - (Vector3.one * .0001f);
        //Vector3 _overlapCenter = Vector3.zero;
        //Vector3 _overlapExtents = Vector3.one;

        //// X axis movement test
        //if (_movementVector.x != 0)
        //{
        //    // Get the extents & center positon for the overlap
        //    _overlapExtents = new Vector3(Mathf.Abs(_movementVector.x) / 2, _colliderExtents.y, _colliderExtents.z);

        //    _overlapCenter = new Vector3(_colliderWorldPosition.x + ((_colliderExtents.x + _overlapExtents.x) * Mathf.Sign(_movementVector.x)), _colliderWorldPosition.y, _colliderWorldPosition.z);

        //    // Overlaps in the position where the player would be after the X movement ;
        //    // If nothing is touched, then the player can move in X
        //    if ((touchedCollidersAmount = Physics.OverlapBoxNonAlloc(_overlapCenter, _overlapExtents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore)) > 0)
        //    {
        //        float _xLimit = 0;

        //        // Get the X position of the nearest collider limit, and set the position of the player against it
        //        if (_movementVector.x > 0)
        //        {
        //            _xLimit = touchedColliders[0].bounds.center.x - touchedColliders[0].bounds.extents.x;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _xLimit = Mathf.Min(_xLimit, touchedColliders[_i].bounds.center.x - touchedColliders[_i].bounds.extents.x);
        //            }

        //            _newPosition.x = _xLimit - (_colliderExtents.x + _colliderCenter.x) - .001f;
        //        }
        //        else
        //        {
        //            _xLimit = touchedColliders[0].bounds.center.x + touchedColliders[0].bounds.extents.x;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _xLimit = Mathf.Max(_xLimit, touchedColliders[_i].bounds.center.x + touchedColliders[_i].bounds.extents.x);
        //            }

        //            _newPosition.x = _xLimit + (_colliderExtents.x + _colliderCenter.x) + .001f;
        //        }

        //        _movementVector.x = _newPosition.x - transform.position.x;
        //    }
        //}

        //// Z axis movement test
        //if (_movementVector.z != 0)
        //{
        //    // Get the extents & center positon for the overlap ;
        //    // If the player can move in X, overlap from this new X position
        //    _overlapExtents = new Vector3(_colliderExtents.x, _colliderExtents.y, Mathf.Abs(_movementVector.z) / 2);

        //    _overlapCenter = new Vector3(_colliderWorldPosition.x + _movementVector.x, _colliderWorldPosition.y, _colliderWorldPosition.z + ((_colliderExtents.z + _overlapExtents.z) * Mathf.Sign(_movementVector.z)));

        //    // Overlaps in the position where the player would be after the Z movement ;
        //    // If nothing is touched, then the player can move in Z
        //    if ((touchedCollidersAmount = Physics.OverlapBoxNonAlloc(_overlapCenter, _overlapExtents, touchedColliders, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore)) > 0)
        //    {
        //        float _zLimit = 0;

        //        // Get the Z position of the nearest collider limit, and set the position of the player against it
        //        if (_movementVector.z > 0)
        //        {
        //            _zLimit = touchedColliders[0].bounds.center.z - touchedColliders[0].bounds.extents.z;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _zLimit = Mathf.Min(_zLimit, touchedColliders[_i].bounds.center.z - touchedColliders[_i].bounds.extents.z);
        //            }

        //            _newPosition.z = _zLimit - (_colliderExtents.z + _colliderCenter.z) - .001f;
        //        }
        //        else
        //        {
        //            _zLimit = touchedColliders[0].bounds.center.z + touchedColliders[0].bounds.extents.z;
        //            for (int _i = 1; _i < touchedCollidersAmount; _i++)
        //            {
        //                _zLimit = Mathf.Max(_zLimit, touchedColliders[_i].bounds.center.z + touchedColliders[_i].bounds.extents.z);
        //            }

        //            _newPosition.z = _zLimit + (_colliderExtents.z + _colliderCenter.z) + .001f;
        //        }
        //    }
        //}

        //// Move the player
        //if (transform.position != _newPosition)
        //{
        //    transform.position = _newPosition;

        //    // If starting moving, update informations
        //    if (!isMoving)
        //    {
        //        isMoving = true;
        //        SetAnimOnline(PlayerAnimState.Run);
        //    }
        //}
        //else if (isMoving)
        //{
        //    isMoving = false;
        //    SetAnimOnline(PlayerAnimState.Idle);
        //}
    }

    /// <summary>
    /// Starts a brand new jump !
    /// </summary>
    public virtual void StartJump()
    {
        // If there is already a jump coroutine running, stop it before starting the new one
        if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);

        jumpCoroutine = StartCoroutine(Jump());
    }

    /// <summary>
    /// Starts Moving the player inside the visible zone.
    /// </summary>
    public void StartMovingPlayerInView()
    {
        if ((movePlayerInViewCoroutine != null) || !gameObject.activeInHierarchy) return;
        movePlayerInViewCoroutine = StartCoroutine(MovePlayerInView());
    }

    /// <summary>
    /// Stops moving the player inside the visible zone.
    /// </summary>
    public void StopMovingPlayerInView()
    {
        if (movePlayerInViewCoroutine != null)
        {
            StopCoroutine(movePlayerInViewCoroutine);
            movePlayerInViewCoroutine = null;

            if (isMovingPlayerInView)
            {
                SetAnim(PlayerAnimState.Idle);
                rigidbody.isKinematic = false;

                isMoving = false;
                enabled = true;

                isMovingPlayerInView = false;
                UnfreezePlayer();
            }
        }
    }

    /// <summary>
    /// Unfreezes the player's movements and actions.
    /// </summary>
    public virtual void UnfreezePlayer()
    {
        if (!photonView.isMine)
        {
            if (PhotonNetwork.isMasterClient)
            {
                TDS_RPCManager.Instance.CallRPC(photonView.owner, photonView, GetType(), "UnfreezePlayer", new object[] { });
            }

            return;
        }

        if (TDS_GameManager.IsInCutscene) return;

        IsPlayable = true;
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set this player animator informations.
    /// </summary>
    /// <param name="_state">State of the player animator to set.</param>
    public void SetAnim(PlayerAnimState _state)
    {
        switch (_state)
        {
            case PlayerAnimState.Idle:
                animator.SetBool(isMoving_Hash, false);
                break;

            case PlayerAnimState.Run:
                animator.SetBool(isMoving_Hash, true);
                break;

            case PlayerAnimState.Hit:
                animator.SetTrigger(hit_Hash);
                break;

            case PlayerAnimState.Die:
                animator.SetTrigger(die_Hash);
                animator.SetInteger(groundState_Hash, 0);
                break;

            case PlayerAnimState.Dodge:
                animator.SetTrigger(dodge_Hash);
                break;

            case PlayerAnimState.Throw:
                if (!isGrounded) animator.SetInteger(groundState_Hash, 0);
                animator.SetTrigger(throw_Hash);
                break;

            case PlayerAnimState.Catch:
                // Nothing for now
                break;

            case PlayerAnimState.LightAttack:
                animator.SetTrigger(lightAttack_Hash);
                break;

            case PlayerAnimState.HeavyAttack:
                animator.SetTrigger(heavyAttack_Hash);
                break;

            case PlayerAnimState.ComboBreaker:
                animator.SetTrigger(comboBreaker_Hash);
                break;

            case PlayerAnimState.Super:
                // Nothing for now
                break;

            case PlayerAnimState.Grounded:
                animator.SetInteger(groundState_Hash, 0);
                break;

            case PlayerAnimState.Jumping:
                animator.SetInteger(groundState_Hash, 1);
                break;

            case PlayerAnimState.Falling:
                animator.SetInteger(groundState_Hash, -1);
                break;

            case PlayerAnimState.HasObject:
                animator.SetBool(hasObject_Hash, true);
                break;

            case PlayerAnimState.LostObject:
                animator.SetBool(hasObject_Hash, false);
                break;

            case PlayerAnimState.Parrying:
                animator.SetBool(isParrying_Hash, true);
                break;

            case PlayerAnimState.NotParrying:
                animator.SetBool(isParrying_Hash, false);
                break;

            case PlayerAnimState.Sliding:
                animator.SetBool(isSliding_Hash, true);
                break;

            case PlayerAnimState.NotSliding:
                animator.SetBool(isSliding_Hash, false);
                break;

            case PlayerAnimState.Down:
                animator.SetTrigger(down_Hash);
                break;

            case PlayerAnimState.BackFromTheDeads:
                animator.SetTrigger(revive_Hash);
                break;

            case PlayerAnimState.JumpAttack:
                animator.SetTrigger(jumpAttack_Hash);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Set this player animator informations.
    /// </summary>
    /// <param name="_animState">State of the player animator to set.</param>
    public void SetAnim(int _animState)
    {
        SetAnim((PlayerAnimState)_animState); 
    }

    /// <summary>
    /// Set this player animator informations, for all game clients.
    /// </summary>
    /// <param name="_state">State of the player animator to set.</param>
    public void SetAnimOnline(PlayerAnimState _state)
    {
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "SetAnim", new object[] { (int)_state });
        }

        SetAnim(_state);
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    /// <returns>Returns an int indicating at which step the method returned :
    /// 0 if everything went good ;
    /// A negative number if an action has been performed ;
    /// 1 if parrying or preparing an attack ;
    /// 3 if attacking or dodging.</returns>
    public virtual int CheckActionsInputs()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M)) OnStartAttack += () => SetBonusDamages(200);
        #endif

        // If dodging, parrying or attacking, do not perform action, and return 1
        if (isParrying || isPreparingAttack) return 1;

        // Check non-agressive actions
        if (controller.GetButtonDown(ButtonType.Interact) && !isAttacking && !isDodging)
        {
            Interact();
            return -1;
        }

        // Checks potentially agressives actions
        if (!IsPacific)
        {
            if (controller.GetButtonDown(ButtonType.LightAttack))
            {
                if (throwable && (playerType != PlayerType.Juggler))
                {
                    // Throw the object
                    IsPlayable = false;
                    SetAnimOnline(PlayerAnimState.Throw);
                    return -1;
                }

                StartPreparingAttack(true);
                return -1;
            }
            if (controller.GetButtonDown(ButtonType.HeavyAttack))
            {
                if (throwable && (playerType != PlayerType.Juggler))
                {
                    // Throw the object
                    IsPlayable = false;
                    SetAnimOnline(PlayerAnimState.Throw);
                    return -1;
                }

                StartPreparingAttack(false);
                return -1;
            }

            if (isDodging) return 3;

            /*if (TDS_InputManager.GetButtonDown(TDS_InputManager.CATCH_BUTTON))
            {
                Catch();
                return -1;
            }
            if (TDS_InputManager.GetButtonDown(TDS_InputManager.SUPER_ATTACK_BUTTON))
            {
                SuperAttack();
                return -1;
            }
            if (TDS_InputManager.GetButtonDown(TDS_InputManager.USE_OBJECT_BUTTON))
            {
                UseObject();
                return -1;
            }*/
        }

        // If dodging, return 3
        if (isDodging) return 3;

        if (controller.GetButtonDown(ButtonType.Dodge) && !IsParalyzed)
        {
            if (throwable && (playerType != PlayerType.Juggler))
            {
                DropObject();
            }

            StartDodge();
            return -1;
        }

        // If attacking, return 3
        if (isAttacking) return 3;

        if (controller.GetButton(ButtonType.Parry) && isGrounded && !IsInvulnerable)
        {
            if (throwable && (playerType != PlayerType.Juggler))
            {
                DropObject();
            }

            StartCoroutine(Parry());
            return -1;
        }

        // If everything went good, return 0
        return 0;
    }

    /// <summary>
    /// Checks the player menu-related inputs.
    /// </summary>
    public virtual void CheckMenuInputs()
    {
        // Check pause input
        if (controller.GetButtonDown(ButtonType.Pause))
        {
            if (TDS_GameManager.IsInCutscene)
            {
                // Skip it
                if (PhotonNetwork.isMasterClient)
                    TDS_LevelManager.Instance.SkipCutscene();
            }
            else
            {
                TDS_GameManager.SetPause(!TDS_GameManager.IsPaused);
            }
        }

        if (TDS_GameManager.IsInCutscene) return;

        // Check how to play related input
        if (controller.GetButtonDown(ButtonType.HowToPlay))
        {
            OnTriggerHowToPlay?.Invoke();
        }
    }

    /// <summary>
    /// Checks inputs for this player's movements.
    /// </summary>
    public virtual void CheckMovementsInputs()
    {
        // If the character is paralyzed or dodging, do not move
        if (IsParalyzed || isDodging) return;

        // Moves the player on the X & Z axis regarding the the axis pressure.
        float _horizontal = controller.GetAxis(AxisType.Horizontal);
        float _vertical = controller.GetAxis(AxisType.Vertical) * 2f;

        if (_horizontal != 0 || _vertical != 0)
        {
            // Set a minimum to X movement if not null
            if ((_horizontal != 0 ) && (Mathf.Abs(_horizontal) < MOVEMENT_MINIMUM_VALUE)) _horizontal = MOVEMENT_MINIMUM_VALUE * Mathf.Sign(_horizontal);

            // Flip the player on the X axis if needed
            if (((_horizontal > 0 && !isFacingRight) || (_horizontal < 0 && isFacingRight)) && CanFlip) Flip();

            // If attacking or parrying, do not move
            if (isAttacking || isParrying || (isPreparingAttack && (playerType != PlayerType.FireEater))) return;

            // Set a minimum to Z movement if not null
            if ((_vertical != 0) && (Mathf.Abs(_vertical) < MOVEMENT_MINIMUM_VALUE)) _vertical = MOVEMENT_MINIMUM_VALUE * Mathf.Sign(_vertical);

            MoveInDirection(transform.position + new Vector3(_horizontal, 0, _vertical));
        }
        // If stoping moving, update informations
        else if (isMoving)
        {
            isMoving = false;
            SpeedCurrent = 0;

            SetAnimOnline(PlayerAnimState.Idle);
        }

        // If attacking or parrying, do not move
        if (isAttacking || isPreparingAttack || isParrying) return;

        // When pressing the jump method, check if on ground ; If it's all good, then let's jump
        if (controller.GetButtonDown(ButtonType.Jump) && IsGrounded)
        {
            StartJump();
        }
    }
    #endregion

    #region Others
    /// <summary>
    /// Makes the player disappear before respawning.
    /// </summary>
    public void DisappearBeforeRespawn(float _xPos, float _yPos, float _zPos)
    {
        // Call this method for other clients
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "DisappearBeforeRespawn", new object[] { _xPos, _yPos, _zPos });
        }

        sprite.enabled = false;
        HealthCurrent = healthMax;

        if (photonView.isMine)
        {
            rigidbody.isKinematic = true;
            collider.enabled = false;
            IsPlayable = false;
            transform.position = new Vector3(_xPos, _yPos, _zPos);

            TDS_Camera.Instance.Target = transform;
        }
    }

    /// <summary>
    /// Makes the player respawn.
    /// </summary>
    public void RespawnPlayer()
    {
        // Call this method for other clients
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "RespawnPlayer", new object[] { });
        }

        sprite.enabled = true;

        if (photonView.isMine)
        {
            StartDodge();
            OnStopDodgeOneShot += SetPlayerPlayable;
        }
    }

    /// <summary>
    /// Set the player playable, after respawning.
    /// </summary>
    private void SetPlayerPlayable()
    {
        rigidbody.isKinematic = false;
        collider.enabled = true;

        if (PhotonNetwork.offlineMode && !sprite.isVisible)
        {
            StartMovingPlayerInView();
            return;
        }
        IsPlayable = true;
    }
    #endregion

    #endregion

    #region Photon Methods
    // Called to send and receive streams
    public void OnPhotonSerializeView(PhotonStream _stream, PhotonMessageInfo _info)
    {
        if (_stream.isWriting)
        {
            Vector4 _bounds = TDS_Camera.Instance.GetSendingBounds();

            _stream.SendNext(_bounds.x);
            _stream.SendNext(_bounds.y);
            _stream.SendNext(_bounds.z);
            _stream.SendNext(_bounds.w);
        }
        else
        {
            Vector4 _bounds = new Vector4();

            _bounds.x = (float)_stream.ReceiveNext();
            _bounds.y= (float)_stream.ReceiveNext();
            _bounds.z = (float)_stream.ReceiveNext();
            _bounds.w = (float)_stream.ReceiveNext();

            TDS_Camera.Instance.SetBoundsByOnline(_bounds);
        }
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Try to get components references if they are missing
        if (!interactionBox)
        {
            interactionBox = GetComponentInChildren<TDS_PlayerInteractionBox>();

#if UNITY_EDITOR
            if (!interactionBox) Debug.LogWarning("The Interaction Detector of \"" + name + "\" for script TDS_Player is missing !");
#endif
        }
        if(!spriteHolder)
        {
            spriteHolder = GetComponentInChildren<TDS_PlayerSpriteHolder>();
#if UNITY_EDITOR
            if (!spriteHolder) Debug.LogWarning("The Sprite Holder of \"" + name + "\" for script TDS_Player is missing !");
#endif
        }
        if (spriteHolder)
        {
            if (!spriteHolder.Owner) spriteHolder.Owner = this;
            if (!spriteHolder.PlayerSprite) spriteHolder.PlayerSprite = sprite;
        }

        // Set animation on revive
        OnDie += () => StartCoroutine(TDS_LevelManager.Instance.CheckLivingPlayers());

        // Add local player tag if it's mine, and set controller
        if (photonView.isMine)
        {
            gameObject.AddTag("Local Player");
        }
    }

    // Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations
    protected virtual void FixedUpdate()
    {
        // Checks if the player is grounded or not, and all related elements
        if (photonView.isMine && !isDead && IsPlayable && (Time.timeScale != 0))
        {
            CheckGrounded();
        }
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    private void LateUpdate()
    {
        // If dead, return
        if (isDead) return;

        // At the end of the frame, set the previous position as this one
        previousPosition = transform.position;
        previousColliderPosition = collider.bounds.center;
    }

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    protected override void OnDestroy()
    {
        TDS_LevelManager.Instance?.RemoveOnlinePlayer(this);
    }

#if UNITY_EDITOR
    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Draws the ground detection box gizmos
        groundDetectionBox.DrawGizmos(transform.position);
    }
#endif

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        if(!photonView.isMine)
        {
            TDS_LevelManager.Instance?.InitOnlinePlayer(this); 
        }

        // Initializes ground detection box X & Z size based on collider size
        groundDetectionBox.Size.x = collider.size.x - .001f;
        groundDetectionBox.Size.z = collider.size.z - .001f;

        // Set health if needed
        if (TDS_GameManager.CurrentSceneIndex != 0)
        {
            int _health = TDS_GameManager.PlayersInfo.First(p => p.PlayerType == PlayerType).Health;

            if (_health > 0) HealthCurrent = _health;
        }

        if (photonView.isMine)
        {
            if (!PhotonNetwork.offlineMode || TDS_GameManager.PlayersInfo.Count == 1)
            {
                controller = TDS_GameManager.InputsAsset.Controllers[0];
            }
            else controller = TDS_GameManager.PlayersInfo.Where(p => p.PlayerType == playerType).First().Controller;
        }

        //Initialize the player LifeBar
        TDS_UIManager.Instance?.SetPlayerLifeBar(this);
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        // If dead or not playable, return
        if (!photonView.isMine) return;

        // Check menu-related inputs
        CheckMenuInputs();

        if (isDead || (Time.timeScale == 0) || TDS_GameManager.IsInCutscene) return;

        // Adjust the position of the player for each axis of the rigidbody velocity where a force is exercised
        AdjustPositionOnRigidbody();
        
        // If not playable or down, return
        if (!IsPlayable) return;

        // Check the player inputs
        CheckMovementsInputs();
        CheckActionsInputs();
	}
#endregion

#endregion
}
