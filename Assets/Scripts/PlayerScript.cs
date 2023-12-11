using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private bool isInCombat = false;
    public GameObject sword;
    public float combatRange = 10f;

    public int baseDamage = 10;
    public float critChance = 0.1f;
    public float critDamageMultiplier = 1f;
    public int maxHealth = 100;
    private int currentHealth;

    public PlayerStatsUI playerStatsUI;
    public CanvasGroup canvasGroup;

    private bool isUIVisible = false;

    void Start()
    {
        if (sword != null)
        {
            sword.SetActive(false);
        }

        currentHealth = maxHealth;
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
    }

    void Update()
    {

        if (IsEnemyInRange())
        {
            SetInCombat(true);
        }
        else
        {
            SetInCombat(false);
        }

        if (isInCombat && Input.GetButtonDown("Fire1"))
        {
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            TogglePlayerStatsUI();
        }
    }

    void SetInCombat(bool value)
    {
        isInCombat = value;
        ToggleSwordVisibility();
    }

    void ToggleSwordVisibility()
    {
        if (sword != null)
        {
            sword.SetActive(isInCombat);
        }
    }

    bool IsEnemyInRange()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, combatRange);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                return true;
            }
        }

        return false;
    }

    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, combatRange);

        foreach (Collider enemy in hitEnemies)
        {
            EnemyAI enemyAI = enemy.GetComponentInParent<EnemyAI>();

            if (enemyAI != null)
            {
                int damage = CalculateDamage();

                enemyAI.TakeDamage(damage);
            }
        }
    }
    

    int CalculateDamage()
    {
        int damage = baseDamage;

        if (Random.value < critChance)
        {
            damage = Mathf.RoundToInt(damage * critDamageMultiplier);
            Debug.Log("Critical Hit!");
        }

        return damage;
    }

    public void UpgradeDamage(int amount)
    {
        baseDamage += 2;
        Debug.Log("Player's damage increased to: " + baseDamage);
    }

    public void UpgradeCritChance(float amount)
    {
        critChance += 0.05f;
        Debug.Log("Player's crit chance increased to: " + critChance);
    }

    public void UpgradeCritDamage(float amount)
    {
        critDamageMultiplier += 0.2f;
        Debug.Log("Player's crit damage multiplier increased to: " + critDamageMultiplier);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log("Player healed. Current health: " + currentHealth);
    }

    void TogglePlayerStatsUI()
    {
        isUIVisible = !isUIVisible;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = isUIVisible ? 1f : 0f;
        }
    }

}
