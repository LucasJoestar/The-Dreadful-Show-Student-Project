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

    /// <summary>Backing field for <see cref="AxisBehaviour"/>.</summary>
    [SerializeField] private AxisBehaviour axisBehaviour = AxisBehaviour.Both;

    /// <summary>
    /// Behaviour of the axis.
    /// </summary>
    public AxisBehaviour AxisBehaviour { get { return axisBehaviour; } }


    /// <summary>Backing field for <see cref="LastState"/>.</summary>
    [SerializeField, HideInInspector] private AxisState lastState = AxisState.None;

    /// <summary>
    /// Last registered state of this axis.
    /// </summary>
    public AxisState LastState { get { return lastState; } }
    #endregion

    #region Constructors
    /// <summary>
    /// Creates a simple new axis to input.
    /// </summary>
    public TDS_AxisToInput() { }

    /// <summary>
    /// Creates a new axis to input with a given name.
    /// </summary>
    /// <param name="_name">Name of the object.</param>
    public TDS_AxisToInput(string _name)
    {
        axisName = _name;
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

        if (axisBehaviour != AxisBehaviour.Both)
        {
            if (((axisBehaviour == AxisBehaviour.Positive) && (_value < 0)) ||
                ((axisBehaviour == AxisBehaviour.Negative) && (_value > 0))) return;
        }

        // Updates the state of this axis depending on the last one and its actual value
        switch (lastState)
        {
            case AxisState.Key:
                if (_value != 0)
                {
                    if (axisBehaviour == AxisBehaviour.Both) break;
                    if (axisBehaviour == AxisBehaviour.Positive)
                    {
                        if (_value > 0) break;
                    }
                    else if (_value < 0) break;
                }

                lastState = AxisState.KeyUp;
                break;

            case AxisState.KeyDown:
                if (((_value > 0) && (axisBehaviour != AxisBehaviour.Negative)) ||
                    ((_value < 0) && (axisBehaviour != AxisBehaviour.Positive)))
                {
                    lastState = AxisState.Key;
                }
                else lastState = AxisState.KeyUp;
                break;

            case AxisState.KeyUp:
                if (((_value > 0) && (axisBehaviour != AxisBehaviour.Negative)) ||
                    ((_value < 0) && (axisBehaviour != AxisBehaviour.Positive)))
                {
                    lastState = AxisState.KeyDown;
                }
                else lastState = AxisState.None;
                break;

            case AxisState.None:
                if (_value == 0) break;
                if (axisBehaviour == AxisBehaviour.Positive)
                {
                    if (_value < 0) break;
                }
                else if ((axisBehaviour == AxisBehaviour.Negative) && (_value > 0)) break;

                lastState = AxisState.KeyDown;
                break;

            default:
                // Mhmm...
                break;
        }
    }
    #endregion
}
