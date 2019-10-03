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

    /// <summary>
    /// First known X position of the object.
    /// </summary>
    private float startPositionX = 0;

    /// <summary>
    /// X position of the object when stopping scrolling for the first time.
    /// </summary>
    private float exitPositionX = 0;
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
    public void ChangeActivation()
    {
        isActive = !isActive;

        if (!isActive) exitPositionX = transform.position.x;
    }

    /// <summary>
    /// Desactivate this parallax scrolling.
    /// </summary>
    public void Desactivate()
    {
        if (!isActive) return;

        isActive = false;
        exitPositionX = transform.position.x;
    }


    /// <summary>
    /// Makes the GameObject scroll along the camera X movement.
    /// </summary>
    /// <param name="_cameraMovement">Movement of the camera on the X axis.</param>
    public void Scroll(float _cameraMovement)
    {
        if (!isActive) return;

        Vector3 _newPosition = new Vector3(transform.position.x + (_cameraMovement * scrollingCoef), transform.position.y, transform.position.z);

        if (startPositionX != exitPositionX)
        {
            //_newPosition.x = startPositionX < exitPositionX ? Mathf.Clamp(_newPosition.x, startPositionX, exitPositionX) : Mathf.Clamp(_newPosition.x, exitPositionX, startPositionX);
        }

        transform.position = _newPosition;
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
        startPositionX = transform.position.x;
        TDS_Camera.Instance.OnMoveX += Scroll;
    }
	#endregion

	#endregion
}
