using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonTransformView))]
public abstract class TDS_FleeingThrowable : TDS_Throwable
{
    /* TDS_ThrowableAnimal :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *
     *	Date :	
     *	Author :
     *
     *	Changes :
     *
     *	[CHANGES]
     *
     *	-----------------------------------
    */

    #region Fields / Properties

    #region Components & References
    /// <summary>
    /// Animator of the animal, used to play... animations.
    /// </summary>
    [Header("Animal Settings")]
    [SerializeField] protected Animator animator = null;

    /// <summary>
    /// Detector used to make the animal flee.
    /// </summary>
    [SerializeField] protected TDS_Detector detector = null;
    #endregion

    #region Variables
    /// <summary>
    /// Indicates if the animal is currently facing the right side of screen.
    /// </summary>
    [SerializeField] protected bool isFacingRight = true;

    /// <summary>
    /// Current coroutine used to make the animal flee.
    /// </summary>
    protected Coroutine fleeCoroutine = null;

    /// <summary>
    /// Delay before flee when detecting something (or someone) around.
    /// </summary>
    [SerializeField] protected float fleeDelay = .5f;
    #endregion

    #region Coroutines
    /// <summary>
    /// Coroutine used to make the throwable flee after being free by a drop or a throw.
    /// </summary>
    protected Coroutine fleeAfterFreeCoroutine = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #region Animal
    /// <summary>
    /// Makes the animal flee a certain collider.
    /// </summary>
    /// <param name="_collider">Collider to flee.</param>
    /// <returns></returns>
    protected abstract IEnumerator Flee(Collider _collider);

    /// <summary>
    /// After being free, wait to get velocity at zero for a certain amount of time, and then flee again.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator FleeAfterFree()
    {
        float _timer = 0;
        while (true)
        {
            yield return null;
            if (rigidbody.velocity.magnitude < .01f)
            {
                _timer += Time.deltaTime;

                if (_timer > fleeDelay) break;
            }
            else if (_timer > 0) _timer = 0;
        }

        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
        SetAnimationOnline(0);
        detector.gameObject.SetActive(true);
        fleeAfterFreeCoroutine = null;
    }

    /// <summary>
    /// Set animal animation.
    /// </summary>
    /// <param name="_animationID">ID of the new animation.</param>
    public virtual void SetAnimation(int _animationID)
    {
        animator.SetInteger("State", _animationID);
    }

    /// <summary>
    /// Set animal animation.
    /// </summary>
    /// <param name="_animationID">ID of the new animation.</param>
    public virtual void SetAnimationOnline(int _animationID)
    {
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SetAnimation"), new object[] { _animationID });

        SetAnimation(_animationID);
    }

    /// <summary>
    /// Starts making the animal flee after a delay.
    /// </summary>
    /// <param name="_collider">Danger collider around the animal.</param>
    protected virtual void StartFlee(Collider _collider)
    {
        if (fleeCoroutine != null) StopCoroutine(fleeCoroutine);
        fleeCoroutine = StartCoroutine(Flee(_collider));
    }
    #endregion

    #region Throwable
    /// <summary>
    /// Drop the object from the character who was carring it. 
    /// </summary>
    public override void Drop()
    {
        if (!isHeld) return;

        isFacingRight = owner.IsFacingRight;
        base.Drop();
    }

    /// <summary> 
    /// Let a character pickup the object.
    /// </summary> 
    /// <param name="_owner">Character attempting to pick up the object.</param> 
    /// <returns>Returns true is successfully picked up the object, false if a issue has been encountered.</returns> 
    public override bool PickUp(TDS_Character _owner)
    {
        if (!base.PickUp(_owner)) return false;

        detector.gameObject.SetActive(false);
        
        SetAnimation(2);

        if (fleeCoroutine != null)
        {
            StopCoroutine(fleeCoroutine);
            fleeCoroutine = null;
        }
        if (fleeAfterFreeCoroutine != null)
        {
            StopCoroutine(fleeAfterFreeCoroutine);
            fleeAfterFreeCoroutine = null;
        }

        isFacingRight = owner.IsFacingRight;
        return true;
    }

    /// <summary>
    /// Set throwable independant by nullifying owner and getting back on Object layer.
    /// </summary>
    protected override void SetIndependant()
    {
        base.SetIndependant();
        if (PhotonNetwork.isMasterClient) fleeAfterFreeCoroutine = StartCoroutine(FleeAfterFree());
    }

    /// <summary> 
    /// Throws the object to a given position by converting the final position to velocity.
    /// </summary> 
    /// <param name="_finalPosition">Final position where the object is supposed to be at the end of the trajectory.</param> 
    /// <param name="_angle">Throw angle.</param> 
    /// <param name="_bonusDamage">Bonus damages added to the attack.</param> 
    public override void Throw(Vector3 _finalPosition, float _angle, int _bonusDamage)
    {
        if (!isHeld) return;

        isFacingRight = owner.IsFacingRight;
        base.Throw(_finalPosition, _angle, _bonusDamage);
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        // Get missing component references
        if (!animator) animator = GetComponent<Animator>();
        if (!detector) detector = GetComponentInChildren<TDS_Detector>();
        if (!rigidbody.isKinematic) rigidbody.isKinematic = true;

        if (PhotonNetwork.isMasterClient || !PhotonNetwork.connected)
        {
            detector.OnDetectSomething += StartFlee;
        }
        else
        {
            detector.gameObject.SetActive(false);
        }
    }
    #endregion

    #endregion
}
