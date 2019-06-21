using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class TDS_CloudsBehaviour : MonoBehaviour 
{
    /* TDS_CloudsBehaviour :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	Manages clouds movement over time.
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
	 *	Date :			[21 / 06 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_CloudsBehaviour class.
	 *
	 *	-----------------------------------
	*/

    #region Fields / Properties
    /// <summary>
    /// Maximum speed of a cloud movement.
    /// </summary>
    [SerializeField] private float maxSpeed = 3;

    /// <summary>
    /// Minimum speed of a cloud movement.
    /// </summary>
    [SerializeField] private float minSpeed = 1;

    /// <summary>
    /// Maximum X value used for the clouds bounds.
    /// </summary>
    [SerializeField] float rightBound = 1;

    /// <summary>
    /// Minimum X value used for the clouds bounds.
    /// </summary>
    [SerializeField] float leftBound = -1;

    /// <summary>
    /// Clouds, with transform as key and speed as value.
    /// </summary>
    private Dictionary<Transform, float> clouds = new Dictionary<Transform, float>();
    #endregion

    #region Unity Methods
    // Implement OnDrawGizmosSelected to draw a gizmo if the object is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawSphere(new Vector3(transform.position.x + leftBound, transform.position.y, transform.position.z), .25f);

        Gizmos.color = Color.red;

        Gizmos.DrawSphere(new Vector3(transform.position.x + rightBound, transform.position.y, transform.position.z), .25f);

        Gizmos.color = Color.white;
    }

    // Use this for initialization
    private void Start()
    {
        if (rightBound < leftBound)
        {
            float _left = rightBound;
            rightBound = leftBound;
            leftBound = _left;
        }

        for (int _i = 0; _i < transform.childCount; _i++)
        {
            clouds.Add(transform.GetChild(_i), Random.Range(minSpeed, maxSpeed));
        }
    }
	
	// Update is called once per frame
	private void Update()
    {
        foreach (KeyValuePair<Transform, float> _cloud in clouds)
        {
            _cloud.Key.position = Vector3.Lerp(_cloud.Key.position, _cloud.Key.position + Vector3.left, Time.deltaTime * _cloud.Value);

            if (_cloud.Key.position.x < (transform.position.x + leftBound)) _cloud.Key.position = new Vector3(transform.position.x + rightBound, _cloud.Key.position.y, _cloud.Key.position.z);
        }
	}
	#endregion
}
