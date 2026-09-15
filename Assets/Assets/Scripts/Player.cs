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

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (GameManager.Instance != null)
        {
            currentHealth = GameManager.Instance.savedHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }

        currentEmotionalState = maxEmotionalState;

        Debug.Log("PLAYER INICIALIZADO - VIDA: " + currentHealth);
    }


    void Update()
    {
        if (!canMove)
        {
            movementDirection = Vector2.zero;

            animator.SetBool("isWalking", false);
            animator.SetBool("isUp", false);
            animator.SetBool("isDown", false);

            return;
        }

        movementDirection = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        // Voltear personaje horizontalmente
        if (movementDirection.x > 0)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (movementDirection.x < 0)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }

        // Apagar todas las animaciones
        animator.SetBool("isWalking", false);
        animator.SetBool("isUp", false);
        animator.SetBool("isDown", false);

        // Activar solamente una
        if (Mathf.Abs(movementDirection.x) == 1)
        {
            animator.SetBool("isWalking", true);
        }
        else if (movementDirection.y == 1)
        {
            animator.SetBool("isUp", true);
        }
        else if (movementDirection.y == -1)
        {
            animator.SetBool("isDown", true);
        }
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


    // Movimiento

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled)
        {
            movementDirection = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
    }


    // Vida

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


    // Estado emocional

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


    // Monedas

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


    // Obtener información

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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();

        if (enemy == null)
            return;

        Debug.Log("COLISIÓN CON ENEMIGO: " + collision.gameObject.name);

        TakeDamage(1);

        Destroy(collision.gameObject);
    }
}