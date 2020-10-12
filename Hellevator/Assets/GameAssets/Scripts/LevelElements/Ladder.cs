﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<BasicZombie>().SetOnLadder(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.GetComponent<BasicZombie>().SetOnLadder(false);
    }
}
