using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_Trigger : MonoBehaviour 
{
    /* TDS_Trigger :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Attach this class to any game object with a trigger collider to detect when something enters or exits this one.
     *	    
     *	    Sort detected objects by adding desired tags to the list. If null, let's detect eveything.
     *	    
     *	    Subscribes to this class events to get what comes into or exits the collider
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
	 *	Creation of the TDS_Trigger class.
     *	
     *	    - Added the OnTriggerEnter & OnTriggerExit events.
     *	    - Added the detectedColliders property ; the detectedTags fields & properties ; and the doSortByTag field.
     *	    - Added the Unity OnTriggerEnter & OnTriggerExit methods.
	 *
	 *	-----------------------------------
	*/

    #region Events
    /// <summary>
    /// Subscribes on this to get when a new collider enters this game object trigger.
    /// </summary>
    public event Action<Collider> OnColliderEnter = null;

    /// <summary>
    /// Subscribes on this to get when a collider exits this game object trigger
    /// </summary>
    public event Action<Collider> OnColliderExit = null;

    /// <summary>
    /// Event called when nothing is detected anymore.
    /// </summary>
    public event Action OnNothingDetected = null;

    /// <summary>
    /// Event called when a first object is detected. (After nothing was detected.)
    /// </summary>
    public event Action OnSomethingDetected = null;
    #endregion

    #region Fields / Properties
    /// <summary>
    /// Determines of sorting colliders by tag.
    /// Automatically set to false when <see cref="DetectedTags"/> is empty.
    /// </summary>
    private bool doSortByTag = false;

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
    /// Returns the nearest detected collider in zone (null if none).
    /// </summary>
    public GameObject NearestObject
    {
        get
        {
            switch (detectedColliders.Count)
            {
                case 0:
                    return null;

                case 1:
                    return detectedColliders[0].gameObject;

                default:
                    return detectedColliders.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).First().gameObject;
            }
        }
    }

    /// <summary>Backing field for </summary>
    [SerializeField] private List<string> detectedTags = new List<string>();

    /// <summary>
    /// List of all detected tags used to sort colliders detection.
    /// If empty, just detect everything.
    /// </summary>
    public List<string> DetectedTags
    {
        get { return detectedTags; }
        set
        {
            detectedTags = value;
            doSortByTag = value.Count > 0;
        }
    }
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Clears colliders from the detected colliders list that are disables.
    /// </summary>
    private void ClearData()
    {
        // Get element(s) to remove
        Collider[] _toRemove = new List<Collider>(detectedColliders).Where(d => d.enabled == false).ToArray();

        if (_toRemove.Length == 0) return;

        // Remove all disable colliders
        foreach (Collider _collider in _toRemove)
        {
            DetectedColliders.Remove(_collider);
            OnColliderExit?.Invoke(_collider);
        }

        if (detectedColliders.Count == 0) OnNothingDetected?.Invoke();
    }
    #endregion

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // If sorting by tag and this object one does not match, ignores it
        if ((doSortByTag && !detectedTags.Contains(other.tag)) || detectedColliders.Contains(other)) return;

        DetectedColliders.Add(other);
        OnColliderEnter?.Invoke(other);

        if (detectedColliders.Count == 1) OnSomethingDetected?.Invoke();
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        // If the collider is not in the list of the detected ones, ignores it
        if (!detectedColliders.Contains(other)) return;

        DetectedColliders.Remove(other);
        OnColliderExit?.Invoke(other);

        if (detectedColliders.Count == 0) OnNothingDetected?.Invoke();
    }

    // Use this for initialization
    private void Start()
    {
        InvokeRepeating("ClearData", .1f, .05f);
    }
    #endregion

    #endregion
}
