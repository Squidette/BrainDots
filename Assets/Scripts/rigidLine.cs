using UnityEditorInternal;
using UnityEngine;

public class rigidLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody2D myRigidbody;

    // 원시 콜라이더
    public GameObject hexagonCollider; // 점 위에 그릴것
    public GameObject boxCollider; // 박스 콜라이더를 길게 늘려 선과 선 사이를 이어줄 것

    // 컴포지트 콜라이더가 원시 콜라이더들을 합쳐 준다
    CompositeCollider2D compositeCollider;

    // 아직 확정되지 않은 부분을 시각화해줄 별도의 라인 렌더러 게임오브젝트
    unconfirmedLineVisualize visualizer;

    // 처음 마우스가 클릭된 위치
    Vector3 initialMouseDownPosition;

    float minDistance = 0.4F;
    bool isDrawing = false; // 그리는 중인가? StartLine 시작시 true가 된다
    bool isDrawn = false;   // 다 그려졌는가? EndLine 함수의 끝에서 true가 된다 (isDrawing은 false가 됨)

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
    }

    void Update()
    {
        if (isDrawn) return;

        // 드래그의 각 단계 구현
        if (!isDrawing && Input.GetMouseButtonDown(0)) /// 마우스로 첫 클릭시
        {
            StartLine();
        }
        else if (isDrawing && Input.GetMouseButton(0)) /// 누르면서 움직일 때
        {
            ExtendLine();
        }
        else if (isDrawing && Input.GetMouseButtonUp(0)) /// 마우스를 뗐을 때
        {
            EndLine();
        }
    }

    // 그려진 점을 확정하고 그려 준다
    void ConfirmPoint(ref Vector3 pos)
    {
        if (visualizer.GetIsTriggered()) return;

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos);

        // 점 위에 육각형 콜라이더 만들기
        Instantiate(hexagonCollider, new Vector3(pos.x, pos.y, 0.0F), Quaternion.identity).transform.parent = transform;

        // 박스콜라이더를 선처럼 사용해서 이전 점과 이어주기 - unconfirmedLineVisualizer의 Transform 정보를 그대로 사용
        GameObject boxLine = Instantiate(boxCollider, visualizer.GetBoxColliderTransform().position, visualizer.GetBoxColliderTransform().rotation);
        boxLine.transform.localScale = visualizer.GetBoxColliderTransform().localScale;
        boxLine.transform.parent = transform;

        visualizer.SetStartPointPosition(pos);
    }

    // 라인렌더러의 점들의 무게중심을 구함
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

        if (lineRenderer.positionCount == 1) /// 짧게 그려진 선은 별도의 원 콜라이더를 가진 "점"을 생성
        {
            // 처음부터 확정이 안되었던 선은 그냥 취소하고 처음부터 다시 그리게 함
            if (visualizer.GetIsTriggered())
            {
                visualizer.gameObject.SetActive(false);
                isDrawing = false;
                return;
            }

            // 점은 마우스를 뗀 지점에 만들어주자
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, new Vector3(currentPosition.x, currentPosition.y, 0.0F));
            lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));

            // 콜라이더는 원 콜라이더로 대체
            Destroy(compositeCollider);
            CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
            circleCollider.radius = 0.1F / 2;

            // 질량과 무게중심 설정
            myRigidbody.mass = 1;
            myRigidbody.centerOfMass = new Vector2(currentPosition.x, currentPosition.y);
        }
        else if (lineRenderer.positionCount > 1) /// 점보다 길게 그려진건 일반적인 선을 생성
        {
            // 이때는 아직 덜 그려진 짧은 선이라도 그려진 걸로 쳐 주기.. 그래야 더 자연스러울 것 같아서
            ConfirmPoint(ref currentPosition);

            // 처음 클릭했던 위치에도 점 콜라이더를 생성해 주자
            Instantiate(hexagonCollider, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

            // 질량과 무게중심 설정
            myRigidbody.mass = lineRenderer.positionCount;
            myRigidbody.centerOfMass = GetAvgPosOfLineRenderer();

            // 원시 콜라이더들을 재료로 해서 컴포지트 콜라이더가 도형을 최적화하게 한다
            compositeCollider.GenerateGeometry();

            // 새롭게 만들어진 콜라이더 정보를 컴포지트 콜라이더가 가지고 있으므로 원시 콜라이더들은 제거해줌
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        // 점, 선 공통적으로 설정할 것들
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