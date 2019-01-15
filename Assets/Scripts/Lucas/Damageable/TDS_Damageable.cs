using Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_Damageable : PunBehaviour 
{
    /* TDS_Damageable :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Base class to inherit from to create any object with an amount of health that can take damage in the game "The Dreadful Show".
     *	    You can override inherited methods to implement specific behaviour on objects.
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
	 *	Creation of the TDS_Damageable class.
     *	    - Added the OnDie, OnHealthChanged and OnTakeDamage event.
     *	    - Added animator & collider fields, isDead field & property, IsIndestructible field, IsInvunlerable field, healthCurrent field & property, healthMax field & property, and photonID property.
     *	    - Added the Die and TakeDamage methods.
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Die event.
    /// Called when this object just die, meaning when it is not indestructible and its health reach zero.
    /// </summary>
    public event Action OnDie = null;

    /// <summary>
    /// Health changed event.
    /// Called when this object health changed, with its new value as parameter
    /// </summary>
    public event Action<int> OnHealthChanged = null;

    /// <summary>
    /// Take damage event.
    /// Called when this object take any damage, with the amount as parameter
    /// </summary>
    public event Action<int> OnTakeDamage = null;
    #endregion

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// The animator of this object.
    /// </summary>
    [SerializeField] protected Animator animator = null;

    /// <summary>
    /// The main non-trigger collider of this object, used to detect collisions.
    /// </summary>
    [SerializeField] protected new BoxCollider collider = null;
    #endregion

    #region Variables
    /// <summary>Backing field for <see cref="isDead"/></summary>
    [SerializeField] protected bool isDead = false;

    /// <summary>
    /// Is this damageable dead or still alive ?
    /// (Dies when its health goes below or equals zero.)
    /// </summary>
    public bool IsDead
    {
        get { return isDead; }
        set
        {
            // If the damageable is indestructible, it cannot be dead
            // So, return
            if (IsIndestructible && value == true) return;

            animator?.SetBool("IsDead", value);
            if (value == true)
            {
                OnDie?.Invoke();
                Die();
            }
        }
    }

    /// <summary>
    /// When indestructible, the damageable cannot be destroyed, even if its life goes below or equals zero.
    /// Useful for an never-end life punching ball.
    /// </summary>
    public bool IsIndestructible = false;

    /// <summary>
    /// When invulnerable, the object cannot take any damage and its health will not decrease.
    /// The object will then just ignore any assault.
    /// </summary>
    public bool IsInvulnerable = false;

    /// <summary>Backing field for <see cref="HealthCurrent"/></summary>
    [SerializeField] protected int healthCurrent = 0;

    /// <summary>
    /// The current health of the object.
    /// Its value cannot be less than zero or exceed <see cref="HealthMax"/>.
    /// </summary>
    public int HealthCurrent
    {
        get { return healthCurrent; }
        set
        {
            value = Mathf.Clamp(value, 0, healthMax);
            healthCurrent = value;

            OnHealthChanged?.Invoke(value);
        }
    }

    /// <summary>Backing field for <see cref="HealthMax"/></summary>
    [SerializeField] protected int healthMax = 0;

    /// <summary>
    /// The maximum health of the object.
    /// Its value cannot be less than or equal to zero.
    /// The current health of the object cannot exceed this value.
    /// </summary>
    public int HealthMax
    {
        get { return HealthMax; }
        set
        {
            if (value <= 0) value = 1;
            healthMax = value;
        }
    }
    #endregion

    #region Photon
    /// <summary>
    /// Get the view ID of this object photon view.
    /// </summary>
    public int PhotonID
    {
        get { return photonView.viewID; }
    }
    #endregion

    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected virtual void Die() { }

    /// <summary>
    /// Make this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public virtual bool TakeDamage(int _damage)
    {
        if (IsInvulnerable) return false;

        HealthCurrent -= _damage;
        OnTakeDamage?.Invoke(_damage);

        return true;
    }
    #endregion

    #region Unity Methods
    // Use this for initialization
    protected virtual void Start ()
    {
        // Set the current health of the object to its maximum on start
        HealthCurrent = healthMax;
    }
	
	// Update is called once per frame
	protected virtual void Update ()
    {
        
	}
	#endregion
	#endregion
}
