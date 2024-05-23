using UnityEditorInternal;
using UnityEngine;

public class rigidLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody2D myRigidbody;

    // ���� �ݶ��̴�
    public GameObject hexagonCollider; // �� ���� �׸���
    public GameObject boxCollider; // �ڽ� �ݶ��̴��� ��� �÷� ���� �� ���̸� �̾��� ��

    // ������Ʈ �ݶ��̴��� ���� �ݶ��̴����� ���� �ش�
    CompositeCollider2D compositeCollider;

    // ���� Ȯ������ ���� �κ��� �ð�ȭ���� ������ ���� ������ ���ӿ�����Ʈ
    unconfirmedLineVisualize visualizer;

    // ó�� ���콺�� Ŭ���� ��ġ
    Vector3 initialMouseDownPosition;

    float minDistance = 0.4F;
    bool isDrawing = false; // �׸��� ���ΰ�? StartLine ���۽� true�� �ȴ�
    bool isDrawn = false;   // �� �׷����°�? EndLine �Լ��� ������ true�� �ȴ� (isDrawing�� false�� ��)

    void Start()
    {
        // Get
        lineRenderer = GetComponent<LineRenderer>();
        myRigidbody = GetComponent<Rigidbody2D>();
        compositeCollider = GetComponent<CompositeCollider2D>();

        visualizer = transform.parent.Find("unconfimedLineVisualizer").gameObject.GetComponent<unconfirmedLineVisualize>();
        if (visualizer == null) Debug.LogWarning("unconfimedLineVisualizer�� ã�� ���߾��");
        visualizer.gameObject.SetActive(false);

        // Set
        transform.position = Vector3.zero;

        lineRenderer.positionCount = 1;
        lineRenderer.enabled = false;
        myRigidbody.bodyType = RigidbodyType2D.Static;

        initialMouseDownPosition = Vector3.zero;
    }

    void Update()
    {
        if (isDrawn) return;

        // �巡���� �� �ܰ� ����
        if (!isDrawing && Input.GetMouseButtonDown(0)) /// ���콺�� ù Ŭ����
        {
            StartLine();
        }
        else if (isDrawing && Input.GetMouseButton(0)) /// �����鼭 ������ ��
        {
            ExtendLine();
        }
        else if (isDrawing && Input.GetMouseButtonUp(0)) /// ���콺�� ���� ��
        {
            EndLine();
        }
    }

    // �׷��� ���� Ȯ���ϰ� �׷� �ش�
    void ConfirmPoint(ref Vector3 pos)
    {
        if (visualizer.GetIsTriggered()) return;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);

        // �� ���� ������ �ݶ��̴� �����
        Instantiate(hexagonCollider, new Vector3(pos.x, pos.y, 0.0F), Quaternion.identity).transform.parent = transform;

        // �ڽ��ݶ��̴��� ��ó�� ����ؼ� ���� ���� �̾��ֱ� - unconfirmedLineVisualizer�� Transform ������ �״�� ���
        GameObject boxLine = Instantiate(boxCollider, visualizer.GetBoxColliderTransform().position, visualizer.GetBoxColliderTransform().rotation);
        boxLine.transform.localScale = visualizer.GetBoxColliderTransform().localScale;
        boxLine.transform.parent = transform;

        visualizer.SetStartPointPosition(pos);
    }

    // ���η������� ������ �����߽��� ����
    Vector2 GetAvgPosOfLineRenderer()
    {
        Vector2 avg = Vector2.zero;

        float sum_x = 0.0F;
        float sum_y = 0.0F;

        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            sum_x += lineRenderer.GetPosition(i).x;
            sum_y += lineRenderer.GetPosition(i).y;
        }

        avg.x = sum_x / lineRenderer.positionCount;
        avg.y = sum_y / lineRenderer.positionCount;

        return avg;
    }

    void StartLine()
    {
        isDrawing = true;
        lineRenderer.enabled = true;

        initialMouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        initialMouseDownPosition.z = 0.0F;

        lineRenderer.SetPosition(0, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F));

        visualizer.SetStartPointPosition(initialMouseDownPosition);
        visualizer.SetEndPointPosition(initialMouseDownPosition);
        visualizer.gameObject.SetActive(true);
    }

    void ExtendLine()
    {
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = 0.0F;

        visualizer.SetEndPointPosition(currentPosition);

        if (Vector3.Distance(currentPosition, visualizer.GetStartPointPosition()) > minDistance)
        {
            ConfirmPoint(ref currentPosition);
        }
    }

    public void EndLine()
    {
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentPosition.z = 0.0F;

        if (lineRenderer.positionCount == 1) /// ª�� �׷��� ���� ������ �� �ݶ��̴��� ���� "��"�� ����
        {
            // ó������ Ȯ���� �ȵǾ��� ���� �׳� ����ϰ� ó������ �ٽ� �׸��� ��
            if (visualizer.GetIsTriggered())
            {
                visualizer.gameObject.SetActive(false);
                isDrawing = false;
                return;
            }

            // ���� ���콺�� �� ������ ���������
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(currentPosition.x, currentPosition.y, 0.0F));
            lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));

            // �ݶ��̴��� �� �ݶ��̴��� ��ü
            Destroy(compositeCollider);
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.1F / 2;

            // ������ �����߽� ����
            myRigidbody.mass = 1;
            myRigidbody.centerOfMass = new Vector2(currentPosition.x, currentPosition.y);
        }
        else if (lineRenderer.positionCount > 1) /// ������ ��� �׷����� �Ϲ����� ���� ����
        {
            // �̶��� ���� �� �׷��� ª�� ���̶� �׷��� �ɷ� �� �ֱ�.. �׷��� �� �ڿ������� �� ���Ƽ�
            ConfirmPoint(ref currentPosition);

            // ó�� Ŭ���ߴ� ��ġ���� �� �ݶ��̴��� ������ ����
            Instantiate(hexagonCollider, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

            // ������ �����߽� ����
            myRigidbody.mass = lineRenderer.positionCount;
            myRigidbody.centerOfMass = GetAvgPosOfLineRenderer();

            // ���� �ݶ��̴����� ���� �ؼ� ������Ʈ �ݶ��̴��� ������ ����ȭ�ϰ� �Ѵ�
            compositeCollider.GenerateGeometry();

            // ���Ӱ� ������� �ݶ��̴� ������ ������Ʈ �ݶ��̴��� ������ �����Ƿ� ���� �ݶ��̴����� ��������
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        // ��, �� ���������� ������ �͵�
        myRigidbody.bodyType = RigidbodyType2D.Dynamic;
        if (lineRenderer.positionCount < 40) myRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        visualizer.gameObject.SetActive(false);
        isDrawing = false;
        isDrawn = true;
    }

    public bool GetIsDrawn()
    {
        return isDrawn;
    }

    public bool GetIsDrawing()
    {
        return isDrawing;
    }
}