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
    }

    protected override void DrawSettings()
    {
        base.DrawSettings();

        TDS_EditorUtility.PropertyField("Area activated by Mr Loyal", "Area used in the top phase of Mr Loyal", linkedAreas);
        TDS_EditorUtility.PropertyField("Cats", "Mr Loyal's cats", cats);
        TDS_EditorUtility.FloatSlider("Cats' charge rate", "Waiting seconds before the cats will attack", chargeCatsRate, .1f, 10f);
        TDS_EditorUtility.Vector3Field("Teleportation Position", "Position where Mr Loyal's will teleport himself", teleportationPosition);
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
