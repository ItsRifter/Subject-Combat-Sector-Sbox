﻿using System;
using Sandbox;
using Sandbox.UI;

public partial class SCSHud : Sandbox.HudEntity<RootPanel>
{
	public SCSHud()
	{
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
	}
}
