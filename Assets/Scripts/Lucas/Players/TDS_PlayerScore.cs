using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class TDS_PlayerScore
{
    /* TDS_PlayerInfo :
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

    #region Fields / Properties
    /// <summary>
    /// Detected tags for enemies used to increase score.
    /// </summary>
    private static string[] enemiesTags = new string[] { "Punk", "Mime", "MightyMan", "Fakhir", "Acrobat", "PunkBoss", "Siamese", "MrLoyal" };


    /// <summary>
    /// Score related to collectibles.
    /// </summary>
    public int CollectiblesScore = 0;


    /// <summary>
    /// Amount of knockout enemies.
    /// </summary>
    public int KnockoutEnemiesAmount = 0;


    /// <summary>
    /// All amount of damages inflicted to enemies.
    /// </summary>
    public Dictionary<string, int> EnemiesInflictedDmgs = new Dictionary<string, int>();

    /// <summary>
    /// All amount of damages suffured by enemies.
    /// </summary>
    public Dictionary<string, int> EnemiesSuffuredDmgs = new Dictionary<string, int>();
    #endregion

    #region Constructor
    /// <summary>
    /// Creates a new player score.
    /// </summary>
    public TDS_PlayerScore()
    {
        foreach (string _enemy in enemiesTags)
        {
            EnemiesInflictedDmgs.Add(_enemy, 0);
            EnemiesSuffuredDmgs.Add(_enemy, 0);
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Increases the inflicted damages score.
    /// </summary>
    /// <param name="_enemy">Enemy taking damages.</param>
    /// <param name="_damages">Amount of inflicted damages.</param>
    public void IncreaseInflictedScore(TDS_Enemy _enemy, int _damages)
    {
        string[] _enemyTags = _enemy.gameObject.GetTagNames().Intersect(enemiesTags).ToArray();

        foreach (string _tag in _enemyTags)
        {
            EnemiesInflictedDmgs[_tag] += _damages;
            if (_enemy.IsDead) KnockoutEnemiesAmount++;
        }
    }

    /// <summary>
    /// Increases the suffured damages score.
    /// </summary>
    /// <param name="_enemy">Enemy inflicting damages.</param>
    /// <param name="_damages">Amount of suffured damages.</param>
    public void IncreaseSuffuredScore(TDS_Enemy _enemy, int _damages)
    {
        string[] _enemyTags = _enemy.gameObject.GetTagNames().Intersect(enemiesTags).ToArray();

        foreach (string _tag in _enemyTags)
        {
            EnemiesSuffuredDmgs[_tag] += _damages;
        }
    }
    #endregion
}
