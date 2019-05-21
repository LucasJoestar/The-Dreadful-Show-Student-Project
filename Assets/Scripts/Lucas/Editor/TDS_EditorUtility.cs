using System;
using UnityEditor;
using UnityEngine;

public sealed class TDS_EditorUtility 
{
    /* TDS_EditorUtility :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	    Sealed class referring some useful fields & methods to use in Editor.

	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *  Date :			[11 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the RadioToggle method.
     * 
     *  -----------------------------------
     * 
     *  Date :			[04 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the Vector3Field method.
     * 
     *  -----------------------------------
     * 
     *  Date :			[29 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
     *	
     *	    - Added the PropertyField & TextField methods.
     * 
     *  -----------------------------------
     * 
     *	Date :			[24 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *      - Added the BoxDarkColor & BoxLightColor properties.
     *      - Added the FloatField & FloatSlider methods.
	 *	    - Added one overload for both Button & Toggle methods.
     *	    - Changed the button method.
	 *
	 *	-----------------------------------
     *	
	 *	Date :			[23 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_EditorUtility class.
     *	
     *	    - Added the isLoaded field ; and the headerStyle & labelStyle fields & properties.
     *	    - Added the LoadStyles, Button, IntField, IntSlider, ObjectField, ProgressBar & Toggle methods.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Indicates if the styles are already loaded or not
    /// </summary>
    private static bool isLoaded = false;

    /// <summary>
    /// Color for the dark box background
    /// </summary>
    public static Color BoxDarkColor { get; private set; } = new Color(.55f, .55f, .55f);

    /// <summary>
    /// Color for the light box background
    /// </summary>
    public static Color BoxLightColor { get; private set; } = new Color(.9f, .9f, .9f);

    /// <summary>Backing field for <see cref="HeaderStyle"/>.</summary>
    private static GUIStyle headerStyle = null;

    /// <summary>
    /// GUIStyle planned for headers.
    /// </summary>
    public static GUIStyle HeaderStyle
    {
        get
        {
            if (headerStyle == null) LoadStyles();
            return headerStyle;
        }
        private set { headerStyle = value; }
    }

    /// <summary>Backing field for <see cref="LabelStyle"/>.</summary>
    private static GUIStyle labelStyle = null;

    /// <summary>
    /// GUIStyle planned for labels.
    /// </summary>
    public static GUIStyle LabelStyle
    {
        get
        {
            if (labelStyle == null) LoadStyles();
            return labelStyle;
        }
        private set { labelStyle = value; }
    }
	#endregion

	#region Methods
    /// <summary>
    /// Loads all GUIStyle of the class, allowing the user to use them.
    /// </summary>
    public static void LoadStyles()
    {
        // If the styles have already been loaded, return
        if (isLoaded) return;

        // Prepare all GUIStyles for use
        headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.fontSize = 12;
        headerStyle.padding = new RectOffset(3, 3, 3, 3);
        headerStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        labelStyle = new GUIStyle();
        labelStyle.fontSize = 11;
        labelStyle.padding = new RectOffset(15, 15, 0, 0);
        labelStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

        // Set the boolean indicating all styles have been loaded
        isLoaded = true;
    }

    #region GUI
    /// <summary>
    /// Makes a custom button.
    /// </summary>
    /// <param name="_label">Label to dispaly.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse oveR.</param>
    /// <param name="_guiStyle">GUIStyle to use to display the label.</param>
    /// <returns>Returns true if the user clicked on it, false otherwise.</returns>
    public static bool Button(string _label, string _tooltip, GUIStyle _guiStyle)
    {
        if (GUILayout.Button(new GUIContent(_label, _tooltip), _guiStyle))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Makes a custom button.
    /// </summary>
    /// <param name="_label">Label to dispaly.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_guiStyle">GUIStyle to use to display the label.</param>
    /// <param name="_callback">Method to invoke when clicking on the button.</param>
    /// <returns>Returns true if the user clicked on it, false otherwise.</returns>
    public static bool Button(string _label, string _tooltip, GUIStyle _guiStyle, Action _callback)
    {
        if (GUILayout.Button(new GUIContent(_label, _tooltip), _guiStyle))
        {
            _callback.Invoke();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Draws a custom float field.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool FloatField(string _label, string _tooltip, SerializedProperty _serializedProperty)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the float field next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.showMixedValue = _serializedProperty.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();
        float _newValue = EditorGUILayout.FloatField(_serializedProperty.floatValue, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));
        bool _hasChanged = EditorGUI.EndChangeCheck();

        EditorGUI.showMixedValue = false;

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        if (_hasChanged)
        {
            _serializedProperty.floatValue = _newValue;
        }

        return _hasChanged;
    }

    /// <summary>
    /// Draws a custom float slider.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <param name="_min">Minimum value of the slider.</param>
    /// <param name="_max">Maximum value of the slider.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool FloatSlider(string _label, string _tooltip, SerializedProperty _serializedProperty, float _min, float _max)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the float slider next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Slider(_serializedProperty, _min, _max, string.Empty, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        return EditorGUI.EndChangeCheck();
    }

    /// <summary>
    /// Draws a custom int field.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool IntField(string _label, string _tooltip, SerializedProperty _serializedProperty)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the int field next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.showMixedValue = _serializedProperty.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();
        int _newValue = EditorGUILayout.IntField(_serializedProperty.intValue, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));
        bool _hasChanged = EditorGUI.EndChangeCheck();

        EditorGUI.showMixedValue = false;

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        if (_hasChanged)
        {
            _serializedProperty.intValue = _newValue;
        }

        return _hasChanged;
    }

    /// <summary>
    /// Draws a custom int slider.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <param name="_min">Minimum value of the slider.</param>
    /// <param name="_max">Maximum value of the slider.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool IntSlider(string _label, string _tooltip, SerializedProperty _serializedProperty, int _min, int _max)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the int slider next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.IntSlider(_serializedProperty, _min, _max, string.Empty, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        return EditorGUI.EndChangeCheck();
    }

    /// <summary>
    /// Draws a custom object field.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <param name="_type">Type of the object that can be assigned.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool ObjectField(string _label, string _tooltip, SerializedProperty _serializedProperty, Type _type)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the int slider next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.ObjectField(_serializedProperty, _type, new GUIContent(string.Empty), GUILayout.MinWidth(EditorGUIUtility.fieldWidth));

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        return EditorGUI.EndChangeCheck();
    }

    /// <summary>
    /// Draws a custom progress bar
    /// </summary>
    /// <param name="_height">Height size of the bar.</param>
    /// <param name="_percent">Bar filled percent.</param>
    /// <param name="_label">Label to display on the bar.</param>
    public static void ProgressBar(int _height, float _percent, string _label)
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(labelStyle.padding.left);
        EditorGUI.ProgressBar(GUILayoutUtility.GetRect(_height, _height), _percent, _label);

        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws a custom property field.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    public static bool PropertyField(string _label, string _tooltip, SerializedProperty _serializedProperty)
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(labelStyle.padding.left);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_serializedProperty, new GUIContent(_label, _tooltip), true);
        bool _hasChanges = EditorGUI.EndChangeCheck();

        EditorGUILayout.EndHorizontal();

        return _hasChanges;
    }

    /// <summary>
    /// Draws a custom radio toggle that only shows a properties value.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static void RadioToggle(string _label, string _tooltip, SerializedProperty _serializedProperty)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the int slider next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.showMixedValue = _serializedProperty.hasMultipleDifferentValues;

        EditorGUILayout.Toggle(_serializedProperty.boolValue, new GUIStyle("Radio"), GUILayout.MinWidth(EditorGUIUtility.fieldWidth));

        EditorGUI.showMixedValue = false;

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;
    }

    /// <summary>
    /// Draws a custom text field
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool TextField(string _label, string _tooltip, SerializedProperty _serializedProperty)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the string field next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.showMixedValue = _serializedProperty.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();
        string _newValue = EditorGUILayout.TextField(_serializedProperty.stringValue, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));
        bool _hasChanged = EditorGUI.EndChangeCheck();

        EditorGUI.showMixedValue = false;

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        if (_hasChanged)
        {
            _serializedProperty.stringValue = _newValue;
        }

        return _hasChanged;
    }

    /// <summary>
    /// Draws a custom toggle.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool Toggle(string _label, string _tooltip, SerializedProperty _serializedProperty)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the int slider next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.showMixedValue = _serializedProperty.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();
        bool _newValue = EditorGUILayout.Toggle(_serializedProperty.boolValue, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));
        bool _hasChanged = EditorGUI.EndChangeCheck();

        EditorGUI.showMixedValue = false;

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        if (_hasChanged)
        {
            _serializedProperty.boolValue = _newValue;
        }

        return _hasChanged;
    }

    /// <summary>
    /// Draws a custom toggle.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <param name="_autoSetProperty">Should the property automatically be set when the user wants to change its value or not.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool Toggle(string _label, string _tooltip, SerializedProperty _serializedProperty, bool _autoSetProperty)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the int slider next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.showMixedValue = _serializedProperty.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();
        bool _newValue = EditorGUILayout.Toggle(_serializedProperty.boolValue, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));
        bool _hasChanged = EditorGUI.EndChangeCheck();

        EditorGUI.showMixedValue = false;

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        if (_hasChanged && _autoSetProperty)
        {
            _serializedProperty.boolValue = _newValue;
        }

        return _hasChanged;
    }

    /// <summary>
    /// Draws a custom vector3 field.
    /// </summary>
    /// <param name="_label">Label to display.</param>
    /// <param name="_tooltip">Tooltip displayed when mouse over.</param>
    /// <param name="_serializedProperty">SerializedProperty to use.</param>
    /// <param name="_type">Type of the object that can be assigned.</param>
    /// <returns>Returns true if the value(s) has changed, false otherwise.</returns>
    public static bool Vector3Field(string _label, string _tooltip, SerializedProperty _serializedProperty)
    {
        // Get the original width of the labels for EditorGUI, and reduce it so that it will no longer take so much space
        float _originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth -= LabelStyle.padding.left;

        EditorGUILayout.BeginHorizontal();

        // Draws a label, and the int field next to it
        EditorGUILayout.LabelField(new GUIContent(_label, _tooltip), labelStyle, GUILayout.MaxWidth(EditorGUIUtility.labelWidth));

        EditorGUI.showMixedValue = _serializedProperty.hasMultipleDifferentValues;

        EditorGUI.BeginChangeCheck();
        Vector3 _newValue = EditorGUILayout.Vector3Field(string.Empty, _serializedProperty.vector3Value, GUILayout.MinWidth(EditorGUIUtility.fieldWidth));
        bool _hasChanged = EditorGUI.EndChangeCheck();

        EditorGUI.showMixedValue = false;

        EditorGUILayout.EndHorizontal();

        // Restore the original widths for EditorGUI labels
        EditorGUIUtility.labelWidth = _originalWidth;

        if (_hasChanged)
        {
            _serializedProperty.vector3Value = _newValue;
        }

        return _hasChanged;
    }
    #endregion

    #endregion
}
