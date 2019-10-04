using Photon;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TDS_CreditsManager : PunBehaviour
{
    #region Fields / Properties
    [Header("Victims")]
    [SerializeField] private TextMeshProUGUI punksKO = null;
    [SerializeField] private TextMeshProUGUI punksDamages = null;

    [Space]
    [SerializeField] private TextMeshProUGUI mimesKO = null;
    [SerializeField] private TextMeshProUGUI mimesDamages = null;

    [Space]
    [SerializeField] private TextMeshProUGUI mightyMenKO = null;
    [SerializeField] private TextMeshProUGUI mightyMenDamages = null;

    [Space]
    [SerializeField] private TextMeshProUGUI fakirsKO = null;
    [SerializeField] private TextMeshProUGUI fakirsDamages = null;

    [Space]
    [SerializeField] private TextMeshProUGUI acrobatsKO = null;
    [SerializeField] private TextMeshProUGUI acrobatsDamages = null;

    [Space]
    [SerializeField] private TextMeshProUGUI punkBossKO = null;
    [SerializeField] private TextMeshProUGUI punkBossDamages = null;

    [Space]
    [SerializeField] private TextMeshProUGUI siamesesKO = null;
    [SerializeField] private TextMeshProUGUI siamesesDamages = null;

    [Space]
    [SerializeField] private TextMeshProUGUI mrLoyalKO = null;
    [SerializeField] private TextMeshProUGUI mrLoyalDamages = null;


    [Header("Players"), Space]
    [SerializeField] private TextMeshProUGUI beardLadyKO = null;
    [SerializeField] private TextMeshProUGUI beardLadyDamages = null;
    [SerializeField] private TextMeshProUGUI beardLadyNemesis = null;
    [SerializeField] private Animator beardLadyNemesisIcon = null;

    [Space]
    [SerializeField] private TextMeshProUGUI fireEaterKO = null;
    [SerializeField] private TextMeshProUGUI fireEaterDamages = null;
    [SerializeField] private TextMeshProUGUI fireEaterNemesis = null;
    [SerializeField] private Animator fireEaterNemesisIcon = null;

    [Space]
    [SerializeField] private TextMeshProUGUI grandeDameKO = null;
    [SerializeField] private TextMeshProUGUI grandeDameDamages = null;
    [SerializeField] private TextMeshProUGUI grandeDameNemesis = null;
    [SerializeField] private Animator grandeDameNemesisIcon = null;

    [Space]
    [SerializeField] private TextMeshProUGUI jugglerKO = null;
    [SerializeField] private TextMeshProUGUI jugglerDamages = null;
    [SerializeField] private TextMeshProUGUI jugglerNemesis = null;
    [SerializeField] private Animator jugglerNemesisIcon = null;

    [Header("Icons"), Space]
    [SerializeField] private RuntimeAnimatorController punkIcon = null;
    [SerializeField] private RuntimeAnimatorController mimeIcon = null;
    [SerializeField] private RuntimeAnimatorController mightyManIcon = null;
    [SerializeField] private RuntimeAnimatorController fakirIcon = null;
    [SerializeField] private RuntimeAnimatorController acrobatIcon = null;
    [SerializeField] private RuntimeAnimatorController punkBossIcon = null;
    [SerializeField] private RuntimeAnimatorController siamesesIcon = null;
    [SerializeField] private RuntimeAnimatorController mrLoyalIcon = null;
    #endregion

    #region Methods
    /// <summary>
    /// Set the game credits based on players score.
    /// </summary>
    public void SetCredits()
    {
        TDS_PlayerScore _playerScore = null;
        PlayerType _type = 0;
        Dictionary<PlayerType, string[]> _playersScores = new Dictionary<PlayerType, string[]>();

        // Get all players score
        for (int i = 1; i < 5; i++)
        {
            _type = (PlayerType)(i);
            _playerScore = TDS_GameManager.PlayersInfo.FirstOrDefault(p => p.PlayerType == _type)?.PlayerScore;

            _playersScores.Add(_type, new string[19]);

            // If no player of the type, set all
            if (_playerScore == null)
            {
                for (int _j = 0; _j < 19; _j++)
                {
                    _playersScores[_type][_j] = "-";
                }

                string _nemesisInfo = "\n";

                switch (_type)
                {
                    case PlayerType.BeardLady:
                        _nemesisInfo += "Was lost in the corridors.";
                        break;

                    case PlayerType.FatLady:
                        _nemesisInfo += "Flight over the seven kingdoms.";
                        break;

                    case PlayerType.FireEater:
                        _nemesisInfo += "Too drunk to fight.";
                        break;

                    case PlayerType.Juggler:
                        _nemesisInfo += "Stayed under the table.";
                        break;
                }
                _playersScores[_type][18] += _nemesisInfo;
                continue;
            }

            // Get enemies related score
            _playersScores[_type][0] = _playerScore.KnockoutEnemiesAmount["Punk"].ToString();
            _playersScores[_type][1] = _playerScore.InflictedDmgsToEnemies["Punk"].ToString();

            _playersScores[_type][2] = _playerScore.KnockoutEnemiesAmount["Mime"].ToString();
            _playersScores[_type][3] = _playerScore.InflictedDmgsToEnemies["Mime"].ToString();

            _playersScores[_type][4] = _playerScore.KnockoutEnemiesAmount["Mighty Man"].ToString();
            _playersScores[_type][5] = _playerScore.InflictedDmgsToEnemies["Mighty Man"].ToString();

            _playersScores[_type][6] = _playerScore.KnockoutEnemiesAmount["Fakir"].ToString();
            _playersScores[_type][7] = _playerScore.InflictedDmgsToEnemies["Fakir"].ToString();

            _playersScores[_type][8] = _playerScore.KnockoutEnemiesAmount["Acrobat"].ToString();
            _playersScores[_type][9] = _playerScore.InflictedDmgsToEnemies["Acrobat"].ToString();

            _playersScores[_type][10] = _playerScore.KnockoutEnemiesAmount["Punk Boss"].ToString();
            _playersScores[_type][11] = _playerScore.InflictedDmgsToEnemies["Punk Boss"].ToString();

            _playersScores[_type][12] = _playerScore.KnockoutEnemiesAmount["Siamese"].ToString();
            _playersScores[_type][13] = _playerScore.InflictedDmgsToEnemies["Siamese"].ToString();

            _playersScores[_type][14] = _playerScore.KnockoutEnemiesAmount["Mr Loyal"].ToString();
            _playersScores[_type][15] = _playerScore.InflictedDmgsToEnemies["Mr Loyal"].ToString();

            // Get player related score
            _playersScores[_type][16] = _playerScore.KnockoutAmountFromEnemies.Sum(e => e.Value).ToString();
            _playersScores[_type][17] = _playerScore.SuffuredDmgsFromEnemies.Sum(e => e.Value).ToString();
            _playersScores[_type][18] = _playerScore.SuffuredDmgsFromEnemies.Aggregate((a, b) => a.Value > b.Value ? a : b).Key;

            // If player took no damages, show special content
            if (_playerScore.SuffuredDmgsFromEnemies[_playersScores[_type][18]] == 0)
            {
                string _nemesisInfo = "-\n";
                switch (_type)
                {
                    case PlayerType.BeardLady:
                        _nemesisInfo += "Invincible from beard to feets.";
                        break;

                    case PlayerType.FatLady:
                        _nemesisInfo += "Nobody can touch her grace.";
                        break;

                    case PlayerType.FireEater:
                        _nemesisInfo += "Scared everyone with his fire.";
                        break;

                    case PlayerType.Juggler:
                        _nemesisInfo += "Surprisingly dexterous in fights.";
                        break;
                }
                _playersScores[_type][18] += _nemesisInfo;
            }
        }

        // Set enemies-related players score
        SetPlayersScore(new TextMeshProUGUI[] { punksKO, punksDamages, mimesKO, mimesDamages, mightyMenKO, mightyMenDamages, fakirsKO, fakirsDamages, acrobatsKO, acrobatsDamages, punkBossKO, punkBossDamages, siamesesKO, siamesesDamages, mrLoyalKO, mrLoyalDamages }, _playersScores);

        // Set players scores
        beardLadyKO.text = string.Format(beardLadyKO.text, _playersScores[PlayerType.BeardLady][16]);
        beardLadyDamages.text = string.Format(beardLadyDamages.text, _playersScores[PlayerType.BeardLady][17]);
        beardLadyNemesis.text = string.Format(beardLadyNemesis.text, _playersScores[PlayerType.BeardLady][18]);
        SetNemesisIcon(beardLadyNemesisIcon, _playersScores[PlayerType.BeardLady][18]);

        fireEaterKO.text = string.Format(fireEaterKO.text, _playersScores[PlayerType.FireEater][16]);
        fireEaterDamages.text = string.Format(fireEaterDamages.text, _playersScores[PlayerType.FireEater][17]);
        fireEaterNemesis.text = string.Format(fireEaterNemesis.text, _playersScores[PlayerType.FireEater][18]);
        SetNemesisIcon(fireEaterNemesisIcon, _playersScores[PlayerType.FireEater][18]);

        grandeDameKO.text = string.Format(grandeDameKO.text,  _playersScores[PlayerType.FatLady][16]);
        grandeDameDamages.text = string.Format(grandeDameDamages.text, _playersScores[PlayerType.FatLady][17]);
        grandeDameNemesis.text = string.Format(grandeDameNemesis.text, _playersScores[PlayerType.FatLady][18]);
        SetNemesisIcon(grandeDameNemesisIcon, _playersScores[PlayerType.FatLady][18]);

        jugglerKO.text = string.Format(jugglerKO.text, _playersScores[PlayerType.Juggler][16]);
        jugglerDamages.text = string.Format(jugglerDamages.text, _playersScores[PlayerType.Juggler][17]);
        jugglerNemesis.text = string.Format(jugglerNemesis.text, _playersScores[PlayerType.Juggler][18]);
        SetNemesisIcon(jugglerNemesisIcon, _playersScores[PlayerType.Juggler][18]);
    }
    
    /// <summary>
    /// Set players score in texts for UI.
    /// </summary>
    /// <param name="_texts">Texts to set.</param>
    /// <param name="_scores">Playres score.</param>
    private void SetPlayersScore(TextMeshProUGUI[] _texts, Dictionary<PlayerType, string[]> _scores)
    {
        for (int _i = 0; _i < _texts.Length; _i++)
        {
            _texts[_i].text = string.Format(_texts[_i].text, _scores[PlayerType.BeardLady][_i], _scores[PlayerType.FireEater][_i], _scores[PlayerType.FatLady][_i], _scores[PlayerType.Juggler][_i]);
        }
    }

    /// <summary>
    /// Set nemesis icon.
    /// </summary>
    /// <param name="_icon">Icon to set.</param>
    /// <param name="_nemesis">Nemesis of the player.</param>
    private void SetNemesisIcon(Animator _icon, string _nemesis)
    {
        switch (_nemesis)
        {
            case "Punk":
                _icon.runtimeAnimatorController = punkIcon;
                break;

            case "Mime":
                _icon.runtimeAnimatorController = mimeIcon;
                break;

            case "Mighty Man":
                _icon.runtimeAnimatorController = mightyManIcon;
                break;

            case "Fakir":
                _icon.runtimeAnimatorController = fakirIcon;
                break;

            case "Acrobat":
                _icon.runtimeAnimatorController = acrobatIcon;
                break;

            case "Punk Boss":
                _icon.runtimeAnimatorController = punkBossIcon;
                break;

            case "Siamese":
                _icon.runtimeAnimatorController = siamesesIcon;
                break;

            case "Mr Loyal":
                _icon.runtimeAnimatorController = mrLoyalIcon;
                break;

            default:
                _icon.gameObject.SetActive(false);
                break;
        }
    }
    #endregion
}
