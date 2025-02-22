#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Sets the script to be executed later than all default scripts
// This is helpful for UI, since other things may need to be initialized before setting the UI
[DefaultExecutionOrder(1000)]

public class MenuUIHandler : MonoBehaviour
{
    public TMP_InputField nameInputField;
    public TMP_Text BestScoreText;

    private void Start()
    {
        MenuManager.Instance.LoadHighScore();
        BestScoreText.text = $"Best Score: {MenuManager.Instance.NameHighScore}: {MenuManager.Instance.HighScore}";
        nameInputField.onEndEdit.AddListener(SubmitInput);
    }

    private void SubmitInput(string inputText)
    {
        MenuManager.Instance.NameEntered = inputText;
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToHighScoresScene()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
			Application.Quit();
#endif
    }
}
