using Godot;
using System;

[GlobalClass]
public partial class TweenProperty : Event
{
	[Export]
	public NodePath node;
	[Export]
	public string property;
	[Export]
	public Variant value;
	[Export]
	public float duration;
}
