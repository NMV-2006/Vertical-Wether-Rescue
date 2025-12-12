using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays stamina using a radial/circular meter
/// </summary>
public class StaminaUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the ThirdPersonController to read stamina from")]
    public ThirdPersonController playerController;

    [Header("UI Elements")]
    [Tooltip("The radial fill image (must have Image Type set to 'Filled')")]
    public Image radialFillImage;
    
    [Tooltip("Optional background circle image")]
    public Image backgroundImage;
    
    [Tooltip("Optional text to display stamina percentage")]
    public Text staminaText;

    [Header("Visual Settings")]
    [Tooltip("Color when stamina is full")]
    public Color fullColor = new Color(0.2f, 1f, 0.4f, 1f); // Green
    
    [Tooltip("Color when stamina is medium")]
    public Color mediumColor = new Color(1f, 0.8f, 0.2f, 1f); // Yellow
    
    [Tooltip("Color when stamina is low")]
    public Color lowColor = new Color(1f, 0.2f, 0.2f, 1f); // Red
    
    [Tooltip("Stamina threshold for low color (0-1)")]
    [Range(0f, 1f)]
    public float lowThreshold = 0.25f;
    
    [Tooltip("Stamina threshold for medium color (0-1)")]
    [Range(0f, 1f)]
    public float mediumThreshold = 0.5f;
    
    [Tooltip("Smooth transition speed for fill amount")]
    public float smoothSpeed = 5f;
    
    [Tooltip("Pulse effect when stamina is low")]
    public bool pulseWhenLow = true;
    
    [Tooltip("Pulse speed")]
    public float pulseSpeed = 2f;

    [Header("Optional Effects")]
    [Tooltip("Show glow effect when regenerating")]
    public bool showRegenGlow = true;
    
    [Tooltip("Glow intensity multiplier")]
    public float glowIntensity = 1.5f;

    private float targetFillAmount = 1f;
    private float currentFillAmount = 1f;
    private float lastStamina = 0f;
    private bool isRegenerating = false;

    void Start()
    {
        // Try to find player controller if not assigned
        if (playerController == null)
        {
            playerController = FindObjectOfType<ThirdPersonController>();
            
            if (playerController == null)
            {
                Debug.LogError("StaminaUI: No ThirdPersonController found! Please assign it in the inspector.");
            }
        }

        // Validate radial fill image
        if (radialFillImage != null)
        {
            // Ensure the image is set to Filled type
            radialFillImage.type = Image.Type.Filled;
            radialFillImage.fillMethod = Image.FillMethod.Radial360;
            radialFillImage.fillOrigin = (int)Image.Origin360.Top;
            radialFillImage.fillClockwise = true;
        }
        else
        {
            Debug.LogWarning("StaminaUI: No radial fill image assigned!");
        }
    }

    void Update()
    {
        if (playerController == null || radialFillImage == null) return;

        // Get current stamina
        float currentStamina = playerController.GetStamina();
        float maxStamina = playerController.GetMaxStamina();
        float staminaPercent = playerController.GetStaminaPercent();

        // Detect if regenerating
        isRegenerating = currentStamina > lastStamina;
        lastStamina = currentStamina;

        // Update target fill amount
        targetFillAmount = staminaPercent;

        // Smooth fill amount transition
        currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, smoothSpeed * Time.deltaTime);
        radialFillImage.fillAmount = currentFillAmount;

        // Update color based on stamina level
        UpdateColor(staminaPercent);

        // Update text if available
        if (staminaText != null)
        {
            staminaText.text = Mathf.RoundToInt(staminaPercent * 100f) + "%";
        }

        // Apply pulse effect when low
        if (pulseWhenLow && staminaPercent < lowThreshold)
        {
            ApplyPulseEffect();
        }
    }

    /// <summary>
    /// Update the color of the radial meter based on stamina level
    /// </summary>
    void UpdateColor(float staminaPercent)
    {
        Color targetColor;

        // Determine color based on thresholds
        if (staminaPercent < lowThreshold)
        {
            targetColor = lowColor;
        }
        else if (staminaPercent < mediumThreshold)
        {
            // Lerp between low and medium
            float t = (staminaPercent - lowThreshold) / (mediumThreshold - lowThreshold);
            targetColor = Color.Lerp(lowColor, mediumColor, t);
        }
        else
        {
            // Lerp between medium and full
            float t = (staminaPercent - mediumThreshold) / (1f - mediumThreshold);
            targetColor = Color.Lerp(mediumColor, fullColor, t);
        }

        // Apply glow effect when regenerating
        if (showRegenGlow && isRegenerating)
        {
            targetColor *= glowIntensity;
        }

        radialFillImage.color = targetColor;
    }

    /// <summary>
    /// Apply pulse effect to the radial meter
    /// </summary>
    void ApplyPulseEffect()
    {
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float scale = 1f + (pulse * 0.1f); // Pulse between 1.0 and 1.1 scale
        
        radialFillImage.transform.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// Reset pulse effect
    /// </summary>
    void OnDisable()
    {
        if (radialFillImage != null)
        {
            radialFillImage.transform.localScale = Vector3.one;
        }
    }
}
