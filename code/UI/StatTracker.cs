using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class StatTracker : Panel
{
	public Panel StatsHud;
	public Label PointTracker;
	public TeamPoints teamPoints;
	public StatTracker()
	{
		StyleSheet.Load( "UI/StatTracker.scss" );

		PointTracker = Add.Label( "Points: " );
	}

	public override void Tick()
	{
		base.Tick();


		if( Local.Pawn is SCSPlayer player )
		{
			if ( player.CurTeam == SCSPlayer.TeamEnum.Unspecified )
			{
				SetClass( "active", false );
				return;
			}

			SetClass( "active", true );

			if (teamPoints == null)
			{
				teamPoints = Entity.FindByName( player.CurTeam.ToString() + "_point_counter" ) as TeamPoints;
			}

			PointTracker.SetText( $"Points: {teamPoints.GetTotalPoints()}" );
		}
	}
}

