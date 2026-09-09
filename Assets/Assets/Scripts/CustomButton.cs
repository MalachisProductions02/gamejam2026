using UnityEngine;

public class CustomButton : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject;

    private void Start()
    {
        hoverObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        hoverObject.SetActive(true);
        Debug.Log("Mouse entered");
    }

    private void OnMouseExit()
    {
        hoverObject.SetActive(false);
        Debug.Log("Mouse exited");
    }

    private void OnMouseDown()
    {
        Debug.Log("Mouse clicked");
    }
}
