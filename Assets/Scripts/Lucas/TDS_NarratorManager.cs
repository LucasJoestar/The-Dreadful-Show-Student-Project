using System;
using System.Linq;
using UnityEngine;

using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TDS_NarratorManager : ScriptableObject
{
    /* TDS_NarratorManager :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *	
     *  Date :			
     *	Author :		
     *
     *	Changes :
     *
    */

    #region Fields / Properties

    #region Constant
    /// <summary>
    /// Path of the file from a resources folder.
    /// </summary>
    public static string FILE_PATH = "Narrator/NarratorManager";
    #endregion

    #region Variables
    /// <summary>
    /// Al quotes groups.
    /// </summary>
    [SerializeField] private TDS_NarratorQuoteGroup[] quoteGroups = new TDS_NarratorQuoteGroup[] { };

    /// <summary>
    /// Get all narrator quotes from the game
    /// </summary>
    public TDS_NarratorQuote[] Quotes { get { return quoteGroups.SelectMany(g => g.Quotes).ToArray(); } }
    #endregion

    #endregion

    #region Methods

    #if UNITY_EDITOR
    /// <summary>
    /// Get the narrator manager or create it if exist.
    /// </summary>
    [MenuItem("Tools/Create Narrator Manager")]
    public static void CreateNarratorManager()
    {
        Object _file = Resources.Load(FILE_PATH);
        // If the asset already exist, just return
        if (_file != null)
        {
            Selection.activeObject = _file;
            return;
        }

        TDS_NarratorManager _narrator = CreateInstance<TDS_NarratorManager>();

        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, "Resources", System.IO.Path.GetDirectoryName(FILE_PATH)));

        AssetDatabase.CreateAsset(_narrator, System.IO.Path.Combine("Assets/Resources", FILE_PATH) + ".asset");
        AssetDatabase.SaveAssets();
    }
    #endif

    #endregion
}

[Serializable]
public class TDS_NarratorQuoteGroup
{
    /* TDS_NarratorQuoteGroup :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *	
     *  Date :			
     *	Author :		
     *
     *	Changes :
     *
    */

    #region Fields / Properties
    /// <summary>
    /// Name of the group.
    /// </summary>
    public string Name = "New Group";

    /// <summary>
    /// All quotes from the group.
    /// </summary>
    [SerializeField] private TDS_NarratorQuote[] quotes = new TDS_NarratorQuote[] { };

    /// <summary>Public accessor for <see cref="quotes"/>.</summary>
    public TDS_NarratorQuote[] Quotes { get { return quotes; } }
    #endregion
}

[Serializable]
public class TDS_NarratorQuote
{
    /* TDS_NarratorQuote :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	#####################
     *	### MODIFICATIONS ###
     *	#####################
     *	
     *  Date :			
     *	Author :		
     *
     *	Changes :
     *
    */

    #region Fields / Properties
    /// <summary>
    /// Name of the quote, use to identify it.
    /// </summary>
    public string Name = "New Quote";

    /// <summary>
    /// Audio track of the quote.
    /// </summary>
    [SerializeField] private AudioClip audioTrack = null;

    /// <summary>Public accessor for <see cref="audioTrack"/>.</summary>
    public AudioClip AudioTrack { get { return audioTrack; } }

    /// <summary>
    /// Quote, as text in French.
    /// </summary>
    [SerializeField] private string quote_fr = string.Empty;

    /// <summary>
    /// Quote, as text in English
    /// </summary>
    [SerializeField] private string quote_en = string.Empty; 

    /// <summary>Public accessor for <see cref="quote_fr"/>.</summary>
    public string Quote { get { return TDS_GameManager.LocalisationIsEnglish ? quote_en : quote_fr; } }
    #endregion
}
