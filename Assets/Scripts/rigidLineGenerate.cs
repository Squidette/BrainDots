using UnityEngine;

public class rigidLineGenerate : MonoBehaviour
{
    public GameObject rigidLinePrefab;
    GameObject emptyLine;

    void Start()
    {
        emptyLine = Instantiate(rigidLinePrefab);
        emptyLine.transform.parent = this.transform;
    }

    void Update()
    {
        if (emptyLine.GetComponent<rigidLine>().GetIsDrawn() == true)
        {
            emptyLine = Instantiate(rigidLinePrefab);
            emptyLine.transform.parent = this.transform;
        }
    }
}