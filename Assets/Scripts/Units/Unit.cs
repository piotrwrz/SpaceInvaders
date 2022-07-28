using System;
using UnityEngine;

public class Unit : MonoBehaviour, IDamagable
{
    public event Action<int> Hit = delegate { };
    public UnitData CurrentUnitData
    {
        get;
        protected set;
    }

    public Vector2 Size => rectTransform.sizeDelta;

    [SerializeField]
    protected RectTransform rectTransform = null;

    private ObjectPool<Bullet> bulletPool = null;

    public virtual void Init(ObjectPool<Bullet> pool)
    {
        bulletPool = pool;
    }

    public virtual void Shot()
    {
        var bullet = bulletPool.Get();
        bullet.Fire(rectTransform.position, this);
    }

    public void MakeDamage(int damage)
    {
        CurrentUnitData.Hp = Mathf.Max(CurrentUnitData.Hp - damage, 0);
        Hit(CurrentUnitData.Hp);
        if (CurrentUnitData.Hp <= 0)
        {
            Kill();
        }
    }

    protected virtual void Kill()
    {

    }
}
