using UnityEngine;

public static class TDS_Extensions 
{
    /* TDS_Extensions :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	References all extension methods from all scripts of this project.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Extensions script.
     *	
     *	    - Added a Bool, a Float & a Line Renderer section.
     *	    - Added the ToSign method in the Bool section.
     *	    - Added the two SupToSign methods in the Float section.
     *	    - Added the DrawTrajectory method in the Line Renderer section.
	 *
	 *	-----------------------------------
	*/


    #region Methods

    #region Bool
    /// <summary>
    /// Check if this bool is equal to true or false, and return the result as a 1 or -1 value.
    /// </summary>
    /// <param name="_value">Value to check.</param>
    /// <returns>If this boolean is true, return 1 ; if false, return - 1.</returns>
    public static sbyte ToSign(this bool _value)
    {
        if (_value) return 1;
        return -1;
    }
    #endregion

    #region Float
    /// <summary>
    /// Check if this float is superior to another, and return the result as a 1, 0 or -1 value.
    /// </summary>
    /// <param name="_value">Value to check.</param>
    /// <param name="_compareValue">Value to compare to.</param>
    /// <returns>Returns 1 if this value if superior to the second, 0 if equals & -1 if inferior.</returns>
    public static sbyte SupToSign(this float _value, float _compareValue)
    {
        if (_value > _compareValue) return 1;
        if (_value == _compareValue) return 0;
        return -1;
    }

    /// <summary>
    /// Check if this float is superior to another, and return the result as a 1, 0 or -1 value.
    /// </summary>
    /// <param name="_value">Value to check.</param>
    /// <param name="_compareValue">Value to compare to.</param>
    /// <returns>Returns 1 if this value if superior to the second, 0 if equals & -1 if inferior.</returns>
    public static sbyte SupToSign(this float _value, int _compareValue)
    {
        if (_value > _compareValue) return 1;
        if (_value == _compareValue) return 0;
        return -1;
    }
    #endregion

    #region Int
    /// <summary>
    /// Returns this int as a boolean depending on its sign.
    /// </summary>
    /// <param name="_value">Value to check.</param>
    /// <returns>Returns true if the int has a value superior or equal to zero, false if negative.</returns>
    public static bool ToBool(this int _value)
    {
        if (_value < 0) return false;
        return true;
    }
    #endregion

    #region Line Renderer
    /// <summary>
    /// Draws a trajectory using an array of positions.
    /// </summary>
    /// <param name="_value">Line renderer used to draw the trajectory.</param>
    /// <param name="_positions">All positions of the trajectory used to render it. The more you add, the more precise it is.</param>
    public static void DrawTrajectory(this LineRenderer _value, Vector3[] _positions)
    {
        _value.positionCount = _positions.Length;
        _value.SetPositions(_positions);
    }
    #endregion

    #endregion
}
