using UnityEngine;
using System.Collections.Generic; 

#pragma warning disable 0414
[RequireComponent(typeof(Rigidbody))]
public class TDS_Throwable : MonoBehaviour 
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

    #endregion

    #region Fields / Properties
    #region ObjectSettings
    [SerializeField,Header("Object settings")]
    protected bool canBeTakeByEnemies = false;
    [SerializeField]
    protected bool isHeld = false;
    [SerializeField]
    protected bool isHoldByPlayer = false;
    [SerializeField]
    protected float bouncePower = .5f;
    [SerializeField]
    protected float objectSpeed = 15f;
    [SerializeField, Range(0, 20)]
    protected int bonusDamage = 0;
    [SerializeField,Range(0,10)]
    protected int durabilityToWithdraw = 2;
    [SerializeField, Range(0, 10)]
    protected int objectDurability = 10;
    [SerializeField]
    protected int weight = 2;
    public int Weight { get { return weight; } }
    [SerializeField]
    protected new Rigidbody rigidbody = null;    
    #endregion
    #region PlayerSettings
    [SerializeField, Header("Character settings")]       
    protected TDS_Character owner = null;
    #endregion
    #region Hitbox
    [SerializeField]
    protected TDS_HitBox hitBox;
    [SerializeField]
    protected TDS_Attack attack = new TDS_Attack();
    [SerializeField]
    protected LayerMask whatDesactivate = new LayerMask();
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// bounce object when it touches a collider
    /// </summary>
    protected void BounceObject()
    {
        rigidbody.velocity *= bouncePower*-1;
    }
    /// <summary>
    /// Destroy the gameObject Throwable if the durability is less or equal to zero 
    /// </summary>
    protected virtual void DestroyThrowableObject()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// Unparent the object from the character who was carring it. 
    /// </summary>
    public void Drop()
    {
        if (!isHeld) return;
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        isHeld = false;
    }
    /// <summary> 
    /// Reduces the durability of the object and if the durability is lower or equal to zero called the method that destroys the object. 
    /// </summary> 
    /// <param name="_valueToWithdraw"></param> 
    protected virtual void LoseDurability()
    {
        objectDurability -= durabilityToWithdraw;
        if (!(objectDurability <= 0)) return;
        DestroyThrowableObject();
    }

    /// <summary> 
    /// Picks up the object and parent it at the corresponding root 
    /// </summary> 
    /// <param name="_carrier"></param> 
    /// <param name="_rootCharacterObject"></param> 
    /// <returns></returns> 
    public bool PickUp(TDS_Character _carrier, Transform _rootCharacterObject)
    {
        if (isHeld) return false;
        //gameObject.layer = LayerMask.NameToLayer("Player");
        rigidbody.isKinematic = true;
        transform.position = _rootCharacterObject.transform.position;
        transform.SetParent(_rootCharacterObject.transform, true);
        isHeld = true;
        if(_carrier is TDS_Player)
        {
            isHoldByPlayer = true;
            gameObject.layer = _carrier.gameObject.layer;
        }
        owner = _carrier;
        return true;
    }

    /// <summary> 
    /// Throws the object to a given position by converting the final position to velocity 
    /// </summary> 
    /// <param name="_finalPosition"></param> 
    /// <param name="_angle"></param> 
    /// <param name="_bonusDamage"></param> 
    public virtual void Throw(Vector3 _finalPosition,float _angle, int _bonusDamage)
    {
        if (!isHeld) return;
        if(hitBox.IsActive)
        {
            hitBox.Desactivate();
        }
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        bonusDamage = _bonusDamage;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position,_finalPosition,_angle);
        Tags _hitableTags = owner.HitBox.HittableTags; 
        if (owner is TDS_Enemy)
        {
            _hitableTags.AddTag("Enemy");
        }
        hitBox.Activate(attack, owner, _hitableTags.ObjectTags);
        isHeld = false;
    }
    #endregion

    #region Unity Methods
    protected virtual void Awake()
    {
        if (!rigidbody) rigidbody = GetComponent<Rigidbody>();
        if (!hitBox)
        {
            hitBox = GetComponentInChildren<TDS_HitBox>();
        }
        hitBox.OnTouch += hitBox.Desactivate;
        hitBox.OnTouch += BounceObject;
        hitBox.OnTouch += LoseDurability;
        hitBox.OnTouch += () => owner = null;
    }

    protected virtual void OnCollisionEnter(Collision other)
    {
        hitBox.Desactivate();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if(whatDesactivate != (whatDesactivate | (1 << other.gameObject.layer))) return;
        hitBox.Desactivate();
    }

    protected virtual void Start ()
    {
    }
    

	#endregion
	#endregion
}