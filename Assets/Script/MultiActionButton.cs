using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MultiActionButton : MonoBehaviour
{
    public Button triggerButton;

    [Header("Objects To Control")]
    public GameObject objectToHide;        // Will be SetActive(false)
    public GameObject objectToReorder;     // Will call SetAsLastSibling()
    public GameObject objectToShow;        // Will be SetActive(true)

    public float delay = 2f;               // Delay before actions happen

    void Start()
    {
        if (triggerButton != null)
            triggerButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        StartCoroutine(HandleActionsAfterDelay());
    }

    private IEnumerator HandleActionsAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // 1. Hide first GameObject
        if (objectToHide != null)
            objectToHide.SetActive(false);

        // 2. Move second GameObject to top of hierarchy
        if (objectToReorder != null)
            objectToReorder.transform.SetAsLastSibling();

        // 3. Activate third GameObject
        if (objectToShow != null)
            objectToShow.SetActive(true);
    }
}
