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
     *      Nothing to see here...
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
	 *	    • Moved editor to the TagsEditor class for TagsSO custom editor.
     *	Now, this window display the editor of the object.
     * 
	 *	-----------------------------------
     * 
     *	Date :			[03 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
     *	    • Finally, tags can be created & removed from this window, and everything is fully
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
	 *	    • The window loads & write dynamically project tags on a scriptable object in the
     *	Resources folder. Plus, all of them are drawn on the window. Pretty cool.
     * 
	 *	-----------------------------------
     * 
	 *	Date :			[20 / 01 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Creation of the TagsEditorWindow class.
     *	    
     *	    • The window can now be called from a Menu Toolbar button, and... That's it.
     *	Yeah, I know...
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// E-mail address used for contact.
    /// </summary>
    private const string                            CONTACT_EMAIL                   = "LucasGuibert.Pro@laposte.net";


    /// <summary>Backing field for <see cref="Reference"/>.</summary>
    [SerializeField] private TagsSO                 reference                       = null;

    /// <summary>
    /// Editor of the tags object reference.
    /// </summary>
    [SerializeField] private TagsEditor             referenceEditor                 = null;


    /// <summary>
    /// Index used for the window toolbar.
    /// </summary>
    [SerializeField] private int                    toolbarIndex                    = 0;

    /// <summary>
    /// All available options of the toolbar.
    /// </summary>
    [SerializeField] private readonly GUIContent[]  toolbarOptions                  = new GUIContent[] { new GUIContent("Tags", "Edit the tags of the project."), new GUIContent("Informations", "Everything you need to know to perfectly use these tags."), new GUIContent("Contact", "How to contact me, if you have any question or suggestion.") };


    /// <summary>
    /// GUIStyle used to draw a clickable link.
    /// </summary>
    [SerializeField] private GUIStyle               linkStyle                       = null;


    /// <summary>
    /// Vector of this window scrollbar.
    /// </summary>
    [SerializeField] private Vector2                scrollbar                       = Vector2.zero;
    #endregion

    #region Methods

    #region Original Methods

    #region Editor Window
    /// <summary>
    /// Draws the toolbar on the top of the tags editor window.
    /// </summary>
    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Draws a button to create a brand new cool tag
        if (GUILayout.Button("Create Tag", EditorStyles.toolbarButton))
        {
            CreateTagWindow.CallWindow();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws tags of the project.
    /// </summary>
    private void DrawTags()
    {
        referenceEditor.DrawTags();
    }

    /// <summary>
    /// Draws informations about tags and how to use them.
    /// </summary>
    private void DrawInformations()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Multi-Tags System : What it is and how does it works", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("     This Multi-Tags system has been created it the purpose of adding multiple tags to a single GameObject. " +
                        "Its structure is reflected around the Unity tag system, to avoid adding any component, and allowing the use it directly on any GameObject.\n\n" +
                        "This system works by creating a Unity tag containing all your GameObject tags separated by a specific char.\n\n" +
                        "All created tags are stored in a Scriptable Object located in a Resources folder, in another folder named \"Tags\". " +
                        "You can copy / paste this asset to import the tags of a project to another one.", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("How to add tags to a GameObject", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("     You can add any created tag to a GameObject by using the little \"+\" button in top of the inspector, in the Tag section. " +
                        "To remove a tag, just click on the \"-\" button to the left of the tag you want to remove.\n\n" +
                        "You can create a new tag in the Tags editor window, which you can open by clicking on the \"Edit Tags\" button to the right of the Tag section in the Inspector " +
                        "of any GameObject, or in the main toolbar by clicking on \"Window/Tags\".", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("The Tag & Tags classes", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("     In order to simplify the use of tags, two classes are here to help you : the Tag & Tags ones.\n\n" +
                        "   • The first one, \"Tag\", allows you to stock a simple tag reference in a field. You can access this tag name with the \"Name\" field inside it.\n\n" +
                        "   • The second one, \"Tags\", allows you to stock multiple tag references in one field. It contains a Tag array in the field named \"ObjectTags\", " +
                        "along with multiple methods to add or remove tags from it. You can also get all this object tag names with the property \"TagNames\".\n\n" +
                        "Those two classes also have their own property drawer, to make them more pleasant to use.", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Compare GameObjects tags", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("     You can access any GameObject tags with the \"GetTags\" and \"GetTagNames\" extension methods. " +
                        "To compare the tags of a GameObject, you can use the \"HasTag\" and \"HasTags\" extension methods. Note that for this, you can store tags to compare to " +
                        "in your own scripts in a \"Tag\" or \"Tags\" field, two classes which are described just above.", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Useful utility classes", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("     There are also two big static classes that should prove their utility if you want to have more control over your tags.\n\n" +
                        "   • The first one if the \"MultiTags\" class : this is a runtime accessible class with multiple useful methods and properties allowing things such as " +
                        "finding a GameObject with a specific tag, getting all tags of the game, getting the Tag object from a tag name, adding a new tag to the game at runtime, and more.\n\n" +
                        "   • The second one is called \"MultiTagsUtility\" : this is an editor only class with multiple useful methods and properties, allowing things like adding or removing " +
                        "a tag to a GameObject, creating or destroying a tag, or draw Tag and Tags fields in the Editor with or without auto-layout.", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Other stuff", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("     All classes, along with their methods, fields and properties, are fully commentated, so feel free to explore them so that you'll get a better understanding " +
                        "of their behaviour and all the things you can do with them.\n\n" +
                        "Note that you cannot change a GameObject tags at runtime. Well, that's true ; by working around the Unity tag system, this contrains such possibilities, since " +
                        "tags need to be created before compilation.\n " +
                        "But in fact, there is a trick : you can access your GameObject tags with the \"GetTags\" extension method, which returns you a Tag array, and then create " +
                        "a Tags object with it, and store it in a field. Then, you can change the tags contained in this field, and compare these tags instead of " +
                        "the GameObject ones. Yeah, this is not the most practical way to proceed, but it's a possible way to do that.", EditorStyles.wordWrappedLabel);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("That's the end of that overview", EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("     Well, now you have all the informations that you need to use this Multi-Tags system ; so go on, give it a try, and I hope you'll have fun with it !",
                        EditorStyles.wordWrappedLabel);

        GUILayout.Space(15);
    }

    /// <summary>
    /// Draw contact informations.
    /// </summary>
    private void DrawContact()
    {
        GUILayout.Space(10);

        GUILayout.Label("     If you encounter any bug, issue, or other problem with this Multi-Tags system, or if you have any suggestion to improve this plugin, " +
                        "you can reach me at the following e-mail address :\n", EditorStyles.wordWrappedLabel);

        if (linkStyle == null)
        {
            linkStyle = new GUIStyle(EditorStyles.label);
            linkStyle.richText = true;
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(10);
        if (GUILayout.Button("<b><color=blue>" + CONTACT_EMAIL + "</color></b>", linkStyle))
        {
            Application.OpenURL("mailto: " + CONTACT_EMAIL + " ? subject = " + "Multi-Tags");

        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(15);
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
            // Call the OnDisable & OnEnable methods to refresh the window
            OnDisable();
            OnEnable();
        }

        // Start the scroll view
        scrollbar = EditorGUILayout.BeginScrollView(scrollbar);

        // Draw a mini toolbar followed by a menu toolbat in top of the window
        DrawToolbar();
        toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarOptions, GUILayout.Height(25), GUILayout.ExpandWidth(true));

        GUILayout.Space(5);

        switch (toolbarIndex)
        {
            case 0:
                // Edit this project tags
                DrawTags();
                break;

            case 1:
                // Show informations about tags
                DrawInformations();
                break;

            case 2:
                // How to contact me
                DrawContact();
                break;

            default:
                // Nothing to see here...
                break;
        }

        // End the scroll view
        EditorGUILayout.EndScrollView();
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
    private string          tagName                         = "New Tag";

    /// <summary>
    /// Color of the new tag to create.
    /// </summary>
    private Color           color                           = Color.white;

    /// <summary>
    /// Indicates if the name entered is empty.
    /// </summary>
    private bool            isNameEmpty                     = false;

    /// <summary>
    /// Indicates if a tag with the same name already exist or not.
    /// </summary>
    private bool            doesNameAlreadyExist            = false;

    /// <summary>
    /// Indicates if the name entered contains the tag separator or not.
    /// </summary>
    private bool            doesNameContainSeparator        = false;
    #endregion

    #region Methods

    #region Original Method
    /// <summary>
    /// Call this editor window.
    /// </summary>
    public static void CallWindow()
    {
        GetWindow<CreateTagWindow>("Create new Tag").Show();
    }

    /// <summary>
    /// Draws this editor window content.
    /// </summary>
    private void DrawEditor()
    {
        EditorGUILayout.BeginHorizontal();

        // Set the name & color of the new tag
        EditorGUILayout.LabelField(new GUIContent("Name :", "Name of the new tag to create"), GUILayout.Width(50));
        EditorGUI.BeginChangeCheck();
        tagName = EditorGUILayout.TextField(tagName, GUILayout.Width(100));
        if (EditorGUI.EndChangeCheck()) CheckName();

        EditorGUILayout.LabelField(new GUIContent("Color :", "Color of the new tag to create"), GUILayout.Width(50));
        color = EditorGUILayout.ColorField(color, GUILayout.Width(100));

        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create", EditorStyles.miniButton, GUILayout.ExpandWidth(false)) && !doesNameAlreadyExist && !doesNameContainSeparator && !isNameEmpty)
        {
            // If everything is okay, create the new tag
            MultiTagsUtility.CreateTag(new Tag(tagName, color));

            Close();
        }
        EditorGUILayout.EndHorizontal();

        // If the name contains the tag separator, indicate it
        if (doesNameContainSeparator)
        {
            EditorGUILayout.HelpBox("The name of the tag cannot contains \'" + MultiTags.TAG_SEPARATOR + "\'.", MessageType.Warning);
        }
        // If the name is not valid, indicate it
        else if (doesNameAlreadyExist)
        {
            EditorGUILayout.HelpBox("A tag with the same name already exist.", MessageType.Warning);
        }
        // If the name is empty, indicate it
        else if (isNameEmpty)
        {
            EditorGUILayout.HelpBox("You must enter a name.", MessageType.Warning);
        }
    }

    /// <summary>
    /// Check if the tag name is valid.
    /// </summary>
    private void CheckName()
    {
        // If the new tag name entered contains the tag separator, indicate it and refuse to create the tag
        if (tagName.Contains(MultiTags.TAG_SEPARATOR))
        {
            doesNameContainSeparator = true;
            doesNameAlreadyExist = false;
            isNameEmpty = false;

            SetBigSize();
        }
        else if (doesNameContainSeparator)
        {
            doesNameContainSeparator = false;

            SetSmallSize();
        }

        // If a tag with the same name already exist, indicate it and refuse to create the tag
        if (MultiTagsUtility.GetUnityTags().Contains(tagName))
        {
            doesNameAlreadyExist = true;
            isNameEmpty = false;

            SetBigSize();
        }
        else if (doesNameAlreadyExist)
        {
            doesNameAlreadyExist = false;

            SetSmallSize();
        }

        // If the name entered is empty, indicate it and refuse to create the tag
        if (string.IsNullOrEmpty(tagName.Trim()))
        {
            isNameEmpty = true;

            SetBigSize();
        }
        else if (isNameEmpty)
        {
            isNameEmpty = false;
            SetSmallSize();
        }
    }

    /// <summary>
    /// Set the window size as "big".
    /// </summary>
    private void SetBigSize()
    {
        Vector2 _size = new Vector2(300, 92);
        minSize = _size;
        maxSize = _size;

        Repaint();
    }

    /// <summary>
    /// Set the window size as "small".
    /// </summary>
    private void SetSmallSize()
    {
        Vector2 _size = new Vector2(300, 50);
        minSize = _size;
        maxSize = _size;

        Repaint();
    }
    #endregion

    #region Unity Methods
    // This function is called when the object is loaded
    private void OnEnable()
    {
        SetSmallSize();
        CheckName();
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
