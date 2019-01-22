/* TDS_EnumsHolder :
 *
 *	#####################
 *	###### PURPOSE ######
 *	#####################
 *
 *	References all enums used for the project "The Dreadful Show"
 *
 *	#####################
 *	### MODIFICATIONS ###
 *	#####################
 *
 *	Date :			[21 / 01 / 2019]
 *	Author :		[Guibert Lucas]
 *
 *	Changes :
 *
 *  Initialization of the class.
 *	    - Added the PlayerType enum.
 *
 *	-----------------------------------
 *	
 *	Date :			[22 / 01 / 2019]
 *	Author :		[THIEBAUT Alexis]
 *
 *	Changes :
 *
 *  Adding more enums.
 *  	- Added the EnemyState enum.
*/

public enum PlayerType
{
    BeardLady,
    FatLady,
    FireEater,
    Juggler,
    Unknown
}

public enum EnemyState
{
    Searching, 
    ComputingPath, 
    GettingInRange, 
    Attacking, 
    HoldingObject 
}