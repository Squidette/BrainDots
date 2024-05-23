using UnityEngine;

// ���� Ȯ������ ���� ���� �ð�ȭ ����
// �ڽ��ݶ��̴��� ���±濡 �ɸ��°� ������ Ȯ��

public class unconfirmedLineVisualize : MonoBehaviour
{
    LineRenderer lineRenderer;
    GameObject boxCollider;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        boxCollider = transform.Find("Collider").gameObject;
        if (boxCollider == null) Debug.LogWarning("Collider�� ã�� ���߾��");
    }

    public void SetStartPointPosition(Vector3 input)
    {
        lineRenderer.SetPosition(0, input);
        UpdateBoxColliderPosition();
    }

    public Vector3 GetStartPointPosition()
    {
        return lineRenderer.GetPosition(0);
    }

    public void SetEndPointPosition(Vector3 input)
    {
        lineRenderer.SetPosition(1, input);
        UpdateBoxColliderPosition();
    }

    public Transform GetBoxColliderTransform()
    {
        return boxCollider.transform;
    }

    public bool GetIsTriggered()
    {
        return boxCollider.GetComponent<TriggerDetect>().isTriggered;
    }

    void UpdateBoxColliderPosition()
    {
        // ���η������� �� ���� ��ġ�� ���� �ڽ��ݶ��̴� ��ġ ���
        Vector3 pos = new Vector3(
            (lineRenderer.GetPosition(0).x + lineRenderer.GetPosition(1).x) / 2,
            (lineRenderer.GetPosition(0).y + lineRenderer.GetPosition(1).y) / 2,
            0.0F
            );

        float distance = Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
        if (distance < 0.01F) distance = 0.01f;

        float angle = Mathf.Atan2(
            lineRenderer.GetPosition(1).y - lineRenderer.GetPosition(0).y,
            lineRenderer.GetPosition(1).x - lineRenderer.GetPosition(0).x
            ) * Mathf.Rad2Deg;

        // ����� ��ġ�� �ڽ��ݶ��̴��� ����
        boxCollider.transform.position = pos;
        boxCollider.transform.localScale = new Vector3(distance, boxCollider.transform.localScale.y, boxCollider.transform.localScale.z);
        boxCollider.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}