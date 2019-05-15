using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TDS_Detector : MonoBehaviour 
{
    /* TDS_Detector :
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
	 *	Creation of the TDS_Detector class.
     *	
     *	    - Added the OnTriggerEnter & OnTriggerExit events.
     *	    - Added the detectedColliders property ; the detectedTags fields & properties ; and the doSortByTag field.
     *	    - Added the Unity OnTriggerEnter & OnTriggerExit methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
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

    /// <summary>
    /// List of all detected tags used to sort colliders detection.
    /// </summary>
    public Tags DetectedTags = new Tags();
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Clears colliders from the detected colliders list that are disables.
    /// </summary>
    private void ClearData()
    {
        if (detectedColliders.Count == 0) return;

        // Get element(s) to remove
        Collider[] _toRemove = new List<Collider>(detectedColliders).Where(d => (d == null) || (d.enabled == false)).ToArray();

        if (_toRemove.Length == 0) return;

        // Remove all disable colliders
        foreach (Collider _collider in _toRemove)
        {
            DetectedColliders.Remove(_collider);
        }
    }
    #endregion

    #region Unity Methods
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
        if (detectedColliders.Contains(other)) DetectedColliders.Remove(other);
    }

    // Use this for initialization
    private void Start()
    {
        InvokeRepeating("ClearData", .1f, .05f);
    }
    #endregion

    #endregion
}
