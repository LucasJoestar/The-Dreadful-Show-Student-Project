using System;
using System.Collections;
using System.Collections.Generic;
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
     *	    - Added a Line Renderer section.
     *	    - Added the DrawTrajectory method in the Line Renderer section.
	 *
	 *	-----------------------------------
	*/


    #region Methods

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
