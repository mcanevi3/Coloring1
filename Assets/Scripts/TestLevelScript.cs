using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TestLevelScript : MonoBehaviour
{
    [SerializeField]
    private Button exitButton;

    void OnEnable()
    {
        if (exitButton == null)
        {
            Debug.LogError("Exit button is not assigned in the inspector.");
            return;
        }
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    void OnDisable()
    {
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }

    private void OnExitButtonClicked()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
