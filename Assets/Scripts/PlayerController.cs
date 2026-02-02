using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private Vector2 lastMoveInput;
    private Transform spotLight;
    
    // Force animator update in WebGL
    private int moveXHash;
    private int moveYHash;
    private int isMovingHash;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spotLight = transform.Find("Light 2D");
        lastMoveInput = Vector2.right;
        
        // Cache parameter hashes for WebGL
        moveXHash = Animator.StringToHash("MoveX");
        moveYHash = Animator.StringToHash("MoveY");
        isMovingHash = Animator.StringToHash("IsMoving");
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
            
            // Use hashes instead of strings for WebGL
            animator.SetBool(isMovingHash, true);
            animator.SetFloat(moveXHash, lastMoveInput.x);
            animator.SetFloat(moveYHash, lastMoveInput.y);
        }
        else
        {
            animator.SetBool(isMovingHash, false);
        }
        
        // Force animator update in WebGL
#if UNITY_WEBGL
        animator.Update(0);
#endif
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput.normalized * moveSpeed;
    }
}