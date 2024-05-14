using UnityEngine;

public class rigidLineGenerate : MonoBehaviour
{
    public GameObject rigidLinePrefab;
    GameObject emptyLine;

    void Start()
    {
        CreateNewEmptyLine();
    }

    void Update()
    {
        if (emptyLine.GetComponent<rigidLine>().GetIsDrawn() == true)
        {
            CreateNewEmptyLine();
        }
    }

    void CreateNewEmptyLine()
    {
        emptyLine = Instantiate(rigidLinePrefab);
        emptyLine.transform.parent = this.transform;
    }
}