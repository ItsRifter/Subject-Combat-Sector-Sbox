using System;
using Sandbox;

namespace SCS;

public partial class SCSGame
{
	[ConVar.Replicated]
	public static int StartingPoints { get; set; } = 4;
	[ConVar.Replicated]
	public static int TeamAmount { get; set; } = 2;
	[ConVar.Replicated]
	public static int WinsNeeded { get; set; } = 2;
}

