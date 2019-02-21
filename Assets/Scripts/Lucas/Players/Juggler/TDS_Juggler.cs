using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TDS_Juggler : TDS_Player 
{
    /* TDS_Juggler :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Script of the Juggler controller. Now, you can play him. Yes you can.
     *	There should be only one juggler in by scene.
     *	    Just add this script to an object to make it behaviour as the Juggler,
     *	and be able to play with him.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[21 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    - Added the UpdateJuggleParameters method.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[20 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the JuggleSpeed field & property.
     *	    - Added the Juggle method.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[19 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the CurrentThrowableAmount & SelectedThrowableIndex properties ; and the ThrowableDistanceFromCenter & MaxThrowableAmount fields & properties.
     *	    - Replaced the SelectedThrowable property by an override of the Throwable property.
     *	    - Added the Juggle method.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[07 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added the SetAnimHeavyAttack & SetAnimLightAttack methods.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[04 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Juggler class.
     *	
     *	    - Added the selectedThrowableIndex & Throwables fields ; added the SelectedThrowable property.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// The actual selected throwable. It is the object to throw, when throwing... Yep.
    /// </summary>
    public new TDS_Throwable Throwable
    {
        get { return throwable; }
        set
        {
            // Try first to grab the object if not having it in hands
            if (!Throwables.Contains(value))
            {
                GrabObject(throwable);
                return;
            }

            throwable = value;
            selectedThrowableIndex = Throwables.IndexOf(value);
        }
    }

    /// <summary>
    /// All throwables the juggler has in hands.
    /// </summary>
    public List<TDS_Throwable> Throwables = new List<TDS_Throwable>();
    #endregion

    #region Variables
    /// <summary>Backing field for <see cref="JuggleSpeed"/>.</summary>
    [SerializeField] private float juggleSpeed = 1;

    /// <summary>
    /// Speed used by the juggler to juggle.
    /// </summary>
    public float JuggleSpeed
    {
        get { return juggleSpeed; }
        set
        {
            if (value < 0) value = 0;
            juggleSpeed = value;
        }
    }

    /// <summary>Backing field for <see cref="ThrowableDistanceFromCenter"/>.</summary>
    [SerializeField] private float throwableDistanceFromCenter = 1;

    /// <summary>
    /// Distance of all throwables juggling with from the <see cref="TDS_Character.handsTransform"/> point following a circle around it.
    /// </summary>
    public float ThrowableDistanceFromCenter
    {
        get { return throwableDistanceFromCenter; }
        set
        {
            if (value < 0) value = 0;
            throwableDistanceFromCenter = value;
        }
    }

    /// <summary>
    /// Current amount of throwable the Juggler has in hands.
    /// </summary>
    public int CurrentThrowableAmount
    {
        get { return Throwables.Count; }
    }

    /// <summary>Backing field for <see cref="MaxThrowableAmount"/>.</summary>
    [SerializeField] private int maxThrowableAmount = 5;

    /// <summary>
    /// Maximum amount of throwable this Juggler can carry on.
    /// </summary>
    public int MaxThrowableAmount
    {
        get { return maxThrowableAmount; }
        set
        {
            if (value < 1) value = 1;
            maxThrowableAmount = value;
        }
    }

    /// <summary>Backing field for <see cref="SelectedThrowableIndex"/>.</summary>
    [SerializeField] private int selectedThrowableIndex = 0;

    /// <summary>
    /// Index of the current selected throwable from <see cref="Throwables"/>.
    /// </summary>
    public int SelectedThrowableIndex
    {
        get { return selectedThrowableIndex; }
        set
        {
            if (Throwables.Count == 0)
            {
                value = 0;
                throwable = null;
            }
            else
            {
                // Clamps the index on the list range
                value = Mathf.Clamp(value, 0, Throwables.Count - 1);

                // Set the actual throwable
                throwable = Throwables[value];
            }

            selectedThrowableIndex = value;
        }
    }
    #endregion

    #region Debugs & Memory variables
    /// <summary>
    /// Counter helping to position the objects juggling with.
    /// </summary>
    private float jugglerCounter = 0;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions

    #region Aim & Throwables
    /// <summary>
    /// Update aim point on flip.
    /// </summary>
    protected override void AimFlip()
    {
        // Reset x & z position, we don't want to move them when juggling
        throwAimingPoint.z *= -1;
        ThrowAimingPoint = throwAimingPoint;
    }

    /// <summary>
    /// Method called in the Aim coroutine.
    /// </summary>
    protected override void AimMethod()
    {
        // Let the player aim the point he wants, 'cause the juggler can do that. Yep

        // Aim with IJKL or the right joystick axis
        float _xMovement = Input.GetAxis(RightStickXAxis);
        float _zMovement = Input.GetAxis(RightStickYAxis);

        if (_xMovement != 0 || _zMovement != 0)
        {
            _xMovement *= -isFacingRight.ToSign();
            _zMovement *= -isFacingRight.ToSign();

            ThrowAimingPoint = Vector3.Lerp(throwAimingPoint, new Vector3(throwAimingPoint.x + _xMovement, throwAimingPoint.y, throwAimingPoint.z + _zMovement), Time.deltaTime * 15);
        }

        // Raycast along the trajectory preview and stop the trail when hit something
        base.AimMethod();
    }

    /// <summary>
    /// Drop the weared throwable.
    /// </summary>
    public override void DropObject()
    {
        // If no throwable, return
        if (!throwable) return;

        // Drooop
        throwable.Drop();
        Throwables.Remove(throwable);
        SelectedThrowableIndex = selectedThrowableIndex;

        // Updates juggling informations
        UpdateJuggleParameters(false);

        // Updates the animator informations
        if (!throwable) SetAnimHasObject(false);
    }

    /// <summary>
    /// Try to grab a throwable.
    /// When grabbed, the object follows the character and can be thrown by this one.
    /// </summary>
    /// <param name="_throwable">Throwable to try to grab.</param>
    /// <returns>Returns true if the throwable was successfully grabbed, false either.</returns>
    public override bool GrabObject(TDS_Throwable _throwable)
    {
        // If currently wearing the maximum amount of throwables he can, return
        if (CurrentThrowableAmount == maxThrowableAmount) return false;

        // Take the object
        if (!_throwable.PickUp(this, handsTransform)) return false;
        Throwables.Add(_throwable);
        Throwable = _throwable;

        // Updates juggling informations
        UpdateJuggleParameters(true);

        // Updates animator informations
        SetAnimHasObject(true);

        return true;
    }

    /// <summary>
    /// Make the Juggler juggle ! Yeeeaah !
    /// </summary>
    private void Juggle()
    {
        // Updates hands transform position by lerp
        Vector3 _newPos = handsTransformMemoryLocalPosition;
        _newPos.x *= -isFacingRight.ToSign();
        _newPos += transform.position;

        // If not at point, lerp position and update trajectory preview if aiming
        if (handsTransform.position != _newPos)
        {
            handsTransform.position = Vector3.Lerp(handsTransform.position, _newPos, Time.deltaTime * 10);

            if (isAiming)
            {
                ThrowAimingPoint = throwAimingPoint;
            }
        }

        // If not having any throwable, return
        if (CurrentThrowableAmount == 0) return;

        float _baseTheta = 2 * Mathf.PI / CurrentThrowableAmount;

        for (int _i = 0; _i < CurrentThrowableAmount; _i++)
        {
            // Create variables
            TDS_Throwable _throwable = Throwables[_i];

            // Get theta value to position the object
            float _theta = _i + jugglerCounter;
            if (_theta > CurrentThrowableAmount) _theta -= CurrentThrowableAmount;
            _theta *= _baseTheta;

            Vector3 _newPosition = new Vector3(Mathf.Sin(_theta), Mathf.Cos(_theta), 0f) * throwableDistanceFromCenter;
            _newPosition.y += throwableDistanceFromCenter;

            // Position update
            _throwable.transform.localPosition = Vector3.Lerp(_throwable.transform.localPosition, _newPosition, Time.deltaTime * juggleSpeed * 2.5f);

            // Rotates the object
            _throwable.transform.Rotate(Vector3.forward, Time.deltaTime * (1000f / _throwable.Weight));
        }

        // Increase counter
        jugglerCounter += Time.deltaTime * juggleSpeed;
        if (jugglerCounter > CurrentThrowableAmount) jugglerCounter = 0;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    public override void ThrowObject()
    {
        // If no throwable, return
        if (!throwable) return;

        // Alright, then throw it !
        // Get the destination point in world space
        Vector3 _destinationPosition = new Vector3(transform.position.x + (throwAimingPoint.x * -isFacingRight.ToSign()), transform.position.y + throwAimingPoint.y, transform.position.z + (throwAimingPoint.z * -isFacingRight.ToSign()));

        // Now, throw that object
        throwable.transform.localPosition = Vector3.zero;
        throwable.Throw(_destinationPosition, aimAngle, RandomThrowBonusDamages);
        Throwables.Remove(throwable);
        SelectedThrowableIndex = selectedThrowableIndex;

        // Updates juggling informations
        UpdateJuggleParameters(false);

        // Triggers the throw animation ;
        // If not having throwable anymore, update the animator
        if (isGrounded) SetAnimThrow();
        if (!throwable) SetAnimHasObject(false);
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land.</param>
    public override void ThrowObject(Vector3 _targetPosition)
    {
        // If no throwable, return
        if (!throwable) return;

        // Now, throw that object
        throwable.transform.localPosition = Vector3.zero;
        throwable.Throw(_targetPosition, aimAngle, RandomThrowBonusDamages);
        Throwables.Remove(throwable);
        SelectedThrowableIndex = selectedThrowableIndex;

        // Updates juggling informations
        UpdateJuggleParameters(false);

        // Triggers the throw animation ;
        // If not having throwable anymore, update the animator
        if (isGrounded) SetAnimThrow();
        if (!throwable) SetAnimHasObject(false);
    }

    /// <summary>
    /// Updates juggle parameters depending on juggling objects amount.
    /// </summary>
    /// <param name="_doIncrease">Has the amount of objects increased or decreased ?</param>
    private void UpdateJuggleParameters(bool _doIncrease)
    {
        switch (CurrentThrowableAmount)
        {
            case 0:
                // Alright, do nothing
                break;

            case 1:
                juggleSpeed = 2;
                throwableDistanceFromCenter = .25f;
                break;

            case 2:
                juggleSpeed = 2.5f;
                throwableDistanceFromCenter = .5f;
                break;

            case 3:
                juggleSpeed = 2.8f;
                throwableDistanceFromCenter = .8f;
                break;

            case 4:
                juggleSpeed = 3.15f;
                throwableDistanceFromCenter = 1f;
                break;

            case 5:
                juggleSpeed = 3.5f;
                throwableDistanceFromCenter = 1.25f;
                break;

            // If amount is superior to 5
            default:
                if (_doIncrease)
                {
                    juggleSpeed += .15f;
                    throwableDistanceFromCenter += .1f;
                }
                else
                {
                    juggleSpeed -= .15f;
                    throwableDistanceFromCenter -= .1f;
                }
                break;
        }
    }
    #endregion

    #region Attacks
    /// <summary>
    /// Makes the player perform and light or heavy attack.
    /// </summary>
    /// <param name="_isLight">Is this a light attack ? Otherwise, it will be heavy.</param>
    public override void Attack(bool _isLight)
    {
        base.Attack(_isLight);

        // Triggers the right actions
        switch (comboCurrent.Count)
        {
            case 1:
                if (_isLight)
                {
                    currentAttack = attacks[0];
                    SetAnimLightAttack();
                }
                else
                {
                    currentAttack = attacks[1];
                    SetAnimHeavyAttack();
                }
                break;
            default:
                Debug.Log($"The Juggler was not intended to have more than one attack per combo, so... What's going on here ?");
                break;
        }
    }
    #endregion

    #region Animations
    /// <summary>
    /// Set this player heavy attack animation.
    /// </summary>
    public void SetAnimHeavyAttack()
    {
        animator.SetTrigger("Heavy Attack");
    }

    /// <summary>
    /// Set this player light attack animation.
    /// </summary>
    public void SetAnimLightAttack()
    {
        animator.SetTrigger("Light Attack");
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    public override void CheckActionsInputs()
    {
        base.CheckActionsInputs();

        // Check aiming point / angle changes
        if (TDS_Input.GetAxisDown(DPadXAxis))
        {
            int _index = selectedThrowableIndex + (int)Input.GetAxis(DPadXAxis);
            if (_index < 0) _index = CurrentThrowableAmount - 1;
            else if (_index == CurrentThrowableAmount) _index = 0;

            SelectedThrowableIndex = _index;
        }

        if (TDS_Input.GetAxis(DPadYAxis))
        {
            AimAngle += Input.GetAxis(DPadYAxis);
            ThrowAimingPoint = throwAimingPoint;
        }
    }
    #endregion

    #region Movements
    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        base.Flip();

        // Reverse X position
        throwAimingPoint.x *= -1;
        ThrowAimingPoint = throwAimingPoint;
    }
    #endregion

    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
    }

    // Frame-rate independent MonoBehaviour.FixedUpdate message for physics calculations
    protected override void FixedUpdate()
    {
        // If dead, return
        if (isDead) return;

        base.FixedUpdate();

        // 3, 2, 1... Let's Jam !
        Juggle();
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        // If dead, return
        if (isDead) return;

        base.Update();
	}
	#endregion

	#endregion
}
