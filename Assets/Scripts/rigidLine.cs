using Unity.VisualScripting;
using UnityEngine;

public class rigidLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody2D myRigidbody;

    // ������Ʈ �ݶ��̴��� ������ �ڽ� �ݶ��̴��� ���� �ش�
    CompositeCollider2D compositeCollider;

    public GameObject hexagonCollider; // �� �ݶ��̴��� ��ü�� ������ �ݶ��̴�
    public GameObject boxCollider; // �� �ݶ��̴��� ��ü�� �ڽ� �ݶ��̴�
    
    Vector3 prevPosition;
    Vector3 initialMouseDownPosition;

    float minDistance = 0.4F;
    bool isDoneDrawing = false;

    // AddValuesToPoints �Լ��� ���ϰ� �ִ� ������
    float PositionSumX = 0.0F;
    float PositionSumY = 0.0F;
    int PositionCount = 0;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        myRigidbody = GetComponent<Rigidbody2D>();
        compositeCollider = GetComponent<CompositeCollider2D>();

        transform.position = Vector3.zero;

        lineRenderer.positionCount = 1;
        lineRenderer.enabled = false;
        myRigidbody.bodyType = RigidbodyType2D.Static;

        prevPosition = Vector3.zero;
        initialMouseDownPosition = Vector3.zero;
    }

    void Update()
    {
        if (isDoneDrawing) return;

        if (Input.GetMouseButtonDown(0)) // ù Ŭ����
        {
            lineRenderer.enabled = true;

            initialMouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            initialMouseDownPosition.z = 0.0F;

            lineRenderer.SetPosition(0, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F));

            AddValuesToPoints(initialMouseDownPosition.x, initialMouseDownPosition.y);
            prevPosition = initialMouseDownPosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0F;

            if (Vector3.Distance(currentPosition, prevPosition) > minDistance) // �巡�׽�
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(currentPosition.x, currentPosition.y, 0.0F));

                // �� ���� ������ �ݶ��̴� �����
                Instantiate(hexagonCollider, new Vector3(currentPosition.x, currentPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

                // �� ���̸� �ڽ��ݶ��̴��� ��ó�� �̾��ֱ�
                Vector3 boxPosition = new Vector3((prevPosition.x + currentPosition.x) / 2, (prevPosition.y + currentPosition.y) / 2, 0.0F);
                float distance = Vector2.Distance(currentPosition, prevPosition);
                float angle = Mathf.Atan2(currentPosition.y - prevPosition.y, currentPosition.x - prevPosition.x) * Mathf.Rad2Deg;
                var boxLine = Instantiate(boxCollider, boxPosition, Quaternion.Euler(0, 0, angle));
                boxLine.transform.localScale = new Vector3(distance, boxLine.transform.localScale.y, boxLine.transform.localScale.z);
                boxLine.transform.parent = transform;

                AddValuesToPoints(currentPosition.x, currentPosition.y);
                prevPosition = currentPosition;
            }
        }
        else if (Input.GetMouseButtonUp(0)) // ���콺�� ���� ��
        {
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            isDoneDrawing = true;

            // ������Ʈ �ݶ��̴��� �ݶ��̴��� �������ش�
            compositeCollider.GenerateGeometry();
            myRigidbody.mass = PositionCount;

            // �巡�� �ܰ踦 ��ġ�� ���� ä�� ���콺�� �ôٸ� ���� ������
            if (lineRenderer.positionCount == 1)
            {
                // ���η������� ���� ������ 2�� �̻��϶��� ������ �׷� �ֹǷ� 2���� �����ؼ� ���� ������ ���� ��
                lineRenderer.positionCount = 2;

                // ���� ���콺�� �� ������ ���������
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPosition.z = 0.0F;
                lineRenderer.SetPosition(0, new Vector3(currentPosition.x, currentPosition.y, 0.0F));
                lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));

                CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                circleCollider.radius = 0.05F;

                // �����߽��� ���� �ִ� ������ �ٲ� �ֱ�
                myRigidbody.centerOfMass = new Vector2(currentPosition.x, currentPosition.y);
            }
            else if (lineRenderer.positionCount > 1)
            {
                // ���� �����Ǿ��ٸ� ó�� Ŭ���ߴ� ��ġ���� �� �ݶ��̴��� ������ ����
                Instantiate(hexagonCollider, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

                // �����߽��� ������ �����ġ�� �ٲ� �ֱ�
                myRigidbody.centerOfMass = new Vector2(PositionSumX / PositionCount, PositionSumY / PositionCount);
            }
        }
    }

    void AddValuesToPoints(float x, float y)
    {
        PositionSumX += x;
        PositionSumY += y;
        PositionCount++;
    }

    public bool GetIsDrawn()
    {
        return isDoneDrawing;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (myRigidbody) Gizmos.DrawSphere(myRigidbody.worldCenterOfMass, 0.1F);
    }
}