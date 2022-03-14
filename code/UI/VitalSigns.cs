using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class VitalSigns : Panel
{
	public Panel VitalHud;
	public Panel HealthIcon;
	public Image HealthHundredthIcon;
	public Image HealthTenthIcon;
	public Image HealthUnitIcon;

	public VitalSigns()
	{
		StyleSheet.Load( "UI/VitalSigns.scss" );

		VitalHud = Add.Panel( "vitalsigns" );
		
		HealthIcon = VitalHud.Add.Panel( "healthIcon" );

		HealthHundredthIcon = VitalHud.Add.Image( "", "healthHundreth" );
		HealthTenthIcon = VitalHud.Add.Image( "", "healthTenth" );
		HealthUnitIcon = VitalHud.Add.Image( "", "healthUnit" );
	}

	public override void Tick()
	{
		base.Tick();

		if( Local.Pawn is SCSPlayer player )
		{
			VitalHud.SetClass( "active", player.Health > 0 );
			
			if( player.Health == 100 )
			{
				HealthHundredthIcon.Style.SetBackgroundImage( "ui/health_1.png" );
				HealthTenthIcon.Style.SetBackgroundImage( "ui/health_0.png" );
				HealthUnitIcon.Style.SetBackgroundImage( "ui/health_0.png" );

				HealthTenthIcon.Style.Left = 88;
				HealthUnitIcon.Style.Left = 116;

			} else if (player.Health < 100 && player.Health > 10)
			{
				HealthHundredthIcon.Style.SetBackgroundImage( "" );

				char trimmedTenth = player.Health.ToString()[0];
				char trimmedUnit = player.Health.ToString()[1];

				HealthTenthIcon.Style.SetBackgroundImage( "ui/health_" + trimmedTenth + ".png" );
				HealthUnitIcon.Style.SetBackgroundImage( "ui/health_" + trimmedUnit + ".png" );

				HealthTenthIcon.Style.Left = 64;
				HealthUnitIcon.Style.Left = 92;

			} else if (player.Health < 10 && player.Health > 0)
			{
				HealthHundredthIcon.Style.SetBackgroundImage( "" );
				HealthTenthIcon.Style.SetBackgroundImage( "" );
				char trimmedUnit = player.Health.ToString()[0];

				HealthUnitIcon.Style.SetBackgroundImage( "ui/health_" + trimmedUnit + ".png" );

				HealthUnitIcon.Style.Left = 64;
			}

		}
	}
}

