using System;
using UnityEngine;

[Serializable]
public class TDS_VirtualSphere 
{
    /* TDS_VirtualSphere :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Class used to create a virtual sphere, as a serializable class, with all its setttings.
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
	 *	Creation of the TDS_VirtualSphere class.
     *	
     *	    - Added the Color, Radius, WhatTouch, TriggerInteraction & LocalPosition fields.
     *	    - Added the DrawGizmos & Overlap methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Color of the virtual sphere, used to draw its gizmos.
    /// </summary>
    public Color Color = Color.green;

    /// <summary>
    /// Radius of the sphere.
    /// </summary>
    public float Radius = 1;

    /// <summary>
    /// LayerMask of what to detect when using this sphere for overlap.
    /// </summary>
    public LayerMask WhatTouch = new LayerMask();

    /// <summary>
    /// Trigger Interaction when using this sphere for overlap.
    /// </summary>
    public QueryTriggerInteraction TriggerInteraction = QueryTriggerInteraction.Ignore;

    /// <summary>
    /// Local position of the sphere, regarding the transform of the object this script is attached to.
    /// </summary>
    public Vector3 LocalPosition = Vector3.zero;
	#endregion

	#region Methods
    /// <summary>
    /// Call this method in a MonoBehaviour OnDrawGizmos method to draw this sphere gizmos.
    /// </summary>
    /// <param name="_parentPosition">Position in relation to which this sphere should by drawn, in world space. Mostly the position of the game object this script is attached to.</param>
    public void DrawGizmos(Vector3 _parentPosition)
    {
        Color _original = Gizmos.color;
        Gizmos.color = Color;

        Gizmos.DrawWireSphere(_parentPosition + LocalPosition, Radius);

        Gizmos.color = _original;
    }

    /// <summary>
    /// Overlaps in the sphere to get all colliders in it.
    /// </summary>
    /// <param name="_parentPosition">Position in relation to which this sphere should by drawn, in world space. Mostly the position of the game object this script is attached to.</param>
    /// <returns>Returns all colliders touching the sphere.</returns>
    public Collider[] Overlap(Vector3 _parentPosition)
    {
        return Physics.OverlapSphere(_parentPosition + LocalPosition, Radius, WhatTouch, TriggerInteraction);
    }
	#endregion
}
