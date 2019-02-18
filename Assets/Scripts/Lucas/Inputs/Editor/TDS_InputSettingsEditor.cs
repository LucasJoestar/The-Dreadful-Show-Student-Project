using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_InputSettings))]
public class TDS_InputSettingsEditor : Editor
{
    /* TDS_InputEditorWindow :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *

	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[14 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_InputSettingsEditor class.
     *	
     *	    - Added the inputManager, axes, axisNames & buttons fields.
     *	    - Added the DrawEditor method.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Input Manager
    /// <summary>
    /// Unity Input Manager from the project.
    /// </summary>
    [SerializeField] private static SerializedObject inputManager = null;

    /// <summary>
    /// Axes from the Unity InputManager system.
    /// </summary>
    [SerializeField] private static SerializedProperty axes = null;
    #endregion

    #region SerializedProperties
    /// <summary>
    /// SerializedProperty for <see cref="TDS_InputSettings.AxisNames"/> of type <see cref="string"/>[].
    /// </summary>
    [SerializeField] private SerializedProperty axisNames = null;

    /// <summary>
    /// SerializedProperty for <see cref="TDS_InputSettings.Buttons"/> of type <see cref="TDS_Button"/>[].
    /// </summary>
    [SerializeField] private SerializedProperty buttons = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor for editing project inputs.
    /// </summary>
    public void DrawEditor()
    {
        // Update editing object(s) before display
        serializedObject.Update();
        inputManager.Update();

        EditorGUILayout.PropertyField(buttons, true);
        EditorGUILayout.PropertyField(axisNames, true);

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("To configure game axes, please use the Unity InputManager system . You can edit it just below.", MessageType.Info);
        GUILayout.Space(5);

        EditorGUILayout.PropertyField(axes, new GUIContent("Unity InputManager Axes"), true);

        // Apply modifications on object(s)
        serializedObject.ApplyModifiedProperties();
        inputManager.ApplyModifiedProperties();
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // If this editor should not exist, kill it
        if (this != TDS_InputEditorWindow.ScriptableObjectEditor)
        {
            DestroyImmediate(this);
            return;
        }

        // Get serializedProperties from the object
        buttons = serializedObject.FindProperty("buttons");
        axisNames = serializedObject.FindProperty("axisNames");

        // Get the Unity Input Manager of the project
        inputManager = new SerializedObject(AssetDatabase.LoadMainAssetAtPath("ProjectSettings/InputManager.asset"));
        axes = inputManager.FindProperty("m_Axes");

        //Debug.Log("Get Axes => " + axes.arraySize);

        axisNames.arraySize = axes.arraySize;
        serializedObject.ApplyModifiedProperties();
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawEditor();
    }
	#endregion

	#endregion
}
