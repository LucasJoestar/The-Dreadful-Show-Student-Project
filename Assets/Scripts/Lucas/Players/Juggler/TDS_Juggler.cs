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
	 *	### MODIFICATIONS ###
	 *	#####################
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

    #endregion

    #region Methods

    #region Original Methods

    #region Attacks & Actions

    #region Aim & Throwables
    /// <summary>
    /// Method called in the Aim coroutine.
    /// </summary>
    protected override void AimMethod()
    {
        // Let the player aim the point he wants, 'cause the juggler can do that. Yep

        // Aim with IJKL or the right joystick axis

        // Raycast along the trajectory preview and stop the trail when hit something
        Vector3 _fromPos = handsTransform.localPosition + (throwable.transform.rotation * throwable.transform.localPosition);

        throwVelocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(_fromPos, throwAimingPoint, aimAngle);

        throwTrajectoryMotionPoints = TDS_ThrowUtility.GetThrowMotionPoints(_fromPos, throwAimingPoint, throwVelocity.magnitude, aimAngle, throwPreviewPrecision);

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
        _throwable.PickUp(this, handsTransform);
        Throwables.Add(_throwable);
        Throwable = _throwable;

        // Updates animator informations
        SetAnimHasObject(true);

        return true;
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
        throwable.Throw(_destinationPosition, aimAngle, RandomThrowBonusDamages);
        Throwables.Remove(throwable);
        SelectedThrowableIndex = selectedThrowableIndex;

        // Triggers the throw animation ;
        // If not having throwable anymore, update the animator
        SetAnimThrow();
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
        throwable.Throw(_targetPosition, aimAngle, RandomThrowBonusDamages);
        Throwables.Remove(throwable);
        SelectedThrowableIndex = selectedThrowableIndex;

        // Triggers the throw animation ;
        // If not having throwable anymore, update the animator
        SetAnimThrow();
        if (!throwable) SetAnimHasObject(false);
    }

    /// <summary>
    /// Make the Juggler juggle ! Yeeeaah !
    /// </summary>
    private void Juggle()
    {
        // If not having any throwable, return
        if (CurrentThrowableAmount == 0) return;

        float _baseTheta = 2 * Mathf.PI / CurrentThrowableAmount;

        for (int _i = 0; _i < CurrentThrowableAmount; _i++)
        {
            // Create variables
            TDS_Throwable _throwable = Throwables[_i];

            float _theta = _baseTheta * _i;
            Vector3 _newPosition = new Vector3(Mathf.Sin(_theta), Mathf.Cos(_theta), 0f) * 1;

            // Position update
            _throwable.transform.localPosition = Vector3.Lerp(_throwable.transform.localPosition, _newPosition, Time.deltaTime * 10);

            // Rotates the object
            _throwable.transform.rotation = Quaternion.Lerp(_throwable.transform.rotation, Quaternion.Euler(_throwable.transform.rotation.eulerAngles + Vector3.forward), Time.deltaTime * 100);
        }

        // Rotates the hands transform to make all objects rotate
        handsTransform.Rotate(Vector3.forward, Time.deltaTime * 100);
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
