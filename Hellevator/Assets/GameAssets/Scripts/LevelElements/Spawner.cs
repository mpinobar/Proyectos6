﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] DemonBase m_demonToSpawn;
    [SerializeField] bool spawnWhenPlayerPossesses;
    private List<DemonBase> m_spawnedDemons;
    [SerializeField] float m_spawnTimer = 6f;
    [SerializeField] int m_maxSpawnedDemons;
    float maxRange;

    private float m_timer;

    public float MaxRange { get => maxRange; set => maxRange = value; }

    private void Start()
    {
        m_spawnedDemons = new List<DemonBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveDemons() < m_maxSpawnedDemons)
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0)
            {
                m_spawnedDemons.Add(Instantiate(m_demonToSpawn, transform.position, Quaternion.identity,transform));
                m_spawnedDemons[m_spawnedDemons.Count - 1].transform.parent = null;
                if (maxRange != 0)
                {
                    m_spawnedDemons[m_spawnedDemons.Count - 1].MaximumPossessionRange = maxRange;
                }

                m_timer = m_spawnTimer;
            }
        }
    }

    /// <summary>
    /// Calcula cuántos demonios activos hay pertenecientes al spawner
    /// </summary>
    /// <returns>El numero de demonios que hay activos que haya spawneado este spawner</returns>
    private int ActiveDemons()
    {
        int activeDemonsFromThisSpawner = 0;
        for (int i = 0; i < m_spawnedDemons.Count; i++)
        {
            if (m_spawnedDemons[i] != null)
            {
                if (spawnWhenPlayerPossesses)
                {
                    if (!m_spawnedDemons[i].IsControlledByPlayer)
                    {
                        activeDemonsFromThisSpawner++;
                    }
                    else
                    {
                        m_spawnedDemons.RemoveAt(i);
                        i--;
                    }
                }
                else
                    activeDemonsFromThisSpawner++;
            }
        }
        return activeDemonsFromThisSpawner;
    }
}
