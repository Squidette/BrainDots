using UnityEngine;

public class rigidLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody2D myRigidbody;

    // 컴포지트 콜라이더의 재료로 쓰일 원시 콜라이더
    public GameObject hexagonCollider; // 원 콜라이더를 대체할 육각형 콜라이더
    public GameObject boxCollider; // 선 콜라이더를 대체할 박스 콜라이더

    // 컴포지트 콜라이더가 원시 콜라이더들을 합쳐 준다
    CompositeCollider2D compositeCollider;

    // 아직 확정되지 않은 부분을 시각화해줄 별도의 라인 렌더러
    unconfirmedLineVisualize visualizer;

    // 처음 마우스가 클릭된 위치
    Vector3 initialMouseDownPosition;

    // 그려진 점들의 개수와 좌표 총합을 담는 구조체 (질량과 무게중심 설정에 씀)
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
        if (visualizer == null) Debug.LogWarning("unconfimedLineVisualizer를 찾지 못했어요");
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

        if (Input.GetMouseButtonDown(0)) /// 첫 클릭시
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
        else if (Input.GetMouseButton(0)) /// 누르면서 움직일 때
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0F;

            visualizer.SetEndPointPosition(currentPosition);

            if (Vector3.Distance(currentPosition, visualizer.GetStartPointPosition()) > minDistance)
            {
                ConfirmPoint(ref currentPosition);
            }
        }
        else if (Input.GetMouseButtonUp(0)) /// 마우스를 뗐을 때
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0.0F;

            ConfirmPoint(ref currentPosition);

            // 처음 클릭했던 위치에도 점 콜라이더를 생성해 주자
            Instantiate(hexagonCollider, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

            // 질량과 무게중심 설정
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            myRigidbody.mass = myPoints.Count;
            myRigidbody.centerOfMass = new Vector2(myPoints.GetAverageCoords_X(), myPoints.GetAverageCoords_Y());

            // 원시 콜라이더들을 재료로 해서 컴포지트 콜라이더가 도형을 최적화하게 한다
            compositeCollider.GenerateGeometry();

            // 새롭게 만들어진 콜라이더 정보를 컴포지트 콜라이더가 가지고 있으므로 원시 콜라이더들은 제거해줌
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            visualizer.gameObject.SetActive(false);
            isDrawing = false;
            isDrawn = true;
        }
    }

    // 점을 확정하고 그려 준다
    void ConfirmPoint(ref Vector3 pos)
    {
        if (visualizer.GetIsTriggered()) return;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);

        // 점 위에 육각형 콜라이더 만들기
        Instantiate(hexagonCollider, new Vector3(pos.x, pos.y, 0.0F), Quaternion.identity).transform.parent = transform;

        // 박스콜라이더를 선처럼 사용해서 이전 점과 이어주기
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