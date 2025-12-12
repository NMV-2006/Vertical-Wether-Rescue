using UnityEngine;

/// <summary>
/// Third Person Character Controller
/// Handles player movement, jumping, camera control, and platform interactions
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Movement speed in units per second")]
    public float moveSpeed = 5f;
    
    [Tooltip("Sprint speed multiplier when on ground")]
    public float sprintMultiplier = 1.5f;
    
    [Tooltip("How fast the character turns to face movement direction")]
    public float rotationSpeed = 10f;

    [Header("Glide Settings")]
    [Tooltip("Enable gliding by holding Shift in the air")]
    public bool canGlide = true;
    
    [Tooltip("Gravity multiplier while gliding (lower = slower fall)")]
    public float glideGravity = -2f;
    
    [Tooltip("Forward speed while gliding")]
    public float glideSpeed = 8f;

    [Header("Stamina Settings")]
    [Tooltip("Enable stamina system for air actions (jumping and gliding)")]
    public bool useStamina = true;
    
    [Tooltip("Maximum stamina")]
    public float maxStamina = 100f;
    
    [Tooltip("Stamina cost per jump")]
    public float jumpStaminaCost = 20f;
    
    [Tooltip("Stamina drain rate per second while gliding")]
    public float glidingStaminaDrain = 10f;
    
    [Tooltip("Stamina regeneration rate per second when grounded")]
    public float staminaRegenRate = 25f;

    [Header("Jump Settings")]
    [Tooltip("Jump height in units")]
    public float jumpHeight = 2f;
    
    [Tooltip("Maximum number of jumps (1 = single jump, 2 = double jump, etc.)")]
    public int maxJumps = 2;
    
    [Tooltip("Gravity multiplier")]
    public float gravity = -9.81f;
    
    [Tooltip("Additional downward force when falling")]
    public float fallMultiplier = 2.5f;

    [Header("Ground Detection")]
    [Tooltip("Distance to check for ground")]
    public float groundCheckDistance = 0.2f;
    
    [Tooltip("Layer mask for ground detection")]
    public LayerMask groundMask;

    [Header("Camera Settings")]
    [Tooltip("Reference to the camera that follows the character")]
    public Transform cameraTransform;
    
    [Tooltip("Camera rotation sensitivity")]
    public float cameraSensitivity = 2f;
    
    [Tooltip("Minimum vertical camera angle")]
    public float minCameraAngle = -30f;
    
    [Tooltip("Maximum vertical camera angle")]
    public float maxCameraAngle = 60f;
    
    [Tooltip("Distance of camera from character")]
    public float cameraDistance = 5f;
    
    [Tooltip("Height offset for camera")]
    public float cameraHeight = 2f;

    // Private variables
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private int jumpCount = 0; // Track number of jumps performed
    private bool isGliding = false; // Track if currently gliding
    private float currentStamina; // Current stamina for all air actions
    private float cameraVerticalAngle = 0f;
    private float cameraHorizontalAngle = 0f;
    private Transform currentPlatform;
    private Vector3 lastPlatformPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentStamina = maxStamina; // Initialize stamina
        
        // If no camera is assigned, try to find the main camera
        if (cameraTransform == null)
        {
            if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
            else
            {
                Debug.LogWarning("No camera assigned to ThirdPersonController!");
            }
        }
        
        // Initialize camera angles based on current camera position
        if (cameraTransform != null)
        {
            Vector3 angles = cameraTransform.eulerAngles;
            cameraHorizontalAngle = angles.y;
            cameraVerticalAngle = angles.x;
        }
        
        // Lock cursor for better camera control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleJump();
        HandleGlide();
        HandleGravity();
        HandleCameraRotation();
        HandlePlatformMovement();
        HandleStamina();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    /// <summary>
    /// Check if the character is on the ground
    /// </summary>
    void HandleGroundCheck()
    {
        // Cast a ray downward to check for ground
        isGrounded = Physics.Raycast(
            transform.position, 
            Vector3.down, 
            controller.height / 2 + groundCheckDistance,
            groundMask
        );

        // Reset vertical velocity and jump counter when grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep grounded
            jumpCount = 0; // Reset jump counter when landing
        }
    }

    /// <summary>
    /// Handle character movement based on input
    /// </summary>
    void HandleMovement()
    {
        // Get input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calculate movement direction relative to camera
        Vector3 cameraForward = Vector3.zero;
        Vector3 cameraRight = Vector3.zero;

        if (cameraTransform != null)
        {
            cameraForward = cameraTransform.forward;
            cameraRight = cameraTransform.right;
        }
        else
        {
            cameraForward = transform.forward;
            cameraRight = transform.right;
        }

        // Flatten the camera direction (ignore Y axis)
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

        // Apply sprint if holding shift AND on ground (not gliding)
        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            currentSpeed *= sprintMultiplier;
        }

        // Move the character
        if (moveDirection.magnitude >= 0.1f)
        {
            // Rotate character to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );

            // Apply movement
            controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Handle jump input - supports multi-jump (double jump, triple jump, etc.)
    /// </summary>
    void HandleJump()
    {
        // Check if we have enough stamina to jump (if stamina system is enabled)
        bool hasEnoughStamina = !useStamina || currentStamina >= jumpStaminaCost;
        
        // Allow jump if we haven't exceeded max jumps and have enough stamina
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumps && hasEnoughStamina)
        {
            // Calculate jump velocity using physics formula: v = sqrt(2 * h * g)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++; // Increment jump counter
            
            // Consume stamina for jumping (except first jump from ground)
            if (useStamina && jumpCount > 1)
            {
                currentStamina -= jumpStaminaCost;
                currentStamina = Mathf.Max(0, currentStamina);
            }
        }
    }

    /// <summary>
    /// Handle gliding mechanic when holding Shift in the air
    /// </summary>
    void HandleGlide()
    {
        // Check if we should be gliding
        bool wantsToGlide = Input.GetKey(KeyCode.LeftShift) && !isGrounded && canGlide;
        
        // Check stamina if enabled
        if (useStamina && currentStamina <= 0)
        {
            wantsToGlide = false;
        }
        
        isGliding = wantsToGlide;
        
        // Apply glide movement if gliding
        if (isGliding)
        {
            // Get camera forward direction for glide direction
            Vector3 glideDirection = Vector3.zero;
            
            if (cameraTransform != null)
            {
                glideDirection = cameraTransform.forward;
            }
            else
            {
                glideDirection = transform.forward;
            }
            
            // Flatten and normalize
            glideDirection.y = 0f;
            glideDirection.Normalize();
            
            // Apply forward glide movement
            controller.Move(glideDirection * glideSpeed * Time.deltaTime);
            
            // Rotate character to face glide direction
            if (glideDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(glideDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    /// <summary>
    /// Apply gravity to the character (modified for gliding)
    /// </summary>
    void HandleGravity()
    {
        // Use different gravity when gliding
        float currentGravity = isGliding ? glideGravity : gravity;
        
        // Apply stronger gravity when falling (but not when gliding)
        if (velocity.y < 0 && !isGliding)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += currentGravity * Time.deltaTime;
        }

        // Apply vertical velocity
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Handle stamina regeneration and consumption for all air actions
    /// </summary>
    void HandleStamina()
    {
        if (!useStamina) return;
        
        if (isGliding)
        {
            // Consume stamina while gliding
            currentStamina -= glidingStaminaDrain * Time.deltaTime;
            currentStamina = Mathf.Max(0, currentStamina);
        }
        else if (isGrounded)
        {
            // Regenerate stamina when on ground or platform
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(maxStamina, currentStamina);
        }
        // Note: Stamina does NOT regenerate while in air (not gliding)
    }

    /// <summary>
    /// Handle camera rotation based on mouse input
    /// </summary>
    void HandleCameraRotation()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

        // Update camera angles
        cameraHorizontalAngle += mouseX;
        cameraVerticalAngle -= mouseY;

        // Clamp vertical angle
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, minCameraAngle, maxCameraAngle);

        // Toggle cursor lock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    /// <summary>
    /// Update camera position to follow the character
    /// </summary>
    void UpdateCameraPosition()
    {
        if (cameraTransform == null) return;

        // Calculate camera position based on angles
        Quaternion rotation = Quaternion.Euler(cameraVerticalAngle, cameraHorizontalAngle, 0f);
        Vector3 offset = rotation * new Vector3(0f, cameraHeight, -cameraDistance);
        
        Vector3 targetPosition = transform.position + offset;

        // Smooth camera movement
        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position, 
            targetPosition, 
            10f * Time.deltaTime
        );

        // Make camera look at character
        Vector3 lookTarget = transform.position + Vector3.up * cameraHeight;
        cameraTransform.LookAt(lookTarget);
    }

    /// <summary>
    /// Handle movement when standing on a moving platform
    /// </summary>
    void HandlePlatformMovement()
    {
        // Check if we're on a moving platform
        RaycastHit hit;
        if (Physics.Raycast(
            transform.position, 
            Vector3.down, 
            out hit, 
            controller.height / 2 + groundCheckDistance,
            groundMask))
        {
            // Check if the hit object has a different parent (likely a platform)
            if (hit.transform != transform && hit.transform != currentPlatform)
            {
                currentPlatform = hit.transform;
                lastPlatformPosition = currentPlatform.position;
            }
            else if (hit.transform == currentPlatform)
            {
                // Move with the platform
                Vector3 platformMovement = currentPlatform.position - lastPlatformPosition;
                controller.Move(platformMovement);
                lastPlatformPosition = currentPlatform.position;
            }
        }
        else
        {
            // Not on a platform anymore
            currentPlatform = null;
        }
    }

    /// <summary>
    /// Draw debug information in the scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * (GetComponent<CharacterController>()?.height / 2 + groundCheckDistance ?? 1f);
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.1f);
        
        // Draw glide indicator
        if (isGliding)
        {
            Gizmos.color = Color.cyan;
            Vector3 glideDir = transform.forward * 2f;
            Gizmos.DrawRay(transform.position, glideDir);
        }
    }
    
    /// <summary>
    /// Get current stamina (for UI display)
    /// </summary>
    public float GetStamina()
    {
        return currentStamina;
    }
    
    /// <summary>
    /// Get max stamina (for UI display)
    /// </summary>
    public float GetMaxStamina()
    {
        return maxStamina;
    }
    
    /// <summary>
    /// Get stamina as a percentage (0-1) for UI bars
    /// </summary>
    public float GetStaminaPercent()
    {
        return maxStamina > 0 ? currentStamina / maxStamina : 0f;
    }
    
    /// <summary>
    /// Check if currently gliding (for animations/effects)
    /// </summary>
    public bool IsGliding()
    {
        return isGliding;
    }
    
    /// <summary>
    /// Apply external force to the character (used by jump pads, wind zones, etc.)
    /// </summary>
    /// <param name="force">The force vector to apply</param>
    /// <param name="overrideVelocity">If true, replaces current velocity. If false, adds to it.</param>
    public void ApplyExternalForce(Vector3 force, bool overrideVelocity = true)
    {
        if (overrideVelocity)
        {
            velocity = force;
        }
        else
        {
            velocity += force;
        }
    }
    
    /// <summary>
    /// Reset jump count (used by jump pads to allow multi-jump after bouncing)
    /// </summary>
    public void ResetJumpCount()
    {
        jumpCount = 0;
    }
    
    /// <summary>
    /// Check if character is grounded (for external scripts)
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }
}
