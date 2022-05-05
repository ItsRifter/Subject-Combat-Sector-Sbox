using Sandbox;
using System;
using System.Linq;

public partial class Scout : NPCBase
{
	public override int BaseHealth => 185;
	public override float BaseSpeed => 50;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 0.8f;
	public override float AlertRange => 150;
	public override float AttackRange => 75;
	public override int AttackDamage => 20;
	public override float AttackCooldown => 1.25f;
	public override void Spawn()
	{
		base.Spawn();

		SetAnimParameter( "holdtype", 1 );
		SetAnimParameter( "aim_body_weight", 1.0f );
	}


}
