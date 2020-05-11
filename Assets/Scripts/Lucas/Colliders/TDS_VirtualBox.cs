using System;
using UnityEngine;

[Serializable]
public class TDS_VirtualBox 
{
    /* TDS_VirtualBox :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class used to create a virtual box, as a serializable class, with all its setttings.
     *	    
     *	    This can be useful for overlaps, if not wanting to use a collider.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[21 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_VirtualBox class.
     *	
     *	    - Added the Color, WhatTouch, TriggerInteraction, LocalPosition & Size fields ; and the Extents property.
     *	    - Added the DrawGizmos & Overlap methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

#if UNITY_EDITOR
    /// <summary>
    /// Color of the virtual box, used to draw its gizmos.
    /// </summary>
    public Color Color = Color.green;
#endif

    /// <summary>
    /// LayerMask of what to detect when using this box for overlap.
    /// </summary>
    public LayerMask WhatDetect = new LayerMask();

    /// <summary>
    /// Trigger Interaction when using this box for overlap.
    /// </summary>
    public QueryTriggerInteraction TriggerInteraction = QueryTriggerInteraction.Ignore;

    /// <summary>
    /// Extents of the box, that is half of the size.
    /// </summary>
    public Vector3 Extents { get { return Size / 2; } }

    /// <summary>
    /// Local position of the box, regarding the transform of the object this script is attached to.
    /// </summary>
    public Vector3 LocalPosition = Vector3.zero;

    /// <summary>
    /// Global size of the box.
    /// </summary>
    public Vector3 Size = Vector3.one;
    #endregion

    #region Methods

#if UNITY_EDITOR
    /// <summary>
    /// Call this method in a MonoBehaviour OnDrawGizmos method to draw this box gizmos.
    /// </summary>
    /// <param name="_parentPosition">Position in relation to which this box should by drawn, in world space. Mostly the position of the game object this script is attached to.</param>
    public void DrawGizmos(Vector3 _parentPosition)
    {
        Color _original = Gizmos.color;
        Gizmos.color = Color;

        Gizmos.DrawWireCube(_parentPosition + LocalPosition, Size);

        Gizmos.color = _original;
    }
#endif

    /// <summary>
    /// Overlaps in the box to get all colliders in it.
    /// </summary>
    /// <param name="_parentPosition">Position in relation to which this box should by drawn, in world space. Mostly the position of the game object this script is attached to.</param>
    /// <returns>Returns all colliders touching the box.</returns>
    public Collider[] Overlap(Vector3 _parentPosition)
    {
        return Physics.OverlapBox(_parentPosition + LocalPosition, Extents, Quaternion.identity, WhatDetect, TriggerInteraction);
    }
    #endregion
}
