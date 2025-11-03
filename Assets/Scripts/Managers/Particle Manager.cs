using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private static ParticleManager _instance;

    public static ParticleManager Instance
    {
        get
        {
            if (_instance == null) Debug.Log("ParticleManager is null");

            return _instance;
        }
    }

    [System.Serializable]
    public class ParticleEffect
    {
        public string name;
        public GameObject gameObject;
    }

    public List<ParticleEffect> effects = new List<ParticleEffect>();

    private void Awake()
    {
        _instance = this;
    }

    public GameObject GetParticleEffect(string name)
    {
        foreach(var effect in effects)
        {
            if(effect.name == name)
            {
                return effect.gameObject;
            }
        }

        Debug.LogWarning("Cannot find particle!");
        return null;
    }

}
