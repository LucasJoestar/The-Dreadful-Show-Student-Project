using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(BoxCollider))]
public class TDS_PlayerInteractionBox : MonoBehaviour 
{
    /* TDS_Detector :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Manages all possible interactions for a player.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[27 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    - Added a method called at a constant interval that clear disable colliders
     *	from the list of the detected ones.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[12 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Trigger class.
     *	
     *	    - Added the detectedColliders field ; and the NearestObject property.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[17 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Detector class.
     *	
     *	    - Added the OnTriggerEnter & OnTriggerExit events.
     *	    - Added the detectedColliders property ; the detectedTags fields & properties ; and the doSortByTag field.
     *	    - Added the Unity OnTriggerEnter & OnTriggerExit methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Indicates if feedback should be displayed above nearest object.
    /// </summary>
    private bool doDisplayFeedback = true;

    /// <summary>
    /// Nearest collider in detected ones.
    /// </summary>
    private Collider nearestCollider = null;

    /// <summary>Backing field for <see cref="DetectedColliders"/>.</summary>
    [SerializeField] private List<Collider> detectedColliders = new List<Collider>();

    /// <summary>
    /// List of a colliders actually in this game object trigger.
    /// </summary>
    public List<Collider> DetectedColliders
    {
        get { return detectedColliders; }
        private set { detectedColliders = value; }
    }

    /// <summary>
    /// Returns the GameObject of the nearest collider.
    /// </summary>
    public GameObject NearestObject { get { return nearestCollider ? nearestCollider.gameObject : null; } }

    /// <summary>
    /// Feedback displayed in top of the nearest object to show it to the player.
    /// </summary>
    [SerializeField] private TextMeshPro interactText = null;

    /// <summary>Public accessor for <see cref="interactText"/>.</summary>
    public TextMeshPro InteractText { get { return interactText; } }

    /// <summary>
    /// List of all detected tags used to sort colliders detection.
    /// </summary>
    public Tags DetectedTags = new Tags();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Indicates if wether or not the interaction feedback should be displayed.
    /// </summary>
    /// <param name="_doDisplay">Should the feedback be displayed or not.</param>
    public void DisplayInteractionFeedback(bool _doDisplay)
    {
        doDisplayFeedback = _doDisplay;

        if (!_doDisplay)
        {
            if (nearestCollider)
            {
                if (interactText.gameObject.activeInHierarchy) interactText.gameObject.SetActive(false);

                // If the object has outline tag, desactivate the outline on it
                if (nearestCollider.gameObject.HasTag("Outline")) nearestCollider.GetComponentInChildren<TDS_DiffuseOutline>().DisableOutline();
            }
            
        }
        else if (nearestCollider != null)
        {
            interactText.gameObject.SetActive(true);

            // If the object has outline tag, activate the outline on it
            if (nearestCollider.gameObject.HasTag("Outline")) nearestCollider.GetComponentInChildren<TDS_DiffuseOutline>().EnableOutline();
        }
    }

    /// <summary>
    /// Clears colliders from the detected colliders list that are disables.
    /// </summary>
    private void OrderObjects()
    {
        // If not enabled or not detecting anything, return
        if (detectedColliders.Count == 0)
        {
            // Desactivate feedback if needed
            if (interactText.gameObject.activeInHierarchy) interactText.gameObject.SetActive(false);

            return;
        }

        // Get element(s) to remove
        Collider[] _toRemove = new List<Collider>(detectedColliders).Where(d => (d == null) || !d.enabled || !d.gameObject.activeInHierarchy).ToArray();

        if (_toRemove.Length > 0)
        {
            // Remove all disable colliders
            foreach (Collider _collider in _toRemove)
            {
                DetectedColliders.Remove(_collider);

                // Do some actions if the nearest object gets removed from list
                if (nearestCollider == _collider)
                {
                    // Disable outline on remove from list
                    if ((_collider != null) && nearestCollider.gameObject.HasTag("Outline")) nearestCollider.GetComponentInChildren<TDS_DiffuseOutline>().DisableOutline();
                }
            }
        }

        // Order objects
        if (detectedColliders.Count > 0)
        {
            Vector2 _pos = new Vector2(transform.position.x, transform.position.z);
            detectedColliders = detectedColliders.OrderBy(c => Mathf.Abs((_pos - new Vector2(c.transform.position.x, c.transform.position.z)).magnitude)).ToList();

            // Set nearest collider if different
            if (nearestCollider != detectedColliders[0])
            {
                // Disable outline
                if (nearestCollider && nearestCollider.gameObject.HasTag("Outline")) nearestCollider.GetComponentInChildren<TDS_DiffuseOutline>().DisableOutline();

                nearestCollider = detectedColliders[0];

                // If feedback should be displayed, display it
                if (doDisplayFeedback && !interactText.gameObject.activeInHierarchy) interactText.gameObject.SetActive(true);

                // If the object has outline tag, activate the outline on it
                if (doDisplayFeedback && nearestCollider.gameObject.HasTag("Outline")) nearestCollider.GetComponentInChildren<TDS_DiffuseOutline>().EnableOutline();
            }
        }
        // Desactivate feedback if needed
        else if (interactText.gameObject.activeInHierarchy) interactText.gameObject.SetActive(false);
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Get missing component
        if (!interactText)
        {
            interactText = transform.GetChild(1).GetComponentInChildren<TextMeshPro>();
        }
        if (interactText.gameObject.activeInHierarchy) interactText.gameObject.SetActive(false);

        // When player flip, reverse flip the feedback
        GetComponentInParent<TDS_Player>().OnFlip += () => interactText.transform.Rotate(Vector3.up, -180);
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // If detected object has matching tag, add it
        if (other.gameObject.HasTag(DetectedTags.ObjectTags) && !DetectedColliders.Contains(other)) DetectedColliders.Add(other);
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        // Remove detected object when leaving
        if (detectedColliders.Contains(other))
        {
            DetectedColliders.Remove(other);

            // Do some actions if the nearest object gets removed
            if (nearestCollider == other)
            {
                // Disable outline on remove from list
                if (nearestCollider.gameObject.HasTag("Outline")) nearestCollider.GetComponentInChildren<TDS_DiffuseOutline>().DisableOutline();
            }

            // Desactivate feedback if needed
            if (detectedColliders.Count == 0)
            {
                nearestCollider = null;

                // Desactivate feedback
                if (interactText.gameObject.activeInHierarchy) interactText.gameObject.SetActive(false);
            }
        }
    }

    // Use this for initialization
    private void Start()
    {
        InvokeRepeating("OrderObjects", .1f, .05f);
    }

    // Update is called once per frame
    private void Update()
    {
        // Set feedback object position.
        if ((nearestCollider != null) && (interactText.transform.position != new Vector3(nearestCollider.bounds.center.x, nearestCollider.bounds.max.y + .5f, nearestCollider.bounds.center.z)))
        {
            interactText.transform.position = new Vector3(nearestCollider.bounds.center.x, nearestCollider.bounds.max.y + .5f, nearestCollider.bounds.center.z);
        }
    }
    #endregion

    #endregion
}
