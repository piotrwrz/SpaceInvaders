using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Enemy : Unit
{
    public event Action<Enemy> Died = delegate { };

    public EEnemyType EnemyType => enemyType;
    public EBulletType BulletType => bulletType;
    public string InputFilePath => inputFilePath;
    public int BaseHp => baseHp;

    [SerializeField]
    private EEnemyType enemyType = default;
    [SerializeField]
    private EBulletType bulletType = default;
    [SerializeField]
    private string inputFilePath = "";
    [SerializeField]
    private int baseHp = 1;

    private ObjectPool<Enemy> enemiesPool;
    private float shootingTimer;

    public void MyUpdate()
    {
        shootingTimer += Time.deltaTime;
        if (shootingTimer >= CurrentUnitData.ReloadTime)
        {
            shootingTimer = 0f;
            var rand = UnityEngine.Random.Range(0f, 100f);
            if (rand <= CurrentUnitData.ShotFrequency)
            {
                Shot();
            }
        }
    }

    public void Setup(UnitData data)
    {
        CurrentUnitData = data.Duplicate();
    }

    public void Setup(UnitData data, Vector2 position, ObjectPool<Enemy> pool)
    {
        Setup(data);
        shootingTimer = UnityEngine.Random.Range(-CurrentUnitData.ShotFrequency, 0f);
        gameObject.SetActive(true);
        enemiesPool = pool;
        rectTransform.anchoredPosition = position;
    }

    public void SetupSpecialEnemy(UnitData data, Vector2 position)
    {
        Setup(data);
        gameObject.SetActive(true);
        rectTransform.anchoredPosition = position;
    }

    public void Return()
    {
        gameObject.SetActive(false);
        enemiesPool?.Return(this);
    }

    protected override void Kill()
    {
        base.Kill();
        Return();
        GameManager.Instance.ParticleManager.SpawnParticle(EParticleEffect.Kaboom, transform.position);
        Died(this);
    }
}
