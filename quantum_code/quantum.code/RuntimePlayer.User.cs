﻿using Photon.Deterministic;

namespace Quantum
{
	partial class RuntimePlayer
	{
		public AssetRefEntityPrototype CharacterPrototype;
		public string PlayerName;

		partial void SerializeUserData(BitStream stream)
		{
			stream.Serialize(ref CharacterPrototype.Id.Value);
			stream.Serialize(ref PlayerName);
		}
	}
}