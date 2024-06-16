using Quantum;
using UnityEngine;

public class CustomCallbacks : QuantumCallbacks
{
	[SerializeField] private RuntimePlayer runtimePlayer;

	public override void OnGameStart(Quantum.QuantumGame game)
	{
		// paused on Start means waiting for Snapshot
		if (game.Session.IsPaused)
		{
			return;
		}

		QuantumLoadBalancingClient client = QuantumConnection.Instance.Client;

		if (client is { IsConnected: true })
		{
			runtimePlayer.PlayerName = client.LocalPlayer.NickName;
		}

		foreach (var localPlayer in game.GetLocalPlayers())
		{
			Debug.Log($"CustomCallbacks - sending player: {localPlayer}");
			game.SendPlayerData(localPlayer, runtimePlayer);
		}
	}

	public override void OnGameResync(Quantum.QuantumGame game)
	{
		Debug.Log("Detected Resync. Verified tick: " + game.Frames.Verified.Number);
	}
}