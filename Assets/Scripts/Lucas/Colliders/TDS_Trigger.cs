using System.Linq;
using System.Collections.Generic;
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

    #region Events
    /// <summary>
    /// Event called when something enters this trigger.
    /// </summary>
    public UnityEvent OnTriggerEnterE = new UnityEvent();

    /// <summary>
    /// Event called when something leaves this trigger.
    /// </summary>
    public UnityEvent OnTriggerExitE = new UnityEvent();
    #endregion

    #region Fields / Properties
    /// <summary>
    /// Tags used to detect only authorized objects.
    /// </summary>
    [SerializeField] private List<string> detectedTags = new List<string>();
    #endregion

    #region Unity Methods
    // OnTriggerEnter is called when the GameObject collides with another GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetTags().Union(detectedTags).Any())
        {
            OnTriggerEnterE.Invoke();
        }
    }

    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetTags().Union(detectedTags).Any())
        {
            OnTriggerExitE.Invoke();
        }
    }
    #endregion
}
