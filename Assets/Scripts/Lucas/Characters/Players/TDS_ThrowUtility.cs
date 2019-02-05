using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDS_ThrowUtility 
{
	/* TDS_ThrowUtility :
	 *
	 *	#####################
	 *	###### PURPOSE ######
	 *	#####################
	 *
	 *	References utility methods for throw in the game
	 *
	 *	#####################
	 *	### MODIFICATIONS ###
	 *	#####################
	 *
	 *	Date :			[05 / 02 / 2019]
	 *	Author :		[Guibert Lucas]
	 *
	 *	Changes :
	 *
	 *	Creation of the TDS_ThrowUtility class.
     *	
     *	    - Added the GetThrowMotionPoints & GetProjectileVelocityAsVector3 methods.
	 *
	 *	-----------------------------------
	*/

	#region Methods
    /// <summary>
    /// Get motion point of a projectile trajectory.
    /// </summary>
    /// <param name="_from">Position from which to throw.</param>
    /// <param name="_to">Position where the projectile should land.</param>
    /// <param name="_velocity">Velocity used to throw the projectile.</param>
    /// <param name="_angle">Angle used to throw the projectile.</param>
    /// <param name="_pointsAmount">Amount of points to return. The more they are, the more precise it is.</param>
    /// <returns></returns>
    public static Vector3[] GetThrowMotionPoints(Vector3 _from, Vector3 _to, float _velocity, float _angle, int _pointsAmount)
    {
        // Create and set variables
        if (_pointsAmount < 3) _pointsAmount = 3;

        Vector3[] _motionPoints = new Vector3[_pointsAmount];
        _motionPoints[0] = _from; _motionPoints[_pointsAmount - 1] = _to;

        // Get distance between in each point in x, z & on both
        float _xPointsDistance = (_to.x - _from.x) / _pointsAmount;
        float _zPointsDistance = (_to.z - _from.z) / _pointsAmount;
        float _xzPointsDistance = Mathf.Sqrt(Mathf.Pow(_xPointsDistance, 2) + Mathf.Pow(_zPointsDistance, 2));

        // Get each position
        for (int _i = 1; _i < _pointsAmount - 1; _i++)
        {
            float _t = (_xzPointsDistance * _i) / (_velocity * Mathf.Cos(_angle));
            float _x = _xPointsDistance * _i;
            float _z = _zPointsDistance * _i;
            float _y = -.5f * Physics.gravity.magnitude * Mathf.Pow(_t, 2) + _velocity * Mathf.Sin(_angle) * _t;

            _motionPoints[_i] = _from + new Vector3(_x, _y, _z);
        }

        return _motionPoints;
    }

    /// <summary>
    /// Get the velocity of a projectile motion.
    /// <param name="_from">Position from which to throw.</param>
    /// <param name="_to">Position where the projectile should land.</param>
    /// <param name="_angle">Angle used to throw the projectile.</param>
    /// <returns>Returns the velocity as a Vector3.</returns>
    public static Vector3 GetProjectileVelocityAsVector3(Vector3 _from, Vector3 _to, float _angle)
    {
        // Get the distance between the two points on x & z
        float _xzDistance = Mathf.Sqrt(Mathf.Pow(_to.x - _from.x, 2) + Mathf.Pow(_to.z - _from.z, 2));

        float _yOffset = _from.y - _to.y;

        // Calculates the initial velocity
        float _initVelocity = (1 / Mathf.Cos(_angle)) * Mathf.Sqrt(.5f * Physics.gravity.magnitude * Mathf.Pow(_xzDistance, 2) / (_xzDistance * Mathf.Tan(_angle) + _yOffset));

        // Get the velocity of the object as a Vector3 with a default orientation
        Vector3 _velocity = new Vector3(0, _initVelocity * Mathf.Sin(_angle), _initVelocity * Mathf.Cos(_angle));

        float _motionAngle = Vector3.Angle(Vector3.forward, new Vector3(_to.x - _from.x, 0, _to.z - _from.z));

        if (_to.x < _from.x) _motionAngle = -_motionAngle;

        // Get the velocity with the good orientation
        _velocity = Quaternion.AngleAxis(_motionAngle, Vector3.up) * _velocity;

        return _velocity;
    }
    #endregion
}
