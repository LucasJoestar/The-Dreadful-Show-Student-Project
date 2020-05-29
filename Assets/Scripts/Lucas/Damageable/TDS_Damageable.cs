﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider), typeof(Animator), typeof(PhotonTransformView))]
public abstract class TDS_Damageable : TDS_Object
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
    /// Die event.
    /// Called when this object just die, with the object as parameter.
    /// </summary>
    public static event Action<GameObject> OnDieWithObject = null;

    /// <summary>
    /// Heal event.
    /// Called when the object is healed, with the amount as parameter
    /// </summary>
    public event Action OnHeal = null;

    /// <summary>
    /// Health changed event.
    /// Called when this object health changed, with its new value as parameter
    /// </summary>
    public event Action OnHealthChanged = null;

    /// <summary>
    /// Event called when the damageable come back from the deads.
    /// </summary>
    public event Action OnRevive = null;

    /// <summary>
    /// Event called when stopping this damageable from bringing it closer.
    /// </summary>
    public event Action OnStopBringingCloser = null;

    /// <summary>
    /// Take damage event.
    /// Called when this object take any damage, with the amount as parameter
    /// </summary>
    public event Action OnTakeDamage = null;
    #endregion

    #region Fields / Properties

    #region Constants
    /// <summary>
    /// Speed movement when bringing closer something or someone.
    /// </summary>
    public const float BRING_CLOSER_SPEED = .1f;
    #endregion

    #region Components & References
    /// <summary>
    /// The animator of this object.
    /// </summary>
    [SerializeField] protected Animator animator = null;

    /// <summary>
    /// Actual object used to simulate the burn on this object. None if not burning.
    /// </summary>
    [SerializeField] protected Animator burnEffect = null;

    /// <summary>
    /// The main non-trigger collider of this object, used to detect collisions.
    /// </summary>
    [SerializeField] protected new BoxCollider collider = null;

    /// <summary>Public accessor for <see cref="collider"/>.</summary>
    public BoxCollider Collider { get { return collider; } }

    /// <summary>
    /// Rigidbody of this damageable.
    /// Mainly used to project this one in the air.
    /// </summary>
    [SerializeField] protected new Rigidbody rigidbody = null;

    /// <summary>
    /// The sprite renderer used to render this object in the scene.
    /// </summary>
    [SerializeField] protected SpriteRenderer sprite = null;
    #endregion

    #region Variables
    /// <summary>
    /// Indicates if this damageable can be moved by attack effects.
    /// </summary>
    [SerializeField] protected bool canBeMoved = true;

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
#endif

            if (value == false)
            {
                Revive();
            }
            else
            {
                Die();
            }
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
            value = Mathf.Clamp(value, isIndestructible ? 1 : 0, healthMax);
            healthCurrent = value;

            if (value == 0) IsDead = true;
            else if (isDead) IsDead = false;

            OnHealthChanged?.Invoke();
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

    /// <summary>
    /// Layer of this object just before its death.
    /// </summary>
    private int layerBeforeDeath = 0;
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine for the effect-related movement to bring the damageable closer.
    /// </summary>
    protected Coroutine bringingCloserCoroutine = null;

    /// <summary>
    /// Coroutine used for knockback effect, pushing the damageable backward.
    /// </summary>
    protected Coroutine knockbackCoroutine = null;

    /// <summary>
    /// Coroutines for the burning effect.
    /// </summary>
    protected Dictionary<int, Coroutine> burningCoroutines = new Dictionary<int, Coroutine>();
    #endregion

    #region Sound
    [SerializeField] protected string hitSoundEvent = string.Empty;

    [SerializeField] protected string deathSoundEvent = string.Empty;
    #endregion

    #region Animator
    private readonly int vanish_Hash = Animator.StringToHash("Vanish");
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Health
    /// <summary>
    /// Method called when the object dies.
    /// Override this to implement code for a specific object.
    /// </summary>
    protected virtual void Die()
    {
        // Stop effects
        if (PhotonNetwork.isMasterClient)
        {
            StopBurning();
        }
        if (photonView.isMine)
        {
            if (bringingCloserCoroutine != null) StopBringingCloser();
        }

        // Call events
        OnDie?.Invoke();
        OnDieWithObject?.Invoke(gameObject);

        // Change object layer to avoid problems
        layerBeforeDeath = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dead");

        // Play death sound
        if (!string.IsNullOrEmpty(deathSoundEvent))
            AkSoundEngine.PostEvent(deathSoundEvent, gameObject);
    }

    /// <summary>
    /// Makes this object be healed and restore its health.
    /// </summary>
    /// <param name="_heal">Amount of health point to restore.</param>
    public virtual void Heal(int _heal)
    {
        if (PhotonNetwork.isMasterClient)
        {
            HealthCurrent += _heal;
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "Heal", new object[] { healthCurrent });
        }
        else
        {
            HealthCurrent = _heal;
        }

        OnHeal?.Invoke();
    }

    /// <summary>
    /// Method called when this object gets back from the deads.
    /// </summary>
    protected virtual void Revive()
    {
        OnRevive?.Invoke();
        gameObject.layer = layerBeforeDeath;
    }

    protected void SetInvulnerable(bool _isInvulnerable)
    {
        if (PhotonNetwork.isMasterClient)
        {
            IsInvulnerable = _isInvulnerable;
        }
        else if (photonView.isMine)
        {
            IsInvulnerable = _isInvulnerable;

            TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, photonView, GetType(), "SetInvulnerable", new object[] { _isInvulnerable });
        }
    }

    /// <summary>
    /// Makes this object take damage and decrease its health if it is not invulnerable.
    /// </summary>
    /// <param name="_damage">Amount of damage this inflect to this object.</param>
    /// <returns>Returns true if some damages were inflicted, false if none.</returns>
    public virtual bool TakeDamage(int _damage)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (IsInvulnerable || isDead) return false;

            HealthCurrent -= _damage;
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "TakeDamage", new object[] { healthCurrent });
        }
        else
        {
            HealthCurrent = _damage;
        }
        
        OnTakeDamage?.Invoke();

        // Play hit sounds
        if (!string.IsNullOrEmpty(hitSoundEvent))
            AkSoundEngine.PostEvent(hitSoundEvent, gameObject);

        // Play feedback sounds
        AkSoundEngine.PostEvent("Play_FEEDBACK_HIT", gameObject);
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

    #region Effects

    #region Bring Closer
    /// <summary>
    /// Bring this damageable closer from a certain distance.
    /// </summary>
    /// <param name="_distance">Distance to browse.</param>
    public virtual bool BringCloser(float _distance)
    {
        if (!photonView.isMine)
        {
            TDS_RPCManager.Instance.CallRPC(photonView.owner, photonView, GetType(), "BringCloser", new object[] { _distance });
            return false;
        }

        // If can't be bringed closer, just return
        if (!canBeMoved) return false;

        if (knockbackCoroutine != null) StopKnockback();
        if (bringingCloserCoroutine != null) StopBringingCloser();

        bringingCloserCoroutine = StartCoroutine(BringingCloser(_distance));

        return true;
    }

    /// <summary>
    /// Bring the damageable closer across time.
    /// </summary>
    /// <param name="_distance">Distance to browse.</param>
    /// <returns>IEnumerator, baby.</returns>
    protected virtual IEnumerator BringingCloser(float _distance)
    {
        float _sign = Mathf.Sign(_distance);
        _distance = Mathf.Abs(_distance);

        while (_distance > .5f)
        {
            transform.position += Vector3.left * BRING_CLOSER_SPEED * _sign;
            _distance -= BRING_CLOSER_SPEED;
            yield return null;
        }
        yield return null;
        StopBringingCloser();
    }

    /// <summary>
    /// Method called when stopped being bringed closer.
    /// </summary>
    public virtual void StopBringingCloser()
    {
        if (bringingCloserCoroutine != null)
        {
            StopCoroutine(bringingCloserCoroutine);
            bringingCloserCoroutine = null;
        }

        if (!PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.MasterClient, photonView, GetType(), "CallOnStopBringingCloser", new object[] { });
        }

        OnStopBringingCloser?.Invoke();
    }

    protected void CallOnStopBringingCloser()
    {
        OnStopBringingCloser?.Invoke();
    }
    #endregion

    #region Burn
    /// <summary>
    /// Make this damageable burn.
    /// </summary>
    /// <param name="_damagesMin">Minimum damages of the flames.</param>
    /// <param name="_damagesMax">Maximum damages of the flames.</param>
    /// <param name="_duration">Duration of the burn.</param
    public virtual void Burn(int _damagesMin, int _damagesMax, float _duration)
    {
        int _id = 0;

        while (burningCoroutines.ContainsKey(_id))
        {
            _id = Random.Range(0, 999);
        }

        // If starting burning, instantiate a burn effect
        if (burningCoroutines.Count == 0) InstantiateFireEffect();

        burningCoroutines.Add(_id, StartCoroutine(Burning(_damagesMin, _damagesMax, _duration, _id)));
    }

    /// <summary>
    /// Burn the damageable across time.
    /// </summary>
    /// <param name="_damagesMin">Minimum damages of the flames.</param>
    /// <param name="_damagesMax">Maximum damages of the flames.</param>
    /// <param name="_duration">Duration of the burn.</param
    /// <returns>IEnumerator, baby.</returns>
    protected virtual IEnumerator Burning(int _damagesMin, int _damagesMax, float _duration, int _id)
    {
        _damagesMax++;

        while (_duration > 0)
        {
            float _wait = Random.Range(1f, 2.5f);
            yield return new WaitForSeconds(_wait);

            int _damages = Random.Range(_damagesMin, _damagesMax);
            TakeDamage(_damages);
            _duration -= _wait;
        }

        burningCoroutines[_id] = null;
        burningCoroutines.Remove(_id);

        if (burningCoroutines.Count == 0) DestroyFireEffect();
    }

    /// <summary>
    /// Destroys the fire effect object.
    /// </summary>
    protected virtual void DestroyFireEffect()
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "DestroyFireEffect", new object[] { });
        }

        burnEffect.SetTrigger(vanish_Hash);

        // Stop burn sound
        AkSoundEngine.PostEvent("Stop_BURNING", gameObject);
    }

    /// <summary>
    /// Instantiate the fire effect for burning.
    /// </summary>
    protected virtual void InstantiateFireEffect()
    {
        if (PhotonNetwork.isMasterClient)
        {
            TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "InstantiateFireEffect", new object[] { });
        }

        burnEffect = ((GameObject)Instantiate(Resources.Load("FireBurn"), new Vector3(transform.position.x, transform.position.y, transform.position.z - .05f), Quaternion.identity)).GetComponent<Animator>();

        burnEffect.transform.SetParent(transform, true);

        // Play burn effect
        AkSoundEngine.PostEvent("Play_BURNING", gameObject);
    }

    /// <summary>
    /// Stops this object from burning.
    /// </summary>
    protected virtual void StopBurning()
    {
        if (burningCoroutines.Count > 0)
        {
            foreach (KeyValuePair<int, Coroutine> _burning in burningCoroutines)
            {
                StopCoroutine(_burning.Value);
            }
            burningCoroutines.Clear();

            DestroyFireEffect();
        }
    }
    #endregion

    #region Knockback
    /// <summary>
    /// Apply knockback on this damageable.
    /// </summary>
    /// <param name="_toRight">Should the damageable be pushed to the right of left.</param>
    /// <returns>Returns true if successfully applied knockback on this damageable, false otherwise.</returns>
    public virtual bool Knockback(bool _toRight)
    {
        if (!photonView.isMine)
        {
            TDS_RPCManager.Instance.CallRPC(photonView.owner, photonView, GetType(), "Knockback", new object[] { _toRight });
            return false;
        }

        if (!canBeMoved || (bringingCloserCoroutine != null)) return false;

        knockbackCoroutine = StartCoroutine(KnockbackCoroutine(_toRight));
        return true;
    }

    /// <summary>
    /// Apply knockback on this damageable.
    /// </summary>
    /// <param name="_toRight">Should the damageable be pushed to the right of left.</param>
    /// <returns>Returns true if successfully applied knockback on this damageable, false otherwise.</returns>
    protected virtual IEnumerator KnockbackCoroutine(bool _toRight)
    {
        float _direction = _toRight.ToSign() * .05f;
        float _timer = 0;

        while (_timer < .15f)
        {
            transform.position = new Vector3(transform.position.x + _direction, transform.position.y, transform.position.z);

            yield return null;
            _timer += Time.deltaTime;
        }

        knockbackCoroutine = null;
    }

    /// <summary>
    /// Stops knockback.
    /// </summary>
    public virtual void StopKnockback()
    {
        if (knockbackCoroutine != null)
        {
            StopCoroutine(knockbackCoroutine);
            knockbackCoroutine = null;
        }
    }
    #endregion

    #region Project
    /// <summary>
    /// Project this damageable in the air.
    /// </summary>
    /// <param name="_toRight">Should the damageable be pushed to the right of left.</param>
    /// <returns>Returns true if successfully projected this damageable in the air, false otherwise.</returns>
    public virtual bool Project(bool _toRight)
    {
        if (!photonView.isMine)
        {
            TDS_RPCManager.Instance.CallRPC(photonView.owner, photonView, GetType(), "Project", new object[] { _toRight });
            return false;
        }

        if (!canBeMoved) return false;

        if (bringingCloserCoroutine != null) StopBringingCloser();

        return true;
    }
    #endregion

    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected virtual void Awake()
    {
        // Try yo get components references of they are missing
        if (!animator)
        {
            animator = GetComponent<Animator>();
        }
        if (!collider)
        {
            collider = GetComponent<BoxCollider>();
        }
        if (!sprite)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
    }

    protected virtual void OnDestroy()
    {
        AkSoundEngine.PostEvent("STOP_ALL", gameObject);
    }

    // Use this for initialization
    protected virtual void Start ()
    {
        if (!photonView.isMine && PhotonNetwork.connected)
        {
            if (rigidbody)
            {
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
            }
        }
    }
    #endregion

    #endregion
}
