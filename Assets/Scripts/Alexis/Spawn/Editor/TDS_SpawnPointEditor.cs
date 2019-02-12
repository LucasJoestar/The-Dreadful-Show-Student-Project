using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TDS_SpawnPoint))]
public class TDS_SpawnPointEditor : PropertyDrawer 
{
    /* TDS_SpawnPointEditorWindow :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Editor of the Spawn Point class
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[11/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the editor class]
	 *  https://docs.unity3d.com/ScriptReference/PropertyDrawer.html
     *      - Implementation de la methode OnGUI pour afficher les settings 
	 *	-----------------------------------
	*/

    #region Methods
    #region Unity Method
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Label to title the part
        GUILayout.Label("SPAWN POINT SETTINGS", TDS_EditorUtility.HeaderStyle); 
        // Int slider to modify the waveIndex property
        TDS_EditorUtility.IntSlider("Wave Index", "", property.FindPropertyRelative("waveIndex"), 0, property.serializedObject.FindProperty("wavesLength").intValue);
        // Float slider to modify the spawnRange property
        TDS_EditorUtility.FloatSlider("Spawn Range", "", property.FindPropertyRelative("spawnRange"), 0, 15);
        // Vector Field to modify the centerPosition property
        TDS_EditorUtility.Vector3Field("Center Position", "", property.FindPropertyRelative("spawnPosition"));
        // PropertyField to modify the waveElement property
        EditorGUILayout.PropertyField(property.FindPropertyRelative("waveElement"));
    }
    #endregion 
    #endregion
}

