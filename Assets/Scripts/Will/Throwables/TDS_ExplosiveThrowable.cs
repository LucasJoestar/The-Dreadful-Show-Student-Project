using System.Collections;
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

    #region Animator
    private static readonly int speedMultiplier_Hash = Animator.StringToHash("speedMultiplier");
    #endregion

    [Header("Explosion Settings")]
    [SerializeField, Range(1,60)] private float explodingDelay = 5;

    [SerializeField] private Animator animator = null;

    [SerializeField, Range(.5f, 1f)] private float animationSpeedMin = .5f;
    [SerializeField, Range(1,5)] private float animationSpeedMax = 2;

    private bool hasHit = false;
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
        float _timer = 0.1f;

        float _animationSpeed = animationSpeedMin; 

        while (_timer < explodingDelay)
        {
            _timer += Time.deltaTime; 
            _animationSpeed = Mathf.MoveTowards(_animationSpeed, animationSpeedMax, (Time.deltaTime * _timer));
            animator.SetFloat(speedMultiplier_Hash, _animationSpeed); 
            yield return new WaitForEndOfFrame();
        }
        TDS_VFXManager.Instance.SpawnEffect(FXType.Kaboom, transform.position + (Vector3.up * 1.75f));

        collider.enabled = false;
        hitBox.Activate(attack, owner);
        DestroyThrowableObject(1); 
    }
    #endregion

    #region Overriden Methods
    protected override void ActivateHitbox(int _bonusDamages)
    {
        if (hitBox.IsActive) hitBox.Desactivate();

        hitBox.BonusDamages = bonusDamage + _bonusDamages;
        hitBox.Activate(attack, owner);
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
        base.Throw(_finalPosition, _angle, _bonusDamage);

        if (hitBox.IsActive)
            hitBox.Desactivate();
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

    protected override void OnCollisionEnter(Collision other)
    {
        if (!hasHit)
        {
            hasHit = true;
            audioSource.Play();

            if (PhotonNetwork.isMasterClient) explosionCoroutine = StartCoroutine(SetupExplosion());
        }

        // Play sound
        TDS_SoundManager.Instance.PlayEffectSound(TDS_GameManager.AudioAsset.S_Drop, audioSource);
        return; 
    }
    #endregion

    #endregion
}
