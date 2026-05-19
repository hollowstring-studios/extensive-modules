using Godot;

[GlobalClass]
public partial class MiniGame : Control
{
	[Signal]
	public delegate void StartedEventHandler();
	[Signal]
	public delegate void FinishedEventHandler();
	
	public virtual void Setup(Variant[] args) {}
	public virtual void Reset() {}
}
