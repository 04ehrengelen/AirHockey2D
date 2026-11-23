using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIController : MonoBehaviour
{
    [Header("Puck References")]
    public Transform puck;
    public Rigidbody2D puckRb;

    [Header("AI Movement")]
    public float moveSpeed = 5f;
    public float attackOffset = 0.45f;

    [Header("AI Position")]
    public float homeX = 3.2f;
    public float homeY = 0f;

    [Header("Movement Boundaries")]
    public float minX = 0.35f;
    public float maxX = 4f;
    public float minY = -2f;
    public float maxY = 2f;

    [Header("Prediction Settings")]
    public float interceptX = 3.1f;
    public float minimumPuckSpeed = 0.1f;

    [Header("Corner Behavior")]
    public float cornerAvoidanceX = 3.2f;
    public float cornerAvoidanceY = 1.45f;
    public float cornerSafeY = 1.2f;
    public float cornerSpeedMultiplier = 0.65f;

    private Rigidbody2D rb;

    private Vector2 targetPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (puck == null)
        {
            Debug.LogError("Puck belum dihubungkan!");
            enabled = false;
            return;
        }

        if (puckRb == null)
        {
            puckRb = puck.GetComponent<Rigidbody2D>();
        }

        if (puckRb == null)
        {
            Debug.LogError("Rigidbody2D Puck tidak ditemukan!");
            enabled = false;
            return;
        }

        SetDifficulty(GameSettings.difficulty);
    }

    private void FixedUpdate()
    {
        if (puck == null || puckRb == null)
            return;

        Vector2 puckPosition = puck.position;
        Vector2 puckVelocity = puckRb.velocity;

        float currentMoveSpeed = moveSpeed;

        if (puckPosition.x > 0f)
        {
            if (IsPuckInCorner(puckPosition))
            {
                AvoidCorner(puckPosition);

                // Kurangi kecepatan AI di area pojok
                currentMoveSpeed *= cornerSpeedMultiplier;
            }
            else
            {
                ChasePuck(puckPosition);
            }
        }
        else if (puckVelocity.x > minimumPuckSpeed)
        {
            DefendPuck(puckPosition, puckVelocity);
        }
        else
        {
            ReturnToHome();
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            currentMoveSpeed * Time.fixedDeltaTime
        );

        rb.MovePosition(newPosition);
    }

    private bool IsPuckInCorner(Vector2 puckPosition)
    {
        return puckPosition.x >= cornerAvoidanceX &&
               Mathf.Abs(puckPosition.y) >= cornerAvoidanceY;
    }

    private void AvoidCorner(Vector2 puckPosition)
    {
        float safeY = puckPosition.y > 0f
            ? cornerSafeY
            : -cornerSafeY;

        float safeX = Mathf.Min(
            puckPosition.x + attackOffset,
            maxX - 0.5f
        );

        targetPosition = new Vector2(safeX, safeY);
    }

    private void ChasePuck(Vector2 puckPosition)
    {
        // AI bergerak ke sebelah kanan puck
        // agar dapat mendorong puck ke kiri
        targetPosition = new Vector2(
            puckPosition.x + attackOffset,
            puckPosition.y
        );
    }

    private void DefendPuck(
        Vector2 puckPosition,
        Vector2 puckVelocity
    )
    {
        // Menghitung waktu puck mencapai garis pertahanan
        float timeToIntercept =
            (interceptX - puckPosition.x) / puckVelocity.x;

        timeToIntercept = Mathf.Max(
            0f,
            timeToIntercept
        );

        // Prediksi posisi Y puck
        float predictedY =
            puckPosition.y +
            puckVelocity.y * timeToIntercept;

        // Memperhitungkan pantulan pada dinding atas/bawah
        predictedY = ReflectY(predictedY);

        targetPosition = new Vector2(
            homeX,
            predictedY
        );
    }

    private void ReturnToHome()
    {
        targetPosition = new Vector2(
            homeX,
            homeY
        );
    }

    private float ReflectY(float predictedY)
    {
        float range = maxY - minY;

        float reflected = Mathf.PingPong(
            predictedY - minY,
            range * 2f
        );

        return minY + reflected;
    }

    public void SetDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 0: // Easy
                moveSpeed = 3f;
                attackOffset = 0.7f;
                break;

            case 1: // Medium
                moveSpeed = 5f;
                attackOffset = 0.45f;
                break;

            case 2: // Hard
                moveSpeed = 7f;
                attackOffset = 0.25f;
                break;
        }

        Debug.Log("AI Difficulty diterapkan: " + difficulty);
    }
}