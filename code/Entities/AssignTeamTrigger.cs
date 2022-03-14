using System;
using Sandbox;

[Library( "scs_assignteam", Description = "Behaves like a teleporter but assigns the player to a team" )]
[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
[Hammer.Model]
[Hammer.SupportsSolid]
public partial class AssignTeamTrigger : TriggerTeleport
{
	public enum TeamType
	{
		Unspecified,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Property("Assignment"), Description("Assign the player to the team")]
	public TeamType TeamAssign { get; set; } = TeamType.Unspecified;

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is SCSPlayer player )
		{
			switch ( TeamAssign )
			{
				case TeamType.Unspecified:
					player.CurTeam = SCSPlayer.TeamEnum.Unspecified;
					break;
				case TeamType.Red:
					player.CurTeam = SCSPlayer.TeamEnum.Red;
					break;
				case TeamType.Blue:
					player.CurTeam = SCSPlayer.TeamEnum.Blue;
					break;
				case TeamType.Green:
					player.CurTeam = SCSPlayer.TeamEnum.Green;
					break;
				case TeamType.Yellow:
					player.CurTeam = SCSPlayer.TeamEnum.Yellow;
					break;
			}
		}
	}
}

