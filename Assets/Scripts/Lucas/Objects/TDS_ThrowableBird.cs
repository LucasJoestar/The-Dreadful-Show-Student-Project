using System.Collections;
using UnityEngine;

public class TDS_ThrowableBird : TDS_ThrowableAnimal
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

        Vector3 _movement = new Vector3(Mathf.Sign(detector.Collider.bounds.center.x - _collider.bounds.center.x), 1.5f, 0);

        if (_movement.x != isFacingRight.ToSign())
        {
            transform.Rotate(Vector3.up, 180);
            isFacingRight = !isFacingRight;
        }

        _movement.x *= 1.5f;

        while (sprite.isVisible)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + _movement, Time.deltaTime);
            _movement.y *= 1.0125f;
            _movement.x *= 1.01f;

            yield return null;
        }

        PhotonNetwork.Destroy(gameObject);
    }
    #endregion
}
