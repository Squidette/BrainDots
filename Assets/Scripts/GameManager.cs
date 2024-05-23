using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �� ������Ʈ���� ���������� �� ������ ������Ʈ
    public GameObject Lines;

    // �� ����
    public GameObject pinkTarget;   // ��ũ��
    public GameObject blueTarget;   // ��纼

    // �� ������
    public GameObject rigidLinePrefab;

    // �׷������� ������� �� ������Ʈ�� ��� ����
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

        // ������ ������ ���� ���̻� �׸��°��� ���´�
        if (blueTarget.GetComponent<BlueTargetBall>().GetIsLevelEnd() == true)
        {
            // ���� �� �׸��� �ִ� ��� �׳� ���� ������� ���� �����ع���
            if (emptyLine.GetComponent<rigidLine>().GetIsDrawing() == false)
            {
                Destroy(emptyLine);
            }
            else // �̹� �׸��� ������ ��� ������ ���� ������ ���̻� �׸��� ���ϰ� ��
            {
                emptyLine.GetComponent<rigidLine>().EndLine();
            }

            canDrawLine = false;
            return;
        }

        // ���� �׸��� �ִ� ���� �� �׸������� ���ο� ���� �׸��� �ְ� ������Ʈ�� ����Ų��
        if (emptyLine.GetComponent<rigidLine>().GetIsDrawn() == true)
        {
            // ù ��° ������ �� �׸��¼��� ��纼�� ��ũ���� ������ �ۿ��ϰԵȴ�
            if (!isFirstLineDrawn)
            {
                isFirstLineDrawn = true;
                pinkTarget.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                pinkTarget.GetComponent<Rigidbody2D>().mass = 5.0F;
                blueTarget.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                blueTarget.GetComponent<Rigidbody2D>().mass = 5.0F;
            }

            // ���̻� �ʿ���� ��ũ��Ʈ�� ������ �� ��ũ��Ʈ�� ���Ƴ�������
            Destroy(emptyLine.GetComponent<rigidLine>());
            emptyLine.AddComponent<DestroyFalling>();
            CreateNewEmptyLine();
        }
    }

    void CreateNewEmptyLine()
    {
        // �� �׸� �� ������Ʈ�� ������ �������� �ʰ� �׳� ���� ������ ���� Instantiate
        emptyLine = Instantiate(rigidLinePrefab);
        emptyLine.transform.SetParent(Lines.transform);
    }
}