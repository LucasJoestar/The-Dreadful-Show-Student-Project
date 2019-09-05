using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TDS_AudioSO : ScriptableObject
{
    /* TDS_AudioSO :
     *
     *	#####################
     *	###### PURPOSE ######
     *	#####################
     *
     *	
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
     *	Date :			
     *	Author :		
     *
     *	Changes :
     *
     *	-----------------------------------
    */

    #region Fields / Properties

    #region Constant
    /// <summary>
    /// Path of the file from a resources folder.
    /// </summary>
    public static string FILE_PATH = "Audio/AudioManager";
    #endregion

    #region Variables

    #region Music
    /// <summary>
    /// Music of the title screen.
    /// </summary>
    [Header("Musics")]
    [SerializeField] private AudioClip m_TitleScreen = null;

    /// <summary>Public accessor for <see cref="m_TitleScreen"/>.</summary>
    public AudioClip M_TitleScreen { get { return m_TitleScreen; } }

    /// <summary>
    /// Music of the intro cutscene.
    /// </summary>
    [SerializeField] private AudioClip m_Intro = null;

    /// <summary>Public accessor for <see cref="m_Intro"/>.</summary>
    public AudioClip M_Intro { get { return m_Intro; } }

    /// <summary>
    /// Music of the outro cutscene.
    /// </summary>
    [SerializeField] private AudioClip m_Outro = null;

    /// <summary>Public accessor for <see cref="m_Outro"/>.</summary>
    public AudioClip M_Outro { get { return m_Outro; } }

    /// <summary>
    /// Intro for the fight music.
    /// </summary>
    [SerializeField] private AudioClip m_FightIntro = null;

    /// <summary>Public accessor for <see cref="m_FightIntro"/>.</summary>
    public AudioClip M_FightIntro { get { return m_FightIntro; } }

    /// <summary>
    /// Loop of the fight music.
    /// </summary>
    [SerializeField] private AudioClip m_FightLoop = null;

    /// <summary>Public accessor for <see cref="m_FightLoop"/>.</summary>
    public AudioClip M_FightLoop { get { return m_FightLoop; } }

    /// <summary>
    /// Intro of the main game theme music.
    /// </summary>
    [SerializeField] private AudioClip m_ThemeIntro = null;

    /// <summary>Public accessor for <see cref="m_ThemeIntro"/>.</summary>
    public AudioClip M_ThemeIntro { get { return m_ThemeIntro; } }

    /// <summary>
    /// Loop of the main game theme music.
    /// </summary>
    [SerializeField] private AudioClip m_ThemeLoop = null;

    /// <summary>Public accessor for <see cref="m_ThemeLoop"/>.</summary>
    public AudioClip M_ThemeLoop { get { return m_ThemeLoop; } }
    #endregion

    #endregion

    #endregion

    #region Methods

#if UNITY_EDITOR
    /// <summary>
    /// Get the audio manager or create it if exist.
    /// </summary>
    [MenuItem("Tools/Create Audio Manager")]
    public static void CreateNarratorManager()
    {
        Object _file = Resources.Load(FILE_PATH);
        // If the asset already exist, just return
        if (_file != null)
        {
            Selection.activeObject = _file;
            return;
        }

        TDS_AudioSO _narrator = CreateInstance<TDS_AudioSO>();

        System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Application.dataPath, "Resources", System.IO.Path.GetDirectoryName(FILE_PATH)));

        AssetDatabase.CreateAsset(_narrator, System.IO.Path.Combine("Assets/Resources", FILE_PATH) + ".asset");
        AssetDatabase.SaveAssets();
    }
    #endif

    #endregion
}
