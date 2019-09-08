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

        // Trigger animation
        SetAnimationOnline(1);

        Vector3 _movement = new Vector3(Mathf.Sign(detector.Collider.bounds.center.x - _collider.bounds.center.x), speed, 0);

        if (_movement.x != isFacingRight.ToSign())
        {
            transform.Rotate(Vector3.up, 180);
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z * -1);

            isFacingRight = !isFacingRight;
        }

        _movement.x *= speed;

        MeshRenderer _shadow = shadow.GetComponent<MeshRenderer>();

        while (sprite.isVisible && _shadow.isVisible)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + _movement, Time.deltaTime);
            _movement.y *= 1.01f;
            _movement.x *= 1.01f;

            yield return null;
        }

        DestroyThrowableObject();
    }
    #endregion
}
