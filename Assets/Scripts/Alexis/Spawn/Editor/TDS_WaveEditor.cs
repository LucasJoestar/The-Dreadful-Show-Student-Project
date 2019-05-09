using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; 

[CustomPropertyDrawer(typeof(TDS_Wave))]
public class TDS_WaveEditor : PropertyDrawer 
{
    /* TDS_WaveEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Draw Wave editor]
     *	    - Show Every spawn point contained in the wave and display a button to edit them or delete them
     *	    - Display a Color field to select the debug color of the wave
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[14/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation de la class TDS_WaveEditor]
	 *      -  Moving the InitWindow Method from TDS_SpawnerAreaEditor
     *      - Implementing the ONGUI Method to display the custom editor
     *      
	 *	-----------------------------------
	*/

    #region Methods

    #region Original Methods

    /// <summary>
    /// Init a window to edit the spawn point at the index i in the list of spawn points
    /// </summary>
    /// <param name="_pointIndex">index of the edited point</param>
    void InitWindow(SerializedProperty property, int _pointIndex)
    {
        //Create window
        TDS_SpawnPointEditorWindow _window = (TDS_SpawnPointEditorWindow)EditorWindow.GetWindow(typeof(TDS_SpawnPointEditorWindow));
        //Get Serialized Property
        _window.Init(property);
        _window.Show();
    }
    #endregion

    #region Unity Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Begin the property drawer
        EditorGUI.BeginProperty(position, label, property);

        //Get a rect to draw a label
        Rect _rect = new Rect(position.position.x , position.position.y, position.width / 2, 20);
        EditorGUI.LabelField(_rect, label, TDS_EditorUtility.HeaderStyle);

        // Get a rect to draw a color field for the property debugColor
        _rect = new Rect(position.position.x + position.width/2, position.position.y, position.width/2, 20);
        property.FindPropertyRelative("debugColor").colorValue = EditorGUI.ColorField(_rect, property.FindPropertyRelative("debugColor").colorValue);

        // Get a rect for the fold out
        _rect = new Rect(position.position.x + 10, _rect.position.y + 25, position.width - 10, 20);
        property.FindPropertyRelative("isWaveFoldOut").boolValue = EditorGUI.Foldout(_rect, property.FindPropertyRelative("isWaveFoldOut").boolValue, new GUIContent("Spawn Points"), true); 

        // Display them if the fold out is true
        if(property.FindPropertyRelative("isWaveFoldOut").boolValue)
        {
            //Is the wave activated by event
            TDS_EditorUtility.Toggle("Is Activated by event", "Are the enemies of this wave starting behaviour on event", property.FindPropertyRelative("isActivatedByEvent")); 
            //Get the size of the array 
            int _arraySize = property.FindPropertyRelative("spawnPoints").arraySize;
            GUIContent _label;
            SerializedProperty _pointProperty;
            for (int i = 0; i < _arraySize; i++)
            {
                //Get the property
                _pointProperty = property.FindPropertyRelative($"spawnPoints.Array.data[{i}]");
                // Get a rect to draw the edition button
                _rect = new Rect(position.position.x, _rect.position.y + 25, position.width / 2, 20);
                _label = new GUIContent($"Edit Spawn Point n° {i}");
                //Open the SpawnPointEditorWindow for the spawn point
                if (GUI.Button(_rect, _label)) InitWindow(_pointProperty, i);

                // Get a rect to draw the delete button
                _rect = new Rect(position.position.x + position.width / 2, _rect.position.y, position.width / 2, 20);
                _label = new GUIContent($"Delete Spawn Point n° {i}");
                // Remove the point when the button is pressed
                if (GUI.Button(_rect, _label)) property.FindPropertyRelative("spawnPoints").DeleteArrayElementAtIndex(i);
            }
            // Draw a rect to display the add spawn point button
            _rect = new Rect(position.position.x, _rect.position.y + 25, position.width, 20);
            _label = new GUIContent($"Add Spawn Point");
            if (GUI.Button(_rect, _label))
            {
                // Add a new spawn point
                property.FindPropertyRelative("spawnPoints").InsertArrayElementAtIndex(0);
            }
        }      
        EditorGUI.EndProperty(); 
    }


    #endregion

    #endregion
}
