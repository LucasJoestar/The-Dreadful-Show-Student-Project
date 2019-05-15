using UnityEngine;

public class TDS_LevelBounds : MonoBehaviour 
{
    /* TDS_LevelBounds :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Activate bounds for the player & the camera when entering trigger,
     *	and set this as current bounds in the Level Manager ;
     *	desactivate them with a public method.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	Hum...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[01 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_LevelBounds class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Should this object be destroyed after being activated ?
    /// </summary>
    [SerializeField] private bool doDestroyOnActivate = true;

    /// <summary>
    /// Collider trigger to enable these bounds.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;


    /// <summary>
    /// Top bound position.
    /// </summary>
    [SerializeField] private Vector3 topBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="topBound"/>.</summary>
    public Vector3 TopBound { get { return topBound; } }

    /// <summary>
    /// Left bound position.
    /// </summary>
    [SerializeField] private Vector3 leftBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="leftBound"/>.</summary>
    public Vector3 LeftBound { get { return leftBound; } }

    /// <summary>
    /// Right bound position.
    /// </summary>
    [SerializeField] private Vector3 rightBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="rightBound"/>.</summary>
    public Vector3 RightBound { get { return rightBound; } }

    /// <summary>
    /// Bottom bound position.
    /// </summary>
    [SerializeField] private Vector3 bottomBound = Vector3.zero;

    /// <summary>Public accessor for <see cref="bottomBound"/>.</summary>
    public Vector3 BottomBound { get { return bottomBound; } }

    /// <summary>
    /// Tags detected by the trigger to enable.
    /// </summary>
    [SerializeField] private Tags detectedTags = new Tags();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activate these bounds.
    /// </summary>
    public void Activate()
    {
        TDS_Camera.Instance.SetBounds(this);
        if (doDestroyOnActivate) Destroy(this);
        else enabled = false;
    }
    #endregion

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.HasTag(detectedTags.ObjectTags)) Activate();
    }

    // Use this for initialization
    private void Start()
    {
        // Try to get missing references
        if (!collider)
        {
            collider = transform.GetComponent<BoxCollider>();
            if (!collider) Debug.LogWarning($"The Bounds \"{name}\" collider is missing !");
        }
        else if (!collider.isTrigger) collider.isTrigger = true;
    }
	#endregion

	#endregion
}
