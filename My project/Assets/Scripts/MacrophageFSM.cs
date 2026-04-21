using UnityEngine;

public class MacrophageFSM : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Alert,
        Chase,
        Attack,
        ReturnToPatrol
    }

    public State currentState;

    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float returnSpeed = 3f;

    [Header("Ranges")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Patrol Zone (IMPORTANT FIX)")]
    public float patrolRadius = 12f;
    private Vector3 patrolCenter;

    [Header("Alert Settings")]
    public float alertDuration = 0.7f;
    public float shakeIntensity = 0.1f;

    private float alertTimer;

    private int patrolIndex = 0;
    private int returnIndex = 0;

    private Vector3 originalPosition;

    void Start()
    {
        currentState = State.Patrol;
        originalPosition = transform.position;

        patrolCenter = GetPatrolCenter();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                PatrolState(distance);
                break;

            case State.Alert:
                AlertState();
                break;

            case State.Chase:
                ChaseState(distance);
                break;

            case State.Attack:
                AttackState(distance);
                break;

            case State.ReturnToPatrol:
                ReturnToPatrolState();
                break;
        }
    }

    // ---------------- PATROL ----------------
    void PatrolState(float distance)
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[patrolIndex];
        MoveTo(target.position, patrolSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.8f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        if (distance <= detectionRange)
        {
            originalPosition = transform.position;
            alertTimer = alertDuration;
            currentState = State.Alert;
        }
    }

    // ---------------- ALERT ----------------
    void AlertState()
    {
        alertTimer -= Time.deltaTime;

        Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
        transform.position = originalPosition + shakeOffset;

        if (alertTimer <= 0f)
        {
            transform.position = originalPosition;
            currentState = State.Chase;
        }
    }

    // ---------------- CHASE (FIXED LEASH SYSTEM) ----------------
    void ChaseState(float distance)
    {
        float distanceFromCenter = Vector3.Distance(transform.position, patrolCenter);

        // 🔥 PATROL ZONE LEASH (MAIN FIX)
        if (distanceFromCenter > patrolRadius)
        {
            returnIndex = GetNearestPatrolPointIndex();
            currentState = State.ReturnToPatrol;
            return;
        }

        // 🔥 PLAYER DISENGAGE RULE
        if (distance > detectionRange)
        {
            returnIndex = GetNearestPatrolPointIndex();
            currentState = State.ReturnToPatrol;
            return;
        }

        MoveTo(player.position, chaseSpeed);

        if (distance <= attackRange)
        {
            currentState = State.Attack;
        }
    }

    // ---------------- ATTACK (NO STATE LOOP BUG) ----------------
    void AttackState(float distance)
    {
        MoveTo(transform.position, 0f);

        Debug.Log(gameObject.name + " is attacking!");

        if (distance > attackRange)
        {
            currentState = State.Chase;
        }

        if (distance > detectionRange)
        {
            returnIndex = GetNearestPatrolPointIndex();
            currentState = State.ReturnToPatrol;
        }
    }

    // ---------------- RETURN ----------------
    void ReturnToPatrolState()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[returnIndex];
        MoveTo(target.position, returnSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.8f)
        {
            patrolIndex = returnIndex;
            currentState = State.Patrol;
        }
    }

    // ---------------- UTIL ----------------
    Vector3 GetPatrolCenter()
    {
        Vector3 sum = Vector3.zero;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            sum += patrolPoints[i].position;
        }

        return sum / patrolPoints.Length;
    }

    int GetNearestPatrolPointIndex()
    {
        int nearestIndex = 0;
        float shortestDistance = Mathf.Infinity;

        for (int i = 0; i < patrolPoints.Length; i++)
        {
            float dist = Vector3.Distance(transform.position, patrolPoints[i].position);

            if (dist < shortestDistance)
            {
                shortestDistance = dist;
                nearestIndex = i;
            }
        }

        return nearestIndex;
    }

    // ---------------- MOVEMENT ----------------
    void MoveTo(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        Vector3 lookDir = target - transform.position;

        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookDir),
                5f * Time.deltaTime
            );
        }
    }
}