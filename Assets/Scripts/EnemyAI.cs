using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;
    public Animator enemyAnimator;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float health;

    public float timeBetweenAttacks;
    bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private bool isAttacking = false;

    public int damageAmount = 10;

    private bool isStunned = false;

    private Transform player;

    void StopAgent()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    void ResumeAgent()
    {
        if (agent != null)
        {
            agent.isStopped = false;
        }
    }
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)
        {
            Patroling();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            ChasePlayer();
        }
        else if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
    }

    private void Patroling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
            enemyAnimator.SetFloat("Locomotion", 1f);
        }

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
            enemyAnimator.SetFloat("Locomotion", 0f);
        }
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        enemyAnimator.SetFloat("Locomotion", 1f);
    }

    private void AttackPlayer()
    {
        transform.LookAt(player);

        if (!isAttacking)
        {
            isAttacking = true;
            enemyAnimator.SetBool("IsAttacking", true);
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        agent.enabled = false;

        yield return new WaitForSeconds(timeBetweenAttacks);

        DealDamageToPlayer();

        isAttacking = false;
        enemyAnimator.SetBool("IsAttacking", false);
        agent.enabled = true;
        if (agent.enabled)
        {
            agent.SetDestination(transform.position);
        }
    }

    private void DealDamageToPlayer()
    {
        if (playerInAttackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isStunned) return;

        health -= damage;

        if (health <= 0)
        {
            StartCoroutine(StunEnemy());
            FreezeAgentForSeconds(30f);
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(StunEnemy());
        }
    }

    private IEnumerator StunEnemy()
    {
        isStunned = true;

        enemyAnimator.SetBool("IsStunned", true);

        float stunDuration = 0.7f;
        yield return new WaitForSeconds(stunDuration);

        isStunned = false;
        enemyAnimator.SetBool("IsStunned", false);
    }

    void FreezeAgentForSeconds(float seconds)
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    private IEnumerator Die()
    {
        enemyAnimator.SetBool("IsDead", true);
        float deathDuration = 3f;
        yield return new WaitForSeconds(deathDuration);
        Destroy(gameObject);
    }
}


