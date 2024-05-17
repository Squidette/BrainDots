using UnityEngine;

public class TriggerDetect : MonoBehaviour
{
    public bool isTriggered;

    void Start()
    {
        isTriggered = false;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col) isTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col) isTriggered = false;
    }
}