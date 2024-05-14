using Unity.VisualScripting;
using UnityEngine;

public class rigidLine : MonoBehaviour
{
    LineRenderer lineRenderer;
    Rigidbody2D myRigidbody;

    // 컴포지트 콜라이더가 육각과 박스 콜라이더를 합쳐 준다
    CompositeCollider2D compositeCollider;

    public GameObject hexagonCollider; // 원 콜라이더를 대체할 육각형 콜라이더
    public GameObject boxCollider; // 선 콜라이더를 대체할 박스 콜라이더
    
    Vector3 prevPosition;
    Vector3 initialMouseDownPosition;

    float minDistance = 0.4F;
    bool isDoneDrawing = false;

    // AddValuesToPoints 함수로 더하고 있는 변수들
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

        if (Input.GetMouseButtonDown(0)) // 첫 클릭시
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

            if (Vector3.Distance(currentPosition, prevPosition) > minDistance) // 드래그시
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, new Vector3(currentPosition.x, currentPosition.y, 0.0F));

                // 점 위에 육각형 콜라이더 만들기
                Instantiate(hexagonCollider, new Vector3(currentPosition.x, currentPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

                // 점 사이를 박스콜라이더로 선처럼 이어주기
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
        else if (Input.GetMouseButtonUp(0)) // 마우스를 뗐을 때
        {
            myRigidbody.bodyType = RigidbodyType2D.Dynamic;
            isDoneDrawing = true;

            // 컴포지트 콜라이더가 콜라이더를 보정해준다
            compositeCollider.GenerateGeometry();
            myRigidbody.mass = PositionCount;

            // 드래그 단계를 거치지 않은 채로 마우스를 뗐다면 점을 생성함
            if (lineRenderer.positionCount == 1)
            {
                // 라인렌더러는 정점 정보가 2개 이상일때만 뭔가를 그려 주므로 2개를 생성해서 같은 정보를 담을 것
                lineRenderer.positionCount = 2;

                // 점은 마우스를 뗀 지점에 만들어주자
                Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPosition.z = 0.0F;
                lineRenderer.SetPosition(0, new Vector3(currentPosition.x, currentPosition.y, 0.0F));
                lineRenderer.SetPosition(1, lineRenderer.GetPosition(0));

                CircleCollider2D circleCollider = gameObject.AddComponent<CircleCollider2D>();
                circleCollider.radius = 0.05F;

                // 무게중심을 점이 있는 곳으로 바꿔 주기
                myRigidbody.centerOfMass = new Vector2(currentPosition.x, currentPosition.y);
            }
            else if (lineRenderer.positionCount > 1)
            {
                // 선이 생성되었다면 처음 클릭했던 위치에도 점 콜라이더를 생성해 주자
                Instantiate(hexagonCollider, new Vector3(initialMouseDownPosition.x, initialMouseDownPosition.y, 0.0F), Quaternion.identity).transform.parent = transform;

                // 무게중심을 점들의 평균위치로 바꿔 주기
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