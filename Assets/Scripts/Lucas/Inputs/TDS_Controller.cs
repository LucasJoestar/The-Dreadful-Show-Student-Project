using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class TDS_Controller
{
    #region Fields & Properties
    /// <summary>
    /// Name of the controller.
    /// </summary>
    [SerializeField] private string name = "New Controller";


    /// <summary>
    /// All axis associated with this controller.
    /// </summary>
    [SerializeField] private TDS_AxisToInput[] axis = new TDS_AxisToInput[] { };

    /// <summary>Public accessor for <see cref="axis"/>.</summary>
    public TDS_AxisToInput[] Axis { get { return axis; } }

    /// <summary>
    /// All buttons associated with this controller.
    /// </summary>
    [SerializeField] private TDS_Button[] buttons = new TDS_Button[] { };

    /// <summary>Public accessor for <see cref="buttons"/>.</summary>
    public TDS_Button[] Buttons { get { return buttons; } }
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a brand new controller.
    /// </summary>
    public TDS_Controller()
    {
        name = "New Controller";

        AxisType[] _axis = (AxisType[])Enum.GetValues(typeof(AxisType));

        axis = new TDS_AxisToInput[_axis.Length];
        for (int _i = 0; _i < _axis.Length; _i++)
        {
            axis[_i] = new TDS_AxisToInput(_axis[_i].ToString());
        }

        ButtonType[] _buttons = (ButtonType[])Enum.GetValues(typeof(ButtonType));

        buttons = new TDS_Button[_buttons.Length];
        for (int _i = 0; _i < _buttons.Length; _i++)
        {
            buttons[_i] = new TDS_Button(_buttons[_i].ToString());
        }
    }
    #endregion

    #region Methods

    #region Axis
    /// <summary>
    /// Get the value of a certain axis.
    /// </summary>
    /// <param name="_name">Axis to get value.</param>
    /// <returns>Returns the value of the given axis.</returns>
    public float GetAxis(AxisType _axis)
    {
        return Input.GetAxis(axis[(int)_axis].AxisName);
    }

    /// <summary>
    /// Get if an certain axis is held down.
    /// </summary>
    /// <param name="_name">Axis to check state.</param>
    /// <returns>Returns true if the axis is held, false otherwise.</returns>
    public bool GetAxisHeld(AxisType _axis)
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
    /// Get if an certain axis is pressed down.
    /// </summary>
    /// <param name="_name">Axis to check state.</param>
    /// <param name="_value">Int to get axis value.</param>
    /// <returns>Returns true if the axis is pressed down, false otherwise.</returns>
    public bool GetAxisDown(AxisType _axis, out int _value)
    {
        _value = (int)Mathf.Sign(Input.GetAxis(axis[(int)_axis].AxisName));
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
