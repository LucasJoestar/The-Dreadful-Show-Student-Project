﻿using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public abstract class TDS_Character : TDS_Damageable
{
    /* TDS_Character :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class to inherit from for all characters types, that is for Player and Enemy classes.
     *	    
     *	    Contains everything needed for both players and enemies they share in common.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
     * 
     *  Date :			[12 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the handsTransform field ; the RandomThrowBonusDamages & Throwable properties ; and the throwBonusDamagesMax & throwBonusDamagesMin fields & properties.
     *	    
     *	    Fulfilled all throwables related methods so that characters can now pick-up objects and throw them.
	 *
	 *	-----------------------------------
     * 
     *  Date :			[06 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the isAttacking field.
     *	    - Added the StopAttack method.
	 *
	 *	-----------------------------------
     * 
     *  Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the OnFlip event.
     *	    - Added the throwAimingPoint field ; and the aimAngle field & property.
	 *
	 *	-----------------------------------
     * 
     *  Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Modified the SpeedMax & SpeedInitial properties.
     *	    - Modified the debugs for component missing in Awake.
     *	    - Removed the attacks field & property.
	 *
	 *	-----------------------------------
     * 
     *  Date :			[22 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the IncreaseSpeed method.
	 *
	 *	-----------------------------------
     * 
     *  Date :			[17 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the Rigidbody field ; the SpeedAccelerationTime, SpeedCoef, SpeedCurrent, SpeedInitial & SpeedMax properties.
	 *
	 *	-----------------------------------
     * 
     *  Date :			[16 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the Attacks  & IsAttacking properties.
     *	    - Renamed the CanAttack field in IsPacific.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[15 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Character class.
     *	
     *	    - Added hitBox, healthBar & Throwable fields ; CanAttack field ; isFacingRight field & property ; IsParalyzed, SpeedAccelerationTime, SpeedCoef, speedCurrent, SpeedInitial & SpeedMax fields ; and attacks field.
     *	    - Added Flip method ; DropObject, GrabObject & ThrowObject empty methods to fill later, when the TDS_Throwable class will be fulfilled.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Hit box of this character.
    /// Contains all informations about the current attack being proceeded.
    /// </summary>
    [SerializeField] protected TDS_HitBox hitBox = null;

    public TDS_HitBox HitBox { get { return hitBox; } }

    /// <summary>Backing field for <see cref="HealthBar"/>.</summary>
    [SerializeField] private TDS_LifeBar healthBar;

    /// <summary>
    /// LifeBar of this character to display its current health status.
    /// </summary>
    public TDS_LifeBar HealthBar
    {
        get { return healthBar; }
        set
        {
            healthBar = value;
        }
    }

    /// <summary>Backing field for <see cref="Throwable"/>.</summary>
    [SerializeField] protected TDS_Throwable throwable = null;

    /// <summary>
    /// The throwable this character is currently wearing.
    /// </summary>
    public TDS_Throwable Throwable
    {
        get { return throwable; }
        protected set { throwable = value; }
    }

    /// <summary>
    /// Transform set at the hands position of the character.
    /// </summary>
    [SerializeField] protected Transform handsTransform = null;

    /// <summary>
    /// Transform of the character's shadow.
    /// </summary>
    [SerializeField] protected Transform shadowTransform = null;
    #endregion

    #region Variables
    /// <summary>Backing field for <see cref="IsAttacking"/>.</summary>
    [SerializeField] protected bool isAttacking = false;

    /// <summary>
    /// Is this character currently attacking, or not ?
    /// </summary>
    public bool IsAttacking
    {
        get { return isAttacking; }
        protected set
        {
            isAttacking = value;
        }
    }

    /// <summary>
    /// Indicates if the character is down on the ground.
    /// </summary>
    public bool IsDown { get; protected set; } = false;

    /// <summary>Backing field for <see cref="IsFacingRight"/>.</summary>
    [SerializeField] protected bool isFacingRight = true;

    /// <summary>
    /// Indicates which side the character is facing.
    /// If false, the character is facing left.
    /// </summary>
    public bool IsFacingRight
    {
        get { return isFacingRight; }
        protected set { isFacingRight = value; }
    }

    /// <summary>
    /// If pacific, the character is deprived to attack.
    /// </summary>
    public bool IsPacific = false;

    /// <summary>
    /// If paralyzed, the character cannot move.
    /// </summary>
    public bool IsParalyzed = false;

    /// <summary>
    /// Indicates if this character is currently projected.
    /// </summary>
    public bool IsProjected { get; protected set; } = false;

    /// <summary>Backing field for <see cref="AimAngle"/>.</summary>
    [SerializeField] protected float aimAngle = 45;

    /// <summary>
    /// Angle used to aim and throw objects.
    /// </summary>
    public float AimAngle
    {
        get { return aimAngle; }
        set
        {
            value = Mathf.Clamp(value, 15, 60);
            aimAngle = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedAccelerationTime"/></summary>
    [SerializeField] protected float speedAccelerationTime = .5f;

    /// <summary>
    /// The time it takes (in seconds) for this character speed (<see cref="SpeedCurrent"/>) from its initial value when starting to move (<see cref="SpeedInitial"/>) to reach its limit (<see cref="SpeedMax"/>).
    /// </summary>
    public float SpeedAccelerationTime
    {
        get { return speedAccelerationTime; }
        set
        {
            if (value < 0) value = 0;
            speedAccelerationTime = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedCoef"/></summary>
    [SerializeField] protected float speedCoef = 1;

    /// <summary>
    /// Coefficient used to multiply all speed values of this character.
    /// Useful to slow down or speed up.
    /// </summary>
    public float SpeedCoef
    {
        get { return speedCoef; }
        set
        {
            if (value < 0) value = 0;
            speedCoef = value;
        }
    }

    /// <summary>Backing field for <see cref="speedCurrent"/></summary>
    [SerializeField] protected float speedCurrent = 0;

    /// <summary>
    /// Current speed of the character movements.
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedCurrent
    {
        get { return speedCurrent; }
        protected set
        {
            value = Mathf.Clamp(value, 0, SpeedMax);
            speedCurrent = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedInitial"/></summary>
    [SerializeField] protected float speedInitial = 1;

    /// <summary>
    /// Initial speed of the character when starting moving.
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedInitial
    {
        get { return speedInitial; }
        set
        {
            value = Mathf.Clamp(value, 0, speedMax);
            speedInitial = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedMax"/></summary>
    [SerializeField] protected float speedMax = 2;

    /// <summary>
    /// Maximum speed of the character
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedMax
    {
        get { return speedMax; }
        set
        {
            if (value < 0) value = 0;
            speedMax = value;

            if (speedCurrent > value) SpeedCurrent = value;
        }
    }

    /// <summary>
    /// Get a random throw bonus damages value between <see cref="throwBonusDamagesMin"/> & <see cref="throwBonusDamagesMax"/>.
    /// </summary>
    public int RandomThrowBonusDamages
    {
        get
        {
            return Random.Range(throwBonusDamagesMin, throwBonusDamagesMax);
        }
    }

    /// <summary>Backing field for <see cref="ThrowBonusDamagesMax"/>.</summary>
    [SerializeField] protected int throwBonusDamagesMax = 45;

    /// <summary>
    /// Maximum amount of damages to add to a throw.
    /// </summary>
    public int ThrowBonusDamagesMax
    {
        get { return throwBonusDamagesMax; }
        set
        {
            if (value < 0) value = 0;
            throwBonusDamagesMax = value;
        }
    }

    /// <summary>Backing field for <see cref="ThrowBonusDamagesMin"/>.</summary>
    [SerializeField] protected int throwBonusDamagesMin = 45;

    /// <summary>
    /// Minimum amount of damages to add to a throw.
    /// </summary>
    public int ThrowBonusDamagesMin
    {
        get { return throwBonusDamagesMin; }
        set
        {
            value = Mathf.Clamp(value, 0, throwBonusDamagesMax);
            throwBonusDamagesMin = value;
        }
    }

    /// <summary>
    /// Point where the character is aiming to throw (Local space).
    /// </summary>
    [SerializeField] protected Vector3 throwAimingPoint = Vector3.zero;

    /// <summary>
    /// Returns <see cref="throwAimingPoint"/> vector3 in world space.
    /// </summary>
    public virtual Vector3 ThrowAimingPoint
    {
        get
        {
            return transform.position + new Vector3(throwAimingPoint.x * isFacingRight.ToSign(), throwAimingPoint.y, throwAimingPoint.z);
        }
    }
    #endregion

    #region Sound
    [SerializeField] protected string foostepsSoundEvent = "Play_FOOT_jugler";
    #endregion

    #endregion
    
    #region Methods

    #region Original Methods

    #region Character
    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public virtual void Flip()
    {
        isFacingRight = !isFacingRight;

        if (photonView.isMine)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "Flip", new object[] { isFacingRight });
        }

        transform.Rotate(Vector3.up, 180);

        Vector3 _vector = transform.localScale;
        transform.localScale = new Vector3(_vector.x, _vector.y, _vector.z * -1);

        _vector = shadowTransform.localPosition;
        shadowTransform.localPosition = new Vector3(_vector.x, _vector.y, _vector.z * -1);
    }

    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public void Flip(bool _isFacingRight)
    {
        if (isFacingRight != _isFacingRight)
            Flip();
    }

    /// <summary>
    /// Method called when this character hit something.
    /// Override it to implement feedback and other things.
    /// </summary>
    /// <param name="_opponentXCenter">X value of the opponent collider center position.</param>
    /// <param name="_opponentYMax">Y value of the opponent collider max position.</param>
    /// <param name="_opponentZ">Z value of the opponent position.</param>
    public virtual void HitCallback(float _opponentXCenter, float _opponentYMax, float _opponentZ)
    {
    }

    /// <summary>
    /// Automatically increases the speed of the character, according to all speed settings.
    /// </summary>
    protected virtual void IncreaseSpeed()
    {
        if (speedCurrent == 0) SpeedCurrent = speedInitial;
        else
        {
            SpeedCurrent += Time.deltaTime * ((speedMax - speedInitial) / speedAccelerationTime);
        }
    }

    /// <summary>
    /// Set the bonus damages of the actual attack.
    /// </summary>
    /// <param name="_bonusDamages"></param>
    public virtual void SetBonusDamages(int _bonusDamages) => hitBox.BonusDamages = _bonusDamages;

    /// <summary>
    /// Stop or ends the current attack of the character.
    /// </summary>
    public virtual void StopAttack()
    {
        IsAttacking = false;
        hitBox.Desactivate();
    }
    #endregion

    #region Throwable Object
    protected virtual bool CanGrabObject() => !throwable;

    /// <summary>
    /// Try to grab a throwable.
    /// When grabbed, the object follows the character and can be thrown by this one.
    /// </summary>
    /// <param name="_throwable">Throwable to try to grab.</param>
    /// <returns>Returns true if the throwable was successfully grabbed, false either.</returns>
    public virtual bool GrabObject(TDS_Throwable _throwable)
    {
        // Call this method in master client only
        if (!PhotonNetwork.isMasterClient)
        {
            if (CanGrabObject() && !_throwable.IsHeld)
            {
                TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, photonView, GetType(), "GrabObject", new object[] { _throwable.photonView.viewID });

                if (_throwable is TDS_SpecialThrowable _special)
                    _special.ActiveSpecialEvent();

                SetThrowable(_throwable);
                return true;
            }
            return false;
        }

        // Take the object if possible
        if (!CanGrabObject() || !_throwable.PickUp(this))
        {
            TDS_RPCManager.Instance.CallRPC(photonView.owner, photonView, GetType(), "RemoveThrowable", new object[] { });
            return false;
        }
        return true;
    }

    /// <summary>
    /// Try to grab a throwable.
    /// When grabbed, the object follows the character and can be thrown by this one.
    /// </summary>
    /// <param name="_throwable">ID of the throwable to try to grab.</param>
    /// <returns>Returns true if the throwable was successfully grabbed, false either.</returns>
    public virtual bool GrabObject(int _throwableID)
    {
        TDS_Throwable _throwable = PhotonView.Find(_throwableID).GetComponent<TDS_Throwable>();
        if (!_throwable)
            return false;

        return GrabObject(_throwable);
    }

    /// <summary>
    /// Drop the weared throwable.
    /// </summary>
    /// <returns>Returns true if successfully dropped the object, false if not having an object to drop.</returns>
    public virtual bool DropObject()
    {
        if (!throwable)
            return false;

        // Call this method in MasterClient
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, photonView, GetType(), "DropObject", new object[] { });

            if (!throwable.DropLocal())
                RemoveThrowable();

            return true;
        }

        // Drop
        throwable.Drop();
        return true;
    }

    /// <summary>
    /// Removes the throwable from the character.
    /// </summary>
    /// <returns>Returns true if successfully removed the throwable, false otherwise.</returns>
    public virtual bool RemoveThrowable()
    {
        if (!throwable)
            return false;

        if (throwable.transform.parent == handsTransform)
            throwable.transform.SetParent(null, true);

        throwable = null;
        return true;
    }

    /// <summary>
    /// Set this character throwable.
    /// </summary>
    /// <param name="_throwable">Throwable to set.</param>
    /// <returns>Returns true if successfully set the throwable, false otherwise.</returns>
    public virtual bool SetThrowable(TDS_Throwable _throwable)
    {
        if (_throwable)
        {
            if (throwable == _throwable)
                return true;
            else
                RemoveThrowable();

            Throwable = _throwable;
            _throwable.transform.SetParent(handsTransform, true);
            _throwable.transform.localPosition = Vector3.zero;
            _throwable.transform.rotation = Quaternion.Euler(0, _throwable.transform.transform.rotation.eulerAngles.y, 0);

            if (isFacingRight != (_throwable.transform.lossyScale.z > 0))
            {
                _throwable.transform.Rotate(Vector3.up, 180);
                _throwable.transform.localScale = new Vector3(_throwable.transform.localScale.x, _throwable.transform.localScale.y, _throwable.transform.localScale.z * -1);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Set this character throwable.
    /// </summary>
    /// <param name="_throwableID">ID of the throwable to set.</param>
    protected virtual void SetThrowable(int _throwableID)
    {
        PhotonView _throwable = PhotonView.Find(_throwableID);
        if (_throwable)
            SetThrowable(_throwable.GetComponent<TDS_Throwable>());
    }

    /// <summary>
    /// Throws the weared throwable from animation.
    /// </summary>
    public virtual bool ThrowObject_A()
    {
        if (photonView.isMine)
        {
            if (PhotonNetwork.isMasterClient)
            {
                return ThrowObject(ThrowAimingPoint);
            }

            if (throwable && throwable.ThrowLocal(ThrowAimingPoint, aimAngle))
            {
                TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, photonView, GetType(), "ThrowObject", new object[] { ThrowAimingPoint.x, ThrowAimingPoint.y, ThrowAimingPoint.z });
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land</param>
    public virtual bool ThrowObject(Vector3 _targetPosition)
    {
        if (!throwable)
            return false;

        throwable.Throw(_targetPosition, aimAngle, RandomThrowBonusDamages);
        return true;
    }

    protected void ThrowObject(float _x, float _y, float _z)
    {
        // If object throw is refused,
        // then owner shouldn't have it.
        // If so, the object is in another character's hand,
        // so its position will be soon updated by grab RPC.
        ThrowObject(new Vector3(_x, _y, _z));
    }
    #endregion

    #region Health
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected override void Die()
    {
        base.Die();
        // Stop all sounds

        if (!photonView.isMine) return;

        if (IsDown) GetUp();
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        if (base.TakeDamage(_damage))
        {
            if (PhotonNetwork.isMasterClient)
                DropObject();

            return true;
        }
        return false;
    }
    #endregion

    #region Effects
    /// <summary>
    /// Bring this damageable closer from a certain distance.
    /// </summary>
    /// <param name="_distance">Distance to browse.</param>
    public override bool BringCloser(float _distance)
    {
        if (IsDown) return false;

        // Drop object if having one
        if (throwable) DropObject();

        return base.BringCloser(_distance);
    }

    /// <summary>
    /// Tells the Character that he's getting up.
    /// </summary>
    public virtual void GetUp()
    {
        IsDown = false;
    }

    /// <summary>
    /// Apply knockback on this damageable.
    /// </summary>
    /// <param name="_toRight">Should the damageable be pushed to the right of left.</param>
    /// <returns>Returns true if successfully applied knockback on this damageable, false otherwise.</returns>
    public override bool Knockback(bool _toRight)
    {
        if (IsDown) return false;
        return base.Knockback(_toRight);
    }

    /// <summary>
    /// Project this damageable in the air.
    /// </summary>
    /// <param name="_toRight">Should the damageable be pushed to the right of left.</param>
    /// <returns>Returns true if successfully projected this damageable in the air, false otherwise.</returns>
    public override bool Project(bool _toRight)
    {
        if (IsDown || !base.Project(_toRight)) return false;

        IsDown = true;
        return true;
    }

    /// <summary>
    /// Put the character on the ground.
    /// </summary>
    public virtual bool PutOnTheGround()
    {
        if (!photonView.isMine)
        {
            TDS_RPCManager.Instance.CallRPC(photonView.owner, photonView, GetType(), "PutOnTheGround", new object[] { });
            return false;
        }

        if (IsDown) return false;

        // Drop object if having one
        if (throwable) DropObject();

        if (bringingCloserCoroutine != null) StopBringingCloser();

        IsDown = true;

        return true;
    }
    #endregion

    #region Sound
    /// <summary>
    /// Plays sound for when this character's body fall down.
    /// </summary>
    protected void PlayBodyFall()
    {
        // Play body fall
        AkSoundEngine.PostEvent("Play_BODYFALL", gameObject);
    }

    /// <summary>
    /// Plays sound for when this character's body fall down.
    /// </summary>
    protected void PlaySlide()
    {
        // Play slide
        AkSoundEngine.PostEvent("Play_SLIDE", gameObject);
    }

    /// <summary>
    /// Plays sound for when hitting something brutally.
    /// </summary>
    protected void PlayBrutalHit()
    {
        // Play brutal hit
        AkSoundEngine.PostEvent("Play_BRUTAL_HIT", gameObject);
    }

    /// <summary>
    /// Plays sound for this character's footsteps.
    /// </summary>
    protected virtual void PlayFootsteps()
    {
        AkSoundEngine.SetRTPCValue("foot_surface", TDS_LevelManager.Instance.IsDirtGround ? .2f : .1f, gameObject);
        AkSoundEngine.PostEvent(foostepsSoundEvent, gameObject);
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        // Try to get components references if they are missing
        if (!hitBox)
        {
            hitBox = GetComponentInChildren<TDS_HitBox>();
            if (!hitBox) Debug.LogWarning("The HitBox of \"" + name + "\" for script TDS_Character is missing !");
        }
        if (!rigidbody)
        {
            rigidbody = GetComponent<Rigidbody>();
            if (!rigidbody) Debug.LogWarning("The Rigidbody of \"" + name + "\" for script TDS_Character is missing !");
        }
        if (!handsTransform)
        {
            Debug.LogWarning("The Hands Transform of \"" + name + "\" for script TDS_Character is missing !");
        }
        if (!shadowTransform)
        {
            Debug.LogWarning("The Shadow Transform of \"" + name + "\" for script TDS_Character is missing !");
        }
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    protected virtual void OnDrawGizmos()
    {
        if (collider)
        {
            Gizmos.color = IsInvulnerable ? new Color(0, 0, 1, .5f) : isDead ? new Color(0, 0, 0, .5f) : new Color(0, 1, 0, .5f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }
    #endregion

    #endregion
}
