using System;
using System.Collections.Generic;
using UnityEngine;

public class TDS_Character : TDS_Damageable 
{
    /* TDS_Character :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class to inherit from for all characters types, that is for Player and Enemy class.
     *	    
     *	    Contains everything needed for both players and enemies they share in common.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[15 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Character class.
     *	    - Added hitBox, healthBar & Throwable fields ; CanAttack field ; isFacingRight field & property ; IsParalyzed, SpeedAccelerationTime, SpeedCoef, speedCurrent, SpeedInitial & SpeedMax fields ; and attacks field.
     *	    - Added Flip method ; DropObject, GrabObject & ThrowObject empty methods to fill later, when the TDS_Throwable class will be fulfilled.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Hit box of this character.
    /// Contains all informations about the current attack being proceeded.
    /// </summary>
    [SerializeField] protected TDS_HitBox hitBox = null;

    /// <summary>
    /// Image used to display this character current health status.
    /// </summary>
    [SerializeField] protected UnityEngine.UI.Image healthBar = null;

    /// <summary>
    /// The throwable this character is currently wearing.
    /// </summary>
    public TDS_Throwable Throwable = null;
    #endregion

    #region Variables
    /// <summary>
    /// Can this character currently attack or not ?
    /// </summary>
    public bool CanAttack = true;

    /// <summary>Backing field for <see cref="IsFacingRight"/></summary>
    protected bool isFacingRight = true;

    /// <summary>
    /// Indicates which side the character is facing.
    /// If false, the character is facing left.
    /// </summary>
    public bool IsFacingRight
    {
        get { return isFacingRight; }
        private set { isFacingRight = value; }
    }

    /// <summary>
    /// If paralyzed, the character cannot move or attack.
    /// </summary>
    public bool IsParalyzed = false;

    /// <summary>
    /// The time it takes for this character speed (<see cref="speedCurrent"/>) from its initial value when starting to move (<see cref="SpeedInitial"/>) to reach its limit (<see cref="SpeedMax"/>).
    /// </summary>
    public float SpeedAccelerationTime = .5f;

    /// <summary>
    /// Coefficient used to multiply all speed values of this character.
    /// Useful to slow down or speed up.
    /// </summary>
    public float SpeedCoef = 1;

    /// <summary>
    /// Current speed of the character movements.
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    protected float speedCurrent = 0;

    /// <summary>
    /// Initial speed of the character when starting moving.
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedInitial = 1;

    /// <summary>
    /// Maximum speed of the character
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedMax = 2;
    #endregion

    /// <summary>
    /// All attacks this character can perform.
    /// Contains informations about their animation, damages, effect and others.
    /// </summary>
    [SerializeField] protected List<TDS_Attack> attacks = new List<TDS_Attack>();

    #endregion

    #region Methods

    #region Original Methods

    /// <summary>
    /// Flips this character to have they looking at the opposite side.
    /// </summary>
    public void Flip()
    {
        transform.Rotate(Vector3.up, 180);

        // Rotates the sprite transform in X axis to match the camera orientation
        sprite.transform.rotation = Quaternion.Euler(Camera.main.transform.eulerAngles.x, sprite.transform.eulerAngles.y, sprite.transform.eulerAngles.z);

        isFacingRight = !isFacingRight;
    }

    #region Throwable Object
    /// <summary>
    /// Drop the weared throwable.
    /// </summary>
    public virtual void DropObject()
    {
        // Drop it
    }

    /// <summary>
    /// Try to grab a throwable.
    /// When grabbed, the object follows the character and can be thrown by this one.
    /// </summary>
    /// <param name="_throwable">Throwable to try to grab.</param>
    /// <returns>Returns true if the throwable was successfully grabbed, false either.</returns>
    public virtual bool GrabObject(TDS_Throwable _throwable)
    {
        return true;
    }

    /// <summary>
    /// Throws the weared throwable.
    /// </summary>
    /// <param name="_targetPosition">Position where the object should land</param>
    public virtual void ThrowObject(Vector3 _targetPosition)
    {
        // Throw it
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
            if (!hitBox) Debug.LogWarning("The Character " + name + " HitBox is missing !");
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
