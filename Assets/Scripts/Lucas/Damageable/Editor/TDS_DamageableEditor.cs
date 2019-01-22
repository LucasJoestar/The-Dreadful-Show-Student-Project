using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Damageable), true), CanEditMultipleObjects]
public class TDS_DamageableEditor : Editor 
{
    /* TDS_DamageableEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Custom editor for the TDS_Damageable class.
     *  
     *      Allows to use properties & methods in the inspector.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[22 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_DamageableEditor script.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Foldouts
    /// <summary>
    /// Indicates if the editor for the Damageable class is unfolded or not.
    /// </summary>
    protected bool isDamageableUnfolded = true;
    #endregion

    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        // Records any changements on the editing objects to allow undo
        Undo.RecordObjects(targets, "Damageable script settings");

        // Updates the SerializedProperties to get the latest values
        serializedObject.Update();

        GUILayout.Space(10);

        isDamageableUnfolded = EditorGUILayout.Foldout(isDamageableUnfolded, "Damageable", true);

        // If unfolded, draws the custom editor for the Damageable class
        if (isDamageableUnfolded)
        {
            ((TDS_Damageable)target).HealthCurrent = EditorGUILayout.IntField("Health", ((TDS_Damageable)target).HealthCurrent);
            ((TDS_Damageable)target).HealthMax = EditorGUILayout.IntField("Health Max", ((TDS_Damageable)target).HealthMax);
        }

        // Applies all modified properties on the SerializedObjects
        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #endregion
}
