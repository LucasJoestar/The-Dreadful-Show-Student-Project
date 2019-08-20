using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_HittingWall : MonoBehaviour
{
    /* TDS_LoadSceneWindow# :
 *
 *	#####################
 *	###### PURPOSE ######
 *	#####################
 *
 *	#####################
 *	####### TO DO #######
 *	#####################
 *
 *	#####################
 *	### MODIFICATIONS ###
 *	#####################
 *
 *	-----------------------------------
*/

    #region Fields / Properties
    /// <summary>
    /// Indicates if this script behaviour is activated.
    /// </summary>
    [SerializeField] protected bool isActivated = false;

    /// <summary>
    /// Should this script be destroyed when desactivated ?
    /// </summary>
    [SerializeField] protected bool doDestroyOnDesactivate = true;

    /// <summary>
    /// Collider of this object.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>
    /// Maximum damages this wall can deal.
    /// </summary>
    [SerializeField] private int damagesMax = 2;

    /// <summary>
    /// Minimum damages this wall can deal.
    /// </summary>
    [SerializeField] private int damagesMin = 1;

    /// <summary>
    /// Tags this wall detect to hit and expulse.
    /// </summary>
    [SerializeField] private Tags detectedTags = new Tags();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activates this hitting wall.
    /// </summary>
    /// <returns></returns>
    public bool Activate() => isActivated = true;

    /// <summary>
    /// Destroys this script.
    /// </summary>
    public void Desactivate()
    {
        if (doDestroyOnDesactivate)
        {
            Destroy(this);
            return;
        }

        isActivated = false;
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!collider) collider = GetComponent<BoxCollider>();

        if (!PhotonNetwork.isMasterClient && enabled) enabled = false;
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // If detected object doesn't have detecting tags, just ignore it
        if (!isActivated || !other.gameObject.HasTag(detectedTags.ObjectTags)) return;

        TDS_Damageable _damageable = other.GetComponent<TDS_Damageable>();
        if (_damageable)
        {
            _damageable.TakeDamage(Random.Range(damagesMin, damagesMax + 1));
        }

        // Expulse collider
        Vector3 _nearestPoint = collider.bounds.ClosestPoint(other.bounds.center);
        Vector3 _direction = _nearestPoint - collider.bounds.center;
        Vector3 _otherColliderCenter = other.bounds.center - other.transform.position;

        other.transform.position = new Vector3(_nearestPoint.x + (other.bounds.extents.x * Mathf.Sign(_direction.x)) + _otherColliderCenter.x, other.transform.position.y, _nearestPoint.z + (other.bounds.extents.z * Mathf.Sign(_direction.z)) + _otherColliderCenter.z);
    }
    #endregion

    #endregion
}
