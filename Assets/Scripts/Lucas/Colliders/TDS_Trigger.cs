using System;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary>
    /// List of a colliders actually in this game object trigger.
    /// </summary>
    public List<Collider> DetectedColliders { get; private set; } = new List<Collider>();

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

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        // If sorting by tag and this object one does not match, ignores it
        if (doSortByTag && !detectedTags.Contains(other.tag)) return;

        DetectedColliders.Add(other);
        OnColliderEnter?.Invoke(other);

        if (DetectedColliders.Count == 1) OnSomethingDetected?.Invoke();
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        // If the collider is not in the list of the detected ones, ignores it
        if (!DetectedColliders.Contains(other)) return;

        DetectedColliders.Remove(other);
        OnColliderExit?.Invoke(other);

        if (DetectedColliders.Count == 0) OnNothingDetected?.Invoke();
    }
    #endregion
}
