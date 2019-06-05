using System.Collections;
using System.Linq;
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
     *	Date :			[11 / 04 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    Adjusted the bounds system, once again, but now it's perfect ! Really cool, I mean.
	 *
	 *	-----------------------------------
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
    /// <summary>
    /// Coefficient used in viewport calculs due to camera rotation ; work for a 17 angle.
    /// </summary>
    public const float VIEWPORT_CALCL_Y_COEF = 1.04569165f;

    /// <summary>
    /// Maximum value of the max bound on z on viewport to be allowed.
    /// </summary>
    public const float VIEWPORT_Y_MAX_BOUND_VALUE = .4f;


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
            currentBounds = value;

            // Set new bounds position
            topBound.transform.position = value.ZMaxVector;
            leftBound.transform.position = value.XMinVector;
            rightBound.transform.position = value.XMaxVector;
            bottomBound.transform.position = value.ZMinVector;
        }
    }

    /// <summary>
    /// Base bounds for entire level ; when CurrentBounds are set to null, set the this instead.
    /// </summary>
    [SerializeField] private TDS_Bounds levelBounds = new TDS_Bounds(-10, 10, -10, 10);


    /// <summary>
    /// Bottom bound collider of the level.
    /// </summary>
    [SerializeField] private BoxCollider bottomBound = null;

    /// <summary>
    /// Left bound collider of the level.
    /// </summary>
    [SerializeField] private BoxCollider leftBound = null;

    /// <summary>
    /// Right bound collider of the level.
    /// </summary>
    [SerializeField] private BoxCollider rightBound = null;

    /// <summary>
    /// Top bound collider of the level.
    /// </summary>
    [SerializeField] private BoxCollider topBound = null;


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
    /// Coroutine used to look a target.
    /// </summary>
    private Coroutine lookTargetCoroutine = null;

    /// <summary>
    /// Coroutine used to wait before setting bounds when needed.
    /// </summary>
    private Coroutine waitToSetBoundsCoroutine = null;

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


    /// <summary>
    /// Property to set the bottom bound position.
    /// </summary>
    private Vector3 bottomBoundVector
    {
        set
        {
            currentBounds.ZMinVector = value;
            bottomBound.transform.position = value;
        }
    }

    /// <summary>
    /// Property to set the left bound position.
    /// </summary>
    private Vector3 leftBoundVector
    {
        set
        {
            currentBounds.XMinVector = value;
            leftBound.transform.position = value;
        }
    }

    /// <summary>
    /// Property to set the right bound position.
    /// </summary>
    private Vector3 rightBoundVector
    {
        set
        {
            currentBounds.XMaxVector = value;
            rightBound.transform.position = value;
        }
    }

    /// <summary>
    /// Property to set the top bound position.
    /// </summary>
    private Vector3 topBoundVector
    {
        set
        {
            currentBounds.ZMaxVector = value;
            topBound.transform.position = value;
        }
    }
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
    /// Clamps the camera position between the bounds
    /// </summary>
    public void ClampInBounds()
    {
        // Set the camera position between bounds
        // Get movement
        Vector3 _destination = transform.position;
        Vector3 _viewport;

        // Clamp position
        if ((_viewport = camera.WorldToViewportPoint(currentBounds.XMaxVector)).x < 1f)
        {
            _destination.x -= camera.orthographicSize * ((float)Screen.width / Screen.height) * 2 * (1 - _viewport.x);
        }
        else if ((_viewport = camera.WorldToViewportPoint(currentBounds.XMinVector)).x > 0f)
        {
            _destination.x += camera.orthographicSize * ((float)Screen.width / Screen.height) * 2 * _viewport.x;
        }

        if ((_viewport = camera.WorldToViewportPoint(currentBounds.ZMinVector)).y > 0f)
        {
            _destination.y += camera.orthographicSize * 2 * _viewport.y * VIEWPORT_CALCL_Y_COEF;
        }
        else if ((_viewport = camera.WorldToViewportPoint(currentBounds.ZMaxVector)).y < .4f)
        {
            _destination.y -= camera.orthographicSize * 2 * (.4f - _viewport.y) * VIEWPORT_CALCL_Y_COEF;
        }

        // Moves the camera
        transform.position = _destination;
    }

    /// <summary>
    /// Makes this camera follow its target.
    /// </summary>
    private void FollowTarget()
    {
        // If no target, return
        if (!target || (lookTargetCoroutine != null)) return;

        // If reaching destination, stop moving
        if ((transform.position - (target.position + Offset)).magnitude < .01f)
        {
            if (isMoving)
            {
                speedCurrent = 0;
                isMoving = false;
            }

            return;
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

            if (_movement.y < .01f)
            {
                _destination.y = transform.position.y;
                _movement.y = 0;
            }

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
            if (camera.WorldToViewportPoint(currentBounds.ZMinVector).y > -.01f)
            {
                if (_movement.y < 0) _destination.y = transform.position.y;
                if (_movement.z < 0) _destination.z = transform.position.z;
            }
            if (camera.WorldToViewportPoint(currentBounds.ZMaxVector).y < .4f)
            {
                if (_movement.y > 0) _destination.y = transform.position.y;
                if (_movement.z > 0) _destination.z = transform.position.z;
            }

            // Moves the camera
            transform.position = _destination;
        }
    }

    /// <summary>
    /// Method starting a coroutine that makes the camera look a particular transform for a certain duration.
    /// </summary>
    /// <param name="_x">X position to look.</param>
    /// <param name="_y">Y position to look.</param>
    /// <param name="_z">Z position to look.</param>
    /// <param name="_duration">Time during which fixing the target.</param>
    /// <param name="_speedCoef">New camera speed coefficient.</param>
    /// <returns>Returns the started coroutine.</returns>
    public Coroutine LookTarget(float _x, float _y, float _z, float _duration, float _speedCoef)
    {
        if (lookTargetCoroutine != null) StopCoroutine(lookTargetCoroutine);

        return lookTargetCoroutine = StartCoroutine(LookTargetCoroutine(new Vector3(_x, _y, _z), _duration, _speedCoef));
    }

    /// <summary>
    /// Coroutine to make the camera look a particular transform for a certain duration.
    /// </summary>
    /// <param name="_target">Target position to look.</param>
    /// <param name="_duration">Time during which fixing the target.</param>
    /// <param name="_speedCoef">New camera speed coefficient.</param>
    private IEnumerator LookTargetCoroutine(Vector3 _target, float _duration, float _speedCoef)
    {
        while (_duration > 0)
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
            Vector3 _destination = Vector3.Lerp(transform.position, _target + Offset, ((Time.deltaTime * speedCurrent) / 5) * _speedCoef);

            transform.position = _destination;

            yield return null;

            Vector3 _movement = _destination - transform.position;

            if (_movement.magnitude <= .001f)
            {
                _duration -= Time.deltaTime;
            }
        }

        lookTargetCoroutine = null;
    }

    /// <summary>
    /// Reset the level bounds.
    /// </summary>
    public void ResetBounds()
    {
        CurrentBounds = levelBounds;
    }

    /// <summary>
    /// Set new bounds for the camera.
    /// </summary>
    /// <param name="_levelBounds">New level bounds.</param>
    public void SetBounds(TDS_LevelBounds _levelBounds)
    {
        TDS_Bounds _bounds = new TDS_Bounds(_levelBounds.LeftBound.x != 0 ?
                                            _levelBounds.LeftBound :                             
                                            currentBounds.XMinVector,

                                            _levelBounds.RightBound.x != 0 ?   
                                            _levelBounds.RightBound :
                                            currentBounds.XMaxVector,

                                            _levelBounds.BottomBound.z != 0 ? 
                                            _levelBounds.BottomBound :
                                            currentBounds.ZMinVector,

                                            _levelBounds.TopBound.z != 0 ?
                                            _levelBounds.TopBound :
                                            currentBounds.ZMaxVector);

        if (currentBounds == _bounds) return;

        if (waitToSetBoundsCoroutine != null) StopCoroutine(waitToSetBoundsCoroutine);
        waitToSetBoundsCoroutine = StartCoroutine(SetBoundsInTime(_bounds));
    }

    /// <summary>
    /// Wait to set bounds if minimum x value is visible by the camera or to its right.
    /// </summary>
    /// <param name="_bounds">Bounds to set.</param>
    /// <returns></returns>
    private IEnumerator SetBoundsInTime(TDS_Bounds _bounds)
    {
        // Get the movement direction of the bounds
        int[] _boundsMovement = new int[4];

        if ((TDS_LevelManager.Instance.LocalPlayer.transform.position.x > (_bounds.XMin + 1)) &&
            (camera.WorldToViewportPoint(_bounds.XMinVector).x < .0001f))
        {
            leftBoundVector = _bounds.XMinVector;
            _boundsMovement[0] = 0;
        }
        else _boundsMovement[0] = _bounds.XMin > currentBounds.XMin ? 1 : -1;

        if ((TDS_LevelManager.Instance.LocalPlayer.transform.position.x < (_bounds.XMax - 1)) &&
            (camera.WorldToViewportPoint(_bounds.XMaxVector).x > .9999f))
        {
            rightBoundVector = _bounds.XMaxVector;
            _boundsMovement[1] = 0;
        }
        else _boundsMovement[1] = _bounds.XMax > currentBounds.XMax ? 1 : -1;

        if ((TDS_LevelManager.Instance.LocalPlayer.transform.position.z > (_bounds.ZMin + 1)) &&
            (camera.WorldToViewportPoint(_bounds.ZMinVector).y < .0001f))
        {
            bottomBoundVector = _bounds.ZMinVector;
            _boundsMovement[2] = 0;
        }
        else _boundsMovement[2] = _bounds.ZMin > currentBounds.ZMin ? 1 : -1;

        if ((TDS_LevelManager.Instance.LocalPlayer.transform.position.z < (_bounds.ZMax - 1)) &&
            (camera.WorldToViewportPoint(_bounds.ZMaxVector).z > (VIEWPORT_Y_MAX_BOUND_VALUE - .0001f)))
        {
            topBoundVector = _bounds.ZMaxVector;
            _boundsMovement[3] = 0;
        }
        else _boundsMovement[3] = _bounds.ZMax > currentBounds.ZMax ? 1 : -1;


        while (_boundsMovement.Any(m => m != 0))
        {
            // Left bound move
            if (_boundsMovement[0] != 0)
            {
                float _xMin = camera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
                int _movement = _xMin == currentBounds.XMin ? 0 : _xMin > currentBounds.XMin ? 1 : -1;

                if ((_boundsMovement[0] == _movement) && (TDS_LevelManager.Instance.LocalPlayer.transform.position.x > (_xMin + 1)))
                {
                    if (((_movement == 1) && (_xMin >= _bounds.XMin)) || ((_movement == -1) && (_xMin <= _bounds.XMin)))
                    {
                        leftBoundVector = _bounds.XMinVector;
                        _boundsMovement[0] = 0;
                    }
                    else
                    {
                        leftBoundVector = new Vector3(_xMin, _bounds.XMinVector.y, _bounds.XMinVector.z);
                        if (camera.ViewportToWorldPoint(new Vector3(1.01f, 0, 0)).x >= currentBounds.XMax) _boundsMovement[0] = 0;
                    }
                }
            }
            // Right bound move
            if (_boundsMovement[1] != 0)
            {
                float _xMax = camera.ViewportToWorldPoint(new Vector3(1.01f, 0, 0)).x;
                int _movement = _xMax == currentBounds.XMax ? 0 : _xMax > currentBounds.XMax ? 1 : -1;

                if ((_boundsMovement[1] == _movement) && (TDS_LevelManager.Instance.LocalPlayer.transform.position.x < (_xMax - 1)))
                {
                    if (((_movement == 1) && (_xMax >= _bounds.XMax)) || ((_movement == -1) && (_xMax <= _bounds.XMax)))
                    {
                        rightBoundVector = _bounds.XMaxVector;
                        _boundsMovement[1] = 0;
                    }
                    else
                    {
                        rightBoundVector = new Vector3(_xMax, _bounds.XMaxVector.y, _bounds.XMaxVector.z);
                        if (camera.ViewportToWorldPoint(new Vector3(-.01f, 0, 0)).x <= currentBounds.XMin) _boundsMovement[1] = 0;
                    }
                }
            }
            // Bottom bound move
            if (_boundsMovement[2] != 0)
            {
                float _zMin = camera.ViewportToWorldPoint(new Vector3(0, -.01f, 0)).x;
                int _movement = _zMin == currentBounds.ZMin ? 0 : _zMin > currentBounds.ZMin ? 1 : -1;

                if ((_boundsMovement[2] == _movement) && (TDS_LevelManager.Instance.LocalPlayer.transform.position.z > (_zMin + 1)))
                {
                    if (((_movement == 1) && (_zMin >= _bounds.ZMin)) || ((_movement == -1) && (_zMin <= _bounds.ZMin)))
                    {
                        bottomBoundVector = _bounds.ZMinVector;
                        _boundsMovement[2] = 0;
                    }
                    else
                    {
                        bottomBoundVector = new Vector3(_bounds.ZMinVector.x, _bounds.ZMinVector.y, _zMin);
                        if (camera.ViewportToWorldPoint(new Vector3(.3f, 0, 0)).z >= currentBounds.ZMax) _boundsMovement[2] = 0;
                    }
                }
            }
            // Top bound move
            if (_boundsMovement[3] != 0)
            {
                float _zMax = camera.ViewportToWorldPoint(new Vector3(0, .3f, 0)).x;
                int _movement = _zMax == currentBounds.ZMax ? 0 : _zMax > currentBounds.ZMax ? 1 : -1;

                if ((_boundsMovement[3] == _movement) && (TDS_LevelManager.Instance.LocalPlayer.transform.position.z < (_zMax - 1)))
                {
                    if (((_movement == 1) && (_zMax >= _bounds.ZMax)) || ((_movement == -1) && (_zMax <= _bounds.ZMax)))
                    {
                        topBoundVector = _bounds.ZMaxVector;
                        _boundsMovement[3] = 0;
                    }
                    else
                    {
                        topBoundVector = new Vector3(_bounds.ZMaxVector.x, _bounds.ZMaxVector.y, _zMax);
                        if (camera.ViewportToWorldPoint(new Vector3(-.01f, 0, 0)).z <= currentBounds.ZMin) _boundsMovement[3] = 0;
                    }
                }
            }

            yield return null;
        }

        waitToSetBoundsCoroutine = null;
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

    /// <summary>
    /// Make a screen shake of a specified force.
    /// </summary>
    /// <param name="_force">Screen shake force.</param>
    /// <param name="_time">Total duration of the screen shake.</param>
    public IEnumerator ScreenShake(float _force, float _time)
    {
        float _timer = _time;

        while (_timer > 0)
        {
            Vector3 _force3 = ((Vector3)Random.insideUnitCircle.normalized) * _force;
            transform.position += _force3;

            yield return null;
            _timer -= Time.deltaTime;
        }
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance if null
        if (!Instance) Instance = this;

        // Debug warning is missing bound(s)
        if (!topBound || !leftBound || !rightBound || !bottomBound)
        {
            Debug.LogWarning($"Missing bound(s) for the Camera \"{name}\"");
        }
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
    }

    // Use this for initialization
    private void Start ()
    {
        // Set default level bounds
        levelBounds = new TDS_Bounds(leftBound.transform.position, rightBound.transform.position, bottomBound.transform.position, topBound.transform.position);
        currentBounds = levelBounds;

        // Set the camera position between bounds
        ClampInBounds();
    }
	
	// Update is called once per frame
	private void Update ()
    {
        FollowTarget();
	}
	#endregion

	#endregion
}
