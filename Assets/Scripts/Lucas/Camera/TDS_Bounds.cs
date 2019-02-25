using System;
using UnityEngine;

[Serializable]
public class TDS_Bounds 
{
    /* TDS_Bounds :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Bounds object used to clamp the camera between a min. & a max. value in the X & Z axis.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[25 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Bounds class.
     *	
     *	    - Added the XBounds & ZBounds fields.
     *	    - Added the two Vector2 parameters constructor.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// X min. & max. values used for these bounds.
    /// (X value for min., Y value for max.)
    /// </summary>
    public Vector2 XBounds = Vector2.one;

    /// <summary>
    /// Z min. & max. values used for these bounds.
    /// (X value for min., Y value for max.)
    /// </summary>
    public Vector2 ZBounds = Vector2.one;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new Bounds object with specified X & Z limits.
    /// </summary>
    /// <param name="_xBounds">Bounds on X axis.</param>
    /// <param name="_zBounds">Bounds on Z axis.</param>
    public TDS_Bounds(Vector2 _xBounds, Vector2 _zBounds)
    {
        XBounds = _xBounds;
        ZBounds = _zBounds;
    }
    #endregion
}
