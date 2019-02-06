using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
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
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Moved the throwAimingPoint field ; and the aimAngle field & property to the TDS_Character class.
     *	    - Added the lineRenderer, ParryButton fields, throwVelocity & throwTrajectoryMotionPoints fields ; the ThrowAimingPoint property ; and the throwPreviewPosition field & property.
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

    #region Enums
    protected enum PlayerAnimations
    {
        Idle,               //      =>      0
        Run,                //      =>      1
        Jump,               //      =>      2
        Fall,               //      =>      3
        Hit,                //      =>      4
        Die,                //      =>      5
        Dodge,              //      =>      6
        Catch,              //      =>      7
        Throw,              //      =>      8
        Parade  ,           //      =>      9
        IdleWithObject,     //      =>      10
        RunWithObject       //      =>      11
    }
    #endregion

    #region Events

    #endregion

    #region Fields / Properties

    #region Components & References
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
    [SerializeField] protected TDS_Trigger interactionsDetector = null;

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
    /// Name of the button used to cancel a throw.
    /// </summary>
    public string CancelThrowButton = "Cancel Throw";

    /// <summary>
    /// Name of the button used to dodge.
    /// </summary>
    public string DodgeButton = "Dodge";

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
    private Coroutine aimCoroutine = null;

    /// <summary>
    /// References the current coroutine of the jump method. Null if none is actually running.
    /// </summary>
    protected Coroutine jumpCoroutine = null;
    #endregion

    #region Variables
    /// <summary>Backing field for <see cref="Attacks"/></summary>
    [SerializeField] protected List<TDS_Attack> attacks = new List<TDS_Attack>();

    /// <summary>
    /// All attacks this player can perform.
    /// Contains informations about their animation, damages, effect and others.
    /// </summary>
    public List<TDS_Attack> Attacks
    {
        get { return attacks; }
        protected set { attacks = value; }
    }

    /// <summary>Backing field for <see cref="IsAiming"/>.</summary>
    [SerializeField] private bool isAiming = false;

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

    /// <summary>Backing field for <see cref="IsGrounded"/></summary>
    [SerializeField] private bool isGrounded = true;

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
    [SerializeField] private bool isJumping = false;

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
    [SerializeField] private float jumpMaximumTime = 1.5f;

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
                throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(Vector3.zero, throwAimingPoint, throwVelocity.magnitude, aimAngle, value);
            }
            #endif
        }
    }

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
    /// Property for <see cref="ThrowAimingPoint"/> to update <see cref="throwVelocity"/> && <see cref="throwTrajectoryMotionPoints"/> on changes.
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
                throwVelocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(Vector3.zero, value, aimAngle);

                throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(Vector3.zero, value, throwVelocity.magnitude, aimAngle, throwPreviewPrecision);
            }
            #endif
        }
    }
    #endregion

    #region Debug & Script memory Variables
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
        while (Input.GetButton(ThrowButton))
        {
            // Raycast along the trajectory preview and stop the trail when hit something
            RaycastHit _hit = new RaycastHit();
            Vector3[] _raycastedMotionPoints = throwTrajectoryMotionPoints;

            for (int _i = 0; _i < _raycastedMotionPoints.Length - 1; _i++)
            {
                // Get the points to raycast from & to in world space
                Vector3 _from = transform.position + new Vector3(throwTrajectoryMotionPoints[_i].x * isFacingRight.ToSign(), throwTrajectoryMotionPoints[_i].y, throwTrajectoryMotionPoints[_i].z);
                Vector3 _to = transform.position + new Vector3(throwTrajectoryMotionPoints[_i + 1].x * isFacingRight.ToSign(), throwTrajectoryMotionPoints[_i + 1].y, throwTrajectoryMotionPoints[_i + 1].z);

                // If hit something, set the hit point as end of the preview trajectory
                if (Physics.Linecast(_from, _to, out _hit, WhatIsObstacle))
                {
                    _raycastedMotionPoints = new Vector3[_i + 2];
                    for (int _j = 0; _j <= _i; _j++)
                    {
                        _raycastedMotionPoints[_j] = throwTrajectoryMotionPoints[_j];
                    }
                    // Get the hit point as absolute value in local space ; so as distance
                    _raycastedMotionPoints[_i + 1] = new Vector3(Mathf.Abs(_hit.point.x - transform.position.x), _hit.point.y - transform.position.y, Mathf.Abs(_hit.point.z - transform.position.z));

                    // Adjust the sign of the hit point Vector3 according to the one of the aiming point

                    /*if (Mathf.Sign(_raycastedMotionPoints[_i + 1].x) != Mathf.Sign(throwAimingPoint.x)) _raycastedMotionPoints[_i + 1].x *= -1;
                    if (Mathf.Sign(_raycastedMotionPoints[_i + 1].y) != Mathf.Sign(throwAimingPoint.y)) _raycastedMotionPoints[_i + 1].y *= -1;
                    if (Mathf.Sign(_raycastedMotionPoints[_i + 1].z) != Mathf.Sign(throwAimingPoint.z)) _raycastedMotionPoints[_i + 1].z *= -1;*/

                    break;
                }
            }

            // Draws the trajectory preview
            lineRenderer.DrawTrajectory(_raycastedMotionPoints);

            yield return new WaitForSeconds(.05f);
        }

        // Throws the object to the aiming position
        ThrowObject();

        StopAiming();

        yield break;
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
        return true;
    }

    /// <summary>
    /// Prepare a throw, if not already preparing one.
    /// </summary>
    /// <returns>Returns true if successfully prepared a throw ; false if one is already, or if cannot do this.</returns>
    public virtual bool PrepareThrow()
    {
        //if (isAiming || !Throwable) return false;
    
        isAiming = true;
        aimCoroutine = StartCoroutine(Aim());
        return true;
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public virtual void Attack(bool _isLight)
    {
        // If already attacking, return
        if (IsAttacking) return;

        CancelInvoke("ResetCombo");

        // Attack !
        IsAttacking = true;

        switch (comboCurrent.Count)
        {
            default:
                break;
        }
        ComboCurrent.Add(_isLight);

        if (comboCurrent.Count < comboMax)
        {
            Invoke("StopAttack", .5f);
            Invoke("ResetCombo", comboResetTime);
        }

        else
        {
            Invoke("ResetCombo", .5f);
        }

        Debug.Log("Attack " + (_isLight ? "Light" : "Heavy") + " | Combo => " + comboCurrent.Count + " / " + comboMax);
    }

    /// <summary>
    /// Resets the current combo.
    /// </summary>
    public virtual void ResetCombo()
    {
        ComboCurrent = new List<bool>();
        IsAttacking = false;

        Debug.Log("Reset Combo");
    }

    /// <summary>
    /// Stops this player's current attack if attacking
    /// </summary>
    public override void StopAttack()
    {
        if (!IsAttacking) return;

        // Stop it, please
        base.StopAttack();
    }
    #endregion

    /// <summary>
    /// Performs the catch attack of this player.
    /// </summary>
    public virtual void Catch()
    {
        // Catch
    }

    /// <summary>
    /// Performs a dodge.
    /// While dodging, the player cannot take damage or attack.
    /// </summary>
    public virtual void Dodge()
    {
        // Dodge !

        IsInvulnerable = true;
    }

    /// <summary>
    /// Set the player in a parade position.
    /// While parrying, the player avoid to take damages in front of him.
    /// </summary>
    public virtual void Parry()
    {
        // Parry
    }

    /// <summary>
    /// Stops the current dodge if dodging.
    /// </summary>
    public virtual void StopDodge()
    {
        // Stop dodging
        IsInvulnerable = false;
    }

    /// <summary>
    /// Performs the Super attack if the gauge is filled enough.
    /// </summary>
    public virtual void SuperAttack()
    {
        // SUPER attack
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void UseObject()
    {
        // Use
    }
    #endregion

    #region Heal
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected override void Die()
    {
        base.Die();
    }

    /// <summary>
    /// Makes this object be healed and restore its health.
    /// </summary>
    /// <param name="_heal">Amount of health point to restore.</param>
    public override void Heal(int _heal)
    {
        base.Heal(_heal);
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        bool _isTakingDamage = base.TakeDamage(_damage);
        if (!_isTakingDamage) return false;

        // Is aiming, cancel the preparing throw
        if (isAiming) StopAiming();

        return true;
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    public virtual void CheckActionsInputs()
    {
        // Check non-agressive actions
        if (Input.GetButtonDown(DodgeButton)) Dodge();

        else if (Input.GetButtonDown(InteractButton)) Interact();

        // If the character is pacific, forbid him to attack
        if (IsPacific) return;

        // Checks potentially agressives actions
        if (Input.GetButtonDown(CatchButton)) Catch();

        else if (Input.GetButtonDown(ThrowButton)) PrepareThrow();

        else if (Input.GetButtonDown(CancelThrowButton)) StopAiming();

        else if (Input.GetButtonDown(LightAttackButton)) Attack(true);

        else if (Input.GetButtonDown(HeavyAttackButton)) Attack(false);

        else if (Input.GetButtonDown(SuperAttackButton)) SuperAttack();

        else if (Input.GetButtonDown(UseObjectButton)) UseObject();
    }

    /// <summary>
    /// Checks inputs for this player's movements.
    /// </summary>
    public virtual void CheckMovementsInputs()
    {
        // If the character is paralyzed, do not move
        if (IsParalyzed) return;

        // Moves the player on the X & Z axis regarding the the axis pressure.
        float _horizontal = Input.GetAxis(HorizontalAxis);
        float _vertical = Input.GetAxis(VerticalAxis);

        if (_horizontal != 0 || _vertical != 0)
        {
            // Flip the player on the X axis if needed
            if ((_horizontal > 0 && !isFacingRight) || (_horizontal < 0 && isFacingRight)) Flip();

            Move(new Vector3(_horizontal, 0, _vertical), true);
        }
        else SpeedCurrent = 0;

        // When pressing the jump method, check if on ground ; If it's all good, then let's jump
        if (Input.GetButtonDown(JumpButton) && IsGrounded)
        {
            // If there is already a jump coroutine running, stop it before starting the new one
            if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);

            jumpCoroutine = StartCoroutine(Jump());
        }
    }
    #endregion

    #region Interactions
    /// <summary>
    /// Interacts with the nearest available object in range.
    /// </summary>
    /// <returns>Returns true if interacted with something. False if nothing was found.</returns>
    public virtual bool Interact()
    {
        // Interact

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
        Vector3 _colliderWorldPosition = collider.bounds.center;
        Vector3 _colliderExtents = collider.bounds.extents - (Vector3.one * .0001f);
        Vector3 _overlapCenter = Vector3.zero;
        Vector3 _overlapExtents = Vector3.one;

        // X axis adjustment
        if (rigidbody.velocity.x != 0)
        {
            // Get the extents & center position for the overlap
            _overlapExtents = new Vector3(Mathf.Abs(_movementVector.x) / 2, _colliderExtents.y, _colliderExtents.z);

            _overlapCenter = new Vector3(previousPosition.x + ((_colliderExtents.x + _overlapExtents.x) * Mathf.Sign(rigidbody.velocity.x)), previousPosition.y, previousPosition.z);

            // Overlap in the zone where the player would be from the previous position after the movement on the Y axis.
            // If something is touched, then adjust the position of the player against it
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            if (_touchedColliders.Length > 0)
            {
                Debug.Log("Get back in X");

                float _xLimit = 0;

                // Get the Y position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.x > 0)
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x - c.bounds.extents.x).OrderBy(c => c).First();

                    _newPosition.x = _xLimit - _colliderExtents.x - collider.center.x - .0001f;
                }
                else
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x + c.bounds.extents.x).OrderBy(c => c).Last();

                    _newPosition.x = _xLimit + _colliderExtents.x + collider.center.x + .0001f;
                }

                _movementVector.x = _newPosition.x - previousPosition.x;

                // Reset the Y velocity
                _newVelocity.x = 0;
            }
        }

        // Y axis adjustment
        if (rigidbody.velocity.y != 0)
        {
            // Get the extents & center position for the overlap
            _overlapExtents = new Vector3(_colliderExtents.x, Mathf.Abs(_movementVector.y) / 2, _colliderExtents.z);

            _overlapCenter = new Vector3(previousPosition.x, previousPosition.y + ((_colliderExtents.y + _overlapExtents.y) * Mathf.Sign(rigidbody.velocity.y)), previousPosition.z);

            // Overlap in the zone where the player would be from the previous position after the movement on the Y axis.
            // If something is touched, then adjust the position of the player against it
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            if (_touchedColliders.Length > 0)
            {
                Debug.Log("Get back in Y");

                float _yLimit = 0;

                // Get the Y position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.y > 0)
                {
                    _yLimit = _touchedColliders.Select(c => c.bounds.center.y - c.bounds.extents.y).OrderBy(c => c).First();

                    _newPosition.y = _yLimit - _colliderExtents.y - collider.center.y - .0001f;
                }
                else
                {
                    _yLimit = _touchedColliders.Select(c => c.bounds.center.y + c.bounds.extents.y).OrderBy(c => c).Last();

                    _newPosition.y = _yLimit + _colliderExtents.y + collider.center.y + .0001f;
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

            _overlapCenter = new Vector3(previousPosition.x, previousPosition.y, previousPosition.z + ((_colliderExtents.z + _overlapExtents.z) * Mathf.Sign(rigidbody.velocity.z)));

            // Overlap in the zone where the player would be from the previous position after the movement on the Y axis.
            // If something is touched, then adjust the position of the player against it
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            if (_touchedColliders.Length > 0)
            {
                Debug.Log("Get back in Z");

                float _zLimit = 0;

                // Get the Y position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.z > 0)
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z - c.bounds.extents.z).OrderBy(c => c).First();

                    _newPosition.z = _zLimit - _colliderExtents.z - collider.center.z - .0001f;
                }
                else
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z + c.bounds.extents.z).OrderBy(c => c).Last();

                    _newPosition.z = _zLimit + _colliderExtents.z + collider.center.z + .0001f;
                }

                _movementVector.z = _newPosition.z - previousPosition.z;

                // Reset the Y velocity
                _newVelocity.z = 0;
            }
        }

        // Set the position of the player as the new calculated one, and reset the velocity for the recalculated axis
        transform.position = _newPosition;
        rigidbody.velocity = _newVelocity;
    }

    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        base.Flip();

        // Also flip the velocity used to throw objects
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
    /// <param name="_newPosition">Position where to move the player.</param>
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
        Vector3 _colliderWorldPosition = collider.bounds.center;
        Vector3 _colliderExtents = collider.bounds.extents - (Vector3.one * .0001f);
        Vector3 _overlapCenter = Vector3.zero;
        Vector3 _overlapExtents = Vector3.one;
        Collider[] _touchedColliders = new Collider[] { };
        bool _canMoveInX = false;

        // X axis movement test
        if (_movementVector.x != 0)
        {
            // Get the extents & center positon for the overlap
            _overlapExtents = new Vector3(Mathf.Abs(_movementVector.x) / 2, _colliderExtents.y, _colliderExtents.z);

            _overlapCenter = new Vector3(_colliderWorldPosition.x + ((_colliderExtents.x + _overlapExtents.x) * Mathf.Sign(_movementVector.x)), _colliderWorldPosition.y, _colliderWorldPosition.z);

            // Overlaps in the position where the player would be after the X movement ;
            // If nothing is touched, then the player can move in X
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            _canMoveInX = _touchedColliders.Length == 0;

            // If the player cannot move in X, set its position against the nearest collider
            if (!_canMoveInX)
            {
                float _xLimit = 0;

                // Get the X position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.x > 0)
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x - c.bounds.extents.x).OrderBy(c => c).First();

                    _newPosition.x = _xLimit - _colliderExtents.x - collider.center.x - .001f;
                }
                else
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x + c.bounds.extents.x).OrderBy(c => c).Last();

                    _newPosition.x = _xLimit + _colliderExtents.x + collider.center.x + .001f;
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
                float _zLimit = 0;

                // Get the Z position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.z > 0)
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z - c.bounds.extents.z).OrderBy(c => c).First();

                    _newPosition.z = _zLimit - _colliderExtents.z - collider.center.z - .001f;
                }
                else
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z + c.bounds.extents.z).OrderBy(c => c).Last();

                    _newPosition.z = _zLimit + _colliderExtents.z + collider.center.z + .001f;
                }
            }
        }

        // Move the player
        transform.position = _newPosition;
    }

    /// <summary>
    /// Moves the player in a direction according to a position.
    /// </summary>
    /// <param name="_newPosition">Position where to move the player.</param>
    /// <param name="_isDirection">Is the given position a direction regarding to this player position ? False mean it's a position in world space. Defaults to false.</param>
    public void Move(Vector3 _newPosition, bool _isDirection)
    {
        // Transforms the given vector in a world position if it's a direction
        if (_isDirection) _newPosition += transform.position;

        Move(_newPosition);
    }
    #endregion

    #region Others
    /// <summary>
    /// Draws the preview trajectory of the player throw, when aiming.
    /// </summary>
    private void DrawPreviewTrajectory()
    {

    }

    /// <summary>
    /// Set this object animator to a new state.
    /// </summary>
    /// <param name="_state">ID of the new animator state. It is recommanded to use an animator enum value converted as int.</param>
    public void SetAnimatorState(int _state)
    {
        animator.SetInteger("State", _state);
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
            lineRenderer = GetComponent<LineRenderer>();
            if (!lineRenderer) Debug.LogWarning("The LineRenderer of \"" + name + "\" for script TDS_Player is missing !");
        }
    }

    // Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations
    protected virtual void FixedUpdate()
    {
        // Set the player as grounded if something is detected in the ground detection box
        IsGrounded = groundDetectionBox.Overlap(transform.position).Length > 0;
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    private void LateUpdate()
    {
        // At the end of the frame, set the previous position as this one
        previousPosition = transform.position;
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        // Draws the ground detection box gizmos
        groundDetectionBox.DrawGizmos(transform.position);
    }

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        // Since all players except the Juggler cannot change their throw angle & the point they are aiming,
        // get the throw velocity & projectile motion in local space at start time
        throwVelocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(Vector3.zero, throwAimingPoint, aimAngle);

        throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(Vector3.zero, throwAimingPoint, throwVelocity.magnitude, aimAngle, throwPreviewPrecision);
    }
	
	// Update is called once per frame
	protected override void Update ()
    {
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
