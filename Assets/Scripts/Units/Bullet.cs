using UnityEngine;

public class Bullet : MonoBehaviour, IDamagable
{
    public EBulletType BulletType => bulletType;

    [SerializeField]
    private EBulletType bulletType = default;
    [SerializeField]
    private RectTransform rectTransform = null;
    [SerializeField]
    private float speed = 1f;

    private int direction;
    private Unit shooter;
    private ObjectPool<Bullet> bulletPool;
    private float maxHeight;

    private void Update()
    {
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + direction * speed * Time.deltaTime);
        if (Mathf.Abs(rectTransform.anchoredPosition.y) > Screen.height / 2f)
        {
            Return();
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        collider.GetComponent<IDamagable>().MakeDamage(shooter.CurrentUnitData.Damage);
        Return();
    }

    public void MakeDamage(int damage)
    {
        Return();
    }

    public void Setup(ObjectPool<Bullet> pool)
    {
        bulletPool = pool;
        maxHeight = Screen.height / 2f - 50f;
    }

    public void Fire(Vector3 startPos, Unit shooter)
    {
        gameObject.SetActive(true);
        this.shooter = shooter;
        bool rotationUp = shooter is Player;
        rectTransform.position = startPos;
        direction = rotationUp ? 1 : -1;
        rectTransform.rotation = rotationUp ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
    }

    private void Return()
    {
        if (gameObject.activeInHierarchy)
        {
            GameManager.Instance.ParticleManager.SpawnParticle(EParticleEffect.BulletDestroy, transform.position);
            gameObject.SetActive(false);
            bulletPool.Return(this);
        }
    }
}