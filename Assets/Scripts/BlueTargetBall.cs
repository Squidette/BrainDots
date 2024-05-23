using UnityEngine;

public class BlueTargetBall : MonoBehaviour
{
    public ParticleSystem myParticle;

    // 두 공이 만난곳에서부터 이펙트로 동그랗게 퍼질 원
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
        // 게임 클리어 조건: 블루볼과 핑크볼이 만난다
        if (col.gameObject.tag == "PinkTargetBall")
        {
            isLevelEnd = true;
            Time.timeScale = 0;

            myParticle.transform.position = new Vector3(
                col.contacts[0].point.x,
                col.contacts[0].point.y,
                -5.0F // 파티클이 3D 시스템이라 소팅레이어 대신 Z축 거리로 순서 조정
                );
            myParticle.gameObject.SetActive(true);
            myParticle.Emit(10);

            haloSprite.transform.position = col.contacts[0].point;
            haloSprite.SetActive(true);

            //스크린 캡처
            //ScreenCapture.CaptureScreenshot("test.png");
        }
    }

    void Update()
    {
        if (haloSprite.activeSelf)
        {
            if (haloSprite.transform.localScale.x < maximumHaloScale)
            {
                // 점점 커지게
                haloSprite.transform.localScale += new Vector3(Time.unscaledDeltaTime * maximumHaloScale, Time.unscaledDeltaTime * maximumHaloScale, 0.0F);

                // 점점 투명해지게
                haloSprite.GetComponent<SpriteRenderer>().color = new Color(
                    haloSprite.GetComponent<SpriteRenderer>().color.r,
                    haloSprite.GetComponent<SpriteRenderer>().color.g,
                    haloSprite.GetComponent<SpriteRenderer>().color.b,
                    1 - haloFadeSpeed * haloSprite.transform.localScale.x / maximumHaloScale
                    );
            }
            else // 다 펴지는데 약 1초가 걸릴것이다
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