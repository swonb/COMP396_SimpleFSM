using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFSM : MonoBehaviour
{
    public enum EnemySimpleFSM
    {
        Seek_Waypoint,
        Chase,
        Attack,
        Evade
    }

    public EnemySimpleFSM currentState;
    public GameObject player;
    public float distanceToChase = 10;  // chase if < 10m away
    public bool InFront;                // if and (fwd, player) <= 90 deg (or FOV/2)
    public float distanceToAttack = 2;  // attack if <= 2m away

    public float FOV = 60;              // 60 deg Field of View

    // New on Oct. 2nd
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

    // Start is called before the first frame update
    void Start()
    {
        FOV_in_RAD = Deg2Rad(FOV);
        csCosFOV_2 = Mathf.Cos(FOV_in_RAD / 2);
    }

    void TransitionTo(EnemySimpleFSM newState)
    {
        currentState = newState;
    }

    void SeekWaypoint()
    {
        // Enemy transition to Chase if it is:
        // < 10m away and ...
        // within FOV=60 deg.
        //if (!ReadyToChase() && !Attacking()) { }

        Vector3 playerHeading = player.transform.position - this.transform.position;
        float distanceToPlayer = playerHeading.magnitude;
        //Vector3 directionToPlayer = playerHeading / distanceToPlayer;    // unit vector
        Vector3 directionToPlayer = playerHeading.normalized;    // unit vector

        //InFront = (Vector3.Dot(this.transform.forward, directionToPlayer) > 0);
        float csFwd2Ply = Vector3.Dot(this.transform.forward, directionToPlayer);
        float angleFwd2Ply_in_RAD = Mathf.Acos(csFwd2Ply);
        float angleFwd2Ply_in_DEG = Rad2Deg(angleFwd2Ply_in_RAD);

        InFront = (Vector3.Dot(this.transform.forward, directionToPlayer) >= csCosFOV_2);   // visible

        if (InFront)
        {
            if (distanceToPlayer <= distanceToAttack)
            {
                Attack();
            }
            else if (distanceToPlayer <= distanceToChase)
            {
                Chase();
            }

            return;
        }

        Debug.Log("Seek waypoint");
    }

    void Chase()
    {
        //Debug.Log("Chase");

        // New on Oct. 2nd

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
    }

    void Attack()
    {
        Debug.Log("Attack");
    }

    void Evade()
    {
        Debug.Log("Evade");
    }

    // Update is called once per frame
    void Update()
    {
        FSM();
    }

    void FSM()
    {
        switch(currentState)
        {
            case EnemySimpleFSM.Seek_Waypoint:

                SeekWaypoint();
                break;
            case EnemySimpleFSM.Chase:
                Chase();
                break;
            case EnemySimpleFSM.Attack:
                Attack();
                break;
            case EnemySimpleFSM.Evade:
                Evade();
                break;
            default:
                break;
        }
    }
}
