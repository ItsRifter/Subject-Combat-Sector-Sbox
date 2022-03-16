using System;
using Sandbox;

[Library( "scs_team_teleport", Description = "Behaves like a teleporter but only teleports specific team types" )]
[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
[Hammer.Model]
[Hammer.SupportsSolid]
public partial class TeamTeleporter : TriggerTeleport
{
	public enum TeleTeamEnum
	{
		Unspecified,
		Red,
		Blue,
		Green,
		Yellow
	}

	[Property("Assignment"), Description("Assign the player to which team")]
	public TeleTeamEnum TeamToTeleport { get; set; } = TeleTeamEnum.Unspecified;

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( SCSGame.Current.RoundStatus != SCSGame.RoundEnum.Post )
			return;

		if ( other is SCSPlayer player )
		{
			switch ( TeamToTeleport )
			{
				case TeleTeamEnum.Red:
					player.Transform = FindByName( "dest_redroom" ).Transform;
					break;
				case TeleTeamEnum.Blue:
					player.Transform = FindByName( "dest_blueroom" ).Transform;
					break;
				case TeleTeamEnum.Green:
					player.Transform = FindByName( "dest_greenroom" ).Transform;
					break;
				case TeleTeamEnum.Yellow:
					player.Transform = FindByName( "dest_yellowroom" ).Transform;
					break;
			}

			player.Position += Vector3.Zero;
		}
	}
}

