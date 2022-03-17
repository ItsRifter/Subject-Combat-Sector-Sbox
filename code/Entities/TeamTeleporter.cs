using System;
using Sandbox;

[Library( "scs_team_teleport", Description = "Behaves like a teleporter but only teleports specific team types" )]
[Hammer.AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
[Hammer.Model]
[Hammer.SupportsSolid]
public partial class TeamTeleporter : TriggerTeleport
{
	[Property( "RedRoomTarget" )]
	public string RedTarget { get; set; }

	[Property("BlueRoomTarget")]
	public string BlueTarget { get; set; }

	[Property( "GreenRoomTarget" )]
	public string GreenTarget { get; set; }

	[Property( "YellowRoomTarget" )]
	public string YellowTarget { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Enabled = !StartDisabled;
	}

	public override void StartTouch( Entity other )
	{
		if ( !Enabled ) return;
		
		base.StartTouch( other );
		
		var targetRed = Entity.FindByName( RedTarget );
		var targetBlue = Entity.FindByName( BlueTarget );
		var targetGreen = Entity.FindByName( GreenTarget );
		var targetYellow = Entity.FindByName( YellowTarget );

		if ( other is SCSPlayer player )
		{
			switch ( player.CurTeam )
			{
				case SCSPlayer.TeamEnum.Red:
					player.Transform = targetRed.Transform;
					break;
				case SCSPlayer.TeamEnum.Blue:
					player.Transform = targetBlue.Transform;
					break;
				case SCSPlayer.TeamEnum.Green:
					player.Transform = targetGreen.Transform;
					break;
				case SCSPlayer.TeamEnum.Yellow:
					player.Transform = targetYellow.Transform;
					break;
			}

			player.Position += Vector3.Zero;
		}
	}
}

