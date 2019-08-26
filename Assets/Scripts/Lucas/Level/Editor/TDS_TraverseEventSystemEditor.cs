using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_TraverseEventSystem))]
public class TDS_TraverseEventSystemEditor : TDS_EventSystemEditor
{
    /* TDS_EventSystemEditor :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
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
     *	Date :			
     *	Author :		
     *
     *	Changes :
     *
     *	-----------------------------------
    */

    #region Fields / Properties
    /// <summary>SerializedProperty for <see cref="TDS_TraverseEventSystem.exitEvents"/> of type <see cref="TDS_Event"/>[].</summary>
    protected SerializedProperty exitEvents = null;

    /// <summary>
    /// Folout booleans for exit events array.
    /// </summary>
    [SerializeField] protected bool[] exitFoldouts = new bool[] { };
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor of the editing scripts.
    /// </summary>
    public override void DrawEditor()
    {
        // Make a space at the beginning of the editor
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;
        Color _originalGUIColor = GUI.color;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        // Records any changements on the editing objects to allow undo
        Undo.RecordObjects(targets, "Event System settings");

        // Updates the SerializedProperties to get the latest values
        serializedObject.Update();
        GUILayout.Space(3);

        TDS_EditorUtility.RadioToggle("Activated", "Is this event system activated or not", isActivated);

        if (isActivated.boolValue)
        {
            GUILayout.Space(3);

            EditorGUILayout.LabelField(new GUIContent("Current Event", "Current event processing"), new GUIContent(currentEvent.FindPropertyRelative("Name").stringValue));
        }

        GUILayout.Space(5);

        TDS_EditorUtility.Toggle("Des. Object when Finished", "Should this object be deactivated when the event system gets finished", doDesObjectOnFinish);

        GUILayout.Space(3);
        GUI.backgroundColor = _originalColor;

        TDS_EditorUtility.PropertyField("Detected Tags", "Tags detected to trigger this event", detectedTags);

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        GUILayout.Space(5);

        // Draws enter events
        EditorGUILayout.LabelField("Enter Events");
        DrawEvents(events, ref foldouts);

        GUILayout.Space(5);

        // Draws exit events
        EditorGUILayout.LabelField("Exit Events");
        DrawEvents(exitEvents, ref exitFoldouts);

        // Applies all modified properties on the SerializedObjects
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
        GUI.color = _originalGUIColor;
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected override void OnEnable()
    {
        base.OnEnable();

        exitEvents = serializedObject.FindProperty("exitEvents");
        exitFoldouts = new bool[exitEvents.arraySize];
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawEditor();
    }
    #endregion

    #endregion
}
