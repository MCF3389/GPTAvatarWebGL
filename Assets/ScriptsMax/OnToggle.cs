using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class OnToggle : MonoBehaviour
{
    [SerializeField] private Button toggleButton;  // The button
    [SerializeField] private TMP_Text textToToggle;  // The TextMeshPro component to enable/disable
    [SerializeField] private string buttonText;  // The text to display on the button

    private bool isOn = true;  // Track the toggle state

    // Method to be called when the button is clicked
    public void Toggle()
    {
        // Toggle the state
        isOn = !isOn;

        // Update the button text based on the new state
        TMP_Text buttonLabel = toggleButton.GetComponentInChildren<TMP_Text>();
        if (buttonLabel != null)
        {
            buttonLabel.text = isOn ? buttonText + " On" : buttonText + " Off";
        }

        // Enable/Disable the TextMeshPro component
        textToToggle.enabled = isOn;
    }
}
