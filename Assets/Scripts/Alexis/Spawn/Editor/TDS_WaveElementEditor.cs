using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary; 

[CustomPropertyDrawer(typeof(TDS_WaveElement))]
public class TDS_WaveElementEditor : PropertyDrawer
{
    /* TDS_WaveElementEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *  Editor of the WaveElement class
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[12/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation de la class TDS_WaveElementEditor]
     *	    - Implémentation de la méthode OnGUI pour afficher les settings de WaveElement
	 *
	 *	-----------------------------------
	*/

    #region Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        //Int sliders to display and modify the min and max random Spawn
        Rect _rect = new Rect(position.position.x, position.position.y, position.width - 25, 20);
        property.FindPropertyRelative("minRandomSpawn").intValue = EditorGUI.IntSlider(_rect, "Min Random Spawn",  property.FindPropertyRelative("minRandomSpawn").intValue, 0, property.FindPropertyRelative("maxRandomSpawn").intValue);
        _rect = new Rect(position.position.x, position.position.y + 25, position.width - 25, 20);
        property.FindPropertyRelative("maxRandomSpawn").intValue = EditorGUI.IntSlider(_rect, "Max Random Spawn", property.FindPropertyRelative("maxRandomSpawn").intValue, property.FindPropertyRelative("minRandomSpawn").intValue, 10);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("Box");
        GUILayout.Label("Normal Spawns", TDS_EditorUtility.HeaderStyle);
        // Display the settings of the normal spawning Informations
        for (int i = 0; i < property.FindPropertyRelative("spawningInformations").arraySize; i++)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative($"spawningInformations.Array.data[{i}]"));
            //Create a button to remove the spawning informations
            GUITools.ActionButton("Delete this info", property.FindPropertyRelative($"spawningInformations.Array").DeleteArrayElementAtIndex, i, Color.white, Color.black); 
        }
        GUILayout.EndVertical();

        //GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Random Spawns", TDS_EditorUtility.HeaderStyle);
        // Display the settings of the random spawning Informations
        for (int i = 0; i < property.FindPropertyRelative("randomSpawningInformations").arraySize; i++)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative($"randomSpawningInformations.Array.data[{i}]"));
            //Create a button to remove the spawning informations
            GUITools.ActionButton("Delete this info", property.FindPropertyRelative($"randomSpawningInformations.Array").DeleteArrayElementAtIndex, i, Color.white, Color.black);
        }
        GUILayout.Space(5);
        property.FindPropertyRelative("resetDecreasingCount").boolValue = EditorGUILayout.Toggle("Reset Decreasing Count", property.FindPropertyRelative("resetDecreasingCount").boolValue); 
        GUILayout.EndVertical();

    }
    #endregion
}
