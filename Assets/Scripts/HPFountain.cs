using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPFountain : MonoBehaviour
{
    public int maxHealthIncrease = 10;
    public Color usedColor = Color.gray;
    public float activationRange = 3f;
    public float colorChangeSpeed = 2f;

    private bool isUsed = false;
    private Renderer fountainRenderer;
    private Material originalMaterial;
    private Color currentColor;

    // UI elements
    public Canvas promptCanvas;

    private void Start()
    {
        fountainRenderer = GetComponent<Renderer>();
        originalMaterial = new Material(fountainRenderer.sharedMaterial);

        if (promptCanvas != null)
        {
            promptCanvas.enabled = false;
        }
    }

    private void Update()
    {
        if (!isUsed && IsPlayerNearby())
        {
            ShowPrompt();

            if (Input.GetKeyDown(KeyCode.F))
            {
                ActivateFountain();
            }
        }
        else
        {
            HidePrompt();
        }
    }

    bool IsPlayerNearby()
    {
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            float distance = Vector3.Distance(transform.position, playerHealth.transform.position);
            return distance <= activationRange;
        }

        return false;
    }

    void ActivateFountain()
    {
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.IncreaseMaxHealth(maxHealthIncrease);
            playerHealth.Heal(maxHealthIncrease);

            isUsed = true;
        }
    }

    void ShowPrompt()
    {
        if (promptCanvas != null)
        {
            promptCanvas.enabled = true;
        }
    }

    void HidePrompt()
    {
        if (promptCanvas != null)
        {
            promptCanvas.enabled = false;
        }
        if (isUsed && currentColor != usedColor)
        {
            currentColor = Color.Lerp(currentColor, usedColor, colorChangeSpeed * Time.deltaTime);
            fountainRenderer.material.color = currentColor;
        }
    }
}