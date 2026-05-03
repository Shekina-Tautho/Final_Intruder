using UnityEngine;

public class MacrophageFSM : MonoBehaviour
{
    public PlayerStats playerStats;

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

    [Header("Combat")]
    public float damagePerSecond = 5f;

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

    [Header("Spatial Audio")]
    public AudioSource audioSource;

    public AudioClip alertClip;
    public AudioClip chaseLoopClip;
    public AudioClip attackClip;

    private bool chaseSoundPlaying = false;
    private bool alertSoundPlayed = false;
    private bool attackSoundPlaying = false;

    private float alertTimer;

    private int patrolIndex = 0;
    private int returnIndex = 0;

    private Vector3 originalPosition;

    public bool isDead = false;

    void Start()
    {
        currentState = State.Patrol;
        originalPosition = transform.position;

        patrolCenter = GetPatrolCenter();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (player == null) return;

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
        StopChaseSound();
        StopAttackSound();

        alertSoundPlayed = false;

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
        if (isDead) return;

        StopChaseSound();
        StopAttackSound();

        if (!alertSoundPlayed)
        {
            if (audioSource != null && alertClip != null)
            {
                audioSource.PlayOneShot(alertClip, 2f);
            }

            alertSoundPlayed = true;
        }

        alertTimer -= Time.deltaTime;

        Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
        transform.position = originalPosition + shakeOffset;

        if (alertTimer <= 0f)
        {
            transform.position = originalPosition;
            currentState = State.Chase;
        }

        
    }

    // ---------------- CHASE ----------------
    void ChaseState(float distance)
    {
        StopAttackSound();

        // ✅ FIX: only start if not already playing
        if (!chaseSoundPlaying)
        {
            StartChaseSound();
        }

        float distanceFromCenter = Vector3.Distance(transform.position, patrolCenter);

        if (distanceFromCenter > patrolRadius)
        {
            returnIndex = GetNearestPatrolPointIndex();
            currentState = State.ReturnToPatrol;
            return;
        }

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

    // ---------------- ATTACK ----------------
    void AttackState(float distance)
    {
        StopChaseSound();

        if (!attackSoundPlaying)
        {
            StartAttackSound();
        }

        MoveTo(transform.position, 0f);

        if (playerStats != null)
        {
            playerStats.TakeDamage(damagePerSecond * Time.deltaTime);
        }

        Debug.Log(gameObject.name + " is attacking!");

        if (distance > attackRange)
        {
            StopAttackSound();
            currentState = State.Chase;
        }

        if (distance > detectionRange)
        {
            StopAttackSound();
            returnIndex = GetNearestPatrolPointIndex();
            currentState = State.ReturnToPatrol;
        }
    }

    // ---------------- RETURN ----------------
    void ReturnToPatrolState()
    {
        StopChaseSound();
        StopAttackSound();

        alertSoundPlayed = false;

        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[returnIndex];
        MoveTo(target.position, returnSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.8f)
        {
            patrolIndex = returnIndex;
            currentState = State.Patrol;
        }
    }

    // ---------------- AUDIO ----------------
    void StartChaseSound()
    {
        if (audioSource == null || chaseLoopClip == null) return;

        audioSource.clip = chaseLoopClip;
        audioSource.loop = true;
        audioSource.Play();

        chaseSoundPlaying = true;
    }

    void StopChaseSound()
    {
        if (!chaseSoundPlaying) return;

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        chaseSoundPlaying = false;
    }

    void StartAttackSound()
    {
        if (audioSource == null || attackClip == null) return;

        audioSource.clip = attackClip;
        audioSource.loop = true;
        audioSource.Play();

        attackSoundPlaying = true;
    }

    void StopAttackSound()
    {
        if (!attackSoundPlaying) return;

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        attackSoundPlaying = false;
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
        if (isDead) return; // 🔥 IMPORTANT LOCK

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