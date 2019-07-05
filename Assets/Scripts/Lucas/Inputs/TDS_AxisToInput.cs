using System;
using UnityEngine;

[Serializable]
public class TDS_AxisToInput 
{
    /* TDS_AxisToInput :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Class used to convert and use an axis as an input.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[13 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      - Added the AxisName property.
	 *	    - Added the one string parameter constructor.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_AxisToInput class.
     *	
     *	    - Added the AxisName field ; and the lastState field & property.
     *	    - Added the UpdateState method.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="AxisName"/>.</summary>
    [SerializeField] private string axisName = "Unknown Axis";

    /// <summary>
    /// Name of the axis to convert.
    /// </summary>
    public string AxisName { get { return axisName; } }

    /// <summary>Backing field for <see cref="LastState"/>.</summary>
    [SerializeField, HideInInspector] private AxisState lastState = AxisState.None;

    /// <summary>
    /// Last registered state of this axis.
    /// </summary>
    public AxisState LastState { get { return lastState; } }
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new AxisToInput object with a specified axis name.
    /// </summary>
    /// <param name="_axisName">Name of the axis to use.</param>
    public TDS_AxisToInput(string _axisName)
    {
        axisName = _axisName;
    }
    #endregion

    #region Methods
    /// <summary>
    /// Updates the state of this axis.
    /// </summary>
    public void UpdateState()
    {
        // Get the value of this axis
        float _value = Input.GetAxis(axisName);

        // Updates the state of this axis depending on the last one and its actual value
        switch (lastState)
        {
            case AxisState.Key:
                if (_value == 0) lastState = AxisState.KeyUp;
                break;

            case AxisState.KeyDown:
                if (_value == 0) lastState = AxisState.KeyUp;
                else lastState = AxisState.Key;
                break;

            case AxisState.KeyUp:
                if (_value == 0) lastState = AxisState.None;
                else lastState = AxisState.KeyDown;
                break;

            case AxisState.None:
                if (_value != 0) lastState = AxisState.KeyDown;
                break;

            default:
                // Mhmm...
                break;
        }
    }
	#endregion
}
