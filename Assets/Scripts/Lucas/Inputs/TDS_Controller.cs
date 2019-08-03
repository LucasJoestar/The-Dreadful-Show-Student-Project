using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class TDS_Controller
{
    #region Fields & Properties
    /// <summary>
    /// All axis associated with this controller.
    /// </summary>
    [SerializeField] private TDS_AxisToInput[] axis = new TDS_AxisToInput[] { };

    /// <summary>
    /// All buttons associated with this controller.
    /// </summary>
    [SerializeField] private TDS_Button[] buttons = new TDS_Button[] { };
    #endregion

    #region Methods

    #region Axis
    /// <summary>
    /// Get if an certain axis is held down.
    /// </summary>
    /// <param name="_name">Axis to check state.</param>
    /// <returns>Returns true if the axis is held, false otherwise.</returns>
    public bool GetAxis(AxisType _axis)
    {
        return (axis[(int)_axis].LastState == AxisState.Key) || (axis[(int)_axis].LastState == AxisState.KeyDown);
    }

    /// <summary>
    /// Get if an certain axis is pressed down.
    /// </summary>
    /// <param name="_name">Axis to check state.</param>
    /// <returns>Returns true if the axis is pressed down, false otherwise.</returns>
    public bool GetAxisDown(AxisType _axis)
    {
        return axis[(int)_axis].LastState == AxisState.KeyDown;
    }

    /// <summary>
    /// Get if an certain axis is released.
    /// </summary>
    /// <param name="_name">Axis to check state.</param>
    /// <returns>Returns true if the axis is released, false otherwise.</returns>
    public bool GetAxisUp(AxisType _axis)
    {
        return axis[(int)_axis].LastState == AxisState.KeyUp;
    }
    #endregion

    #region Buttons
    /// <summary>
    /// Get if a button is held down.
    /// </summary>
    /// <param name="_name">Button to check state.</param>
    /// <returns>Returns true if the button is held, false otherwise.</returns>
    public bool GetButton(ButtonType _button)
    {
        TDS_Button _selected = buttons[(int)_button];

        // Get if a key of the button is held
        if (_selected.Keys.Any(k => Input.GetKey(k))) return true;

        // If not, return if the associated input is held
        return (_selected.Axis.LastState == AxisState.Key) || (_selected.Axis.LastState == AxisState.KeyDown);
    }

    /// <summary>
    /// Get if a button is pressed down.
    /// </summary>
    /// <param name="_name">Button to check state.</param>
    /// <returns>Returns true if the button is pressed down, false otherwise.</returns>
    public bool GetButtonDown(ButtonType _button)
    {
        TDS_Button _selected = buttons[(int)_button];

        // Get if a key of the button is held
        if (_selected.Keys.Any(k => Input.GetKeyDown(k))) return true;

        // If not, return if the associated input is held
        return _selected.Axis.LastState == AxisState.KeyDown;
    }

    /// <summary>
    /// Get if a button is released.
    /// </summary>
    /// <param name="_name">Button to check state.</param>
    /// <returns>Returns true if the button is released, false otherwise.</returns>
    public bool GetButtonUp(ButtonType _button)
    {
        TDS_Button _selected = buttons[(int)_button];

        // Get if a key of the button is held
        if (_selected.Keys.Any(k => Input.GetKeyUp(k))) return true;

        // If not, return if the associated input is held
        return _selected.Axis.LastState == AxisState.KeyUp;
    }
    #endregion

    #endregion
}
