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

    /// <summary>
    /// Base bounds for entire level ; when CurrentBounds are set to null, set the this instead.
    /// </summary>
    [SerializeField] private TDS_Bounds levelBounds = new TDS_Bounds(new Vector2(-10, 10), new Vector2(-10, 10));

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
                target = value;
            }
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
    /// Makes this camera follow its target.
    /// </summary>
    private void FollowTarget()
    {
        // If no target, return
        if (!Target) return;

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

            // Moves the camera
            Vector3 _destination = target.transform.position + Offset;
            _destination.x = Mathf.Clamp(_destination.x, levelBounds.XBounds.x, levelBounds.XBounds.y);
            _destination.z = Mathf.Clamp(_destination.z, levelBounds.ZBounds.x, levelBounds.ZBounds.y);

            transform.position = Vector3.Lerp(transform.position, _destination, Time.deltaTime * speedCurrent * speedCoef);
        }
    }

    /// <summary>
    /// Set new bounds for the camera.
    /// </summary>
    /// <param name="_bounds">New bounds of the camera.</param>
    public void SetBounds(TDS_Bounds _bounds)
    {
        if (_bounds != null) currentBounds = _bounds;
        else currentBounds = levelBounds;
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

        Gizmos.DrawCube(new Vector3(levelBounds.XBounds.x, transform.position.y, levelBounds.ZBounds.x), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XBounds.x, transform.position.y, levelBounds.ZBounds.x), new Vector3(levelBounds.XBounds.y, transform.position.y, levelBounds.ZBounds.x));
        Gizmos.DrawCube(new Vector3(levelBounds.XBounds.x, transform.position.y, levelBounds.ZBounds.y), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XBounds.x, transform.position.y, levelBounds.ZBounds.x), new Vector3(levelBounds.XBounds.x, transform.position.y, levelBounds.ZBounds.y));
        Gizmos.DrawCube(new Vector3(levelBounds.XBounds.y, transform.position.y, levelBounds.ZBounds.x), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XBounds.x, transform.position.y, levelBounds.ZBounds.y), new Vector3(levelBounds.XBounds.y, transform.position.y, levelBounds.ZBounds.y));
        Gizmos.DrawCube(new Vector3(levelBounds.XBounds.y, transform.position.y, levelBounds.ZBounds.y), Vector3.one * .25f);
        Gizmos.DrawLine(new Vector3(levelBounds.XBounds.y, transform.position.y, levelBounds.ZBounds.y), new Vector3(levelBounds.XBounds.y, transform.position.y, levelBounds.ZBounds.x));
    }

    // Use this for initialization
    private void Start ()
    {
        if (currentBounds == null) currentBounds = levelBounds;
    }
	
	// Update is called once per frame
	private void Update ()
    {
        FollowTarget();
	}
	#endregion

	#endregion
}
