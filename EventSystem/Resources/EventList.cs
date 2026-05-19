using Godot;
using Godot.Collections;
using System;

[GlobalClass]
public partial class EventList : Resource
{
	[Export]
	public Array<Event> items {get; set;}= new Array<Event>();
}
