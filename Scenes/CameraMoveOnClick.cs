using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class CameraMoveOnClick : MonoBehaviour
{
    public Transform[] levelPositions;
    public float moveSpeed = 2.0f;
    public GameObject pressToContinueText;
    public GameObject buttonsPanel;
    public GameObject levelSelectPanel;

    public TextMeshProUGUI levelNameText;

    private bool isMoving = false;
    private int currentLevel = 0;
    private Vector3 targetPosition;
    public AudioSource MainBackgroundMusic;
    public AudioSource ButtonClick;
    public AudioSource LoadLevel;
    public AudioSource ErrorSound;
    public AudioSource TransitionSound;

    public Image fadeImage;
    public Image loadingGif;
    public float fadeDuration = 1.0f;

    private bool mainMenuActive = true;

    void Start()
    {
        if (MainBackgroundMusic == null) Debug.LogError("MainBackgroundMusic is not assigned.");
        if (pressToContinueText == null) Debug.LogError("pressToContinueText is not assigned.");
        if (buttonsPanel == null) Debug.LogError("buttonsPanel is not assigned.");
        if (levelSelectPanel == null) Debug.LogError("levelSelectPanel is not assigned.");
        if (fadeImage == null) Debug.LogError("fadeImage is not assigned.");
        if (loadingGif == null) Debug.LogError("loadingGif is not assigned.");
        if (levelNameText == null) Debug.LogError("levelNameText is not assigned.");

        MainBackgroundMusic.Play();
        pressToContinueText.SetActive(true);
        buttonsPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        fadeImage.gameObject.SetActive(false);
        loadingGif.gameObject.SetActive(false);

        if (levelPositions.Length > 0)
        {
            targetPosition = levelPositions[0].position;
            UpdateLevelName();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving && mainMenuActive)
        {
            if (levelPositions.Length == 0)
            {
                Debug.LogError("No level positions assigned!");
                return;
            }

            isMoving = true;
            pressToContinueText.SetActive(false);
            if (TransitionSound != null) TransitionSound.Play();
            Debug.Log("Initial move triggered, moving to position " + currentLevel);
        }

        if (isMoving)
        {
            MoveCameraToTarget(targetPosition);
        }
    }

    void MoveCameraToTarget(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) 
        {
            isMoving = false;
            UpdateLevelName();

            if (mainMenuActive)
            {
                buttonsPanel.SetActive(true);
                Debug.Log("Camera reached the first position, showing main menu buttons.");
            }
            else
            {
                levelSelectPanel.SetActive(true);
                Debug.Log("Camera reached the second position, showing level select panel.");
            }
        }
    }

    void UpdateLevelName()
    {
        switch (currentLevel)
        {
            case 0:
                levelNameText.text = "";
                break;
            case 1:
                levelNameText.text = "Shooting Range";
                break;
            case 2:
                levelNameText.text = "Ukraine";
                break;
            case 3:
                levelNameText.text = "Russia";
                break;
            default:
                levelNameText.text = "Unknown Location";
                break;
        }
    }

    public void PlayGame()
    {
        if (!isMoving && mainMenuActive)
        {
            ButtonClick.Play();
            mainMenuActive = false;
            buttonsPanel.SetActive(false);
            currentLevel = 1;
            targetPosition = levelPositions[currentLevel].position;
            isMoving = true;
            if (TransitionSound != null) TransitionSound.Play();
        }
    }

    public void MoveLeft()
    {
        if (currentLevel > 0 && !isMoving)
        {
            ButtonClick.Play();
            currentLevel--;
            isMoving = true;
            Debug.Log("Moving left to level: " + currentLevel);
            targetPosition = levelPositions[currentLevel].position;
            if (TransitionSound != null) TransitionSound.Play();
        }
        else
        {
            if (ErrorSound != null) ErrorSound.Play();
            Debug.Log("Already at the leftmost level.");
        }
    }

    public void MoveRight()
    {
        if (currentLevel < levelPositions.Length - 1 && !isMoving)
        {
            ButtonClick.Play();
            currentLevel++;
            isMoving = true;
            Debug.Log("Moving right to level: " + currentLevel);
            targetPosition = levelPositions[currentLevel].position;
            if (TransitionSound != null) TransitionSound.Play();
        }
        else
        {
            if (ErrorSound != null) ErrorSound.Play();
            Debug.Log("Already at the rightmost level.");
        }
    }

    public void PlayLevel()
    {
        ButtonClick.Play();
        StartCoroutine(FadeAndLoadLevel());
    }

    private IEnumerator FadeAndLoadLevel()
    {
        // Show the fade image and loading GIF
        levelSelectPanel.SetActive(false);
        fadeImage.gameObject.SetActive(true);
        loadingGif.gameObject.SetActive(true);

        // Fade to black
        float t = 0;
        Color fadeColor = fadeImage.color;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            fadeColor.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = fadeColor;
            yield return null;
        }

        yield return new WaitForSeconds(5);

        switch (currentLevel)
        {
            case 1:
                Debug.Log("Playing level 1");
                SceneManager.LoadScene("SampleScene");
                break;
            case 2:
                Debug.Log("Playing level 2");
                SceneManager.LoadScene("Level2Scene");
                break;
            case 3:
                Debug.Log("Playing level 3");
                SceneManager.LoadScene("Level3Scene");
                break;
            default:
                Debug.Log("Playing default level");
                SceneManager.LoadScene("SampleScene");
                break;
        }
    }
}
