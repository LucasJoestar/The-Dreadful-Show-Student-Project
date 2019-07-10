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
     *	Date :			[21 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    - Removed aim system from Player class, and set it only in the Juggler one.
	 *
	 *	-----------------------------------
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
    /// Zone at the end of the projectile preview, for feedback value.
    /// </summary>
    [SerializeField] private GameObject projectilePreviewEndZone = null;

    /// <summary>
    /// Line renderer used to draw a preview for the preparing throw trajectory.
    /// </summary>
    [SerializeField] protected LineRenderer lineRenderer = null;

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

    /// <summary>Backing field for <see cref="ThrowPreviewPrecision"/>.</summary>
    [SerializeField] private int throwPreviewPrecision = 10;

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
    [SerializeField] private LayerMask whatIsAllButThis = new LayerMask();

    /// <summary>
    /// Transform used to set as children objects juggling with.
    /// </summary>
    [SerializeField] private Transform juggleTransform = null;

    /// <summary>
    /// The ideal position of the juggle transform in local space ;
    /// Used to lerp the transform to a new position when moving.
    /// </summary>
    [SerializeField] private Vector3 juggleTransformIdealLocalPosition = Vector3.zero;

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

    #region Coroutines
    /// <summary>
    /// Reference of the current coroutine of the aim method.
    /// </summary>
    private Coroutine aimCoroutine = null;

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

    /// <summary>
    /// Points used to draw a preview of the projectile trajectory when preparing a throw (Local space).
    /// </summary>
    private Vector3[] throwTrajectoryMotionPoints = new Vector3[] { };
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions

    #region Attacks
    /// <summary>
    /// Makes the player active its planned attack.
    /// </summary>
    /// <param name="_attackIndex">Index of the attack to activate from <see cref="attacks"/>.</param>
    public override void ActiveAttack(int _attackIndex)
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // Attack
        base.ActiveAttack(_attackIndex);
    }

    /// <summary>
    /// Performs the catch attack of this player.
    /// </summary>
    public override void Catch()
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // Catch
        base.Catch();
    }

    /// <summary>
    /// Performs the Super attack if the gauge is filled enough.
    /// </summary>
    public override void SuperAttack()
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // SUPER attack
        base.SuperAttack();
    }
    #endregion

    #region Aim & Throwables
    /// <summary>
    /// Makes the character aim for a throw. When releasing the throw button, throw the selected object.
    /// If the cancel throw button is pressed, cancel the throw, as it name indicate it.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Aim()
    {
        // While holding the throw button, aim a position
        while (TDS_InputManager.GetButton(TDS_InputManager.THROW_BUTTON))
        {
            // Draws the preview of the projectile trajectory while holding the throw button
            AimMethod();

            yield return null;

            if (TDS_InputManager.GetButtonDown(TDS_InputManager.PARRY_BUTTON))
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
    protected void AimMethod()
    {
        // Let the player aim the point he wants, 'cause the juggler can do that. Yep
        // Aim with IJKL or the right joystick axis
        float _xMovement = Input.GetAxis(TDS_InputManager.RIGHT_STICK_X_Axis);
        float _zMovement = Input.GetAxis(TDS_InputManager.RIGHT_STICK_Y_AXIS);

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
                projectilePreviewEndZone.transform.position = _hit.point;

                Quaternion _rotation = Quaternion.Lerp(projectilePreviewEndZone.transform.rotation, Quaternion.FromToRotation(Vector3.up, _hit.normal), Time.deltaTime * 15);

                projectilePreviewEndZone.transform.rotation = _rotation;

                // Set indicative boolean
                _hasHit = true;

                break;
            }
        }

        // If no touch, update end zone position & rotation
        if (!_hasHit)
        {
            // Updates the position of the end preview zone & its rotation according to the hit point
            projectilePreviewEndZone.transform.position = new Vector3(transform.position.x + (throwAimingPoint.x * isFacingRight.ToSign()), transform.position.y + throwAimingPoint.y, transform.position.z + throwAimingPoint.z);

            Quaternion _rotation = Quaternion.Lerp(projectilePreviewEndZone.transform.rotation, Quaternion.FromToRotation(Vector3.up, Vector3.up), Time.deltaTime * 15);
            _rotation.x *= isFacingRight.ToSign();

            projectilePreviewEndZone.transform.rotation = _rotation;

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
    public override bool DropObject()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "DropObject"), new object[] { });
            return false;
        }

        // If no throwable, return
        if (!throwable) return false;

        // Drooop
        throwable.Drop();

        // Remove the throwable for all clients
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetThrowable"), new object[] { throwable.photonView.viewID, false });

        // Set ownership
        throwable.photonView.TransferOwnership(PhotonNetwork.masterClient);

        // Set new throwable
        throwable = null;
        if (CurrentThrowableAmount > 0)
        {
            Throwable = Throwables[0];
        }

        // Updates the animator informations
        if (CurrentThrowableAmount == 0)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", photonView.owner, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)PlayerAnimState.LostObject });
        }
        else
        {
            // Updates juggling informations
            UpdateJuggleParameters(false);
        }

        return true;
    }

    /// <summary>
    /// Try to grab a throwable.
    /// When grabbed, the object follows the character and can be thrown by this one.
    /// </summary>
    /// <param name="_throwable">Throwable to try to grab.</param>
    /// <returns>Returns true if the throwable was successfully grabbed, false either.</returns>
    public override bool GrabObject(TDS_Throwable _throwable)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "GrabObject"), new object[] { _throwable.photonView.viewID });
            return false;
        }

        // If currently wearing the maximum amount of throwables he can, return
        if (CurrentThrowableAmount == maxThrowableAmount) return false;

        // Take the object
        if (!_throwable.PickUp(this, handsTransform)) return false;

        // Set the throwable for all clients
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetThrowable"), new object[] { _throwable.photonView.viewID, true });

        Throwable = _throwable;

        // Updates juggling informations
        UpdateJuggleParameters(true);

        // Updates animator informations
        if (CurrentThrowableAmount > 0)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", photonView.owner, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)PlayerAnimState.HasObject });
        }

        // Set ownership
        _throwable.photonView.TransferOwnership(photonView.owner);

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
            _throwable.transform.Rotate(Vector3.forward, Time.deltaTime * (_throwable.Weight * 5));
        }

        // Increase counter
        jugglerCounter += Time.deltaTime * juggleSpeed;
        if (jugglerCounter > CurrentThrowableAmount) jugglerCounter = -CurrentThrowableAmount;
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
    /// Prepare a throw, if not already preparing one.
    /// </summary>
    /// <returns>Returns true if successfully prepared a throw ; false if one is already, or if cannot do this.</returns>
    public virtual bool PrepareThrow()
    {
        if (isAiming || !throwable) return false;

        isAiming = true;
        aimCoroutine = StartCoroutine(Aim());

        projectilePreviewEndZone.SetActive(true);

        return true;
    }

    /// <summary>
    /// Set this character throwable (Grab or Throw / Drop).
    /// </summary>
    /// <param name="_throwableID">ID of the throwable to set.</param>
    /// <param name="_doGrab">Indicates if the character grabs the throwable or throw / drop it.</param>
    public override void SetThrowable(int _throwableID, bool _doGrab)
    {
        TDS_Throwable _throwable = PhotonView.Find(_throwableID).GetComponent<TDS_Throwable>();

        if (_throwable)
        {
            if (_doGrab)
            {
                _throwable.transform.SetParent( handsTransform, true);
                Throwable = _throwable;
            }
            else
            {
                _throwable.transform.SetParent(null, true);
                throwable = null;
                if (CurrentThrowableAmount > 0)
                {
                    Throwable = Throwables[0];
                }
            }
        }
        else
        {
            throwable = null;
            if (CurrentThrowableAmount > 0)
            {
                Throwable = Throwables[0];
            }
        }
    }

    /// <summary>
    /// Stops the preparing throw, if preparing one.
    /// </summary>
    /// <returns>Returns true if canceled the throw, false if there was nothing to cancel.</returns>
    public bool StopAiming()
    {
        if (!isAiming && aimCoroutine == null) return false;

        if (isAiming) isAiming = false;
        if (aimCoroutine != null)
        {
            StopCoroutine(aimCoroutine);
        }

        lineRenderer.DrawTrajectory(new Vector3[0]);
        projectilePreviewEndZone.SetActive(false);

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
    public override bool ThrowObject()
    {
        // Get the destination point in world space
        Vector3 _destinationPosition = new Vector3(transform.position.x + (throwAimingPoint.x * isFacingRight.ToSign()), transform.position.y + throwAimingPoint.y, transform.position.z + throwAimingPoint.z);

        return ThrowObject(_destinationPosition);
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land.</param>
    public override bool ThrowObject(Vector3 _targetPosition)
    {
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "ThrowObject"), new object[] { _targetPosition.x, _targetPosition.y, _targetPosition.z });
            return false;
        }

        // If no throwable, return
        if (!throwable) return false;

        // Now, throw that object
        throwable.transform.localPosition = Vector3.zero;
        throwable.Throw(_targetPosition, aimAngle, RandomThrowBonusDamages);

        // Remove the throwable for all clients
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetThrowable"), new object[] { throwable.photonView.viewID, false });

        // Set ownership
        throwable.photonView.TransferOwnership(PhotonNetwork.masterClient);

        // Set new throwable
        throwable = null;
        if (CurrentThrowableAmount > 0)
        {
            Throwable = Throwables[0];
        }

        // Triggers the throw animation ;
        // If not having throwable anymore, update the animator
        if (isGrounded)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", photonView.owner, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)PlayerAnimState.Throw });
        }
        if (CurrentThrowableAmount == 0)
        {
            TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", photonView.owner, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnim"), new object[] { (int)PlayerAnimState.LostObject });
        }
        else
        {
            // Updates juggling informations
            UpdateJuggleParameters(false);
        }

        return true;
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

    #region Actions
    /// <summary>
    /// Performs a dodge.
    /// While dodging, the player cannot take damage or attack.
    /// </summary>
    protected override IEnumerator Dodge()
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        yield return null;

        dodgeCoroutine = StartCoroutine(base.Dodge());
    }

    /// <summary>
    /// Set the player in parry position.
    /// While parrying, the player avoid to take damages.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator Parry()
    {
        // If aiming, cannot parry
        if (isAiming) yield break;

        StartCoroutine(base.Parry());
    }

    /// <summary>
    /// Use the selected object in the inventory.
    /// </summary>
    public override void UseObject()
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // Use
        base.UseObject();
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

        // Drop all objects
        while (throwable)
        {
            DropObject();
        }

        return true;
    }

    /// <summary>
    /// Put the character on the ground.
    /// </summary>
    public override bool PutOnTheGround()
    {
        if (!base.PutOnTheGround()) return false;

        // Drop all objects
        while (throwable)
        {
            DropObject();
        }

        return true;
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

        // Drop all objects juggling with
        while (throwable)
        {
            DropObject();
        }
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        // Executes base method
        if (!base.TakeDamage(_damage)) return false;

        // Is aiming, cancel the preparing throw
        if (isAiming) StopAiming();

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
        // Executes base method
        if (!base.TakeDamage(_damage)) return false;

        // Is aiming, cancel the preparing throw
        if (isAiming) StopAiming();

        return true;
    }
    #endregion

    #region Inputs
    /// <summary>
    /// Checks inputs for this player's all actions.
    /// </summary>
    public override void CheckActionsInputs()
    {
        base.CheckActionsInputs();

        // Check throw
        if (TDS_InputManager.GetButtonDown(TDS_InputManager.THROW_BUTTON)) PrepareThrow();

        // Check aiming point / angle changes
        if (TDS_InputManager.GetAxisDown(TDS_InputManager.D_PAD_X_Axis) && (Throwables.Count > 0))
        {
            SwitchThrowable((int)Input.GetAxis(TDS_InputManager.D_PAD_X_Axis));
        }

        if (TDS_InputManager.GetAxis(TDS_InputManager.D_PAD_Y_Axis))
        {
            AimAngle += Input.GetAxis(TDS_InputManager.D_PAD_Y_Axis);
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
        if (!lineRenderer)
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            if (!lineRenderer) Debug.LogWarning("The LineRenderer of \"" + name + "\" for script TDS_Player is missing !");
        }
        if (!juggleTransform)
        {
            Debug.LogWarning("The Juggle Transform of \"" + name + "\" for script TDS_Juggler is missing !");
        }
        if (!projectilePreviewEndZone)
        {
            Debug.LogWarning("The Projectile Preview End Zone of \"" + name + "\" for script TDS_Juggler is missing !");
        }

        // Set player type, just in case
        PlayerType = PlayerType.Juggler;
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
        base.Start();

        // Set the juggle transform ideal position
        juggleTransformIdealLocalPosition = new Vector3(.3f, .85f, 0);

        base.Start();

        // Get trajectory motion points
        throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(handsTransform.localPosition, throwAimingPoint, throwVelocity.magnitude, aimAngle, throwPreviewPrecision);

        // Get layer for everything except this player one
        whatIsAllButThis = ~(1 << gameObject.layer | 1 << LayerMask.NameToLayer("Object"));

        // Get default aiming point
        defaultAimingPoint = throwAimingPoint;

        // Set events
        if (photonView.isMine) SetEvents();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // 3, 2, 1... Let's Jam !
        if (photonView.isMine && !isDead) Juggle();
    }
	#endregion

	#endregion
}
