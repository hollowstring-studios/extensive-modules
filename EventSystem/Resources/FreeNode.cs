using Godot;
using System;

[GlobalClass]
public partial class FreeNode : Event
{
	[Export]
	public NodePath node {get; set;}
}
