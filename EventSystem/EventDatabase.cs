using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class EventDatabase : Node
{
	[Export]
	Dictionary<String, EventList> entries {get; set;} = new Dictionary<string, EventList>();

	public void ExecuteEvent(String key)
	{
		EventList eventList = entries[key];

		EventExecuter executer = new EventExecuter();
		executer.eventList = eventList;

		AddChild(executer);
	}
}
