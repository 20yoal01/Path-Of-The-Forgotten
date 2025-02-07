using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI buttonText;
    public Image arrowImageL;
    public Image arrowImageR;
    public TMP_ColorGradient highlightColor;
    public TMP_ColorGradient originalColor;
    public TMP_ColorGradient disableColor;

    private bool isMouseHovering = false;
    private bool isKeyboardSelected = false;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (arrowImageL != null && arrowImageR != null)
        {
            SetArrowVisibility(0); // Hide arrows initially
        }
    }

    void Update()
    {
        // Check if the button is selected via keyboard
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            if (!isKeyboardSelected && button.interactable)
            {
                isKeyboardSelected = true;
                if (!isMouseHovering) // Prevent double hover effect
                {
                    ApplyHoverEffect();
                }
            }
        }
        else if (isKeyboardSelected)
        {
            isKeyboardSelected = false;
            if (!isMouseHovering) // Only remove if mouse is not hovering
            {
                RemoveHoverEffect();
            }
        }

        if (button != null && !button.interactable)
        {
            HandleButtonDisabled();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseHovering = true;
        if (button != null && button.interactable)
        {
            ApplyHoverEffect();
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseHovering = false;
        if (!isKeyboardSelected) // Only remove if keyboard is not selecting
        {
            RemoveHoverEffect();
        }
    }

    private void ApplyHoverEffect()
    {
        if (buttonText != null)
            buttonText.colorGradientPreset = highlightColor;

        if (arrowImageL != null && arrowImageR != null)
        {
            SetArrowVisibility(1); // Show arrows
        }
    }

    private void RemoveHoverEffect()
    {
        if (buttonText != null)
            buttonText.colorGradientPreset = originalColor;

        if (arrowImageL != null && arrowImageR != null)
        {
            SetArrowVisibility(0); // Hide arrows
        }
    }

    private void HandleButtonDisabled()
    {
        if (buttonText != null)
            buttonText.colorGradientPreset = disableColor;

        if (arrowImageL != null && arrowImageR != null)
        {
            SetArrowVisibility(0); // Hide arrows
        }
    }

    private void SetArrowVisibility(float alpha)
    {
        arrowImageL.color = new Color(arrowImageL.color.r, arrowImageL.color.g, arrowImageL.color.b, alpha);
        arrowImageR.color = new Color(arrowImageR.color.r, arrowImageR.color.g, arrowImageR.color.b, alpha);
    }
}
