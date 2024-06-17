using Photon.Deterministic;

namespace Quantum.PlayerSystem
{
	public unsafe class PlayerDataSystem : SystemMainThread, ISignalOnPlayerDataSet, ISignalOnPlayerDisconnected
	{
		public new int RuntimeIndex { get; }

		public void OnPlayerDisconnected(Frame frame, PlayerRef player)
		{
			foreach (EntityComponentPair<PlayerLink> playerLink in frame.GetComponentIterator<PlayerLink>())
			{
				if (playerLink.Component.Player != player)
				{
					continue;
				}

				frame.Destroy(playerLink.Entity);
			}
		}

		public void OnPlayerDataSet(Frame frame, PlayerRef player)
		{
			RuntimePlayer data = frame.GetPlayerData(player);

			EntityPrototype prototypeEntity = frame.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

			EntityRef createdEntity = frame.Create(prototypeEntity);

			if (frame.Unsafe.TryGetPointer(createdEntity, out PlayerLink* playerLink))
			{
				playerLink->Player = player;
			}

			if (frame.Unsafe.TryGetPointer(createdEntity, out Transform3D* transform))
			{
				transform->Position = GetSpawnPosition(player);
			}
		}

		private FPVector3 GetSpawnPosition(int playerNumber)
		{
			return new FPVector3(-4 + (playerNumber * 2) + 1, 0, 0);
		}

		public override void Update(Frame f)
		{
		}
	}
}