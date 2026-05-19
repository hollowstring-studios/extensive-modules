using Godot;
using System;

[GlobalClass]
public partial class WaitFor : Event
{
	[Export]
	public NodePath node;
	[Export]
	public string signal;
}
