using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using System.Linq; 

public class TDS_OptionManager : MonoBehaviour 
{
    /* TDS_OptionManager :
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
    [Header("Audio")]
    [SerializeField] private AudioMixer mixerMusic = null;
    [SerializeField] private AudioMixer mixerVoices = null;
    [SerializeField] private AudioMixer mixerFX = null;

    [SerializeField] private Slider sliderMusic = null;
    [SerializeField] private Slider sliderVoices = null;
    [SerializeField] private Slider sliderFX = null;

    [Header("Graphics")]
    [SerializeField] private TMP_Text textResolution = null;
    [SerializeField] private Toggle toggleFullScreen = null;
    [SerializeField] private Button buttonPreviousResolution = null;
    [SerializeField] private Button buttonNextResolution = null;
    private int selectedResolutionIndex = 0;

    [Header("Subtitles")]
    [SerializeField] private Button frenchButton = null;
    [SerializeField] private Button englishButton = null;

    [Header("Buttons")]
    [SerializeField] private Button applyButton = null;
    [SerializeField] private Button backButton = null;
    #endregion

    #region Methods

    #region Original Methods

    #region Set Volume
    private void SetMusicVolume(float _value)
    {
        if (!mixerMusic) return;
        mixerMusic.SetFloat("MusicVolume", _value); 
    }
    private void SetVoicesVolume(float _value)
    {
        if (!mixerVoices) return;
        mixerVoices.SetFloat("VoicesVolume", _value);
    }
    private void SetFXVolume(float _value)
    {
        if (!mixerFX) return;
        mixerFX.SetFloat("FXVolume", _value);
    }
    #endregion

    #region Resolution
    private void SelectNextResolution()
    {
        selectedResolutionIndex++;
        if (selectedResolutionIndex >= Screen.resolutions.Length) selectedResolutionIndex = 0; 
        if(textResolution) textResolution.text = $"{Screen.resolutions[selectedResolutionIndex].width} x {Screen.resolutions[selectedResolutionIndex].height}";
    }

    private void SelectPreviousResolution()
    {
        selectedResolutionIndex--;
        if (selectedResolutionIndex < 0) selectedResolutionIndex = Screen.resolutions.Length - 1;
        if (textResolution) textResolution.text = $"{Screen.resolutions[selectedResolutionIndex].width} x {Screen.resolutions[selectedResolutionIndex].height}";
    }

    private void ApplyNewResolution()
    {
        Resolution _newResolution = Screen.resolutions[selectedResolutionIndex];
        Screen.SetResolution(_newResolution.width, _newResolution.height, toggleFullScreen ? toggleFullScreen.isOn : true);
        TDS_GameManager.SetResolution(_newResolution);
    }
    #endregion

    #region Reset
    public void ResetDisplayedSettings()
    {
        float _value = 0;
        if(mixerMusic)
        {
            mixerMusic.GetFloat("MusicVolume", out _value);
            if (sliderMusic) sliderMusic.value = _value;
        }

        if (mixerVoices)
        {
            mixerVoices.GetFloat("VoicesVolume", out _value);
            if (sliderVoices) sliderVoices.value = _value;
        }

        if(mixerFX)
        {
            mixerFX.GetFloat("FXVolume", out _value);
            if (sliderFX) sliderFX.value = _value;
        }


        if (textResolution) textResolution.text = $"{Screen.currentResolution.width} x {Screen.currentResolution.height}";
        if (toggleFullScreen) toggleFullScreen.isOn = Screen.fullScreen;
        selectedResolutionIndex = Screen.resolutions.ToList().IndexOf(Screen.currentResolution);

        buttonPreviousResolution.Select(); 
    }
    #endregion

    /// <summary>
    /// Set the localisation and the state for the buttons
    /// </summary>
    /// <param name="_isLocalisationEnglish"></param>
    public void SetLocalisation(bool _isLocalisationEnglish)
    {
        if (_isLocalisationEnglish == TDS_GameManager.LocalisationIsEnglish) return;
        TDS_GameManager.LocalisationIsEnglish = _isLocalisationEnglish; 
        englishButton.GetComponent<Animator>().SetBool("IsSelected", _isLocalisationEnglish);
        frenchButton.GetComponent<Animator>().SetBool("IsSelected", !_isLocalisationEnglish);
    }

    /// <summary>
    /// Reset the button animator
    /// </summary>
    private void ResetLocalisation()
    {
        if (frenchButton && englishButton)
        {
            if (TDS_GameManager.LocalisationIsEnglish)
            {
                frenchButton.GetComponent<Animator>().SetBool("IsSelected", false);
                englishButton.GetComponent<Animator>().SetBool("IsSelected", true);
            }
            else
            {
                frenchButton.GetComponent<Animator>().SetBool("IsSelected", true);
                englishButton.GetComponent<Animator>().SetBool("IsSelected", false);
            }
        }
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        ResetDisplayedSettings();
        if (sliderMusic) sliderMusic.onValueChanged.AddListener(SetMusicVolume);
        if (sliderVoices) sliderVoices.onValueChanged.AddListener(SetVoicesVolume);
        if (sliderFX) sliderFX.onValueChanged.AddListener(SetFXVolume);

        if (buttonPreviousResolution) buttonNextResolution.onClick.AddListener(SelectNextResolution);
        if (buttonNextResolution) buttonPreviousResolution.onClick.AddListener(SelectPreviousResolution);

        if (applyButton) applyButton.onClick.AddListener(ApplyNewResolution);
        if (backButton) backButton.onClick.AddListener(() => TDS_UIManager.Instance?.DisplayOptions(false)); 
    }

    private void OnEnable()
    {
        ResetLocalisation();
    }
    #endregion

    #endregion
}
