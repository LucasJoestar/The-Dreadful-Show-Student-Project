using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private CustomNavMeshAgent agent = null;
    [SerializeField] private CatState catState = CatState.LeftPerch;
    [SerializeField] private PerchInformations leftPerchInfos = null;
    [SerializeField] private PerchInformations rightPerchInfos = null; 

    private Coroutine movementCoroutine = null; 
	#endregion

	#region Methods

	#region Original Methods
    public void ActivateCat()
    {
        if (!PhotonNetwork.isMasterClient || isDead || !agent || catState == CatState.Travelling) return;
        movementCoroutine = StartCoroutine(ReachNextPerch()); 
    }

    private IEnumerator ReachNextPerch()
    {
        PerchInformations _infosStart;
        PerchInformations _infosEnd; 
        switch (catState)
        {
            case CatState.LeftPerch:
                _infosStart = leftPerchInfos;
                _infosEnd = rightPerchInfos; 
                break;
            case CatState.Travelling:
                yield break; 
            case CatState.RightPerch:
                _infosStart = rightPerchInfos;
                _infosEnd = leftPerchInfos;
                break;
            default:
                yield break;
        }
        SetAnimationState((int)CatAnimationState.Jump);
        yield return new WaitForSeconds(.2f); 
        float _delta = 0;
        float _ratio = 0;
        while (_delta < LANDING_TIME)
        {
            _ratio = _delta / LANDING_TIME;
            transform.position = new Vector3(Mathf.Lerp(_infosStart.perchPosition.x, _infosStart.landingPosition.x, _ratio), _infosStart.landingCurve.Evaluate(_ratio));
            yield return null;
            _delta += Time.deltaTime; 
        }
        agent.SetDestination(_infosEnd.landingPosition);
        SetAnimationState((int)CatAnimationState.Run);
        SetAnimationState((int)CatAnimationState.Jump);
        while (agent.IsMoving)
        {
            yield return null;
        }
        SetAnimationState((int)CatAnimationState.Jump);
        yield return new WaitForSeconds(.2f); 
        _delta = 0;
        _ratio = 0;
        while (_delta < LANDING_TIME)
        {
            _ratio = _delta / LANDING_TIME;
            transform.position = new Vector3(Mathf.Lerp(_infosStart.perchPosition.x, _infosStart.landingPosition.x, _ratio), _infosStart.landingCurve.Evaluate(1 - _ratio));
            yield return null;
            _delta += Time.deltaTime;
        }
        SetAnimationState((int)CatAnimationState.Idle);
        SetAnimationState((int)CatAnimationState.Jump);
        yield return null;
        Flip(); 
    }

    private void SetAnimationState(int _animationID)
    {
        switch ((CatAnimationState)_animationID)
        {
            case CatAnimationState.Idle:
                animator.SetBool("isRunning", false); 
                break;
            case CatAnimationState.Run:
                animator.SetBool("isRunning", true); 
                break;
            case CatAnimationState.Hit:
                animator.SetTrigger("HitTrigger"); 
                break;
            case CatAnimationState.Jump:
                animator.SetTrigger("JumpTrigger");
                break;
            default:
                break;
        }
        if (PhotonNetwork.isMasterClient) TDS_RPCManager.Instance?.RPCPhotonView.RPC("CallMethodOnline", PhotonTargets.Others, TDS_RPCManager.GetInfo(photonView, this.GetType(), "SetAnimationState"), new object[] { (int)_animationID });

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
	#endregion

	#endregion
}

[Serializable]
public class PerchInformations
{
    public Vector3 perchPosition { get; set; }
    public Vector3 landingPosition { get; set; }
    public AnimationCurve landingCurve { get; set; }
}
