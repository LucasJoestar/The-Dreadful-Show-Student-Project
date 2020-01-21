using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq; 

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
    private SerializedProperty fakirQuote = null;
    private SerializedProperty mimeQuote = null;
    private SerializedProperty acrobatQuote = null;
    private SerializedProperty mightyQuote = null;
    private SerializedProperty catQuote = null;
    private SerializedProperty tauntQuote = null; 

    private SerializedProperty tauntRateMin = null;
    private SerializedProperty tauntRateMax = null; 
    #endregion

    #region Methods

    #region Original Methods
    private void DrawLoadQuoteButton(SerializedProperty _quoteProperty)
    {
        Color _originalColor = GUI.color;
        GUI.color = new Color(.7f, .35f, .75f);
        if (GUILayout.Button(new GUIContent("Load Quote", "Loads the quote with the ID entered as Text ID"), GUILayout.Width(150)))
        {
            string _fieldInfo = _quoteProperty.FindPropertyRelative("Name").stringValue;
            if (_fieldInfo != string.Empty)
            {
                TDS_NarratorQuote _quote = ((TDS_NarratorManager)Resources.Load(TDS_NarratorManager.FILE_PATH)).Quotes.FirstOrDefault(q => q.Name == _fieldInfo);
                if (_quote != null)
                {
                    _quoteProperty.FindPropertyRelative("audioTrack").objectReferenceValue = _quote.AudioTrack;
                    _quoteProperty.FindPropertyRelative("quote_fr").stringValue = _quote.QuoteFr;
                    _quoteProperty.FindPropertyRelative("quote_en").stringValue = _quote.QuoteEn;
                }
            }
        }

        GUI.color = _originalColor;
    }

    private void DrawLoadQuotesButton(SerializedProperty _quotesProperty)
    {
        Color _originalColor = GUI.color;
        GUI.color = new Color(.7f, .35f, .75f);
        if (GUILayout.Button(new GUIContent("Load Quotes", "Loads the quote with the ID entered as Text ID"), GUILayout.Width(150)))
        {
            for (int i = 0; i < _quotesProperty.arraySize; i++)
            {
                string _fieldInfo = _quotesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Name").stringValue;
                if (_fieldInfo != string.Empty)
                {
                    TDS_NarratorQuote _quote = ((TDS_NarratorManager)Resources.Load(TDS_NarratorManager.FILE_PATH)).Quotes.FirstOrDefault(q => q.Name == _fieldInfo);
                    if (_quote != null)
                    {
                        _quotesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("audioTrack").objectReferenceValue = _quote.AudioTrack;
                        _quotesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("quote_fr").stringValue = _quote.QuoteFr;
                        _quotesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("quote_en").stringValue = _quote.QuoteEn;
                    }
                }
            }            
        }
        GUI.color = _originalColor;
    }
    #endregion

    #region Unity Methods
    protected override void OnEnable()
    {
        base.OnEnable();
        linkedAreas = serializedObject.FindProperty("linkedAreas");
        cats = serializedObject.FindProperty("cats");
        chargeCatsRate = serializedObject.FindProperty("chargeCatsRate");
        teleportationPosition = serializedObject.FindProperty("teleportationPosition");
        fakirQuote = serializedObject.FindProperty("fakirQuote");
        mimeQuote = serializedObject.FindProperty("mimeQuote");
        acrobatQuote = serializedObject.FindProperty("acrobatQuote");
        mightyQuote = serializedObject.FindProperty("mightyManQuote");
        catQuote = serializedObject.FindProperty("catQuote");
        tauntRateMin = serializedObject.FindProperty("tauntRateMin");
        tauntRateMax = serializedObject.FindProperty("tauntRateMax");
        tauntQuote = serializedObject.FindProperty("tauntQuotes"); 
    }

    protected override void DrawSettings()
    {
        base.DrawSettings();

        TDS_EditorUtility.PropertyField("Area activated by Mr Loyal", "Area used in the top phase of Mr Loyal", linkedAreas);
        TDS_EditorUtility.PropertyField("Cats", "Mr Loyal's cats", cats);
        TDS_EditorUtility.FloatSlider("Cats' charge rate", "Waiting seconds before the cats will attack", chargeCatsRate, 5, 25);
        TDS_EditorUtility.Vector3Field("Teleportation Position", "Position where Mr Loyal's will teleport himself", teleportationPosition);

        EditorGUILayout.LabelField("SOUNDS", TDS_EditorUtility.HeaderStyle);
        TDS_EditorUtility.PropertyField("Callout Fakir Quote", "", fakirQuote);
        DrawLoadQuoteButton(fakirQuote); 
        TDS_EditorUtility.PropertyField("Callout Mime Quote", "", mimeQuote);
        DrawLoadQuoteButton(mimeQuote);
        TDS_EditorUtility.PropertyField("Callout Acrobat Quote", "", acrobatQuote);
        DrawLoadQuoteButton(acrobatQuote);
        TDS_EditorUtility.PropertyField("Callout MightyMan Quote", "", mightyQuote);
        DrawLoadQuoteButton(mightyQuote);
        TDS_EditorUtility.PropertyField("Callout Cat Quote", "", catQuote);
        DrawLoadQuoteButton(catQuote);
        TDS_EditorUtility.PropertyField("Taunt Quotes", "", tauntQuote);
        DrawLoadQuotesButton(tauntQuote);


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
