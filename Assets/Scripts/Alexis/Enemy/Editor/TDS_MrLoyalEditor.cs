using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_MrLoyal))]
public class TDS_MrLoyalEditor : TDS_BossEditor 
{
    /* TDS_MrLoyalEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[PURPOSE]
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	[TO DO]
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[DATE]
	 *	Author :		[NAME]
	 *
	 *	Changes :
	 *
	 *	[CHANGES]
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties
    private SerializedProperty linkedAreas = null;
    private SerializedProperty cats = null;
    private SerializedProperty chargeCatsRate = null;
    private SerializedProperty teleportationPosition = null;
    private SerializedProperty audioSourceTaunt = null;
    private SerializedProperty  fakirAudioClip = null;
    private SerializedProperty  mimeAudioClip = null;
    private SerializedProperty  acrobatAudioClip = null;
    private SerializedProperty  mightyManAudioClip = null;
    private SerializedProperty  catAudioClip = null;
    private SerializedProperty tauntAudioClips = null; 

    private SerializedProperty tauntRateMin = null;
    private SerializedProperty tauntRateMax = null; 
    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        linkedAreas = serializedObject.FindProperty("linkedAreas");
        cats = serializedObject.FindProperty("cats");
        chargeCatsRate = serializedObject.FindProperty("chargeCatsRate");
        teleportationPosition = serializedObject.FindProperty("teleportationPosition");
        fakirAudioClip = serializedObject.FindProperty("fakirAudioClip");
        mimeAudioClip = serializedObject.FindProperty("mimeAudioClip");
        acrobatAudioClip = serializedObject.FindProperty("acrobatAudioClip");
        mightyManAudioClip = serializedObject.FindProperty("mightyManAudioClip");
        catAudioClip = serializedObject.FindProperty("catAudioClip");
        tauntRateMin = serializedObject.FindProperty("tauntRateMin");
        tauntRateMax = serializedObject.FindProperty("tauntRateMax");
        tauntAudioClips = serializedObject.FindProperty("tauntAudioClips"); 
    }

    protected override void DrawSettings()
    {
        base.DrawSettings();

        TDS_EditorUtility.PropertyField("Area activated by Mr Loyal", "Area used in the top phase of Mr Loyal", linkedAreas);
        TDS_EditorUtility.PropertyField("Cats", "Mr Loyal's cats", cats);
        TDS_EditorUtility.FloatSlider("Cats' charge rate", "Waiting seconds before the cats will attack", chargeCatsRate, 5, 25);
        TDS_EditorUtility.Vector3Field("Teleportation Position", "Position where Mr Loyal's will teleport himself", teleportationPosition);

        EditorGUILayout.LabelField("SOUNDS", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.PropertyField("Callout Fakir Sound", "", fakirAudioClip);
        TDS_EditorUtility.PropertyField("Callout Mime Sound", "", mimeAudioClip);
        TDS_EditorUtility.PropertyField("Callout Acrobat Sound", "", acrobatAudioClip);
        TDS_EditorUtility.PropertyField("Callout MightyMan Sound", "", mightyManAudioClip);
        TDS_EditorUtility.PropertyField("Callout Cat Sound", "", catAudioClip);
        TDS_EditorUtility.PropertyField("Taunt audio clips", "", tauntAudioClips); 

        TDS_EditorUtility.FloatSlider("Main Taunt rate", "", tauntRateMin, 3, tauntRateMax.floatValue);
        TDS_EditorUtility.FloatSlider("Max Taunt rate", "", tauntRateMax, tauntRateMin.floatValue, 25);

        serializedObject.ApplyModifiedProperties(); 
    }

    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();
        teleportationPosition.vector3Value = Handles.PositionHandle(teleportationPosition.vector3Value, Quaternion.identity);
        Handles.Label(teleportationPosition.vector3Value, "Teleportation Position"); 
        serializedObject.ApplyModifiedProperties(); 
    }
    #endregion

    #endregion
}
