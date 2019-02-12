using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal.Input;

public sealed class TDS_Input
{
    /* TDS_Input :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	References all custom inputs of the project into a scriptable object, that subscribes itself to the MonoBehaviour Awake & Update.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Input class.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    /// <summary>
    /// All axis from this project.
    /// </summary>
    private static TDS_AxisToInput[] axis = new TDS_AxisToInput[] { };
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Get if an axis with a specified name was pressed down.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <returns>Returns true if an axis with this name was pressed down, false otherwise.</returns>
    public static bool GetAxisDown(string _name)
    {
        return axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.KeyDown);
    }

    /// <summary>
    /// Get if an axis with a specified name was released.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <returns>Returns true if an axis with this name was released, false otherwise.</returns>
    public static bool GetAxisUp(string _name)
    {
        return axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.KeyUp);
    }
    #endregion

    #region Unity Methods-Like
    /// <summary>
    /// My custom awake method, that should be called on game load.
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        Debug.Log("Mhmm...");

        // Get all axis from the project
        

        // Subscribe a custom method to the input update system
        NativeInputSystem.onUpdate -= MyUpdate;
        NativeInputSystem.onUpdate += MyUpdate;
    }

    /// <summary>
    /// My custom update method, that should be called on MonoBehaviour Update.
    /// </summary>
    private static void MyUpdate(NativeInputUpdateType _type, int _int, IntPtr _intPtr)
    {
        // Updates each axis state
        foreach (TDS_AxisToInput _axis in axis)
        {
            _axis.UpdateState();
        }
    }
    #endregion

    #endregion
}
