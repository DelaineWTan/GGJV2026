using UnityEngine;

public class AudioDirectionalIndicator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SensorySystem sensorySystem;
    [SerializeField] private Transform player;
    [SerializeField] private Transform exit;
    [SerializeField] private SpriteRenderer arrowSprite;
    
    [Header("Settings")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float distanceFromPlayer = 1.5f;
    
    private void Start()
    {
        arrowSprite.enabled = false;
    }
    
    private void Update()
    {
        // Only show when hearing is positive
        bool showIndicator = sensorySystem && 
                            sensorySystem.hearState == SensorySystem.SenseState.Positive;
        
        if (!showIndicator)
        {
            arrowSprite.enabled = false;
            return;
        }
        
        // Find closest target
        Transform target = GetClosestTarget();
        
        if (!target)
        {
            arrowSprite.enabled = false;
            return;
        }
        
        arrowSprite.enabled = true;
        
        // Calculate direction from player to target
        Vector3 direction = (target.position - player.position).normalized;
        
        // Position arrow near player in direction of target
        transform.position = player.position + direction * distanceFromPlayer;
        
        // Rotate arrow to point at target
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // Sharp ping - quick flash then long pause
        float t = Time.time * pulseSpeed;
        float pulse = Mathf.Max(0, Mathf.Sin(t));
        pulse = Mathf.Pow(pulse, 5f); // Very sharp flash
        arrowSprite.color = new Color(1, 1, 1, pulse);
    }
    
    private Transform GetClosestTarget()
    {
        var collectibles = FindObjectsByType<Collectible>(FindObjectsSortMode.None);
        
        if (collectibles.Length > 0)
        {
            Transform closest = null;
            float minDistance = float.MaxValue;
            
            foreach (var c in collectibles)
            {
                float distance = Vector2.Distance(player.position, c.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = c.transform;
                }
            }
            
            return closest;
        }
        
        if (exit && GameManager.Instance && GameManager.Instance.AllCollected())
        {
            return exit;
        }
        
        return null;
    }
}
