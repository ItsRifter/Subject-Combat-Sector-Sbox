using System;
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


	public RealTimeSince KeyInputDelay = 0f;

	private int roundSetting = 1;
	private int pointSetting = 4;
	public HostSetup()
	{
		SetTemplate("UI/HostSetup.html");
		StyleSheet.Load( "UI/HostSetup.scss" );
		HostPanel = Add.Panel( "hostMenu" );

		Panel hostSection = HostPanel.Add.Panel( "menu" );

		Panel roundPnl = HostPanel.Add.Panel( "roundPnl" );
		Panel pointsPnl = HostPanel.Add.Panel( "pointsPnl" );

		RoundIndexLbl = roundPnl.Add.Label();
		PointsLbl = pointsPnl.Add.Label();

		Panel roundBtnUp = roundPnl.Add.Panel( "btnUp" );

		roundBtnUp.AddEventListener( "onclick", () =>
		{
			UpdateRoundSetting( 1 );
		} );

		Panel roundBtnDown = roundPnl.Add.Panel( "btnDown" );

		roundBtnDown.AddEventListener( "onclick", () =>
		{
			UpdateRoundSetting( -1 );
		} );

		Panel pointsBtnUp = pointsPnl.Add.Panel( "btnUp" );

		pointsBtnUp.AddEventListener( "onclick", () =>
		{
			UpdatePointsSetting( 1 );
		} );

		Panel pointsBtnDown = pointsPnl.Add.Panel( "btnDown" );

		pointsBtnDown.AddEventListener( "onclick", () =>
		{
			UpdatePointsSetting( -1 );
		} );

		Panel submitBtn = HostPanel.Add.Panel( "submitBtn" );
		Label submitLbl = submitBtn.Add.Label( "Start game", "submitText" );

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

		if( roundSetting < 1)
			roundSetting = 1;
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

	public void SubmitSettings()
	{
		ConsoleSystem.Run( "scs_cmd_startgame", roundSetting, pointSetting );
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

		RoundIndexLbl.SetText( "Max Rounds: " + roundSetting );
		PointsLbl.SetText( "Starting Points: " + pointSetting );
	}
}
