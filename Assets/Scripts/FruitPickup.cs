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
    
    [Header("Pulse Effect")]
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private float pulseSpeed = 2f;
    
    private Transform _player;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _col;
    private float _nextSpawnTime;
    private Vector3 _baseScale;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _col = GetComponent<Collider2D>();
        _baseScale = transform.localScale;
        
        SpawnNearPlayer();
        _nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (Time.time >= _nextSpawnTime)
        {
            SpawnNearPlayer();
            _nextSpawnTime = Time.time + spawnInterval;
        }
        
        // Pulse the scale
        if (_spriteRenderer.enabled)
        {
            float scale = Mathf.Lerp(minScale, maxScale, 
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
            transform.localScale = _baseScale * scale;
        }
    }

    private void SpawnNearPlayer()
    {
        Vector3 validPosition = Vector3.zero;
        bool foundValidSpot = false;
        
        while (!foundValidSpot)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            Vector3 testPosition = _player.position + new Vector3(randomOffset.x, randomOffset.y, 0);
            
            Collider2D hit = Physics2D.OverlapCircle(testPosition, checkRadius, obstacleLayer);
            
            if (!hit)
            {
                validPosition = testPosition;
                foundValidSpot = true;
            }
        }
        
        transform.position = validPosition;
        _spriteRenderer.enabled = true;
        _col.enabled = true;
        
        AudioUtils.PlayOneShotSFX(spawnSound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SensorySystem sensorySystem = GameManager.Instance.GetComponent<SensorySystem>();
            sensorySystem.RollGoodDie();
            
            _spriteRenderer.enabled = false;
            _col.enabled = false;
            transform.localScale = _baseScale; // Reset scale
        }
    }
}
