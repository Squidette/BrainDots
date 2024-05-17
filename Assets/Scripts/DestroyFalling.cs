using UnityEngine;

public class DestroyFalling : MonoBehaviour
{
    void FixedUpdate()
    {
        if (transform.position.y < -30)
        {
            Destroy(gameObject);
        }
    }
}