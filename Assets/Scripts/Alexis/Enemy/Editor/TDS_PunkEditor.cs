using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TDS_Punk))]
public class TDS_PunkEditor : TDS_EnemyEditor 
{
    /* TDS_PunkEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Editor class of the TDS_Punk
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[13/02/2019]
	 *	Author :		[Thiebaut Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the editor class]
     *	    - Create a serialized field attacks for the enemyAttacks of the punk
     *	    - Create bool to display or not the attacks 
	 *
	 *	-----------------------------------
	*/

    #region Fields and properties

    #region SerializedProperties
    /// <summary>SerializedProperty for <see cref="TDS_Punk.attacks"/> of type <see cref="TDS_EnemyAttack[]"/>.</summary>
    private SerializedProperty attacks = null;
    #endregion

    #region Fold Out
    /// <summary>Backing field for <see cref="IsPunkUnfolded"/></summary>
    private bool isPunkUnfolded = false;

    /// <summary>
    /// Indicates if the editor for the Punk class is unfolded or not.
    /// </summary>
    public bool IsPunkUnfolded
    {
        get { return isPunkUnfolded; }
        set
        {
            isPunkUnfolded = value;

            // Saves this value
            EditorPrefs.SetBool("isPunkUnfolded", value);
        }
    }
    #endregion

    #region Target Scripts Info
    /// <summary>
    /// Is the user currently editing multiple instances of this class ?
    /// </summary>
    private bool isPunkMultiEditing = false;

    /// <summary>
    /// All editing instances of the Enemy class.
    /// </summary>
    private List<TDS_Punk> punks = new List<TDS_Punk>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    private void DrawPunkSettings()
    {
        GUILayout.Space(10);
        Color _originalColor = GUI.backgroundColor;

        GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        EditorGUILayout.BeginVertical("HelpBox");

        if (TDS_EditorUtility.Button("Punk", "Wrap / unwrap Punk class settings", TDS_EditorUtility.HeaderStyle)) IsPunkUnfolded = !isPunkUnfolded;
        if (isPunkUnfolded)
        {
            GUI.backgroundColor = TDS_EditorUtility.BoxLightColor;
            EditorGUILayout.BeginVertical("HelpBox");

            //Draw a header for the enemy evolution settings
            EditorGUILayout.LabelField("Attack Settings", TDS_EditorUtility.HeaderStyle);
            TDS_EditorUtility.PropertyField("Punk Attacks", "", attacks);

            EditorGUILayout.EndVertical();

        }

        serializedObject.ApplyModifiedProperties(); 
        EditorGUILayout.EndVertical();
        GUI.backgroundColor = _originalColor;
    }
    #endregion

    #region Unity Methods
    public override void OnInspectorGUI()
    {
        DrawPunkSettings(); 
        base.OnInspectorGUI();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //Get the target editiong scripts
        targets.ToList().ForEach(t => punks.Add((TDS_Punk)t));
        if (targets.Length == 1) isPunkMultiEditing = false;
        else isPunkMultiEditing = true; 

       //Get the property
       attacks = serializedObject.FindProperty("attacks");

        //Load the editor folded and unfolded values of this class
        isPunkUnfolded = EditorPrefs.GetBool("isPunkUnfolded", isPunkUnfolded);
    }
    #endregion

    #endregion
}
