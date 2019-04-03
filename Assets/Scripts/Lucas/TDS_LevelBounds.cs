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
    /// Collider trigger to enable these bounds.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>
    /// Parent of all bounds to activate with the bounds.
    /// </summary>
    [SerializeField] private GameObject boundsParent = null;

    /// <summary>Public accessor of <see cref="boundsParent"/>.</summary>
    public GameObject BoundsParent { get { return boundsParent; } }

    /// <summary>
    /// Left bound, so X minimum bounds value.
    /// </summary>
    [SerializeField] private Transform leftBound = null;

    /// <summary>
    /// Right bound, so X maximum bounds value.
    /// </summary>
    [SerializeField] private Transform rightBound = null;

    /// <summary>
    /// Bottom bound, so Z minimum bounds value.
    /// </summary>
    [SerializeField] private Transform bottomBound = null;

    /// <summary>
    /// Top bound, so Z maximum bounds value.
    /// </summary>
    [SerializeField] private Transform topBound = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activate these bounds.
    /// </summary>
    public void Activate()
    {
        TDS_Camera.Instance.SetBounds(leftBound.position.x, rightBound.position.x, bottomBound.position.z, topBound.position.z);
        boundsParent.SetActive(true);
    }

    /// <summary>
    /// Desactivate these bounds.
    /// </summary>
    public void Desactivate()
    {
        TDS_Camera.Instance.SetBounds(null);
        gameObject.SetActive(false);
    }
	#endregion

	#region Unity Methods
	// Awake is called when the script instance is being loaded
    private void Awake()
    {

    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        Activate();
    }

    // Use this for initialization
    private void Start()
    {
        // Try to get missing references
		if (!leftBound || !rightBound || !bottomBound || !topBound)
        {
            Debug.LogWarning($"Some Bounds of \"{name}\" are missing !");
        }
        if (!boundsParent)
        {
            boundsParent = transform.GetChild(0).gameObject;
            if (!boundsParent) Debug.LogWarning($"The Bounds \"{name}\" doesn't have any child colliders attached !");
        }
        if (!collider)
        {
            collider = transform.GetComponent<BoxCollider>();
            if (!collider) Debug.LogWarning($"The Bounds \"{name}\" collider is missing !");
        }
        else if (!collider.isTrigger) collider.isTrigger = true;
    }
	
	// Update is called once per frame
	private void Update()
    {
        
	}
	#endregion

	#endregion
}
