using UnityEngine;

public class MacrophageFSM : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase,
        Attack
    }

    public State currentState;

    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Ranges")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    private int patrolIndex = 0;

    void Start()
    {
        currentState = State.Patrol;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                PatrolState(distance);
                break;

            case State.Chase:
                ChaseState(distance);
                break;

            case State.Attack:
                AttackState(distance);
                break;
        }
    }

    void PatrolState(float distance)
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[patrolIndex];

        MoveTo(target.position, patrolSpeed);

        // Switch immediately to next point (no stopping)
        if (Vector3.Distance(transform.position, target.position) < 0.8f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        if (distance <= detectionRange)
        {
            currentState = State.Chase;
        }
    }

    void ChaseState(float distance)
    {
        MoveTo(player.position, chaseSpeed);

        if (distance <= attackRange)
        {
            currentState = State.Attack;
        }
        else if (distance > detectionRange + 3f)
        {
            currentState = State.Patrol;
        }
    }

    void AttackState(float distance)
    {
        Debug.Log(gameObject.name + " caught the virus!");

        if (distance > attackRange)
        {
            currentState = State.Chase;
        }
    }

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