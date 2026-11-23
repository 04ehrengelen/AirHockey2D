
using UnityEngine;

public class PaddleBoundary : MonoBehaviour
{
    [Header("Movement Boundaries")]
    public float minX = -4f;
    public float maxX = -0.35f;

    public float minY = -2f;
    public float maxY = 2f;

    private void LateUpdate()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(
            position.x,
            minX,
            maxX
        );

        position.y = Mathf.Clamp(
            position.y,
            minY,
            maxY
        );

        transform.position = position;
    }
}
