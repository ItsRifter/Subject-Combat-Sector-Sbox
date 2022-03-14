using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox;

public partial class NPCBase : AnimEntity
{
	//Basics
	public virtual string BaseModel => "models/citizen/citizen.vmdl";
	public virtual int BaseHealth => 1;
	public virtual float BaseSpeed { get; set; } = 1;
	public virtual float NPCScale => 1;

	public enum TeamAssignEnum
	{
		Unknown,
		Red,
		Blue,
		Green,
		Yellow
	}

	public TeamAssignEnum TeamNPC = TeamAssignEnum.Unknown;

	public enum SpecialType
	{
		Standard,
		Armoured,
		Cloaked
	}

	public virtual SpecialType NPCType => SpecialType.Standard;

	[ConVar.Replicated]
	public static bool scs_npc_drawoverlay { get; set; }
	public NPCDebugDraw Draw => NPCDebugDraw.Once;

	Vector3 LookDir;

	Vector3 InputVelocity;

	NPCNavigation Path = new NPCNavigation();
	public NPCSteering Steer;

	public override void Spawn()
	{ 
		SetModel( BaseModel );

		Scale = NPCScale;
		Health = BaseHealth;

		EnableHitboxes = true;

		Tags.Add( "NPC" );

		SetBodyGroup( 1, 0 );

		Steer = new NPCSteering();
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
				DebugOverlay.Sphere( Position + Vector3.Down * 8, 42, Color.Red );

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
