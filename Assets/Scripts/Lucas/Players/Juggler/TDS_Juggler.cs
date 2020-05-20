using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    #region Events
    /// <summary>
    /// Event called when the player grab an object or loose it.
    /// </summary>
    public new event Action<bool> OnHasObject = null;
    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Target object of the Juggler.
    /// </summary>
    private Collider targetObject = null;

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
                value.transform.rotation = Quaternion.Euler(0, value.transform.rotation.eulerAngles.y, 0);
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
    /// <summary>
    /// Indicates if the juggler can actually shoot.
    /// </summary>
    [SerializeField] private bool canShoot = true;

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

    /// <summary>Backing field for <see cref="aimingSpeedCoef"/>.</summary>
    [SerializeField] private float aimingSpeedCoef = .35f;

    /// <summary>
    /// Speed coefficient applied to the character while he's aiming.
    /// </summary>
    public float AimingSpeedCoef
    {
        get { return aimingSpeedCoef; }
        set
        {
            if (value < 0) value = 0;
            aimingSpeedCoef = value;
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

    /// <summary>Backing field for <see cref="TimeBetweenShoots"/>.</summary>
    [SerializeField] private float timeBetweenShoots = 1.5f;

    /// <summary>
    /// Minimum time to spend between two object shoots.
    /// </summary>
    public float TimeBetweenShoots
    {
        get { return timeBetweenShoots; }
        set
        {
            if (value < 0) value = 0;
            timeBetweenShoots = value;
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
    /// Tags detected as target when aiming.
    /// </summary>
    [SerializeField] private Tags aimDetectTags = new Tags();

    /// <summary>
    /// Aiming bounds on the X axis.
    /// </summary>
    private Vector2 aimXBounds = new Vector2();

    /// <summary>
    /// Aiming bounds on the Y axis.
    /// </summary>
    private Vector2 aimYBounds = new Vector2();

    /// <summary>
    /// The ideal position of the juggle transform in local space ;
    /// Used to lerp the transform to a new position when moving.
    /// </summary>
    [SerializeField] private Vector3 juggleTransformIdealLocalPosition = Vector3.zero;

    /// <summary>
    /// Returns <see cref="throwAimingPoint"/> vector3 in world space.
    /// </summary>
    public override Vector3 ThrowAimingPoint
    {
        get
        {
            return throwAimingPoint;
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Reference of the current coroutine of the aim method.
    /// </summary>
    private Coroutine aimCoroutine = null;

    /// <summary>
    /// Coroutine used to give the Juggler the ability to shoot back again after a shoot.
    /// </summary>
    private Coroutine shootCooldownCoroutine = null;

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

    #region Sounds
    /// <summary>
    /// Sound to play when locking an enemy
    /// </summary>
    [SerializeField] private AudioClip lockSound = null;
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
        while (controller.GetButton(ButtonType.Aim))
        {
            // Aim while holding the button
            AimMethod();

            yield return null;

            if (controller.GetButtonDown(ButtonType.Shoot) && canShoot)
            {
                TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Neutral);

                IsPlayable = false;
                if (targetEnemy) targetEnemy = null;
                if (targetObject) targetObject = null;
                SetAnimOnline(PlayerAnimState.Throw);
                canShoot = false;

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
        // If having a target, follows it 'til death or getting out of screen
        if (targetEnemy)
        {
            Vector3 _screenPos;
            if (targetEnemy.IsDead || !TDS_Camera.Instance.Camera.pixelRect.Contains(_screenPos = TDS_Camera.Instance.Camera.WorldToScreenPoint(targetEnemy.Collider.bounds.center)))
            {
                targetEnemy = null;
                TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Neutral);
            }
            else
            {
                if (aimTargetTransform.position != _screenPos)
                {
                    aimTargetTransform.position = _screenPos;
                }
                // Switch target when moving related axis
                if (controller.GetAxisDown(AxisType.HorizontalAim))
                {
                    TDS_Enemy[] _enemies = TDS_Enemy.AllEnemies.ToArray();
                    int _selection = controller.GetAxis(AxisType.HorizontalAim) > 0 ? 1 : -1;
                    int _index = Array.IndexOf(_enemies, targetEnemy);

                    for (int _i = 0; _i < _enemies.Length; _i++)
                    {
                        // Increase index
                        _index += _selection;
                        if (_index < 0) _index = _enemies.Length - 1;
                        else if (_index >= _enemies.Length) _index = 0;

                        // Get new available target position ; if on screen, take it
                        _screenPos = TDS_Camera.Instance.Camera.WorldToScreenPoint(_enemies[_index].Collider.bounds.center);

                        if (TDS_Camera.Instance.Camera.pixelRect.Contains(_screenPos) && (targetEnemy != _enemies[_index]))
                        {
                            // Set target enemy
                            targetEnemy = _enemies[_index];
                            aimTargetTransform.position = _screenPos;

                            // Play sound
                            PlayLock();
                            break;
                        }
                    }
                }

                return;
            }
        }

        // Get active enemies and sort them by position relative to the Juggler
        TDS_Enemy[] _activeEnemies = TDS_Enemy.AllEnemies.ToArray();

        if (_activeEnemies.Length > 0)
        {
            _activeEnemies = _activeEnemies.OrderBy(e => Mathf.Abs(e.transform.position.x - transform.position.x)).ToArray();
            Vector2 _enemyPosOnScreen = new Vector2();

            foreach (TDS_Enemy _enemy in _activeEnemies)
            {
                _enemyPosOnScreen = TDS_Camera.Instance.Camera.WorldToScreenPoint(_enemy.Collider.bounds.center);
                if (TDS_Camera.Instance.Camera.pixelRect.Contains(_enemyPosOnScreen))
                {
                    // Nullify target object
                    if (targetObject) targetObject = null;

                    // Set target enemy
                    targetEnemy = _enemy;

                    // Play sound
                    PlayLock();

                    // Set aim target & arrow positions
                    aimTargetTransform.position = _enemyPosOnScreen;
                    aimArrowTransform.anchoredPosition = (Vector2)TDS_Camera.Instance.Camera.WorldToScreenPoint(new Vector3(_enemy.Collider.bounds.center.x, _enemy.Collider.bounds.max.y + .1f, _enemy.Collider.bounds.center.z)) - (Vector2)aimTargetTransform.position;

                    // Set lock animation
                    TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Locked);
                    return;
                }
            }
        }

        // If no target, just let the player aim the point he wants, 'cause the juggler can do that. Yep
        Vector2 _newTarget = aimTargetTransform.anchoredPosition;

        // Aim with IJKL or the right joystick axis
        float _xMovement = controller.GetAxis(AxisType.HorizontalAim) * 50;
        float _yMovement = controller.GetAxis(AxisType.VerticalAim) * 50;

        // Clamp X target position in screen
        if (_xMovement != 0)
        {
            _newTarget.x += _xMovement;

            if (_xMovement < 0)
            {
                if (_newTarget.x < aimXBounds.x)
                {
                    _newTarget.x = aimXBounds.x;
                }
            }
            else if (_newTarget.x > aimXBounds.y)
            {
                _newTarget.x = aimXBounds.y;
            }
        }
        // Now clamp Y target position in screen
        if (_yMovement != 0)
        {
            _newTarget.y += _yMovement;

            if (_yMovement < 0)
            {
                if (_newTarget.y < aimYBounds.x)
                {
                    _newTarget.y = aimYBounds.x;
                }
            }
            else if (_newTarget.y > aimYBounds.y)
            {
                _newTarget.y = aimYBounds.y;
            }
        }

        // Set new target if different
        if (_newTarget != aimTargetTransform.anchoredPosition) aimTargetTransform.anchoredPosition = _newTarget;

        // Check if target is under enemy
        Ray _ray = TDS_Camera.Instance.Camera.ScreenPointToRay(aimTargetTransform.position);
        RaycastHit _hit = new RaycastHit();

        if (Physics.Raycast(_ray, out _hit, 100, whatCanAim) && _hit.collider.gameObject.HasTag(aimDetectTags.ObjectTags))
        {
            if (!targetObject) TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.UnderTarget);
            if (targetObject != _hit.collider) targetObject = _hit.collider;
        }
        else if (targetObject)
        {
            targetObject = null;
            TDS_UIManager.Instance.SetJugglerAimTargetAnim(JugglerAimTargetAnimState.Neutral);
        }
    }

    /// <summary>
    /// Allows back the Juggler to shoot. Yes !
    /// </summary>
    private IEnumerator AllowToShootCoroutine()
    {
        yield return new WaitForSeconds(timeBetweenShoots);

        canShoot = true;
        shootCooldownCoroutine = null;
    }

    /// <summary>
    /// Allows back the Juggler to shoot. Yes !
    /// </summary>
    public void CheckShootAbility()
    {
        if (!canShoot && (shootCooldownCoroutine == null)) canShoot = true;
    }

    /// <summary>
    /// Get juggling objects back in hands.
    /// </summary>
    private void GetBackJuggle()
    {
        // Do that for all other clients too
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "GetBackJuggle"), new object[] { });
        }

        juggleKickOutHeight = 0;
    }

    // -----------

    protected override bool CanGrabObject() => CurrentThrowableAmount < MaxThrowableAmount;

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
    private void KickOutJuggleLight()
    {
        // Do that for all other clients too
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "KickOutJuggleLight"), new object[] { });
        }

        juggleKickOutHeight = 5f;
    }

    /// <summary>
    /// Kicks out juggling objects out of screen.
    /// </summary>
    private void KickOutJuggleHeavy()
    {
        // Do that for all other clients too
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "KickOutJuggleHeavy"), new object[] { });
        }

        juggleKickOutHeight = 15;
    }

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
    /// Prepare a throw, if not already preparing one.
    /// </summary>
    /// <returns>Returns true if successfully prepared a throw ; false if one is already, or if cannot do this.</returns>
    public virtual bool PrepareThrow()
    {
        if (isAiming || !throwable) return false;

        isAiming = true;
        SpeedCoef *= aimingSpeedCoef;
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

        if (!throwable.IsHeld || (throwable.Owner == this)) throwable.transform.SetParent(null, true);

        throwable = null;
        
        if (CurrentThrowableAmount > 0)
        {
            Throwable = Throwables[0];

            // Updates juggling informations
            UpdateJuggleParameters(false);
        }
        else
        {
            // Triggers event
            if (photonView.isMine) OnHasObject?.Invoke(false);
        }

        if (CurrentThrowableAmount < MaxThrowableAmount)
        {
            // Activates the detection box
            interactionBox.DisplayInteractionFeedback(true);

            if (CurrentThrowableAmount == 0)
            {
                SetAnim(PlayerAnimState.LostObject);
                audioSource.Stop();
            }
        }

        return true;
    }

    /// <summary>
    /// Set bounds for aiming position, related to resolution and camera rect.
    /// </summary>
    public void SetAimBounds()
    {
        aimXBounds = new Vector2()
        {
            x = Screen.currentResolution.width * TDS_Camera.Instance.Camera.rect.x,
            y = Screen.currentResolution.width * (TDS_Camera.Instance.Camera.rect.width + TDS_Camera.Instance.Camera.rect.x)
        };

        aimYBounds = new Vector2()
        {
            x = (Screen.currentResolution.height / TDS_Camera.Instance.Camera.rect.height) * TDS_Camera.Instance.Camera.rect.y,
            y = (Screen.currentResolution.height + (TDS_Camera.Instance.Camera.rect.y * (Screen.currentResolution.height / TDS_Camera.Instance.Camera.rect.height))) * TDS_Camera.Instance.Camera.rect.width
        };
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
            if (!_wasJuggling)
            {
                SetAnim(PlayerAnimState.HasObject);

                audioSource.time = 0;
                audioSource.Play();
            }

            // Updates juggling informations
            UpdateJuggleParameters(true);

            if (CurrentThrowableAmount == MaxThrowableAmount)
            {
                // Desactivates the detection box
                interactionBox.DisplayInteractionFeedback(false);
            }
        }

        // Triggers event
        if (photonView.isMine) OnHasObject?.Invoke(true);

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

        // Fix all kinds of aiming-related things
        if (targetObject) targetObject = null;
        if (targetEnemy) targetEnemy = null;
        if (aimCoroutine != null)
        {
            StopCoroutine(aimCoroutine);
            aimCoroutine = null;
        }

        // Reset back speed coefficient.
        SpeedCoef /= aimingSpeedCoef;

        // Desactivate aim target
        TDS_UIManager.Instance.DesctivateJugglerAimTarget();

        return true;
    }

    /// <summary>
    /// Switch the selected throwable with one among throwables juggling with.
    /// </summary>
    /// <param name="_doIncrease">Should the new throwable be at the increased index of the current one.</param>
    public void SwitchThrowable(bool _doIncrease)
    {
        // Do that for all other clients too
        if (photonView.isMine)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SwitchThrowable"), new object[] { _doIncrease });
        }

        // Get selected throwable & place the previous one in the juggling list
        TDS_Throwable _selected = null;
        Transform[] _objectAnchors = new Transform[objectAnchors.Length];

        if (!_doIncrease)
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
        _selected.transform.rotation = Quaternion.Euler(0, _selected.transform.transform.rotation.eulerAngles.y, 0);

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
        if (!photonView.isMine)
            return false;

        // Get the destination point in world space
        Ray _ray = TDS_Camera.Instance.Camera.ScreenPointToRay(aimTargetTransform.position);
        RaycastHit _info = new RaycastHit();

        if (Physics.Raycast(_ray, out _info, 100, whatCanAim))
            throwAimingPoint = _info.point;
        else
            throwAimingPoint = _ray.origin + (_ray.direction * 75);

        if (base.ThrowObject_A())
        {
            shootCooldownCoroutine = StartCoroutine(AllowToShootCoroutine());
            return true;
        }

        canShoot = true;
        return false;
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

        yield return base.Dodge();
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
        if (PhotonNetwork.isMasterClient)
        {
            while (throwable)
            {
                DropObject();
            }
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
        if (!isAiming && controller.GetButton(ButtonType.Aim))
        {
            PrepareThrow();
            return -1;
        }

        // If no throwable, return 0
        if (Throwables.Count == 0) return 0;

            // Check aiming point / angle changes
        if (controller.GetButtonDown(ButtonType.SwitchPlus))
        {
            SwitchThrowable(true);
            return -1;
        }
        if (controller.GetButtonDown(ButtonType.SwitchMinus))
        {
            SwitchThrowable(false);
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
        juggleTransform.localScale = new Vector3(juggleTransform.localScale.x, juggleTransform.localScale.y, juggleTransform.localScale.z * -1);
    }

    /// <summary>
    /// Freezes the player's movements and actions.
    /// </summary>
    public override void FreezePlayer()
    {
        base.FreezePlayer();

        if (isAiming) StopAiming();
    }
    #endregion

    #region Sounds
    /// <summary>
    /// Plays sound for when locking an enemy.
    /// </summary>
    protected void PlayLock() => TDS_SoundManager.Instance.PlayEffectSound(lockSound, audioSource);
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

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    protected override void OnDestroy()
    {
        base.OnDestroy();

        // Stop aiming on destroy
        if (isAiming) StopAiming();
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

        if (photonView.isMine)
        {
            // Set events 
            SetEvents();

            // Get aim target & arrow RectTransform
            aimTargetTransform = TDS_UIManager.Instance.JugglerAimTargetTransform;
            aimArrowTransform = TDS_UIManager.Instance.JugglerAimArrowTransform;

            // Set aim bounds on X & Y
            SetAimBounds();
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
