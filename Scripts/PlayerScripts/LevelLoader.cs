using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    // Update is called once per frame
    void Update()
    {
        if (MainMenu.goToMain)
        {
            LoadNextLevel();
        } else if (NextLevel.goToQuiz)
        {
            LoadNextLevel();
        } else if (PauseMenu.returnToMenu)
        {
            LoadNextLevel();
        } else if (GameManager.returnToHome)
        {
            LoadNextLevel();
        } else if (GameManager.restartQuiz)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        if (PauseMenu.returnToMenu)
        {
            StartCoroutine(LoadLevel(0));
        } else if (GameManager.returnToHome)
        {
            StartCoroutine(LoadLevel(1));
        } else if (GameManager.restartQuiz)
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
        }
        else
        {
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }
        
        MainMenu.goToMain = false;
        NextLevel.goToQuiz = false;
        PauseMenu.returnToMenu = false;
        GameManager.returnToHome = false;
        GameManager.restartQuiz = false;
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

}
