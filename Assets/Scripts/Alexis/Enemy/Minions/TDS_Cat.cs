using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomNavMeshAgent))]
public class TDS_Cat : TDS_Character 
{
    /* TDS_Cat :
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
    #region Constants
    private const float LANDING_TIME = .8f;
    #endregion

    #region Animator
    private static readonly int endJump_Hash = Animator.StringToHash("EndJumpTrigger");
    private static readonly int hit_Hash = Animator.StringToHash("HitTrigger");
    private static readonly int isDead_Hash = Animator.StringToHash("isDead");
    private static readonly int isRunning_Hash = Animator.StringToHash("isRunning");
    private static readonly int jump_Hash = Animator.StringToHash("JumpTrigger");
    #endregion

    [SerializeField] private CustomNavMeshAgent agent = null;
    [SerializeField] private CatState catState = CatState.LeftPerch;
    [SerializeField] private PerchInformations leftPerchInfos = null;
    [SerializeField] private PerchInformations rightPerchInfos = null;

    [SerializeField] private TDS_Attack catAttack = null;
    [SerializeField] private float chargeRate = 10; 

    private Coroutine movementCoroutine = null;
    private bool isIndependant = false; 
	#endregion

	#region Methods

	#region Original Methods
    public void ActivateCat()
    {
        if (!PhotonNetwork.isMasterClient || isDead || !agent) return;
        if (!hitBox.IsActive) hitBox.Activate(catAttack, this);
        movementCoroutine = StartCoroutine(GetDownFromPerch());
    }

    public void RestartMovement()
    {
        if(movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null; 
        }
        if (IsDead) return; 
        movementCoroutine = StartCoroutine(MoveCat()); 
    }

    private IEnumerator GetDownFromPerch()
    {
        PerchInformations _infosStart;
        switch (catState)
        {
            case CatState.LeftPerch:
                _infosStart = leftPerchInfos;
                catState = CatState.RightPerch; 
                break;
            case CatState.RightPerch:
                _infosStart = rightPerchInfos;
                catState = CatState.LeftPerch;
                break;
            default:
                yield break;
        }
        SetAnimationState((int)CatAnimationState.Jump);
        yield return new WaitForSeconds(.2f); 
        float _delta = 0;
        float _ratio = 0;
        float _height = Mathf.Abs(_infosStart.PerchPosition.y - _infosStart.LandingPosition.y); 
        while (_delta < LANDING_TIME)
        {
            _ratio = _delta / LANDING_TIME;
            transform.position = new Vector3(Mathf.Lerp(_infosStart.PerchPosition.x, _infosStart.LandingPosition.x, _ratio), _infosStart.LandingCurve.Evaluate(_ratio) * _height, Mathf.Lerp(_infosStart.PerchPosition.z, _infosStart.LandingPosition.z, _ratio));
            yield return null;
            _delta += Time.deltaTime; 
        }
        movementCoroutine = StartCoroutine(MoveCat());
    }

    private IEnumerator GetOnPerch()
    {
        PerchInformations _infosEnd;
        switch (catState)
        {
            case CatState.LeftPerch:
                _infosEnd = leftPerchInfos;
                break;
            case CatState.RightPerch:
                _infosEnd = rightPerchInfos;
                break;
            default:
                yield break;
        }
        SetAnimationState((int)CatAnimationState.Jump);
        //yield return new WaitForSeconds(.2f); 
        float _delta = 0;
        float _ratio = 0;
        float _height = Mathf.Abs(_infosEnd.PerchPosition.y - _infosEnd.LandingPosition.y);
        while (_delta < LANDING_TIME)
        {
            _ratio = _delta / LANDING_TIME;
            transform.position = new Vector3(Mathf.Lerp(_infosEnd.LandingPosition.x, _infosEnd.PerchPosition.x, _ratio), _infosEnd.LandingCurve.Evaluate(1 - _ratio) * _height, Mathf.Lerp(_infosEnd.LandingPosition.z, _infosEnd.PerchPosition.z, _ratio));
            yield return null;
            _delta += Time.deltaTime;
        }
        SetAnimationState((int)CatAnimationState.Idle);
        SetAnimationState((int)CatAnimationState.EndJump);
        yield return null;
        Flip();
        if (isIndependant)
        {
            yield return new WaitForSeconds(chargeRate);
            ActivateCat(); 
        }
        movementCoroutine = null;
    }

    private IEnumerator MoveCat()
    {
        rigidbody.useGravity = true;
        agent.SetDestination(catState == CatState.RightPerch ? rightPerchInfos.LandingPosition : leftPerchInfos.LandingPosition);
        SetAnimationState((int)CatAnimationState.Run);
        SetAnimationState((int)CatAnimationState.EndJump);
        while (agent.IsMoving)
        {
            yield return null;
        }
        movementCoroutine = StartCoroutine(GetOnPerch());
        rigidbody.useGravity = false; 
    }

    private void SetAnimationState(int _animationID)
    {
        switch ((CatAnimationState)_animationID)
        {
            case CatAnimationState.Idle:
                animator.SetBool(isRunning_Hash, false); 
                break;
            case CatAnimationState.Run:
                animator.SetBool(isRunning_Hash, true); 
                break;
            case CatAnimationState.Hit:
                animator.SetTrigger(hit_Hash); 
                break;
            case CatAnimationState.Jump:
                animator.SetTrigger(jump_Hash);
                break;
            case CatAnimationState.EndJump:
                animator.SetTrigger(endJump_Hash);
                break;
            case CatAnimationState.Die:
                animator.SetBool(isDead_Hash, true);
                break; 
            default:
                break;
        }
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, GetType(), "SetAnimationState"), new object[] { _animationID });

    }

    public override bool TakeDamage(int _damage)
    {
        bool _takeDamages = base.TakeDamage(_damage);
        if(_takeDamages)
        {
            if (movementCoroutine != null)
            {
                StopCoroutine(movementCoroutine);
                movementCoroutine = null; 
            }
            agent.StopAgent();
            SetAnimationState((int)CatAnimationState.Hit); 
        }
        return _takeDamages; 
    }

    protected override void Die()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
        agent.StopAgent();
        hitBox.Desactivate(); 
        SetAnimationState((int)CatAnimationState.Die); 
        base.Die();
    }

    public void SetCatIndependant()
    {
        isIndependant = true;
        ActivateCat(); 
    }

    public void DesactivateCat()
    {
        if (isDead) return; 
        isIndependant = false; 
        if(movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null; 
        }
        SetAnimationState((int)CatAnimationState.Idle);
        hitBox.Desactivate(); 
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();
        if (!agent) agent = GetComponent<CustomNavMeshAgent>(); 
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start(); 
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(leftPerchInfos.PerchPosition, .25f); 
        Gizmos.DrawSphere(rightPerchInfos.PerchPosition, .25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(leftPerchInfos.LandingPosition, .25f);
        Gizmos.DrawSphere(rightPerchInfos.LandingPosition, .25f);
    }
    #endregion

    #endregion
}

[Serializable]
public class PerchInformations
{
    [SerializeField] private Vector3 perchPosition = Vector3.zero; 
    public Vector3 PerchPosition { get { return perchPosition; } }
    [SerializeField] private Vector3 landingPosition = Vector3.zero;
    public Vector3 LandingPosition { get { return landingPosition;  } }
    [SerializeField] private AnimationCurve landingCurve = new AnimationCurve(); 
    public AnimationCurve LandingCurve { get { return landingCurve; } }
}
