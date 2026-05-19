using Godot;
using System;
using System.IO;

[GlobalClass]
public partial class FollowObject : Node3D
{
	[Export]
	public PathFollow3D path_follow {get; set;}
	[Export]
	public float follow_speed {get; set;} = 1.0f;

	[Signal]
	public delegate void FollowedEventHandler();

	public override void _Ready()
	{
		GlobalPosition = path_follow.GlobalPosition;
		Reparent(path_follow);
		path_follow.Progress = 0.0f;

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(path_follow, "progress_ratio", 1.0f, 1.0f / follow_speed);
		tween.TweenCallback(Callable.From(() => { QueueFree(); }));
	}
}
