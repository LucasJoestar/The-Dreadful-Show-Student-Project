using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

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
     *	Date :			[07 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	    The camera with bounds now works PERFECTLY good, what a great news. Yeah.
	 *
	 *	-----------------------------------
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

    #region Events
    /// <summary>
    /// Event called when the camera moves on X with the movement value as parameter.
    /// </summary>
    public event Action<float> OnMoveX = null;

    /// <summary>
    /// Event called the Bound X minimum value is changed.
    /// </summary>
    public event Action OnXMinBoundChanged = null;

    public event Action OnBoundFreeze = null;
    #endregion

    #region Fields / Properties

    #region Constants
    /// <summary>
    /// Target aspect width used to calculate the camera rect.
    /// </summary>
    public const float CAMERA_ASPECT_WIDTH = 16f;

    /// <summary>
    /// Target aspect height used to calculate the camera rect.
    /// </summary>
    public const float CAMERA_ASPECT_HEIGHT = 9f;

    /// <summary>
    /// Coefficient used in viewport calculs due to camera rotation ; work for a 17 angle.
    /// </summary>
    public const float VIEWPORT_CALCL_Y_COEF = 1.04569165f;
    #endregion

    #region Screen Ratio
    /// <summary>
    /// Camera X ratio used for many calculs related to camera bounds and screen size.
    /// </summary>
    public float CameraXRatio { get; private set; } = 0;
    #endregion

    #region Variables
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
    [SerializeField] private TDS_Bounds currentBounds = new TDS_Bounds();

    public TDS_Bounds CurrentBounds
    {
        get { return currentBounds; }
        set
        {
            currentBounds = value;

            // Set new bounds position
            topBound.position = value.ZMaxVector;
            leftBound.position = value.XMinVector;
            rightBound.position = value.XMaxVector;
            bottomBound.position = value.ZMinVector;

            OnXMinBoundChanged?.Invoke();
        }
    }

    /// <summary>
    /// Base bounds for entire level ; when CurrentBounds are set to null, set the this instead.
    /// </summary>
    [SerializeField] private TDS_Bounds levelBounds = new TDS_Bounds(-10, 10, -10, 10);


    /// <summary>
    /// Bottom bound collider of the level.
    /// </summary>
    [SerializeField] private Transform bottomBound = null;

    /// <summary>
    /// Left bound collider of the level.
    /// </summary>
    [SerializeField] private Transform leftBound = null;

    /// <summary>
    /// Right bound collider of the level.
    /// </summary>
    [SerializeField] private Transform rightBound = null;

    /// <summary>
    /// Top bound collider of the level.
    /// </summary>
    [SerializeField] private Transform topBound = null;


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
    private Coroutine setBoundsCoroutine = null;

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
    /// Maximum value of the max bound on z on viewport to be allowed.
    /// </summary>
    public float ViewportYMacBoundValue = .2f;

    /// <summary>
    /// Players to follow for local multiplayer mode.
    /// </summary>
    [SerializeField] private List<TDS_Player> players = new List<TDS_Player>();

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
            bottomBound.position = value;
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
            leftBound.position = value;
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
            rightBound.position = value;
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
            topBound.position = value;
        }
    }


    /// <summary>
    /// Position of the camera on the previous frame.
    /// </summary>
    private Vector3 previousPosition = new Vector3();

    /// <summary>
    /// Bounds value updated when setting new bounds.
    /// This field contains bounds values when set, which then need to be send to other players if online (-999 value means bounds hasn't changed).
    /// </summary>
    private Vector4 onlineSendingBounds = new Vector4(-999, -999, -999, -999);
    #endregion

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
    /// Adds a player to follow.
    /// </summary>
    /// <param name="_player">Player to follow.</param>
    public void AddLocalPlayer(TDS_Player _player)
    {
        players.Add(_player);
    }

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
            _destination.x -= CameraXRatio * 2 * (1 - _viewport.x);
        }
        else if ((_viewport = camera.WorldToViewportPoint(currentBounds.XMinVector)).x > 0f)
        {
            _destination.x += CameraXRatio * 2 * _viewport.x;
        }

        if ((_viewport = camera.WorldToViewportPoint(currentBounds.ZMinVector)).y > 0f)
        {
            _destination.y += camera.orthographicSize * 2 * _viewport.y * VIEWPORT_CALCL_Y_COEF;
        }
        else if ((_viewport = camera.WorldToViewportPoint(currentBounds.ZMaxVector)).y < ViewportYMacBoundValue)
        {
            _destination.y -= camera.orthographicSize * 2 * (ViewportYMacBoundValue - _viewport.y) * VIEWPORT_CALCL_Y_COEF;
        }

        // Moves the camera
        transform.position = _destination;
    }

    /// <summary>
    /// Makes this camera follow the players.
    /// </summary>
    private void FollowPlayers()
    {
        // If no target, return
        if ((players.Count == 0) || (lookTargetCoroutine != null)) return;

        TDS_Player[] _playersByX = players.OrderBy(p => p.transform.position.x).ToArray();
        TDS_Player _mostRightPlayer = _playersByX.Last();

        float _mostLeftPlayerFromView = _playersByX[0].transform.position.x - (transform.position.x - CameraXRatio);
        bool _isPlayerOutOfScreen = _mostLeftPlayerFromView < 1;

        // Get movement
        float _xIdealPos = _mostRightPlayer.transform.position.x + Offset.x - (_isPlayerOutOfScreen ? CameraXRatio - 2 : _mostLeftPlayerFromView < CameraXRatio ? CameraXRatio - _mostLeftPlayerFromView : 0);
        float _yIdealPos = transform.position.y + (-(.5f - camera.WorldToViewportPoint(players.OrderBy(p => p.transform.position.z).First().transform.position).y) * camera.orthographicSize * 2 * VIEWPORT_CALCL_Y_COEF) + Offset.y;

        // If reaching destination, stop moving
        if ((transform.position - new Vector3(_xIdealPos, _yIdealPos, Offset.z)).magnitude < .01f)
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
            else if (speedCurrent < speedMax)
            {
                SpeedCurrent += Time.deltaTime * ((speedMax - speedInitial) / speedAccelerationTime);
            }

            Vector3 _destination = new Vector3()
            {
                x = Mathf.Lerp(transform.position.x, _xIdealPos, Time.deltaTime * speedCurrent * speedCoef),
                y = Mathf.Lerp(transform.position.y, _yIdealPos, Time.deltaTime * speedCurrent * speedCoef),
                z = Offset.z
            };

            Vector3 _movement = _destination - transform.position;

            // Clamp position
            float _newBound;

            // X movement
            if (_movement.x != 0)
            {
                if (_movement.x < 0)
                {
                    _newBound = _destination.x - CameraXRatio;

                    if (_newBound < currentBounds.XMin)
                    {
                        _destination.x += CameraXRatio * 2 * camera.WorldToViewportPoint(currentBounds.XMinVector).x;

                        // Cancel movement if needed
                        if ((_destination.x - transform.position.x) < .0001f) _destination.x = transform.position.x;
                    }
                }
                else
                {
                    _newBound = _destination.x + CameraXRatio;

                    if (_newBound > currentBounds.XMax)
                    {
                        _destination.x -= CameraXRatio * 2 * (1 - camera.WorldToViewportPoint(currentBounds.XMaxVector).x);

                        // Cancel movement if needed
                        if ((transform.position.x - _destination.x) < .0001f) _destination.x = transform.position.x;
                    }
                }
            }
            // Y movement
            if (_movement.y != 0)
            {
                if (_movement.y < 0)
                {
                    _newBound = camera.WorldToViewportPoint(currentBounds.ZMinVector - _movement).y;

                    if (_newBound > 0)
                    {
                        _destination.y += camera.orthographicSize * 2 * VIEWPORT_CALCL_Y_COEF * camera.WorldToViewportPoint(currentBounds.ZMinVector).y;

                        // Cancel movement if needed
                        if ((_destination.y - transform.position.y) < .0001f) _destination.y = transform.position.y;
                    }
                }
                else
                {
                    _newBound = camera.WorldToViewportPoint(currentBounds.ZMaxVector - _movement).y;

                    if (_newBound < ViewportYMacBoundValue)
                    {
                        _destination.y -= camera.orthographicSize * 2 * VIEWPORT_CALCL_Y_COEF * (ViewportYMacBoundValue - camera.WorldToViewportPoint(currentBounds.ZMaxVector).y);

                        // Cancel movement if needed
                        if ((transform.position.y - _destination.y) < .0001f) _destination.y = transform.position.y;
                    }
                }
            }

            // Moves the camera if needed, or stop moving
            if (transform.position == _destination)
            {
                if (isMoving)
                {
                    isMoving = false;
                    SpeedCurrent = 0;
                }
            }
            else
            {
                transform.position = _destination;
            }
        }
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
            else if (speedCurrent < speedMax)
            {
                SpeedCurrent += Time.deltaTime * ((speedMax - speedInitial) / speedAccelerationTime);
            }

            // Get movement
            float _yIdealPos = transform.position.y + (-(.5f - camera.WorldToViewportPoint(target.transform.position).y) * camera.orthographicSize * 2 * VIEWPORT_CALCL_Y_COEF) + Offset.y;

            Vector3 _destination = new Vector3()
            {
                x = Mathf.Lerp(transform.position.x, target.transform.position.x + Offset.x, Time.deltaTime * speedCurrent * speedCoef),
                y = Mathf.Lerp(transform.position.y, _yIdealPos, Time.deltaTime * speedCurrent * speedCoef),
                z = Offset.z
            };

            Vector3 _movement = _destination - transform.position;

            // Clamp position
            float _newBound;

            // X movement
            if (_movement.x != 0)
            {
                if (_movement.x < 0)
                {
                    _newBound = _destination.x - CameraXRatio;

                    if (_newBound < currentBounds.XMin)
                    {
                        _destination.x += CameraXRatio * 2 * camera.WorldToViewportPoint(currentBounds.XMinVector).x;

                        // Cancel movement if needed
                        if ((_destination.x - transform.position.x) < .0001f) _destination.x = transform.position.x;
                    }
                }
                else
                {
                    _newBound = _destination.x + CameraXRatio;

                    if (_newBound > currentBounds.XMax)
                    {
                        _destination.x -= CameraXRatio * 2 * (1 - camera.WorldToViewportPoint(currentBounds.XMaxVector).x);

                        // Cancel movement if needed
                        if ((transform.position.x - _destination.x) < .0001f) _destination.x = transform.position.x;
                    }
                }
            }
            // Y movement
            if (_movement.y != 0)
            {
                if (_movement.y < 0)
                {
                    _newBound = camera.WorldToViewportPoint(currentBounds.ZMinVector - _movement).y;

                    if (_newBound > 0)
                    {
                        _destination.y += camera.orthographicSize * 2 * VIEWPORT_CALCL_Y_COEF * camera.WorldToViewportPoint(currentBounds.ZMinVector).y;

                        // Cancel movement if needed
                        if ((_destination.y - transform.position.y) < .0001f) _destination.y = transform.position.y;
                    }
                }
                else
                {
                    _newBound = camera.WorldToViewportPoint(currentBounds.ZMaxVector - _movement).y;

                    if (_newBound < ViewportYMacBoundValue)
                    {
                        _destination.y -= camera.orthographicSize * 2 * VIEWPORT_CALCL_Y_COEF * (ViewportYMacBoundValue - camera.WorldToViewportPoint(currentBounds.ZMaxVector).y);

                        // Cancel movement if needed
                        if ((transform.position.y - _destination.y) < .0001f) _destination.y = transform.position.y;
                    }
                }
            }

            // Moves the camera if needed, or stop moving
            if (transform.position == _destination)
            {
                if (isMoving)
                {
                    isMoving = false;
                    SpeedCurrent = 0;
                }
            }
            else
            {
                transform.position = _destination;
            }
        }
    }

    /// <summary>
    /// Get newly set bounds to send online to other players.
    /// </summary>
    /// <returns></returns>
    public Vector4 GetSendingBounds()
    {
        Vector4 _bounds = onlineSendingBounds;
        onlineSendingBounds = new Vector4(-999, -999, -999, -999);

        return _bounds;
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
    /// Removes a player from following.
    /// </summary>
    /// <param name="_player">Player to remove.</param>
    public void RemoveLocalPlayer(TDS_Player _player)
    {
        players.Remove(_player);
    }

    /// <summary>
    /// Reset the level bounds.
    /// </summary>
    public void ResetBounds() => CurrentBounds = levelBounds;

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

        if (setBoundsCoroutine != null) StopCoroutine(setBoundsCoroutine);
        setBoundsCoroutine = StartCoroutine(SetBoundsInTime(_bounds));
    }

    /// <summary>
    /// Set bounds by values of online players.
    /// </summary>
    /// <param name="_bounds">New bounds value.</param>
    public void SetBoundsByOnline(Vector4 _bounds)
    {
        if ((_bounds.x > -999) && (_bounds.x != currentBounds.XMin))
        {
            leftBoundVector = new Vector3(_bounds.x, currentBounds.XMinVector.y, currentBounds.XMinVector.z);
            OnXMinBoundChanged?.Invoke();
        }
        if ((_bounds.y > -999) && (_bounds.y != currentBounds.XMax))
        {
            rightBoundVector = new Vector3(_bounds.y, currentBounds.XMaxVector.y, currentBounds.XMaxVector.z);
        }
        if ((_bounds.z > -999) && (_bounds.z != currentBounds.ZMin))
        {
            bottomBoundVector = new Vector3(currentBounds.ZMinVector.x, currentBounds.ZMinVector.y, _bounds.z);
        }
        if ((_bounds.w > -999) && (_bounds.w != currentBounds.ZMax))
        {
            topBoundVector = new Vector3(currentBounds.ZMaxVector.x, currentBounds.ZMaxVector.y, _bounds.w);
        }
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

        _boundsMovement[0] = _bounds.XMin > currentBounds.XMin ? 1 : _bounds.XMin < currentBounds.XMin ? - 1 : 0;

        _boundsMovement[1] = _bounds.XMax > currentBounds.XMax ? 1 : _bounds.XMax < currentBounds.XMax ? - 1 : 0;

        _boundsMovement[2] = _bounds.ZMin > currentBounds.ZMin ? 1 : _bounds.ZMin < currentBounds.ZMin ? - 1 : 0;

        _boundsMovement[3] = _bounds.ZMax > currentBounds.ZMax ? 1 : _bounds.ZMax < currentBounds.ZMax ? - 1 : 0;

        Vector3 _localPlayerPosition = new Vector3();
        Vector3[] _playerPositions = new Vector3[TDS_LevelManager.Instance.OtherPlayers.Count];

        bool _isFreezeX = (_boundsMovement[0] == 0) && (_boundsMovement[1] == 0);
        SetBoundObjectCollision(!_isFreezeX);

        yield return null;

        // While all the bounds are not in the right place, set their position
        while (_boundsMovement.Any(m => m != 0))
        {
            // Get all players position
            if (!PhotonNetwork.offlineMode)
            {
                _localPlayerPosition = TDS_LevelManager.Instance.LocalPlayer.transform.position;
                if (_playerPositions.Length != TDS_LevelManager.Instance.OtherPlayers.Count)
                {
                    _playerPositions = new Vector3[TDS_LevelManager.Instance.OtherPlayers.Count];
                }

                for (int _i = 0; _i < TDS_LevelManager.Instance.OtherPlayers.Count; _i++)
                {
                    _playerPositions[_i] = TDS_LevelManager.Instance.OtherPlayers[_i].transform.position;
                }
            }

            // Left bound move
            if (_boundsMovement[0] != 0)
            {
                if (PhotonNetwork.offlineMode || (_playerPositions.Length == 0) || _playerPositions.All(p => p.x > _localPlayerPosition.x))
                {
                    float _xMin = camera.WorldToViewportPoint(_bounds.XMinVector).x;
                    if (_xMin < .0001f)
                    {
                        leftBoundVector = _bounds.XMinVector;
                        _boundsMovement[0] = 0;

                        if (!_isFreezeX)
                        {
                            _isFreezeX = _boundsMovement[1] == 0;
                            if (_isFreezeX)
                            {
                                SetBoundObjectCollision(false);
                            }
                        }

                        OnXMinBoundChanged?.Invoke();

                        // Set X Min bound value to send online
                        onlineSendingBounds.x = currentBounds.XMin;
                    }
                    else
                    {
                        _xMin = transform.position.x - CameraXRatio;
                        if (_xMin != currentBounds.XMin)
                        {
                            leftBoundVector = new Vector3(_xMin, leftBound.position.y, leftBound.position.z);

                            OnXMinBoundChanged?.Invoke();

                            // Set X Min bound value to send online
                            onlineSendingBounds.x = currentBounds.XMin;
                        }
                    }
                }
                else if (currentBounds.XMin == _bounds.XMin)
                {
                    _boundsMovement[0] = 0;
                }
            }
            // Right bound move
            if (_boundsMovement[1] != 0)
            {
                if (PhotonNetwork.offlineMode || (_playerPositions.Length == 0) || _playerPositions.All(p => p.x < _localPlayerPosition.x))
                {
                    float _xMax = camera.WorldToViewportPoint(_bounds.XMaxVector).x;
                    if (_xMax > .9999f)
                    {
                        rightBoundVector = _bounds.XMaxVector;
                        _boundsMovement[1] = 0;

                        if (!_isFreezeX)
                        {
                            _isFreezeX = _boundsMovement[0] == 0;
                            if (_isFreezeX)
                            {
                                SetBoundObjectCollision(false);
                            }
                        }

                        // Set X Max bound value to send online
                        onlineSendingBounds.y = currentBounds.XMax;
                    }
                    else
                    {
                        _xMax = transform.position.x + CameraXRatio;
                        if (_xMax != currentBounds.XMax)
                        {
                            rightBoundVector = new Vector3(_xMax, rightBound.position.y, rightBound.position.z);

                            // Set X Max bound value to send online
                            onlineSendingBounds.y = currentBounds.XMax;
                        }
                    }
                }
                else if (currentBounds.XMax == _bounds.XMax)
                {
                    _boundsMovement[1] = 0;
                }
            }
            // Bottom bound move
            if (_boundsMovement[2] != 0)
            {
                if (PhotonNetwork.offlineMode || (_playerPositions.Length == 0) || _playerPositions.All(p => p.z > _localPlayerPosition.z))
                {
                    float _zMin = camera.WorldToViewportPoint(_bounds.ZMinVector).y;
                    if (_zMin < .0001f)
                    {
                        bottomBoundVector = _bounds.ZMinVector;
                        _boundsMovement[2] = 0;

                        // Set Z Min bound value to send online
                        onlineSendingBounds.z = currentBounds.ZMin;
                    }
                    else
                    {
                        _zMin = bottomBound.transform.position.z - (camera.orthographicSize * 2 * camera.WorldToViewportPoint(currentBounds.ZMinVector).y * VIEWPORT_CALCL_Y_COEF);
                        if (_zMin != currentBounds.ZMin)
                        {
                            bottomBoundVector = new Vector3(bottomBound.position.x, bottomBound.position.y, _zMin);

                            // Set Z Min bound value to send online
                            onlineSendingBounds.z = currentBounds.ZMin;
                        }
                    }
                }
                else if (currentBounds.ZMin == _bounds.ZMin)
                {
                    _boundsMovement[2] = 0;
                }
            }
            // Top bound move
            if (_boundsMovement[3] != 0)
            {
                if (PhotonNetwork.offlineMode || (_playerPositions.Length == 0) || _playerPositions.All(p => p.z < _localPlayerPosition.z))
                {
                    float _mostUpPlayer = PhotonNetwork.offlineMode ? players.OrderBy(p => p.transform.position.z).Last().transform.position.z : target.position.z;

                    float _zMax = camera.WorldToViewportPoint(_bounds.ZMaxVector).y;
                    if ((_zMax > (ViewportYMacBoundValue - .0001f)) && (_mostUpPlayer + 1 < _bounds.ZMax))
                    {
                        topBoundVector = _bounds.ZMaxVector;
                        _boundsMovement[3] = 0;

                        // Set Z Max bound value to send online
                        onlineSendingBounds.w = currentBounds.ZMax;
                    }
                    else
                    {
                        _zMax = topBound.transform.position.z + (camera.orthographicSize * 2 * (ViewportYMacBoundValue - camera.WorldToViewportPoint(currentBounds.ZMaxVector).y) * VIEWPORT_CALCL_Y_COEF);

                        if ((_zMax != currentBounds.ZMax) && (_mostUpPlayer + 1 < _zMax))
                        {
                            topBoundVector = new Vector3(topBound.position.x, topBound.position.y, _zMax);

                            // Set Z Max bound value to send online
                            onlineSendingBounds.w = currentBounds.ZMax;
                        }
                    }
                }
                else if (currentBounds.ZMax == _bounds.ZMax)
                {
                    _boundsMovement[3] = 0;
                }
            }

            yield return null;
        }

        OnBoundFreeze?.Invoke();
        setBoundsCoroutine = null;
    }


    private int boundLayer = 0;
    private int objectLayer = 0;
    private void SetBoundObjectCollision(bool _doIgnore)
    {
        Physics.IgnoreLayerCollision(boundLayer, objectLayer, _doIgnore);
    }


    /// <summary>
    /// Calculates and set the Camera view aspect based on target ratio.
    /// </summary>
    public void SetCameraAspect()
    {
        // Set aspect ratio in 16/9
        camera.rect = TDS_GameManager.CameraRect;

        // Get X ratio
        CameraXRatio = camera.orthographicSize * (((float)TDS_GameManager.CurrentResolution.width / TDS_GameManager.CurrentResolution.height) / (camera.rect.height / camera.rect.width));
    }

    /// <summary>
    /// Set camera settings for local multiplayer.
    /// </summary>
    public void SetLocalMultiplayerCamera()
    {
        players = TDS_LevelManager.Instance.AllPlayers.ToList();
    }

    /// <summary>
    /// Make a screen shake of a specified force.
    /// </summary>
    /// <param name="_force">Screen shake force.</param>
    public void StartScreenShake(float _force, float _time = 0)
    {
        StartCoroutine(ScreenShake(_force, _time));
    }

    /// <summary>
    /// Make a screen shake of a specified force.
    /// </summary>
    /// <param name="_force">Screen shake force.</param>
    /// <param name="_time">Total duration of the screen shake.</param>
    public IEnumerator ScreenShake(float _force, float _time)
    {
        if (_time == 0) _time = .0001f;

        while (_time > 0)
        {
            yield return new WaitForEndOfFrame();

            Vector3 _force3 = ((Vector3)Random.insideUnitCircle.normalized) * _force;
            transform.position += _force3;

            _time -= Time.deltaTime;
        }
    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Set the singleton instance if null
        if (!Instance) Instance = this;
        else
        {
            Destroy(this);
            return;
        }

        // Debug warning is missing bound(s)
        if (!topBound || !leftBound || !rightBound || !bottomBound)
        {
            Debug.LogWarning($"Missing bound(s) for the Camera \"{name}\"");
        }

        // Calculate aspect
        SetCameraAspect();
    }

    // LateUpdate is called every frame, if the Behaviour is enabled
    private void LateUpdate()
    {
        // If camera moved on X axis, call related event
        float _xMovement = transform.position.x - previousPosition.x;
        if (_xMovement != 0) OnMoveX?.Invoke(_xMovement);

        // Set position as previous one
        previousPosition = transform.position;
    }

    // Destroying the attached Behaviour will result in the game or Scene receiving OnDestroy
    private void OnDestroy()
    {
        // Nullify the singleton instance if needed
        if (Instance == this) Instance = null;
    }

    // Use this for initialization
    private void Start ()
    {
        // Set default level bounds
        levelBounds = new TDS_Bounds(leftBound.position, rightBound.position, bottomBound.position, topBound.position);
        currentBounds = levelBounds;

        // Set previous position as current one
        previousPosition = transform.position;

        // Set the camera position between bounds
        ClampInBounds();

        boundLayer = LayerMask.NameToLayer("Bound");
        objectLayer = LayerMask.NameToLayer("Object");

        TDS_GameManager.SetMainCamera(camera);
    }
	
	// Update is called once per frame
	private void Update ()
    {
        if (!PhotonNetwork.offlineMode) FollowTarget();
        else FollowPlayers();
	}
	#endregion

	#endregion
}
