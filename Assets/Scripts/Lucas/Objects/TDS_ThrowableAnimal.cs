using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CustomNavMeshAgent))]
public class TDS_ThrowableAnimal : TDS_FleeingThrowable
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
    /// <summary>
    /// Agent of the animal used to make it move.
    /// </summary>
    [SerializeField] protected CustomNavMeshAgent agent = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Makes the animal flee a certain collider.
    /// </summary>
    /// <param name="_collider">Collider to flee.</param>
    /// <returns></returns>
    protected override IEnumerator Flee(Collider _collider)
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
                    Flip();
                }

                // New destination ignored, got to be fixed
                _actualDestination = _newDestination;
                //Debug.Log(_actualDestination); 
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
    public override void SetAnimation(int _animationID)
    {
        if (_animationID != 1) agent.StopAgent();

        base.SetAnimation(_animationID);
    }

    /// <summary>
    /// Set new destination.
    /// </summary>
    /// <param name="_x">X coordinate.</param>
    /// <param name="_y">Y coordinate.</param>
    /// <param name="_z">Z coordinate.</param>
    public void SetDestination(float _x, float _y, float _z)
    {
        Vector3 _destination = new Vector3(_x, _y, _z);
        if (Mathf.Sign(_destination.x - transform.position.x) != isFacingRight.ToSign())
        {
            Flip();
        }
        agent.SetDestination(_destination);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {
        base.Awake();

        if (!agent) agent = GetComponent<CustomNavMeshAgent>();
    }
    #endregion

    #endregion
}
