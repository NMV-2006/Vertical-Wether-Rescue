using UnityEngine;

/// <summary>
/// Jump Pad - Launches the character upward when they step on it
/// Perfect for vertical platforming gameplay
/// </summary>
public class JumpPad : MonoBehaviour
{
    [Header("Jump Settings")]
    [Tooltip("Force applied to launch the character upward")]
    public float jumpForce = 20f;
    
    [Tooltip("Optional horizontal boost direction (leave at 0,0,0 for purely vertical)")]
    public Vector3 horizontalBoost = Vector3.zero;
    
    [Tooltip("Override character's current velocity (true) or add to it (false)")]
    public bool overrideVelocity = true;
    
    [Tooltip("Reset jump count when bouncing (allows multi-jump after bounce)")]
    public bool resetJumpCount = true;

    [Header("Cooldown Settings")]
    [Tooltip("Cooldown time before the pad can be used again (0 = no cooldown)")]
    public float cooldownTime = 0.5f;
    
    private float lastBounceTime = -999f;

    [Header("Visual Feedback")]
    [Tooltip("Animate the pad when activated")]
    public bool animateOnBounce = true;
    
    [Tooltip("Compression amount when bounced (0-1)")]
    public float compressionAmount = 0.3f;
    
    [Tooltip("Animation speed")]
    public float animationSpeed = 10f;
    
    [Tooltip("Optional particle effect to play on bounce")]
    public ParticleSystem bounceEffect;
    
    [Tooltip("Optional audio clip to play on bounce")]
    public AudioClip bounceSound;

    [Header("Color Feedback")]
    [Tooltip("Change color when activated")]
    public bool changeColorOnBounce = true;
    
    [Tooltip("Color when ready")]
    public Color readyColor = new Color(0.2f, 1f, 0.4f, 1f); // Green
    
    [Tooltip("Color when on cooldown")]
    public Color cooldownColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Gray
    
    [Tooltip("Flash color on bounce")]
    public Color bounceColor = new Color(1f, 1f, 0.2f, 1f); // Yellow

    private Vector3 originalScale;
    private Vector3 targetScale;
    private Renderer padRenderer;
    private AudioSource audioSource;
    private Color currentColor;
    private Color targetColor;

    void Start()
    {
        // Store original scale for animation
        originalScale = transform.localScale;
        targetScale = originalScale;
        
        // Get renderer for color changes
        padRenderer = GetComponent<Renderer>();
        
        // Setup audio source if bounce sound is assigned
        if (bounceSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = bounceSound;
            audioSource.playOnAwake = false;
        }
        
        // Initialize colors
        currentColor = readyColor;
        targetColor = readyColor;
        
        if (padRenderer != null && changeColorOnBounce)
        {
            padRenderer.material.color = readyColor;
        }
    }

