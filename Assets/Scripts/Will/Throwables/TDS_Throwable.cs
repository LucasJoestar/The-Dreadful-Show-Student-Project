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
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
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
    float objectSpeed = 15f;
    [SerializeField, Range(0, 20)]
    int bonusDamage = 0;
    [SerializeField,Range(0,10)]
    int objectDurability = 10;
    [SerializeField]
    new Rigidbody rigidbody = null;    
    #endregion
    #region PlayerSettings
    [SerializeField, Header("Character settings")]       
    TDS_Character owner = null;
    #endregion
    #endregion

    #region Methods
    #region Original Methods
    void DestroyThrowableObject()
    {
        Destroy(gameObject);
    }
    public void Drop()
    {
        if (!isHeld) return;
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        isHeld = false;
    }
    public void LoseDurability(int _valueToWithdraw)
    {
        objectDurability -= _valueToWithdraw;
        if (!(objectDurability <= 0)) return;
        DestroyThrowableObject();
    }
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
    public void Throw(Vector3 _finalPosition,float _angle, int _bonusDamage)
    {
        if (!isHeld) return;
        gameObject.layer = LayerMask.NameToLayer("Object");
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        bonusDamage = _bonusDamage;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position,_finalPosition,_angle);        
        owner = null;
        isHeld = false;
    }    
	#endregion

	#region Unity Methods
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