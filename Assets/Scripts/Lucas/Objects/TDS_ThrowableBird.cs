using System.Collections;
using UnityEngine;

public class TDS_ThrowableBird : TDS_FleeingThrowable
{
    /* TDS_ThrowableBird :
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
    /// Speed at which the bird flees.
    /// </summary>
    [SerializeField] private float speed = 1.5f;

    /// <summary>
    /// Coroutine for fleeing online.
    /// </summary>
    private Coroutine fleeOnlineCoroutine = null;
    #endregion

    #region Methods
    /// <summary>
    /// Makes the animal flee a certain collider.
    /// </summary>
    /// <param name="_collider">Collider to flee.</param>
    /// <returns></returns>
    protected override IEnumerator Flee(Collider _collider)
    {
        yield return new WaitForSeconds(fleeDelay);

        detector.gameObject.SetActive(false);
        
        // Trigger animation
        SetAnimationOnline(1);

        Vector3 _movement = new Vector3(Mathf.Sign(detector.Collider.bounds.center.x - _collider.bounds.center.x), speed, 0);

        if (_movement.x != isFacingRight.ToSign())
        {
            Flip();
        }

        TDS_RPCManager.Instance.CallRPC(PhotonTargets.Others, photonView, GetType(), "StartFleeOnline", new object[] { isFacingRight });

        _movement.x *= speed;

        MeshRenderer _shadow = shadow.GetComponent<MeshRenderer>();

        while ((transform.position.x < (TDS_Camera.Instance.CurrentBounds.XMax + 2)) && (transform.position.x > (TDS_Camera.Instance.CurrentBounds.XMin - 2)))
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + _movement, Time.deltaTime);
            _movement.y *= 1.01f;
            _movement.x *= 1.01f;

            yield return null;
        }

        if (PhotonNetwork.isMasterClient) DestroyThrowableObject();
    }

    /// <summary>
    /// Flee for online clients.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FleeOnline()
    {
        Vector3 _movement = new Vector3(isFacingRight.ToSign(), speed, 0);
    
        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + _movement, Time.deltaTime);
            _movement.y *= 1.01f;
            _movement.x *= 1.01f;

            yield return null;
        }
    }

    /// <summary> 
    /// Let a character pickup the object.
    /// </summary> 
    /// <param name="_owner">Character attempting to pick up the object.</param> 
    /// <returns>Returns true is successfully picked up the object, false if a issue has been encountered.</returns> 
    public override bool PickUp(TDS_Character _owner)
    {
        if (!base.PickUp(_owner)) return false;

        if (fleeOnlineCoroutine != null) StopCoroutine(fleeOnlineCoroutine);
        return true;
    }

    /// <summary>
    /// Start fleeing for online clients.
    /// </summary>
    /// <param name="_isFacingRight">Indicates if the object should face right side or not.</param>
    private void StartFleeOnline(bool _isFacingRight)
    {
        if (fleeOnlineCoroutine != null) StopCoroutine(fleeOnlineCoroutine);

        if (isFacingRight != _isFacingRight) Flip();
        fleeOnlineCoroutine = StartCoroutine(FleeOnline());

    }

    /// <summary>
    /// Set animal animation.
    /// </summary>
    /// <param name="_animationID">ID of the new animation.</param>
    public override void SetAnimation(int _animationID)
    {
        base.SetAnimation(_animationID);
        if (_animationID == 1) rigidbody.isKinematic = true;
    }
    #endregion
}
