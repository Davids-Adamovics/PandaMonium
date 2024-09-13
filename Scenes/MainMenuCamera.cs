using UnityEngine;
using UnityEngine.UI;

public class CameraMoveOnClick : MonoBehaviour
{
    public Transform targetPosition;
    public float moveSpeed = 2.0f;
    public GameObject pressToContinueText;
    public GameObject buttonsPanel;

    private bool isMoving = false;

    void Start()
    {
        pressToContinueText.SetActive(true);
        buttonsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            isMoving = true;
            pressToContinueText.SetActive(false);
        }

        if (isMoving)
        {
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition.position, Time.deltaTime * moveSpeed);
        if (Vector3.Distance(transform.position, targetPosition.position) < 5f)
        {
            isMoving = false;
            buttonsPanel.SetActive(true);
        }
    }
}
