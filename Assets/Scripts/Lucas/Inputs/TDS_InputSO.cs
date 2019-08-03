using UnityEngine;

public class TDS_InputSO : ScriptableObject
{
    /* TDS_InputSO :
     * 
     *  #####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Stock all game inputs into a scriptable object
     *	
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[05 / 07 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TDS_InputSO class.
    */

    #region Fields / Properties
    /// <summary>
    /// All controllers available for the game.
    /// </summary>
    public TDS_Controller[] controllers = new TDS_Controller[] { };

    /// <summary>
    /// All axis from this project.
    /// </summary>
    public TDS_AxisToInput[] Axis = new TDS_AxisToInput[] { };

    /// <summary>
    /// All custom buttons from this project.
    /// </summary>
    public TDS_Button[] Buttons = new TDS_Button[] { };
    #endregion
}
