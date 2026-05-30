using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class AIController : MonoBehaviour
{
    [Header("Movement Settings")]
    public NavMeshAgent navMeshAgent;
    public float startWaitTime = 4f;
    public float timeToRotate = 2f;
    public float speedWalk = 6f;
    public float speedRun = 9f;

    [Header("Vision Settings")]
    public float viewRadius = 15f;
    public float viewAngle = 90f;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    public float meshResolution = 1f;
    public int edgeIterations = 4;
    public float edgeDistance = 0.5f;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    int m_CurrentWaypointIndex;

    // Private State Variables
    Vector3 playerLastPosition = Vector3.zero;
    Vector3 m_PlayerPosition;

    float m_WaitTime;
    float m_TimeToRotate;
    bool m_PlayerInRange;
    bool m_PlayerNear;
    bool m_IsPatrol;
    bool m_CaughtPlayer;

    // Performance Optimization Variable
    private Transform playerTransform;

    void Start()
    {
        m_PlayerPosition = Vector3.zero;
        m_IsPatrol = true;
        m_CaughtPlayer = false;
        m_PlayerInRange = false;
        m_WaitTime = startWaitTime;
        m_TimeToRotate = timeToRotate;

        m_CurrentWaypointIndex = 0;
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Cache the player transform safely at start so we don't spam lookups
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speedWalk;

        SetWaypointDestination();
    }

    void Update()
    {
        EnviromentView();

        if (!m_IsPatrol)
        {
            Chasing();
        }
        else
        {
            Patroling();
        }
    }

    private void Chasing()
    {
        if (playerTransform == null)
        {
            m_IsPatrol = true;
            return;
        }

        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)
        {
            Move(speedRun);

            // Lock onto the player's live position during a chase
            m_PlayerPosition = playerTransform.position;
            navMeshAgent.SetDestination(m_PlayerPosition);
        }

        // Check if the AI reached the last recorded position of the player
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            // If we are close to the destination, we haven't caught them, they are far away, and we lost line of sight
            if (m_WaitTime <= 0 && !m_CaughtPlayer && Vector3.Distance(transform.position, playerTransform.position) >= 6f && !m_PlayerInRange)
            {
                m_IsPatrol = true;
                m_PlayerNear = false;
                Move(speedWalk);
                m_TimeToRotate = timeToRotate;
                m_WaitTime = startWaitTime;
                SetWaypointDestination();
            }
            else
            {
                // If the player is still relatively near but breaking visual tracking loops
                if (Vector3.Distance(transform.position, playerTransform.position) >= 2.5f)
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Patroling()
    {
        if (m_PlayerNear)
        {
            if (m_TimeToRotate <= 0)
            {
                Move(speedWalk);
                LookingPlayer(playerLastPosition);
            }
            else
            {
                Stop();
                m_TimeToRotate -= Time.deltaTime;
            }
        }
        else
        {
            m_PlayerNear = false;
            playerLastPosition = Vector3.zero;

            // Wait until the agent arrives physically at the current patrol node
            if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (m_WaitTime <= 0)
                {
                    nextPoint();
                    Move(speedWalk);
                    m_WaitTime = startWaitTime;
                }
                else
                {
                    Stop();
                    m_WaitTime -= Time.deltaTime; // Dwell/Wait timer countdown at waypoints
                }
            }
        }
    }

    void Move(float speed)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = 0;
    }

    public void nextPoint()
    {
        if (waypoints.Length == 0) return;
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        SetWaypointDestination();
    }

    void CaughtPlayer()
    {
        m_CaughtPlayer = true;
    }

    void LookingPlayer(Vector3 player)
    {
        navMeshAgent.SetDestination(player);
        if (Vector3.Distance(transform.position, player) <= 0.3f)
        {
            if (m_WaitTime <= 0)
            {
                m_PlayerNear = false;
                Move(speedWalk);
                SetWaypointDestination();
                m_WaitTime = startWaitTime;
                m_TimeToRotate = timeToRotate;
            }
            else
            {
                Stop();
                m_WaitTime -= Time.deltaTime;
            }
        }
    }

    void EnviromentView()
    {
        Collider[] playerInRange = Physics.OverlapSphere(transform.position, viewRadius, playerMask);
        bool spottedThisFrame = false;

        for (int i = 0; i < playerInRange.Length; i++)
        {
            Transform player = playerInRange[i].transform;
            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);

                // Raycast to check if an obstacle is blocking the clear vision path
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, obstacleMask))
                {
                    spottedThisFrame = true;
                    m_PlayerInRange = true;
                    m_IsPatrol = false;
                    m_PlayerPosition = player.position;
                }
            }
        }

        if (!spottedThisFrame)
        {
            m_PlayerInRange = false;
        }
    }

    // Helper to send the agent to its current patrol index safely without continuous loop stuttering
    void SetWaypointDestination()
    {
        if (waypoints != null && waypoints.Length > 0 && waypoints[m_CurrentWaypointIndex] != null)
        {
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
    }
}