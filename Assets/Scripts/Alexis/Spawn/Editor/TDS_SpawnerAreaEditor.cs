using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary; 

[CustomEditor(typeof(TDS_SpawnerArea))]
public class TDS_SpawnerAreaEditor : Editor 
{
    /* TDS_SpawnerAreaEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Editor class of the TDS_SpawnerArea class]
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
	 *	[Initialisation of the TDS_SpawnerArea Editor class]
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    #region FoldOut
    /// <summary>Backing field for <see cref="AreSpawnerAreaComponentsUnfolded"/></summary>
    private bool areSpawnerAreaComponentsUnfolded = false;
    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    public bool AreSpawnerAreaComponentsUnfolded
    {
        get { return areSpawnerAreaComponentsUnfolded; }
        set
        {
            areSpawnerAreaComponentsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areSpawnerAreaComponentsUnfolded", value);
        }
    }

    /// <summary>Backing field for <see cref="AreSpawnerAreaSettingsUnfolded"/></summary>
    private bool areSpawnerAreaSettingsUnfolded = false;
    /// <summary>
    /// Are the components of the Character class unfolded for editor ?
    /// </summary>
    public bool AreSpawnerAreaSettingsUnfolded
    {
        get { return areSpawnerAreaSettingsUnfolded; }
        set
        {
            areSpawnerAreaSettingsUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("areSpawnerAreaSettingsUnfolded", value);
        }
    }
    #endregion

    #region Components and references
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.photonView"/> of type <see cref="PhotonView"/>.</summary>
    private SerializedProperty photonView = null;
    #endregion

    #region Variables
    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.isLooping"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isLooping = null;

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.waves"/> of type <see cref="List{TDS_Wave}"/>.</summary>
    private SerializedProperty waves = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Display components and references
    /// </summary>
    void DrawComponentsAndReferences()
    {
        TDS_EditorUtility.ObjectField("Photon View", "Photon view of this area.", photonView, typeof(CustomNavMeshAgent));
        GUILayout.Space(3);
    }

    /// <summary>
    /// Sraw spawner Area Editor
    /// </summary>
    void DrawSpawnerAreaEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        
        // Button to show or not the Character class components
        if (TDS_EditorUtility.Button("Components & References", "Wrap / unwrap Components & References settings", TDS_EditorUtility.HeaderStyle)) AreSpawnerAreaComponentsUnfolded = !areSpawnerAreaComponentsUnfolded;

        // If unfolded, draws the custom editor for the Components & References
        if (areSpawnerAreaComponentsUnfolded)
        {
            DrawComponentsAndReferences();
        }

        EditorGUILayout.EndVertical();
        GUILayout.Space(15);
        EditorGUILayout.BeginVertical("HelpBox");

        // Button to show or not the Character class settings
        if (TDS_EditorUtility.Button("Settings", "Wrap / unwrap settings", TDS_EditorUtility.HeaderStyle)) AreSpawnerAreaSettingsUnfolded = !areSpawnerAreaSettingsUnfolded;

        // If unfolded, draws the custom editor for the sttings
        if (areSpawnerAreaSettingsUnfolded)
        {
            DrawSettings();
        }

        EditorGUILayout.EndVertical();

        // Applies all modified properties on the SerializedObjects
        serializedObject.ApplyModifiedProperties();

        GUI.backgroundColor = _originalColor;
    }

    /// <summary>
    /// Display settings
    /// </summary>
    void DrawSettings()
    {
        // Draw a header for the Spawner Area global settings
        EditorGUILayout.LabelField("Global settings", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.Toggle("is Looping", "Is the area start again when all the waves are cleared.", isLooping);
        GUILayout.Space(10);
        GUITools.ActionButton("Add Wave", waves.InsertArrayElementAtIndex, 0, Color.white, Color.white);
        // SerializedProperty _prop;
        // Rect _r;
        // Rect _lastRect;
        // GUIContent _content; 
        for (int i = 0; i < waves.arraySize; i++)
        {
            GUILayout.Space(10);
            DrawWave(i);
        }
    }

    /// <summary>
    /// Draw the wave settings
    /// </summary>
    /// <param name="_index">indew of the wave in the array</param>
    void DrawWave(int _index)
    {
        //Get the serialized Property 
        SerializedProperty _prop = serializedObject.FindProperty($"{waves.name}").GetArrayElementAtIndex(_index);
        float _size = 50; 
        if(_prop.FindPropertyRelative("isWaveFoldOut").boolValue)
        {
            _size += (_prop.FindPropertyRelative("spawnPoints").arraySize + 1) * 25; 
        }
        // Get the last rect drawn
        Rect _lastRect = GUILayoutUtility.GetLastRect();
        // Set a new based on the last rect position
        Rect _r = new Rect(_lastRect.position.x, _lastRect.position.y + 4, _lastRect.width, _size);
        // Reserve the rect
        _r = GUILayoutUtility.GetRect(_r.width, _r.height, GUIStyle.none);
        GUIContent _content = new GUIContent($"Waves n°{_index}");
        // Draw the Wave Editor
        EditorGUI.PropertyField(_r, _prop, _content);
        // Draw a button to destroy the wave
        GUITools.ActionButton("Delete Wave", waves.DeleteArrayElementAtIndex, _index, Color.white, Color.white);
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        //Get the serialized properties from the serializedObject
        photonView = serializedObject.FindProperty("photonView");

        isLooping = serializedObject.FindProperty("isLooping");
        waves = serializedObject.FindProperty("waves");

        //Load the editor folded and unfolded values of this class
        areSpawnerAreaComponentsUnfolded = EditorPrefs.GetBool("areSpawnerAreaComponentsUnfolded", areSpawnerAreaComponentsUnfolded);
        areSpawnerAreaSettingsUnfolded = EditorPrefs.GetBool("areSpawnerAreaSettingsUnfolded", areSpawnerAreaSettingsUnfolded);
    }

    public override void OnInspectorGUI()
    {
        DrawSpawnerAreaEditor(); 
    }

    private void OnSceneGUI()
    {
        for (int i = 0; i < waves.arraySize; i++)
        {
            SerializedProperty _wave = waves.GetArrayElementAtIndex(i); 
            if(_wave.FindPropertyRelative("isWaveFoldOut").boolValue)
            {
                GUI.color = _wave.FindPropertyRelative("debugColor").colorValue;
                Handles.color = GUI.color; 
                for (int j = 0; j < _wave.FindPropertyRelative("spawnPoints").arraySize; j++)
                {
                    SerializedProperty _p = _wave.FindPropertyRelative($"spawnPoints").GetArrayElementAtIndex(j);
                    Handles.Label(_p.FindPropertyRelative("spawnPosition").vector3Value + Vector3.up, $"Spawn Point n°{j}");
                    Handles.DrawWireDisc(_p.FindPropertyRelative("spawnPosition").vector3Value, Vector3.up, _p.FindPropertyRelative("spawnRange").floatValue);
                    _p.FindPropertyRelative("spawnPosition").vector3Value = Handles.PositionHandle(_p.FindPropertyRelative("spawnPosition").vector3Value, Quaternion.identity);
                }
            }
        }
        serializedObject.ApplyModifiedProperties(); 
    }
    #endregion

    #endregion
}
