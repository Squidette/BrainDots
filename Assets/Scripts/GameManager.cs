using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject pinkTarget;
    public GameObject blueTarget;

    public GameObject rigidLinePrefab;
    GameObject emptyLine;

    bool isFirstLineDrawn = false;

    void Start()
    {
        CreateNewEmptyLine();
    }

    void Update()
    {
        if (emptyLine.GetComponent<rigidLine>().GetIsDrawn() == true)
        {
            if (!isFirstLineDrawn)
            {
                isFirstLineDrawn = true;
                pinkTarget.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                blueTarget.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }

            Destroy(emptyLine.GetComponent<rigidLine>());
            emptyLine.AddComponent<DestroyFalling>();
            CreateNewEmptyLine();
        }
    }

    void CreateNewEmptyLine()
    {
        emptyLine = Instantiate(rigidLinePrefab);
        emptyLine.transform.parent = this.transform;
    }
}