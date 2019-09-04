using UnityEngine;
using UnityEngine.Events;

public class TDS_SpecialThrowable : TDS_Throwable
{
    /* TDS_Throwable :
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

    #region Fields / Properties
    /// <summary>
    /// Indicates if the throwable has already been held once.
    /// </summary>
    private bool hasBeenHeld = false;

    /// <summary>
    /// Unity event called the first time this throwable is grabbed.
    /// </summary>
    [SerializeField] private UnityEvent OnGrabbedFirstTime = new UnityEvent();
    #endregion

    #region Methods
    /// <summary> 
    /// Let a character pickup the object.
    /// </summary> 
    /// <param name="_owner">Character attempting to pick up the object.</param> 
    /// <returns>Returns true is successfully picked up the object, false if a issue has been encountered.</returns> 
    public override bool PickUp(TDS_Character _owner)
    {
        if (!base.PickUp(_owner)) return false;

        if (!hasBeenHeld)
        {
            hasBeenHeld = true;
            if (sprite.maskInteraction != SpriteMaskInteraction.None) sprite.maskInteraction = SpriteMaskInteraction.None;
            OnGrabbedFirstTime.Invoke();
        }

        return true;
    }
    #endregion
}
