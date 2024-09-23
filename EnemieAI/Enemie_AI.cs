using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Enemy_AI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public LayerMask whatIsGround, whatIsPlayer;

    public float health = 100;
    public Image enemyHealthBar;
    public GameObject damageTextPrefab;

    public PostProcessVolume postProcessVolume;
    private Vignette vignette;
    private bool isPulsing = false;
    private bool isDead = false;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public ParticleSystem deathParticlesBlood;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private int accumulatedDamage = 0;

    private void Awake()
    {
        player = GameObject.Find("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.autoTraverseOffMeshLink = false;
        }
        else
        {
            Debug.LogError("NavMeshAgent component is missing from this GameObject.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator component is missing from this GameObject.");
        }

        if (postProcessVolume != null)
        {
            if (!postProcessVolume.profile.TryGetSettings(out vignette))
            {
                Debug.LogError("Vignette setting is missing in the PostProcessVolume profile.");
            }
        }
        else
        {
            Debug.LogError("PostProcessVolume is not assigned.");
        }
    }

    private void Update()
    {
        if (isDead) return;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        animator.SetBool("IsPatrolling", true);
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", false);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
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
        animator.SetBool("IsPatrolling", false);
        animator.SetBool("IsChasing", true);
        animator.SetBool("IsAttacking", false);
    }

    private void AttackPlayer()
    {
        if (isDead) return;

        agent.SetDestination(transform.position);

        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);

        animator.SetBool("IsPatrolling", false);
        animator.SetBool("IsChasing", false);
        animator.SetBool("IsAttacking", true);

        if (!alreadyAttacked)
        {
            Vector3 projectileSpawnPosition = transform.position + new Vector3(0, -0.3f, 0);

            GameObject projectileInstance = Instantiate(projectile, projectileSpawnPosition, Quaternion.identity);
            Rigidbody rb = projectileInstance.GetComponent<Rigidbody>();

            projectileInstance.transform.rotation = Quaternion.LookRotation(transform.forward);

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public int TakeDamage(int damage)
    {
        if (isDead) return 0;

        accumulatedDamage += damage;
        health -= damage;
        enemyHealthBar.fillAmount = health / 250f;

        if (health <= 0) StartCoroutine(DestroyEnemy());

        return accumulatedDamage;
    }

    private IEnumerator DestroyEnemy()
    {
        isDead = true;
        accumulatedDamage = 0;
        agent.enabled = false;
        animator.enabled = false;
        enemyHealthBar.enabled = false;
        if (deathParticlesBlood != null)
        {
            deathParticlesBlood.Play();
        }
        else
        {
            Debug.LogError("Death particles are not assigned!");
        }


        yield return new WaitForSeconds(5);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
