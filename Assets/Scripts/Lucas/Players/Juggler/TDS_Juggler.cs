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
    /// Current target enemy of the Juggler.
    /// </summary>
    private TDS_Enemy targetEnemy = null;

    /// <summary>
    /// Dictionnary containing juggling throwables as keys with their anchor as value.
    /// </summary>
    private Dictionary<TDS_Throwable, Transform> throwableAnchors = new Dictionary<TDS_Throwable, Transform>();

    /// <summary>
    /// The actual selected throwable. It is the object to throw, when throwing... Yep.
    /// </summary>
    public new TDS_Throwable Throwable
    {
        get { return throwable; }
        set
        {
            // Stop coroutine if needed
            if (throwableLerpCoroutine != null)
            {
                StopCoroutine(throwableLerpCoroutine);
                throwableLerpCoroutine = null;
            }

            // Set the new one
            if (value != null)
            {
                if (Throwables.Contains(value))
                {
                    // Reorder the anchor array
                    Transform[] _objectAnchors = new Transform[objectAnchors.Length];
                    int _index = Throwables.IndexOf(value);
                    for (int _i = 0; _i < _index; _i++)
                    {
                        _objectAnchors[_i] = objectAnchors[_i];
                    }
                    for (int _i = _index; _i < objectAnchors.Length - 1; _i++)
                    {
                        _objectAnchors[_i] = objectAnchors[_i + 1];
                    }
                    _objectAnchors[objectAnchors.Length - 1] = objectAnchors[_index];

                    objectAnchors = _objectAnchors;

                    // Remove the object
                    Throwables.Remove(value);
                }
                value.transform.rotation = Quaternion.identity;
                value.transform.SetParent(handsTransform, true);

                // Starts position lerp coroutine
                throwableLerpCoroutine = StartCoroutine(LerpThrowableToHand());
            }

            // Set the old selected one position
            if (throwable)
            {
                throwable.transform.position = juggleTransform.position;
                Throwables.Add(throwable);

                // Associate the new throwabke juggling with with an anchor
                Transform _anchor = objectAnchors[Throwables.Count - 1];
                _anchor.position = throwable.Sprite.bounds.center;
                throwable.transform.SetParent(_anchor);
            }

            throwable = value;
        }
    }

    /// <summary>
    /// All throwables the juggler has in hands.
    /// </summary>
    public List<TDS_Throwable> Throwables = new List<TDS_Throwable>();

    /// <summary>
    /// Transform of the target point currently aiming.
    /// </summary>
    private RectTransform aimTargetTransform = null;

    /// <summary>
    /// Transform of the arrow for the locked aiming enemy.
    /// </summary>
    private RectTransform aimArrowTransform = null;

    /// <summary>
    /// Transform used to set as children objects juggling with.
    /// </summary>
    [SerializeField] private Transform juggleTransform = null;

    /// <summary>
    /// Anchors used to juggle with throwables.
    /// </summary>
    [SerializeField] private Transform[] objectAnchors = new Transform[] { };
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

    /// <summary>
    /// Maximum amount of throwable this Juggler can carry on.
    /// </summary>
    public int MaxThrowableAmount
    {
        get { return objectAnchors.Length; }
    }

    /// <summary>
    /// Layer mask referencing what the player can aim at.
    /// </summary>
    [SerializeField] private LayerMask whatCanAim = new LayerMask();

    /// <summary>
    /// The ideal position of the juggle transform in local space ;
    /// Used to lerp the transform to a new position when moving.
    /// </summary>
    [SerializeField] private Vector3 juggleTransformIdealLocalPosition = Vector3.zero;
    #endregion

    #region Coroutines
    /// <summary>
    /// Reference of the current coroutine of the aim method.
    /// </summary>
    private Coroutine aimCoroutine = null;

    /// <summary>
    /// Reference of the current coroutine of the lock behaviour.
    /// </summary>
    private Coroutine lockCoroutine = null;

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
    /// <returns>Returns true if the attack as correctly been activated, false otherwise.</returns>
    public override bool ActiveAttack(int _attackIndex)
    {
        // If aiming, stop
        if (isAiming) StopAiming();

        // Attack
        return base.ActiveAttack(_attackIndex);
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
        // Activate aiming target and set its default position
        TDS_UIManager.Instance.ActivateJugglerAimTarget();

        aimTargetTransform.anchoredPosition = TDS_Camera.Instance.Camera.WorldToScreenPoint(ThrowAimingPoint);

        // While holding the throw button, aim a position
        while (TDS_InputManager.GetButton(TDS_InputManager.THROW_BUTTON))
        {
            // Draws the preview of the projectile trajectory while holding the throw button
            if (lockCoroutine == null) AimMethod();

            yield return null;

            if (TDS_InputManager.GetButtonUp(TDS_InputManager.PARRY_BUTTON))
            {
                targetEnemy = null;
                // Triggers the throw animation
                if (lockCoroutine != null)
                {
                    StopCoroutine(lockCoroutine);
                    lockCoroutine = null;
                }
                TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Neutral);

                IsPlayable = false;
                SetAnimOnline(PlayerAnimState.Throw);

                if (CurrentThrowableAmount == 0) break;
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
        Vector2 _newTarget = aimTargetTransform.anchoredPosition;

        // Aim with IJKL or the right joystick axis
        float _xMovement = Input.GetAxis(TDS_InputManager.HORIZONTAL_ALT_AXIS) * 30;
        float _yMovement = Input.GetAxis(TDS_InputManager.VERTICAL_ALT_AXIS) * 30;

        // Clamp X target position in screen
        if (_xMovement != 0)
        {
            _newTarget.x += _xMovement;

            if (_xMovement > 0)
            {
                if (_newTarget.x > Screen.currentResolution.width) _newTarget.x = Screen.currentResolution.width;
            }
            else if (_newTarget.x < 0) _newTarget.x = 0;
        }
        // Now clamp Y target position in screen
        if (_yMovement != 0)
        {
            _newTarget.y += _yMovement;

            if (_yMovement > 0)
            {
                if (_newTarget.y > Screen.currentResolution.height) _newTarget.y = Screen.currentResolution.height;
            }
            else if (_newTarget.y < 0) _newTarget.y = 0;
        }

        // Set new target if different
        if (_newTarget != aimTargetTransform.anchoredPosition) aimTargetTransform.anchoredPosition = _newTarget;

        // Check if target is under enemy
        Ray _ray = TDS_Camera.Instance.Camera.ScreenPointToRay(aimTargetTransform.anchoredPosition);
        RaycastHit _hit = new RaycastHit();

        if (Physics.Raycast(_ray, out _hit, 100, whatCanAim) && _hit.collider.gameObject.HasTag("Enemy"))
        {
            if (!targetEnemy) TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.UnderTarget);
            TDS_Enemy _target = _hit.collider.GetComponent<TDS_Enemy>();

            if (targetEnemy != _target) targetEnemy = _target;

            // If pressing throw button, lock the enemy !
            if (TDS_InputManager.GetButtonDown(TDS_InputManager.PARRY_BUTTON))
            {
                lockCoroutine = StartCoroutine(LockEnemy());
            }
        }
        else if (targetEnemy)
        {
            targetEnemy = null;
            TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Neutral);
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
        // Call this method in master client only
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, GetType(), "GrabObject"), new object[] { _throwable.photonView.viewID });
            return false;
        }

        // If currently wearing the maximum amount of throwables he can, return
        if ((CurrentThrowableAmount == MaxThrowableAmount) || !_throwable.PickUp(this)) return false;

        return true;
    }

    /// <summary>
    /// Make the Juggler juggle ! Yeeeaah !
    /// </summary>
    private void Juggle()
    {
        // If not having any throwable, just set position if different and return
        if (CurrentThrowableAmount == 0)
        {
            if (juggleTransform.localPosition != juggleTransformIdealLocalPosition)
            {
                juggleTransform.localPosition = juggleTransformIdealLocalPosition;
            }
            return;
        }

        // Updates hands transform position by lerp
        Vector3 _newPos = juggleTransformIdealLocalPosition;

        // Set juggling point height depending if kicked-out objects or not
        _newPos.y += juggleKickOutHeight;

        // If not at point, lerp position and update trajectory preview if aiming
        if (juggleTransform.localPosition != _newPos)
        {
            juggleTransform.localPosition = Vector3.Lerp(juggleTransform.localPosition, _newPos, Time.deltaTime * 5);
        }

        // Juggle now !
        float _baseTheta = 2 * Mathf.PI / CurrentThrowableAmount;
        float _theta = 0;

        for (int _i = 0; _i < CurrentThrowableAmount; _i++)
        {
            // Create variables
            Transform _throwable = objectAnchors[_i];

            // Get theta value to position the object
            _theta = _i + jugglerCounter;
            if (_theta > CurrentThrowableAmount) _theta -= CurrentThrowableAmount;
            _theta *= _baseTheta;

            Vector3 _newPosition = new Vector3(
                                   Mathf.Sin(_theta) * throwableDistanceFromCenter, 
                                   Mathf.Cos(_theta) * throwableDistanceFromCenter);
            _newPosition.y += throwableDistanceFromCenter;

            // Position update
            _throwable.localPosition = Vector3.Lerp(_throwable.localPosition, _newPosition, Time.deltaTime * juggleSpeed);

            // Rotates the object
            _throwable.Rotate(Vector3.forward, Time.deltaTime * 200 + (2f / Throwables[_i].Weight));
        }

        // Increase counter
        jugglerCounter += Time.deltaTime * juggleSpeed;
        if (jugglerCounter > CurrentThrowableAmount) jugglerCounter = -CurrentThrowableAmount;
    }

    /// <summary>
    /// Kicks out juggling objects just above character.
    /// </summary>
    private void KickOutJuggleLight() => juggleKickOutHeight = 5f;

    /// <summary>
    /// Kicks out juggling objects out of screen.
    /// </summary>
    private void KickOutJuggleHeavy() => juggleKickOutHeight = 15;

    /// <summary>
    /// Lerps the selected throwable position to the hand transform position.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpThrowableToHand()
    {
        yield return null;

        while (throwable)
        {
            throwable.transform.position = Vector3.Lerp(throwable.transform.position, handsTransform.position, Time.deltaTime * 7.5f);

            if (throwable.transform.position == handsTransform.position)
            {
                yield break;
            }

            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// Locks an enemy.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LockEnemy()
    {
        // Wait a few before starting
        yield return new WaitForSeconds(.1f);

        // Set aim target & arrow positions
        aimTargetTransform.anchoredPosition = TDS_Camera.Instance.Camera.WorldToScreenPoint(targetEnemy.Collider.bounds.center);
        aimArrowTransform.anchoredPosition = (Vector2)TDS_Camera.Instance.Camera.WorldToScreenPoint(new Vector3(targetEnemy.Collider.bounds.center.x, targetEnemy.Collider.bounds.max.y + .1f, targetEnemy.Collider.bounds.center.z)) - aimTargetTransform.anchoredPosition;

        // Set lock animation
        TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Locked);

        // Lock the target enemy !
        while (targetEnemy && TDS_InputManager.GetButton(TDS_InputManager.PARRY_BUTTON))
        {
            Vector2 _newTarget = TDS_Camera.Instance.Camera.WorldToScreenPoint(targetEnemy.Collider.bounds.center);

            if (_newTarget != aimTargetTransform.anchoredPosition)
            {
                aimTargetTransform.anchoredPosition = _newTarget;
            }

            yield return null;
        }

        lockCoroutine = null;
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

        return true;
    }

    /// <summary>
    /// Removes the throwable from the character.
    /// </summary>
    /// <returns>Returns true if successfully removed the throwable, false otherwise.</returns>
    public override bool RemoveThrowable()
    {
        if (!throwable) return false;

        throwable = null;
        if (CurrentThrowableAmount > 0)
        {
            Throwable = Throwables[0];

            // Updates juggling informations
            UpdateJuggleParameters(false);
        }
        if (CurrentThrowableAmount == 0) SetAnim(PlayerAnimState.LostObject);

        return true;
    }

    /// <summary>
    /// Set this character throwable.
    /// </summary>
    /// <param name="_throwable">Throwable to set.</param>
    /// <returns>Returns true if successfully set the throwable, false otherwise.</returns>
    public override bool SetThrowable(TDS_Throwable _throwable)
    {
        // Get if was juggling before taking this throwable
        bool _wasJuggling = CurrentThrowableAmount > 0;

        if (!_throwable) return false;
        Throwable = _throwable;

        if (CurrentThrowableAmount > 0)
        {
            if (!_wasJuggling) SetAnim(PlayerAnimState.HasObject);

            // Updates juggling informations
            UpdateJuggleParameters(true);
        }

        return true;
    }

    /// <summary>
    /// Stops the preparing throw, if preparing one.
    /// </summary>
    /// <returns>Returns true if canceled the throw, false if there was nothing to cancel.</returns>
    public bool StopAiming()
    {
        if (!isAiming && aimCoroutine == null) return false;

        if (isAiming) isAiming = false;

        targetEnemy = null;
        if (aimCoroutine != null)
        {
            StopCoroutine(aimCoroutine);
            aimCoroutine = null;
        }
        if (lockCoroutine != null)
        {
            StopCoroutine(lockCoroutine);
            lockCoroutine = null;
        }

        // Desactivate aim target
        TDS_UIManager.Instance.DesctivateJugglerAimTarget();

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
        Transform[] _objectAnchors = new Transform[objectAnchors.Length];

        if (_index < 0)
        {
            _selected = Throwables[CurrentThrowableAmount - 1];
            Throwables.RemoveAt(CurrentThrowableAmount - 1);
            Throwables.Insert(0, throwable);

            // Reorder the anchor array
            _objectAnchors[0] = objectAnchors[CurrentThrowableAmount - 1];
            _objectAnchors[0].position = throwable.Sprite.bounds.center;
            throwable.transform.SetParent(_objectAnchors[0]);

            for (int _i = 0; _i < CurrentThrowableAmount - 1; _i++)
            {
                _objectAnchors[_i + 1] = objectAnchors[_i];
            }
            for (int _i = CurrentThrowableAmount; _i < objectAnchors.Length; _i++)
            {
                _objectAnchors[_i] = objectAnchors[_i];
            }
        }
        else
        {
            _selected = Throwables[0];
            Throwables.RemoveAt(0);
            Throwables.Add(throwable);

            // Reorder the anchor array
            _objectAnchors[CurrentThrowableAmount - 1] = objectAnchors[0];
            _objectAnchors[CurrentThrowableAmount - 1].position = throwable.Sprite.bounds.center;
            throwable.transform.SetParent(_objectAnchors[CurrentThrowableAmount - 1]);

            for (int _i = 0; _i < CurrentThrowableAmount - 1; _i++)
            {
                _objectAnchors[_i] = objectAnchors[_i + 1];
            }
            for (int _i = CurrentThrowableAmount; _i < objectAnchors.Length; _i++)
            {
                _objectAnchors[_i] = objectAnchors[_i];
            }
        }

        objectAnchors = _objectAnchors;

        // Stop coroutine if needed
        if (throwableLerpCoroutine != null)
        {
            StopCoroutine(throwableLerpCoroutine);
            throwableLerpCoroutine = null;
        }

        // Set the new throwable
        
        _selected.transform.SetParent(handsTransform, true);
        _selected.transform.rotation = Quaternion.identity;

        throwable = _selected;

        // Starts position lerp coroutine
        throwableLerpCoroutine = StartCoroutine(LerpThrowableToHand());
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    public override bool ThrowObject_A()
    {
        // If not mine, return false
        if (!photonView.isMine) return false;

        // Get the destination point in world space
        Ray _ray = TDS_Camera.Instance.Camera.ScreenPointToRay(aimTargetTransform.anchoredPosition);
        RaycastHit _info = new RaycastHit();

        if (Physics.Raycast(_ray, out _info, 100, whatCanAim))
        {
            return ThrowObject(_info.point);
        }

        return ThrowObject(_ray.origin + (_ray.direction * 75));
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
                throwableDistanceFromCenter = 1;
                break;

            case 2:
                juggleSpeed = 2.5f;
                throwableDistanceFromCenter = 1.25f;
                break;

            case 3:
                juggleSpeed = 3.5f;
                throwableDistanceFromCenter = 1.5f;
                break;

            case 4:
                juggleSpeed = 3.75f;
                throwableDistanceFromCenter = 2f;
                break;

            case 5:
                juggleSpeed = 4f;
                throwableDistanceFromCenter = 2.5f;
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
        if (photonView.isMine && isAiming) StopAiming();

        return true;
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
    public override int CheckActionsInputs()
    {
        int _result = base.CheckActionsInputs();
        if (_result != 0) return _result;

        // Check throw
        if (TDS_InputManager.GetButtonDown(TDS_InputManager.THROW_BUTTON))
        {
            PrepareThrow();
            return -1;
        }

        // Check aiming point / angle changes
        if (TDS_InputManager.GetAxisDown(TDS_InputManager.D_PAD_X_Axis) && (Throwables.Count > 0))
        {
            SwitchThrowable((int)Input.GetAxis(TDS_InputManager.D_PAD_X_Axis));
            return -1;
        }

        // If everything went good, return 0
        return 0;
    }
    #endregion

    #region Movements
    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public override void Flip()
    {
        // Get juggle transform rotation before flip
        Quaternion _baseRotation = juggleTransform.rotation;

        base.Flip();

        // Rotates the juggle transform so that it stays at the same location
        juggleTransform.rotation = _baseRotation;
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
        if (objectAnchors.Length == 0)
        {
            objectAnchors = juggleTransform.GetComponentsInChildren<Transform>();
        }

        // Set player type, just in case
        PlayerType = PlayerType.Juggler;
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Draws a gizmos at the juggle transform ideal position
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position + new Vector3(juggleTransformIdealLocalPosition.x * isFacingRight.ToSign(), juggleTransformIdealLocalPosition.y, juggleTransformIdealLocalPosition.z), .1f);
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        // Set the juggle transform ideal position
        juggleTransformIdealLocalPosition = new Vector3(.3f, .85f, 0);

        base.Start();

        if (photonView.isMine)
        {
            // Set events 
            SetEvents();

            // Get aim target & arrow RectTransform
            aimTargetTransform = TDS_UIManager.Instance.JugglerAimTargetTransform;
            aimArrowTransform = TDS_UIManager.Instance.JugglerAimArrowTransform;
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        // 3, 2, 1... Let's Jam !
        if (!isDead) Juggle();
    }
	#endregion

	#endregion
}
