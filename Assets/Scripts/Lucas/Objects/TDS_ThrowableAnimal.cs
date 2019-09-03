using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomNavMeshAgent))]
public class TDS_ThrowableAnimal : TDS_Throwable
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
    /// Agent of the animal used to make it move.
    /// </summary>
    [SerializeField] protected CustomNavMeshAgent agent = null;

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
    protected virtual IEnumerator Flee(Collider _collider)
    {
        yield return new WaitForSeconds(fleeDelay);

        // Trigger animation
        SetAnimationOnline(1);

        float _xMinDistance = _collider.bounds.extents.x + detector.Collider.bounds.size.x;
        float _zMinDistance = _collider.bounds.extents.z + detector.Collider.bounds.size.z;

        Vector3 _actualDestination = new Vector3();
        Vector3 _newDestination = new Vector3();

        float _direction = 0;

        // Move while in range
        while ((Mathf.Abs(_direction = (detector.Collider.bounds.center.x - _collider.bounds.center.x)) < _xMinDistance) && (Mathf.Abs(_collider.bounds.center.z - detector.Collider.bounds.center.z) < _zMinDistance))
        {
            _direction = Mathf.Sign(_direction);
            _newDestination = new Vector3(_collider.bounds.center.x + (_xMinDistance * 1.5f * _direction), transform.position.y, transform.position.z);

            if (_newDestination != _actualDestination)
            {
                if (_direction != isFacingRight.ToSign())
                {
                    transform.Rotate(Vector3.up, 180);
                    isFacingRight = !isFacingRight;
                }

                // New destination ignored, got to be fixed
                _actualDestination = _newDestination;
                Debug.Log(_actualDestination); 
                agent.SetDestination(_actualDestination);

                // Set destination for other clients
                TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SetDestination"), new object[] { _actualDestination.x, _actualDestination.y, _actualDestination.z });
            }

            yield return null;
        }

        // Trigger animation
        SetAnimationOnline(0);

        fleeCoroutine = null;
    }

    /// <summary>
    /// Set animal animation.
    /// </summary>
    /// <param name="_animationID">ID of the new animation.</param>
    public void SetAnimation(int _animationID)
    {
        if (_animationID != 1) agent.StopAgent();

        animator.SetInteger("State", _animationID);
    }

    /// <summary>
    /// Set animal animation.
    /// </summary>
    /// <param name="_animationID">ID of the new animation.</param>
    public void SetAnimationOnline(int _animationID)
    {
        TDS_RPCManager.Instance.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SetAnimation"), new object[] { _animationID });

        SetAnimation(_animationID);
    }

    /// <summary>
    /// Set new destination.
    /// </summary>
    /// <param name="_x">X coordinate.</param>
    /// <param name="_y">Y coordinate.</param>
    /// <param name="_z">Z coordinate.</param>
    public void SetDestination(float _x, float _y, float _z)
    {
        agent.SetDestination(new Vector3(_x, _y, _z));
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
        if (!agent) agent = GetComponent<CustomNavMeshAgent>();
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
