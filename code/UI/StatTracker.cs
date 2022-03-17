using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class StatTracker : Panel
{
	public Panel StatsHud;
	public Label PointTracker;
	public Label CurrentTeam;
	public Label WinsCount;
	public TeamPoints teamPoints;
	public StatTracker()
	{
		StyleSheet.Load( "UI/StatTracker.scss" );

		PointTracker = Add.Label( "Points: " );
		CurrentTeam = Add.Label( "Current Team" );
		WinsCount = Add.Label( "Score: " );
	}

	public override void Tick()
	{
		base.Tick();


		if ( Local.Pawn is SCSPlayer player )
		{
			if ( player.CurTeam == SCSPlayer.TeamEnum.Unspecified )
			{
				SetClass( "active", false );
				return;
			}

			SetClass( "active", true );

			if ( teamPoints == null )
			{
				teamPoints = Entity.FindByName( player.CurTeam.ToString() + "_point_counter" ) as TeamPoints;
			}

			if(SCSGame.Current.GetWinningTeam() != SCSPlayer.TeamEnum.Unspecified)
			{
				WinsCount.SetText( SCSGame.Current.GetWinningTeam() + " team has won the game!" );
				CurrentTeam.SetText( "" );
				PointTracker.SetText( "" );
				return;
			}

			PointTracker.SetText( $"Research points: {teamPoints.GetTotalPoints()}" );
			CurrentTeam.SetText( $"{player.CurTeam} Team" );

			switch ( player.CurTeam )
			{
				case SCSPlayer.TeamEnum.Red:
					WinsCount.SetText( $"Score: {SCSGame.Current.GetTeamScore( "red" )}/{SCSGame.Current.GetTargetScore()}" );
					break;
				case SCSPlayer.TeamEnum.Blue:
					WinsCount.SetText( $"Score: {SCSGame.Current.GetTeamScore( "blue" )}/{SCSGame.Current.GetTargetScore()}" );
					break;
				case SCSPlayer.TeamEnum.Green:
					WinsCount.SetText( $"Score: {SCSGame.Current.GetTeamScore( "green" )}/{SCSGame.Current.GetTargetScore()}" );
					break;
				case SCSPlayer.TeamEnum.Yellow:
					WinsCount.SetText( $"Score: {SCSGame.Current.GetTeamScore( "yellow" )}/{SCSGame.Current.GetTargetScore()}" );
					break;
			}

		}
	}
}

