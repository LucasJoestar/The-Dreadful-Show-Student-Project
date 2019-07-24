using System;
using UnityEngine;
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

    #region Events
    /// <summary>
    /// Event called when the character flip on the X axis.
    /// </summary>
    public event Action OnFlip = null;
    #endregion

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

    /// <summary>
    /// Rigidbody of this character.
    /// Mainly used to project this one in the air.
    /// </summary>
    [SerializeField] protected new Rigidbody rigidbody = null;

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
    public bool IsDown = false;

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
        transform.Rotate(Vector3.up, 180);
        shadowTransform.localPosition = new Vector3(shadowTransform.localPosition.x, shadowTransform.localPosition.y, shadowTransform.localPosition.z * -1);
        // if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Flip"), new object[] { });
        OnFlip?.Invoke();
    }

    /// <summary>
    /// Method called when this character hit something.
    /// Override it to implement feedback and other things.
    /// </summary>
    public virtual void HitCallback()
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
    /// <summary>
    /// Drop the weared throwable.
    /// </summary>
    /// <returns>Returns true if successfully dropped the object, false if not having an object to drop.</returns>
    public virtual bool DropObject()
    {
        // Call this method in master client only
        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "DropObject"), new object[] { });
            return false;
        }

        // If no throwable, return
        if (!throwable) return false;

        // Drooop
        throwable.Drop();

        // Set the throwable as null for all clients
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetThrowable"), new object[] { throwable.photonView.viewID, false });

        Throwable = null;

        return true;
    }

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
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "GrabObject"), new object[] { _throwable.photonView.viewID });
            return false;
        }

        // Take the object if possible
        if (throwable || !_throwable.PickUp(this, handsTransform)) return false;

        // Set the throwable for all clients
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetThrowable"), new object[] { _throwable.photonView.viewID, true });

        Throwable = _throwable;

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
        if (!_throwable) return false;

        return GrabObject(_throwable);
    }

    /// <summary>
    /// Set this character throwable (Grab or Throw / Drop).
    /// </summary>
    /// <param name="_throwableID">ID of the throwable to set.</param>
    /// <param name="_doGrab">Indicates if the character grabs the throwable or throw / drop it.</param>
    public virtual void SetThrowable(int _throwableID, bool _doGrab)
    {
        TDS_Throwable _throwable = PhotonView.Find(_throwableID).GetComponent<TDS_Throwable>();
        if (_throwable)
        {
            _throwable.transform.SetParent(_doGrab ? handsTransform : null, true);
            Throwable = _throwable;
        }
        else throwable = null;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    public virtual bool ThrowObject_A()
    {
        // Get the destination point in world space
        Vector3 _destinationPosition = new Vector3(transform.position.x + (throwAimingPoint.x * isFacingRight.ToSign()), transform.position.y + throwAimingPoint.y, transform.position.z + throwAimingPoint.z);

        return ThrowObject(_destinationPosition);
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land</param>
    public virtual bool ThrowObject(Vector3 _targetPosition)
    {
        // Call this method in master client only
        if (!PhotonNetwork.isMasterClient || !throwable) return false;

        // Alright, then throw it !
        throwable.Throw(_targetPosition, aimAngle, RandomThrowBonusDamages);

        // Set the throwable as null for all clients
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetThrowable"), new object[] { throwable.photonView.viewID, false });

        Throwable = null;

        return true;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_x">X position where to throw.</param>
    /// <param name="_y">Y position where to throw.</param>
    /// <param name="_z">Z position where to throw.</param>
    public virtual void ThrowObject(float _x, float _y, float _z)
    {
        ThrowObject(new Vector3(_x, _y, _z));
    }
    #endregion

    #region Health
    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public override bool TakeDamage(int _damage)
    {
        if (base.TakeDamage(_damage))
        {
            if (throwable) DropObject();

            return true;
        }

        return false;
    }
    #endregion

    #region UI 
    /// <summary>
    /// Fill the life bar
    /// </summary>
    /// <param name="_health"></param>
    public virtual void UpdateLifeBar(int _health)
    {
        if (!HealthBar || !TDS_UIManager.Instance) return;
        float _fillingValue = Mathf.Clamp((float)healthCurrent / (float)healthMax, 0, 1);
        TDS_UIManager.Instance.FillImage(HealthBar, _fillingValue); 
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
    /// Put the character on the ground.
    /// </summary>
    public virtual bool PutOnTheGround()
    {
        if (IsDown) return false;

        // Drop object if having one
        if (throwable) DropObject();

        if (bringingCloserCoroutine != null) StopBringingCloser();

        IsDown = true;

        return true;
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
        if (!photonView.isMine)
        {
            rigidbody.isKinematic = true;
            rigidbody.useGravity = false;
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
