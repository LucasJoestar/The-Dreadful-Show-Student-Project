using Photon;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private Image beardLadyNemesisIcon = null;

    [Space]
    [SerializeField] private TextMeshProUGUI fireEaterKO = null;
    [SerializeField] private TextMeshProUGUI fireEaterDamages = null;
    [SerializeField] private TextMeshProUGUI fireEaterNemesis = null;
    [SerializeField] private Image fireEaterNemesisIcon = null;

    [Space]
    [SerializeField] private TextMeshProUGUI grandeDameKO = null;
    [SerializeField] private TextMeshProUGUI grandeDameDamages = null;
    [SerializeField] private TextMeshProUGUI grandeDameNemesis = null;
    [SerializeField] private Image grandeDameNemesisIcon = null;

    [Space]
    [SerializeField] private TextMeshProUGUI jugglerKO = null;
    [SerializeField] private TextMeshProUGUI jugglerDamages = null;
    [SerializeField] private TextMeshProUGUI jugglerNemesis = null;
    [SerializeField] private Image jugglerNemesisIcon = null;

    [Header("Icons"), Space]
    [SerializeField] private Image punkIcon = null;
    [SerializeField] private Image mimeIcon = null;
    [SerializeField] private Image mightyManIcon = null;
    [SerializeField] private Image fakirIcon = null;
    [SerializeField] private Image acrobatIcon = null;
    [SerializeField] private Image punkBossIcon = null;
    [SerializeField] private Image siamesesIcon = null;
    [SerializeField] private Image mrLoyalIcon = null;
    #endregion

    #region Methods
    /// <summary>
    /// Set the game credits based on players score.
    /// </summary>
    public void SetCredits()
    {
        Dictionary<PlayerType, TDS_PlayerInfo> _playersScore = new Dictionary<PlayerType, TDS_PlayerInfo>() { { PlayerType.BeardLady, TDS_GameManager.PlayersInfo.FirstOrDefault(p => p.PlayerType == PlayerType.BeardLady) }, { PlayerType.FireEater, TDS_GameManager.PlayersInfo.FirstOrDefault(p => p.PlayerType == PlayerType.FireEater) }, { PlayerType.FatLady, TDS_GameManager.PlayersInfo.FirstOrDefault(p => p.PlayerType == PlayerType.FatLady) }, { PlayerType.Juggler, TDS_GameManager.PlayersInfo.FirstOrDefault(p => p.PlayerType == PlayerType.Juggler) } };

        TDS_PlayerInfo _playerInfo = null;
        PlayerType _type = 0;
        Dictionary<PlayerType, string[]> _playersScores = new Dictionary<PlayerType, string[]>();

        for (int i = 1; i < 5; i++)
        {
            _type = (PlayerType)(i - 1);
            _playerInfo = TDS_GameManager.PlayersInfo.FirstOrDefault(p => p.PlayerType == _type);
            
            _playersScores.Add(_type, new string[17] );

            // If no player of the type, just set all to null
            if (_playerInfo == null)
            {
                for (int j = 0; j < _playersScores[_type].Length; j++)
                {
                    _playersScores[_type][j] = "-";
                }
                continue;
            }

            _playersScores[_type][0] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Punk"].ToString();
            _playersScores[_type][1] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Punk"].ToString();

            _playersScores[_type][2] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Mime"].ToString();
            _playersScores[_type][3] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Mime"].ToString();

            _playersScores[_type][4] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Mighty Man"].ToString();
            _playersScores[_type][5] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Mighty Man"].ToString();

            _playersScores[_type][0] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Fakir"].ToString();
            _playersScores[_type][1] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Fakir"].ToString();

            _playersScores[_type][0] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Acrobat"].ToString();
            _playersScores[_type][1] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Acrobat"].ToString();

            _playersScores[_type][0] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Boss Punk"].ToString();
            _playersScores[_type][1] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Boss Punk"].ToString();

            _playersScores[_type][0] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Siamese"].ToString();
            _playersScores[_type][1] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Mr Siamese"].ToString();

            _playersScores[_type][0] = _playerInfo.PlayerScore.KnockoutEnemiesAmount["Mr Loyal"].ToString();
            _playersScores[_type][1] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Mr Loyal"].ToString();

            //_playersScores[_type][0] = _playerInfo.PlayerScore.EnemiesSuffuredDmgs.Keys.Sum().ToString();
            //_playersScores[_type][1] = _playerInfo.PlayerScore.EnemiesInflictedDmgs["Punk"].ToString();
        }

        punksKO.text = string.Format(punksKO.text, "truc");
    }
    #endregion
}
