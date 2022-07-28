using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnableParticle : MonoBehaviour
{
    public EParticleEffect ParticleType => particleType;

    [SerializeField]
    private EParticleEffect particleType = default;
    [SerializeField]
    private float timeToHide = 3f;

    private float timer;
    private ObjectPool<SpawnableParticle> particlesPool;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= timeToHide)
        {
            gameObject.SetActive(false);
            particlesPool.Return(this);
        }
    }

    public void Setup(ObjectPool<SpawnableParticle> pool)
    {
        particlesPool = pool;
    }

    public void Set(Vector3 position)
    {
        gameObject.SetActive(true);
        timer = 0f;
        transform.position = position;
    }
}
