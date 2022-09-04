using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public static bool goToQuiz = false;

    public void LoadQuiz ()
    {
        goToQuiz = true;
    }
}
