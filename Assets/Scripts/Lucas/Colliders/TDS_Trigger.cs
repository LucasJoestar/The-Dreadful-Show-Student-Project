using UnityEngine;
using UnityEngine.Events;

public class TDS_Trigger : MonoBehaviour 
{
    /* TDS_Trigger :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Simple trigger calling an event when an authorized obejct enters in it.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	Humf !
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[10 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Trigger class.
     *	
     *	    • Unity events are now called when entering and leaving this trigger.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Indicates if this object should be destroyed when entering trigger.
    /// </summary>
    public bool doDestroyAfterTriggerEnter = true;

    /// <summary>
    /// Indicates if this object should be destroyed when exiting trigger.
    /// </summary>
    public bool doDestroyAfterTriggerExit = true;

    /// <summary>
    /// Tags used to detect only authorized objects.
    /// </summary>
    [SerializeField] private Tags detectedTags = new Tags();

    /// <summary>
    /// Event called when something enters this trigger.
    /// </summary>
    public UnityEvent OnTriggerEnterE = new UnityEvent();

    /// <summary>
    /// Event called when something leaves this trigger.
    /// </summary>
    public UnityEvent OnTriggerExitE = new UnityEvent();
    #endregion

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.HasTag(detectedTags.ObjectTags))
        {
            OnTriggerEnterE.Invoke();

            if (doDestroyAfterTriggerEnter) Destroy(this);
        }
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.HasTag(detectedTags.ObjectTags))
        {
            OnTriggerExitE.Invoke();

            if (doDestroyAfterTriggerExit) Destroy(this);
        }
    }
    #endregion
}
