using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_ExplosiveThrowable : TDS_Throwable 
{
    /* TDS_ExplosiveThrowable :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Throwable that explodes after a delay]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[20/05/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the class]
	 *  - Initialisation of the Overriden methods of the throwable class
     *  - Initalise the explosion settings
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    [Header("Explosion Settings")]
    [SerializeField, Range(1,60)] private float explodingDelay = 5;

    [SerializeField] private ParticleSystem explosionParticles = null;

    [SerializeField] private Animator animator = null; 

    private int speedStatesCount = 4; 

    private Coroutine explosionCoroutine = null; 
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Setup the explosion 
    /// Wait a delay method before playing the particles and activate the Hitbox
    /// Then wait and destroy the Throwable
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetupExplosion()
    {
        float _timer = 0;
        int _speedState = 0;
        float _animationSpeed = 0;  
        while (_timer < explodingDelay)
        {
            Debug.Log(_animationSpeed); 
            _timer += Time.deltaTime;
            if (_speedState == 0)
            {
                _speedState++;
                _animationSpeed = explodingDelay / speedStatesCount * _speedState;
                if (animator) animator.SetFloat("speedMultiplier", _animationSpeed); 
                yield return new WaitForEndOfFrame();
                continue; 
            }
            if(_timer > explodingDelay / speedStatesCount * _speedState)
            {
                _speedState++;
                _animationSpeed = explodingDelay / speedStatesCount * _speedState;
                if (animator) animator.SetFloat("speedMultiplier", _animationSpeed);
            }
            yield return new WaitForEndOfFrame();
        }
        if (isHeld) owner.DropObject();  
        if (explosionParticles) explosionParticles.Play();
        hitBox.Activate(attack);
        if (explosionParticles)
            yield return new WaitForSeconds(explosionParticles.main.duration);
        else yield return new WaitForSeconds(1);
        DestroyThrowableObject(); 
    }
    #endregion

    #region Overriden Methods
    /// <summary>
    /// Desactivate the hitbox and destroy the object
    /// </summary>
    protected override void DestroyThrowableObject()
    {
        hitBox.Desactivate();
        if (isHeld) owner.DropObject(); 
        base.DestroyThrowableObject();
    }

    /// <summary>
    /// The Throwable don't loose durability when it's exploding
    /// </summary>
    protected override void LoseDurability()
    {
        //No durability
    }

    /// <summary>
    /// Modify the Throwable Position as in <see cref="TDS_Throwable.Throw(Vector3, float, int)"/> 
    /// But don't Activate the hitbox
    /// </summary>
    /// <param name="_finalPosition"></param>
    /// <param name="_angle"></param>
    /// <param name="_bonusDamage"></param>
    public override void Throw(Vector3 _finalPosition, float _angle, int _bonusDamage)
    {
        if (!isHeld) return;
        if (hitBox.IsActive)
        {
            hitBox.Desactivate();
        }
        rigidbody.isKinematic = false;
        transform.SetParent(null, true);
        bonusDamage = _bonusDamage;
        rigidbody.velocity = TDS_ThrowUtility.GetProjectileVelocityAsVector3(transform.position, _finalPosition, _angle);
        Tags _hitableTags = owner.HitBox.HittableTags;
        if (owner is TDS_Enemy)
        {
            _hitableTags.AddTag("Enemy");
        }
        isHeld = false;
        if(explosionCoroutine == null)
        {
            explosionCoroutine = StartCoroutine(SetupExplosion());
        }
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        //base.Awake(); 
        if (!hitBox)
        {
            hitBox = GetComponentInChildren<TDS_HitBox>();
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnCollisionEnter(Collision other)
    {
        return; 
    }

    protected override void OnTriggerEnter(Collider other)
    {
        return;
    }
    #endregion

    #endregion
}
