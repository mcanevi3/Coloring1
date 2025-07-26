using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField]
    private Button playButton;
    [SerializeField]
    private Button exitButton;

    void OnEnable()
    {
        if (playButton == null)
        {
            Debug.LogError("Play button is not assigned in the inspector.");
            return;
        }
        playButton.onClick.AddListener(OnPlayButtonClicked);

        if (exitButton == null)
        {
            Debug.LogError("Exit button is not assigned in the inspector.");
            return;
        }
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    void OnDisable()
    {
        if (playButton != null)
        {
            playButton.onClick.RemoveListener(OnPlayButtonClicked);
        }
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene("TestLevelScene");
        Debug.Log("Play button clicked! Loading TestLevelScene...");
    }
    private void OnExitButtonClicked()
    {
        Debug.Log("Exit button clicked!");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    void Start()
    {
    }

    void Update()
    {
    }
        
}
