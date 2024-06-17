using Cinemachine;
using Quantum;
using UnityEngine;
public class PlayerConfigs : MonoBehaviour
{
	[SerializeField]
	private EntityView entityView;

	[SerializeField] private PlayerHUD playerHud;

	public void OnEntityInstantiated()
	{
		QuantumGame game = QuantumRunner.Default.Game;

		Frame frame = game.Frames.Verified;

		if (frame.TryGet(entityView.EntityRef, out PlayerLink playerLink))
		{
			if (game.PlayerIsLocal(playerLink.Player))
			{
				CinemachineVirtualCamera virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
				virtualCamera.m_Follow = transform;
			}
			
			string playerName = frame.GetPlayerData(playerLink.Player).PlayerName;
			playerHud.SetPlayerName(playerName);
		}
	}

}