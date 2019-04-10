using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TDS_Camera : MonoBehaviour 
{
    /* TDS_Camera :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Behaves the game camera.
     *	
     *	    It can follow a transform target, with a certain speed,
     *	and be clamped by boundaries.
	 *
     *	#####################
	 *	####### TO DO #######
	 *	#####################
     * 
     *  - Add the option to change target when dead.
     * 
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
     *	Date :			[04 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Updated new bounds system.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[03 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    When settings new bounds, wait to set them as current if target is not in.
	 *
	 *	-----------------------------------
     * 
     *	Date :			[25 / 03 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Created a singleton instance of the class.
	 *
	 *	-----------------------------------
     * 
	 *	Date :			[25 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Camera class.
     *	
     *	    - Added the Bounds & Offset fields ; and the IsMoving, Camera, Rotation, SpeedAccelerationTime, SpeedCoef, SpeedCurrent, SpeedInitial, SpeedMax & Target fields & properties.
     *	    - Added the FollowTarget method.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>Backing field for <see cref="IsMoving"/></summary>
    [SerializeField] private bool isMoving = false;

    /// <summary>
    /// Indicates if the camera is currently moving.
    /// </summary>
    public bool IsMoving
    {
        get { return isMoving; }
        set
        {
            isMoving = value;
        }
    }

    /// <summary>
    /// Current bounds to clamp the camera position.
    /// </summary>
    [SerializeField] private TDS_Bounds currentBounds = null;

    public TDS_Bounds CurrentBounds
    {
        get { return currentBounds; }
        set
        {
            float _xMin = camera.ViewportToWorldPoint(new Vector3(-.01f, 0, 0)).x;
            float _xMax = camera.ViewportToWorldPoint(new Vector3(1.01f, 0, 0)).x;

            if (value.XMin > _xMin) value.XMinVector.x = _xMin;
            if (value.XMax < _xMax) value.XMaxVector.x = _xMax;

            currentBounds = value;
        }
    }

    /// <summary>
    /// Base bounds for entire level ; when CurrentBounds are set to null, set the this instead.
    /// </summary>
    [SerializeField] private TDS_Bounds levelBounds = new TDS_Bounds(-10, 10, -10, 10);

    /// <summary>Backing field for <see cref="Camera"/>.</summary>
    [SerializeField] private new Camera camera = null;

    /// <summary>
    /// Camera attached to this script.
    /// </summary>
    public Camera Camera
    {
        get { return camera; }
        private set
        {
            camera = value;
        }
    }

    /// <summary>
    /// Coroutine used to lerp to bounds.
    /// </summary>
    private Coroutine lerpToBoundsCoroutine = null;

    /// <summary>Backing field for <see cref="Rotation"/></summary>
    [SerializeField] private float rotation = 0;

    /// <summary>
    /// Rotation in X axis of the camera.
    /// </summary>
    public float Rotation
    {
        get { return rotation; }
        set
        {
            transform.rotation = Quaternion.Euler(value, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

            rotation = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedAccelerationTime"/></summary>
    [SerializeField] private float speedAccelerationTime = .5f;

    /// <summary>
    /// The time it takes (in seconds) for this camera speed (<see cref="SpeedCurrent"/>) from its initial value when starting to move (<see cref="SpeedInitial"/>) to reach its limit (<see cref="SpeedMax"/>).
    /// </summary>
    public float SpeedAccelerationTime
    {
        get { return speedAccelerationTime; }
        set
        {
            if (value < 0) value = 0;
            speedAccelerationTime = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedCoef"/></summary>
    [SerializeField] private float speedCoef = 1;

    /// <summary>
    /// Coefficient used to multiply all speed values of this camera.
    /// Useful to slow down or speed up.
    /// </summary>
    public float SpeedCoef
    {
        get { return speedCoef; }
        set
        {
            if (value < 0) value = 0;
            speedCoef = value;
        }
    }

    /// <summary>Backing field for <see cref="speedCurrent"/></summary>
    [SerializeField] private float speedCurrent = 0;

    /// <summary>
    /// Current speed of the camera.
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedCurrent
    {
        get { return speedCurrent; }
        protected set
        {
            value = Mathf.Clamp(value, 0, SpeedMax);
            speedCurrent = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedInitial"/></summary>
    [SerializeField] private float speedInitial = 1;

    /// <summary>
    /// Initial speed of the camera when starting moving.
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedInitial
    {
        get { return speedInitial; }
        set
        {
            value = Mathf.Clamp(value, 0, speedMax);
            speedInitial = value;
        }
    }

    /// <summary>Backing field for <see cref="SpeedMax"/></summary>
    [SerializeField] private float speedMax = 2;

    /// <summary>
    /// Maximum speed of the camera.
    /// (Without taking into account the speed coefficient.)
    /// </summary>
    public float SpeedMax
    {
        get { return speedMax; }
        set
        {
            if (value < 0) value = 0;
            speedMax = value;

            if (speedCurrent > value) SpeedCurrent = value;
        }
    }

    /// <summary>
    /// Current level bounds object.
    /// </summary>
    [SerializeField] protected TDS_LevelBounds currentLevelBounds = null;

    /// <summary>Backing field for <see cref="Target"/>.</summary>
    [SerializeField] private Transform target = null;

    /// <summary>
    /// Transform this camera follows.
    /// </summary>
    public Transform Target
    {
        get { return target; }
        set
        {
            if (isMoving)
            {
                speedCurrent = 0;
                isMoving = false;
            }

            target = value;
        }
    }

    /// <summary>
    /// Offset of the camera in X, Y & Z.
    /// </summary>
    public Vector3 Offset = Vector3.zero;
    #endregion

    #region Singleton
    /// <summary>
    /// Singleton instance of this class.
    /// </summary>
    public static TDS_Camera Instance = null;
    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Reset the level bounds.
    /// </summary>
    public void ResetBounds()
    {
        if (currentLevelBounds)
        {
            currentLevelBounds.gameObject.SetActive(false);
            currentLevelBounds = null;
        }
        CurrentBounds = levelBounds;
    }

    /// <summary>
    /// Makes this camera follow its target.
    /// </summary>
    private void FollowTarget()
    {
        // If no target, return
        if (!Target || (lerpToBoundsCoroutine != null)) return;

        // If reaching destination, stop moving
        if (transform.position == target.position + Offset)
        {
            if (isMoving)
            {
                speedCurrent = 0;
                isMoving = false;
            }
        }
        else
        {
            // When starting moving, initializes initial speed
            if (!isMoving)
            {
                speedCurrent = speedInitial;
                isMoving = true;
            }
            // If not, increase speed if needed
            else if (speedCurrent != speedMax)
            {
                SpeedCurrent += Time.deltaTime * ((speedMax - speedInitial) / speedAccelerationTime);
            }

            // Get movement
            Vector3 _destination = Vector3.Lerp(transform.position, target.transform.position + Offset, Time.deltaTime * speedCurrent * speedCoef);
            Vector3 _movement = _destination - transform.position;

            // Clamp position

            // X movement
            if (_movement.x != 0)
            {
                if (_movement.x > 0)
                {
                    if (camera.WorldToViewportPoint(currentBounds.XMaxVector).x < 1.01f)
                    {
                        _destination.x = transform.position.x;
                    }
                }
                else if (camera.WorldToViewportPoint(currentBounds.XMinVector).x > -.01f)
                {
                    _destination.x = transform.position.x;
                }
            }

            // Y & Z movement
            if (camera.WorldToViewportPoint(currentBounds.ZMaxVector).y < .3f)
            {
                if (_movement.z > 0) _destination.z = transform.position.z;
            }
            if (camera.WorldToViewportPoint(currentBounds.ZMinVector).y > -.01f)
            {
                if (_movement.y < 0) _destination.y = transform.position.y;
                if (_movement.z < 0) _destination.z = transform.position.z;
            }

            // Moves the camera
            transform.position = _destination;
        }
    }

    /// <summary>
    /// Lerp the camera position to be between bounds.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LerpToBounds()
    {
        bool _isGoodInX = false;

        yield return null;

        while (true)
        {
            yield return new WaitForEndOfFrame();

            // Get movement
            Vector3 _destination = transform.position;

            // Clamp position
            if (camera.WorldToViewportPoint(currentBounds.XMaxVector).x < 1.02f)
            {
                _destination.x -= 1;
            }
            else if (camera.WorldToViewportPoint(currentBounds.XMinVector).x > -.02f)
            {
                _destination.x += 1;
            }
            else _isGoodInX = true;

            if (camera.WorldToViewportPoint(currentBounds.ZMinVector).y > -.02f)
            {
                _destination.z += 1;
            }
            else if (_isGoodInX) break;

            // Moves the camera
            transform.position = Vector3.Lerp(transform.position, _destination, Time.deltaTime * speedMax * 2);
        }

        lerpToBoundsCoroutine = null;
    }

    /// <summary>
    /// Set new bounds for the camera.
    /// </summary>
    /// <param name="_levelBounds">New level bounds.</param>
    public void SetBounds(TDS_LevelBounds _levelBounds)
    {
        if (currentLevelBounds == _levelBounds) return;

        TDS_Bounds _bounds = new TDS_Bounds(_levelBounds.LeftBound != null ?
                                            _levelBounds.LeftBound.position.x :                             
                                            levelBounds.XMin,

                                            _levelBounds.RightBound != null ?   
                                            _levelBounds.RightBound.position.x :                    
                                            levelBounds.XMax,

                                            _levelBounds.BottomBound != null ? 
                                            _levelBounds.BottomBound.position.z : 
                                            levelBounds.ZMin,

                                            _levelBounds.TopBound != null ?
                                            _levelBounds.TopBound.position.z :                        
                                            levelBounds.ZMax);

        if (currentLevelBounds) currentLevelBounds.Desactivate();
        currentLevelBounds = _levelBounds;

        CurrentBounds = _bounds;
    }

    /// <summary>
    /// Make a screen shake of a specified force.
    /// </summary>
    /// <param name="_force">Screen shake force.</param>
    public void ScreenShake(float _force)
    {
        Vector3 _force3 = ((Vector3)Random.insideUnitCircle.normalized) * _force;
        transform.position += _force3;
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance if null
        if (!Instance) Instance = this;
    }

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        // Nullify the singleton instance if needed
        if (Instance == this) Instance = null;
    }

    // Implement OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmos()
    {
        // Draws the camera bounds with lines
        Gizmos.color = Color.yellow;

        Gizmos.DrawCube(new Vector3(levelBounds.XMin, transform.position.y, levelBounds.ZMin), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XMin, transform.position.y, levelBounds.ZMin), new Vector3(levelBounds.XMax, transform.position.y, levelBounds.ZMin));
        Gizmos.DrawCube(new Vector3(levelBounds.XMin, transform.position.y, levelBounds.ZMax), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XMin, transform.position.y, levelBounds.ZMin), new Vector3(levelBounds.XMin, transform.position.y, levelBounds.ZMax));
        Gizmos.DrawCube(new Vector3(levelBounds.XMax, transform.position.y, levelBounds.ZMin), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XMin, transform.position.y, levelBounds.ZMax), new Vector3(levelBounds.XMax, transform.position.y, levelBounds.ZMax));
        Gizmos.DrawCube(new Vector3(levelBounds.XMax, transform.position.y, levelBounds.ZMax), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XMax, transform.position.y, levelBounds.ZMax), new Vector3(levelBounds.XMax, transform.position.y, levelBounds.ZMin));
    }

    // Use this for initialization
    private void Start ()
    {
    }
	
	// Update is called once per frame
	private void Update ()
    {
        FollowTarget();
	}
	#endregion

	#endregion
}
