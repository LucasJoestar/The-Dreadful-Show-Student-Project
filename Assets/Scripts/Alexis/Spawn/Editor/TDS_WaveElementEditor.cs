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
        TDS_EditorUtility.IntSlider("Minimum Random Spawn", "", property.FindPropertyRelative("minRandomSpawn"), 0, property.FindPropertyRelative("maxRandomSpawn").intValue);
        TDS_EditorUtility.IntSlider("Maximum Random Spawn", "", property.FindPropertyRelative("maxRandomSpawn"), property.FindPropertyRelative("minRandomSpawn").intValue, 10);
        GUILayout.Space(10); 
        // Display the settings of the normal spawning Informations
        GUILayout.Box("NORMAL SPAWNING"); 
        for (int i = 0; i < property.FindPropertyRelative("spawningInformations").arraySize; i++)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative($"spawningInformations.Array.data[{i}]"));
            //Create a button to remove the spawning informations
            GUITools.ActionButton("Delete this info", property.FindPropertyRelative($"spawningInformations.Array").DeleteArrayElementAtIndex, i, Color.white, Color.white); 
        }
        // Display the settings of the random spawning Informations
        GUILayout.Box("RANDOM SPAWNING");
        for (int i = 0; i < property.FindPropertyRelative("randomSpawningInformations").arraySize; i++)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative($"randomSpawningInformations.Array.data[{i}]"));
            //Create a button to remove the spawning informations
            GUITools.ActionButton("Delete this info", property.FindPropertyRelative($"randomSpawningInformations.Array").DeleteArrayElementAtIndex, i, Color.white, Color.white);
        }
        EditorGUILayout.HelpBox("", MessageType.None, true); 
    }
    #endregion
}
