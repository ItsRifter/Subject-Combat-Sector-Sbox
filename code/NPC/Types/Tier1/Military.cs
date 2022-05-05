using Sandbox;
using System;
using System.Linq;

public partial class Military : NPCBase
{
	public override int BaseHealth => 250;
	public override float BaseSpeed => 15;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 1.0f;
	public override float AlertRange => 100;
	public override float AttackRange => 125;
	public override int AttackDamage => 35;
	public override float AttackCooldown => 2.10f;

	public override void Spawn()
	{
		base.Spawn();

		var beard = new ModelEntity();
		beard.SetModel( "models/citizen_clothes/beards/beard_trucker_brown.vmdl_c" );
		beard.SetParent( this, true );

		var vest = new ModelEntity();
		vest.SetModel( "models/citizen_clothes/vest/Tactical_Vest/Models/tactical_vest.vmdl_c" );
		vest.SetParent( this, true );

		SetAnimParameter( "holdtype", 1 );
		SetAnimParameter( "aim_body_weight", 1.0f );
	}
}
