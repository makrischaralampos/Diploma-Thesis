using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour
{
    public TextAsset Q1 = null;
    public TextAsset Q2 = null;
    public TextAsset Q3 = null;
    public TextAsset Q4 = null;

    public static bool returnToHome = false;
    public static bool restartQuiz = false;

    private Data data = new Data();

    [SerializeField] GameEvents events = null;

    [SerializeField] Animator timerAnimator = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    [SerializeField] Color timerHalfWayOutColor = Color.yellow;
    [SerializeField] Color timerAlmostOutColor = Color.red;
    private Color timerDefaultColor = Color.white;

    private List<AnswerData> PickedAnswers = new List<AnswerData>();
    private List<int> FinishedQuestions = new List<int>();
    private int currentQuestion = 0;

    private int timerStateParaHash = 0;

    private IEnumerator IE_WaitTillNextRound = null;
    private IEnumerator IE_StartTimer = null;

    private int quizQuestionsCount = 10;

    private bool IsFinished
    {
        get
        {
            return (FinishedQuestions.Count < quizQuestionsCount) ? false : true;
        }
    }

    void OnEnable ()
    {
        events.UpdateQuestionAnswer += UpdateAnswers;
    }
    void OnDisable ()
    {
        events.UpdateQuestionAnswer -= UpdateAnswers;
    }

    void Awake ()
    {
        events.CurrentFinalScore = 0;
    }

    void Start ()
    {
        events.StartupHighscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);

        timerDefaultColor = timerText.color;
        LoadData();

        timerStateParaHash = Animator.StringToHash("TimerState");

        var seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        UnityEngine.Random.InitState(seed);

        Display();
    }

    public void UpdateAnswers (AnswerData newAnswer)
    {
        if (data.Questions[currentQuestion].Type == AnswerType.Single)
        {
            foreach (var answer in PickedAnswers)
            {
                if (answer != newAnswer)
                {
                    answer.Reset();
                }
            }
            PickedAnswers.Clear();
            PickedAnswers.Add(newAnswer);
        }
        else
        {
            bool alreadyPicked = PickedAnswers.Exists(x => x == newAnswer);
            if (alreadyPicked)
            {
                PickedAnswers.Remove(newAnswer);
            }
            else
            {
                PickedAnswers.Add(newAnswer);
            }
        }
    }

    public void EraseAnswers ()
    {
        PickedAnswers = new List<AnswerData>();
    }

    void Display()
    {
        EraseAnswers();
        var question = GetRandomQuestion();

        if (events.UpdateQuestionUI != null)
        {
            events.UpdateQuestionUI(question);
        }
        else { Debug.LogWarning("Ups! Something went wrong while trying to display new Question UI Data. GameEvents.UpdateQuestionUI is null. Issue occured in GameManager.Display() method."); }

        if (question.UseTimer)
        {
            UpdateTimer(question.UseTimer);
        }
    }

    public void Accept ()
    {
        UpdateTimer(false);
        bool isCorrect = CheckAnswers();
        FinishedQuestions.Add(currentQuestion);

        UpdateScore(isCorrect ? data.Questions[currentQuestion].AddScore : -data.Questions[currentQuestion].AddScore);

        if (IsFinished)
        {
            SetHighscore();
        }

        var type = (IsFinished) ? UIManager.ResolutionScreenType.Finish : (isCorrect) ? UIManager.ResolutionScreenType.Correct : UIManager.ResolutionScreenType.Incorrect;

        events.DisplayResolutionScreen?.Invoke(type, data.Questions[currentQuestion].AddScore);

        AudioManager.Instance.PlaySound((isCorrect) ? "CorrectSFX" : "IncorrectSFX");

        if (type != UIManager.ResolutionScreenType.Finish)
        {
            if (IE_WaitTillNextRound != null)
            {
                StopCoroutine(IE_WaitTillNextRound);
            }
            IE_WaitTillNextRound = WaitTillNextRound();
            StartCoroutine(IE_WaitTillNextRound);
        }
    }

    void UpdateTimer (bool state)
    {
        switch (state)
        {
            case true:
                IE_StartTimer = StartTimer();
                StartCoroutine(IE_StartTimer);

                timerAnimator.SetInteger(timerStateParaHash, 2);
                break;
            case false:
                if (IE_StartTimer != null)
                {
                    StopCoroutine(IE_StartTimer);
                }

                timerAnimator.SetInteger(timerStateParaHash, 1);
                break;
        }
    }

    IEnumerator StartTimer ()
    {
        var totalTime = data.Questions[currentQuestion].Timer;
        var timeLeft = totalTime;

        timerText.color = timerDefaultColor;
        while (timeLeft > 0)
        {
            timeLeft--;

            AudioManager.Instance.PlaySound("CountdownSFX");

            if (timeLeft < totalTime / 2 && timeLeft > totalTime / 4)
            {
                timerText.color = timerHalfWayOutColor;
            }
            if (timeLeft < totalTime / 4)
            {
                timerText.color = timerAlmostOutColor;
            }

            timerText.text = timeLeft.ToString();
            yield return new WaitForSeconds(1.0f);
        }
        Accept();
    }
    IEnumerator WaitTillNextRound ()
    {
        yield return new WaitForSeconds(GameUtility.ResolutionDelayTime);
        Display();
    }

    Question GetRandomQuestion ()
    {
        var randomIndex = GetRandomQuestionIndex();
        currentQuestion = randomIndex;

        return data.Questions[currentQuestion];
    }
    int GetRandomQuestionIndex ()
    {
        var random = 0;
        if (FinishedQuestions.Count < data.Questions.Length)
        {
            do
            {
                random = UnityEngine.Random.Range(0, data.Questions.Length);
            } while (FinishedQuestions.Contains(random) || random == currentQuestion);
        }
        return random;
    }

    bool CheckAnswers ()
    {
        if(!CompareAnswers())
        {
            return false;
        }
        return true;
    }
    bool CompareAnswers ()
    {
        if (PickedAnswers.Count > 0)
        {
            List<int> c = data.Questions[currentQuestion].GetCorrectAnswers();
            List<int> p = PickedAnswers.Select(x => x.AnswerIndex).ToList();

            var f = c.Except(p).ToList();
            var s = p.Except(c).ToList();

            return !f.Any() && !s.Any();
        }
        return false;
    }

    /// <summary>
    /// Function that is called to load data from the xml file.
    /// </summary>
    void LoadData()
    {
        var path = Path.Combine(GameUtility.FileDir, GameUtility.FileName + GameEvents.level + ".xml");
        data = Data.Fetch(path);
    }

    public void RestartGame ()
    {
        restartQuiz = true;
    }
    public void QuitGame ()
    {
        Application.Quit();
    }
    public void GoFirstLevel()
    {
        returnToHome = true;
    }

    private void SetHighscore ()
    {
        var highscore = PlayerPrefs.GetInt(GameUtility.SavePrefKey);
        if (highscore < events.CurrentFinalScore)
        {
            PlayerPrefs.SetInt(GameUtility.SavePrefKey, events.CurrentFinalScore);
        }
    }
    private void UpdateScore (int add)
    {
        events.CurrentFinalScore += add;
        events.ScoreUpdated?.Invoke();
    }
}
