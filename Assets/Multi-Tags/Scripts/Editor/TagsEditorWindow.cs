using System.Linq;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor window to manages all this project tags.
/// </summary>
public class TagsEditorWindow : EditorWindow 
{
    /* TagsEditorWindow :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Editor window used to add, remove and personalize project tags.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     * 
     *      • Search system for tags.
     *      
     *      • Remove tag from all objects in loaded scene(s).
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[04 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Moved editor to the TagsEditor class for TagsSO custom editor.
     *	Now, this window display the editor of the object.
     * 
	 *	-----------------------------------
     * 
     *	Date :			[03 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    Finally, tags can be created & removed from this window, and everything is fully
     *	saved on a scriptable object reference. Pretty cool, it is.
     *	    Still, tags are not displayed the way I want to, so got to find a way to display them
     *	horizontally and automatically go to the line when reaching the end of the screen.
     *	    Hard, it is. When trying to check this by rect size, 'got a weird error 'cause events
     *	Layout & Repaint have different informations.
     *	
     *	    At worst, can try with non-layout GUI.
	 *
     *	-----------------------------------
     * 
     *	Date :			[30 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    The window loads & write dynamically project tags on a scriptable object in the
     *	Resources folder. Plus, all of them are drawn on the window. Pretty cool.
     * 
	 *	-----------------------------------
     * 
	 *	Date :			[20 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TagsEditorWindow class.
     *	    
     *	    The window can now be called from a Menu Toolbar button, and... That's it.
     *	Yeah, I know...
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="Reference"/>.</summary>
    private TagsSO reference = null;

    /// <summary>
    /// Editor of the tags object reference.
    /// </summary>
    private TagsEditor referenceEditor = null;
    #endregion

    #region Methods

    #region Original Methods

    #region Editor
    /// <summary>
    /// Draws the toolbar on the top of the tags editor window.
    /// </summary>
    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Draws a button to create a brand new cool tag
        if (GUILayout.Button("Create Tag", EditorStyles.toolbarButton))
        {
            GetWindow<CreateTagWindow>("Create new Tag").Show(this);
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
    #endregion

    #region Menu Navigation
    /// <summary>
    /// Opens the tags editor window.
    /// </summary>
    [MenuItem("Window/Tags")]
    public static TagsEditorWindow CallWindow()
    {
        TagsEditorWindow _window = GetWindow<TagsEditorWindow>("Tags Editor");
        _window.Show();
        return _window;
    }
    #endregion

    #endregion

    #region Unity Methods
    // This function is called when the scriptable object goes out of scope
    private void OnDisable()
    {
        // Destroys the created editor
        if (referenceEditor) DestroyImmediate(referenceEditor);
    }

    // This function is called when the object is loaded
    private void OnEnable()
    {
        // Get the scriptable object of the project containing all tags and load them
        reference = MultiTagsUtility.GetTagsAsset();

        // Creates editor for the reference
        referenceEditor = (TagsEditor)Editor.CreateEditor(reference);
    }

    // Implement your own editor GUI here
    private void OnGUI()
    {
        if (!reference)
        {
            // Call the OnDisable & OnEnable methods to refresh window
            OnDisable();
            OnEnable();
        }

        // Draws tags editor
        DrawToolbar();
        referenceEditor.DrawTagsEditor();
    }

    // OnInspectorUpdate is called at 10 frames per second to give the inspector a chance to update
    private void OnInspectorUpdate()
    {
        // Repaint the editor. This is necessary to display tags color when modifying them.
        Repaint();
    }
    #endregion

    #endregion
}

/// <summary>
/// Editor window used to create a new tag.
/// </summary>
public class CreateTagWindow : EditorWindow
{
    /* CreateTagWindow :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	    Editor window used to create a new tag.
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *
     *	Date :			[03 / 02 / 2019]
     *	Author :		[Guibert Lucas]
     *
     *	Changes :
     *
     *	Creation of the CreateTagWindow class.
     *	    
     *	    This is a little cool window that allow to create a new tag.
     *	If the tag cannot be created, a little message indicates it to the user.
     *	When created, the window closes by itself. And, it works fine.
     *
     *	-----------------------------------
    */

    #region Fields & Properties
    /// <summary>
    /// Name of the new tag to create.
    /// </summary>
    private string tagName = "New Tag";

