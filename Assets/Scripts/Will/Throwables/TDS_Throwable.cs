using System;
using UnityEngine;

#pragma warning disable 0414
public class TDS_Throwable : TDS_Object
{
    /* TDS_Throwable :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Throwable object behavior]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :	12/02/2019
	 *	Author :William COMMINGES
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Event called when the object is destroyed.
    /// </summary>
    public event Action OnDestroyed = null;
    #endregion

    #region Fields / Properties
    /// <summary>
    /// Owner of the throwable, that is the last character who had it in the hands.
    /// </summary>
    [Header("Owner"), SerializeField] protected TDS_Character owner = null;

    public TDS_Character Owner { get { return owner; } }

    /// <summary>
    /// Attack scriptable object used by this throwable to deal damages.
    /// </summary>
    [Header("Components & References")]
    [SerializeField] protected TDS_Attack attack = null;

    /// <summary>
    /// Accessor for this throwable attack effect type. Returns <see cref="AttackEffectType.None"/> of the object <see cref="attack"/> is null.
    /// </summary>
    public AttackEffectType ThrowableAttackEffectType
    {
        get
        {
            if (!attack) return AttackEffectType.None;
            return attack.Effect.EffectType;
        }
    }

    /// <summary>
    /// Sound to play when this object hits something.
    /// </summary>
    [SerializeField] protected AudioClip hitSound = null;

    /// <summary>
    /// Collider of the object.
    /// </summary>
    [SerializeField] protected new BoxCollider collider = null;

    /// <summary>
    /// Shadow of the object.
    /// </summary>
    [SerializeField] protected GameObject shadow = null;

    /// <summary>
    /// Hitbox of the throwable, used to to deal damages.
    /// </summary>
    [SerializeField] protected TDS_HitBox hitBox;

    /// <summary>Public accessor for <see cref="hitBox"/>.</summary>
    public TDS_HitBox HitBox { get { return hitBox; } }

    /// <summary>
    /// FX used for feedback purpose.
    /// </summary>
    [SerializeField] private GameObject feedbackFX;

    /// <summary>
    /// Rigidbody of the object.
    /// </summary>
    [SerializeField] protected new Rigidbody rigidbody = null;

    /// <summary>
    /// SpriteRenderer of the throwable.
    /// </summary>
    [SerializeField] protected SpriteRenderer sprite = null;

    /// <summary>Public accessor for <see cref="sprite"/>.</summary>
    public SpriteRenderer Sprite { get { return sprite; } }


    [Header("Settings")]
    /// <summary>
    /// Indicates if the throwable is destroyed or not.
    /// </summary>
    private bool isDestroyed = false;

    /// <summary>
    /// Indicates if the throwable is currently held by someone.
    /// </summary>
    [SerializeField] protected bool isHeld = false;
    public bool IsHeld { get { return isHeld; } }

    /// <summary>
    /// Bounce power used to make the object bounce on object hit.
    /// </summary>
    [SerializeField] protected float bouncePower = .5f;

    /// <summary>
    /// Bonus damages of the throwable.
    /// </summary>
    [SerializeField, Range(0, 20)] protected int bonusDamage = 0;

    /// <summary>
    /// Durability of the object.
    /// </summary>
    public int ObjectDurability = 2;

    /// <summary>
    /// Weight of the object.
    /// </summary>
    [SerializeField] protected int weight = 2;

    /// <summary>Public accessor for <see cref="weight"/>.</summary>
    public int Weight { get { return weight; } }
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activate this object hitbox.
    /// </summary>
    /// <param name="_bonusDamages">Bonus damages of the attack.</param>
    protected virtual void ActivateHitbox(int _bonusDamages)
    {
        if (hitBox.IsActive) hitBox.Desactivate();

        Tags _hitableTags = owner.HitBox.HittableTags;
        if (owner is TDS_Enemy) _hitableTags.AddTag("Enemy");
        hitBox.BonusDamages = bonusDamage + _bonusDamages;
        hitBox.Activate(attack, owner, _hitableTags.ObjectTags);
    }

    /// <summary>
    /// Bonce the object when hitting something.
    /// </summary>
    protected virtual void BounceObject()
    {
        rigidbody.velocity *= bouncePower * -1;

        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "BounceObject"), new object[] { transform.position.x, transform.position.y, transform.position.z, rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z });
    }

    /// <summary>
    /// Bonce the object when hitting something.
    /// </summary>
    /// <param name="_xPos">X position of the object.</param>
    /// <param name="_yPos">Y position of the object.</param>
    /// <param name="_zPos">Z position of the object.</param>
    /// <param name="_xVelo">X velocity of the object rigidbody.</param>
    /// <param name="_yVelo">Y velocity of the object rigidbody.</param>
    /// <param name="_zVelo">Z velocity of the object rigidbody.</param>
    protected virtual void BounceObject(float _xPos, float _yPos, float _zPos, float _xVelo, float _yVelo, float _zVelo)
    {
        transform.position = new Vector3(_xPos, _yPos, _zPos);
        rigidbody.velocity = new Vector3(_xVelo, _yVelo, _zVelo);
    }

    protected void CallDestroyEvent() => OnDestroyed?.Invoke();

    /// <summary>
    /// Destroy the gameObject Throwable if the durability is less or equal to zero 
    /// </summary>
    protected virtual void DestroyThrowableObject()
    {
        if (isDestroyed || !PhotonNetwork.isMasterClient) return;

        OnDestroyed?.Invoke();

        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "DestroyFeedback"), new object[] { });

        DestroyFeedback();
        Invoke("Destroy", 2);
    }

    /// <summary>
    /// Plays feedback for when the throwable gets destroyed.
    /// </summary>
    protected virtual void DestroyFeedback()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        rigidbody.isKinematic = true;
        collider.enabled = false;
        sprite.enabled = false;
        shadow.SetActive(false);

        TDS_SoundManager.Instance.PlayEffectSound(TDS_GameManager.AudioAsset.S_MagicPoof, audioSource);

        if (!feedbackFX) return;

        feedbackFX.gameObject.SetActive(true);
    }

    /// <summary>
    /// Drop the object from the character who was carring it. 
    /// </summary>
    public virtual void Drop()
    {
        if (!isHeld) return;

        if (owner.photonView.isMine)
        {
            // Drop the throwable for other players
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "Drop"), new object[] { transform.position.x, transform.position.y, transform.position.z });
        }

        rigidbody.isKinematic = false;
        collider.enabled = true;
        isHeld = false;

        owner.RemoveThrowable();
        SetIndependant();
    }

    /// <summary>
    /// Drop the object from the character who was carring it. 
    /// </summary>
    /// <param name="_x">X position where to drop it from.</param>
    /// <param name="_y">Y position where to drop it from.</param>
    /// <param name="_z">Z position where to drop it from.</param>
    protected virtual void Drop(float _x, float _y, float _z)
    {
        transform.position = new Vector3(_x, _y, _z);
        Drop();
    }

    /// <summary> 
    /// Reduces the durability of the object and if the durability is lower or equal to zero called the method that destroys the object. 
    /// </summary> 
    /// <param name="_valueToWithdraw"></param> 
    protected virtual void LoseDurability()
    {
        ObjectDurability --;
        if (ObjectDurability <= 0) DestroyThrowableObject();
    }

    /// <summary>
    /// Method called when this throwable hitbox hit something.
    /// </summary>
    protected virtual void OnHitSomething()
    {
        if (PhotonNetwork.isMasterClient) BounceObject();
        ResetThrowable();

        // Play sound
        TDS_SoundManager.Instance.PlayEffectSound(hitSound, audioSource);
    }

    /// <summary> 
    /// Let a character pickup the object.
    /// </summary> 
    /// <param name="_owner">Character attempting to pick up the object.</param> 
    /// <returns>Returns true is successfully picked up the object, false if a issue has been encountered.</returns> 
    public virtual bool PickUp(TDS_Character _owner)
    {
        if (isHeld) return false;

        isHeld = true;
        owner = _owner;
        owner.SetThrowable(this);

        if (hitBox.IsActive) hitBox.Desactivate();

        if (PhotonNetwork.isMasterClient)
        {
            // Pickup the throwable for other players
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "PickUp"), new object[] { _owner.PhotonID });
        }

        gameObject.layer = _owner.gameObject.layer;

        if (shadow && !shadow.activeInHierarchy) shadow.SetActive(true);
        rigidbody.isKinematic = true;
        collider.enabled = false;

        // Play sound
        TDS_SoundManager.Instance.PlayEffectSound(TDS_GameManager.AudioAsset.S_Pickup, audioSource);

        return true;
    }

    /// <summary> 
    /// Let a character pickup the object.
    /// </summary> 
    /// <param name="_owner">Photon ID of the character attempting to pick up the object.</param> 
    protected virtual void PickUp(int _ownerID)
    {
        PhotonView _photonView = PhotonView.Find(_ownerID);
        if (!photonView) return;

        PickUp(_photonView.GetComponent<TDS_Character>());
    }

    /// <summary>
    /// Resets this throwable parameters, when hitting something or colliding after a throw.
    /// </summary>
    protected virtual void ResetThrowable()
    {
        SetIndependant();

        if (PhotonNetwork.isMasterClient)
        {
            LoseDurability();

            if (!isDestroyed) TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "LoseDurability"), new object[] { });
            
        }
        else
        {
            if (ObjectDurability == 1) DestroyFeedback();
        }
    }

    /// <summary>
    /// Set throwable independant by nullifying owner and getting back on Object layer.
    /// </summary>
    protected virtual void SetIndependant()
    {
        if (hitBox.IsActive) hitBox.Desactivate();
        if (!owner) return;

        owner = null;
        gameObject.layer = LayerMask.NameToLayer("Object");
    }

    /// <summary> 
    /// Throws the object to a given position by converting the final position to velocity.
    /// </summary> 
    /// <param name="_finalPosition">Final position where the object is supposed to be at the end of the trajectory.</param> 
    /// <param name="_angle">Throw angle.</param> 
    /// <param name="_bonusDamage">Bonus damages added to the attack.</param> 
    public virtual void Throw(Vector3 _finalPosition, float _angle, int _bonusDamage)
    {
        if (!isHeld) return;

        if (owner.photonView.isMine)
        {
            // Throw the throwable for other players
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "Throw"), new object[] { transform.position.x, transform.position.y, transform.position.z, _finalPosition.x, _finalPosition.y, _finalPosition.z, _angle, _bonusDamage });
        }

        ActivateHitbox(_bonusDamage);

        owner.RemoveThrowable();

        rigidbody.isKinematic = false;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position,_finalPosition,_angle);

        collider.enabled = true;
        isHeld = false;

        // Play sound
        TDS_SoundManager.Instance.PlayEffectSound(TDS_GameManager.AudioAsset.S_Throw, audioSource);
    }

    /// <summary>
    /// Throws the object to a given position by converting the final position to velocity.
    /// </summary>
    /// <param name="_xFrom">X position from where the object is thrown.</param>
    /// <param name="_yFrom">Y position from where the object is thrown.</param>
    /// <param name="_zFrom">Z position from where the object is thrown.</param>
    /// <param name="_xTo">X position where to throw the object.</param>
    /// <param name="_yTo">Y position where to throw the object.</param>
    /// <param name="_zTo">Z position where to throw the object.</param>
    /// <param name="_angle">Throw angle.</param> 
    /// <param name="_bonusDamage">Bonus damages added to the attack.</param>
    protected virtual void Throw(float _xFrom, float _yFrom, float _zFrom, float _xTo, float _yTo, float _zTo, float _angle, int _bonusDamage)
    {
        transform.position = new Vector3(_xFrom, _yFrom, _zFrom);
        Throw(new Vector3(_xTo, _yTo, _zTo), _angle, _bonusDamage);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Get missing references
        if (!rigidbody) rigidbody = GetComponentInChildren<Rigidbody>();
        if (!collider) collider = GetComponentInChildren<BoxCollider>();
        if (!hitBox) hitBox = GetComponentInChildren<TDS_HitBox>();
        if (!shadow) shadow = transform.GetChild(0).GetChild(1).gameObject;
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();

        // Set event on hitbox hit
        hitBox.OnTouch += OnHitSomething;
    }

    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (Time.timeSinceLevelLoad > .1f)
        {
            if (hitBox.IsActive)
            {
                ResetThrowable();
            }

            // Play sound
            TDS_SoundManager.Instance.PlayEffectSound(TDS_GameManager.AudioAsset.S_Drop, audioSource);
        }
    }

    private void OnDestroy()
    {
        if (owner && PhotonNetwork.connected && gameObject.activeInHierarchy && !isDestroyed)
        {
            owner.DropObject();
        }
        if (hitBox.IsActive) hitBox.Desactivate();
    }
    #endregion

    #endregion
}
