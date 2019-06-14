using UnityEngine;

public class TDS_ParallaxScrolling : MonoBehaviour 
{
    /* TDS_ParallaxScrolling :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Creates a parallax scrolling effect between this script GameObject and the Camera.
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
	 *	Date :			[13 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_ParallaxScrolling class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Indicates if the parallax scrolling is active or not.
    /// </summary>
    [SerializeField] private bool isActive = true;

    /// <summary>
    /// Scrolling coefficient related to the camera movement used to move the GameObject.
    /// </summary>
    [SerializeField] private float scrollingCoef = 1;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Activate this parallax scrolling.
    /// </summary>
    public void Activate() => isActive = true;

    /// <summary>
    /// Change the activation of the parallax scrolling.
    /// </summary>
    public void ChangeActivation() => isActive = !isActive;

    /// <summary>
    /// Desactivate this parallax scrolling.
    /// </summary>
    public void Desactivate() => isActive = false;


    /// <summary>
    /// Makes the GameObject scroll along the camera X movement.
    /// </summary>
    /// <param name="_cameraMovement">Movement of the camera on the X axis.</param>
    public void Scroll(float _cameraMovement)
    {
        if (!isActive) return;

        transform.position = new Vector3(transform.position.x + (_cameraMovement * scrollingCoef), transform.position.y, transform.position.z);
    }
    #endregion

    #region Unity Methods
    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        if (TDS_Camera.Instance) TDS_Camera.Instance.OnMoveX -= Scroll;
    }

    // Use this for initialization
    private void Start()
    {
        TDS_Camera.Instance.OnMoveX += Scroll;
    }
	#endregion

	#endregion
}
