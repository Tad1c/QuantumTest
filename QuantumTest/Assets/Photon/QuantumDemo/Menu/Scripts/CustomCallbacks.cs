using Quantum;
using UnityEngine;

public class CustomCallbacks : QuantumCallbacks
{
	[SerializeField] private RuntimePlayer runtimePlayer;

	public override void OnGameStart(QuantumGame game)
	{
		if (game.Session.IsPaused)
		{
			return;
		}
		
		QuantumLoadBalancingClient client = QuantumConnection.Instance.Client;
		foreach (var localPlayer in game.GetLocalPlayers())
		{
			if (client is { IsConnected: true })
			{
				runtimePlayer.PlayerName = client.LocalPlayer.NickName;
			}

			Debug.Log($"CustomCallbacks - sending player: {localPlayer}");
			game.SendPlayerData(localPlayer, runtimePlayer);
		}
	}

	public override void OnGameResync(QuantumGame game)
	{
		Debug.Log("Detected Resync. Verified tick: " + game.Frames.Verified.Number);
	}
}