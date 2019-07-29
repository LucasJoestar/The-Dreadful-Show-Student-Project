using UnityEngine;
using Photon;

#pragma warning disable 0414
[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class TDS_Throwable : PunBehaviour
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

    #region Fields / Properties
    /// <summary>
    /// Owner of the throwable, that is the last character who had it in the hands.
    /// </summary>
    [Header("Owner"), SerializeField] protected TDS_Character owner = null;

    /// <summary>
    /// Attack scriptable object used by this throwable to deal damages.
    /// </summary>
    [Header("Components & References")]
    [SerializeField] protected TDS_Attack attack = null;

    /// <summary>
    /// Accessor for this throwable attack effect type. Returns <see cref="AttackEffectType.None"/> of the object <see cref="attack"/> is null.
    /// </summary>
    public AttackEffectType ThrowableAttackType
    {
        get
        {
            if (!attack) return AttackEffectType.None;
            return attack.Effect.EffectType;
        }
    }

    /// <summary>
    /// Collider of the object.
    /// </summary>
    [SerializeField] protected new BoxCollider collider = null;

    /// <summary>
    /// Shadow of the object.
    /// </summary>
    [SerializeField] private GameObject shadow = null;

    /// <summary>
    /// Hitbox of the throwable, used to to deal damages.
    /// </summary>
    [SerializeField] protected TDS_HitBox hitBox;

    /// <summary>Public accessor for <see cref="hitBox"/>.</summary>
    public TDS_HitBox HitBox { get { return hitBox; } }

    /// <summary>
    /// Rigidbody of the object.
    /// </summary>
    [SerializeField] protected new Rigidbody rigidbody = null;

    /// <summary>
    /// Indicates if the throwable is currently held by someone.
    /// </summary>
    [Header("Settings")]
    [SerializeField] protected bool isHeld = false;

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
        // Bounce the object from other players.
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "BounceObject"), new object[] { transform.position.x, transform.position.y, transform.position.z, rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z });

        rigidbody.velocity *= bouncePower * -1;
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
        rigidbody.velocity = new Vector3(_xVelo, _yVelo, _zVelo) * bouncePower * -1;
    }

    /// <summary>
    /// Destroy the gameObject Throwable if the durability is less or equal to zero 
    /// </summary>
    protected virtual void DestroyThrowableObject()
    {
        TDS_VFXManager.Instance.SpawnEffect(FXType.MagicDisappear, transform.position);
        PhotonNetwork.Destroy(gameObject);
    }

    /// <summary>
    /// Drop the object from the character who was carring it. 
    /// </summary>
    public virtual void Drop()
    {
        if (!isHeld) return;

        if (photonView.isMine)
        {
            // Drop the throwable for other players
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "Drop"), new object[] { transform.position.x, transform.position.y, transform.position.z });
        }

        if (PhotonNetwork.isMasterClient)
        {
            // Transfer ownership
            photonView.TransferOwnership(PhotonNetwork.masterClient);
        }

        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        collider.enabled = true;
        isHeld = false;

        owner.RemoveThrowable();
        owner = null;

        gameObject.layer = LayerMask.NameToLayer("Object");
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
        hitBox.Desactivate();
        BounceObject();

        LoseDurability();
        owner = null;
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

        if (PhotonNetwork.isMasterClient)
        {
            if (hitBox.IsActive) hitBox.Desactivate();

            // Transfer ownership
            photonView.TransferOwnership(owner.photonView.owner);

            // Pickup the throwable for other players
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "PickUp"), new object[] { _owner.PhotonID });
        }

        gameObject.layer = _owner.gameObject.layer;

        if (shadow && !shadow.activeInHierarchy) shadow.SetActive(true);
        rigidbody.isKinematic = true;
        collider.enabled = false;

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
    /// Throws the object to a given position by converting the final position to velocity.
    /// </summary> 
    /// <param name="_finalPosition">Final position where the object is supposed to be at the end of the trajectory.</param> 
    /// <param name="_angle">Throw angle.</param> 
    /// <param name="_bonusDamage">Bonus damages added to the attack.</param> 
    public virtual void Throw(Vector3 _finalPosition, float _angle, int _bonusDamage)
    {
        if (!isHeld) return;

        if (photonView.isMine)
        {
            // Throw the throwable for other players
            TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "Throw"), new object[] { transform.position.x, transform.position.y, transform.position.z, _finalPosition.x, _finalPosition.y, _finalPosition.z, _angle, _bonusDamage });
        }

        if (PhotonNetwork.isMasterClient)
        {
            ActivateHitbox(_bonusDamage);

            // Transfer ownership
            photonView.TransferOwnership(PhotonNetwork.masterClient);
        }

        transform.SetParent(null, true);

        rigidbody.isKinematic = false;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position,_finalPosition,_angle);

        collider.enabled = true;
        isHeld = false;

        owner.RemoveThrowable();

        gameObject.layer = LayerMask.NameToLayer("Object");
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
    protected virtual void Awake()
    {
        // Get missing references
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
        if (!collider) collider = GetComponent<BoxCollider>();
        if (!hitBox) hitBox = GetComponentInChildren<TDS_HitBox>();
        if (!shadow) shadow = transform.GetChild(1).gameObject;

        // Set event on hitbox hit
        hitBox.OnTouch += OnHitSomething;
    }

    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
    protected virtual void OnCollisionEnter(Collision other)
    {
        if (hitBox.IsActive)
        {
            hitBox.Desactivate();
            LoseDurability();
            owner = null;
        }
    }
    #endregion

    #endregion
}
