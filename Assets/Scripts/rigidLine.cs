using UnityEngine;

public class rigidLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody2D myRigidbody;

    // ������Ʈ �ݶ��̴��� ���� ���� ���� �ݶ��̴�
    public GameObject hexagonCollider; // �� �ݶ��̴��� ��ü�� ������ �ݶ��̴�
    public GameObject boxCollider; // �� �ݶ��̴��� ��ü�� �ڽ� �ݶ��̴�

    // ������Ʈ �ݶ��̴��� ���� �ݶ��̴����� ���� �ش�
    CompositeCollider2D compositeCollider;

    // ���� Ȯ������ ���� �κ��� �ð�ȭ���� ������ ���� ������
    unconfirmedLineVisualize visualizer;

    // ó�� ���콺�� Ŭ���� ��ġ
    Vector3 initialMouseDownPosition;

    // �׷��� ������ ������ ��ǥ ������ ��� ����ü (������ �����߽� ������ ��)
    CoordsSum myPoints;

    float minDistance = 0.4F;
    bool isDrawing = false;
    bool isDrawn = false;

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

        myPoints = new CoordsSum(0.0F, 0.0F, 0);
    }

    void Update()
    {
        if (isDrawn) return;

        if (Input.GetMouseButtonDown(0)) /// ù Ŭ����
        {
            isDrawing = true;
            lineRenderer.enabled = true;

            initialMouseDownPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            initialMouseDownPosition.z = 0.0F;

            lineRenderer.SetPosition(0, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F));

            visualizer.SetStartPointPosition(initialMouseDownPosition);
            visualizer.SetEndPointPosition(initialMouseDownPosition);
            visualizer.gameObject.SetActive(true);

            myPoints.AddPointInfo(initialMouseDownPosition.x, initialMouseDownPosition.y);
        }
        else if (Input.GetMouseButton(0)) /// �����鼭 ������ ��
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0F;

            visualizer.SetEndPointPosition(currentPosition);

            if (Vector3.Distance(currentPosition, visualizer.GetStartPointPosition()) > minDistance)
            {
                ConfirmPoint(ref currentPosition);
            }
        }
        else if (Input.GetMouseButtonUp(0)) /// ���콺�� ���� ��
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0F;

            ConfirmPoint(ref currentPosition);

            // ó�� Ŭ���ߴ� ��ġ���� �� �ݶ��̴��� ������ ����
            Instantiate(hexagonCollider, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

            // ������ �����߽� ����
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            myRigidbody.mass = myPoints.Count;
            myRigidbody.centerOfMass = new Vector2(myPoints.GetAverageCoords_X(), myPoints.GetAverageCoords_Y());

            // ���� �ݶ��̴����� ���� �ؼ� ������Ʈ �ݶ��̴��� ������ ����ȭ�ϰ� �Ѵ�
            compositeCollider.GenerateGeometry();

            // ���Ӱ� ������� �ݶ��̴� ������ ������Ʈ �ݶ��̴��� ������ �����Ƿ� ���� �ݶ��̴����� ��������
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            visualizer.gameObject.SetActive(false);
            isDrawing = false;
            isDrawn = true;
        }
    }

    // ���� Ȯ���ϰ� �׷� �ش�
    void ConfirmPoint(ref Vector3 pos)
    {
        if (visualizer.GetIsTriggered()) return;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);

        // �� ���� ������ �ݶ��̴� �����
        Instantiate(hexagonCollider, new Vector3(pos.x, pos.y, 0.0F), Quaternion.identity).transform.parent = transform;

        // �ڽ��ݶ��̴��� ��ó�� ����ؼ� ���� ���� �̾��ֱ�
        GameObject boxLine = Instantiate(boxCollider, visualizer.GetBoxColliderTransform().position, visualizer.GetBoxColliderTransform().rotation);
        boxLine.transform.localScale = visualizer.GetBoxColliderTransform().localScale;
        boxLine.transform.parent = transform;

        visualizer.SetStartPointPosition(pos);
        myPoints.AddPointInfo(pos.x, pos.y);
    }

    public bool GetIsDrawn()
    {
        return isDrawn;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (myRigidbody) Gizmos.DrawSphere(myRigidbody.worldCenterOfMass, 0.1F);
    }

    struct CoordsSum
    {
        float coordsSum_X;
        float coordsSum_Y;
        int count;

        public int Count { get { return count; } }

        public CoordsSum(float x, float y, int c)
        {
            coordsSum_X = x;
            coordsSum_Y = y;
            count = c;
        }

        public void AddPointInfo(float x, float y)
        {
            coordsSum_X += x;
            coordsSum_Y += y;
            count++;
        }

        public float GetAverageCoords_X() { return coordsSum_X / count; }
        public float GetAverageCoords_Y() { return coordsSum_Y / count; }
    }
}