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
        Rect _rect = new Rect(position.position.x, position.position.y, position.width - 25, 20);
        GUI.Label(_rect , "SPAWN POINT SETTINGS", TDS_EditorUtility.HeaderStyle);

        // Float slider to modify the spawnRange property
        _rect = new Rect(position.position.x, _rect.position.y + 25, position.width - 25, 20);
        property.FindPropertyRelative("spawnRange").floatValue = EditorGUI.Slider(_rect, "Spawn Range", property.FindPropertyRelative("spawnRange").floatValue, 0, 15);
        // Vector Field to modify the centerPosition property

        _rect = new Rect(position.position.x, _rect.position.y + 25, position.width - 25, 20);
        property.FindPropertyRelative("spawnPosition").vector3Value = EditorGUI.Vector3Field(_rect, "Spawn Position", property.FindPropertyRelative("spawnPosition").vector3Value);

        GUILayout.Space(_rect.position.y + 40); 
        
        // PropertyField to modify the waveElement property
        EditorGUILayout.PropertyField(property.FindPropertyRelative("waveElement"));
    }
    #endregion 
    #endregion
}

