using System;
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
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this script.
    /// </summary>
    public static TDS_InputManager Instance = null;
    #endregion

    #region Methods

    #region Original Methods
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
            if (!string.IsNullOrEmpty(_button.Axis.AxisName) && (_button.Axis.AxisName != "Unknown Axis"))
            {
                OnUpdate += _button.Axis.UpdateState;
            }
        }
    }
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

        foreach (TDS_Controller _controller in TDS_GameManager.InputsAsset.Controllers)
        {
            SubscribeController(_controller);
        }

        #if UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined;
        #endif
    }

    Vector3 mousePosition = Vector3.zero;
    float mouseTimer = 0;
    bool isMouseActive = true;

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update()
    {
        // Calls the OnUpdate event
        OnUpdate?.Invoke();

        if (Input.mousePosition != mousePosition)
        {
            mousePosition = Input.mousePosition;
            mouseTimer = 0;

            if (!isMouseActive)
            {
                isMouseActive = true;
                Cursor.visible = true;
            }
        }
        else if (isMouseActive)
        {
            mouseTimer += Time.deltaTime;

            if (mouseTimer > 2)
            {
                isMouseActive = false;
                Cursor.visible = false;
            }
        }
    }
#endregion

#endregion
}
