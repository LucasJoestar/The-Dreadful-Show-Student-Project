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
    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Zone at the end of the projectile preview, for feedback value.
    /// </summary>
    public GameObject ProjectilePreviewEndZone = null;

    /// <summary>
    /// Line renderer used to draw a preview for the preparing throw trajectory.
    /// </summary>
    [SerializeField] protected LineRenderer lineRenderer = null;

    /// <summary>
    /// The summoner this player is currently carrying.
    /// </summary>
    public TDS_Summoner Summoner = null;

    /// <summary>
    /// <see cref="TDS_Trigger"/> used to detect when possible interactions with the environment are availables.
    /// </summary>
    [SerializeField] protected TDS_Trigger interactionDetector = null;

    /// <summary>
    /// Virtual box used to detect if the player is grounded or not.
    /// </summary>
    [SerializeField] protected TDS_VirtualBox groundDetectionBox = new TDS_VirtualBox();
    #endregion

    #region Inputs
    /// <summary>
    /// Name of the button used to perform a catch.
    /// </summary>
    public string CatchButton = "Catch";

    /// <summary>
    /// Name of the button used to dodge.
    /// </summary>
    public string DodgeButton = "Dodge";

    /// <summary>
    /// Name of the joystick D-Pad X axis.
    /// </summary>
    public string DPadXAxis = "D-Pad X";

    /// <summary>
    /// Name of the joystick D-Pad Y axis.
    /// </summary>
    public string DPadYAxis = "D-Pad Y";

    /// <summary>
    /// Name of the button used to perform a heavy attack.
    /// </summary>
    public string HeavyAttackButton = "Heavy Attack";

    /// <summary>
    /// Name of the axis used to move on the X axis.
    /// </summary>
    public string HorizontalAxis = "Horizontal";

    /// <summary>
    /// Name of the button used to interact with the environment.
    /// </summary>
    public string InteractButton = "Interact";

    /// <summary>
    /// Name of the button used to jump.
    /// </summary>
    public string JumpButton = "Jump";

    /// <summary>
    /// Name of the button used to perform a light attack.
    /// </summary>
    public string LightAttackButton = "Light Attack";

    /// <summary>
    /// Name of the button used to parry.
    /// </summary>
    public string ParryButton = "Parry";

    /// <summary>
    /// Name of the joystick right stick X axis.
    /// </summary>
    public string RightStickXAxis = "Right Stick X";

    /// <summary>
    /// Name of the joystick right stick Y axis.
    /// </summary>
    public string RightStickYAxis = "Right Stick Y";

    /// <summary>
    ///Name of the button used to perform the super attack.
    /// </summary>
    public string SuperAttackButton = "Super Attack";

    /// <summary>
    /// Name of the button used to throw an object.
    /// </summary>
    public string ThrowButton = "Throw";

    /// <summary>
    /// Name of the button used to use the selected object.
    /// </summary>
    public string UseObjectButton = "Use Object";

    /// <summary>
    /// Name of the axis used to move on the Z axis.
    /// </summary>
    public string VerticalAxis = "Vertical";
    #endregion

    #region Coroutines
    /// <summary>
    /// Reference of the current coroutine of the aim method.
    /// </summary>
    protected Coroutine aimCoroutine = null;

    /// <summary>
    /// Reference of the current coroutine of the dodge method.
    /// </summary>
    protected Coroutine dodgeCoroutine = null;

    /// <summary>
    /// References the current coroutine of the jump method. Null if none is actually running.
    /// </summary>
    protected Coroutine jumpCoroutine = null;
    #endregion

    #region Variables
    /// <summary>
    /// The actually performing attack.
    /// </summary>
    [SerializeField] protected TDS_Attack currentAttack = null;

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

    /// <summary>Backing field for <see cref="IsAiming"/>.</summary>
    [SerializeField] protected bool isAiming = false;

    /// <summary>
    /// Indicates if the player is currently aiming or not.
    /// </summary>
    public bool IsAiming
    {
        get { return isAiming; }
        protected set
        {
            isAiming = value;
        }
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

    /// <summary>Backing field for <see cref="ThrowPreviewPrecision"/>.</summary>
    [SerializeField] protected int throwPreviewPrecision = 10;

    /// <summary>
    /// Amount of point used to draw the throw preview trajectory.
    /// </summary>
    public int ThrowPreviewPrecision
    {
        get { return throwPreviewPrecision; }
        set
        {
            if (value < 1) value = 1;
            throwPreviewPrecision = value;

            #if UNITY_EDITOR
            // Updates the trajectory preview
            if (UnityEditor.EditorApplication.isPlaying)
            {
                throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(handsTransform.localPosition, throwAimingPoint, throwVelocity.magnitude, aimAngle, value);
            }
            #endif
        }
    }

    /// <summary>
    /// Layer mask referencing everything except this player layer.
    /// </summary>
    [SerializeField] protected LayerMask whatIsAllButThis = new LayerMask();

    /// <summary>
    /// LayerMask used to detect what is an obstacle for the player movements.
    /// </summary>
    public LayerMask WhatIsObstacle = new LayerMask();

    /// <summary>Backing field for <see cref="PlayerType"/></summary>
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

    /// <summary>
    /// Property for <see cref="throwAimingPoint"/> to update <see cref="throwVelocity"/> && <see cref="throwTrajectoryMotionPoints"/> on changes.
    /// </summary>
    public Vector3 ThrowAimingPoint
    {
        get { return throwAimingPoint; }
        set
        {
            throwAimingPoint = value;

            #if UNITY_EDITOR
            // Updates the velocity & trajectory preview
            if (UnityEditor.EditorApplication.isPlaying)
            {
                throwVelocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(handsTransform.localPosition, value, aimAngle);

                throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(handsTransform.localPosition, value, throwVelocity.magnitude, aimAngle, throwPreviewPrecision);
            }
            #endif
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

    /// <summary>
    /// Points used to draw a preview of the projectile trajectory when preparing a throw (Local space).
    /// </summary>
    protected Vector3[] throwTrajectoryMotionPoints = new Vector3[] { };
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions

    #region Aim & Throwables
    /// <summary>
    /// Makes the character aim for a throw. When releasing the throw button, throw the selected object.
    /// If the cancel throw button is pressed, cancel the throw, as it name indicate it.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Aim()
    {
        // While holding the throw button, aim a position
        while (Input.GetButton(ThrowButton) || TDS_Input.GetAxis(ThrowButton))
        {
            // Draws the preview of the projectile trajectory while holding the throw button
            AimMethod();

            yield return null;

            if (Input.GetButtonDown(ParryButton) || TDS_Input.GetAxisDown(ParryButton))
            {
                // Throws the object to the aiming position
                ThrowObject();

                if (!throwable) break;
            }
        }

        StopAiming();

        yield break;
    }

    /// <summary>
    /// Method called in the Aim coroutine.
    /// </summary>
    protected virtual void AimMethod()
    {
        // Raycast along the trajectory preview and stop the trail when hit something
        RaycastHit _hit = new RaycastHit();
        Vector3[] _raycastedMotionPoints = (Vector3[])throwTrajectoryMotionPoints.Clone();
        Vector3 _endPoint = new Vector3();
        bool _hasHit = false;

        for (int _i = 0; _i < _raycastedMotionPoints.Length - 1; _i++)
        {
            // Get the points to raycast from & to in world space
            Vector3 _from = transform.position + new Vector3(_raycastedMotionPoints[_i].x * isFacingRight.ToSign(), _raycastedMotionPoints[_i].y, _raycastedMotionPoints[_i].z);

            Vector3 _to = transform.position + new Vector3(_raycastedMotionPoints[_i + 1].x * isFacingRight.ToSign(), _raycastedMotionPoints[_i + 1].y, _raycastedMotionPoints[_i + 1].z);

            // If hit something, set the hit point as end of the preview trajectory
            if (Physics.Linecast(_from, _to, out _hit, whatIsAllButThis, QueryTriggerInteraction.Ignore))
            {
                // Get the hit point in local space
                _endPoint = transform.InverseTransformPoint(_hit.point);
                _endPoint.z *= isFacingRight.ToSign();

                // Get the throw preview motion points with the new hit point
                _raycastedMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(handsTransform.localPosition, _endPoint, throwVelocity.magnitude, aimAngle, throwPreviewPrecision);

                // Updates the position of the end preview zone & its rotation according to the hit point
                ProjectilePreviewEndZone.transform.position = _hit.point;

                Quaternion _rotation = Quaternion.Lerp(ProjectilePreviewEndZone.transform.rotation, Quaternion.FromToRotation(Vector3.up, _hit.normal), Time.deltaTime * 15);

                ProjectilePreviewEndZone.transform.rotation = _rotation;

                // Set indicative boolean
                _hasHit = true;

                break;
            }
        }
        
        // If no touch, update end zone position & rotation
        if (!_hasHit)
        {
            // Updates the position of the end preview zone & its rotation according to the hit point
            ProjectilePreviewEndZone.transform.position = new Vector3(transform.position.x + (throwAimingPoint.x * isFacingRight.ToSign()), transform.position.y + throwAimingPoint.y, transform.position.z + throwAimingPoint.z);

            Quaternion _rotation = Quaternion.Lerp(ProjectilePreviewEndZone.transform.rotation, Quaternion.FromToRotation(Vector3.up, Vector3.up), Time.deltaTime * 15);
            _rotation.x *= isFacingRight.ToSign();

            ProjectilePreviewEndZone.transform.rotation = _rotation;

            // Set end point
            _endPoint = throwAimingPoint;
        }

        // Draws the trajectory preview
        for (int _i = 0; _i < _raycastedMotionPoints.Length; _i++)
        {
            _raycastedMotionPoints[_i].z *= isFacingRight.ToSign();
        }

        lineRenderer.DrawTrajectory(_raycastedMotionPoints);
    }

    /// <summary>
    /// Drop the weared throwable.
    /// </summary>
    public override void DropObject()
    {
        base.DropObject();

        // Updates the animator informations
        SetAnim(PlayerAnimState.LostObject);
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

        // Updates animator informations
        SetAnim(PlayerAnimState.HasObject);
        return true;
    }

    /// <summary>
    /// Prepare a throw, if not already preparing one.
    /// </summary>
    /// <returns>Returns true if successfully prepared a throw ; false if one is already, or if cannot do this.</returns>
    public virtual bool PrepareThrow()
    {
        if (isAiming || !throwable) return false;

        isAiming = true;
        aimCoroutine = StartCoroutine(Aim());

        ProjectilePreviewEndZone.SetActive(true);

        return true;
    }

    /// <summary>
    /// Stops the preparing throw, if preparing one.
    /// </summary>
    /// <returns>Returns true if canceled the throw, false if there was nothing to cancel.</returns>
    public virtual bool StopAiming()
    {
        if (!isAiming && aimCoroutine == null) return false;

        if (isAiming) isAiming = false;
        if (aimCoroutine != null)
        {
            StopCoroutine(aimCoroutine);
        }

        lineRenderer.DrawTrajectory(new Vector3[0]);
        ProjectilePreviewEndZone.SetActive(false);

        return true;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    public override void ThrowObject()
    {
        base.ThrowObject();

        // Triggers the throw animation ;
        // Update the animator
        if (isGrounded) SetAnim(PlayerAnimState.Throw);
        SetAnim(PlayerAnimState.LostObject);
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land.</param>
    public override void ThrowObject(Vector3 _targetPosition)
    {
        base.ThrowObject(_targetPosition);

        // Triggers the throw animation ;
        // Update the animator
        if (isGrounded) SetAnim(PlayerAnimState.Throw);
        SetAnim(PlayerAnimState.LostObject);
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Makes the player active its planned attack.
    /// </summary>
    public virtual void ActiveAttack()
    {
        // If not currently having an attack to perform, return
        if (currentAttack == null)
        {
            Debug.LogWarning($"The Player \"{name}\" has no selected attack to perform");
            return;
        }

        // If aiming, stop
        if (isAiming) StopAiming();

        // Activate the hit box
        hitBox.Activate(currentAttack);
    }

    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public virtual void Attack(bool _isLight)
    {
        IsAttacking = true;
        OnStartAttack?.Invoke();

        CancelInvoke("ResetCombo");

        // Adds the current combo to the list
        ComboCurrent.Add(_isLight);

        // If haven't yet reached the end of the combo, plan to reset it in X seconds if  not attacking before
        if (comboCurrent.Count < comboMax)
        {
            Invoke("ResetCombo", comboResetTime);
        }
    }

    /// <summary>
    /// Ends definitively the current attack and enables back the capacity to attack once more.
    /// </summary>
    protected virtual void EndAttack()
    {
        IsAttacking = false;
        OnStopAttack?.Invoke();

        // Reset the combo when reaching its end
        if (comboCurrent.Count == comboMax) ResetCombo();
    }

    /// <summary>
    /// Resets the current combo.
    /// </summary>
    public virtual void ResetCombo()
    {
        ComboCurrent = new List<bool>();

        if (IsAttacking) StopAttack();

        SetAnim(PlayerAnimState.ComboBreaker);
    }

    /// <summary>
    /// Stops this player's current attack if attacking
    /// </summary>
    public override void StopAttack()
    {
        currentAttack = null;

        // Stop it, please
        hitBox.Desactivate();

        Invoke("EndAttack", .1f);
    }

    /// <summary>
    /// Performs the Super attack if the gauge is filled enough.
    /// </summary>
    public virtual void SuperAttack()
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // SUPER attack
        Debug.Log("Super Attack !!");
    }
    #endregion

    #region Actions
    /// <summary>
    /// Performs the catch attack of this player.
    /// </summary>
    /// <param name="_minion">Minion to try to catch</param>
    public virtual void Catch(/*TDS_Minion _minion*/)
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // Catch

        // Triggers the associated animation
        SetAnim(PlayerAnimState.Catch);
    }

    /// <summary>
    /// Performs a dodge.
    /// While dodging, the player cannot take damage or attack.
    /// </summary>
    public virtual IEnumerator Dodge()
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // Dodge !
        IsInvulnerable = true;
        isDodging = true;

        OnStartDodging?.Invoke();

        // Adds an little force at the start of the dodge
        rigidbody.AddForce(Vector3.right * Mathf.Clamp(speedCurrent, speedInitial, speedMax) * speedCoef * isFacingRight.ToSign() * speedMax * 10);

        // Triggers the associated animation
        SetAnim(PlayerAnimState.Dodge);

        // Adds a little force to the player to move him along while dodging
        while (true)
        {
            rigidbody.AddForce(Vector3.right * isFacingRight.ToSign() * speedCoef * speedMax * 4);
            Move(transform.position + (isFacingRight ? Vector3.right : Vector3.left));

            yield return new WaitForEndOfFrame();
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
        isParrying = true;
        SetAnim(PlayerAnimState.Parrying);

        OnStartParry?.Invoke();

        // While holding the parry button, parry attacks
        while (Input.GetButton(ParryButton) || TDS_Input.GetAxis(ParryButton))
        {
            yield return null;
        }

        // Stop parrying
        SetAnim(PlayerAnimState.NotParrying);
        isParrying = false;

        OnStopParry?.Invoke();
    }

    /// <summary>
    /// Stops the current dodge if dodging.
    /// </summary>
    public virtual void StopDodge()
    {
        if (!isDodging) return;

        // If dodge coroutine still active, disable it
        if (dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);

        // Stop dodging
        IsInvulnerable = false;
        isDodging = false;

        OnStopDodge?.Invoke();
    }

    /// <summary>
    /// Stops the automatic movement when dodging.
    /// </summary>
    public void StopDodgeMove()
    {
        if (dodgeCoroutine != null) StopCoroutine(dodgeCoroutine);
    }

    /// <summary>
    /// Use the selected object in the inventory.
    /// </summary>
    public virtual void UseObject()
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // Use
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

        // Triggers associated animation
        SetAnim(PlayerAnimState.Die);
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        // If parrying, do not take damage
        if (isParrying) return false;

        // Executes base method
        if (!base.TakeDamage(_damage)) return false;

        // Is aiming, cancel the preparing throw
        // And if in combo, reset it
        if (isAiming) StopAiming();
        if (comboCurrent.Count > 0) ResetCombo();

        // If not dead, be just hit
        if (!isDead)
        {
            // Triggers associated animation
            SetAnim(PlayerAnimState.Hit);
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
        // If parrying, do not take damage
        if (isParrying) return false;

        // Executes base method
        if (!base.TakeDamage(_damage, _position)) return false;

        // Is aiming, cancel the preparing throw
        // And if in combo, reset it
        if (isAiming) StopAiming();
        if (comboCurrent.Count > 0) ResetCombo();

        // If not dead, be just hit
        if (!isDead)
        {
            // Triggers associated animation
            SetAnim(PlayerAnimState.Hit);
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
        // Get the nearest object in range ; if null, cannot interact, so return false
        GameObject _nearestObject = interactionDetector.NearestObject;

        if (!_nearestObject) return false;

        TDS_Throwable _throwable = null;

        // Interact now with the object depending on its type
        if (_throwable = _nearestObject.GetComponent<TDS_Throwable>())
        {
            GrabObject(_throwable);

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

                SetAnim(PlayerAnimState.Grounded);

                // Activates event
                OnGetOnGround?.Invoke();
            }
        }

        // Updates animator grounded informations
        if (!_isGrounded && !isDodging)
        {
            if (rigidbody.velocity.y < 0) SetAnim(PlayerAnimState.Falling);
            else SetAnim(PlayerAnimState.Jumping);

            // If were attacking, stop the attack
            if (isAttacking) StopAttack();
        }
    }
    
    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        base.Flip();

        // Flip X throw velocity
        throwVelocity.x *= -1;
    }

    /// <summary>
    /// Starts a jump.
    /// Jump higher while maintaining the jump button.
    /// When releasing the button, stop adding force to the jump.
    /// <see cref="JumpMaximumTime"/> determines the maximum time of a jump.
    /// </summary>
    /// <returns>Returns the world.</returns>
    public IEnumerator Jump()
    {
        // Gives a little force to the player's jump
        rigidbody.AddForce(Vector3.right * isFacingRight.ToSign() * speedCurrent * 5);

        // Creates a float to use as timer
        float _timer = 0;

        isJumping = true;

        // Adds a base vertical force to the rigidbody to expels the player in the air
        rigidbody.AddForce(Vector3.up * JumpForce);

        while(Input.GetButton(JumpButton) && _timer < JumpMaximumTime)
        {
            rigidbody.AddForce(Vector3.up * (JumpForce / JumpMaximumTime) * Time.deltaTime);
            yield return null;

            _timer += Time.deltaTime;
        }

        isJumping = false;
    }

    /// <summary>
    /// Moves the player in a direction according to a position.
    /// </summary>
    /// <param name="_newPosition">Position where to move the player. (World space)</param>
    public void Move(Vector3 _newPosition)
    {
        // Increases speed if needed
        if (speedCurrent < SpeedMax)
        {
            IncreaseSpeed();
        }

        float _speed = speedCurrent * speedCoef;

        // Adjust future position by checking possible collisions
        _newPosition = Vector3.Lerp(transform.position, _newPosition, Time.deltaTime * _speed);

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
    #endregion

    #region Animator
    /// <summary>
    /// Set this player animator informations.
    /// </summary>
    /// <param name="_state">State of the player animator to set.</param>
    public void SetAnim(PlayerAnimState _state)
    {
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

            default:
                break;
        }
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    public virtual void CheckActionsInputs()
    {
        // If dodging, parrying or attacking, do not perform action
        if (isAttacking || isDodging || isParrying) return;

        // Check non-agressive actions
        if (Input.GetButtonDown(InteractButton)) Interact();

        else if (Input.GetButtonDown(DodgeButton)) dodgeCoroutine = StartCoroutine(Dodge());

        else if ((Input.GetButtonDown(ParryButton) || TDS_Input.GetAxisDown(ParryButton)) && isGrounded && !isAiming) StartCoroutine(Parry());

        // If the character is pacific, forbid him to attack
        if (IsPacific) return;

        // Checks potentially agressives actions
        if (Input.GetButtonDown(ThrowButton) || TDS_Input.GetAxisDown(ThrowButton)) PrepareThrow();

        // If not on ground, return
        if (!isGrounded) return;

        if (Input.GetButtonDown(CatchButton)) Catch();

        else if (Input.GetButtonDown(LightAttackButton)) Attack(true);

        else if (Input.GetButtonDown(HeavyAttackButton)) Attack(false);

        else if (Input.GetButtonDown(SuperAttackButton) || (TDS_Input.GetAxisDown(SuperAttackButton) && (Input.GetAxis(SuperAttackButton) == 0))) SuperAttack();

        else if (Input.GetButtonDown(UseObjectButton)) UseObject();
    }

    /// <summary>
    /// Checks inputs for this player's movements.
    /// </summary>
    public virtual void CheckMovementsInputs()
    {
        // If the character is paralyzed or attacking, do not move
        if (IsParalyzed || isAttacking || isParrying || isDodging) return;

        // Moves the player on the X & Z axis regarding the the axis pressure.
        float _horizontal = Input.GetAxis(HorizontalAxis);
        float _vertical = Input.GetAxis(VerticalAxis) * .75f;

        if (_horizontal != 0 || _vertical != 0)
        {
            // Flip the player on the X axis if needed
            if ((_horizontal > 0 && !isFacingRight) || (_horizontal < 0 && isFacingRight)) Flip();

            Move(transform.position + new Vector3(_horizontal, 0, _vertical));
        }
        // If stoping moving, update informations
        else if (isMoving)
        {
            isMoving = false;
            SpeedCurrent = 0;

            SetAnim(PlayerAnimState.Idle);
        }

        // When pressing the jump method, check if on ground ; If it's all good, then let's jump
        if (Input.GetButtonDown(JumpButton) && IsGrounded)
        {
            // If there is already a jump coroutine running, stop it before starting the new one
            if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);

            jumpCoroutine = StartCoroutine(Jump());
        }
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Try to get components references if they are missing
        if (!lineRenderer)
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            if (!lineRenderer) Debug.LogWarning("The LineRenderer of \"" + name + "\" for script TDS_Player is missing !");
        }
        if (!interactionDetector)
        {
            interactionDetector = GetComponentInChildren<TDS_Trigger>();
            if (!interactionDetector) Debug.LogWarning("The Interaction Detector of \"" + name + "\" for script TDS_Player is missing !");
        }
    }

    // Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations
    protected virtual void FixedUpdate()
    {
        // If dead, return
        if (isDead) return;

        // Checks if the player is grounded or not, and all related elements
        CheckGrounded();
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

        // Draws a gizmos at the hands transform ideal position
        Gizmos.DrawSphere(handsTransform.position, .07f);
        Gizmos.DrawIcon(handsTransform.position, "HandIcon", true);
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        // Since all players except the Juggler cannot change their throw angle & the point they are aiming,
        // get the throw velocity & projectile motion in local space at start time
        throwVelocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(handsTransform.localPosition, throwAimingPoint, aimAngle);

        throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(handsTransform.localPosition, throwAimingPoint, throwVelocity.magnitude, aimAngle, throwPreviewPrecision);

        // Get layer for everything except this player one
        whatIsAllButThis = -1;
        whatIsAllButThis = ~(1 << gameObject.layer);

        // Initializes ground detection box X & Z size based on collider size
        groundDetectionBox.Size.x = collider.size.x;
        groundDetectionBox.Size.z = collider.size.z;
    }
	
	// Update is called once per frame
	protected override void Update ()
    {
        // If dead, return
        if (isDead) return;

        base.Update();

        // Adjust the position of the player for each axis of the rigidbody velocity where a force is exercised
        AdjustPositionOnRigidbody();

        // Check the player inputs
        CheckMovementsInputs();
        CheckActionsInputs();
	}
	#endregion

	#endregion
}
