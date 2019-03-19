using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	 *	####### TO DO #######
	 *	#####################
     * 
     *  - Auto-aim system.
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[28 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    - Fixed the aim problem where preview was upside down.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[27 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    - Clamped the aiming point.
     *	    - Fixed the changing selected object problem.
     *	    - Kick-out objects in the air when performing action.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[26 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    - Set selected objet in the third hand.
     *	    - Move juggling transform when moving.
     *	    - Base system for sending objects in the air while performing actions.
     *	    - Bugged system of changing selected object.
     *	    - Various little fixs.
	 *
	 *	-----------------------------------
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
            // Set the old selected one position
            if (throwable)
            {
                throwable.transform.position = juggleTransform.position;
                throwable.transform.SetParent(juggleTransform);
                Throwables.Add(throwable);

                // Stop coroutine if needed
                if (throwableLerpCoroutine != null) StopCoroutine(throwableLerpCoroutine);
            }

            // Set the new one
            if (value != null)
            {
                if (Throwables.Contains(value)) Throwables.Remove(value);
                value.transform.rotation = Quaternion.identity;
                value.transform.SetParent(handsTransform);

                // Starts position lerp coroutine
                throwableLerpCoroutine = StartCoroutine(LerpThrowableToHand());
            }

            throwable = value;
        }
    }

    /// <summary>
    /// All throwables the juggler has in hands.
    /// </summary>
    public List<TDS_Throwable> Throwables = new List<TDS_Throwable>();

    /// <summary>
    /// Transform at the juggler's third hand position.
    /// Used to set the selected throwable as child.
    /// </summary>
    [SerializeField] private Transform thirdHandTransform = null;
    #endregion

    #region Variables
    /// <summary>
    /// Value to increase <see cref="juggleTransformIdealLocalPosition"/>.y by.
    /// Used to kick-out juggling throwables in the air.
    /// </summary>
    private float juggleKickOutHeight = 0;

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

    /// <summary>
    /// Transform used to set as children objects juggling with.
    /// </summary>
    [SerializeField] protected Transform juggleTransform = null;

    /// <summary>
    /// The ideal position of the juggle transform in local space ;
    /// Used to lerp the transform to a new position when moving.
    /// </summary>
    [SerializeField] protected Vector3 juggleTransformIdealLocalPosition = Vector3.zero;

    /// <summary>
    /// The position of the juggle transform in local space.
    /// </summary>
    public Vector3 JuggleTransformLocalPosition
    {
        get
        {
            Vector3 _return = juggleTransform.position - transform.position;
            _return.x *= isFacingRight.ToSign();
            return _return;
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine lerping throwable position to hands transform position.
    /// </summary>
    private Coroutine throwableLerpCoroutine = null;
    #endregion

    #region Debugs & Memory variables
    /// <summary>
    /// Counter helping to position the objects juggling with.
    /// </summary>
    private float jugglerCounter = 0;

    /// <summary>
    /// Default aiming point, when starting aiming.
    /// </summary>
    private Vector3 defaultAimingPoint = Vector3.zero;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Aim & Throwables
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
            _xMovement *= isFacingRight.ToSign();

            // Clamp aiming point
            Vector3 _newAimingPoint = Vector3.ClampMagnitude(new Vector3(throwAimingPoint.x + _xMovement, throwAimingPoint.y, throwAimingPoint.z + _zMovement), 15);
            _newAimingPoint.y = -10;

            // Lerp aiming point position
            ThrowAimingPoint = Vector3.Lerp(throwAimingPoint, _newAimingPoint, Time.deltaTime * 15);
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

        // Set new throwable
        throwable = null;
        if (CurrentThrowableAmount > 0)
        {
            Throwable = Throwables[0];
        }

        // Updates the animator informations
        if (CurrentThrowableAmount == 0) SetAnim(PlayerAnimState.LostObject);
        else
        {
            // Updates juggling informations
            UpdateJuggleParameters(false);
        }
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

        Throwable = _throwable;

        // Updates juggling informations
        UpdateJuggleParameters(true);

        // Updates animator informations
        if (CurrentThrowableAmount > 0) SetAnim(PlayerAnimState.HasObject);

        return true;
    }

    /// <summary>
    /// Make the Juggler juggle ! Yeeeaah !
    /// </summary>
    private void Juggle()
    {
        // Updates hands transform position by lerp
        Vector3 _newPos = juggleTransformIdealLocalPosition;
        _newPos.x *= isFacingRight.ToSign();
        _newPos += transform.position;

        // Set juggling point height depending if kicked-out objects or not
        _newPos.y += juggleKickOutHeight;

        // If not at point, lerp position and update trajectory preview if aiming
        if (juggleTransform.position != _newPos)
        {
            juggleTransform.position = Vector3.Lerp(juggleTransform.position, _newPos, Time.deltaTime * 7);

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
    /// Kicks out juggling objects just above character.
    /// </summary>
    private void KickOutJuggleLight() => juggleKickOutHeight = 1.75f;

    /// <summary>
    /// Kicks out juggling objects out of screen.
    /// </summary>
    private void KickOutJuggleHeavy() => juggleKickOutHeight = 5;

    /// <summary>
    /// Lerps the selected throwable position to the hand transform position.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpThrowableToHand()
    {
        yield return null;

        while (throwable)
        {
            throwable.transform.position = Vector3.Lerp(throwable.transform.position, handsTransform.position, Time.deltaTime * 7);

            if (throwable.transform.position == handsTransform.position)
            {
                yield break;
            }

            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// Get juggling objects back in hands.
    /// </summary>
    private void GetBackJuggle() => juggleKickOutHeight = 0;

    /// <summary>
    /// Stops the preparing throw, if preparing one.
    /// </summary>
    /// <returns>Returns true if canceled the throw, false if there was nothing to cancel.</returns>
    public override bool StopAiming()
    {
        if (!base.StopAiming()) return false;

        // Reset throw aiming point
        ThrowAimingPoint = defaultAimingPoint;

        return true;
    }

    /// <summary>
    /// Switch the selected throwable with one among throwables juggling with.
    /// </summary>
    /// <param name="_index">Index of the new selected throwable among <see cref="Throwables"/>.</param>
    public void SwitchThrowable(int _index)
    {
        // Get selected throwable & place the previous one in the juggling list
        TDS_Throwable _selected = null;
        if (_index < 0)
        {
            _selected = Throwables[CurrentThrowableAmount - 1];
            Throwables.RemoveAt(CurrentThrowableAmount - 1);
            Throwables.Insert(0, throwable);
        }
        else
        {
            _selected = Throwables[0];
            Throwables.RemoveAt(0);
            Throwables.Add(throwable);
        }

        // Set previous one transform
        throwable.transform.SetParent(juggleTransform);

        // Stop coroutine if needed
        if (throwableLerpCoroutine != null) StopCoroutine(throwableLerpCoroutine);

        // Set the new throwable
        _selected.transform.rotation = Quaternion.identity;
        _selected.transform.SetParent(handsTransform);

        throwable = _selected;

        // Starts position lerp coroutine
        throwableLerpCoroutine = StartCoroutine(LerpThrowableToHand());
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
        Vector3 _destinationPosition = new Vector3(transform.position.x + (throwAimingPoint.x * isFacingRight.ToSign()), transform.position.y + throwAimingPoint.y, transform.position.z + throwAimingPoint.z);

        // Now, throw that object
        throwable.transform.localPosition = Vector3.zero;
        throwable.Throw(_destinationPosition, aimAngle, RandomThrowBonusDamages);

        // Set new throwable
        throwable = null;
        if (CurrentThrowableAmount > 0)
        {
            Throwable = Throwables[0];
        }

        // Triggers the throw animation ;
        // If not having throwable anymore, update the animator
        if (isGrounded) SetAnim(PlayerAnimState.Throw);
        if (CurrentThrowableAmount == 0) SetAnim(PlayerAnimState.LostObject);
        else
        {
            // Updates juggling informations
            UpdateJuggleParameters(false);
        }
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

        // Set new throwable
        throwable = null;
        if (CurrentThrowableAmount > 0)
        {
            Throwable = Throwables[0];
        }

        // Triggers the throw animation ;
        // If not having throwable anymore, update the animator
        if (isGrounded) SetAnim(PlayerAnimState.Throw);
        if (CurrentThrowableAmount == 0) SetAnim(PlayerAnimState.LostObject);
        else
        {
            // Updates juggling informations
            UpdateJuggleParameters(false);
        }
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
                    SetAnim(PlayerAnimState.LightAttack);
                }
                else
                {
                    currentAttack = attacks[1];
                    SetAnim(PlayerAnimState.HeavyAttack);
                }
                break;
            default:
                Debug.Log($"The Juggler was not intended to have more than one attack per combo, so... What's going on here ?");
                break;
        }
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
        if (TDS_Input.GetAxisDown(DPadXAxis) && (Throwables.Count > 0))
        {
            SwitchThrowable((int)Input.GetAxis(DPadXAxis));
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
        if (isAiming)
        {
            throwAimingPoint.x *= -1;
            ThrowAimingPoint = throwAimingPoint;
        }
    }
    #endregion

    #region Others
    /// <summary>
    /// Set events used by the script. Should be called on start.
    /// </summary>
    private void SetEvents()
    {
        // Set events to kick-out & get back juggling objects
        OnStartAttack += KickOutJuggleLight;
        OnStartDodging += KickOutJuggleLight;
        OnGetOffGround += KickOutJuggleLight;
        OnStartParry += KickOutJuggleHeavy;

        OnStopAttack += GetBackJuggle;
        OnStopDodge += GetBackJuggle;
        OnGetOnGround += GetBackJuggle;
        OnStopParry += GetBackJuggle;
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Try to get components references if they are missing
        if (!juggleTransform)
        {
            Debug.LogWarning("The Juggle Transform of \"" + name + "\" for script TDS_Juggler is missing !");
        }
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

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Draws a gizmos at the juggle transform ideal position
        Gizmos.DrawSphere(transform.position + (juggleTransformIdealLocalPosition * isFacingRight.ToSign()), .1f);
    }

    // Use this for initialization
    protected override void Start()
    {
        // Set the juggle transform ideal position
        juggleTransformIdealLocalPosition = new Vector3(.3f, .85f, 0);

        base.Start();

        // Get default aiming point
        defaultAimingPoint = throwAimingPoint;

        // Set events
        SetEvents();
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
