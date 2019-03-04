using System;
using System.Linq;
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
    /// Name of the button, used to reference it.
    /// </summary>
    public string Name
    {
        get { return name; }
        private set { name = value; }
    }

    /// <summary>
    /// All keys used to detect that button.
    /// </summary>
    public KeyCode[] Keys = new KeyCode[] { };
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new button with a given name.
    /// </summary>
    /// <param name="_name">Name of the newly created button.</param>
    public TDS_Button(string _name)
    {
        Name = _name;
    }
    #endregion
}
