using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waypointReachDistance = 0.1f;
    
    [Header("Chase/Repel")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float repelSpeed = 3f;
    
    [Header("Damage")]
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private float damageCooldown = 1f;
    
    private Transform player;
    private Rigidbody2D rb;
    private int _currentWaypointIndex = 0;
    private int _direction = 1; // 1 = forward, -1 = backward
    private float _lastDamageTime;
    
    // Behavior states - set by player's mask system
    public enum BehaviorState { Patrol, Chase, Repel }
    public BehaviorState currentBehavior = BehaviorState.Patrol;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Check if player is in range
        if (distanceToPlayer <= detectionRange)
        {
            if (currentBehavior == BehaviorState.Chase)
                ChasePlayer();
            else if (currentBehavior == BehaviorState.Repel)
                RepelFromPlayer();
            else
                Patrol(); // Stay on patrol even if in range
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;
    
        Transform targetWaypoint = waypoints[_currentWaypointIndex];
        Vector2 moveDirection = (targetWaypoint.position - transform.position).normalized;
        rb.linearVelocity = moveDirection * patrolSpeed;
    
        float distance = Vector2.Distance(transform.position, targetWaypoint.position);
        if (distance < waypointReachDistance)
        {
            _currentWaypointIndex += _direction;
        
            // Reverse direction at endpoints
            if (_currentWaypointIndex >= waypoints.Length || _currentWaypointIndex < 0)
            {
                _direction *= -1;
                _currentWaypointIndex += _direction;
            }
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
    }

    private void RepelFromPlayer()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        rb.linearVelocity = direction * repelSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Damage player on cooldown
            if (Time.time >= _lastDamageTime + damageCooldown)
            {
                // TODO: Call player's TakeDamage method
                Debug.Log($"Enemy dealt {damageAmount} damage to player!");
                _lastDamageTime = Time.time;
            }
        }
    }
    
    // Call this from your mask/player system
    public void SetBehavior(BehaviorState newBehavior)
    {
        currentBehavior = newBehavior;
    }
    
    void OnDrawGizmosSelected()
    {
        // Visualize detection range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
