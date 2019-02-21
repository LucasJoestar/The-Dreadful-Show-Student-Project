using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal.Input;

public static class TDS_Input
{
    /* TDS_Input :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	References all custom inputs of the project into a static class, that subscribes itself to the Input Update.
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
	 *	    - Added the buttons field ; and the AxisNames & ButtonNames properties.
     *	    - Added the GetAxis, GetAxisDown, GetAxisUp, GetButton, GetButtonDown, GetButtonUp & ResetInputs methods.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Input class.
     *	
     *	    - Added the axis fields.
     *	    - Added the Initialize & MyUpdate methods.
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

    /// <summary>
    /// All custom buttons from this project.
    /// </summary>
    private static TDS_Button[] buttons = new TDS_Button[] { };

    /// <summary>
    /// Name of all axis from this project.
    /// </summary>
    public static string[] AxisNames { get; private set; } = new string[] { "Throw", "Parry", "Super Attack", "D-Pad X", "D-Pad Y" };

    /// <summary>
    /// Name of all custom buttons from this project.
    /// </summary>
    public static string[] ButtonNames { get; private set; } = new string[] { };

    /// <summary>
    /// Helper to update axis each two updates instead of all the time.
    /// </summary>
    private static bool doUpdate = true;
    #endregion

    #region Methods

    #region Original Methods

    #region Axis
    /// <summary>
    /// Get if an axis with a specified name is held down.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <returns>Returns true if an axis with this name is held, false otherwise.</returns>
    public static bool GetAxis(string _name)
    {
        return axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.Key || a.LastState == AxisState.KeyDown);
    }

    /// <summary>
    /// Get if an axis with a specified name is pressed down.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <returns>Returns true if an axis with this name is pressed down, false otherwise.</returns>
    public static bool GetAxisDown(string _name)
    {
        return axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.KeyDown);
    }

    /// <summary>
    /// Get if an axis with a specified name is released.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <returns>Returns true if an axis with this name is released, false otherwise.</returns>
    public static bool GetAxisUp(string _name)
    {
        return axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.KeyUp);
    }
    #endregion

    #region Buttons
    /// <summary>
    /// Get if this button is held down during this frame.
    /// </summary>
    /// <param name="_name">Name of the button to check.</param>
    /// <returns>Returns true if the button is held, false otherwise.</returns>
    public static bool GetButton(string _name)
    {
        // Get if a button with the given name is held
        bool _isButtonHeld = buttons.Where(b => b.Name == _name).FirstOrDefault().Keys.Any(k => Input.GetKey(k));

        // If one is, return true ; if not, return if an axis button is
        if (_isButtonHeld) return true;
        else return GetAxis(_name);
    }

    /// <summary>
    /// Get if this button is pressed down during this frame.
    /// </summary>
    /// <param name="_name">Name of the button to check.</param>
    /// <returns>Returns true if the button is pressed, false otherwise.</returns>
    public static bool GetButtonDown(string _name)
    {
        // Get if a button with the given name is pressed
        bool _isButtonDown = buttons.Where(b => b.Name == _name).FirstOrDefault().Keys.Any(k => Input.GetKeyDown(k));

        // If one is, return true ; if not, return if an axis button is
        if (_isButtonDown) return true;
        else return GetAxisDown(_name);
    }

    /// <summary>
    /// Get if this button is released during this frame.
    /// </summary>
    /// <param name="_name">Name of the button to check.</param>
    /// <returns>Returns true if the button is released, false otherwise.</returns>
    public static bool GetButtonUp(string _name)
    {
        // Get if a button with the given name is released
        bool _isButtonUp = buttons.Where(b => b.Name == _name).FirstOrDefault().Keys.Any(k => Input.GetKeyUp(k));

        // If one is, return true ; if not, return if an axis button is
        if (_isButtonUp) return true;
        else return GetAxisUp(_name);
    }
    #endregion

    #region Other
    /// <summary>
    /// Reset all configured inputs as in the scriptable object reference.
    /// </summary>
    public static void ResetInputs()
    {

    }
    #endregion

    #endregion

    #region Unity Methods-Like
    /// <summary>
    /// My custom awake method, that should be called on game load.
    /// </summary>
    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
        #if UNITY_EDITOR
        Debug.Log("Mhmm...");
        // Get axis & buttons informations from a scriptable object
#else
        // Get axis & buttons informations from a PlayerPref, or from a scriptable object if null
#endif
        // Creates an axis object for each axis name
        axis = new TDS_AxisToInput[AxisNames.Length];
        for (int _i = 0; _i < AxisNames.Length; _i++)
        {
            axis[_i] = new TDS_AxisToInput(AxisNames[_i]);
        }

        //Debug.Log("Start => " + Resources.Load<TDS_InputSettings>(TDS_InputSettings.INPUT_SO_DEFAULT_PATH).AxisNames.Length);
        // Subscribe a custom method to the input update system
        NativeInputSystem.onUpdate -= MyUpdate;
        NativeInputSystem.onUpdate += MyUpdate;
    }

    /// <summary>
    /// My custom update method, that should be called on MonoBehaviour Update.
    /// </summary>
    private static void MyUpdate(NativeInputUpdateType _type, int _int, IntPtr _intPtr)
    {
        if (doUpdate)
        {
            // Updates each axis state
            foreach (TDS_AxisToInput _axis in axis)
            {
                _axis.UpdateState();
            }
        }

        doUpdate = !doUpdate;
    }
    #endregion

    #endregion
}
