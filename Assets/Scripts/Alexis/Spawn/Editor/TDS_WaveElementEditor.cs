using UnityEngine;
using UnityEditor;
using UtilsLibrary.GUILibrary; 

[CustomPropertyDrawer(typeof(TDS_WaveElement))]
public class TDS_WaveElementEditor : PropertyDrawer
{
    /* TDS_WaveElementEditor :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *  Editor of the WaveElement class
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[12/02/2019]
	 *	Author :		[THIEBAUT Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation de la class TDS_WaveElementEditor]
     *	    - Implémentation de la méthode OnGUI pour afficher les settings de WaveElement
	 *
	 *	-----------------------------------
	*/

    #region Methods
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Int sliders to display and modify the min and max random Spawn
        Rect _rect = new Rect(0,0,0,0);
        if (property.FindPropertyRelative("minRandomSpawn").arraySize == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                property.FindPropertyRelative("minRandomSpawn").InsertArrayElementAtIndex(0);
                property.FindPropertyRelative("minRandomSpawn").GetArrayElementAtIndex(i).intValue = 0; 
            }
        }
        if (property.FindPropertyRelative("maxRandomSpawn").arraySize == 0)
        {
            for (int i = 0; i < 4; i++)
            {
                property.FindPropertyRelative("maxRandomSpawn").InsertArrayElementAtIndex(0);
                property.FindPropertyRelative("maxRandomSpawn").GetArrayElementAtIndex(i).intValue = 1;
            }
        }

        for (int i = 0; i < property.FindPropertyRelative("minRandomSpawn").arraySize; i++)
        {
            _rect = new Rect(position.position.x, position.position.y + (75 * i), position.width - 25, 20);
            EditorGUI.LabelField(_rect, $"{i + 1} Players", TDS_EditorUtility.HeaderStyle); 
            _rect = new Rect(position.position.x, position.position.y + 25 + (75 * i), position.width - 25, 20);
            property.FindPropertyRelative("minRandomSpawn").GetArrayElementAtIndex(i).intValue = EditorGUI.IntSlider(_rect, $"Min Random Spawn for {i + 1} player", property.FindPropertyRelative("minRandomSpawn").GetArrayElementAtIndex(i).intValue, 0, property.FindPropertyRelative("maxRandomSpawn").GetArrayElementAtIndex(i).intValue);

            _rect = new Rect(position.position.x, position.position.y + (50 + (75 * i)), position.width - 25, 20);
            property.FindPropertyRelative("maxRandomSpawn").GetArrayElementAtIndex(i).intValue = EditorGUI.IntSlider(_rect, $"Max Random Spawn for {i + 1} player", property.FindPropertyRelative("maxRandomSpawn").GetArrayElementAtIndex(i).intValue, property.FindPropertyRelative("minRandomSpawn").GetArrayElementAtIndex(i).intValue, 10);
        }
        

        for (int i = 0; i < 50; i++)
        {
            EditorGUILayout.Space(); 
        }


        GUILayout.BeginVertical("Box");
        GUILayout.Label("Normal Spawns", TDS_EditorUtility.HeaderStyle);
        // Display the settings of the normal spawning Informations
        for (int i = 0; i < property.FindPropertyRelative("spawningInformations").arraySize; i++)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative($"spawningInformations.Array.data[{i}]"));
            //Create a button to remove the spawning informations
            GUITools.ActionButton("Delete this info", property.FindPropertyRelative($"spawningInformations.Array").DeleteArrayElementAtIndex, i, Color.white, Color.black); 
        }
        GUILayout.EndVertical();

        //GUI.backgroundColor = TDS_EditorUtility.BoxDarkColor;
        GUILayout.BeginVertical("Box");
        GUILayout.Label("Random Spawns", TDS_EditorUtility.HeaderStyle);
        // Display the settings of the random spawning Informations
        for (int i = 0; i < property.FindPropertyRelative("randomSpawningInformations").arraySize; i++)
        {
            EditorGUILayout.PropertyField(property.FindPropertyRelative($"randomSpawningInformations.Array.data[{i}]"));
            //Create a button to remove the spawning informations
            GUITools.ActionButton("Delete this info", property.FindPropertyRelative($"randomSpawningInformations.Array").DeleteArrayElementAtIndex, i, Color.white, Color.black);
        }
        GUILayout.Space(5);
        GUILayout.EndVertical();

    }
    #endregion
}
