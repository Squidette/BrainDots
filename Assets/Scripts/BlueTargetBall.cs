using UnityEngine;

public class BlueTargetBall : MonoBehaviour
{
    public ParticleSystem myParticle;

    // �� ���� �������������� ����Ʈ�� ���׶��� ���� ��
    public GameObject haloSprite;

    bool isLevelEnd;

    int maximumHaloScale = 6;
    float haloFadeSpeed = 2.0F;

    void Start()
    {
        isLevelEnd = false;

        haloSprite.SetActive(false);
        haloSprite.transform.localScale = new Vector3(0.0F, 0.0F, 1.0F);

        myParticle.gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // ���� Ŭ���� ����: ��纼�� ��ũ���� ������
        if (col.gameObject.tag == "PinkTargetBall")
        {
            isLevelEnd = true;
            Time.timeScale = 0;

            myParticle.transform.position = new Vector3(
                col.contacts[0].point.x,
                col.contacts[0].point.y,
                -5.0F // ��ƼŬ�� 3D �ý����̶� ���÷��̾� ��� Z�� �Ÿ��� ���� ����
                );
            myParticle.gameObject.SetActive(true);
            myParticle.Emit(10);

            haloSprite.transform.position = col.contacts[0].point;
            haloSprite.SetActive(true);

            //��ũ�� ĸó
            //ScreenCapture.CaptureScreenshot("test.png");
        }
    }

    void Update()
    {
        if (haloSprite.activeSelf)
        {
            if (haloSprite.transform.localScale.x < maximumHaloScale)
            {
                // ���� Ŀ����
                haloSprite.transform.localScale += new Vector3(Time.unscaledDeltaTime * maximumHaloScale, Time.unscaledDeltaTime * maximumHaloScale, 0.0F);

                // ���� ����������
                haloSprite.GetComponent<SpriteRenderer>().color = new Color(
                    haloSprite.GetComponent<SpriteRenderer>().color.r,
                    haloSprite.GetComponent<SpriteRenderer>().color.g,
                    haloSprite.GetComponent<SpriteRenderer>().color.b,
                    1 - haloFadeSpeed * haloSprite.transform.localScale.x / maximumHaloScale
                    );
            }
            else // �� �����µ� �� 1�ʰ� �ɸ����̴�
            {
                haloSprite.SetActive(false);
            }
        }
    }

    public bool GetIsLevelEnd()
    {
        return isLevelEnd;
    }
}