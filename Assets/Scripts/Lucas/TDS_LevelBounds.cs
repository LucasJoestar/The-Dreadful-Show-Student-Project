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
    /// Bounds of the camera to set as new when get activated.
    /// </summary>
    [SerializeField] private TDS_Bounds cameraBounds = null;

    /// <summary>Public accessor of <see cref="cameraBounds"/>.</summary>
    public TDS_Bounds CameraBounds { get { return cameraBounds; } }

    /// <summary>
    /// Collider trigger to enable these bounds.
    /// </summary>
    [SerializeField] private new BoxCollider collider = null;

    /// <summary>
    /// Child parent of all colliders to activate with the bounds.
    /// </summary>
    [SerializeField] private GameObject colliderChild = null;

    /// <summary>Public accessor of <see cref="colliderChild"/>.</summary>
    public GameObject ColliderChild { get { return colliderChild; } }
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activate these bounds.
    /// </summary>
    public void Activate()
    {
        TDS_Camera.Instance.SetBounds(cameraBounds);
        colliderChild.SetActive(true);
        collider.enabled = false;
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
		if (!colliderChild)
        {
            colliderChild = transform.GetChild(0).gameObject;
            if (!colliderChild) Debug.LogWarning($"The Bounds \"{name}\" doesn't have any child colliders attached !");
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
