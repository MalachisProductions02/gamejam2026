using UnityEngine;
using System.Collections.Generic;

public class CustomButton : MonoBehaviour
{
    [SerializeField] private GameObject hoverObject;
    [SerializeField] private GameObject hoverPanel;
    [SerializeField] private List<Animator> anims;

    private void Start()
    {
        hoverObject.SetActive(false);
        hoverPanel.SetActive(false);
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
        hoverObject.SetActive(false);
        
        foreach (Animator anim in anims)
        {
            anim.SetBool("Bye", true);
            hoverPanel.SetActive(true);
        }

        Debug.Log("Mouse clicked");
    }
}
