using UnityEngine;

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
    bool canBeTakeByEnemies = false;
    [SerializeField]
    bool isHeld = false;
    [SerializeField]
    bool isHoldByPlayer = false;
    [SerializeField]
    float bouncePower = .5f;
    [SerializeField]
    float objectSpeed = 15f;
    [SerializeField, Range(0, 20)]
    int bonusDamage = 0;
    [SerializeField,Range(0,10)]
    int durabilityToWithdraw = 2;
    [SerializeField, Range(0, 10)]
    int objectDurability = 10;
    [SerializeField]
    int weight = 2;
    public int Weight { get { return weight; } }
    [SerializeField]
    new Rigidbody rigidbody = null;    
    #endregion
    #region PlayerSettings
    [SerializeField, Header("Character settings")]       
    TDS_Character owner = null;
    #endregion
    #region Hitbox
    [SerializeField]
    TDS_HitBox hitBox;
    [SerializeField]
    TDS_Attack attack = new TDS_Attack();
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    /// <summary>
    /// bounce object when it touches a collider
    /// </summary>
    void BounceObject()
    {
        rigidbody.velocity *= bouncePower*-1;
    }
    /// <summary>
    /// Destroy the gameObject Throwable if the durability is less or equal to zero 
    /// </summary>
    void DestroyThrowableObject()
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
    void LoseDurability()
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

        gameObject.layer = LayerMask.NameToLayer("Player");
        rigidbody.isKinematic = true;
        transform.position = _rootCharacterObject.transform.position;
        transform.SetParent(_rootCharacterObject.transform, true);
        isHeld = true;
        
        if(_carrier is TDS_Player)
        {
            isHoldByPlayer = true;
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
    public void Throw(Vector3 _finalPosition,float _angle, int _bonusDamage)
    {
        if (!isHeld) return;
        if(hitBox.IsActive)
        {
            hitBox.Desactivate();
        }
        gameObject.layer = LayerMask.NameToLayer("Object");
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        bonusDamage = _bonusDamage;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position,_finalPosition,_angle);
        hitBox.Activate(attack);
        hitBox.OnTouch += hitBox.Desactivate;
        hitBox.OnTouch += BounceObject;
        hitBox.OnTouch += LoseDurability;
       owner = null;
        isHeld = false;
    }
    #endregion

    #region Unity Methods
    void Awake()
    {
        if(!hitBox)
        {
            hitBox = GetComponentInChildren<TDS_HitBox>();
        }
    }
    void Start ()
    {
        if(!rigidbody) rigidbody = GetComponent<Rigidbody>();
    }	
	void Update ()
    {
        
	}
	#endregion
	#endregion
}