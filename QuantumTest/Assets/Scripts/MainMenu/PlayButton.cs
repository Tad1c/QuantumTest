using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
	[SerializeField] private TMP_Text label;
	[SerializeField] private Button button;

	public void SetText(string input)
	{
		label.SetText(input);
	}

	public void SetStateOfButton(bool isActive)
	{
		button.interactable = isActive;
	}
}