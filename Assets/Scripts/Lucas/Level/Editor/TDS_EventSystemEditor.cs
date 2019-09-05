using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TDS_EventsSystem))]
public class TDS_EventSystemEditor : Editor 
{
    /* TDS_EventSystemEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor ofr the TDS_EventSystem class, to have something prettier and easier to use.
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
	 *	Date :			[02 / 05 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TDS_EventSystemEditor class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.activationMode"/> of type <see cref="TriggerActivationMode"/>.</summary>
    private SerializedProperty activationMode = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.doDesTriggerOnActiv"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty doDesTriggerOnActiv = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.doDesObjectOnFinish"/> of type <see cref="bool"/>.</summary>
    protected SerializedProperty doDesObjectOnFinish = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.doLoop"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty doLoop = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.isActivated"/> of type <see cref="bool"/>.</summary>
    protected SerializedProperty isActivated = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.isLocal"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isLocal = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.currentEvent"/> of type <see cref="TDS_Event"/>.</summary>
    protected SerializedProperty currentEvent = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.events"/> of type <see cref="TDS_Event"/>[].</summary>
    protected SerializedProperty events = null;

    /// <summary>SerializedProperty for <see cref="TDS_EventsSystem.detectedTags"/> of type <see cref="Tags"/>.</summary>
    protected SerializedProperty detectedTags = null;

    /// <summary>
    /// Folout booleans for events array.
    /// </summary>
    [SerializeField] protected bool[] foldouts = new bool[] { };
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the editor of the editing scripts.
    /// </summary>
    public virtual void DrawEditor()
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

        TDS_EditorUtility.Toggle("Local", "Is this event system local-based or online ?", isLocal);
        GUILayout.Space(2);

        TDS_EditorUtility.Toggle("Des. Collider on Activation", "Should this object collider be desactivated when starting events", doDesTriggerOnActiv);
        TDS_EditorUtility.Toggle("Looping", "Should this event system loop when reaching the end or not", doLoop);
        TDS_EditorUtility.Toggle("Des. Object when Finished", "Should this object be deactivated when the event system gets finished", doDesObjectOnFinish);

        GUILayout.Space(2);

        TDS_EditorUtility.PropertyField("Activation mode", "Activation mode used to trigger these events", activationMode);

        if (activationMode.enumValueIndex < 3)
        {
            GUILayout.Space(3);
            GUI.backgroundColor = _originalColor;

            TDS_EditorUtility.PropertyField("Detected Tags", "Tags detected to trigger this event", detectedTags);

            GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        }

        GUILayout.Space(5);

        // Draws events
        DrawEvents(events, ref foldouts);

        // Applies all modified properties on the SerializedObjects
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
        GUI.color = _originalGUIColor;
    }

    /// <summary>
    /// Draws the editor for the events.
    /// </summary>
    public void DrawEvents(SerializedProperty _events, ref bool[] _foldouts)
    {
        // Button to add a new event
        GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
        GUI.color = Color.green;

        if (TDS_EditorUtility.Button("+", "Add a new event", EditorStyles.miniButton))
        {
            _events.InsertArrayElementAtIndex(0);
            _events.GetArrayElementAtIndex(0).FindPropertyRelative("Name").stringValue = "New Event";

            bool[] _newFoldouts = new bool[_foldouts.Length + 1];
            Array.Copy(_foldouts, 0, _newFoldouts, 1, _foldouts.Length);
            _foldouts = _newFoldouts;
        }

        GUI.color = Color.white;
        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;

        for (int _i = 0; _i < _events.arraySize; _i++)
        {
            GUILayout.Space(5);

            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("Box");

            SerializedProperty _event = _events.GetArrayElementAtIndex(_i);
            SerializedProperty _eventName = _event.FindPropertyRelative("Name");

            EditorGUILayout.BeginHorizontal();

            // Button to show or not this event
            if (TDS_EditorUtility.Button(_eventName.stringValue, "Wrap / unwrap this event", TDS_EditorUtility.HeaderStyle)) _foldouts[_i] = !_foldouts[_i];

            GUILayout.FlexibleSpace();

            // BUttons to change the event position in the list
            if ((_i > 0) && TDS_EditorUtility.Button("▲", "Move this element up", EditorStyles.miniButton))
            {
                _events.MoveArrayElement(_i, _i - 1);
            }
            if ((_i < _events.arraySize - 1) && TDS_EditorUtility.Button("▼", "Move this element down", EditorStyles.miniButton))
            {
                _events.MoveArrayElement(_i, _i + 1);
            }

            // Button to delete this event
            GUI.color = Color.red;
            if (TDS_EditorUtility.Button("X", "Delete this event", EditorStyles.miniButton))
            {
                _events.DeleteArrayElementAtIndex(_i);

                bool[] _newFoldouts = new bool[_foldouts.Length - 1];
                Array.Copy(_foldouts, 0, _newFoldouts, 0, _i);
                Array.Copy(_foldouts, _i + 1, _newFoldouts, _i, _foldouts.Length - (_i + 1));
                _foldouts = _newFoldouts;
                break;
            }

            GUI.color = Color.white;
            EditorGUILayout.EndHorizontal();

            // If unfolded, draws this event
            if (_foldouts[_i])
            {
                SerializedProperty _eventType = _event.FindPropertyRelative("eventType");
                SerializedProperty _doRequireType = _event.FindPropertyRelative("doRequireSpecificPlayerType");

                TDS_EditorUtility.TextField("Name", "Name of this event", _eventName);

                GUILayout.Space(3);

                TDS_EditorUtility.PropertyField("Event Type", "Type of this event", _eventType);
                CustomEventType _eventTypeValue = (CustomEventType)_eventType.enumValueIndex;
                if (_eventType.enumValueIndex > 7) _eventTypeValue += 13;

                TDS_EditorUtility.FloatField("Delay", "Delay before starting this event", _event.FindPropertyRelative("delay"));

                GUILayout.Space(3);

                TDS_EditorUtility.Toggle("Require specific Player type", "Should this event require a specific player type to be triggered", _doRequireType);
                if (_doRequireType.boolValue)
                {
                    TDS_EditorUtility.PropertyField("Required type of Player", "Required type of player to trigger this event", _event.FindPropertyRelative("playerType"));
                }

                GUILayout.Space(5);

                switch (_eventTypeValue)
                {
                    case CustomEventType.CameraMovement:
                        TDS_EditorUtility.PropertyField("Target", "Target to make the camera look ", _event.FindPropertyRelative("eventTransform"));

                        TDS_EditorUtility.FloatField("Duration", "Time to look at the target", _event.FindPropertyRelative("cameraWaitTime"));

                        TDS_EditorUtility.FloatField("Speed Coef", "Coefficient applied to the speed of the camera.", _event.FindPropertyRelative("eventFloat"));
                        break;

                    case CustomEventType.DesactiveInfoBox:
                        break;

                    case CustomEventType.DisplayInfoBox:
                        TDS_EditorUtility.TextField("Text ID", "ID of the text to use for the Info Box", _event.FindPropertyRelative("eventString"));
                        break;

                    case CustomEventType.Instantiate:
                        TDS_EditorUtility.PropertyField("Prefab", "Prefab to instantiate", _event.FindPropertyRelative("prefab"));

                        TDS_EditorUtility.PropertyField("Instantiation transform reference", "Transform to use as reference for position & rotation for the transform of the instantiated object", _event.FindPropertyRelative("eventTransform"));
                        break;

                    case CustomEventType.InstantiatePhoton:
                        TDS_EditorUtility.PropertyField("Prefab", "Prefab to instantiate", _event.FindPropertyRelative("prefab"));

                        TDS_EditorUtility.PropertyField("Instantiation transform reference", "Transform to use as reference for position & rotation for the transform of the instantiated object", _event.FindPropertyRelative("eventTransform"));
                        break;

                    case CustomEventType.MovePlayerAroundPoint:
                        TDS_EditorUtility.PropertyField("Position", "Where to move te player around", _event.FindPropertyRelative("eventTransform"));
                        break;

                    case CustomEventType.Narrator:
                        TDS_EditorUtility.PropertyField("Quote", "Narrator quote to play", _event.FindPropertyRelative("quote"));

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        Color _originalColor = GUI.color;
                        GUI.color = new Color(.7f, .35f, .75f);
                        if (GUILayout.Button(new GUIContent("Load Quote", "Loads the quote with the ID entered as Text ID"), GUILayout.Width(150)))
                        {
                            FieldInfo _fieldInfo = typeof(TDS_Event).GetField("quote", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            if (_fieldInfo != null)
                            {
                                TDS_NarratorQuote _quote = ((TDS_NarratorManager)Resources.Load(TDS_NarratorManager.FILE_PATH)).Quotes.FirstOrDefault(q => q.Name == _event.FindPropertyRelative("quote").FindPropertyRelative("Name").stringValue);

                                if (_quote != null)
                                {
                                    TDS_Event[] _objectEvents = (TDS_Event[])typeof(TDS_EventsSystem).GetField(_events.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(target);

                                    typeof(TDS_Event).GetField("quote", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(_objectEvents[_i], _quote);
                                }
                            }
                        }

                        GUI.color = _originalColor;
                        GUILayout.EndHorizontal();
                        break;

                    case CustomEventType.PlayMusic:
                        _event.FindPropertyRelative("eventInt").intValue = EditorGUILayout.IntPopup("Music", _event.FindPropertyRelative("eventInt").intValue, Enum.GetNames(typeof(Music)), (int[])Enum.GetValues(typeof(Music)));

                        TDS_EditorUtility.FloatField("Fade Duration", "Time during which the previous music will fade out before the new one starts.", _event.FindPropertyRelative("eventFloat"));
                        break;

                    case CustomEventType.WaitForObjectDeath:
                        TDS_EditorUtility.PropertyField("Object Tag", "Tag waiting for an object with dies", _event.FindPropertyRelative("eventString"));
                        TDS_EditorUtility.PropertyField("Amount", "Amount of object with this tag to wait for death", _event.FindPropertyRelative("eventInt"));
                        break;

                    case CustomEventType.WaitForSpawnAreaDesactivation:
                        TDS_EditorUtility.PropertyField("Amount", "Amount of spawn area to wait for desactivation", _event.FindPropertyRelative("eventInt"));
                        break;

                    case CustomEventType.UnityEventLocal:
                        TDS_EditorUtility.PropertyField("Unity Event", "Associated Unity Event to this event", _event.FindPropertyRelative("unityEvent"));
                        break;

                    case CustomEventType.UnityEventOnline:
                        TDS_EditorUtility.PropertyField("Unity Event", "Associated Unity Event to this event", _event.FindPropertyRelative("unityEvent"));
                        break;

                    case CustomEventType.WaitForAction:
                        TDS_EditorUtility.PropertyField("Action to wait", "Action to wait the player to perform", _event.FindPropertyRelative("actionType"));
                        break;

                    case CustomEventType.WaitForEveryone:
                        TDS_EditorUtility.PropertyField("Bound min X", "Transform to use for minimum bound X value to wait", _event.FindPropertyRelative("eventTransform"));
                        break;

                    default:
                        // Mhmm...
                        break;
                }

                // Button to add a new event
                GUI.color = Color.green;

                if (TDS_EditorUtility.Button("+", "Add a new event", EditorStyles.miniButton))
                {
                    _events.InsertArrayElementAtIndex(_i);

                    bool[] _newFoldouts = new bool[_foldouts.Length + 1];
                    Array.Copy(_foldouts, 0, _newFoldouts, 0, _i + 1);
                    Array.Copy(_foldouts, _i + 1, _newFoldouts, _i + 2, _foldouts.Length - (_i + 1));
                    _foldouts = _newFoldouts;
                }

                GUI.color = Color.white;
            }

            EditorGUILayout.EndVertical();
        }
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    protected virtual void OnEnable()
    {
        activationMode = serializedObject.FindProperty("activationMode");
        doDesTriggerOnActiv = serializedObject.FindProperty("doDesTriggerOnActiv");
        doDesObjectOnFinish = serializedObject.FindProperty("doDesObjectOnFinish");
        doLoop = serializedObject.FindProperty("doLoop");
        isActivated = serializedObject.FindProperty("isActivated");
        isLocal = serializedObject.FindProperty("isLocal");
        currentEvent = serializedObject.FindProperty("currentEvent");
        events = serializedObject.FindProperty("events");
        detectedTags = serializedObject.FindProperty("detectedTags");

        foldouts = new bool[events.arraySize];
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawEditor();
    }
    #endregion

    #endregion
}
