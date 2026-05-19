using Godot;

[GlobalClass]
public partial class BasicVehicleController3D : VehicleBody3D
{
	[Export]
	public Camera3D camera;

	[Export]
	public bool disabled = false;

	[Export]
	public float max_steer {get; set;} = 1.0f;

	[Export]
	public float power {get; set;} = 500.0f;
	[Export]
	public float handling {get; set;} = 1.0f;

	[Export]
	public string throttle_action = "ui_up";
	[Export]
	public string brake_action = "ui_down";
	[Export]
	public string steer_left = "ui_left";
	[Export]
	public string steer_right = "ui_right";

	public override void _PhysicsProcess(double delta)
	{
		if (disabled)
		{
			return;
		}

		EngineForce = Input.GetAxis(brake_action, throttle_action) * power;
		Steering = Mathf.MoveToward(Steering, Input.GetAxis(steer_right, steer_left) * max_steer, handling * (float)delta);
	}

	public void SetDisabled(bool value)
	{
		disabled = value;
	}
}
