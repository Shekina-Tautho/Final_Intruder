using UnityEngine;
using UnityEngine.SceneManagement;


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
    public Transform currentTarget = null;
    public Transform[] patrolPoints;


    [Header("Combat")]
    public float damagePerSecond = 5f;


    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float returnSpeed = 3f;


    [Header("Ranges")]
    public float detectionRange = 50f;
    public float attackRange = 10f;


    [Header("Patrol Zone (IMPORTANT FIX)")]
    public float patrolRadius = 200f;
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

    private MacrophageAnimation macrophageAnimation;


    void Start()
    {
        currentState = State.Patrol;
        originalPosition = transform.position;


        patrolCenter = GetPatrolCenter();


        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (macrophageAnimation == null)
        {
            macrophageAnimation = GetComponent<MacrophageAnimation>();
        }
    }


    void Update()
    {
        if (player == null) return;

        // ✅ NEW: global stop if player is dead
        // You might be able to delete this; This code is unused
        if (playerStats != null && playerStats.isDead)
        {
            StopAttackSound();
            StopChaseSound();
            return;
        }

        // find target virus (nearest virus)
        currentTarget = setTargetVirus();

        Debug.Log(
            currentTarget != null
                ? "Target = " + currentTarget.name
                : "No Target"
        );

        float distance = Mathf.Infinity;

        // get distance between virus and macrophage
        if (currentTarget != null)
        {
            distance = Vector3.Distance(
                transform.position,
                currentTarget.position
            );
        }

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

        // delete later
        Debug.Log("Nearest NPC Virus: " + currentTarget);
    }


    // ---------------- PATROL ----------------
    void PatrolState(float distance)
    {
        StopChaseSound();
        StopAttackSound();


        alertSoundPlayed = false;


        if (patrolPoints.Length == 0) return;


        Transform target = patrolPoints[patrolIndex];


        // Move towards active patrol points
        if (target.gameObject.activeSelf) {
            MoveTo(target.position, patrolSpeed);
        // Skip inactive patrol points
        } else {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }




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


        // MoveTo(player.position, chaseSpeed);
        MoveTo(currentTarget.position, chaseSpeed);


        if (distance <= attackRange)
        {
            currentState = State.Attack;
        }
    }


    // ---------------- ATTACK ----------------
    void AttackState(float distance)
    {

        // if current target dies
        if (currentTarget == null)
        {
            StopAttackSound();

            // find next target
            currentTarget = setTargetVirus();

            if (currentTarget != null)
            {
                // if there's target within range, chase
                currentState = State.Chase;
            } else {
                // if no target within range, return to patrol
                currentState = State.ReturnToPatrol;
            }

            currentState = State.ReturnToPatrol;
            return;
        }


        StopChaseSound();


        if (!attackSoundPlaying)
        {
            StartAttackSound();
        }


        MoveTo(transform.position, 0f);
        
        if (currentTarget != null)
        {
            if (currentTarget.CompareTag("Virus"))
            {
                if (macrophageAnimation != null)
                {
                    macrophageAnimation.PlayAttack();
                }

                Destroy(currentTarget.gameObject);
                currentState = State.ReturnToPatrol;
            } else if (currentTarget.CompareTag("Player"))
            {
                if (macrophageAnimation != null)
                {
                    macrophageAnimation.PlayAttack();
                }

                // when player dies, go to main menu (you may change this)
                SceneManager.LoadScene("PathoMenu");
            }
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
            // Only return to active patrol points
            if (patrolPoints[i].gameObject.activeSelf)
            {
                float dist = Vector3.Distance(transform.position, patrolPoints[i].position);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nearestIndex = i;
                }
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

    Transform setTargetVirus()
    {
        Transform closestTarget = player;
        float shortestDistance =
            Vector3.Distance(transform.position,
                            player.position);

        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            detectionRange
        );

        // to check every game object within range
        foreach (Collider hitCollider in hitColliders)
        {
            // to exclude game objects without virus tag
            if (!hitCollider.CompareTag("Virus"))
                continue;

            // to get distance between npc virus and macrophage
            float distance =
                Vector3.Distance(
                    transform.position,
                    hitCollider.transform.position
                );

            // compare distance of npc virus and macrophage
            if (distance < shortestDistance)
            {
                // if npc virus is closer, set as target
                shortestDistance = distance;
                closestTarget = hitCollider.transform;
            }
        }

        return closestTarget;
    }
}
