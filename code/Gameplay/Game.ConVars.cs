using System;
using Sandbox;

namespace SCS;

public partial class SCSGame
{
	[ConVar.Replicated("scs_startpoints")]
	public static int StartingPoints { get; set; } = 4;

	//[ConVar.Replicated("scs_totalteams")]
	//public static int TeamAmount { get; set; } = 2;

	[ConVar.Replicated( "scs_redteam" )]
	public static bool RedTeamActive { get; set; } = false;

	[ConVar.Replicated( "scs_blueteam" )]
	public static bool BlueTeamActive { get; set; } = false;

	[ConVar.Replicated( "scs_greenteam" )]
	public static bool GreenTeamActive { get; set; } = false;

	[ConVar.Replicated( "scs_yellowteam" )]
	public static bool YellowTeamActive { get; set; } = false;

	[ConVar.Replicated("scs_wincount")]
	public static int WinsNeeded { get; set; } = 2;
}

