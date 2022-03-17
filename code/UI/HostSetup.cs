using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class HostSetup : Panel
{
	public Panel HostPanel;
	public Label WaitingLbl;
	public Label HostNotice;
	public Label RoundIndexLbl;
	public Label PointsLbl;

	public bool IsOpen = false;
	public bool GameStarted = false;
	public bool[] TeamsEnabled = new bool[4] { false, false, false, false };

	Panel redPnl;
	Panel bluePnl;
	Panel greenPnl;
	Panel yellowPnl;
	RealTimeSince KeyInputDelay = 0f;

	private int roundSetting = 3;
	private int pointSetting = 4;
	public HostSetup()
	{
		SetTemplate("UI/HostSetup.html");
		StyleSheet.Load( "UI/HostSetup.scss" );
		HostPanel = Add.Panel( "hostMenu" );

		Panel hostSection = HostPanel.Add.Panel();
		Panel PointsControl = HostPanel.Add.Panel( "PointsControl" );

			Panel roundPnl = PointsControl.Add.Panel( "roundPnl" );
			Panel pointsPnl = PointsControl.Add.Panel( "pointsPnl" );

			Panel roundBtnUp = roundPnl.Add.Panel( "btnUp" );
			roundBtnUp.AddEventListener( "onclick", () =>
			{
				UpdateRoundSetting( 1 );
			} );

			RoundIndexLbl = roundPnl.Add.Label();
			RoundIndexLbl.AddClass( "setupText" );

			Panel roundBtnDown = roundPnl.Add.Panel( "btnDown" );
			roundBtnDown.AddEventListener( "onclick", () =>
			{
				UpdateRoundSetting( -1 );
			} );

			// 

			Panel pointsBtnUp = pointsPnl.Add.Panel( "btnUp" );
			pointsBtnUp.AddEventListener( "onclick", () =>
			{
				UpdatePointsSetting( 1 );
			} );

			PointsLbl = pointsPnl.Add.Label();
			PointsLbl.AddClass("setupText");

			Panel pointsBtnDown = pointsPnl.Add.Panel( "btnDown" );
			pointsBtnDown.AddEventListener( "onclick", () =>
			{
				UpdatePointsSetting( -1 );
			} );

		Panel teamsPnl = HostPanel.Add.Panel( "teamsPnl" );
		Label teamSelectLbl = teamsPnl.Add.Label("Select teams for this game", "text");
		Panel TeamsSelect = teamsPnl.Add.Panel( "teamsSel" );

		redPnl = TeamsSelect.Add.Panel( "redIcon" );
		redPnl.AddEventListener( "onclick", () =>
		{
			TeamsEnabled[0] = !TeamsEnabled[0];
		} );

		bluePnl = TeamsSelect.Add.Panel( "blueIcon" );
		bluePnl.AddEventListener( "onclick", () =>
		{
			TeamsEnabled[1] = !TeamsEnabled[1];
		} );

		greenPnl = TeamsSelect.Add.Panel( "greenIcon" );
		greenPnl.AddEventListener( "onclick", () =>
		{
			TeamsEnabled[2] = !TeamsEnabled[2];
		} );

		yellowPnl = TeamsSelect.Add.Panel( "yellowIcon" );
		yellowPnl.AddEventListener( "onclick", () =>
		{
			TeamsEnabled[3] = !TeamsEnabled[3];
		} );

		Panel submitBtn = HostPanel.Add.Panel( "submitBtn" );
		Label submitLbl = submitBtn.Add.Label( "Confirm settings", "submitText" );
		submitBtn.AddEventListener( "onclick", () =>
		{
			SubmitSettings();
			IsOpen = false;
		} );
		
		HostNotice = Add.Label( "Press Q to setup game", "hoster" );
		WaitingLbl = Add.Label( "Waiting on Host to setup game", "nonhoster" );
	}

	private void UpdateRoundSetting(int rndUpdate)
	{
		roundSetting += rndUpdate;

		if( roundSetting < 3)
			roundSetting = 3;
		else if ( roundSetting > 20)
			roundSetting = 20;
	}

	private void UpdatePointsSetting( int pointUpdate )
	{
		pointSetting += pointUpdate;

		if ( pointSetting < 4)
			pointSetting = 4;
		else if ( pointSetting > 99 )
			pointSetting = 99;
	}

	public bool CanPlay(bool[] teamsEnabled )
	{
		List<bool> canPlayGame = new List<bool>();

		for ( int i = 0; i < 4; i++ )
		{
			if ( teamsEnabled[i] )
				canPlayGame.Add( teamsEnabled[i] );
		}

		if ( canPlayGame.Count < 2 )
		{
			Log.Error( "Only one or no teams are enabled, there should be at minimum two teams enabled" );
			return false;
		}

		if(Client.All.Count < canPlayGame.Count)
		{
			Log.Error( "There are less players to fill every team making this game unplayable" );
			return false;
		}

		return true;
	}

	public void SubmitSettings()
	{
		bool[] teamCheck = new bool[4] { TeamsEnabled[0], TeamsEnabled[1], TeamsEnabled[2], TeamsEnabled[3] };

		if ( !CanPlay( teamCheck ) )
			return;

		ConsoleSystem.Run( "scs_cmd_startgame", roundSetting, pointSetting, TeamsEnabled[0], TeamsEnabled[1], TeamsEnabled[2], TeamsEnabled[3] );
		GameStarted = true;
	}

	public override void Tick()
	{
		base.Tick();

		if ( GameStarted )
		{
			HostNotice.SetClass( "active", false );
			WaitingLbl.SetClass( "active", false );
			HostPanel.SetClass( "active", false );
			return;
		}

		HostNotice.SetClass( "active", Local.Client.IsListenServerHost && !IsOpen );
		WaitingLbl.SetClass( "active", !Local.Client.IsListenServerHost );

		HostPanel.SetClass( "active", IsOpen && Local.Client.IsListenServerHost );

		if ( Local.Client.IsListenServerHost && Input.Pressed(InputButton.Menu) && KeyInputDelay >= 0.1f)
		{
			IsOpen = !IsOpen;
			KeyInputDelay = 0f;
		}

		RoundIndexLbl.SetText( "Points to win: " + roundSetting );
		PointsLbl.SetText( "Starting Points: " + pointSetting );

		redPnl.SetClass( "active", TeamsEnabled[0] );
		bluePnl.SetClass( "active", TeamsEnabled[1] );
		greenPnl.SetClass( "active", TeamsEnabled[2] );
		yellowPnl.SetClass( "active", TeamsEnabled[3] );
	}
}
