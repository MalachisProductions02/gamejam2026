using UnityEngine;

public class ShotManager : MonoBehaviour
{
    [Header("Prefabs de pensamientos")]
    [SerializeField] private GameObject[] positiveThoughts;
    [SerializeField] private GameObject[] neutralThoughts;
    [SerializeField] private GameObject[] negativeThoughts;

    [Header("Disparo")]
    [SerializeField] private Transform shotPoint;
    [SerializeField] private float positiveDamage = 2f;
    [SerializeField] private float neutralDamage = 1f;
    [SerializeField] private float negativeDamage = 0.5f;

    [Header("Cooldown")]
    [SerializeField] private float shotCooldown = 0.5f;

    private float nextShotTime = 0f;

    [Header("Vida")]
    [SerializeField] private int positiveThreshold = 4;
    [SerializeField] private int neutralThreshold = 2;

    private Player player;
    private Camera mainCamera;

    private void Start()
    {
        player = GetComponent<Player>();
        mainCamera = Camera.main;

        if (player == null)
        {
            Debug.LogError(
                "ShotManager necesita estar en el mismo GameObject que Player."
            );
        }

        if (mainCamera == null)
        {
            Debug.LogError("No se encontró la cámara principal.");
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= nextShotTime)
            {
                Shoot();
                nextShotTime = Time.time + shotCooldown;
            }
        }
    }

    private void Shoot()
    {
        float health =
            player.GetCurrentHealth();

        GameObject[] selectedArray;
        float damage;

        if (health > positiveThreshold)
        {
            selectedArray = positiveThoughts;
            damage = positiveDamage;
        }
        else if (health > neutralThreshold)
        {
            selectedArray = neutralThoughts;
            damage = neutralDamage;
        }
        else
        {
            selectedArray = negativeThoughts;
            damage = negativeDamage;
        }

        if (selectedArray.Length == 0)
        {
            Debug.LogWarning(
                "El arreglo de pensamientos seleccionado está vacío."
            );
            return;
        }

        GameObject selectedThought =
            selectedArray[Random.Range(0, selectedArray.Length)];

        GameObject projectile = Instantiate(
            selectedThought,
            shotPoint.position,
            Quaternion.identity
        );

        ThoughtProjectile thought =
            projectile.GetComponent<ThoughtProjectile>();

        if (thought == null)
        {
            Debug.LogError(
                "El prefab del pensamiento no tiene ThoughtProjectile."
            );

            Destroy(projectile);
            return;
        }

        Vector3 mouseWorldPosition =
            mainCamera.ScreenToWorldPoint(Input.mousePosition);

        mouseWorldPosition.z = 0f;

        Vector2 direction =
            mouseWorldPosition - shotPoint.position;

        thought.Initialize(direction, damage);
    }
}