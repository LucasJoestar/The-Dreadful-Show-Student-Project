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
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[22 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    
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

    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// The summoner this player is currently carrying.
    /// </summary>
    public TDS_Summoner Summoner = null;

    /// <summary>
    /// <see cref="TDS_Trigger"/> used to detect when possible interactions with the environment are available.
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
    /// References the current coroutine of the jump method. Null if none is actually running.
    /// </summary>
    protected Coroutine jumpCoroutine = null;
    #endregion

    #region Variables
    /// <summary>
    /// Is the player touching the ground ?
    /// If true, jump is enabled.
    /// </summary>
    public bool IsGrounded { get; protected set; } = true;

    /// <summary>
    /// Is the player actually performing a jump ?
    /// </summary>
    public bool IsJumping { get; protected set; } = false;

    /// <summary>Backing field for <see cref="ComboCurrent"/></summary>
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

    /// <summary>Backing field for <see cref="ComboMax"/></summary>
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
        }
    }

    /// <summary>Backing field for <see cref="ComboResetTime"/></summary>
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
        }
    }

    /// <summary>
    /// Force given to the rigidbody velocity in Y of this player to perform a jump.
    /// </summary>
    public float JumpForce = 1;

    /// <summary>
    /// Maximum time length of a jump.
    /// </summary>
    public float JumpMaximumTime = 1.5f;

    /// <summary>
    /// LayerMask used to detect what is an obstacle for the player movements.
    /// </summary>
    public LayerMask WhatIsObstacle = new LayerMask();

    /// <summary>
    /// What character type this player is ?
    /// </summary>
    public PlayerType PlayerType { get; protected set; } = PlayerType.Unknown;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions
    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public virtual void Attack(bool _isLight)
    {
        // Attack !

        IsAttacking = true;
    }

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
    /// Resets the current combo.
    /// </summary>
    public virtual void ResetCombo()
    {
        ComboCurrent = new List<bool>();
        IsAttacking = false;
    }

    /// <summary>
    /// Stops this player's current attack if attacking
    /// </summary>
    public virtual void StopAttack()
    {
        if (!IsAttacking) return;

        // Stop it, please
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
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    public virtual void CheckActionsInputs()
    {
        // Check
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

        if (_horizontal != 0 || _vertical != 0) Move(new Vector3(_horizontal, 0, _vertical), true);

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

        // For each axis where the player rigidbody velocity is non null, adjust the player position if it is in another collider

        Collider[] _touchedColliders = new Collider[] { }; 

        // X axis adjustment
        if (rigidbody.velocity.x != 0)
        {

        }

        // Y axis adjustment
        if (rigidbody.velocity.y != 0)
        {
            _touchedColliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            if (_touchedColliders.Length > 0)
            {

            }
        }

        // Z axis adjustment
        if (rigidbody.velocity.z != 0)
        {

        }
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

        // Adds a base vertical force to the rigidbody to expels the player in the air
        rigidbody.AddForce(Vector3.up * JumpForce);

        while(Input.GetButton(JumpButton) && _timer < JumpMaximumTime)
        {
            rigidbody.AddForce(Vector3.up * (JumpForce / JumpMaximumTime) * Time.deltaTime);
            yield return null;

            _timer += Time.deltaTime;
        }
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
            if (speedCurrent == 0) SpeedCurrent = speedInitial;
            else
            {
                SpeedCurrent += Time.deltaTime * ((speedMax - speedInitial) / speedAccelerationTime);
            }
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

            _overlapCenter = new Vector3(_colliderWorldPosition.x + ((_colliderExtents.x + _overlapExtents.x) * (_movementVector.x > 0 ? 1 : -1)), _colliderWorldPosition.y, _colliderWorldPosition.z);

            // Overlaps in the position where the player would be after the X movement ;
            // If nothing is touched, then the player can move in X
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            _canMoveInX = _touchedColliders.Length == 0;

            // If the player cannot move in X, set its position against the nearest collider
            if (!_canMoveInX)
            {
                Debug.Log("Oh no -X");
                float _xLimit = 0;

                // Get the X position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.x > 0)
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x - c.bounds.extents.x).OrderBy(c => c).First();

                    _newPosition.x = _xLimit - _colliderExtents.x - collider.center.x - .001f;
                }
                else
                {
                    _xLimit = _touchedColliders.Select(c => c.bounds.center.x + c.bounds.extents.x).OrderBy(c => c).First();

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

            _overlapCenter = new Vector3(_colliderWorldPosition.x + _movementVector.x, _colliderWorldPosition.y, _colliderWorldPosition.z + ((_colliderExtents.z + _overlapExtents.z) * (_movementVector.z > 0 ? 1 : -1)));

            // Overlaps in the position where the player would be after the Z movement ;
            // If nothing is touched, then the player can move in Z
            _touchedColliders = Physics.OverlapBox(_overlapCenter, _overlapExtents, Quaternion.identity, WhatIsObstacle, QueryTriggerInteraction.Ignore);

            // If the player cannot move in Z, set its position against the nearest collider
            if (_touchedColliders.Length > 0)
            {
                Debug.Log("Oh no -Z");
                float _zLimit = 0;

                // Get the Z position of the nearest collider limit, and set the position of the player against it
                if (_movementVector.z > 0)
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z - c.bounds.extents.z).OrderBy(c => c).First();

                    _newPosition.z = _zLimit - _colliderExtents.z - collider.center.z - .001f;
                }
                else
                {
                    _zLimit = _touchedColliders.Select(c => c.bounds.center.z + c.bounds.extents.z).OrderBy(c => c).First();

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

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
    }

    // Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations
    protected virtual void FixedUpdate()
    {
        // Set the player as grounded if something is detected in the ground detection box
        IsGrounded = groundDetectionBox.Overlap(transform.position).Length > 0;
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
