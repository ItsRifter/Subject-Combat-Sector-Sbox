using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox;

public partial class NPCBase : AnimEntity
{
	//Basics
	public virtual string BaseModel => "models/citizen/citizen.vmdl";
	public virtual int BaseHealth { get; set; } = 1;
	public virtual float BaseSpeed { get; set; } = 1;
	public virtual float NPCScale { get; set; } = 1;
	public virtual float AlertRange => 1;
	public virtual float AttackRange => 1;
	public virtual int AttackDamage { get; set; } = 1;
	public virtual float AttackCooldown { get; set; } = 1;

	private NPCBase curTarget;
	private TimeSince timeLastAttack;

	public enum TeamAssignEnum
	{
		Unknown,
		Red,
		Blue,
		Green,
		Yellow
	}

	public TeamAssignEnum TeamNPC = TeamAssignEnum.Unknown;

	[ConVar.Replicated]
	public static bool scs_npc_drawoverlay { get; set; }
	public NPCDebugDraw Draw => NPCDebugDraw.Once;

	Vector3 LookDir;

	Vector3 InputVelocity;

	public NPCSteering Steer;

	public override void Spawn()
	{ 
		SetModel( BaseModel );

		Scale = NPCScale;
		Health = BaseHealth;

		EnableHitboxes = true;

		Tags.Add( "NPC" );

		SetBodyGroup( 1, 0 );

		Steer = new NPCSteerWander();
	}

	public void SetStatsWithRarity( string rarity )
	{
		if ( rarity == "Rare" )
		{
			BaseHealth *= (int)1.25f;
			BaseSpeed *= 1.5f;
			AttackDamage *= (int)1.65f;
			AttackCooldown /= (int)1.25f;
		}
		else if ( rarity == "Legendary" )
		{
			BaseHealth *= (int)1.5f;
			BaseSpeed *= 1.75f;
			AttackDamage *= (int)1.95f;
			AttackCooldown /= (int)1.50f;
		}
		else if ( rarity == "Godlike" )
		{
			BaseHealth *= (int)1.75f;
			BaseSpeed *= 2.0f;
			AttackDamage *= (int)2.25f;
			AttackCooldown /= (int)1.75f;
		}
		else if ( rarity == "Awesome" )
		{
			BaseHealth *= (int)2.25f;
			BaseSpeed *= 2.25f;
			AttackDamage += (int)2.55f;
			AttackCooldown /= (int)2.0f;
		}
	}

	[Event.Tick.Server]
	public void Tick()
	{
		InputVelocity = 0;		

		if ( Steer != null || !IsValid )
		{
			Steer.Tick( Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, BaseSpeed * 1.5f );
			}

			if( scs_npc_drawoverlay )
				DebugOverlay.Sphere( Position + Vector3.Up * 64, AttackRange, Color.Red );

		}

		Move( Time.Delta );

		var walkVelocity = Velocity.WithZ( 0 );
		if ( walkVelocity.Length > 0.5f )
		{
			var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
			var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}

		var animHelper = new NPCAnimationHelper( this );

		LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		animHelper.WithLookAt( EyePosition + LookDir );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( InputVelocity );

		if( curTarget == null)
		{
			var entities = FindInSphere( Position + Vector3.Up * 64, AlertRange);

			foreach ( var ent in entities )
			{
				if ( ent is NPCBase hostile && hostile.TeamNPC != TeamNPC )
				{
					Steer = new NPCSteering();
					Steer.Target = hostile.Position;

					curTarget = hostile;
				}
			}
		}

		if( curTarget.IsValid() && Position.Distance( curTarget.Position ) <= AttackRange )
		{
			if ( timeLastAttack < AttackCooldown )
			return;

			Steer = null;
			AttackHostile( curTarget );

		} else if ( !curTarget.IsValid() || Position.Distance( curTarget.Position ) > AttackRange )
		{
			if( Steer == null)
				Steer = new NPCSteerWander();

			curTarget = null;
		}
	}

	public virtual void AttackHostile(NPCBase curTarget)
	{
		DamageInfo dmgInfo = new DamageInfo();
		dmgInfo.Damage = AttackDamage;
		dmgInfo.Attacker = this;

		curTarget.TakeDamage( dmgInfo );
		timeLastAttack = Rand.Float( -AttackCooldown, 0 );
	}

	protected virtual void Move( float timeDelta )
	{
		var bbox = BBox.FromHeightAndRadius( 64, 4 );

		MoveHelper move = new( Position, Velocity );
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox );

		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			move.TryUnstuck();
			move.TryMoveWithStep( timeDelta, 30 );
		}

		var tr = move.TraceDirection( Vector3.Down * 10.0f );

		if ( move.IsFloor( tr ) )
		{
			GroundEntity = tr.Entity;

			if ( !tr.StartedSolid )
			{
				move.Position = tr.EndPosition;
			}
			
			if ( InputVelocity.Length > 0 )
			{
				var movement = move.Velocity.Dot( InputVelocity.Normal );
				move.Velocity = move.Velocity - movement * InputVelocity.Normal;
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
				move.Velocity += movement * InputVelocity.Normal;

				NPCDebugDraw.Once.Line( tr.StartPosition, tr.EndPosition );
			}
			else
			{
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
			}
		}
		else
		{
			GroundEntity = null;
			move.Velocity += Vector3.Down * 900 * timeDelta;
			NPCDebugDraw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
		}

		Position = move.Position;
		Velocity = move.Velocity;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;

		Velocity += Input.Rotation * new Vector3( Input.Forward, Input.Left, Input.Up ) * BaseSpeed * 5 * Time.Delta;
		if ( Velocity.Length > BaseSpeed ) Velocity = Velocity.Normal * BaseSpeed;

		Velocity = Velocity.Approach( 0, Time.Delta * BaseSpeed * 3 );

		Position += Velocity * Time.Delta;

		EyePosition = Position;
	}

	public override void TakeDamage( DamageInfo info )
	{
		Health -= info.Damage;

		if ( Health <= 0 )
		{
			OnKilled();
		}
	}

	public override void OnKilled()
	{
		Event.Run( "scs_npckilled", this );
		base.OnKilled();
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;
		Position += Velocity * Time.Delta;
	}
}
