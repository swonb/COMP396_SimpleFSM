using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Demonstrating the use of the StateMachine class
/// New in Oct.06
/// </summary>
public class ClassBasedFSM : MonoBehaviour
{
    StateMachine stateMachine;

    //--- From SimpleFSM ---//
    public GameObject player;
    public float distanceToChase = 10;  // chase if < 10m away
    public bool InFront;                // if and (fwd, player) <= 90 deg (or FOV/2)
    public float distanceToAttack = 2;  // attack if <= 2m away

    public float FOV = 60;              // 60 deg Field of View

    public float speed = 1f;
    public bool strongerThanPlayer = true;
    private float FOV_in_RAD;
    public float csCosFOV_2;

    public float Deg2Rad(float deg)
    {
        return deg / 180 * Mathf.PI;
    }

    public float Rad2Deg(float rad)
    {
        return rad * 180 / Mathf.PI;
    }
    //------//

    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new StateMachine();

        // Seek_Waypoint state
        var seekWaypoint = stateMachine.CreateState("Seek_Waypoint");
        seekWaypoint.onEnter = delegate
        {
            Debug.Log("In Seek_Waypoint.onEnter");
        };
        seekWaypoint.onStay = delegate
        {
            //Debug.Log("In Seek_Waypoint.onStay");

            // Enemy transition to Chase if it is:
            // < 10m away and ...
            // within FOV=60 deg.
            //if (!ReadyToChase() && !Attacking()) { }

            Vector3 playerHeading = player.transform.position - this.transform.position;
            float distanceToPlayer = playerHeading.magnitude;
            //Vector3 directionToPlayer = playerHeading / distanceToPlayer;    // unit vector
            Vector3 directionToPlayer = playerHeading.normalized;    // unit vector

            //InFront = (Vector3.Dot(this.transform.forward, directionToPlayer) > 0);   // Not using FOV, only InFront?
            float csFwd2Ply = Vector3.Dot(this.transform.forward, directionToPlayer);
            float angleFwd2Ply_in_RAD = Mathf.Acos(csFwd2Ply);
            float angleFwd2Ply_in_DEG = Rad2Deg(angleFwd2Ply_in_RAD);

            InFront = (Vector3.Dot(this.transform.forward, directionToPlayer) >= csCosFOV_2);   // visible, using FOV

            if (InFront)
            {
                if (distanceToPlayer <= distanceToAttack)
                {
                    stateMachine.TransitionTo("Attack");
                }
                else if (distanceToPlayer <= distanceToChase)
                {
                    if (strongerThanPlayer)
                    {
                        stateMachine.TransitionTo("Chase");
                    }
                    else
                    {
                        stateMachine.TransitionTo("Evade");
                    }
                }
            }
        };
        seekWaypoint.onExit = delegate
        {
            Debug.Log("In Seek_Waypoint.onExit");
        };

        // Chase state
        var chase = stateMachine.CreateState("Chase");
        chase.onEnter = delegate
        {
            Debug.Log("In Chase.onEnter");
        };
        chase.onStay = delegate
        {
            //Debug.Log("In Chase.onStay");

            // E => Enemy's Position
            // P => Player's Position
            // Heading => Vector from the Enemy to Player
            // HeadingDir => Normalized Heading
            // E.pos += headingDir * enemySpeed * deltaTime

            Vector3 E = this.transform.position;
            Vector3 P = player.transform.position;
            Vector3 Heading = P - E;    // From E to P
            Vector3 HeadingDir = Heading.normalized; // H/|H|

            this.transform.position += HeadingDir * speed * Time.deltaTime; //
        };
        chase.onExit = delegate
        {
            Debug.Log("In Chase.onExit");
        };

        // Attack state
        var attack = stateMachine.CreateState("Attack");
        attack.onEnter = delegate
        {
            Debug.Log("In Attack.onEnter");
        };
        attack.onStay = delegate
        {
            //Debug.Log("In Attack.onStay");
        };
        attack.onExit = delegate
        {
            Debug.Log("In Attack.onExit");
        };

        // Evade state
        var evade = stateMachine.CreateState("Evade");
        evade.onEnter = delegate
        {
            Debug.Log("In Evade.onEnter");
        };
        evade.onStay = delegate
        {
            //Debug.Log("In Evade.onStay");

            // E => Enemy's Position
            // P => Player's Position
            // Heading => Vector from the Enemy to Player
            // HeadingDir => Normalized Heading
            // E.pos += headingDir * enemySpeed * deltaTime

            Vector3 E = this.transform.position;
            Vector3 P = player.transform.position;
            Vector3 Heading = E - P;    // From E to P
            Vector3 HeadingDir = Heading.normalized; // H/|H|

            this.transform.position += HeadingDir * speed * Time.deltaTime; //
        };
        evade.onExit = delegate
        {
            Debug.Log("In Evade.onExit");
        };
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}
