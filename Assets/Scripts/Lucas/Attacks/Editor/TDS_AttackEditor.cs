using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TDS_Attack))]
public class TDS_AttackEditor : PropertyDrawer 
{
    /* TDS_AttackEditor :
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

    #endregion

    #region Methods

    #region Original Methods

    #endregion

    #region Unity Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect _rect = new Rect(position.position.x, position.position.y, position.width, 20);
        property.FindPropertyRelative("isAttackFoldOut").boolValue = EditorGUI.Foldout(_rect, property.FindPropertyRelative("isAttackFoldOut").boolValue, label, true, TDS_EditorUtility.HeaderStyle);

        if(property.FindPropertyRelative("isAttackFoldOut").boolValue)
        {
            _rect = new Rect(position.position.x, position.position.y + 20, position.width, 20);
            property.FindPropertyRelative("name").stringValue = EditorGUI.TextField(_rect, "Name: ", property.FindPropertyRelative("name").stringValue);

            _rect = new Rect(position.position.x, position.position.y + 40, position.width, 40);
            property.FindPropertyRelative("description").stringValue = EditorGUI.TextArea(_rect, property.FindPropertyRelative("description").stringValue);

            _rect = new Rect(position.position.x, position.position.y + 100, position.width, 20);
            EditorGUI.IntSlider(_rect,property.FindPropertyRelative("damagesMin"), 1, property.FindPropertyRelative("damagesMax").intValue);
            _rect = new Rect(position.position.x, position.position.y + 120, position.width, 20);
            EditorGUI.IntSlider(_rect, property.FindPropertyRelative("damagesMax"), property.FindPropertyRelative("damagesMin").intValue, 50);

           // _rect = new Rect(position.position.x, position.position.y + 120, position.width, 20);

        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty(); 

    }
    #endregion

    #endregion
}
