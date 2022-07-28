using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [SerializeField]
    private List<SpawnableParticle> particlePrefabs = null;
    [SerializeField]
    private RectTransform particlesParent = null;

    private Dictionary<EParticleEffect, ObjectPool<SpawnableParticle>> particlePoolsDictionary = new Dictionary<EParticleEffect, ObjectPool<SpawnableParticle>>();

    private void Awake()
    {
        foreach (var particlePrefab in particlePrefabs)
        {
            particlePoolsDictionary[particlePrefab.ParticleType] = new ObjectPool<SpawnableParticle>(() =>
            {
                var particle = Instantiate(particlePrefab, particlesParent);
                particle.Setup(particlePoolsDictionary[particlePrefab.ParticleType]);
                return particle;
            });
        }
    }

    public void SpawnParticle(EParticleEffect particleType, Vector3 position)
    {
        particlePoolsDictionary[particleType].Get().Set(position);
    }
}
