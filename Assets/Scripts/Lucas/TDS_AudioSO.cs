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
    /// <summary>Backing field for <see cref="M_TitleScreen"/>.</summary>
    [Header("Musics")]
    [SerializeField] private AudioClip m_TitleScreen = null;

    /// <summary>
    /// Music of the title screen.
    /// </summary>
    public AudioClip M_TitleScreen { get { return m_TitleScreen; } }


    /// <summary>Backing field for <see cref="M_Intro"/>.</summary>
    [SerializeField] private AudioClip m_Intro = null;

    /// <summary>
    /// Music of the intro cutscene.
    /// </summary>
    public AudioClip M_Intro { get { return m_Intro; } }


    /// <summary>Backing field for <see cref="M_Outro"/>.</summary>
    [SerializeField] private AudioClip m_Outro = null;

    /// <summary>
    /// Music of the outro cutscene.
    /// </summary>
    public AudioClip M_Outro { get { return m_Outro; } }


    /// <summary>Backing field for <see cref="M_FightIntro"/>.</summary>
    [SerializeField] private AudioClip m_FightIntro = null;

    /// <summary>
    /// Intro for the fight music.
    /// </summary>
    public AudioClip M_FightIntro { get { return m_FightIntro; } }


    /// <summary>Backing field for <see cref="M_FightLoop"/>.</summary>
    [SerializeField] private AudioClip m_FightLoop = null;

    /// <summary>
    /// Loop of the fight music.
    /// </summary>
    public AudioClip M_FightLoop { get { return m_FightLoop; } }


    /// <summary>Backing field for <see cref="M_ThemeIntro"/>.</summary>
    [SerializeField] private AudioClip m_ThemeIntro = null;

    /// <summary>
    /// Intro of the main game theme music.
    /// </summary>
    public AudioClip M_ThemeIntro { get { return m_ThemeIntro; } }


    /// <summary>Backing field for <see cref="M_ThemeLoop"/>.</summary>
    [SerializeField] private AudioClip m_ThemeLoop = null;

    /// <summary>
    /// Loop of the main game theme music.
    /// </summary>
    public AudioClip M_ThemeLoop { get { return m_ThemeLoop; } }
    #endregion

    #region Feedbacks
    /// <summary>Backing field for <see cref="S_approachDeath"/>.</summary>
    [Header("Feedbacks")]
    [SerializeField] private AudioClip s_ApproachDeath = null;

    /// <summary>
    /// Sound indicating to the players that their health is approaching zero.
    /// </summary>
    public AudioClip S_approachDeath { get { return s_ApproachDeath; } }
    #endregion

    #endregion

    #endregion

    #region Methods

    #region Editor
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

    #endregion
}
