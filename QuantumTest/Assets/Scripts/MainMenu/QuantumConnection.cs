using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Quantum;
using Quantum.Demo;
using UnityEngine;

public class QuantumConnection : Singleton<QuantumConnection>, IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback, IInRoomCallbacks
{
	[SerializeField] private ConnectionHandler connectionHandler;
	[SerializeField] private MainMenuUI mainMenuUI;

	private QuantumLoadBalancingClient client;
	
	private long mapGuid;

	private enum PhotonEventCode : byte
	{
		StartGame = 110
	}
	
	public QuantumLoadBalancingClient Client => client;
	
	private void Update()
	{
		client?.Service();
	}

	public void OnPlayButtonClicked()
	{
		if (client is { IsConnected: true })
		{
			OnJoinRandomOrCreateRoom();
		}
		else
		{
			ConnectToGame();
		}
	}

	public void OnStartGame()
	{
		if (!client.LocalPlayer.IsMasterClient)
		{
			return;
		}

		RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All
		};

		if (!client.OpRaiseEvent((byte)PhotonEventCode.StartGame, null, raiseEventOptions, SendOptions.SendReliable))
		{
			Debug.LogError("Unable to start the game");
		}

		Debug.Log("Starting the game");
	}

	private void StarQuantumGame()
	{
		if (QuantumRunner.Default != null)
		{
			Debug.LogError("Another QuantumRunner is preventing starting the game");
			return;
		}

		RuntimeConfig runtimeConfig = new RuntimeConfig();
		runtimeConfig.Map.Id = mapGuid;

		QuantumRunner.StartParameters param = new QuantumRunner.StartParameters
		{
			RuntimeConfig = runtimeConfig,
			DeterministicConfig = DeterministicSessionConfigAsset.Instance.Config,
			GameMode = Photon.Deterministic.DeterministicGameMode.Multiplayer,
			FrameData = null,
			InitialFrame = 0,
			PlayerCount = client.CurrentRoom.MaxPlayers,
			LocalPlayerCount = 1,
			RecordingFlags = RecordingFlags.None,
			NetworkClient = client,
			StartGameTimeoutInSeconds = 10.0f
		};

		string clientId = ClientIdProvider.CreateClientId(ClientIdProvider.Type.PhotonUserId, client);

		Debug.Log(
			$"Starting QuantumRunner with client ID {clientId} and map guid {mapGuid}. Local player count {param.LocalPlayerCount}");

		QuantumRunner.StartGame(clientId, param);

		gameObject.SetActive(false);
	}
	
	private void ConnectToGame()
	{
		AppSettings appSettings = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);

		client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);

		client.AddCallbackTarget(this);

		if (string.IsNullOrEmpty(appSettings.AppIdRealtime.Trim()))
		{
			Debug.LogError("Something went wrong");
			return;
		}

		if (string.IsNullOrEmpty(mainMenuUI.PlayerName))
		{
			Debug.LogError("Please enter name");
			return;
		}

		if (!client.ConnectUsingSettings(appSettings, mainMenuUI.PlayerName))
		{
			Debug.LogError("Unable to connect to the game");
			return;
		}

		Debug.Log("Attempt to connect to the game");
	}
	
	private EnterRoomParams CreateEnterRoomParams(string roomName)
	{
		EnterRoomParams enterRoomParams = new EnterRoomParams
		{
			RoomOptions = new RoomOptions()
			{
				IsVisible = true,
				MaxPlayers = 6,
				Plugins = new[] { "QuantumPlugin" },
				CustomRoomProperties = new Hashtable
				{
					{ "MAP-GUID", mapGuid }
				},

				PlayerTtl = PhotonServerSettings.Instance.PlayerTtlInSeconds * 1000,
				EmptyRoomTtl = PhotonServerSettings.Instance.EmptyRoomTtlInSeconds * 1000,
			},
			RoomName = roomName
		};

		return enterRoomParams;
	}
	

	private void OnJoinRandomOrCreateRoom()
	{
		connectionHandler.Client = client;
		connectionHandler.StartFallbackSendAckThread();

		MapAsset[] mapInResources = Resources.LoadAll<MapAsset>(QuantumEditorSettings.Instance.DatabasePathInResources);
		mapGuid = mapInResources[0].AssetObject.Guid.Value;

		Debug.Log($"Using Map with GUID {mapGuid}");

		EnterRoomParams enterRoomParams = CreateEnterRoomParams($"{mainMenuUI.PlayerName}'s Room");

		OpJoinRandomRoomParams joinRandomRoomParams = new OpJoinRandomRoomParams();

		if (!client.OpJoinRandomOrCreateRoom(joinRandomRoomParams, enterRoomParams))
		{
			Debug.LogError("Unable to join random room or to create a room");
			return;
		}

		Debug.Log("Joining into a random room or creating a new room");
	}
	
	public void OnConnected()
	{
		Debug.Log($"OnConnected UsedId: {client.UserId}");
	}
	
	public void OnEvent(EventData photonEvent)
	{
		switch (photonEvent.Code)
		{
			case (byte)PhotonEventCode.StartGame:
				client.CurrentRoom.CustomProperties.TryGetValue("MAP-GUID", out object mapGuidValue);

				if (mapGuidValue == null)
				{
					Debug.LogError("There is no map, disconnecting");
				}

				StarQuantumGame();

				break;
		}
	}

	public void OnConnectedToMaster()
	{
		OnJoinRandomOrCreateRoom();
	}

	public void OnDisconnected(DisconnectCause cause)
	{
	}

	public void OnRegionListReceived(RegionHandler regionHandler)
	{
	}

	public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
	{

	}

	public void OnCustomAuthenticationFailed(string debugMessage)
	{

	}

	public void OnFriendListUpdate(List<FriendInfo> friendList)
	{
	}

	public void OnCreatedRoom()
	{
	}

	public void OnCreateRoomFailed(short returnCode, string message)
	{
	}

	public void OnJoinedRoom()
	{
		mainMenuUI.OnJoinedRoom();
	}

	public void OnJoinRoomFailed(short returnCode, string message)
	{
	}

	public void OnJoinRandomFailed(short returnCode, string message)
	{
	}

	public void OnLeftRoom()
	{
		mainMenuUI.UpdateRoomDetails();
	}

	public void OnPlayerEnteredRoom(Player newPlayer)
	{
		mainMenuUI.UpdateRoomDetails();
	}

	public void OnPlayerLeftRoom(Player otherPlayer)
	{
		mainMenuUI.UpdateRoomDetails();
	}

	public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
		mainMenuUI.UpdateRoomDetails();
	}

	public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	public void OnMasterClientSwitched(Player newMasterClient)
	{
		mainMenuUI.UpdateRoomDetails();
	}
}
