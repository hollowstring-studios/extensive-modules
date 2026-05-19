using Godot;
using System;

[GlobalClass]
public partial class EnemyController3D : CharacterBody3D
{
	public enum NavigationType
	{
		TARGET,
		WANDER,
		SEARCH,
	};

	[Export]
	NavigationAgent3D Agent {get; set;}
	
	[Export]
	public NavigationType Type {get; set;}

	[Export]
	public float Speed {get; set;} = 5.0f;

	[Export]
	public Vector3 MaxRange {get; set;}

	[Export]
	public Vector3 MaxOffset {get; set;}

	[Export]
	public Area3D DetectionArea {get; set;}

	private bool IsSearching {get; set;} = false;
	private RandomNumberGenerator Rng = new RandomNumberGenerator();

	public override void _Ready()
	{
		DetectionArea.BodyEntered += (Godot.Node3D body) =>
		{
			if (body is PlayerController3D)
			{
				IsSearching = false;
			}
		};

		if (Type == NavigationType.WANDER)
		{
			IsSearching = true;
			WanderUntilTargetFound();
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsSearching)
		{
			return;
		}
		
		Vector3 TargetPosition = Agent.GetNextPathPosition();
		Vector3 Direction = (TargetPosition - GlobalPosition).Normalized();
		LookAt(new Vector3(TargetPosition.X, GlobalPosition.Y, TargetPosition.Z), Vector3.Up);
		Velocity = Direction * Speed;
		MoveAndSlide();
	}

	public void SetTargetPosition(Vector3 TargetPosition, Action<Action> Callback)
	{
		IsSearching = true;

		if (Type == NavigationType.SEARCH)
		{
			SearchForTarget(TargetPosition, Callback);
		} else
		{
			GoToTarget(TargetPosition, Callback);
		}
	}

	private void WanderUntilTargetFound()
	{
		Agent.NavigationFinished += () =>
		{	
			if (IsSearching)
			{
				SetTargetPosition(GetWanderTarget(), (ContinueExecution) =>
				{
					ContinueExecution();
				});
			}
		};

		SetTargetPosition(GetWanderTarget(), (ContinueExecution) =>
		{
			ContinueExecution();
		});
	}

	private void SearchForTarget(Vector3 TargetPosition, Action<Action> Callback)
	{
		Agent.NavigationFinished += () =>
		{
			IsSearching = false;

			Callback(() => {
				Agent.SetTargetPosition(GetValueNearTargetPosition(TargetPosition));
				IsSearching = true;
			});
		};

		Agent.SetTargetPosition(GetValueNearTargetPosition(TargetPosition));
	}

	private void GoToTarget(Vector3 TargetPosition, Action<Action> Callback)
	{
		Agent.NavigationFinished += () =>
		{
			IsSearching = false;
		};

		Agent.SetTargetPosition(TargetPosition);
	}

	private Vector3 GetWanderTarget()
	{
		return new Vector3(
			Rng.RandfRange(-MaxRange.X, MaxRange.X),
			Rng.RandfRange(-MaxRange.Y, MaxRange.Y),
			Rng.RandfRange(-MaxRange.Z, MaxRange.Z)
		);
	}

	private Vector3 GetValueNearTargetPosition(Vector3 TargetPosition)
	{
		return new Vector3(
			TargetPosition.X + Rng.RandfRange(-MaxOffset.X, MaxOffset.X),
			TargetPosition.Y + Rng.RandfRange(-MaxOffset.Y, MaxOffset.Y),
			TargetPosition.Z + Rng.RandfRange(-MaxOffset.Z, MaxOffset.Z)
		);
	}
}
