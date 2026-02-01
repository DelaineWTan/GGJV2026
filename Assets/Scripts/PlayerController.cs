using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveInput;
    private Transform spotLight;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spotLight = transform.Find("Light 2D");
        lastMoveInput = Vector2.right;
    }

    private void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.magnitude > 0.1f)
        {
            lastMoveInput = moveInput.normalized;
            
            float angle = Mathf.Atan2(lastMoveInput.y, lastMoveInput.x) * Mathf.Rad2Deg;
            spotLight.rotation = Quaternion.Euler(0f, 0f, angle - 90);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}
