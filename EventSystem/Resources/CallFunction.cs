using Godot;
using System;

[GlobalClass]
public partial class CallFunction : Event
{
	[Export]
	public NodePath node;
	[Export]
	public string function;
}
