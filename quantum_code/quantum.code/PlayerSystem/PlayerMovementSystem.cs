using Photon.Deterministic;

namespace Quantum.PlayerSystem
{
	public unsafe class PlayerMovementSystem : SystemMainThreadFilter<PlayerMovementSystem.Filter>
	{
		public struct Filter
		{
			public EntityRef Entity;
			public CharacterController3D* CharacterController3D;
			public PlayerLink* Link;
		}
		
		public override void Update(Frame frame, ref Filter filter)
		{
			Input* input = frame.GetPlayerInput(filter.Link->Player);

			FPVector2 inputVector = new FPVector2((FP)input->DirectionX / 10, (FP)input->DirectionY / 10);

			if (inputVector.SqrMagnitude > 1)
			{
				inputVector = inputVector.Normalized;
			}
			
			filter.CharacterController3D->Move(frame, filter.Entity, new FPVector3(inputVector.X, 0, inputVector.Y));

			if (input->Jump.WasPressed)
			{
				filter.CharacterController3D->Jump(frame);
			}
		}


	}
}