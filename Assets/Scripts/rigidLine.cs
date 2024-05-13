using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class rigidLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    EdgeCollider2D edgeCollider;
    Rigidbody2D myRigidbody;

    Vector3 prevPosition;
    List<Vector2> pointsData = new List<Vector2>();

    float minDistance = 0.4F;
    bool isDrawn = false;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        myRigidbody = GetComponent<Rigidbody2D>();
        edgeCollider = GetComponent<EdgeCollider2D>();

        transform.position = Vector3.zero;

        lineRenderer.positionCount = 1;
        lineRenderer.enabled = false;
        myRigidbody.bodyType = RigidbodyType2D.Static;

        prevPosition = Vector3.zero;
        pointsData.Clear();
    }

    void Update()
    {
        if (isDrawn) return;

        if (Input.GetMouseButtonDown(0))
        {
            lineRenderer.enabled = true;

            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0F;

            lineRenderer.SetPosition(0, new Vector3(currentPosition.x, currentPosition.y, 0.0F));

            pointsData.Add(new Vector2(currentPosition.x, currentPosition.y));

            prevPosition = currentPosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0F;

            if (Vector3.Distance(currentPosition, prevPosition) > minDistance)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(currentPosition.x, currentPosition.y, 0.0F));

                pointsData.Add(new Vector2(currentPosition.x, currentPosition.y));
                edgeCollider.SetPoints(pointsData);

                prevPosition = currentPosition;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            isDrawn = true;

            // 선이 생성되지 않은 채로 마우스를 뗐다면 점을 생성함
            if (lineRenderer.positionCount == 1)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));
                pointsData[0] = lineRenderer.GetPosition(0);

                if (edgeCollider != null) Destroy(edgeCollider);
                CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                circleCollider.radius = 0.1F;
            }

            myRigidbody.centerOfMass = new Vector2(pointsData.Select(p => p.x).Average(), pointsData.Select(p => p.y).Average());
        }
    }
    public bool GetIsDrawn()
    {
        return isDrawn;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(myRigidbody.worldCenterOfMass, 0.1F);
    }
}