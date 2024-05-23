using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 선 오브젝트들의 폴더역할을 할 껍데기 오브젝트
    public GameObject Lines;

    // 두 공들
    public GameObject pinkTarget;   // 핑크볼
    public GameObject blueTarget;   // 블루볼

    // 선 프리팹
    public GameObject rigidLinePrefab;

    // 그려지려고 대기중인 선 오브젝트를 담는 변수
    GameObject emptyLine;

    bool isFirstLineDrawn = false;
    bool canDrawLine = true;

    void Start()
    {
        CreateNewEmptyLine();
    }

    void Update()
    {
        if (!canDrawLine) return;

        // 게임이 끝나는 순간 더이상 그리는것을 막는다
        if (blueTarget.GetComponent<BlueTargetBall>().GetIsLevelEnd() == true)
        {
            // 아직 안 그리고 있는 경우 그냥 현재 대기중인 선을 삭제해버림
            if (emptyLine.GetComponent<rigidLine>().GetIsDrawing() == false)
            {
                Destroy(emptyLine);
            }
            else // 이미 그리기 시작한 경우 강제로 선을 끝내고 더이상 그리지 못하게 함
            {
                emptyLine.GetComponent<rigidLine>().EndLine();
            }

            canDrawLine = false;
            return;
        }

        // 지금 그리고 있는 선을 다 그릴때마다 새로운 선을 그릴수 있게 오브젝트를 대기시킨다
        if (emptyLine.GetComponent<rigidLine>().GetIsDrawn() == true)
        {
            // 첫 번째 선분을 다 그리는순간 블루볼과 핑크볼의 물리가 작용하게된다
            if (!isFirstLineDrawn)
            {
                isFirstLineDrawn = true;
                pinkTarget.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                pinkTarget.GetComponent<Rigidbody2D>().mass = 5.0F;
                blueTarget.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                blueTarget.GetComponent<Rigidbody2D>().mass = 5.0F;
            }

            // 더이상 필요없을 스크립트는 버리고 새 스크립트로 갈아끼워주자
            Destroy(emptyLine.GetComponent<rigidLine>());
            emptyLine.AddComponent<DestroyFalling>();
            CreateNewEmptyLine();
        }
    }

    void CreateNewEmptyLine()
    {
        // 다 그린 선 오브젝트는 별도로 관리하지 않고 그냥 같은 변수에 새로 Instantiate
        emptyLine = Instantiate(rigidLinePrefab);
        emptyLine.transform.SetParent(Lines.transform);
    }
}