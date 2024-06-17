using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{

	[SerializeField] private TMP_Text roomNameLabel;
	[SerializeField] private TMP_Text playerActiveLabel;

	private int activePlayers;
	private Room room;
	
	private void Start()
	{
		room = QuantumConnection.Instance.Client.CurrentRoom;
		roomNameLabel.SetText(room.Name);
	}

	public void Update()
	{
		if (activePlayers != room.PlayerCount)
		{
			activePlayers = room.PlayerCount;
			playerActiveLabel.SetText($"Active Players: {room.PlayerCount}");
		}
	}
}
