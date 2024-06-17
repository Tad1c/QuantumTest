using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{

	[SerializeField] private TMP_Text lable;

	public void SetPlayerName(string name)
	{
		lable.SetText(name);
	}
}