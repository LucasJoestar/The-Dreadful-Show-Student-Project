using System.Collections;
using UnityEngine;

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

        return true;
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
