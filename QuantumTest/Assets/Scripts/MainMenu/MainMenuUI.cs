using System.Collections.Generic;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
	[SerializeField] private Transform memberList;
	[SerializeField] private PlayerMemberView playerMemberView;
	[SerializeField] private PlayButton playButton;
		
	[Header("Panels")]
	[SerializeField] private GameObject mainMenuPanel;
	[SerializeField] private GameObject roomPanel;
	
	[Header("Texts")]
	[SerializeField] private TMP_Text roomLabel;
	
	private List<PlayerMemberView> playersViews = new List<PlayerMemberView>();

	private TMP_InputField nameInputField;

	private string playerName;

	private Room room;

	public string PlayerName => playerName;

	public void OnPlayerNameAdded(string input)
	{
		playerName = input;
		Debug.Log($"Player name added as {playerName}");
	}
	
	public void OnPlayButton()
	{
		QuantumConnection.Instance.OnPlayButtonClicked();
	}
	
	public void StartGame()
	{
		QuantumConnection.Instance.OnStartGame();
	}

	public void UpdateRoomDetails()
	{
		QuantumLoadBalancingClient client = QuantumConnection.Instance.Client;
		if (!client.InRoom)
		{
			return;
		}
		
		while(playersViews.Count < client.CurrentRoom.MaxPlayers)
		{
			PlayerMemberView playerName = Instantiate(playerMemberView, memberList.transform);
			playersViews.Add(playerName);
		}

		int i = 0;

		foreach (KeyValuePair<int, Player> player in client.CurrentRoom.Players)
		{
			playersViews[i].SetText(player.Value.NickName);
			playersViews[i].Show();
			i++;
		}

		for (; i < playersViews.Count; ++i)
		{
			playersViews[i].Hide();
		}

		PlayButtonState(client);
	}

	private void PlayButtonState(QuantumLoadBalancingClient client)
	{
		if (client.LocalPlayer.IsMasterClient)
		{
			if (room.PlayerCount > 1)
			{
				playButton.SetText("Start the Game");
				playButton.SetStateOfButton(true);
			}
			else
			{
				playButton.SetText("Waiting for others...");
				playButton.SetStateOfButton(false);
			}
		}
		else
		{
			playButton.SetText("Waiting for master to start the game");
			playButton.SetStateOfButton(false);
		}
	}

	public void OnJoinedRoom()
	{
		room ??= QuantumConnection.Instance.Client.CurrentRoom;
		
		UpdateRoomDetails();
		mainMenuPanel.SetActive(false);
		roomPanel.SetActive(true);
		
		roomLabel.SetText(room.Name);
	}
}