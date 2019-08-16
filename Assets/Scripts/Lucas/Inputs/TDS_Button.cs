using System;
using UnityEngine;

[Serializable]
public class TDS_Button 
{
    /* TDS_Button :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Class used to stock informations about an input bouton.
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
	 *	    - Added the Name property.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[13 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Button class.
     *	
     *	    - Added the Name & Keys fields.
     *	    - Added the one string parameter constructor.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="Name"/>.</summary>
    [SerializeField] private string name = "New Button";

    /// <summary>
    /// Name of the button.
    /// </summary>
    public string Name { get { return name; } }

    /// <summary>Backing field for <see cref="Axis"/>.</summary>
    [SerializeField] private TDS_AxisToInput axis = new TDS_AxisToInput();

    /// <summary>
    /// Axis transformed to input associated with this button.
    /// </summary>
    public TDS_AxisToInput Axis { get { return axis; } }

    /// <summary>Backing field for <see cref="Keys"/>.</summary>
    [SerializeField] private KeyCode[] keys = new KeyCode[] { };

    /// <summary>
    /// All keys used to detect that button.
    /// </summary>
    public KeyCode[] Keys { get { return keys; } }
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a simple new button.
    /// </summary>
    public TDS_Button() { }

    /// <summary>
    /// Creates a new button with a given name.
    /// </summary>
    /// <param name="_name">Name of the button.</param>
    public TDS_Button(string _name)
    {
        name = _name;
    }
    #endregion
}
