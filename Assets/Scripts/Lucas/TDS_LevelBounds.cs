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
    /// Should this object be disabled after activated ?
    /// </summary>
    [SerializeField] private bool doDisableAfterActive = true;

    /// <summary>
    /// Collider trigger to enable these bounds.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>
    /// Left bound, so X minimum bounds value.
    /// </summary>
    [SerializeField] private Transform leftBound = null;

    /// <summary>
    /// Public accessor for <see cref="leftBound"/>.
    /// </summary>
    public Transform LeftBound { get { return leftBound; } }

    /// <summary>
    /// Right bound, so X maximum bounds value.
    /// </summary>
    [SerializeField] private Transform rightBound = null;

    /// <summary>
    /// Public accessor for <see cref="rightBound"/>.
    /// </summary>
    public Transform RightBound { get { return rightBound; } }

    /// <summary>
    /// Bottom bound, so Z minimum bounds value.
    /// </summary>
    [SerializeField] private Transform bottomBound = null;

    /// <summary>
    /// Public accessor for <see cref="bottomBound"/>.
    /// </summary>
    public Transform BottomBound { get { return bottomBound; } }

    /// <summary>
    /// Top bound, so Z maximum bounds value.
    /// </summary>
    [SerializeField] private Transform topBound = null;

    /// <summary>
    /// Public accessor for <see cref="topBound"/>.
    /// </summary>
    public Transform TopBound { get { return topBound; } }
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activate these bounds.
    /// </summary>
    public void Activate()
    {
        TDS_Camera.Instance.SetBounds(this);

        if (doDisableAfterActive) gameObject.SetActive(false);
        enabled = false;
    }
	#endregion

	#region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        Activate();
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
