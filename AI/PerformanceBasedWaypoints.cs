using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceBasedWaypoints : MonoBehaviour {

	public Transform[] waypoints;
	public MaxMinWaypoint[] easyHardWaypoints;

	// Adjusts the waypoint location depending on the difficulty setting for the AI.
	public void CalculateWaypoints(int difficulty)
    {
		for (int i = 0; i < easyHardWaypoints.Length; i++)
        {
			waypoints[i].position = easyHardWaypoints[i].easyWaypoint.position;

			float maxWaypointX = easyHardWaypoints[i].easyWaypoint.position.x;
			float maxWaypointY = easyHardWaypoints[i].easyWaypoint.position.y;
			float minWaypointX = easyHardWaypoints[i].hardWaypoint.position.x;
			float minWaypointY = easyHardWaypoints[i].hardWaypoint.position.y;
            
            // Calculating where the waypoint should be between a range of 2 points in space.
            // This allows the waypoint to adjust on the fly based on which difficulty is set for the AI.
            // A higher range = higher AI difficulty, lower range = lower AI difficulty.
			float initXValue = minWaypointX - maxWaypointX;
			float initYValue = minWaypointY - maxWaypointY;
			float initDistance = Vector2.Distance(easyHardWaypoints[i].easyWaypoint.position, easyHardWaypoints[i].hardWaypoint.position);
            float initThetaRad = Mathf.Atan2(initYValue, initXValue);         

			float percentage = 0;
			switch (difficulty)
			{
				case 0:
					percentage = 0.0f;
					break;
				case 1:
					percentage = 0.5f;
					break;
				case 2:
                    percentage = 1.0f;
					break;
			}

			// Calculating the new x and y values of the waypoint.
			float newDistance = initDistance * percentage;
            initThetaRad = Mathf.Abs(initThetaRad);
            float newYValue = (newDistance * Mathf.Sin(initThetaRad)) / Mathf.Sin(90 * Mathf.Deg2Rad);          

			if (initYValue < 0)
            {
                newYValue = -newYValue;            
            }

            float newXValue = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(newDistance, 2) - Mathf.Pow(newYValue, 2)));

			if (initXValue < 0)
            {
                newXValue = -newXValue;            
            }

			waypoints[i].position = new Vector2(waypoints[i].position.x + newXValue, waypoints[i].position.y + newYValue);
		}
    }
}

[System.Serializable]
public class MaxMinWaypoint
{
	public Transform hardWaypoint;
	public Transform easyWaypoint;
}