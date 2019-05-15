using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_LevelBounds))]
public class TDS_LevelBoundsEditor : Editor 
{
    /* TDS_LevelBoundsEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor for a Level Bounds object.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[13 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_LevelBoundsEditor class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.topBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty topBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.leftBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty leftBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.rightBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty rightBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.bottomBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty bottomBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.doDestroyOnActivate"/> field of type <see cref="bool"/>.</summary>
    [SerializeField] private SerializedProperty doDestroyOnActivate = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.detectedTags"/> field of type <see cref="Tags"/>.</summary>
    [SerializeField] private SerializedProperty detectedTags = null;

    /// <summary>
    /// Int indicating the currently editing bound.
    /// </summary>
    private int editingBound = 0;
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Get serialized properties
        topBound = serializedObject.FindProperty("topBound");
        leftBound = serializedObject.FindProperty("leftBound");
        rightBound = serializedObject.FindProperty("rightBound");
        bottomBound = serializedObject.FindProperty("bottomBound");
        doDestroyOnActivate = serializedObject.FindProperty("doDestroyOnActivate");
        detectedTags = serializedObject.FindProperty("detectedTags");
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Color _originalColor = GUI.color;

        doDestroyOnActivate.boolValue = EditorGUILayout.Toggle(new GUIContent("Destroy on Activate", "Should this object be destroyed when activated"), doDestroyOnActivate.boolValue);

        GUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();

        topBound.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("Top Bound", "Top bound position"), topBound.vector3Value);

        GUI.color = editingBound == 1 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(50)))
        {
            if (editingBound != 1) editingBound = 1;
            else editingBound = 0;
        }
        GUI.color = _originalColor;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        leftBound.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("Left Bound", "Left bound position"), leftBound.vector3Value);

        GUI.color = editingBound == 2 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(50)))
        {
            if (editingBound != 2) editingBound = 2;
            else editingBound = 0;
        }
        GUI.color = _originalColor;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        rightBound.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("Right Bound", "Right bound rightBound"), rightBound.vector3Value);

        GUI.color = editingBound == 3 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(50)))
        {
            if (editingBound != 3) editingBound = 3;
            else editingBound = 0;
        }
        GUI.color = _originalColor;

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        bottomBound.vector3Value = EditorGUILayout.Vector3Field(new GUIContent("Bottom Bound", "Bottom bound position"), bottomBound.vector3Value);

        GUI.color = editingBound == 4 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(50)))
        {
            if (editingBound != 4) editingBound = 4;
            else editingBound = 0;
        }
        GUI.color = _originalColor;

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        MultiTagsUtility.GUILayoutTagsField(detectedTags);

        serializedObject.ApplyModifiedProperties();
    }

    // Enables the Editor to handle an event in the Scene view
    private void OnSceneGUI()
    {
        serializedObject.Update();

        // Draw a position handle for the editing bound
        switch (editingBound)
        {
            case 1:
                topBound.vector3Value = Handles.PositionHandle(topBound.vector3Value, Quaternion.identity);
                break;

            case 2:
                leftBound.vector3Value = Handles.PositionHandle(leftBound.vector3Value, Quaternion.identity);
                break;

            case 3:
                rightBound.vector3Value = Handles.PositionHandle(rightBound.vector3Value, Quaternion.identity);
                break;

            case 4:
                bottomBound.vector3Value = Handles.PositionHandle(bottomBound.vector3Value, Quaternion.identity);
                break;

            default:
                break;
        }

        // Draw a little circle at the location of each bound if not null
        if ((topBound.vector3Value != Vector3.zero) || (editingBound == 1))
        {
            Handles.color = Color.red;
            Handles.DrawSolidDisc(topBound.vector3Value, Vector3.up, .5f);
            Handles.Label(topBound.vector3Value + (Vector3.up * 3), new GUIContent("TOP"), EditorStyles.boldLabel);
        }
        if ((leftBound.vector3Value != Vector3.zero) || (editingBound == 2))
        {
            Handles.color = Color.red;
            Handles.DrawSolidDisc(leftBound.vector3Value, Vector3.up, .5f);
            Handles.Label(leftBound.vector3Value + (Vector3.up * 3), new GUIContent("LEFT"), EditorStyles.boldLabel);
        }
        if ((rightBound.vector3Value != Vector3.zero) || (editingBound == 3))
        {
            Handles.color = Color.red;
            Handles.DrawSolidDisc(rightBound.vector3Value, Vector3.up, .5f);
            Handles.Label(rightBound.vector3Value + (Vector3.up * 3), new GUIContent("RIGHT"), EditorStyles.boldLabel);
        }
        if ((bottomBound.vector3Value != Vector3.zero) || (editingBound == 4))
        {
            Handles.color = Color.red;
            Handles.DrawSolidDisc(bottomBound.vector3Value, Vector3.up, .5f);
            Handles.Label(bottomBound.vector3Value + (Vector3.up * 3), new GUIContent("BOTTOM"), EditorStyles.boldLabel);
        }

        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #endregion
}
