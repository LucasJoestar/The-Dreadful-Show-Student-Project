using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Camera)), CanEditMultipleObjects]
public class TDS_CameraEditor : Editor 
{
    /* TDS_CameraEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Custom editor for the TDS_Camera class.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[25 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_CameraEditor class.
     *	
     *	    - Added the isMoving, bounds, camera, rotation, speedAccelerationTime, speedCoef, speedCurrent, speedInitial, speedMax, cTarget, offset & cameras fields.
     *	    - Added the DrawEditor method.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region SerializedProperties
    /// <summary>SerializedProperties for <see cref="TDS_Camera.IsMoving"/> of type <see cref="bool"/>.</summary>
    private SerializedProperty isMoving = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.currentBounds"/> of type <see cref="TDS_Bounds"/>.</summary>
    private SerializedProperty currentBounds = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.LevelBounds"/> of type <see cref="TDS_Bounds"/>.</summary>
    private SerializedProperty levelBounds = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.Camera"/> of type <see cref="Camera"/>.</summary>
    private SerializedProperty camera = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.Rotation"/> of type <see cref="float"/>.</summary>
    private SerializedProperty rotation = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.SpeedAccelerationTime"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedAccelerationTime = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.SpeedCoef"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedCoef = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.SpeedCurrent"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedCurrent = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.SpeedInitial"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedInitial = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.SpeedMax"/> of type <see cref="float"/>.</summary>
    private SerializedProperty speedMax = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.Target"/> of type <see cref="Transform"/>.</summary>
    private SerializedProperty cTarget = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.topBound"/> of type <see cref="BoxCollider"/>.</summary>
    private SerializedProperty topBound = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.leftBound"/> of type <see cref="BoxCollider"/>.</summary>
    private SerializedProperty leftBound = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.rightBound"/> of type <see cref="BoxCollider"/>.</summary>
    private SerializedProperty rightBound = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.bottomBound"/> of type <see cref="BoxCollider"/>.</summary>
    private SerializedProperty bottomBound = null;

    /// <summary>SerializedProperties for <see cref="TDS_Camera.Offset"/> of type <see cref="Vector3"/>.</summary>
    private SerializedProperty offset = null;
    #endregion

    #region Target Scripts Infos
    /// <summary>
    /// All editing TDS_Camera classes.
    /// </summary>
    private List<TDS_Camera> cameras = new List<TDS_Camera>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Draws the custom editor of the editing TDS_Camera objects.
    /// </summary>
    public void DrawEditor()
    {
        // Records any changements on the editing objects to allow undo
        Undo.RecordObjects(targets, "Camera script(s) settings");

        // Updates the SerializedProperties to get the latest values
        serializedObject.Update();

        TDS_EditorUtility.ObjectField("Camera", "Camera component attached to this script", camera, typeof(Camera));

        GUILayout.Space(1);

        if (TDS_EditorUtility.ObjectField("Target", "Target this camera has to follow", cTarget, typeof(Transform)))
        {
            cameras.ForEach(c => c.Target = (Transform)cTarget.objectReferenceValue);
        }

        GUILayout.Space(3);

        TDS_EditorUtility.ObjectField("Top Bound", "Top bound collider of this level", topBound, typeof(BoxCollider));
        TDS_EditorUtility.ObjectField("Left Bound", "Left bound collider of this level", leftBound, typeof(BoxCollider));
        TDS_EditorUtility.ObjectField("Right Bound", "Right bound collider of this level", rightBound, typeof(BoxCollider));
        TDS_EditorUtility.ObjectField("Bottom Bound", "Bottom bound collider of this level", bottomBound, typeof(BoxCollider));

        GUILayout.Space(3);

        TDS_EditorUtility.PropertyField("Current Bounds", "Current bounds of the camera", currentBounds);

        GUILayout.Space(3);

        TDS_EditorUtility.PropertyField("Level Bounds", "Global bounds of the camera in the Level", levelBounds);

        TDS_EditorUtility.Vector3Field("Offset", "Offset of the camera from its target", offset);

        GUILayout.Space(1);

        if (TDS_EditorUtility.FloatSlider("Rotation", "Rotation in X axis of the camera", rotation, 0, 90))
        {
            cameras.ForEach(c => c.Rotation = rotation.floatValue);
        }

        GUILayout.Space(2);

        if (EditorApplication.isPlaying && !serializedObject.isEditingMultipleObjects)
        {
            TDS_EditorUtility.ProgressBar(20, speedCurrent.floatValue / speedMax.floatValue, "Speed");
        }

        if (TDS_EditorUtility.FloatSlider("Initial Speed", "Initial speed of the camera when starting moving", speedInitial, 0, speedMax.floatValue))
        {
            cameras.ForEach(c => c.SpeedInitial = speedInitial.floatValue);
        }
        if (TDS_EditorUtility.FloatField("Max Speed", "Maximum value of this camera speed", speedMax))
        {
            cameras.ForEach(c => c.SpeedMax = speedMax.floatValue);
        }
        if (TDS_EditorUtility.FloatField("Speed Coef", "Coefficient used to multiply the speed of the camera", speedCoef))
        {
            cameras.ForEach(c => c.SpeedCoef = speedCoef.floatValue);
        }
        if (TDS_EditorUtility.FloatField("Speed Accl. Time", "Time the speed of the camera take from its initial value to reach its maximum value", speedAccelerationTime))
        {
            cameras.ForEach(c => c.SpeedAccelerationTime = speedAccelerationTime.floatValue);
        }

        GUILayout.Space(5);

        TDS_EditorUtility.RadioToggle("Moving", "Is the camera currently moving", isMoving);

        // Apply modifications
        serializedObject.ApplyModifiedProperties();
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Get editing scripts
        targets.ToList().ForEach(t => cameras.Add((TDS_Camera)t));

        // Find serialized properties
        isMoving = serializedObject.FindProperty("isMoving");
        currentBounds = serializedObject.FindProperty("currentBounds");
        levelBounds = serializedObject.FindProperty("levelBounds");
        camera = serializedObject.FindProperty("camera");
        rotation = serializedObject.FindProperty("rotation");
        speedAccelerationTime = serializedObject.FindProperty("speedAccelerationTime");
        speedCoef = serializedObject.FindProperty("speedCoef");
        speedCurrent = serializedObject.FindProperty("speedCurrent");
        speedInitial = serializedObject.FindProperty("speedInitial");
        speedMax = serializedObject.FindProperty("speedMax");
        cTarget = serializedObject.FindProperty("target");
        topBound = serializedObject.FindProperty("topBound");
        leftBound = serializedObject.FindProperty("leftBound");
        rightBound = serializedObject.FindProperty("rightBound");
        bottomBound = serializedObject.FindProperty("bottomBound");
        offset = serializedObject.FindProperty("Offset");
    }

    // Implement this function to make a custom inspector
    public override void OnInspectorGUI()
    {
        DrawEditor();
    }
    #endregion

    #endregion
}
