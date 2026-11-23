using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PuckController : MonoBehaviour
{
    [Header("Puck Settings")]
    public float startSpeed = 4f;
    public float maxSpeed = 9f;

    [Header("Random Spawn")]
    public float minSpawnX = 0.8f;
    public float maxSpawnX = 2.5f;
    public float minSpawnY = -1.5f;
    public float maxSpawnY = 1.5f;

    [Header("Anti Stuck Settings")]
    public float horizontalMovementTimeout = 2.5f;
    public float minimumVerticalSpeed = 0.35f;

    [Header("Corner Recovery")]
    public float stuckCheckInterval = 1.5f;
    public float minimumMovementDistance = 0.08f;

    [Header("AI Corner Detection")]
    public Transform aiPaddle;
    public float cornerDistance = 0.8f;
    public float wallDistance = 0.7f;
    public float escapeForce = 5f;

    private Rigidbody2D rb;
    private bool isStopped = false;

    private float horizontalTimer;
    private float stuckTimer;

    private Vector2 lastCheckedPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ResetAndLaunch();
        lastCheckedPosition = rb.position;
    }

    public void ResetAndLaunch()
    {
        isStopped = false;
        rb.simulated = true;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        horizontalTimer = 0f;
        stuckTimer = 0f;

        bool spawnOnPlayerSide = Random.value < 0.5f;

        float spawnX = Random.Range(minSpawnX, maxSpawnX);

        if (spawnOnPlayerSide)
            spawnX = -spawnX;

        float spawnY = Random.Range(minSpawnY, maxSpawnY);

        Vector2 spawnPosition = new Vector2(spawnX, spawnY);

        rb.position = spawnPosition;

        float directionX = spawnOnPlayerSide ? 1f : -1f;
        float directionY = Random.Range(-0.5f, 0.5f);

        Vector2 direction =
            new Vector2(directionX, directionY).normalized;

        rb.velocity = direction * startSpeed;

        lastCheckedPosition = spawnPosition;

        Debug.Log(
            "Puck spawn: " +
            (spawnOnPlayerSide ? "Player Side" : "AI Side")
        );
    }

    public void StopPuck()
    {
        isStopped = true;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.simulated = false;

        Debug.Log("Permainan selesai. Puck benar-benar dihentikan.");
    }

    private void FixedUpdate()
    {
        if (isStopped)
            return;

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        CheckAICornerTrap();
        CheckHorizontalMovement();
        CheckStuckMovement();
    }

    // Mendeteksi puck yang terus bergerak horizontal
    private void CheckHorizontalMovement()
    {
        Vector2 velocity = rb.velocity;

        if (Mathf.Abs(velocity.x) > 0.5f &&
            Mathf.Abs(velocity.y) < minimumVerticalSpeed)
        {
            horizontalTimer += Time.fixedDeltaTime;

            if (horizontalTimer >= horizontalMovementTimeout)
            {
                AddVerticalDirection();

                horizontalTimer = 0f;

                Debug.Log("Puck mendapatkan arah pantulan otomatis.");
            }
        }
        else
        {
            horizontalTimer = 0f;
        }
    }

    // Mendeteksi puck yang berhenti atau tersangkut
    private void CheckStuckMovement()
    {
        stuckTimer += Time.fixedDeltaTime;

        if (stuckTimer >= stuckCheckInterval)
        {
            float distance = Vector2.Distance(
                rb.position,
                lastCheckedPosition
            );

            if (distance < minimumMovementDistance)
            {
                RecoverFromStuck();
            }

            lastCheckedPosition = rb.position;
            stuckTimer = 0f;
        }
    }

    // Memberikan arah diagonal ketika gerakan terlalu horizontal
    private void AddVerticalDirection()
    {
        float verticalDirection;

        if (Mathf.Abs(rb.position.y) > 0.2f)
        {
            verticalDirection = rb.position.y > 0f ? -1f : 1f;
        }
        else
        {
            verticalDirection = Random.value < 0.5f ? -1f : 1f;
        }

        float horizontalDirection = Mathf.Sign(rb.velocity.x);

        Vector2 newDirection = new Vector2(
            horizontalDirection,
            verticalDirection * 0.45f
        ).normalized;

        rb.velocity = newDirection *
            Mathf.Max(rb.velocity.magnitude, startSpeed);
    }

    private void CheckAICornerTrap()
    {
        if (aiPaddle == null)
            return;

        float distanceToAI = Vector2.Distance(
            rb.position,
            aiPaddle.position
        );

        bool nearRightWall = rb.position.x > 3.5f;

        bool nearTopWall = rb.position.y > 1.7f;
        bool nearBottomWall = rb.position.y < -1.7f;

        bool nearCorner = nearTopWall || nearBottomWall;

        if (distanceToAI < cornerDistance &&
            nearRightWall && nearCorner)
        {
            float escapeY = nearTopWall ? -1f : 1f;

            Vector2 escapeDirection = new Vector2(
                -1f,
                escapeY
            ).normalized;

            rb.velocity = escapeDirection * escapeForce;

            Debug.Log("Puck terjepit AI di sudut! Escape aktif.");

            horizontalTimer = 0f;
            stuckTimer = 0f;
            lastCheckedPosition = rb.position;
        }
    }

    // Mendorong puck keluar dari posisi yang tersangkut
    private void RecoverFromStuck()
    {
        Vector2 directionToCenter = -rb.position;

        if (directionToCenter.sqrMagnitude < 0.01f)
        {
            directionToCenter = Random.insideUnitCircle;
        }

        directionToCenter.y +=
            Random.value < 0.5f ? -0.5f : 0.5f;

        directionToCenter.Normalize();

        rb.velocity = directionToCenter *
            Mathf.Max(rb.velocity.magnitude, startSpeed);

        horizontalTimer = 0f;

        Debug.Log("Puck tersangkut! Posisi gerak dipulihkan otomatis.");
    }
}