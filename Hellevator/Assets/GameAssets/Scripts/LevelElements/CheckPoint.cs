﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckPoint : MonoBehaviour
{
    string m_sceneToLoad;
    [SerializeField] AudioClip m_lightUpClip;
    bool m_active;
    bool m_opening;
    [SerializeField] private DemonBase m_demonToSpawn;
    private float m_openingValue;
    SpriteRenderer m_spr;
    [SerializeField] bool m_savesGameToPlayerPrefs = false;
    [SerializeField] int priority;
    public string SceneToLoad { get => m_sceneToLoad; set => m_sceneToLoad = value; }
    public int Priority { get => priority; set => priority = value; }

    [SerializeField] CameraChangeOnTrigger m_cameraChange;

    private void Awake()
    {
        m_spr = GetComponent<SpriteRenderer>();
        m_openingValue = 1;
        m_sceneToLoad = gameObject.scene.name;

    }    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase demon = collision.GetComponentInParent<DemonBase>();
        if (demon && demon.IsControlledByPlayer)
        {
            ActivateCheckPoint();
            m_opening = true;
            if (!m_active)
            {
                if (m_lightUpClip)
                    AudioManager.Instance.PlayAudioSFX(m_lightUpClip, false);
                m_active = true;
            }
        }
    }


    //private void Update()
    //{
    //    if (m_opening)
    //    {
    //        m_openingValue -= Time.deltaTime;
    //        m_openingValue = Mathf.Clamp01(m_openingValue);
    //        //m_spr?.material.SetFloat("_Opening", m_openingValue);
    //        if (m_openingValue <= 0)
    //        {
    //            m_opening = false;
    //        }
    //    }
    //}

    private void ActivateCheckPoint()
    {
        LevelManager.Instance.SetLastCheckPoint(this);
        //m_spr?.material.SetFloat("_Active", 1);
        //m_spr?.material.SetFloat("_Opening", 0);
        LevelManager.Instance.SaveSceneToLoad(SceneToLoad);

    }

    /// <summary>
    /// Spawns player at checkpoint location, summoning and possessing the instantiated type of demon
    /// </summary>
    public void SpawnPlayer()
    {
        if (PossessionManager.Instance.ControlledDemon)
        {
            //Debug.LogError("a");
            Destroy(PossessionManager.Instance.ControlledDemon);            
        }
        if (m_cameraChange)
            m_cameraChange.ChangeCamera();
            //Debug.LogError("b");
        
        DemonBase spawnedDemon = Instantiate(m_demonToSpawn, transform.position - Vector3.up*2, Quaternion.identity);
        PossessionManager.Instance.ControlledDemon = spawnedDemon;
        spawnedDemon.enabled = true;
        spawnedDemon.PossessedOnStart = true;
        spawnedDemon.SetControlledByPlayer();
        spawnedDemon.AssignLastMask();
        ActivateCheckPoint();
        m_openingValue = 1;
        m_spr?.material.SetFloat("_Active", 1);
        m_spr?.material.SetFloat("_Opening", 1);
        //CameraManager.Instance.ChangeCamTarget();
        InputManager.Instance.UpdateDemonReference();
        if (TryGetComponent(out InstantBlend blend))
        {
            blend.InstaBlend();
        }
    }
}
