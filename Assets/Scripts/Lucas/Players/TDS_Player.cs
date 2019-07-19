using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TDS_Player : TDS_Character 
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
    /// Event called when taking an object.
    /// </summary>
    public event Action OnGrabObject = null;

    /// <summary>
    /// Event called when throwing an object.
    /// </summary>
    public event Action OnThrow = null;
    #endregion

    #region Fields / Properties

    #region Constants
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
    /// The summoner this player is currently carrying.
    /// </summary>
    public TDS_Summoner Summoner = null;

    /// <summary>
    /// <see cref="TDS_Detector"/> used to detect when possible interactions with the environment are availables.
    /// </summary>
    [SerializeField] protected TDS_Detector interactionDetector = null;

    /// <summary>
    /// Virtual box used to detect if the player is grounded or not.
    /// </summary>
    [SerializeField] protected TDS_VirtualBox groundDetectionBox = new TDS_VirtualBox();

    [SerializeField] protected TDS_PlayerSpriteHolder spriteHolder; 
    #endregion

    #region Coroutines
    /// <summary>
    /// Reference of the current coroutine of the dodge method.
    /// </summary>
    protected Coroutine dodgeCoroutine = null;

    /// <summary>
    /// References the current coroutine of the jump method. Null if none is actually running.
    /// </summary>
    protected Coroutine jumpCoroutine = null;

    /// <summary>Backing field for <see cref="PreparingAttackCoroutine"/>.</summary>
    protected Coroutine preparingAttackCoroutine = null;

    /// <summary>
    /// Reference of the coroutine used to prepare an attack.
    /// </summary>
    protected Coroutine PreparingAttackCoroutine
    {
        get { return preparingAttackCoroutine; }
        set
        {
            preparingAttackCoroutine = value;

            isPreparingAttack = value != null;
        }
    }
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
    [SerializeField] private bool isPreparingAttack = false;

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

    /// <summary>Backing field for <see cref="NextAttack"/>.</summary>
    [SerializeField] private int nextAttack = 0;

    /// <summary>
    /// Used as a buffer for the next player attack ; light attack if positive, hevay one if negative, and none if null.
    /// </summary>
    public int NextAttack
    {
        get { return nextAttack; }
        set
        {
            nextAttack = value;
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

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions

    #region Aim & Throwables
    /// <summary>
    /// Drop the weared throwable.
    /// </summary>
    public override bool DropObject()
    {
        if (!base.DropObject()) return false;

        // Updates the animator informations
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", photonView.owner, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)PlayerAnimState.LostObject });

        return true;
    }

    /// <summary>
    /// Start a coroutine to drop object if the button is maintained or to throw it.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator DropObjectCoroutine()
    {
        float _timer = DROP_OBJECT_TIME;

        while (TDS_InputManager.GetButton(TDS_InputManager.INTERACT_BUTTON))
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
        if (!throwable) yield break;

        // Throw the object
        if (isGrounded)
        {
            IsPlayable = false;
            SetAnim(PlayerAnimState.Throw);
        }
        else
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "ThrowObject_A"), new object[] { });
        }

        yield break;
    }

    /// <summary>
    /// Try to grab a throwable.
    /// When grabbed, the object follows the character and can be thrown by this one.
    /// </summary>
    /// <param name="_throwable">Throwable to try to grab.</param>
    /// <returns>Returns true if the throwable was successfully grabbed, false either.</returns>
    public override bool GrabObject(TDS_Throwable _throwable)
    {
        if (!base.GrabObject(_throwable)) return false;

        // Triggers event
        OnGrabObject?.Invoke();

        // Updates animator informations
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", photonView.owner, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)PlayerAnimState.HasObject });

        return true;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land.</param>
    public override bool ThrowObject(Vector3 _targetPosition)
    {
        if (!base.ThrowObject(_targetPosition)) return false;

        // Triggers event
        OnThrow?.Invoke();

        // Update the animator
        TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", photonView.owner, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)PlayerAnimState.LostObject });

        return true;
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Makes the player active its planned attack.
    /// </summary>
    /// <param name="_attackIndex">Index of the attack to activate from <see cref="attacks"/>.</param>
    public virtual void ActiveAttack(int _attackIndex)
    {
        #if UNITY_EDITOR
        // If index is out of range, debug it
        if ((_attackIndex < 0) || (_attackIndex >= attacks.Length))
        {
            Debug.LogWarning($"The Player \"{name}\" has no selected attack to perform");
            return;
        }
        #endif

        // Activate the hit box
        hitBox.Activate(attacks[_attackIndex]);
    }

    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public virtual void Attack(bool _isLight)
    {
        IsAttacking = true;
        OnStartAttack?.Invoke();

        if (nextAttack != 0) NextAttack = 0;

        // Adds the current combo to the list
        ComboCurrent.Add(_isLight);

        // Set animator
        if (_isLight) SetAnim(PlayerAnimState.LightAttack);
        else SetAnim(PlayerAnimState.HeavyAttack);

        #if UNITY_EDITOR
        if (comboCurrent.Count > comboMax) Debug.LogError($"Player \"{name}\" should not have a combo of {comboCurrent.Count} !");
        #endif
    }

    /// <summary>
    /// Breaks an on going combo, with animation set and stopping attack if attacking.
    /// </summary>
    public virtual void BreakCombo()
    {
        if (ComboCurrent.Count < comboMax) SetAnim(PlayerAnimState.ComboBreaker);

        ComboCurrent = new List<bool>();
        CancelInvoke("BreakCombo");

        if (IsAttacking) StopAttack();
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
            if (nextAttack != 0) StartPreparingAttack(nextAttack.ToBool());
            else if (comboCurrent.Count > 0)Invoke("BreakCombo", comboResetTime);
        }
        else
        {
            // Reset the combo when reaching its end
            BreakCombo();
        }
    }

    /// <summary>
    /// Makes the player prepare an attack.
    /// By default, the player is supposed to just directly attack ; but for certain situations, the attack might not played directly : that's the goal of this method, to be override to rewrite a pre-attack behaviour.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    protected virtual IEnumerator PrepareAttack(bool _isLight)
    {
        Attack(_isLight);

        PreparingAttackCoroutine = null;
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
    /// Resets the player planned next attack.
    /// </summary>
    public void ResetNextAttack() => NextAttack = 0;

    /// <summary>
    /// Makes the player start preparing an attack. This is the method called just before calling the <see cref="PrepareAttack(bool)"/> coroutine.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public virtual void StartPreparingAttack(bool _isLight)
    {
        if (isPreparingAttack) return;

        // If already attacking, just stock this attack as the next one
        if (isAttacking)
        {
            NextAttack = _isLight.ToSign();
            return;
        }

        CancelInvoke("BreakCombo");

        SetBonusDamages(0);
        PreparingAttackCoroutine = StartCoroutine(PrepareAttack(_isLight));
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
    /// Stops the player from preparing an attack.
    /// </summary>
    /// <returns>Returns true if successfully stopped preparing an attack, false if none was in preparation.</returns>
    public virtual bool StopPreparingAttack()
    {
        if (!isPreparingAttack) return false;

        StopCoroutine(preparingAttackCoroutine);
        return false;
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
        SetAnim(PlayerAnimState.Catch);
    }

    /// <summary>
    /// Performs a dodge.
    /// While dodging, the player cannot take damage or attack.
    /// </summary>
    protected virtual IEnumerator Dodge()
    {
        // Dodge !
        IsInvulnerable = true;
        isDodging = true;

        OnStartDodging?.Invoke();

        // Adds an little force at the start of the dodge
        rigidbody.AddForce(Vector3.right * Mathf.Clamp(speedCurrent, speedInitial, speedMax) * speedCoef * isFacingRight.ToSign() * speedMax * (isGrounded ? 10 : 2));

        // Triggers the associated animation
        SetAnim(PlayerAnimState.Dodge);

        // Adds a little force to the player to move him along while dodging
        while (true)
        {
            float _xForce = isFacingRight.ToSign() * speedCoef * speedMax * (isGrounded ? 7 : 2.5f);
            rigidbody.AddForce(new Vector3(_xForce, isGrounded ? 0 : -.35f, 0));
            MoveInDirection(transform.position + (isFacingRight ? Vector3.right : Vector3.left));

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Set the player in parry position.
    /// While parrying, the player avoid to take damages.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator Parry()
    {
        // Parry
        bool _wasInvulnerable = IsInvulnerable;

        IsInvulnerable = true;
        isParrying = true;
        
        SetAnim(PlayerAnimState.Parrying);

        OnStartParry?.Invoke();

        // While holding the parry button, parry attacks
        while (TDS_InputManager.GetButton(TDS_InputManager.PARRY_BUTTON))
        {
            yield return null;
        }

        // Stop parrying
        SetAnim(PlayerAnimState.NotParrying);
        isParrying = false;
        IsInvulnerable = _wasInvulnerable;

        OnStopParry?.Invoke();
    }

    /// <summary>
    /// Make the player dodge.
    /// </summary>
    public virtual void StartDodge() => dodgeCoroutine = StartCoroutine(Dodge());

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
        IsInvulnerable = false;
        isDodging = false;

        // Call events
        OnStopDodge?.Invoke();

        OnStopDodgeOneShot?.Invoke();
        OnStopDodgeOneShot = null;
    }

    /// <summary>
    /// Stops the automatic movement when dodging.
    /// </summary>
    public void StopDodgeMove()
    {
        if (dodgeCoroutine != null)
        {
            StopCoroutine(dodgeCoroutine);

            float _xVelocity = 0;

            if (isGrounded) _xVelocity = 0;
            else _xVelocity = rigidbody.velocity.x * .8f;

            rigidbody.velocity = new Vector3(_xVelocity, rigidbody.velocity.y, rigidbody.velocity.z);
        }
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

        // Set Animation
        SetAnim(PlayerAnimState.Sliding);

        IsPlayable = false;
        IsInvulnerable = true;

        return true;
    }

    /// <summary>
    /// Tells the Character that he's getting up.
    /// </summary>
    public override void GetUp()
    {
        base.GetUp();

        IsPlayable = true;
        IsInvulnerable = false;
    }

    /// <summary>
    /// Put the character on the ground.
    /// </summary>
    public override bool PutOnTheGround()
    {
        if (!base.PutOnTheGround()) return false;

        // Set animation
        SetAnim(PlayerAnimState.Down);

        IsPlayable = false;
        IsInvulnerable = true;

        return true;
    }

    /// <summary>
    /// Method called when stopped being bringed closer.
    /// </summary>
    protected override void StopBringingCloser()
    {
        base.StopBringingCloser();

        // Set animation
        SetAnim(PlayerAnimState.NotSliding);

        IsPlayable = true;
        IsInvulnerable = false;
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

        // Drop object if needed
        if (throwable) DropObject();

        // Triggers associated animations
        SetAnim(PlayerAnimState.Grounded);
        SetAnim(PlayerAnimState.Die);
    }

    /// <summary>
    /// Method called when this character hit something.
    /// Override it to implement feedback and other things.
    /// </summary>
    public override void HitCallback()
    {
        base.HitCallback();

        // Screen shake guy !
        TDS_Camera.Instance.StartScreenShake(comboCurrent.Count < 3 ? .01f : .012f, .125f);
    }

    /// <summary>
    /// Set invulnerability during a certain time.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Invulnerability()
    {
        IsInvulnerable = true;

        float _timer = INVULNERABILITY_TIME;
        while (_timer > 0)
        {
            yield return new WaitForSeconds(INVULNERABILITY_TIME / 7);
            _timer -= INVULNERABILITY_TIME / 7;
            sprite.gameObject.SetActive(!sprite.gameObject.activeInHierarchy);
        }

        sprite.gameObject.SetActive(true);
        IsInvulnerable = false;
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        // Executes base method
        if (!base.TakeDamage(_damage))
        {
            TDS_Camera.Instance.StartScreenShake(.01f, .05f);
            return false;
        }

        // If preparing an attack, stop it
        if (isPreparingAttack) StopPreparingAttack();

        // And if in combo, reset it
        if (comboCurrent.Count > 0) BreakCombo();

        // If not dead, be just hit
        if (!isDead)
        {
            // Triggers associated animation
            if (!IsDown) SetAnim(PlayerAnimState.Hit);

            StartCoroutine(Invulnerability());

            if (photonView.isMine)
            {
                TDS_Camera.Instance.StartScreenShake(.02f, .15f);
            }
        }
        else if (photonView.isMine)
        {
            TDS_Camera.Instance.StartScreenShake(.05f, .1f);
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
            transform.position += new Vector3(.025f * (_position.x < transform.position.x ? 1 : 1), 0, 0);

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
        GameObject _nearestObject = interactionDetector.NearestObject;

        if (!_nearestObject) return false;

        TDS_Throwable _throwable = null;

        // Interact now with the object depending on its type
        if ((_throwable = _nearestObject.GetComponent<TDS_Throwable>()) && isGrounded)
        {
            GrabObject(_throwable);
            return true;
        }

        TDS_WhiteRabbit _whiteRabbit = null;
        if (_whiteRabbit = _nearestObject.GetComponent<TDS_WhiteRabbit>())
        {
            _whiteRabbit.Use(this);
            return true;
        }

        return false;
    }
    #endregion

    #region Movements
    /// <summary>
    /// Adjusts the position of the player on the axis where a force is exercised on the rigidbody velocity.
    /// </summary>
    private void AdjustPositionOnRigidbody()
    {
        if (!photonView.isMine) return; 
        // If the player rigidbody velocity is null, return
        if (rigidbody.velocity == Vector3.zero) return;

        // Get all touching colliders ; if none, return
        Collider[] _touchedColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

        if (_touchedColliders.Length == 0) return;

        // For each axis where the player rigidbody velocity is non null, adjust the player position if it is in another collider
        // To do this, use the previous position and overlap from this in the actual position for each axis where the velocity is not null

        Vector3 _newPosition = transform.position;
        Vector3 _newVelocity = rigidbody.velocity;
        Vector3 _movementVector = transform.position - previousPosition;
        Vector3 _colliderCenter = Vector3.Scale(collider.center, collider.transform.lossyScale);
        Vector3 _colliderWorldPosition = collider.bounds.center;
        Vector3 _colliderExtents = collider.bounds.extents - (Vector3.one * .0001f);
        Vector3 _overlapCenter = Vector3.zero;
        Vector3 _overlapExtents = Vector3.one;

        // X axis adjustment
        if (rigidbody.velocity.x != 0)
        {
            // Get the extents & center position for the overlap
            _overlapExtents = new Vector3(Mathf.Abs(_movementVector.x) / 2, _colliderExtents.y, _colliderExtents.z);

            _overlapCenter = new Vector3(previousColliderPosition.x + ((_colliderExtents.x + _overlapExtents.x) * Mathf.Sign(rigidbody.velocity.x)), previousColliderPosition.y, previousColliderPosition.z);

            // Overlap in the zone where the player would be from the previous position after the movement on the X axis.
            // If something is touched, then adjust the position of the player against it
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            if (_touchedColliders.Length > 0)
            {
                //Debug.Log("Get back in X");

                float _xLimit = 0;

                // Get the X position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.x > 0)
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x - c.bounds.extents.x).OrderBy(c => c).First();

                    _newPosition.x = _xLimit - (_colliderExtents.x - _colliderCenter.x) - .001f;
                }
                else
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x + c.bounds.extents.x).OrderBy(c => c).Last();

                    _newPosition.x = _xLimit + (_colliderExtents.x - _colliderCenter.x) + .001f;
                }

                _movementVector.x = _newPosition.x - previousPosition.x;

                // Reset the X velocity
                _newVelocity.x = 0;
            }
        }

        // Y axis adjustment
        if (rigidbody.velocity.y != 0)
        {
            // Get the extents & center position for the overlap
            _overlapExtents = new Vector3(_colliderExtents.x, Mathf.Abs(_movementVector.y) / 2, _colliderExtents.z);

            _overlapCenter = new Vector3(previousColliderPosition.x, previousColliderPosition.y + ((_colliderExtents.y + _overlapExtents.y) * Mathf.Sign(rigidbody.velocity.y)), previousColliderPosition.z);

            // Overlap in the zone where the player would be from the previous position after the movement on the Y axis.
            // If something is touched, then adjust the position of the player against it
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            if (_touchedColliders.Length > 0)
            {
                //Debug.Log("Get back in Y");

                float _yLimit = 0;

                // Get the Y position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.y > 0)
                {
                    _yLimit = _touchedColliders.Select(c => c.bounds.center.y - c.bounds.extents.y).OrderBy(c => c).First();

                    _newPosition.y = _yLimit - (_colliderExtents.y - _colliderCenter.y) - .001f;
                }
                else
                {
                    _yLimit = _touchedColliders.Select(c => c.bounds.center.y + c.bounds.extents.y).OrderBy(c => c).Last();

                    _newPosition.y = _yLimit + (_colliderExtents.y - _colliderCenter.y) + .001f;
                }

                _movementVector.y = _newPosition.y - previousPosition.y;

                // Reset the Y velocity
                _newVelocity.y = 0;
            }
        }

        // Z axis adjustment
        if (rigidbody.velocity.z != 0)
        {
            // Get the extents & center position for the overlap
            _overlapExtents = new Vector3(_colliderExtents.x, _colliderExtents.y, Mathf.Abs(_movementVector.z) / 2);

            _overlapCenter = new Vector3(previousColliderPosition.x, previousColliderPosition.y, previousColliderPosition.z + ((_colliderExtents.z + _overlapExtents.z) * Mathf.Sign(rigidbody.velocity.z)));

            // Overlap in the zone where the player would be from the previous position after the movement on the Z axis.
            // If something is touched, then adjust the position of the player against it
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            if (_touchedColliders.Length > 0)
            {
                //Debug.Log("Get back in Z");

                float _zLimit = 0;

                // Get the Z position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.z > 0)
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z - c.bounds.extents.z).OrderBy(c => c).First();

                    _newPosition.z = _zLimit - (_colliderExtents.z - _colliderCenter.z) - .001f;
                }
                else
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z + c.bounds.extents.z).OrderBy(c => c).Last();

                    _newPosition.z = _zLimit + (_colliderExtents.z - _colliderCenter.z) + .001f;
                }

                _movementVector.z = _newPosition.z - previousPosition.z;

                // Reset the Z velocity
                _newVelocity.z = 0;
            }
        }

        // Set the position of the player as the new calculated one, and reset the velocity for the recalculated axis
        transform.position = _newPosition;
        rigidbody.velocity = _newVelocity;
    }

    /// <summary>
    /// Checks if the player is grounded and updates related elements and parameters.
    /// </summary>
    private void CheckGrounded()
    {
        // Set the player as grounded if something is detected in the ground detection box
        bool _isGrounded = groundDetectionBox.Overlap(transform.position).Length > 0;

        // If grounded value changed, updates all necessary things
        if (_isGrounded != IsGrounded)
        {
            // Updates value
            IsGrounded = _isGrounded;

            // Updates the ground state information in the animator

            if (!_isGrounded)
            {
                speedCoef = .7f;

                // Activates event
                OnGetOffGround?.Invoke();
            }
            else
            {
                speedCoef = 1;
                rigidbody.velocity = Vector3.zero;

                // Activates event
                OnGetOnGround?.Invoke();
            }
        }

        // Updates animator grounded informations
        if (!_isGrounded && !isDodging && !isAttacking)
        {
            if (rigidbody.velocity.y < 0) SetAnim(PlayerAnimState.Falling);
            else SetAnim(PlayerAnimState.Jumping);
        }
        else
        {
            SetAnim(PlayerAnimState.Grounded);
        }
    }
    
    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        base.Flip();
        if (photonView.isMine) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Flip"), new object[] { });
        // Flip X throw velocity
        throwVelocity.x *= -1;
    }

    /// <summary>
    /// Freezes the player's movements and actions.
    /// </summary>
    public void FreezePlayer() => IsPlayable = false;

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

        while(TDS_InputManager.GetButton(TDS_InputManager.JUMP_BUTTON) && _timer < JumpMaximumTime)
        {
            rigidbody.AddForce(Vector3.up * (JumpForce / JumpMaximumTime) * Time.deltaTime);
            yield return new WaitForFixedUpdate();

            _timer += Time.fixedDeltaTime;
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

    /// <summary>
    /// Move directly the player to a new position.
    /// </summary>
    /// <param name="_newPosition">New position to move to.</param>
    public void MoveTo(Vector3 _newPosition)
    {
        // For X & Z axis, overlap in the zone between this position and the future one ; priority order is X, & Z.
        // If something is touched, use the bounds of the collider to set the position of the player against the obstacle.

        Vector3 _movementVector = _newPosition - transform.position;
        Vector3 _colliderCenter = Vector3.Scale(collider.center, collider.transform.lossyScale);
        Vector3 _colliderWorldPosition = collider.bounds.center;
        Vector3 _colliderExtents = collider.bounds.extents - (Vector3.one * .0001f);
        Vector3 _overlapCenter = Vector3.zero;
        Vector3 _overlapExtents = Vector3.one;
        Collider[] _touchedColliders = new Collider[] { };

        // X axis movement test
        if (_movementVector.x != 0)
        {
            // Get the extents & center positon for the overlap
            _overlapExtents = new Vector3(Mathf.Abs(_movementVector.x) / 2, _colliderExtents.y, _colliderExtents.z);

            _overlapCenter = new Vector3(_colliderWorldPosition.x + ((_colliderExtents.x + _overlapExtents.x) * Mathf.Sign(_movementVector.x)), _colliderWorldPosition.y, _colliderWorldPosition.z);

            // Overlaps in the position where the player would be after the X movement ;
            // If nothing is touched, then the player can move in X
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            // If the player cannot move in X, set its position against the nearest collider
            if (_touchedColliders.Length > 0)
            {
                //Debug.Log("Back in X !");

                float _xLimit = 0;

                // Get the X position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.x > 0)
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x - c.bounds.extents.x).OrderBy(c => c).First();

                    _newPosition.x = _xLimit - (_colliderExtents.x + _colliderCenter.x) - .001f;
                }
                else
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x + c.bounds.extents.x).OrderBy(c => c).Last();

                    _newPosition.x = _xLimit + (_colliderExtents.x + _colliderCenter.x) + .001f;
                }

                _movementVector.x = _newPosition.x - transform.position.x;
            }
        }

        // Z axis movement test
        if (_movementVector.z != 0)
        {
            // Get the extents & center positon for the overlap ;
            // If the player can move in X, overlap from this new X position
            _overlapExtents = new Vector3(_colliderExtents.x, _colliderExtents.y, Mathf.Abs(_movementVector.z) / 2);

            _overlapCenter = new Vector3(_colliderWorldPosition.x + _movementVector.x, _colliderWorldPosition.y, _colliderWorldPosition.z + ((_colliderExtents.z + _overlapExtents.z) * Mathf.Sign(_movementVector.z)));

            // Overlaps in the position where the player would be after the Z movement ;
            // If nothing is touched, then the player can move in Z
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            // If the player cannot move in Z, set its position against the nearest collider
            if (_touchedColliders.Length > 0)
            {
                //Debug.Log("Back in Z !");

                float _zLimit = 0;

                // Get the Z position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.z > 0)
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z - c.bounds.extents.z).OrderBy(c => c).First();

                    _newPosition.z = _zLimit - (_colliderExtents.z + _colliderCenter.z) - .001f;
                }
                else
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z + c.bounds.extents.z).OrderBy(c => c).Last();

                    _newPosition.z = _zLimit + (_colliderExtents.z + _colliderCenter.z) + .001f;
                }
            }
        }

        // Move the player
        if (transform.position != _newPosition)
        {
            transform.position = _newPosition;

            // If starting moving, update informations
            if (!isMoving)
            {
                isMoving = true;
                SetAnim(PlayerAnimState.Run);
            }
        }
        else if (isMoving)
        {
            isMoving = false;
            SetAnim(PlayerAnimState.Idle);
        }
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
    /// Unfreezes the player's movements and actions.
    /// </summary>
    public void UnfreezePlayer() => IsPlayable = true;
    #endregion

    #region Animations
    /// <summary>
    /// Set this player animator informations.
    /// </summary>
    /// <param name="_state">State of the player animator to set.</param>
    public void SetAnim(PlayerAnimState _state)
    {
        // Online
        if (photonView.isMine)
        {
            // if (!animator) return;
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)_state });
        }

        // Local
        switch (_state)
        {
            case PlayerAnimState.Idle:
                animator.SetBool("IsMoving", false);
                break;

            case PlayerAnimState.Run:
                animator.SetBool("IsMoving", true);
                break;

            case PlayerAnimState.Hit:
                animator.SetTrigger("Hit");
                break;

            case PlayerAnimState.Die:
                animator.SetTrigger("Die");
                break;

            case PlayerAnimState.Dodge:
                animator.SetTrigger("Dodge");
                break;

            case PlayerAnimState.Throw:
                animator.SetTrigger("Throw");
                break;

            case PlayerAnimState.Catch:
                // Nothing for now
                break;

            case PlayerAnimState.LightAttack:
                animator.SetTrigger("LightAttack");
                break;

            case PlayerAnimState.HeavyAttack:
                animator.SetTrigger("HeavyAttack");
                break;

            case PlayerAnimState.ComboBreaker:
                animator.SetTrigger("ComboBreaker");
                break;

            case PlayerAnimState.Super:
                // Nothing for now
                break;

            case PlayerAnimState.Grounded:
                animator.SetInteger("GroundState", 0);
                break;

            case PlayerAnimState.Jumping:
                animator.SetInteger("GroundState", 1);
                break;

            case PlayerAnimState.Falling:
                animator.SetInteger("GroundState", -1);
                break;

            case PlayerAnimState.HasObject:
                animator.SetBool("HasObject", true);
                break;

            case PlayerAnimState.LostObject:
                animator.SetBool("HasObject", false);
                break;

            case PlayerAnimState.Parrying:
                animator.SetBool("IsParrying", true);
                break;

            case PlayerAnimState.NotParrying:
                animator.SetBool("IsParrying", false);
                break;

            case PlayerAnimState.Sliding:
                animator.SetBool("IsSliding", true);
                break;

            case PlayerAnimState.NotSliding:
                animator.SetBool("IsSliding", false);
                break;

            case PlayerAnimState.Down:
                animator.SetTrigger("Down");
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
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    /// <returns>Returns an int indicating at which step the method returned :
    /// 0 if everything went good ;
    /// A negative number if an action has been performed ;
    /// 1 if dodging, parrying or preparing an attack ;
    /// 2 if having a throwable ;
    /// and 3 if attacking.</returns>
    public virtual int CheckActionsInputs()
    {
        // If dodging, parrying or attacking, do not perform action, and return 1
        if (isDodging || isParrying || isPreparingAttack) return 1;

        // Check non-agressive actions
        if (TDS_InputManager.GetButtonDown(TDS_InputManager.INTERACT_BUTTON) && !isAttacking)
        {
            Interact();
            return -1;
        }

        // If having a throwable, return 2
        if (throwable && (playerType != PlayerType.Juggler)) return 2;

        // Checks potentially agressives actions
        if (!IsPacific && isGrounded)
        {
            if (TDS_InputManager.GetButtonDown(TDS_InputManager.LIGHT_ATTACK_BUTTON))
            {
                StartPreparingAttack(true);
                return -1;
            }
            if (TDS_InputManager.GetButtonDown(TDS_InputManager.HEAVY_ATTACK_BUTTON))
            {
                StartPreparingAttack(false);
                return -1;
            }
            if (TDS_InputManager.GetButtonDown(TDS_InputManager.CATCH_BUTTON))
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
            }
        }

        // If attacking, return 3
        if (isAttacking) return 3;

        if (TDS_InputManager.GetButtonDown(TDS_InputManager.DODGE_BUTTON) && !IsParalyzed)
        {
            StartDodge();
            return -1;
        }
        if (TDS_InputManager.GetButtonDown(TDS_InputManager.PARRY_BUTTON) && isGrounded)
        {
            StartCoroutine(Parry());
            return -1;
        }

        // If everything went good, return 0
        return 0;
    }

    /// <summary>
    /// Checks inputs for this player's movements.
    /// </summary>
    public virtual void CheckMovementsInputs()
    {
        // If the character is paralyzed or attacking, do not move
        if (IsParalyzed || isAttacking || isParrying || isDodging) return;

        // Moves the player on the X & Z axis regarding the the axis pressure.
        float _horizontal = Input.GetAxis(TDS_InputManager.HORIZONTAL_AXIS);
        float _vertical = Input.GetAxis(TDS_InputManager.VERTICAL_AXIS) * 2f;

        if (_horizontal != 0 || _vertical != 0)
        {
            // Set a minimum to movement if not null
            if ((_horizontal != 0 ) && (Mathf.Abs(_horizontal) < MOVEMENT_MINIMUM_VALUE)) _horizontal = MOVEMENT_MINIMUM_VALUE * Mathf.Sign(_horizontal);
            if ((_vertical != 0) && (Mathf.Abs(_vertical) < MOVEMENT_MINIMUM_VALUE)) _vertical = MOVEMENT_MINIMUM_VALUE * Mathf.Sign(_vertical);

            // Flip the player on the X axis if needed
            if ((_horizontal > 0 && !isFacingRight) || (_horizontal < 0 && isFacingRight)) Flip();

            MoveInDirection(transform.position + new Vector3(_horizontal, 0, _vertical));
        }
        // If stoping moving, update informations
        else if (isMoving)
        {
            isMoving = false;
            SpeedCurrent = 0;

            SetAnim(PlayerAnimState.Idle);
        }

        // When pressing the jump method, check if on ground ; If it's all good, then let's jump
        if (TDS_InputManager.GetButtonDown(TDS_InputManager.JUMP_BUTTON) && IsGrounded && !throwable && !isPreparingAttack)
        {
            StartJump();
        }
    }
    #endregion

    #region Others
    /// <summary>
    /// Activate or desactivate the player.
    /// </summary>
    /// <param name="_doActive">Should it be activated or desactivated ?</param>
    public void ActivePlayer(bool _doActive)
    {
        rigidbody.isKinematic = !_doActive;
        collider.enabled = _doActive;
        enabled = _doActive;
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Try to get components references if they are missing
        if (!interactionDetector)
        {
            interactionDetector = GetComponentInChildren<TDS_Detector>();
            if (!interactionDetector) Debug.LogWarning("The Interaction Detector of \"" + name + "\" for script TDS_Player is missing !");
        }

        if(!spriteHolder)
        {
            spriteHolder = GetComponentInChildren<TDS_PlayerSpriteHolder>();
            if (!spriteHolder) Debug.LogWarning("The Sprite Holder of \"" + name + "\" for script TDS_Player is missing !");
        }
        if(spriteHolder)
        {
            if (!spriteHolder.Owner) spriteHolder.Owner = this;
            if (!spriteHolder.PlayerSprite) spriteHolder.PlayerSprite = sprite;
        }
        // Set animation on revive
        OnRevive += () => animator.SetTrigger("REVIVE");
        OnDie += TDS_LevelManager.Instance.CheckLivingPlayers; 
    }

    // Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations
    protected virtual void FixedUpdate()
    {
        // If dead, return
        if (!photonView.isMine || isDead || !PhotonNetwork.connected) return;

        // Checks if the player is grounded or not, and all related elements
        if (IsPlayable) CheckGrounded();
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    private void LateUpdate()
    {
        // If dead, return
        if (isDead || !PhotonNetwork.connected) return;

        // At the end of the frame, set the previous position as this one
        previousPosition = transform.position;
        previousColliderPosition = collider.bounds.center;
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Draws the ground detection box gizmos
        groundDetectionBox.DrawGizmos(transform.position);

        // Draws a gizmos at the aiming point in editor
        Vector3 _gizmosPos = throwAimingPoint;
        _gizmosPos.x *= isFacingRight.ToSign();
        _gizmosPos += transform.position;

        Gizmos.DrawIcon(_gizmosPos, "AimIcon", true);

        if (handsTransform)
        {
            // Draws a gizmos at the hands transform ideal position
            Gizmos.DrawSphere(handsTransform.position, .07f);
            Gizmos.DrawIcon(handsTransform.position, "HandIcon", true);
        }
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        if(!photonView.isMine)
        {
            TDS_LevelManager.Instance?.InitOnlinePlayer(this); 
        }
        else
        {
            if (TDS_UIManager.Instance.ComboManager)
                hitBox.OnTouch += TDS_UIManager.Instance.ComboManager.IncreaseCombo; 
        }

        // Since all players except the Juggler cannot change their throw angle & the point they are aiming,
        // get the throw velocity & projectile motion in local space at start time
        throwVelocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(handsTransform.localPosition, throwAimingPoint, aimAngle);

        // Initializes ground detection box X & Z size based on collider size
        groundDetectionBox.Size.x = collider.size.x - .001f;
        groundDetectionBox.Size.z = collider.size.z - .001f;

        //Initialize the player LifeBar
        TDS_UIManager.Instance?.SetPlayerLifeBar(this);
    }

    // Update is called once per frame
    protected override void Update ()
    {
        // If dead or not playable, return
        if (!photonView.isMine || isDead) return;

        base.Update();

        // Adjust the position of the player for each axis of the rigidbody velocity where a force is exercised
        AdjustPositionOnRigidbody();

        // If not playable or down, return
        if (!IsPlayable) return;

        // Check the player inputs
        CheckMovementsInputs();
        CheckActionsInputs();
	}

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    protected virtual void OnDestroy()
    {
        TDS_LevelManager.Instance?.RemoveOnlinePlayer(this); 
    }
    #endregion

    #endregion
}
