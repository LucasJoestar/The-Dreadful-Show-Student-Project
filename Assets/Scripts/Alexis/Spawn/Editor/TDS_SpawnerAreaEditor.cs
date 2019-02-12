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

    private TDS_SpawnerArea p_target = null; 

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

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.wavesLength"/> of type <see cref="int"/>.</summary>
    private SerializedProperty wavesLength = null;

    /// <summary>SerializedProperty for <see cref="TDS_SpawnerArea.spawnPoints"/> of type <see cref="List{TDS_SpawnPoint}"/>.</summary>
    private SerializedProperty spawnPoints = null;
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
        GUILayout.Space(3);
        TDS_EditorUtility.IntSlider("Number of waves", "Number of waves in the area.", wavesLength, 1, 10);
        GUILayout.Space(10);
        GUITools.ActionButton("Add Spawn Point", p_target.AddSpawnPoint, Color.white, Color.white);
        for (int i = 0; i < spawnPoints.arraySize; i++)
        {
            EditorGUILayout.LabelField($"Point n°{i}", TDS_EditorUtility.LabelStyle);
            EditorGUILayout.BeginHorizontal();
            GUITools.ActionButton("Edit Point", InitWindow, i, Color.white, Color.white);
            GUITools.ActionButton("Delete Point", p_target.RemovePoint, i, Color.white, Color.white);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3); 
        }
    }

    /// <summary>
    /// Init a window to edit the spawn point at the index i in the list of spawn points
    /// </summary>
    /// <param name="_pointIndex">index of the edited point</param>
    void InitWindow(int _pointIndex)
    {
        //Create window
        TDS_SpawnPointEditorWindow _window = (TDS_SpawnPointEditorWindow)EditorWindow.GetWindow(typeof(TDS_SpawnPointEditorWindow));
        //Get Serialized Property
        SerializedProperty _prop = serializedObject.FindProperty($"{spawnPoints.name}.Array.data[{_pointIndex}]");
        _window.Init(_prop, p_target.SpawnPoints[_pointIndex]);
        _window.Show(); 
    }
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        //Set the edited target
        p_target = (TDS_SpawnerArea)target; 

        //Get the serialized properties from the serializedObject
        photonView = serializedObject.FindProperty("photonView");

        isLooping = serializedObject.FindProperty("isLooping");
        wavesLength = serializedObject.FindProperty("wavesLength");
        spawnPoints = serializedObject.FindProperty("spawnPoints");

        //Load the editor folded and unfolded values of this class
        areSpawnerAreaComponentsUnfolded = EditorPrefs.GetBool("areSpawnerAreaComponentsUnfolded", areSpawnerAreaComponentsUnfolded);
        areSpawnerAreaSettingsUnfolded = EditorPrefs.GetBool("areSpawnerAreaSettingsUnfolded", areSpawnerAreaSettingsUnfolded);
    }

    public override void OnInspectorGUI()
    {
        DrawSpawnerAreaEditor(); 
    }
    #endregion

    #endregion
}
