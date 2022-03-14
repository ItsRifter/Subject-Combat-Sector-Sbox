using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class HostSetup : Panel
{
	public Panel HostPanel;
	public Label WaitingLbl;
	public Label HostNotice;
	public bool IsOpen = false;

	public HostSetup()
	{
		StyleSheet.Load( "UI/HostSetup.scss" );
		HostPanel = Add.Panel( "hostMenu" );

		Panel hostSection = HostPanel.Add.Panel( "menu" );

		HostNotice = Add.Label( "Press Q to setup game", "hoster" );
		WaitingLbl = Add.Label( "Waiting on Host to setup game", "nonhoster" );
	}

	public override void Tick()
	{
		base.Tick();

		HostNotice.SetClass( "active", Local.Client.IsListenServerHost && !IsOpen );
		WaitingLbl.SetClass( "active", !Local.Client.IsListenServerHost );

		HostPanel.SetClass( "active", IsOpen && Local.Client.IsListenServerHost );

		if (Input.Pressed(InputButton.Menu))
		{
			IsOpen = !IsOpen;
		}
	}
}
