using Photon;
using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Animator), typeof(PhotonView)), RequireComponent(typeof(PhotonTransformView))]
public class TDS_Damageable : PunBehaviour
{
    /* TDS_Damageable :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Base class to inherit from to create any object with an amount of health that can take damage in the game "The Dreadful Show".
     *	    
     *	    You can override inherited methods to implement specific behaviour on objects.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[21 / 03 / 2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
     *      Disable collider when died & enable it again when get resurrected.
     *      
	 *	-----------------------------------
     * 
     *  Date :			[21 / 02 / 2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
     *      - Adding the RequireComponent PhotonView.
     *      - Adding the RequireComponent PhotonTransformView.
     *      
	 *	-----------------------------------
     *	
     *	Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      - Modified the IsDead property.
     *      - Modified the debugs for component missing in Awake.
     *      
	 *	-----------------------------------
     *	
     *	Date :			[23 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      - Added the IsIndestructible property.
     *      - Modified the IsDead, & HealthCurrent properties.
     *      - Removed the Sprite property.
     *      
	 *	-----------------------------------
     *	
     *	Date :			[16 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      - Added the OnHeal event.
     *      - Added the Heal method.
     *      
	 *	-----------------------------------
     * 
	 *	Date :			[15 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Damageable class.
     *	
     *	    - Added the OnDie, OnHealthChanged and OnTakeDamage event.
     *	    - Added animator & collider fields ; isDead field & property ; IsIndestructible field, IsInvunlerable field ; healthCurrent field & property ; healthMax field & property ; and photonID property.
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
    /// Heal event.
    /// Called when the object is healed, with the amount as parameter
    /// </summary>
    public event Action<int> OnHeal = null;

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

    /// <summary>
    /// The sprite renderer used to render this object in the scene.
    /// </summary>
    [SerializeField] protected SpriteRenderer sprite = null;
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
            // When setting the value, check some parameters and set values if needed
            if (value == true)
            {
                // If the damageable is indestructible, it cannot be dead
                // So, return
                if (IsIndestructible) return;
                if (healthCurrent > 0)
                {
                    HealthCurrent = 0;
                    return;
                }
            }
            else if (healthCurrent == 0)
            {
                HealthCurrent = 1;
            }
            isDead = value;

            #if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying) return;
            else if (value == false)
            {
                animator.SetTrigger("REVIVE");
                IsInvulnerable = false;
            }
            else
            {
                IsInvulnerable = true;
            }
            #endif

            if (value == true)
            {
                OnDie?.Invoke();
                Die();
            }
            enabled = !value;
        }
    }

    /// <summary>Backing field for <see cref="IsIndestructible"/></summary>
    [SerializeField] protected bool isIndestructible = false;

    /// <summary>
    /// When indestructible, the damageable cannot be destroyed, even if its life goes below or equals zero.
    /// Useful for an never-end life punching ball.
    /// </summary>
    public bool IsIndestructible
    {
        get { return isIndestructible; }
        set
        {
            isIndestructible = value;
            if (healthCurrent == 0) IsDead = true;
        }
    }

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

            if (value == 0) IsDead = true;
            else if (isDead) IsDead = false;
        }
    }

    /// <summary>Backing field for <see cref="HealthMax"/></summary>
    [SerializeField] protected int healthMax = 1;

    /// <summary>
    /// The maximum health of the object.
    /// Its value cannot be less than or equal to zero.
    /// The current health of the object cannot exceed this value.
    /// </summary>
    public int HealthMax
    {
        get { return healthMax; }
        set
        {
            if (value <= 0) value = 1;
            healthMax = value;

            if (healthCurrent > value) HealthCurrent = value;
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

    protected PhotonView photonView;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected virtual void Die()
    {

    }

    /// <summary>
    /// Makes this object be healed and restore its health.
    /// </summary>
    /// <param name="_heal">Amount of health point to restore.</param>
    public virtual void Heal(int _heal)
    {
        HealthCurrent += _heal;
        OnHeal?.Invoke(_heal);
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public virtual bool TakeDamage(int _damage)
    {
        // Online
      if (photonView.isMine)
      {
          //if (!animator) return;
          TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.MasterClient, TDS_RPCManager.GetInfo(photonView, this.GetType(), "TakeDamage"),new object[] {(int)_damage });
      }

      // Local
      
        if (IsInvulnerable) return false;

        HealthCurrent -= _damage;
        OnTakeDamage?.Invoke(_damage);

        return true;
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <param name="_position">Position in world space from where the hit come from.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public virtual bool TakeDamage(int _damage, Vector3 _position)
    {
        return TakeDamage(_damage);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        // Try yo get components references of they are missing
        if (!animator)
        {
            animator = GetComponent<Animator>();
            if (!animator) Debug.LogWarning("The Animator of \"" + name + "\" for script TDS_Damageable is missing !");
        }
        if (!collider)
        {
            collider = GetComponent<BoxCollider>();
            if (!collider) Debug.LogWarning("The Collider of \"" + name + "\" for script TDS_Damageable is missing !");
        }
        if (!sprite)
        {
            sprite = GetComponent<SpriteRenderer>();
            if (!sprite) Debug.LogWarning("The SpriteRenderer of \"" + name + "\" for script TDS_Damageable is missing !");
        }
        if(!photonView)
        {
            photonView = GetComponent<PhotonView>();
            if (!photonView) Debug.LogWarning("The PhotonView of \"" + name + "\" for script TDS_Damageable is missing !");
        }
    }

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