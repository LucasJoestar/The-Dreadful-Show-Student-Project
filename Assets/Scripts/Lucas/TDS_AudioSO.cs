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

    #region Characters
    /// <summary>Backing field for <see cref="S_Dodge"/>.</summary>
    [Header("Characters")]
    [SerializeField] private AudioClip[] s_Dodge = new AudioClip[] { };

    /// <summary>
    /// Returns a random dodge sound.
    /// </summary>
    public AudioClip S_Dodge { get { return TDS_SoundManager.GetRandomClip(s_Dodge); } }


    /// <summary>Backing field for <see cref="S_BlowBig"/>.</summary>
    [SerializeField] private AudioClip[] s_BlowBig = new AudioClip[] { };

    /// <summary>
    /// Returns a random big hit blows.
    /// </summary>
    public AudioClip S_BlowBig { get { return TDS_SoundManager.GetRandomClip(s_BlowBig); } }


    /// <summary>Backing field for <see cref="S_BlowSmall"/>.</summary>
    [SerializeField] private AudioClip[] s_BlowSmall = new AudioClip[] { };

    /// <summary>
    /// Returns a random small hit blows.
    /// </summary>
    public AudioClip S_BlowSmall { get { return TDS_SoundManager.GetRandomClip(s_BlowSmall); } }


    /// <summary>Backing field for <see cref="S_Parry"/>.</summary>
    [SerializeField] private AudioClip[] s_Parry = new AudioClip[] { };

    /// <summary>
    /// Returns a random parry sound.
    /// </summary>
    public AudioClip S_Parry { get { return TDS_SoundManager.GetRandomClip(s_Parry); } }


    /// <summary>Backing field for <see cref="J_Jump"/>.</summary>
    [SerializeField] private AudioClip[] s_Jump = new AudioClip[] { };

    /// <summary>
    /// Returns a random jumping sound.
    /// </summary>
    public AudioClip J_Jump { get { return TDS_SoundManager.GetRandomClip(s_Jump); } }


    /// <summary>Backing field for <see cref="S_Land"/>.</summary>
    [SerializeField] private AudioClip[] s_Land = new AudioClip[] { };

    /// <summary>
    /// Returns a random landing sound.
    /// </summary>
    public AudioClip S_Land { get { return TDS_SoundManager.GetRandomClip(s_Land); } }


    /// <summary>Backing field for <see cref="s_Hit"/>.</summary>
    [SerializeField] private AudioClip[] s_hit = new AudioClip[] { };

    /// <summary>
    /// Returns a random hitting stroke sound.
    /// </summary>
    public AudioClip s_Hit { get { return TDS_SoundManager.GetRandomClip(s_hit); } }


    /// <summary>Backing field for <see cref="S_BodyFall"/>.</summary>
    [SerializeField] private AudioClip s_BodyFall = null;

    /// <summary>
    /// Sound when a body falls down.
    /// </summary>
    public AudioClip S_BodyFall { get { return s_BodyFall; } }
    #endregion

    #region Objects
    /// <summary>
    /// Sound whe picking an object.
    /// </summary>
    [Header("Objects")]
    [SerializeField] private AudioClip s_Pickup = null;

    /// <summary>
    /// Sound whe throwing an object.
    /// </summary>
    [SerializeField] private AudioClip s_Throw = null;

    /// <summary>
    /// Sound whe dropping an object.
    /// </summary>
    [SerializeField] private AudioClip s_Drop = null;


    /// <summary>
    /// Sound when some supplies land on ground.
    /// </summary>
    [SerializeField] private AudioClip s_Supplies = null;

    /// <summary>
    /// Sound of a crate being destroyed
    /// </summary>
    [SerializeField] private AudioClip s_CrateDestroyed = null;
    #endregion

    #region Feedbacks
    /// <summary>Backing field for <see cref="S_approachDeath"/>.</summary>
    [Header("Feedbacks")]
    [SerializeField] private AudioClip s_ApproachDeath = null;

    /// <summary>
    /// Sound indicating to the players that their health is approaching zero.
    /// </summary>
    public AudioClip S_approachDeath { get { return s_ApproachDeath; } }

    /// <summary>Backing field for <see cref="S_MagicPoof"/>.</summary>
    [SerializeField] private AudioClip s_MagicPoof = null;

    /// <summary>
    /// Sound when a magic poof pops on the screen.
    /// </summary>
    public AudioClip S_MagicPoof { get { return s_MagicPoof; } }

    /// <summary>Backing field for <see cref="S_Checkpoint"/>.</summary>
    [SerializeField] private AudioClip s_Checkpoint = null;

    /// <summary>
    /// Sound when a checkpoint is activated.
    /// </summary>
    public AudioClip S_Checkpoint { get { return s_Checkpoint; } }

    /// <summary>Backing field for <see cref="S_Burning"/>.</summary>
    [SerializeField] private AudioClip s_Burning = null;

    /// <summary>
    /// Sound when something is burning.
    /// </summary>
    public AudioClip S_Burning { get { return s_Burning; } }
    #endregion

    #region UI
    /// <summary>Backing field for <see cref="S_UI_Confirm"/>.</summary>
    [Header("UI")]
    [SerializeField] private AudioClip s_UI_Confirm = null;

    /// <summary>
    /// Sound for confirming in menu.
    /// </summary>
    public AudioClip S_UI_Confirm { get { return s_UI_Confirm; } }

    /// <summary>Backing field for <see cref="S_UI_BigConfirm"/>.</summary>
    [SerializeField] private AudioClip s_UI_BigConfirm = null;

    /// <summary>
    /// Sound for big confirms in menu.
    /// </summary>
    public AudioClip S_UI_BigConfirm { get { return s_UI_BigConfirm; } }

    /// <summary>Backing field for <see cref="S_UI_Over"/>.</summary>
    [SerializeField] private AudioClip s_UI_Over = null;

    /// <summary>
    /// Sound for "over" sound in menu.
    /// </summary>
    public AudioClip S_UI_Over { get { return s_UI_Over; } }

    /// <summary>Backing field for <see cref="S_UI_Ready"/>.</summary>
    [SerializeField] private AudioClip s_UI_Ready = null;

    /// <summary>
    /// Sound for player ready.
    /// </summary>
    public AudioClip S_UI_Ready { get { return s_UI_Ready; } }

    /// <summary>Backing field for <see cref="S_CurtainsIn"/>.</summary>
    [SerializeField] private AudioClip s_CurtainsIn = null;

    /// <summary>
    /// Sound when curtains riding in.
    /// </summary>
    public AudioClip S_CurtainsIn { get { return s_CurtainsIn; } }

    /// <summary>Backing field for <see cref="S_CurtainsOut"/>.</summary>
    [SerializeField] private AudioClip s_CurtainsOut = null;

    /// <summary>
    /// Sound for curtains riding out.
    /// </summary>
    public AudioClip S_CurtainsOut { get { return s_CurtainsOut; } }
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
