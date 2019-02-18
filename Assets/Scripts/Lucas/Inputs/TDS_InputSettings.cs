using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_InputSettings : ScriptableObject 
{
    /* TDS_InputSettings :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Scriptable object containing all inputs of the project and their default values.
     *	This object can be edited through the Unity editor and loaded in game as default input settings.
     *	
     *	    When changing inputs related keys, changes should be written on disk with the PlayerPrefs class using Json and then be loaded in game ; since scriptable object cannot be edited on play, this object will always contain default values.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[14 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_InputSettings class.
     *	    - Added the INPUT_SO_DEFAULT_PATH constant field.
     *	    - Added the buttons & axisNames fields & properties.
	 *
	 *	-----------------------------------
	*/

    #region Utility
    /// <summary>
    /// Path of the input settings scriptable object from Resources folder.
    /// </summary>
    public const string INPUT_SO_DEFAULT_PATH = "Inputs/InputSettings";
    #endregion

    #region Fields / Properties
    /// <summary>Backing field for <see cref="Buttons"/>.</summary>
    [SerializeField] private TDS_Button[] buttons = new TDS_Button[] { };

    /// <summary>
    /// All custom buttons from this project.
    /// </summary>
    public TDS_Button[] Buttons
    {
        get { return buttons; }
        private set { buttons = value; }
    }

    /// <summary>Backing field for <see cref="AxisNames"/>.</summary>
    [SerializeField] private string[] axisNames = new string[] { };

    /// <summary>
    /// Name of all axis from this project.
    /// </summary>
    public string[] AxisNames
    {
        get { return axisNames; }
        private set
        {
            axisNames = value;
        }
    }
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    
    // Awake is called when the script instance is being loaded
    private void Awake()
    {

    }
	#endregion

	#endregion
}
