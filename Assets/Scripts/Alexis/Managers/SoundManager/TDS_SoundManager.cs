using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;

public class TDS_SoundManager : MonoBehaviour
{
    /* TDS_SoundManager :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	[Holds all sounds of the game and play them]
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
	 *	Date :			[05/07/2019]
	 *	Author :		[Thiebaut Alexis]
	 *
	 *	Changes :
	 *
	 *	[Initialisation of the class]
     *	    - Create the audioclip database from a Asset Bundle
     *	    - Get the audioclip by name
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties

    #region Constants 
    public const string MUSIC_GROUP_NAME = "Music";
    public const string VOICES_GROUP_NAME = "Voices";
    public const string FX_GROUP_NAME = "FX";
    #endregion

    #region Audio Sources & Mixer
    /// <summary>
    /// Audio mixer to play sounds.
    /// </summary>
    [SerializeField] private AudioMixer audioMixer = null;


    /// <summary>
    /// Audio source used to play music in the game.
    /// </summary>
    [SerializeField] private AudioSource musicSource = null;

    /// <summary>
    /// Audio source used to play narrator quotes.
    /// </summary>
    [SerializeField] private AudioSource narratorSource = null;
    #endregion

    #region AudioClip
    /// <summary>
    /// Music of the title screen.
    /// </summary>
    [Header("Musics")]
    [SerializeField] private AudioClip mTitleScreen = null;

    /// <summary>
    /// Music of the intro cutscene.
    /// </summary>
    [SerializeField] private AudioClip mIntro = null;

    /// <summary>
    /// Music of the outro cutscene.
    /// </summary>
    [SerializeField] private AudioClip mOutro = null;

    /// <summary>
    /// Intro for the fight music.
    /// </summary>
    [SerializeField] private AudioClip mFightIntro = null;

    /// <summary>
    /// Loop of the fight music.
    /// </summary>
    [SerializeField] private AudioClip mFightLoop = null;

    /// <summary>
    /// Intro of the main game theme music.
    /// </summary>
    [SerializeField] private AudioClip mThemeIntro = null;

    /// <summary>
    /// Loop of the main game theme music.
    /// </summary>
    [SerializeField] private AudioClip mThemeLoop = null;
    #endregion

    #region Coroutines
    /// <summary>
    /// Current coroutine used to play a new music.
    /// </summary>
    private Coroutine playMusicCoroutine = null;
    #endregion

    #region Memory Variables
    /// <summary>
    /// Volume used to play music.
    /// </summary>
    private float musicVolume = 1;
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this script.
    /// </summary>
    public static TDS_SoundManager Instance = null;
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Get the mixer group of the groupName
    /// USE THIS METHOD WITH THE CONST MUSIC_GROUP_NAME. VOICES_GROUP_NAME AND FX_GROUP_NAME
    /// </summary>
    /// <param name="_groupName"></param>
    /// <returns></returns>
    public AudioMixerGroup GetMixerGroupOfName(string _groupName)
    {
        if (!audioMixer) return null;
        return audioMixer.FindMatchingGroups(_groupName).FirstOrDefault(); 
    }

    /// <summary>
    /// Plays a new music.
    /// </summary>
    /// <param name="_music">Music to play.</param>
    /// <param name="_fadeDuration">Duration during which the previous music fades out before the new one starts.</param>
    public void PlayMusic(Music _music, float _fadeDuration)
    {
        if (playMusicCoroutine != null)
        {
            StopCoroutine(playMusicCoroutine);
        }
        else musicVolume = musicSource.volume;

        playMusicCoroutine = StartCoroutine(PlayMusicCoroutine(_music, _fadeDuration));
    }

    /// <summary>
    /// Plays a new music.
    /// </summary>
    /// <param name="_music">Music to play.</param>
    /// <param name="_fadeDuration">Duration during which the previous music fades out before the new one starts.</param>
    /// <returns></returns>
    public IEnumerator PlayMusicCoroutine(Music _music, float _fadeDuration)
    {
        float _timer = _fadeDuration;

        // Fade previous music slowly
        while (_timer > 0)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, 0, 1 - (_timer / _fadeDuration));

            yield return null;
            _timer -= Time.deltaTime;
        }

        // Set back volume
        musicSource.volume = musicVolume;

        // Get the music to play and executes actions depending on it
        switch (_music)
        {
            case Music.TitleScreen:
                musicSource.clip = mTitleScreen;
                break;

            case Music.Intro:
                musicSource.clip = mIntro;
                break;

            case Music.Outro:
                musicSource.clip = mOutro;
                break;

            case Music.GameTheme:
                musicSource.clip = mThemeIntro;
                musicSource.Play();

                yield return new WaitForSeconds(mThemeIntro.length);

                musicSource.clip = mThemeLoop;
                break;

            case Music.Fight:
                musicSource.clip = mFightIntro;
                musicSource.Play();

                yield return new WaitForSeconds(mFightIntro.length);

                musicSource.clip = mFightLoop;
                break;

            default:
                break;
        }

        // Plays the music
        musicSource.Play();
    }
    
    /// <summary>
    /// Plays a narrator quote audio track.
    /// </summary>
    /// <param name="_audioTrack">Audio track to play.</param>
    public void PlayNarratorQuote(AudioClip _audioTrack)
    {
        narratorSource.clip = _audioTrack;
        narratorSource.Play();
    }

    /// <summary>
    /// Play a sound at a position
    /// </summary>
    /// <param name="_source">Source that will play the sound</param>
    /// <param name="_playedClip">Clip taht will be played</param>
    /// <param name="_position">Position where the clip will be played</param>
    /// <param name="_groupName">Group of the audioMixer /!\ USE THE CONST IN THE SOUNDMANAGER TO GET THE RIGHT NAME OF THE GROUP /!\ </param>
    /// <param name="_isLooping">Does the sound has to loop</param>
    public void PlaySoundAtPosition(AudioSource _source, AudioClip _playedClip, Vector3 _position, string _groupName = "Master", bool _isLooping = false, float _volume = 1f)
    {
        _source.transform.position = _position;
        _source.outputAudioMixerGroup = GetMixerGroupOfName(_groupName);
        if(_isLooping)
        {
            _source.loop = true;
            _source.volume = _volume;
            _source.clip = _playedClip;
            _source.Play();
            return; 
        }
        _source.loop = false;
        _source.PlayOneShot(_playedClip, _volume); 
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        if(!Instance)
        {
            Instance = this; 
        }
        else
        {
            Destroy(this);
            return; 
        }
    }
    #endregion

    #endregion
}
