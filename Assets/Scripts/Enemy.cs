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
    
    private Transform _player;
    private Rigidbody2D _rb;
    private int _currentWaypointIndex = 0;
    private int _direction = 1;
    private float _lastDamageTime;
    
    // Behavior states - controlled by SensorySystem
    public enum BehaviorState { Neutral, Chase, Repel }
    private BehaviorState _currentBehavior = BehaviorState.Neutral;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, _player.position);
        
        // Only react to chase/repel if player is in range
        if (distanceToPlayer <= detectionRange)
        {
            switch (_currentBehavior)
            {
                case BehaviorState.Neutral:
                    Patrol(); // Still patrol even when close
                    break;
                case BehaviorState.Chase:
                    ChasePlayer(); // Attracted to player
                    break;
                case BehaviorState.Repel:
                    RepelFromPlayer(); // Flee from player
                    break;
            }
        }
        else
        {
            // Out of range - always patrol regardless of behavior state
            Patrol();
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;
    
        Transform targetWaypoint = waypoints[_currentWaypointIndex];
        Vector2 moveDirection = (targetWaypoint.position - transform.position).normalized;
        _rb.linearVelocity = moveDirection * patrolSpeed;
    
        float distance = Vector2.Distance(transform.position, targetWaypoint.position);
        if (distance < waypointReachDistance)
        {
            _currentWaypointIndex += _direction;
        
            if (_currentWaypointIndex >= waypoints.Length || _currentWaypointIndex < 0)
            {
                _direction *= -1;
                _currentWaypointIndex += _direction;
            }
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * chaseSpeed;
    }

    private void RepelFromPlayer()
    {
        Vector2 direction = (transform.position - _player.position).normalized;
        _rb.linearVelocity = direction * repelSpeed;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= _lastDamageTime + damageCooldown)
            {
                Debug.Log($"Enemy dealt {damageAmount} damage to player!");
                _lastDamageTime = Time.time;
            }
        }
    }
    
    // Called by SensorySystem
    public void SetBehavior(BehaviorState newBehavior)
    {
        _currentBehavior = newBehavior;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
