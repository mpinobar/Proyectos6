﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Satan : MonoBehaviour
{
    public enum Phase
    {
        First, Second, Third, Interphase
    }
    Phase m_phase = Phase.First;

    [Header("Attacks")]
    [SerializeField] float m_timeBetweenAttacks = 5f;
    float m_attackTimer;

    [SerializeField] HandAttackSatan m_verticalHandAttack;
    //[SerializeField] float m_verticalHandAttackDuration;
    [SerializeField] float m_verticalOffsetToSpawn;
    [SerializeField] HandAttackSatan m_horizontalHandAttack;
    //[SerializeField] float m_horizontalHandAttackDuration;

    [Header("Rayo")]
    [SerializeField] Rayo m_rayo;
    [SerializeField] float m_tiempoPreviewRayo = 1f;
    [SerializeField] float m_delayAparicionRayo = 1f;
    [SerializeField] Transform m_previewRayo;

    [Header("Phases")]
    [SerializeField] float m_firstPhaseYCoordinate;
    [SerializeField] Vector2 m_secondPhasePosition;
    [SerializeField] Vector2 m_thirdPhasePosition;
    [SerializeField] float m_disappearSpeed;

    [Space]
    [SerializeField] int m_maxLives = 3;
    int m_currentLives;
    Animator m_anim;
    Vector2 m_bossPosition;
    public static Action OnInterphase;
    public static Action OnDeath;

    bool started;
    private void Start()
    {
        m_attackTimer = m_timeBetweenAttacks;
        m_anim = GetComponentInChildren<Animator>();
        m_previewRayo.gameObject.SetActive(false);
        m_currentLives = m_maxLives;
        m_horizontalHandAttack.gameObject.SetActive(false);
        m_verticalHandAttack.gameObject.SetActive(false);
        mainCam = Camera.main;
    }


    private void Update()
    {
        if (started)
        {
            if (m_phase != Phase.Interphase && PossessionManager.Instance.ControlledDemon)
            {
                m_attackTimer -= Time.deltaTime;
                if (m_attackTimer < 0)
                {
                    Attack();
                }
            }
            else
            {
                m_attackTimer = m_timeBetweenAttacks;
            }
        }
        
    }
    public void SetPhase(Phase newPhase)
    {
        m_phase = newPhase;
        if (newPhase == Phase.Interphase)
            OnInterphase?.Invoke();
        else if (newPhase == Phase.Second || newPhase == Phase.Third)
        {
            transform.localScale = Vector3.one * 1.5f;
        }
    }
    
    private void Attack()
    {
        float f = Random.value;
        if((PossessionManager.Instance.ControlledDemon.transform.position.y > mainCam.transform.position.y) && f < 0.33f)
        {
            f += Random.value * 0.66f;
        }
        if (f < 0.33f)
        {            
            VerticalHandAttack(/*PossessionManager.Instance.ControlledDemon.transform)*/);
        }
            /*StartCoroutine(*/
        else if (f < 0.66f)
            /*StartCoroutine(*/
            HorizontalHandAttack(/*PossessionManager.Instance.ControlledDemon.transform)*/);
        else if (f <= 1)
            StartCoroutine(AtaqueRayo(PossessionManager.Instance.ControlledDemon.transform));
        m_attackTimer = m_timeBetweenAttacks;        
    }

    private void LateUpdate()
    {
        if (started)
        {
            if (PossessionManager.Instance.ControlledDemon)
            {
                if (m_phase == Phase.First)
                {
                    m_bossPosition = PossessionManager.Instance.ControlledDemon.transform.position;
                    m_bossPosition.y = m_firstPhaseYCoordinate;
                }
                else if (m_phase == Phase.Second)
                    m_bossPosition = Vector3.Lerp(m_bossPosition, m_secondPhasePosition, 3 * Time.deltaTime);
                else if (m_phase == Phase.Third)
                    m_bossPosition = Vector3.Lerp(m_bossPosition, m_thirdPhasePosition, 3 * Time.deltaTime);
                else
                    m_bossPosition.y -= m_disappearSpeed * Time.deltaTime;

                transform.position = m_bossPosition;
            }
        }        
    }

    public void ReceiveDamage()
    {
        m_anim.SetTrigger("Hurt");
        m_currentLives--;
        if (m_currentLives <= 0)
        {
            m_anim.SetTrigger("Death");
            OnDeath?.Invoke();            
        }
    }

    public void BeginFight()
    {
        started = true;
    }

    void VerticalHandAttack(/*Transform target*/)
    {
        Debug.LogError("Vertical hand attack");
        if (m_anim)
            m_anim.SetTrigger("Attack");
        m_verticalHandAttack.gameObject.SetActive(true);
        m_verticalHandAttack.transform.position = PossessionManager.Instance.ControlledDemon.transform.position + Vector3.up * m_verticalOffsetToSpawn;
        //yield return new WaitForSeconds(m_verticalHandAttackDuration);
        //m_verticalHandAttack.Enabled = false;
    }

    void HorizontalHandAttack(/*Transform target*/)
    {
        Debug.LogError("Horizontal hand attack");
        if (m_anim)
            m_anim.SetTrigger("Attack");
        m_horizontalHandAttack.gameObject.SetActive(true);
        //yield return new WaitForSeconds(m_horizontalHandAttackDuration);
        //m_horizontalHandAttack.Enabled = false;
    }

    Camera mainCam;
    IEnumerator AtaqueRayo(Transform target)
    {
        Debug.LogError("Lightning attack");
        if (m_anim)
            m_anim.SetTrigger("Attack");
        if (!mainCam)
            mainCam = Camera.main;
        /*new Vector3(PossessionManager.Instance.ControlledDemon.transform.position.x, PossessionManager.Instance.ControlledDemon.transform.position.y + 14.3f, 0);*/
        m_rayo.gameObject.SetActive(false);
        if (m_previewRayo)
        {
            m_previewRayo.gameObject.SetActive(true);
            float t = 0;
            Vector2 posicionPreview = target.position;
            posicionPreview.y = mainCam.ViewportToWorldPoint(new Vector3(0,1,0)).y;
            while (t < m_tiempoPreviewRayo)
            {
                t += Time.deltaTime;
                posicionPreview.x = target.position.x;
                m_previewRayo.position = posicionPreview;
                yield return null;
            }
            yield return new WaitForSeconds(m_delayAparicionRayo);
        }
        RaycastHit2D rayhit = Physics2D.Raycast(m_previewRayo.position + Vector3.up,Vector2.down,100,1<<0);

        m_rayo.transform.position = rayhit.point + Vector2.up * 19.52f;
        //m_rayo.transform.position = new Vector3(m_previewRayo.position.x, m_rayo.transform.position.y, 0);
        m_rayo.gameObject.SetActive(true);
        m_previewRayo.gameObject.SetActive(false);
    }

}
