using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float spawnRadius = 5f;
    [SerializeField] private float checkRadius = 0.5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Spawn SFX")] 
    [SerializeField] private AudioClip spawnSound;
    
    private Transform _player;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _col;
    private float _nextSpawnTime;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        
        // First spawn immediately
        SpawnNearPlayer();
        _nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        // Respawn every interval regardless of pickup
        if (Time.time >= _nextSpawnTime)
        {
            SpawnNearPlayer();
            _nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnNearPlayer()
    {
        Vector3 validPosition = Vector3.zero;
        bool foundValidSpot = false;
        
        // Keep trying until we find a valid spawn position
        while (!foundValidSpot)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 testPosition = _player.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            // Check if this position overlaps with any obstacles
            Collider2D hit = Physics2D.OverlapCircle(testPosition, checkRadius, obstacleLayer);
            
            if (hit == null)
            {
                validPosition = testPosition;
                foundValidSpot = true;
            }
        }
        
        transform.position = validPosition;
        
        // Enable the fruit
        _spriteRenderer.enabled = true;
        _col.enabled = true;
        
        AudioUtils.PlayOneShotSFX(spawnSound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Roll the good die
            SensorySystem sensorySystem = GameManager.Instance.GetComponent<SensorySystem>();
            sensorySystem.RollGoodDie();
            
            // Hide the fruit (will respawn on next interval)
            _spriteRenderer.enabled = false;
            _col.enabled = false;
        }
    }
}
