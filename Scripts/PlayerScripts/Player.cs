using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameObject triggeringNpc;
    private bool triggering;

    public GameObject dialogueBox1;
    public GameObject dialogueBox2;
    public GameObject dialogueBox3;
    public GameObject dialogueBox4;
    public GameObject dialogueBoxFox;
    public GameObject testButton;
    public GameObject startConvoButton;

    private void Update()
    {
        if(triggering && GameEvents.level == 1)
        {
            dialogueBox1.SetActive(true);
            testButton.SetActive(true);
        } else if(triggering && GameEvents.level == 2)
        {
            dialogueBox2.SetActive(true);
            testButton.SetActive(true);
        }
        else if (triggering && GameEvents.level == 3)
        {
            dialogueBox3.SetActive(true);
            testButton.SetActive(true);
        }
        else if (triggering && GameEvents.level == 4)
        {
            dialogueBox4.SetActive(true);
            testButton.SetActive(true);
        }
        else if (triggering && GameEvents.level == 5)
        {
            dialogueBoxFox.SetActive(true);
            startConvoButton.SetActive(true);
        }
        else
        {
            dialogueBox1.SetActive(false);
            dialogueBox2.SetActive(false);
            dialogueBox3.SetActive(false);
            dialogueBox4.SetActive(false);
            dialogueBoxFox.SetActive(false);
            testButton.SetActive(false);
            startConvoButton.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "NPC")
        {
            triggering = true;
            triggeringNpc = other.gameObject;
            GameEvents.level = 1;
        }
        else if(other.tag == "NPC1") 
        {
            triggering = true;
            triggeringNpc = other.gameObject;
            GameEvents.level = 2;
        }
        else if (other.tag == "NPC2")
        {
            triggering = true;
            triggeringNpc = other.gameObject;
            GameEvents.level = 3;
        }
        else if (other.tag == "NPC3")
        {
            triggering = true;
            triggeringNpc = other.gameObject;
            GameEvents.level = 4;
        }
        else if (other.tag == "Fox")
        {
            triggering = true;
            triggeringNpc = other.gameObject;
            GameEvents.level = 5;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "NPC")
        {
            triggering = false;
            triggeringNpc = null;
        }
        else if(other.tag == "NPC1")
        {
            triggering = false;
            triggeringNpc = null;
        }
        else if (other.tag == "NPC2")
        {
            triggering = false;
            triggeringNpc = null;
        }
        else if (other.tag == "NPC3")
        {
            triggering = false;
            triggeringNpc = null;
        }
        else if (other.tag == "Fox")
        {
            triggering = false;
            triggeringNpc = null;
        }
    }
}
