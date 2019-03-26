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

    /// <summary>
    /// Image used to display this character current health status.
    /// </summary>
    [SerializeField] protected UnityEngine.UI.Image healthBar = null;

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
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "Flip"), new object[] { });
        OnFlip?.Invoke();
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
    public virtual void DropObject()
    {
        // If no throwable, return
        if (!throwable) return;

        // Drooop
        throwable.Drop();
        Throwable = null;
    }

    /// <summary>
    /// Try to grab a throwable.
    /// When grabbed, the object follows the character and can be thrown by this one.
    /// </summary>
    /// <param name="_throwable">Throwable to try to grab.</param>
    /// <returns>Returns true if the throwable was successfully grabbed, false either.</returns>
    public virtual bool GrabObject(TDS_Throwable _throwable)
    {
        // If already having a throwable, return false
        if (throwable) return false;

        // Take the object
        bool _canPickUp = _throwable.PickUp(this, handsTransform); 
        if (!_canPickUp) return false;
        Throwable = _throwable;

        return true;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    public virtual void ThrowObject()
    {
        // If no throwable, return
        if (!throwable) return;

        // Alright, then throw it !
        // Get the destination point in world space
        Vector3 _destinationPosition = new Vector3(transform.position.x + (throwAimingPoint.x * isFacingRight.ToSign()), transform.position.y + throwAimingPoint.y, transform.position.z + throwAimingPoint.z);

        // Now, throw that object
        throwable.Throw(_destinationPosition, aimAngle, RandomThrowBonusDamages);
        Throwable = null;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land</param>
    public virtual void ThrowObject(Vector3 _targetPosition)
    {
        // If no throwable, return
        if (!throwable) return;

        // Alright, then throw it !
        throwable.Throw(_targetPosition, aimAngle, RandomThrowBonusDamages);
        Throwable = null;
    }
    #endregion

    #endregion

    #region Photon Methods

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
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    protected virtual void OnDrawGizmos()
    {
        if (collider)
        {
            Gizmos.color = IsInvulnerable ? new Color(0, 0, 1, .5f) : new Color(0, 1, 0, .5f);
            Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }
	
	// Update is called once per frame
	protected override void Update()
    {
        base.Update();
	}
	#endregion

	#endregion
}
