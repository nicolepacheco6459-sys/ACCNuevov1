using DG.Tweening.Core.Easing;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [Header("Sprite Renderer")]
    public SpriteRenderer gunSpriteRenderer;

    [Header("Gun Sprites")]
    public Sprite level1Gun;
    public Sprite level2Gun;
    public Sprite level3Gun;

    [Header("Shooting")]
    public PlayerShooting playerShooting;

    void Start()
    {
        SetGunLevel(((GameManager_SHOOTER)GameManager_SHOOTER.instance).currentLevel);
    }

    public void SetGunLevel(int level)
    {
        switch (level)
        {
            case 1:

                gunSpriteRenderer.sprite = level1Gun;

                playerShooting.damage = 1;

                playerShooting.fireRate = 0.5f;

                break;

            case 2:

                gunSpriteRenderer.sprite = level2Gun;

                playerShooting.damage = 2;

                playerShooting.fireRate = 0.3f;

                break;

            case 3:

                gunSpriteRenderer.sprite = level3Gun;

                playerShooting.damage = 3;

                playerShooting.fireRate = 0.15f;

                break;
        }
    }
}
