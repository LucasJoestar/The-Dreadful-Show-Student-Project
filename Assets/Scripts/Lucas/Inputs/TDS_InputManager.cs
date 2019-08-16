using System;
using System.Linq;
using UnityEngine;

public class TDS_InputManager : MonoBehaviour
{
    /* TDS_Input :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	References all custom inputs of the project into a class
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[30 / 04 / 2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	    - Fix error on Unity Version 2018.3
     *	        -> Change My Update Method to a delegate on initialisation
     *	        -> Had to do unsafe code due to pointer
	 *
	 *	-----------------------------------
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
    /// <summary>
    /// Event called on the Input Manager update.
    /// </summary>
    public event Action OnUpdate = null;
    #endregion

    #region Fields / Properties
    /// <summary>
    /// Game inputs serialized object.
    /// </summary>
    [SerializeField] private static TDS_InputSO inputs = null;


    /// <summary>
    /// Name of the button used to cancel something.
    /// </summary>
    public const string CANCEL_BUTTON = "Cancel";

    /// <summary>
    /// Name of the button used to perform a catch.
    /// </summary>
    public const string CATCH_BUTTON = "Catch";

    /// <summary>
    /// Name of the button used to dodge.
    /// </summary>
    public const string DODGE_BUTTON = "Dodge";

    /// <summary>
    /// Name of the joystick D-Pad X axis.
    /// </summary>
    public const string D_PAD_X_Axis = "D-Pad X";

    /// <summary>
    /// Name of the joystick D-Pad Y axis.
    /// </summary>
    public const string D_PAD_Y_Axis = "D-Pad Y";

    /// <summary>
    /// Name of the button used to perform a heavy attack.
    /// </summary>
    public const string HEAVY_ATTACK_BUTTON = "Heavy Attack";

    /// <summary>
    /// Name of the axis used to move on the X axis.
    /// </summary>
    public const string HORIZONTAL_AXIS = "Horizontal";

    /// <summary>
    /// Name of the second axis used horizontally.
    /// </summary>
    public const string HORIZONTAL_ALT_AXIS = "Horizontal Alt";

    /// <summary>
    /// Name of the button used to interact with the environment.
    /// </summary>
    public const string INTERACT_BUTTON = "Interact";

    /// <summary>
    /// Name of the button used to jump.
    /// </summary>
    public const string JUMP_BUTTON = "Jump";

    /// <summary>
    /// Name of the button used to perform a light attack.
    /// </summary>
    public const string LIGHT_ATTACK_BUTTON = "Light Attack";

    /// <summary>
    /// Name of the button used to parry.
    /// </summary>
    public const string PARRY_BUTTON = "Parry";

    /// <summary>
    /// Name of the button used to get a snack.
    /// </summary>
    public const string SNACK_BUTTON = "Snack";

    /// <summary>
    /// Name of the button used to confirm something.
    /// </summary>
    public const string SUBMIT_BUTTON = "Submit";

    /// <summary>
    ///Name of the button used to perform the super attack.
    /// </summary>
    public const string SUPER_ATTACK_BUTTON = "Super Attack";

    /// <summary>
    /// Name of the button used to throw an object.
    /// </summary>
    public const string THROW_BUTTON = "Throw";

    /// <summary>
    /// Name of the button used to use the selected object.
    /// </summary>
    public const string USE_OBJECT_BUTTON = "Use Object";

    /// <summary>
    /// Name of the axis used to move on the Z axis.
    /// </summary>
    public const string VERTICAL_AXIS = "Vertical";

    /// <summary>
    /// Name of the second axis used vertically.
    /// </summary>
    public const string VERTICAL_ALT_AXIS = "Vertical Alt";
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this script.
    /// </summary>
    public static TDS_InputManager Instance = null;
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
        return inputs.Axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.Key || a.LastState == AxisState.KeyDown);
    }

    /// <summary>
    /// Get if an axis with a specified name is pressed down.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <returns>Returns true if an axis with this name is pressed down, false otherwise.</returns>
    public static bool GetAxisDown(string _name)
    {
        return inputs.Axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.KeyDown);
    }

    /// <summary>
    /// Get if an axis with a specified name is pressed down.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <param name="_value">Get the value of the pressed axis.</param>
    /// <returns>Returns true if an axis with this name is pressed down, false otherwise.</returns>
    public static bool GetAxisDown(string _name, out int _value)
    {
        if (GetAxisDown(_name))
        {
            _value = (int)Mathf.Sign(Input.GetAxis(_name));
            return true;
        }

        _value = 0;
        return false;
    }

    /// <summary>
    /// Get if an axis with a specified name is released.
    /// </summary>
    /// <param name="_name">Name of the axis to check.</param>
    /// <returns>Returns true if an axis with this name is released, false otherwise.</returns>
    public static bool GetAxisUp(string _name)
    {
        return inputs.Axis.Where(a => a.AxisName == _name).Any(a => a.LastState == AxisState.KeyUp);
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
        bool _isButtonHeld = inputs.Buttons.Where(b => b.Name == _name).FirstOrDefault().Keys.Any(k => Input.GetKey(k));

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
        bool _isButtonDown = inputs.Buttons.Where(b => b.Name == _name).FirstOrDefault().Keys.Any(k => Input.GetKeyDown(k));

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
        bool _isButtonUp = inputs.Buttons.Where(b => b.Name == _name).FirstOrDefault().Keys.Any(k => Input.GetKeyUp(k));

        // If one is, return true ; if not, return if an axis button is
        if (_isButtonUp) return true;
        else return GetAxisUp(_name);
    }
    #endregion

    #region Controller
    /// <summary>
    /// Subscribes a controller for axis update to this Input Manager.
    /// </summary>
    /// <param name="_controller">Controller to subscribe.</param>
    public void SubscribeController(TDS_Controller _controller)
    {
        foreach (TDS_AxisToInput _axis in _controller.Axis)
        {
            OnUpdate += _axis.UpdateState;
        }

        foreach (TDS_Button _button in _controller.Buttons)
        {
            if (_button.Axis != default(TDS_AxisToInput))
            {
                OnUpdate += _button.Axis.UpdateState;
                Debug.Log("Subscribe Axis => " + _button.Axis.AxisName);
            }
        }
    }
    #endregion

    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
            return;
        }
    }

    // Use this for initialization
    private void Start()
    {
        // Get input asset
        inputs = TDS_GameManager.InputsAsset;

        // Subscribes controllers input update
        if (PhotonNetwork.offlineMode)
        {
            foreach (TDS_PlayerInfo _player in TDS_GameManager.PlayersInfo)
            {
                SubscribeController(_player.Controller);
            }
        }
        else if (TDS_GameManager.PlayersInfo.Count > 0)
        {
            SubscribeController(TDS_GameManager.PlayersInfo[0].Controller);
        }
    }

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        // Updates each axis state
        foreach (TDS_AxisToInput _axis in inputs.Axis)
        {
            _axis.UpdateState();
        }

        // Calls the OnUpdate event
        OnUpdate?.Invoke();
    }
    #endregion

    #endregion
}
