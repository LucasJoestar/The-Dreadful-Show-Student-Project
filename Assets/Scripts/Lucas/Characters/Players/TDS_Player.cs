using System;
using System.Collections;
using System.Collections.Generic;
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
     *  Date :			[17 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the CatchButton, DodgeButton, HeavyAttackButton, HorizontalAxis, InteractButton, JumpButton, LightAttackButton, SuperAttackButton, ThrowButton, VerticalAxis, IsJumping, JumpForce, JumpMaximumTime & jumpCoroutine fields.
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
    /// <see cref="TDS_Trigger"/> used to detect when the player touches the ground or not.
    /// </summary>
    [SerializeField] protected TDS_Trigger groundDetector = null;

    /// <summary>
    /// <see cref="TDS_Trigger"/> used to detect when possible interactions with the environment are available.
    /// </summary>
    [SerializeField] protected TDS_Trigger interactionsDetector = null;
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
    /// Starts a jump.
    /// Jump higher while maintaining the jump button.
    /// When releasing the button, stop adding force to the jump.
    /// <see cref="JumpMaximumTime"/> determines the maximum time of a jump.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Jump()
    {

        yield return null;
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

        // Raycast current collider without overlap --> Create a method for this
        // Is something is touched, return ; the rigidbody will eject the player
        // Next, raycast in the zone between this position and the future one ; priority order is X, Z then Y axis.
        // If something is touched, use the raycast hit point to set the position against the obstacle.

        // Move the player

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

        // Subscribes to the linked TDS_Trigger classes events
        if (groundDetector)
        {
            groundDetector.OnNothingDetected += () => IsGrounded = false;
            groundDetector.OnSomethingDetected += () => IsGrounded = true;
        }
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
	}
	#endregion

	#endregion
}
