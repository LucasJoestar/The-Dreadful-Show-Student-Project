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

    /// <summary>
    /// Audio source used to play all kind of UI stuff.
    /// </summary>
    [SerializeField] private AudioSource uiSource = null;

    /// <summary>
    /// Audio source used to play all kind of effects stuff.
    /// </summary>
    [SerializeField] private AudioSource effectsSource = null;
    #endregion

    #region Coroutines
    /// <summary>
    /// Current coroutine used to play a new music.
    /// </summary>
    private Coroutine playMusicCoroutine = null;

    /// <summary>
    /// Current coroutine used to unpause the game.
    /// </summary>
    private Coroutine unpauseCoroutine = null;
    #endregion

    #region Memory Variables
    /// <summary>
    /// Volume used to play music.
    /// </summary>
    private float musicVolume = 1;

    /// <summary>
    /// Current music playing.
    /// </summary>
    private Music currentMusic = 0;

    /// <summary>
    /// Music playing before setting pause.
    /// </summary>
    private AudioClip musicBeforePause = null;

    /// <summary>
    /// Time at which was playing the music before setting pause.
    /// </summary>
    private float musicBeforePauseTime = 0;
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this script.
    /// </summary>
    public static TDS_SoundManager Instance = null;
    #endregion

    #region Common Clips
    [SerializeField] private AudioClip[] hitClips = new AudioClip[] { };
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
    /// Returns a random audio clip from an array.
    /// </summary>
    /// <param name="_clips">Clips to change one from.</param>
    /// <returns>Returns a random audio clip.</returns>
    public static AudioClip GetRandomClip(AudioClip[] _clips)
    {
        return _clips[Random.Range(0, _clips.Length)];
    }

    #region Music
    /// <summary>
    /// Plays a new music.
    /// </summary>
    /// <param name="_music">Music to play.</param>
    /// <param name="_fadeDuration">Duration during which the previous music fades out before the new one starts.</param>
    public void PlayMusic(Music _music, float _fadeDuration)
    {
        if (_music == currentMusic) return;

        if (playMusicCoroutine != null)
        {
            StopCoroutine(playMusicCoroutine);
        }
        else musicVolume = musicSource.volume;

        currentMusic = _music;
        playMusicCoroutine = StartCoroutine(PlayMusicCoroutine(_music, _fadeDuration));
    }

    /// <summary>
    /// Plays a new music.
    /// </summary>
    /// <param name="_music">Music to play.</param>
    /// <param name="_fadeDuration">Duration during which the previous music fades out before the new one starts.</param>
    /// <returns></returns>
    private IEnumerator PlayMusicCoroutine(Music _music, float _fadeDuration)
    {
        float _timer = _fadeDuration;

        // Fade previous music slowly
        while (_timer > 0)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, 0f, 1f - (_timer / _fadeDuration));

            yield return null;
            _timer -= Time.deltaTime;
        }
        // Set back volume
        musicSource.volume = musicVolume;
        musicSource.time = 0;

        // Get the music to play and executes actions depending on it
        switch (_music)
        {
            case Music.TitleScreen:
                musicSource.clip = TDS_GameManager.AudioAsset.M_TitleScreen;
                break;

            case Music.Intro:
                musicSource.clip = TDS_GameManager.AudioAsset.M_Intro;
                break;

            case Music.Outro:
                musicSource.clip = TDS_GameManager.AudioAsset.M_Outro;
                break;

            case Music.GameTheme:
                musicSource.clip = TDS_GameManager.AudioAsset.M_ThemeIntro;
                musicSource.Play();

                yield return new WaitForSeconds(TDS_GameManager.AudioAsset.M_ThemeIntro.length);

                musicSource.clip = TDS_GameManager.AudioAsset.M_ThemeLoop;
                break;

            case Music.Fight:
                musicSource.clip = TDS_GameManager.AudioAsset.M_FightIntro;
                musicSource.Play();

                yield return new WaitForSeconds(TDS_GameManager.AudioAsset.M_FightIntro.length);

                musicSource.clip = TDS_GameManager.AudioAsset.M_FightLoop;
                break;

            default:
                musicSource.Stop();
                yield break;
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
    /// Stops the current music.
    /// </summary>
    /// <param name="_fadeDuration">Time to fade current music down.</param>
    public void StopMusic(float _fadeDuration = 0) => PlayMusic((Music)(-1), 1.5f);
    #endregion

    #region UI
    /// <summary>
    /// (Un)pause the game.
    /// </summary>
    /// <param name="_doPause">Should the game be pause or not.</param>
    public void Pause(bool _doPause)
    {
        AudioListener.pause = _doPause;

        if (_doPause)
        {
            musicBeforePause = musicSource.clip;
            musicBeforePauseTime = musicSource.time;

            if (unpauseCoroutine != null)
            {
                StopCoroutine(unpauseCoroutine);
                unpauseCoroutine = null;
            }

            musicSource.clip = TDS_GameManager.AudioAsset.M_TitleScreen;
            musicSource.time = 0;
            musicSource.Play();
        }
        else
        {
            unpauseCoroutine = StartCoroutine(Unpause());
        }
    }

    /// <summary>
    /// Unpauses the game with music back.
    /// </summary>
    /// <returns></returns>
    private IEnumerator Unpause()
    {
        float _timer = 0;
        musicSource.volume = 0;
        musicSource.clip = musicBeforePause;
        musicSource.time = musicBeforePauseTime;
        musicSource.Play();

        // Fade previous music slowly
        while (_timer < 1)
        {
            musicSource.volume = Mathf.Lerp(musicSource.volume, musicVolume, _timer);

            yield return null;
            _timer += Time.deltaTime;
        }
    }


    /// <summary>
    /// Plays a UI sound.
    /// </summary>
    /// <param name="_clip"></param>
    public void PlayUISound(AudioClip _clip)
    {
        if (!uiSource) return;
        uiSource.PlayOneShot(_clip);
    }

    /// <summary>
    /// Plays a sound for confirm in UI.
    /// </summary>
    public void PlayUIConfirm() => PlayUISound(TDS_GameManager.AudioAsset.S_UI_Confirm);

    /// <summary>
    /// Plays a sound for a big confirm in UI.
    /// </summary>
    public void PlayUIBigConfirm() => PlayUISound(TDS_GameManager.AudioAsset.S_UI_BigConfirm);

    /// <summary>
    /// Plays a sound for "over" effect in UI.
    /// </summary>
    public void PlayUIOver() => PlayUISound(TDS_GameManager.AudioAsset.S_UI_Over);

    /// <summary>
    /// Plays a sound when player is ready.
    /// </summary>
    public void PlayUIReady() => PlayUISound(TDS_GameManager.AudioAsset.S_UI_Ready);

    /// <summary>
    /// Plays sound for curtains riding in.
    /// </summary>
    public void PlayCurtainsIn() => PlayUISound(TDS_GameManager.AudioAsset.S_CurtainsIn);

    /// <summary>
    /// Plays sound for curtains riding out.
    /// </summary>
    public void PlayCurtainsOut() => PlayUISound(TDS_GameManager.AudioAsset.S_CurtainsOut);
    #endregion

    #region Feedback
    /// <summary>
    /// Plays sound indicating the player approches death...
    /// </summary>
    public void PlayApproachDeath()
    {
        if (!effectsSource) return;
        effectsSource.PlayOneShot(TDS_GameManager.AudioAsset.S_approachDeath);
    }

    public void PlayHitSound(AudioSource _source)
    {
        return; 
        if (!_source || hitClips.Length == 0) return;
        _source.outputAudioMixerGroup = GetMixerGroupOfName(FX_GROUP_NAME);
        _source.loop = false;
        AudioClip _playedClip = hitClips[(int)Random.Range((int)0, (int)hitClips.Length)];
        if (_playedClip == null) return; 
        _source.PlayOneShot(_playedClip); 
    }
    #endregion

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
        if (_source == null || _playedClip == null) return; 
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

        musicSource.ignoreListenerPause = true;
        uiSource.ignoreListenerPause = true;
    }
    #endregion

    #endregion
}
