using Godot;
using Godot.Collections;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class EventExecuter: Node
{
	[Export]
	public EventList eventList {get; set;}

	[Export]
	public bool loop {get; set;} = false;

	[Signal]
	public delegate void EventFinishedEventHandler();
	
	public override void _Ready()
	{
		if (loop)
		{
			EventFinished += () =>
			{
				ExecuteEvents();
			};
		}
	
		ExecuteEvents();
	}

	public async Task ExecuteEvents()
	{
		for (int i = 0; i < eventList.items.Count; i++)
		{
			switch (eventList.items[i])
			{
				case FreeNode:
					FreeNode freeNodeEvent = eventList.items[i] as FreeNode;
					GetNode(freeNodeEvent.node).CallDeferred("queue_free");
					break;
				case WaitFor:
					WaitFor waitForEvent = eventList.items[i] as WaitFor;
					if (waitForEvent.node != null)
					{
						await ToSignal(GetNode(waitForEvent.node), waitForEvent.signal);
					}
					break;
				case CallFunction:
					CallFunction callFunctionEvent = eventList.items[i] as CallFunction;
					if (callFunctionEvent.node != null)
					{
						GetNode(callFunctionEvent.node).CallDeferred(callFunctionEvent.function);
					}
					break;
				case TweenProperty:
					TweenProperty tweenPropertyEvent = eventList.items[i] as TweenProperty;
					if (tweenPropertyEvent.node != null)
					{
						Tween tween = new Tween();
						tween.TweenProperty(
							GetNode(tweenPropertyEvent.node),
							tweenPropertyEvent.property,
							tweenPropertyEvent.value,
							tweenPropertyEvent.duration
						);
						await ToSignal(tween, "finished");
					}
					break;
				case Event:
					GD.Print("Skipping Empty Event");
					break;
			};
		}

		EmitSignal("EventFinished");
	}
}
