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

    //void OnDrawGizmos() // 무게중심 표시하기
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(GetComponent<Rigidbody2D>().worldCenterOfMass, 0.1F);
    //}
}