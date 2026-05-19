using Godot;

[GlobalClass]
public partial class PlayerController3D : CharacterBody3D
{
	[Export]
	public bool disabled {get; set;} = false;

	[Export]
	public Camera3D camera {get; set;} = null;
	[Export]
	public float camera_sesnv = 0.05f;
	[Export]
	public float camera_deadline_deg = 89.0f;

	[Export]
	public float speed {get; set;} = 10.0f;
	[Export]
	public float jump_velocity {get; set;} = 100.0f;
	[Export]
	public string move_forward_action {get; set;} = "ui_up";
	[Export]
	public string move_backward_action {get; set;} = "ui_down";
	[Export]
	public string move_left_action {get; set;} = "ui_left";
	[Export]
	public string move_right_action {get; set;} = "ui_right";
	[Export]
	public string jump_action {get; set;} = "ui_accept";

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			if (Input.MouseMode == Input.MouseModeEnum.Visible)
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
		}

		if (disabled)
		{
			return;
		}

		if (@event is InputEventMouseMotion e && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			RotateY(-e.Relative.X * camera_sesnv);
			camera.RotateX(-e.Relative.Y * camera_sesnv);
			camera.Rotation = new Vector3(Mathf.Clamp(camera.Rotation.X, Mathf.DegToRad(-camera_deadline_deg), Mathf.DegToRad(camera_deadline_deg)), 0.0f, 0.0f);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (disabled)
		{
			return;
		}
		
		Vector3 movement = new Vector3(0.0f, 0.0f, 0.0f);
		
		if (!IsOnFloor())
		{
			movement += GetGravity();
		}
		else
		{
			if (Input.IsActionJustPressed(jump_action))
			{
				movement.Y = jump_velocity;
			}
		}

		Vector2 input_dir  = Input.GetVector(move_left_action, move_right_action, move_forward_action, move_backward_action);
		Vector3 movement_dir = (Transform.Basis * new Vector3(input_dir.X, 0.0f, input_dir.Y)).Normalized();
	   
		
		movement.X = movement_dir.X * speed;
		movement.Z = movement_dir.Z * speed;
		
		Velocity = movement;

		MoveAndSlide();
	}

	public void SetDisabled(bool value)
	{
		disabled = value;
		
		if (disabled)
		{	
			Hide();
			GlobalPosition = new Vector3(0.0f, 999999.0f, 0.0f);
		} else
		{
			Show();
		}
	}
}
