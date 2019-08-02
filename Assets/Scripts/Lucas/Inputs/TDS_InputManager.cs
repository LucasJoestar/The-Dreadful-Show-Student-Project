using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    #region Fields / Properties
    /// <summary>
    /// Game inputs serialized object.
    /// </summary>
    [SerializeField] private static TDS_InputSO inputs = null;

    /// <summary>
    /// Path from a resources folder where to find the input asset.
    /// </summary>
    private const string INPUT_ASSET_PATH = "Input/InputAsset";


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
    /// Name of the joystick right stick X axis.
    /// </summary>
    public const string RIGHT_STICK_X_Axis = "Right Stick X";

    /// <summary>
    /// Name of the joystick right stick Y axis.
    /// </summary>
    public const string RIGHT_STICK_Y_AXIS = "Right Stick Y";

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
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this script.
    /// </summary>
    public static TDS_InputManager Instance = null;
    #endregion

    #region Methods

    #region Original Methods

    #region Editor
    #if UNITY_EDITOR
    /// <summary>
    /// Creates a input asset if none exist
    /// </summary>
    [MenuItem("Tools/Create Input Asset")]
    public static void CreateInputAsset()
    {
        // If an input asset already exist, just return
        if (Resources.Load(INPUT_ASSET_PATH) != null) return;

        TDS_InputSO _inputAsset = ScriptableObject.CreateInstance<TDS_InputSO>();

        _inputAsset.Axis = new TDS_AxisToInput[] { new TDS_AxisToInput(D_PAD_X_Axis), new TDS_AxisToInput(D_PAD_Y_Axis), new TDS_AxisToInput(RIGHT_STICK_X_Axis), new TDS_AxisToInput(RIGHT_STICK_Y_AXIS), new TDS_AxisToInput(HORIZONTAL_AXIS), new TDS_AxisToInput(VERTICAL_AXIS) };

        _inputAsset.Buttons = new TDS_Button[] { new TDS_Button(CATCH_BUTTON), new TDS_Button(DODGE_BUTTON), new TDS_Button(HEAVY_ATTACK_BUTTON), new TDS_Button(INTERACT_BUTTON), new TDS_Button(JUMP_BUTTON), new TDS_Button(LIGHT_ATTACK_BUTTON), new TDS_Button(PARRY_BUTTON), new TDS_Button(SUPER_ATTACK_BUTTON), new TDS_Button(THROW_BUTTON), new TDS_Button(USE_OBJECT_BUTTON) };


        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, "Resources", System.IO.Path.GetDirectoryName(INPUT_ASSET_PATH)));

        AssetDatabase.CreateAsset(_inputAsset, System.IO.Path.Combine("Assets/Resources", INPUT_ASSET_PATH) + ".asset");
        AssetDatabase.SaveAssets();
    }
    #endif
    #endregion

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

        inputs = Resources.Load<TDS_InputSO>(INPUT_ASSET_PATH);
    }

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        // Updates each axis state
        foreach (TDS_AxisToInput _axis in inputs.Axis)
        {
            _axis.UpdateState();
        }
    }
    #endregion

    #endregion
}
