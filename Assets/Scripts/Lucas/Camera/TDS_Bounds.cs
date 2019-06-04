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
     *	Date :			[03 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Set new Bounds system with 4 float instead of 2 vector2.
	 *
	 *	-----------------------------------
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
    /// X maximum value of the bounds.
    /// </summary>
    public float XMax { get { return XMaxVector.x; } }

    /// <summary>
    /// Vector for X maximum value of the bounds.
    /// </summary>
    public Vector3 XMaxVector = Vector3.zero;

    /// <summary>
    /// X minimum value of the bounds.
    /// </summary>
    public float XMin { get{ return XMinVector.x; } }

    /// <summary>
    /// Vector for X minimum value of the bounds.
    /// </summary>
    public Vector3 XMinVector = Vector3.zero;

    /// <summary>
    /// Z maximum value of the bounds.
    /// </summary>
    public float ZMax { get { return ZMaxVector.z; } }

    /// <summary>
    /// Vector for Z minimum value of the bounds.
    /// </summary>
    public Vector3 ZMinVector = Vector3.zero;

    /// <summary>
    /// Z minimum value of the bounds.
    /// </summary>
    public float ZMin { get { return ZMinVector.z; } }

    /// <summary>
    /// Vector for Z maximum value of the bounds.
    /// </summary>
    public Vector3 ZMaxVector = Vector3.zero;
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new Bounds object.
    /// </summary>
    /// <param name="_xMin">X minimum value of the bounds.</param>
    /// <param name="_xMax">X maximum value of the bounds.</param>
    /// <param name="_zMin">Z minimum value of the bounds.</param>
    /// <param name="_zMax">Z maximum value of the bounds.</param>
    public TDS_Bounds(float _xMin, float _xMax, float _zMin, float _zMax)
    {
        XMinVector = new Vector3(_xMin, 0, 0);
        XMaxVector = new Vector3(_xMax, 0, 0);
        ZMinVector = new Vector3(0, 0, _zMin);
        ZMaxVector = new Vector3(0, 0, _zMax);
    }

    /// <summary>
    /// Creates a new Bounds object.
    /// </summary>
    /// <param name="_xMin">X minimum value of the bounds.</param>
    /// <param name="_xMax">X maximum value of the bounds.</param>
    /// <param name="_zMin">Z minimum value of the bounds.</param>
    /// <param name="_zMax">Z maximum value of the bounds.</param>
    public TDS_Bounds(Vector3 _xMin, Vector3 _xMax, Vector3 _zMin, Vector3 _zMax)
    {
        XMinVector = _xMin;
        XMaxVector = _xMax;
        ZMinVector = _zMin;
        ZMaxVector = _zMax;
    }
    #endregion
}
