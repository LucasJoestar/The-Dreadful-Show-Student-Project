using UnityEngine;

public class TDS_CloudBehaviour : MonoBehaviour 
{
    /* TDS_CloudBehaviour :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages the behaviour (movement) of the clouds.
	 *
	 *	#####################
	 *	####### TO DO #######
	 *	#####################
	 *
	 *	...
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[18 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_CloudBehaviour class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Speed movement of the cloud.
    /// </summary>
    [SerializeField] private float speed = 1;
	#endregion

	#region Methods

	#region Original Methods
    /// <summary>
    /// Moves the cloud.
    /// </summary>
    private void Move()
    {
        transform.position += Vector3.left * Time.deltaTime * speed;
    }
    #endregion

    #region Unity Methods
    // Update is called once per frame
    private void Update()
    {
        Move();
	}
	#endregion

	#endregion
}
