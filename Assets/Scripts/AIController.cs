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

    private Transform playerTransform;
    private Animator animator;

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
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }

        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = speedWalk;
        }

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

        if (navMeshAgent != null && animator != null)
        {
            float currentSpeed = navMeshAgent.velocity.magnitude;
            animator.SetFloat("speed", currentSpeed);
        }
    }

    private void Chasing()
    {
        if (playerTransform == null || !playerTransform.CompareTag("Player"))
        {
            m_IsPatrol = true;
            Move(speedWalk);
            SetWaypointDestination();
            return;
        }

        m_PlayerNear = false;
        playerLastPosition = Vector3.zero;

        if (!m_CaughtPlayer)
        {
            Move(speedRun);
            m_PlayerPosition = playerTransform.position;
            navMeshAgent.SetDestination(m_PlayerPosition);
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
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
                // Cleaned up the distance check condition to prevent the startup lock bug
                if (m_PlayerInRange && Vector3.Distance(transform.position, playerTransform.position) >= 2.5f)
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
                    m_WaitTime -= Time.deltaTime;
                }
            }
        }
    }

    void Move(float speed)
    {
        if (navMeshAgent == null) return;
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = speed;
    }

    void Stop()
    {
        if (navMeshAgent == null) return;
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

            if (!player.CompareTag("Player"))
            {
                continue;
            }

            Vector3 dirToPlayer = (player.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToPlayer) < viewAngle / 2)
            {
                float dstToPlayer = Vector3.Distance(transform.position, player.position);

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

    void SetWaypointDestination()
    {
        if (waypoints != null && waypoints.Length > 0 && waypoints[m_CurrentWaypointIndex] != null && navMeshAgent != null)
        {
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        }
    }
}