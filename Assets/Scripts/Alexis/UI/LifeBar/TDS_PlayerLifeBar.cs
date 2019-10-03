using UnityEngine;
using TMPro;

public class TDS_PlayerLifeBar : TDS_LifeBar 
{
    /* TDS_PlayerLifeBar :
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
    [SerializeField] protected GameObject howToPlayAnchor= null;
    [SerializeField] protected GameObject howToPlayInfo = null;
    [SerializeField] protected Animator howToPlayTextAnimator = null;
    [SerializeField] protected TextMeshProUGUI howToPlayText = null;
    [SerializeField] protected TextMeshProUGUI showHowToPlayText = null;
    [SerializeField] protected TextMeshProUGUI throwObjectText = null;
    [SerializeField] protected TDS_ComboManager comboCounter = null;
    [SerializeField] protected PlayerType playerType = PlayerType.Unknown;

    private bool isController = false;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Hide the player hide to play infos in UI.
    /// </summary>
    public virtual void HideHowToPlayInfo()
    {
        if (howToPlayInfo && howToPlayInfo.activeInHierarchy) TriggerHowToPlayInfo();
    }

    /// <summary>
    /// Show or hide the player hide to play infos in UI.
    /// </summary>
    public virtual void TriggerHowToPlayInfo()
    {
        if (!howToPlayInfo) return;

        bool _areHideInfos = !howToPlayInfo.activeInHierarchy;
        howToPlayInfo.SetActive(_areHideInfos);

        string _controllerInput = isController ? "Controller_Select" : "Keyboard_Tab";
        showHowToPlayText.text = _areHideInfos ? $"Press <sprite name={_controllerInput}> to Hide" : $"<sprite name={ _controllerInput}> How to Play";

        if (howToPlayTextAnimator) howToPlayTextAnimator.SetBool("DisplayHideInfos", _areHideInfos);
    }

    /// <summary>
    /// Show or hide the player throw obejct info in UI.
    /// </summary>
    /// <param name="_doShow">Should the informations be displayed or not.</param>
    public virtual void DisplayThrowObjectInfo(bool _doShow)
    {
        if (!throwObjectText || throwObjectText.gameObject.activeInHierarchy == _doShow) return;

        throwObjectText.gameObject.SetActive(_doShow);
    }

    public override void SetOwner(TDS_Character _owner)
    {
        base.SetOwner(_owner);

        TDS_Player _player = (TDS_Player)_owner;

        if(comboCounter)
        {
            if (_player)
            {
                if((!PhotonNetwork.offlineMode && TDS_GameManager.LocalPlayer == playerType) || PhotonNetwork.offlineMode)
                {
                    ((TDS_Player)_owner).HitBox.OnTouch += comboCounter.IncreaseCombo;
                    comboCounter.ResetComboManager();
                    comboCounter.gameObject.SetActive(true);
                    TDS_UIManager.Instance.ComboManager = comboCounter;
                }
            }
            else
            {
                comboCounter.gameObject.SetActive(false);
            }
        }

        if (!_player.photonView.isMine) return;

        // Set inputs informations
        if (_player)
        {
            if (((_player.Controller == TDS_GameManager.InputsAsset.Controllers[0]) && Input.GetJoystickNames().Length > 0) || (_player.Controller != TDS_GameManager.InputsAsset.Controllers[1]))
            {
                isController = true;
            }

            // Show how to play infos
            TriggerHowToPlayInfo();

            // Set throw infos
            string[] _info = null;

            if (_player.PlayerType != PlayerType.Juggler)
            {
                _info = new string[1];

                if (isController) _info[0] = "Controller_B";
                else _info[0] = "Keyboard_F";

                throwObjectText.text = string.Format(throwObjectText.text, $"<sprite name={_info[0]}>");
            }
            else
            {
                _info = new string[2];

                if (isController)
                {
                    _info[0] = "Controller_LT";
                    _info[1] = "Controller_RT";
                }
                else
                {
                    _info[0] = "Keyboard_Ctrl";
                    _info[1] = "Keyboard_Shift";
                }

                throwObjectText.text = string.Format(throwObjectText.text, $"<sprite name={_info[0]}>", $"<sprite name={_info[1]}>");
            }

            // Set interact button
            _player.InteractionBox.InteractText.text = $"<sprite name={(isController ? "Controller_B" : "Keyboard_F")}>";

            // Set how to play infos
            switch (_player.PlayerType)
                {
                    case PlayerType.BeardLady:
                    // Nothing to change here
                    break;

                    case PlayerType.FatLady:
                    _info = new string[2];

                    if (isController)
                    {
                        _info[0] = "Controller_Y";
                        _info[1] = "Controller_LB";
                    }
                    else
                    {
                        _info[0] = "Keyboard_A";
                        _info[1] = "Keyboard_R";
                    }

                    howToPlayText.text = string.Format(howToPlayText.text, $"<sprite name={_info[0]}>", $"<sprite name={_info[1]}>");
                    break;

                    case PlayerType.FireEater:
                    _info = new string[2];

                    if (isController)
                    {
                        _info[0] = "Controller_X";
                        _info[1] = "Controller_Y";
                    }
                    else
                    {
                        _info[0] = "Keyboard_E";
                        _info[1] = "Keyboard_A";
                    }

                    howToPlayText.text = string.Format(howToPlayText.text, $"<sprite name={_info[0]}>", $"<sprite name={_info[1]}>");
                    break;

                    case PlayerType.Juggler:
                    _info = new string[5];

                    if (isController)
                    {
                        

                        _info[0] = "Controller_B";
                        _info[1] = "Controller_LT";
                        _info[2] = "Controller_Joystick";
                        _info[3] = "Controller_RT";
                        _info[4] = "Controller_DPadX";
                    }
                    else
                    {
                        _info = new string[7];

                        _info[0] = "Keyboard_F";
                        _info[1] = "Keyboard_Ctrl";
                        _info[2] = "Keyboard_J> & <sprite name=Keyboard_L";
                        _info[3] = "Keyboard_Shift";
                        _info[4] = "Keyboard_1> & <sprite name=Keyboard_2";
                    }

                    howToPlayText.text = string.Format(howToPlayText.text, $"<sprite name={_info[0]}>", $"<sprite name={_info[1]}>", $"<sprite name={_info[2]}>", $"<sprite name={_info[3]}>", $"<sprite name={_info[4]}>");
                    break;

                    default:
                    break;
                }
        }

        if (howToPlayAnchor) howToPlayAnchor.SetActive(true);
        if (howToPlayInfo && !howToPlayInfo.activeInHierarchy && (TDS_GameManager.CurrentSceneIndex == 1)) howToPlayInfo.SetActive(true);
    }
    #endregion

    #endregion
}
