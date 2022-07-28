using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public event Action PlayerDead = delegate { };

    public int Speed => speed;

    [SerializeField]
    private PlayerInput playerInput = null;
    [SerializeField]
    private UnitData baseUnitData = null;
    [SerializeField]
    private int speed = 300;

    private float cooldown;
    private float halfScreenWidth;
    private float halfPlayeWidth;

    private void Update()
    {
        cooldown -= Time.deltaTime;
    }

    public override void Init(ObjectPool<Bullet> pool)
    {
        base.Init(pool);
        playerInput.Init(this);
        halfScreenWidth = Screen.width / 2f;
        halfPlayeWidth = rectTransform.sizeDelta.x / 2f;
    }

    public void Setup()
    {
        CurrentUnitData = baseUnitData.Duplicate();
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SetShow(bool show)
    {
        gameObject.SetActive(show);
    }

    public void Move(float value)
    {
        rectTransform.anchoredPosition = new Vector2(Mathf.Clamp(rectTransform.anchoredPosition.x + value, -halfScreenWidth + halfPlayeWidth, halfScreenWidth - halfPlayeWidth), rectTransform.anchoredPosition.y);
    }

    public void SetEnableInput(bool enable)
    {
        playerInput.enabled = enable;
    }

    public override void Shot()
    {
        if (cooldown <= 0)
        {
            base.Shot();
            cooldown = CurrentUnitData.ReloadTime;
        }
    }

    protected override void Kill()
    {
        base.Kill();
        GameManager.Instance.ParticleManager.SpawnParticle(EParticleEffect.Kaboom, transform.position);
        gameObject.SetActive(false);
        PlayerDead();
    }
}
