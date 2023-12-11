using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class ChestScript : MonoBehaviour
{
    public int damageUpgradeAmount = 5;
    public float critChanceUpgrade = 0.05f;
    public float critDamageMultiplierUpgrade = 0.5f;
    public Color usedColor = Color.red;
    public float activationRange = 3f;
    public float colorChangeSpeed = 2f;

    private bool isUsed = false;
    private bool isOptionsVisible = false;
    private Renderer chestRenderer;
    private Material originalMaterial;
    private Color currentColor;

    public CanvasGroup optionsCanvasGroup;
    public Canvas promptCanvas;
    public Button damageButton;
    public Button critChanceButton;
    public Button critDamageButton;
    public TextMeshProUGUI promptText;

    private void Start()
    {
        chestRenderer = GetComponent<Renderer>();
        originalMaterial = new Material(chestRenderer.sharedMaterial);

        if (optionsCanvasGroup != null)
        {
            FadeCanvasGroup(optionsCanvasGroup, 1f, 0f, 0f);
            optionsCanvasGroup.interactable = false;
            optionsCanvasGroup.blocksRaycasts = false;
        }

        if (promptCanvas != null)
        {
            promptCanvas.enabled = false;
        }

        damageButton.onClick.AddListener(UpgradeDamage);
        critChanceButton.onClick.AddListener(UpgradeCritChance);
        critDamageButton.onClick.AddListener(UpgradeCritDamage);
    }

    private void Update()
    {
        if (!isUsed && IsPlayerNearby())
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isOptionsVisible = true;
                ShowOptions();

                DisablePlayerInput();
                StartCoroutine(FadeCanvasGroup(optionsCanvasGroup, 0f, 1f, 1f));

            }
            else
            {
                ShowPrompt();
            }
        }
        else
        {
            HidePrompt();
        }

        if (isOptionsVisible && Input.GetKeyDown(KeyCode.F))
        {
            return;
        }

        if (isUsed && currentColor != usedColor)
        {
            currentColor = Color.Lerp(currentColor, usedColor, colorChangeSpeed * Time.deltaTime);
            chestRenderer.material.color = currentColor;
        }
    }

    bool IsPlayerNearby()
    {
        PlayerScript playerScript = FindObjectOfType<PlayerScript>();

        if (playerScript != null)
        {
            float distance = Vector3.Distance(transform.position, playerScript.transform.position);
            return distance <= activationRange;
        }

        return false;
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
    }

    void ShowOptions()
{
    if (optionsCanvasGroup != null)
    {
        optionsCanvasGroup.interactable = true;
        optionsCanvasGroup.blocksRaycasts = true;

        optionsCanvasGroup.alpha = 1f;
    }
}

void HideOptions()
{
    if (optionsCanvasGroup != null)
    {
        StartCoroutine(FadeCanvasGroup(optionsCanvasGroup, optionsCanvasGroup.alpha, 0f, 0.5f));
        isOptionsVisible = false;
    }
}

    void UpgradeDamage()
    {
        PlayerScript playerScript = FindObjectOfType<PlayerScript>();

        if (playerScript != null)
        {
            playerScript.UpgradeDamage(damageUpgradeAmount);
            DeactivateChest();
        }
    }

    void UpgradeCritChance()
    {
        PlayerScript playerScript = FindObjectOfType<PlayerScript>();

        if (playerScript != null)
        {
            playerScript.UpgradeCritChance(critChanceUpgrade);
            DeactivateChest();
        }
    }

    void UpgradeCritDamage()
    {
        PlayerScript playerScript = FindObjectOfType<PlayerScript>();

        if (playerScript != null)
        {
            playerScript.UpgradeCritDamage(critDamageMultiplierUpgrade);
            DeactivateChest();
        }
    }

    void DisablePlayerInput()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            playerController.DisableInput();
        }
    }

    void EnablePlayerInput()
    {
        PlayerController playerController = FindObjectOfType<PlayerController>();

        if (playerController != null)
        {
            playerController.EnableInput();
        }
    }

    void DeactivateChest()
    {
        isUsed = true;
        HideOptions();

        EnablePlayerInput();
    }

    IEnumerator FadeCanvasGroup(CanvasGroup group, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        group.interactable = true;
        group.blocksRaycasts = true;

        while (elapsedTime < duration)
        {
            group.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        group.alpha = endAlpha;

        if (endAlpha == 0f)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }
}