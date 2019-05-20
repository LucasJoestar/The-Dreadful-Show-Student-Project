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
	 *	[PURPOSE]
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
    [Header("Explosion Settings")]
    [SerializeField, Range(1,60)] private float explodingDelay = 5;

    [SerializeField] private ParticleSystem explosionParticles = null; 
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
        yield return new WaitForSeconds(explodingDelay);
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
        StartCoroutine(SetupExplosion());
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
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
