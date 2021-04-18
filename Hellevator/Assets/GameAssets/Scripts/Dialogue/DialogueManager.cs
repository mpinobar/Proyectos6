﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class DialogueManager : TemporalSingleton<DialogueManager>
{
    //[SerializeField] private TextMeshProUGUI npcNameTxt;
    private TextMeshProUGUI m_dialogueTxt;
    private GameObject m_playerCanvas;
    //[SerializeField] private GameObject m_boxImage;
	//[SerializeField] private Image m_playerIcon;


    private Queue<string> sentences;

    private Dialogue currentDialogue;
	private GameObject currentImage;


	public override void Awake()
    {
		base.Awake();
        sentences = new Queue<string>();
    }
	private void Start()
	{
		InputManager.Instance.OnInteract += NextSentence;
	}

	public void StartTalking(Dialogue dialogue, GameObject canvasText)
    {
		m_playerCanvas = canvasText;
		m_dialogueTxt = canvasText.GetComponentInChildren<TextMeshProUGUI>();


		InputManager.Instance.IsInDialogue = true;
		PossessionManager.Instance.ControlledDemon.StopMovement();

		currentDialogue = dialogue;

		m_playerCanvas.SetActive(true);

		//m_dialogueTxt.gameObject.SetActive(true);
		//if(dialogue.CurrentIcon != null)
		//{
		//	m_playerIcon.sprite = dialogue.CurrentIcon;
		//}
		//m_boxImage.SetActive(true);

		sentences.Clear();

        foreach (string sentence in currentDialogue.Sentences)
        {
            sentences.Enqueue(sentence);
        }
		NextSentence();
    }

	bool coroutineActive = false;
	string sentence; 

	public void NextSentence()
    {
        if (coroutineActive)
        {
			StopAllCoroutines();
			m_dialogueTxt.text = sentence;
			coroutineActive = false;
        }
        else
        {
			if (sentences.Count == 0)
			{
				EndDialogue();
				return;
			}

			sentence = sentences.Dequeue();
			StopAllCoroutines();
			coroutineActive = true;
			StartCoroutine(WriteSentence(sentence));
		}        
    }
    IEnumerator WriteSentence(string sentence)
    {
        m_dialogueTxt.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            m_dialogueTxt.text = m_dialogueTxt.text + letter;
            yield return null;
        }
    }
    public void EndDialogue()
    {
		if (currentDialogue != null)
		{
			if(currentDialogue.Evento != null)
			{
				for (int i = 0; i < currentDialogue.Evento.Length; i++)
				{
					currentDialogue.Evento[i].ActivateEvent();
				}
			}
			InputManager.Instance.IsInDialogue = false;
		}		
    }
	public void DeactivateTextAndImage()
	{
		m_dialogueTxt.gameObject.SetActive(false);
 		m_playerCanvas.SetActive(false);
		//m_playerIcon.sprite = null;
		//m_boxImage.SetActive(false);
		currentDialogue.Triggerer.DestroyTrigger();
	}
}
