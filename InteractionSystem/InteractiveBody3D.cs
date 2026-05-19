using Godot;
using System;

[GlobalClass]
public partial class InteractiveBody3D : RigidBody3D
{
	public virtual void Interact(Variant[] args) {}
}
