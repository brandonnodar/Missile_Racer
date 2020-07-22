using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garage_AutoLander : MonoBehaviour
{
    [Header("-----/// VARIABLES ///-----")]
    public Vector2 landingPosition;
    public Vector3 landingRotation;

    [Space]
    [Header("-----/// CLASSES ///-----")]
    public Player_Manager playerManager;

    [Space]
    [Header("-----/// GAME EVENTS ///-----")]
    public GameEvent landingComplete;

    float wait = 4.0f;
    WaitForSeconds waitTime;

    private void Awake()
    {
        waitTime = new WaitForSeconds(wait);
    }

    public void LandPlayerMissile()
    {
        playerManager.flying.DeactivateMissileFlying();
        playerManager.thrusters.SetThrusterEnableUsage(false);
        playerManager.position.SmoothToPosition(landingPosition, landingRotation);
        StartCoroutine(InitiateLandingSequence());
    }

    public void AllowMissileFlying()
    {
        StopAllCoroutines();
        playerManager.thrusters.SetThrusterEnableUsage(true);
        playerManager.flying.ActivateMissileFlying();
        playerManager.position.StopGarageSmoothPosition();
    }

    IEnumerator InitiateLandingSequence()
    {
        playerManager.thrusters.PlayThrusters();
        yield return waitTime;
        playerManager.thrusters.StopThrusters();

        landingComplete.Raise();
    }
}
