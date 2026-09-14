using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private bool canMove = true;

    [Header("Vida")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Estado emocional")]
    [SerializeField] private float maxEmotionalState = 100f;
    private float currentEmotionalState;

    [Header("Coins")]
    [SerializeField] private int coins = 0;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        currentEmotionalState = maxEmotionalState;
    }


    void Update()
    {
        if (!canMove)
        {
            movementDirection = Vector2.zero;
            return;
        }

        movementDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );
    }


    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = movementDirection * speed;
    }


    // =========================
    // MOVIMIENTO
    // =========================

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled)
        {
            movementDirection = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
    }


    // =========================
    // VIDA
    // =========================

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Daño recibido: " + damage);
        Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }


    private void Die()
    {
        canMove = false;
        movementDirection = Vector2.zero;
        rb.velocity = Vector2.zero;

        Debug.Log("El jugador murió.");
    }


    // ESTADO EMOCIONAL 

    public void ChangeEmotionalState(float amount)
    {
        currentEmotionalState += amount;

        currentEmotionalState = Mathf.Clamp(
            currentEmotionalState,
            0f,
            maxEmotionalState
        );

        Debug.Log("Estado emocional: " + currentEmotionalState);
    }


    // =========================
    // COINS
    // =========================

    public void AddCoins(int amount)
    {
        coins += amount;

        Debug.Log("Coins: " + coins);
    }


    public bool SpendCoins(int amount)
    {
        if (coins < amount)
        {
            return false;
        }

        coins -= amount;

        Debug.Log("Coins: " + coins);

        return true;
    }


    // =========================
    // GETTERS
    // =========================

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public float GetCurrentEmotionalState()
    {
        return currentEmotionalState;
    }

    public float GetMaxEmotionalState()
    {
        return maxEmotionalState;
    }

    public int GetCoins()
    {
        return coins;
    }
}