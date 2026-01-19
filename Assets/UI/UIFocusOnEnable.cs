using UnityEngine;
using UnityEngine.EventSystems;

public class UIFocusOnEnable : MonoBehaviour
{
    [SerializeField] private GameObject firstSelectedButton;

    private void OnEnable()
    {
        StartCoroutine(SetFocusNextFrame());
    }

    private System.Collections.IEnumerator SetFocusNextFrame()
    {
        yield return null;

        if (EventSystem.current == null) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}
