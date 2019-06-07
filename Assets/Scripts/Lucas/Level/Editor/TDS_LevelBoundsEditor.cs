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
    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.doDestroyOnActivate"/> field of type <see cref="bool"/>.</summary>
    [SerializeField] private SerializedProperty doDestroyOnActivate = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.doDisableOnActivate"/> field of type <see cref="bool"/>.</summary>
    [SerializeField] private SerializedProperty doDisableOnActivate = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.topBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty topBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.leftBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty leftBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.rightBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty rightBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.bottomBound"/> field of type <see cref="Vector3"/>.</summary>
    [SerializeField] private SerializedProperty bottomBound = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.detectedTags"/> field of type <see cref="Tags"/>.</summary>
    [SerializeField] private SerializedProperty detectedTags = null;

    /// <summary>SerializedProperty for the <see cref="TDS_LevelBounds.activationMode"/> field of type <see cref="TriggerActivationMode"/>.</summary>
    [SerializeField] private SerializedProperty activationMode = null;

    /// <summary>
    /// Int indicating the currently editing bound.
    /// </summary>
    private int editingBound = 0;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor of the editing scripts.
    /// </summary>
    public void DrawEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;
        Color _originalGUIColor = GUI.color;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");
        GUILayout.Space(3);

        serializedObject.Update();

        TDS_EditorUtility.Toggle("Destroy on Activate", "Should this object be destroyed when activated", doDestroyOnActivate);

        if (!doDestroyOnActivate.boolValue)
        {
            TDS_EditorUtility.Toggle("Disable on Activate", "Should this object be disable when activated", doDisableOnActivate);
        }

        GUILayout.Space(2);
        TDS_EditorUtility.PropertyField("Activation mode", "Activation mode used to trigger these bounds", activationMode);

        if (activationMode.enumValueIndex < 2)
        {
            GUILayout.Space(3);
            GUI.backgroundColor = _originalColor;

            TDS_EditorUtility.PropertyField("Detected Tags", "Tags detected to trigger this event", detectedTags);

            GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        }

        GUILayout.Space(5);
        GUI.backgroundColor = _originalColor;
        EditorGUILayout.BeginHorizontal();

        TDS_EditorUtility.Vector3Field("Top Bound", "Top bound position", topBound);

        GUI.color = editingBound == 1 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(40)))
        {
            if (editingBound != 1) editingBound = 1;
            else editingBound = 0;
        }
        if (bottomBound.vector3Value != Vector3.zero)
        {
            GUI.color = new Color(.9f, .7f, .3f);

            if (GUILayout.Button(new GUIContent("BOTT. SET", "Set the position of the top bound to match the camera size with the bottom bound"), GUILayout.Width(75)))
            {
                Undo.RecordObjects(serializedObject.targetObjects, "set bottom bound position");

                topBound.vector3Value = new Vector3(topBound.vector3Value.x, topBound.vector3Value.y, bottomBound.vector3Value.z + 16.5f);
            }
        }

        GUI.color = _originalGUIColor;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        TDS_EditorUtility.Vector3Field("Left Bound", "Left bound position", leftBound);

        GUI.color = editingBound == 2 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(40)))
        {
            if (editingBound != 2) editingBound = 2;
            else editingBound = 0;
        }
        if (rightBound.vector3Value != Vector3.zero)
        {
            GUI.color = new Color(.9f, .7f, .3f);

            if (GUILayout.Button(new GUIContent("RIGH. SET", "Set the position of the left bound to match the camera size with the right bound"), GUILayout.Width(75)))
            {
                Undo.RecordObjects(serializedObject.targetObjects, "set bottom bound position");

                leftBound.vector3Value = new Vector3(rightBound.vector3Value.x - (Camera.main.orthographicSize * 2 * ((float)Screen.width / Screen.height)), leftBound.vector3Value.y, leftBound.vector3Value.z);
            }
        }

        GUI.color = _originalGUIColor;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        TDS_EditorUtility.Vector3Field("Right Bound", "Right bound rightBound", rightBound);

        GUI.color = editingBound == 3 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(40)))
        {
            if (editingBound != 3) editingBound = 3;
            else editingBound = 0;
        }
        if (leftBound.vector3Value != Vector3.zero)
        {
            GUI.color = new Color(.9f, .7f, .3f);

            if (GUILayout.Button(new GUIContent("LEFT SET", "Set the position of the right bound to match the camera size with the left bound"), GUILayout.Width(75)))
            {
                Undo.RecordObjects(serializedObject.targetObjects, "set bottom bound position");

                rightBound.vector3Value = new Vector3(leftBound.vector3Value.x + (Camera.main.orthographicSize * 2 * ((float)Screen.currentResolution.width / Screen.currentResolution.height)), rightBound.vector3Value.y, rightBound.vector3Value.z);
            }
        }

        GUI.color = _originalGUIColor;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();

        TDS_EditorUtility.Vector3Field("Bottom Bound", "Bottom bound position", bottomBound);

        GUI.color = editingBound == 4 ? Color.grey : Color.cyan;
        if (GUILayout.Button("EDIT", GUILayout.Width(40)))
        {
            if (editingBound != 4) editingBound = 4;
            else editingBound = 0;
        }
        if (topBound.vector3Value != Vector3.zero)
        {
            GUI.color = new Color(.9f, .7f, .3f);

            if (GUILayout.Button(new GUIContent("TOP SET", "Set the position of the bottom bound to match the camera size with the top bound"), GUILayout.Width(75)))
            {
                Undo.RecordObjects(serializedObject.targetObjects, "set bottom bound position");

                bottomBound.vector3Value = new Vector3(bottomBound.vector3Value.x, bottomBound.vector3Value.y, topBound.vector3Value.z - 16.5f);
            }
        }

        GUI.color = _originalGUIColor;
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(5);

        EditorGUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Get serialized properties
        doDestroyOnActivate = serializedObject.FindProperty("doDestroyOnActivate");
        doDisableOnActivate = serializedObject.FindProperty("doDisableOnActivate");
        topBound = serializedObject.FindProperty("topBound");
        leftBound = serializedObject.FindProperty("leftBound");
        rightBound = serializedObject.FindProperty("rightBound");
        bottomBound = serializedObject.FindProperty("bottomBound");
        detectedTags = serializedObject.FindProperty("detectedTags");
        activationMode = serializedObject.FindProperty("activationMode");
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawEditor();
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
