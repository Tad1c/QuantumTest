using TMPro;
using UnityEngine;

public class PlayerMemberView : MonoBehaviour
{
	[SerializeField] private TMP_Text label;

	public void SetText(string input)
	{
		label.SetText(input);
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}