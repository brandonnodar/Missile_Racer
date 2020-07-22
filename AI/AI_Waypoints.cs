using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class AI_Waypoints : MonoBehaviour
{
    public enum EnemyAxis { UseYAxis, UseXAxis };
    public EnemyAxis enemyAxis;

    public WaypointCircuit[] allWaypointCircuits;
    public WaypointProgressTracker waypointProgressTracker;

    public SO_EnemyData enemyData;

    private GameObject enemyLastWaypoint;
    private Vector2 initPosLastWaypoint;

    [HideInInspector] public WaypointCircuit waypointCircuitToUse;

    public void InitializeLastWaypoint()
    {
        enemyLastWaypoint = waypointCircuitToUse.waypointList.items[waypointCircuitToUse.waypointList.items.Length - 1].gameObject;
        initPosLastWaypoint = enemyLastWaypoint.transform.position;
    }

    // Sets the waypoints to use based on missile performance, and difficulty.
    public void Setup(int performanceIndex, int difficulty)
    {
        int circuitToUse = CircuitToUse(performanceIndex, difficulty);
        for (int i = 0; i < allWaypointCircuits.Length; i++)
        {
            if (i == circuitToUse)
                allWaypointCircuits[i].gameObject.SetActive(true);
            else
                allWaypointCircuits[i].gameObject.SetActive(false); 
        }

        waypointCircuitToUse = allWaypointCircuits[circuitToUse];
        waypointProgressTracker.circuit = waypointCircuitToUse;
    }

    // Adjust the position of the last waypoint based on where enemy is located so missile can hit the enemy.
    public void EditEnemyLastWaypoint()
    {
        if (enemyAxis == EnemyAxis.UseYAxis)
        {
            enemyLastWaypoint.transform.position = new Vector2(enemyLastWaypoint.transform.position.x, enemyData.enemyPos.y);
        } else if (enemyAxis == EnemyAxis.UseXAxis)
        {
            enemyLastWaypoint.transform.position = new Vector2(enemyData.enemyPos.x, enemyLastWaypoint.transform.position.y);
        }
        waypointCircuitToUse.RecalculateWaypoints();
    }

    public void ResetEnemyLastWaypoint()
    {
        enemyLastWaypoint.transform.position = initPosLastWaypoint;
        waypointCircuitToUse.RecalculateWaypoints();
    }

    int CircuitToUse(int performanceIndex, int difficulty)
    {
        switch(performanceIndex)
        {
            case 0:
                if (difficulty == 0)
                    return 0;
                else if (difficulty == 1)
                    return 1;
                else if (difficulty == 2)
                    return 2;
                else
                    return 0;
            case 1:
                if (difficulty == 0)
                    return 3;
                else if (difficulty == 1)
                    return 4;
                else if (difficulty == 2)
                    return 5;
                else
                    return 0;
            case 2:
                if (difficulty == 0)
                    return 6;
                else if (difficulty == 1)
                    return 7;
                else if (difficulty == 2)
                    return 8;
                else
                    return 0;
            case 3:
                if (difficulty == 0)
                    return 9;
                else if (difficulty == 1)
                    return 10;
                else if (difficulty == 2)
                    return 11;
                else
                    return 0;
            case 4:
                if (difficulty == 0)
                    return 12;
                else if (difficulty == 1)
                    return 13;
                else if (difficulty == 2)
                    return 14;
                else
                    return 0;
            default:
                return 0;
        }
    }
}
