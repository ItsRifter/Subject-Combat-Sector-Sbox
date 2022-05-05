using Sandbox;
using System;
using System.Linq;

public partial class Security : NPCBase
{
	public override int BaseHealth => 200;
	public override float BaseSpeed => 35;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float NPCScale => 1.0f;
	public override float AlertRange => 120;
	public override float AttackRange => 70;
	public override int AttackDamage => 20;
	public override float AttackCooldown => 2.5f;
	public override void Spawn()
	{
		base.Spawn();
	}
}