    void Update()
    {
        // Animate scale
        if (animateOnBounce)
        {
            transform.localScale = Vector3.Lerp(
                transform.localScale, 
                targetScale, 
                animationSpeed * Time.deltaTime
            );
        }
        
        // Animate color
        if (padRenderer != null && changeColorOnBounce)
        {
            currentColor = Color.Lerp(currentColor, targetColor, animationSpeed * Time.deltaTime);
            padRenderer.material.color = currentColor;
        }
        
        // Update color based on cooldown status
        if (IsOnCooldown())
        {
            targetColor = cooldownColor;
        }
        else
        {
            targetColor = readyColor;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[JumpPad] OnCollisionEnter detected: {collision.gameObject.name}");
        // Try to bounce the character
        TryBounce(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[JumpPad] OnTriggerEnter detected: {other.gameObject.name}");
        // Try to bounce the character (for trigger colliders)
        TryBounce(other.gameObject);
    }

    /// <summary>
    /// Attempt to bounce the character
    /// </summary>
    void TryBounce(GameObject obj)
    {
        Debug.Log($"[JumpPad] TryBounce called for: {obj.name}");
        
        // Check cooldown
        if (IsOnCooldown())
        {
            Debug.Log($"[JumpPad] On cooldown! Time remaining: {(lastBounceTime + cooldownTime) - Time.time}s");
            return;
        }

        // Check if the object has a CharacterController (our player)
        CharacterController controller = obj.GetComponent<CharacterController>();
        Debug.Log($"[JumpPad] CharacterController found: {controller != null}");
        
        if (controller != null)
        {
            // Get the ThirdPersonController to modify velocity
            ThirdPersonController playerController = obj.GetComponent<ThirdPersonController>();
            Debug.Log($"[JumpPad] ThirdPersonController found: {playerController != null}");
            
            if (playerController != null)
            {
                Debug.Log($"[JumpPad] Applying bounce with force: {jumpForce}");
                // Apply the bounce
                ApplyBounce(playerController);
                
                // Update cooldown
                lastBounceTime = Time.time;
                
                // Trigger visual/audio feedback
                TriggerFeedback();
                return; // Exit early after successful bounce
            }
        }
        
        // Also check for Rigidbody (for other physics objects)
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Debug.Log($"[JumpPad] Rigidbody found: {rb != null}");
        
        if (rb != null)
        {
            // Calculate bounce direction
            Vector3 bounceDirection = Vector3.up + horizontalBoost.normalized;
            
            if (overrideVelocity)
            {
                rb.linearVelocity = bounceDirection * jumpForce;
            }
            else
            {
                rb.AddForce(bounceDirection * jumpForce, ForceMode.VelocityChange);
            }
            
            // Update cooldown
            lastBounceTime = Time.time;
            
            // Trigger visual/audio feedback
            TriggerFeedback();
        }
        else
        {
            Debug.LogWarning($"[JumpPad] Object {obj.name} has neither CharacterController nor Rigidbody!");
        }
    }

    /// <summary>
    /// Apply bounce force to the player character
    /// </summary>
    void ApplyBounce(ThirdPersonController player)
    {
        // We need to access the velocity field through reflection or make it public
        // For now, we'll use a public method approach
        
        // Calculate the bounce velocity
        float bounceVelocity = jumpForce;
        
        // Apply through a public method (we'll need to add this to ThirdPersonController)
        player.ApplyExternalForce(Vector3.up * bounceVelocity + horizontalBoost, overrideVelocity);
        
        // Reset jump count if enabled
        if (resetJumpCount)
        {
            player.ResetJumpCount();
        }
    }

    /// <summary>
    /// Trigger visual and audio feedback
    /// </summary>
    void TriggerFeedback()
    {
        // Play particle effect
        if (bounceEffect != null)
        {
            bounceEffect.Play();
        }
        
        // Play sound
        if (audioSource != null && bounceSound != null)
        {
            audioSource.PlayOneShot(bounceSound);
        }
        
        // Animate compression
        if (animateOnBounce)
        {
            targetScale = new Vector3(
                originalScale.x * (1f + compressionAmount),
                originalScale.y * (1f - compressionAmount),
                originalScale.z * (1f + compressionAmount)
            );
            
            // Reset to original scale after a short delay
            Invoke(nameof(ResetScale), 0.1f);
        }
        
        // Flash color
        if (changeColorOnBounce)
        {
            currentColor = bounceColor;
        }
    }

    /// <summary>
    /// Reset scale to original
    /// </summary>
    void ResetScale()
    {
        targetScale = originalScale;
    }

    /// <summary>
    /// Check if the jump pad is on cooldown
    /// </summary>
    bool IsOnCooldown()
    {
        return Time.time < lastBounceTime + cooldownTime;
    }

    /// <summary>
    /// Draw gizmos in the scene view
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw arrow showing bounce direction
        Gizmos.color = Color.yellow;
        Vector3 bounceDir = (Vector3.up + horizontalBoost.normalized).normalized;
        Vector3 start = transform.position;
        Vector3 end = start + bounceDir * 3f;
        
        Gizmos.DrawLine(start, end);
        Gizmos.DrawWireSphere(end, 0.3f);
        
        // Draw the platform bounds
        Gizmos.color = IsOnCooldown() ? Color.red : Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
    }

    /// <summary>
    /// Draw gizmos when selected
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw a more detailed visualization
        Gizmos.color = Color.cyan;
        Vector3 bounceDir = (Vector3.up + horizontalBoost.normalized).normalized;
        
        // Draw multiple arrows to show trajectory
        for (int i = 0; i < 5; i++)
        {
            float t = i / 4f;
            Vector3 pos = transform.position + bounceDir * (3f * t);
            Gizmos.DrawWireSphere(pos, 0.2f);
        }
    }
}