    /// <summary>
    /// Color of the new tag to create.
    /// </summary>
    private Color color = Color.white;

    /// <summary>
    /// Indicates if the name entered is empty.
    /// </summary>
    private bool isNameEmpty = false;

    /// <summary>
    /// Indicates if a tag with the same name already exist or not.
    /// </summary>
    private bool doesNameAlreadyExist = false;

    /// <summary>
    /// Indicates if the name entered contains the tag separator or not.
    /// </summary>
    private bool doesNameContainSeparator = false;

    /// <summary>
    /// Reference editor creating tags for.
    /// </summary>
    private TagsEditor reference = null;
    #endregion

    #region Methods

    #region Original Method
    /// <summary>
    /// Initializes the window with an editor reference.
    /// </summary>
    /// <param name="_reference">Editor to create tag for.</param>
    public void Show(TagsEditor _reference) => reference = _reference;

    /// <summary>
    /// Draws this editor window content.
    /// </summary>
    private void DrawEditor()
    {
        // If the name contains the tag separator, indicate it
        if (doesNameContainSeparator)
        {
            EditorGUILayout.HelpBox("The name of the tag cannot contains \'" + MultiTags.TagSeparator + "\'", MessageType.Error);
        }
        // If the name is not valid, indicate it
        else if (doesNameAlreadyExist)
        {
            EditorGUILayout.HelpBox("A tag with the same name already exist", MessageType.Error);
        }
        // If the name is empty, indicate it
        else if (isNameEmpty)
        {
            EditorGUILayout.HelpBox("You must enter a name", MessageType.Error);
        }

        EditorGUILayout.BeginHorizontal();

        // Set the name & color of the new tag
        EditorGUILayout.LabelField(new GUIContent("Name :", "Name of the new tag to create"), GUILayout.Width(50));
        tagName = EditorGUILayout.TextField(tagName, GUILayout.Width(100));
        EditorGUILayout.LabelField(new GUIContent("Color :", "Color of the new tag to create"), GUILayout.Width(50));
        color = EditorGUILayout.ColorField(color, GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
        {
            // If the new tag name entered contains the tag separator, indicate it and refuse to create the tag
            if (tagName.Contains(MultiTags.TagSeparator))
            {
                doesNameContainSeparator = true;
                doesNameAlreadyExist = false;
                isNameEmpty = false;

                SetBigSize();
                Repaint();
                return;
            }
            else if (doesNameContainSeparator)
            {
                doesNameContainSeparator = false;
                SetSmallSize();
                Repaint();
            }

            // If a tag with the same name already exist, indicate it and refuse to create the tag
            if (MultiTagsUtility.GetUnityTags().Contains(tagName))
            {
                doesNameAlreadyExist = true;
                isNameEmpty = false;

                SetBigSize();
                Repaint();
                return;
            }
            else if (doesNameAlreadyExist)
            {
                doesNameAlreadyExist = false;
                SetSmallSize();
                Repaint();
            }

            // If the name entered is empty, indicate it and refuse to create the tag
            if (string.IsNullOrEmpty(tagName))
            {
                isNameEmpty = true;

                SetBigSize();
                Repaint();
                return;
            }

            // If everything is okay, create the new tag
            if (reference)
            {
                reference.CreateTag(new Tag(tagName, color));
            }
            else
            {
                TagsSO _tagsSO = MultiTagsUtility.GetTagsAsset();

                TagsEditor _editor = (TagsEditor)Editor.CreateEditor(_tagsSO);
                _editor.CreateTag(new Tag(tagName, color));

                DestroyImmediate(_editor);
            }

            Close();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// Set the window size as "big".
    /// </summary>
    private void SetBigSize()
    {
        Vector2 _size = new Vector2(300, 95);
        minSize = _size;
        maxSize = _size;
    }

    /// <summary>
    /// Set the window size as "small".
    /// </summary>
    private void SetSmallSize()
    {
        Vector2 _size = new Vector2(300, 50);
        minSize = _size;
        maxSize = _size;
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        SetSmallSize();
        ShowUtility();
    }

    // Implement your own editor GUI here
    private void OnGUI()
    {
        DrawEditor();
    }
    #endregion

    #endregion
}
