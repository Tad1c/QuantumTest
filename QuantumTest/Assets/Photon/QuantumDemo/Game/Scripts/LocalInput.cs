using Photon.Deterministic;
using Quantum;
using UnityEngine;
using Input = UnityEngine.Input;

public class LocalInput : MonoBehaviour
{
	private void OnEnable()
	{
		QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
	}

	public void PollInput(CallbackPollInput callback)
	{
		Quantum.Input input = new Quantum.Input();

		input.Jump = Input.GetButton("Jump");

		Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

		input.DirectionX = (short)(moveDirection.x * 10);
		input.DirectionY = (short)(moveDirection.y * 10);
		
		callback.SetInput(input, DeterministicInputFlags.Repeatable);
	}
}