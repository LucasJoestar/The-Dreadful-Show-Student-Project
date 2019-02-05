using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class TDS_Juggler : TDS_Player 
{
    /* TDS_Juggler :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Script of the Juggler controller. Now, you can play him. Yes you can.
     *	There should be only one juggler in by scene.
     *	    Just add this script to an object to make it behaviour as the Juggler,
     *	and be able to play with him.
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[04 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_Juggler class.
	 *
	 *	-----------------------------------
	*/

    #region Events

    #endregion

    #region Fields / Properties

    #region Throwables
    /// <summary>
    /// Index of the current selected throwable from <see cref="Throwables"/>.
    /// </summary>
    private int selectedThrowableIndex = 0;

    /// <summary>
    /// The actual selected throwable. It is the object to throw, when throwing... Yep.
    /// </summary>
    public TDS_Throwable SelectedThrowable
    {
        get { return Throwables[selectedThrowableIndex]; }
    }

    /// <summary>
    /// All throwables the juggler has in hands.
    /// </summary>
    public List<TDS_Throwable> Throwables = new List<TDS_Throwable>();

    /// <summary>
    /// Positions used to preview the preparing throw trajectory.
    /// </summary>
    private List<Vector3> throwPreviewTrajectory = new List<Vector3>();
    #endregion

    #endregion

    #region Methods

    #region Original Methods
    /// <summary>
    /// Make the juggler aim for a throw. When releasing the thorw button, throw the selected object.
    /// If the cancel throw button is pressed, cancel the throw, as it name indicate it.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator Aim()
    {
        yield break;
    }

    /// <summary>
    /// Draws the preview trajectory of the juggler throw, when aiming.
    /// </summary>
    private void DrawPreviewTrajectory()
    {

    }
    #endregion

    #region Unity Methods
    // Awake is called when the script instance is being loaded
    protected override void Awake()
    {

    }

    // Use this for initialization
    protected override void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        
	}
	#endregion

	#endregion
}
