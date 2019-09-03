using System;
using UnityEngine;

public class TDS_Detector : MonoBehaviour
{
    /* TDS_Detector :
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

    #region Events
    /// <summary>
    /// Event called when detecting something with the object collider as parameter.
    /// </summary>
    public event Action<Collider> OnDetectSomething = null;
    #endregion

    #region Fields / Properties
    /// <summary>
    /// Collider of the object.
    /// </summary>
    [SerializeField] private new Collider collider = null;

    /// <summary>Public accessor for <see cref="collider"/>.</summary>
    public Collider Collider { get { return collider; } }

    /// <summary>
    /// Tags detected by the detector.
    /// </summary>
    public Tags DetectedTags = new Tags();
    #endregion

    #region Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!collider) collider = GetComponent<Collider>();
    }

    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.HasTag(DetectedTags.ObjectTags)) OnDetectSomething?.Invoke(other);
    }
    #endregion
}
