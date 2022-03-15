using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class CrystalBoxInfo : Panel
{
	public Panel BoxPanel;
	public Label BoxNPCName;
	public Label BoxRarity;
	public Label BoxStrength;
	public Label BoxDesc;
	public Label BoxTier;

	public CrystalBoxInfo()
	{
		StyleSheet.Load( "UI/CrystalBoxInfo.scss" );
		BoxPanel = Add.Panel( "boxPanel" );

		BoxNPCName = BoxPanel.Add.Label( "?", "title" );
		BoxDesc = BoxPanel.Add.Label( "?", "desc" );
		BoxTier = BoxPanel.Add.Label( "?", "level" );
		BoxRarity = BoxPanel.Add.Label( "?", "level" );
		BoxStrength = BoxPanel.Add.Label( "?", "str" );

	}

	public override void Tick()
	{
		base.Tick();

		if(Local.Pawn is SCSPlayer player)
		{
			var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 150 )
				.Ignore( player )
				.EntitiesOnly()
				.Size( 2 )
				.Run();

			SetClass( "isHovering", tr.Entity is TeamCrystalBox);

			if(tr.Entity is TeamCrystalBox teamBox)
			{	
				BoxNPCName.SetText( teamBox.NPCToSpawn );
				BoxDesc.SetText( teamBox.NPCDescription );
				BoxTier.SetText( $"Tier: {teamBox.CrystalTierLevel}" );
				BoxRarity.SetText( $"Rarity: {teamBox.NPCRarity}" );
				BoxStrength.SetText( $"Strength: {teamBox.CrystalStrength}" );
			}
		}
	}
}
