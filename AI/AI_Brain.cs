using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Brain : MonoBehaviour {

    [Header("-----/// Scriptable Objects ///-----")]
    public SO_AI_PerformanceStats performanceStats;
    public SO_CurrentTrackInfo currentTrackInfo;

    [Space]
    [Header("-----/// Components ///-----")]
    public Transform goal;
    public Rigidbody2D rb2d;

    [Space]
    [Header("-----/// Game Objects ///-----")]
	public GameObject aiMissileParent;

    [Space]
    [Header("-----/// Variables ///-----")]
	public AnimationCurve turnCurve;

	Vector2 thrust;
    float thrustPower;
	float maxSpeed;
    float rotationSpeed;
    Vector3 rotation;   
	public bool launch;
    public bool thrusting;
	GameObject[] difficultyWaypoints;     
	public float leftTurnCurveValue;
	public float rightTurnCurveValue;
	bool turning;
	float angle;
	Vector2 targetPos;
	Vector2 thisPos;
	Vector3 direction;

	// Initialize missile physics.
    private void Awake()
    {
        rb2d.centerOfMass = Vector2.zero;
        rb2d.inertia = 1.5f;
        SetMechanics();
    }

    void FixedUpdate () {
		if (launch)
		{
            FlyingMissile();
		}      
	}

    public void SetPerformanceStats()
	{
        int performanceIndex = currentTrackInfo.performanceIndex;
        rotationSpeed = performanceStats.performanceLevels[performanceIndex].rotationSpeed;
        maxSpeed = performanceStats.performanceLevels[performanceIndex].maxSpeed;
        thrustPower = performanceStats.performanceLevels[performanceIndex].thrustPower;

		thrust = new Vector2(thrustPower, 0);
		rotation = new Vector3(0, 0, rotationSpeed);
	}

    public void ActivateMissileLaunch()
	{
        ActivateMissileFlying();
		rb2d.AddRelativeForce(Vector3.right * 330, ForceMode2D.Impulse);
	}

	public void ActivateMissileFlying() {
        launch = true;
        thrusting = true;
        rb2d.isKinematic = false;      
    }

    public void DeactivateMissileFlying()
	{
		launch = false;
		thrusting = false;
		rb2d.isKinematic = true;
		rb2d.velocity = Vector3.zero;
        rb2d.angularVelocity = 0;
	}

    void FlyingMissile()
	{      
		// Calculates the rotation of the missile to look at the waypoint.
		targetPos = goal.position;
        thisPos = rb2d.transform.position;
        targetPos.x = targetPos.x - thisPos.x;
        targetPos.y = targetPos.y - thisPos.y;
        angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;

        direction = goal.position - transform.position;

        Vector3 currentAngle = new Vector3(0, 0, Mathf.LerpAngle(transform.eulerAngles.z, angle, Time.deltaTime * rotationSpeed));
        transform.eulerAngles = currentAngle;

        // Maxes out top speed of missile
        if (thrusting)
        {
            rb2d.AddRelativeForce(thrust);
        }

        if (rb2d.velocity.magnitude > maxSpeed)
        {
            rb2d.velocity = rb2d.velocity.normalized * maxSpeed;
        }
	}

	bool VectorAIsRight(Vector2 a, Vector2 b) {
        return -a.x * b.y + a.y * b.x < 0;
    }

	private void OnTriggerStay2D(Collider2D col)
	{
		if (col.gameObject.CompareTag("AIKillThrusters"))
		{
			thrusting = false;
		}
	}

	private void OnTriggerExit2D(Collider2D col)   
	{
		if (col.gameObject.CompareTag("AIKillThrusters"))
		{
			thrusting = true;
		}
	}

    void SetMechanics()
    {
        string mechanics = currentTrackInfo.gameMechanics;
        switch (mechanics)
        {
            case "NORMAL":
                gameObject.layer = 9;
                break;
            case "INSANITY":
                gameObject.layer = 21;
                break;
        }

        if (currentTrackInfo.gameMode == "DEFENDER")
        {
            gameObject.layer = 21; 
        }
    }
}