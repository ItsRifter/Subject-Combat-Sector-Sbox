using Sandbox;
using System;
using System.Linq;

namespace SCS.Player;

public partial class SCSPawn : AnimatedEntity
{
	[Net, Predicted] public StandardController Controller { get; protected set; }

	public Entity ActiveChild { get; set; }
	[Predicted] Entity LastActiveChild { get; set; }

	public Vector3 EyePosition 
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	[Net, Predicted] public Vector3 EyeLocalPosition { get; set; }
	[Net, Predicted] public Rotation EyeLocalRotation { get; set; }

	public override Ray AimRay => new Ray( EyePosition, EyeRotation.Forward );

	public void MoveToSpawnpoint()
	{
		Entity spawnpoint = All.OfType<SpawnPoint>().OrderBy( x => Guid.NewGuid() ).FirstOrDefault();

		if ( spawnpoint == null )
			return;

		Transform = spawnpoint.Transform;

		SetViewAngles( To.Single(this), spawnpoint.Rotation.Angles() );
		ResetInterpolation();
	}

	public void CreatePhysHull()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

		EnableHitboxes = true;
		EnableLagCompensation = true;
		EnableAllCollisions = true;
	}

	[ConCmd.Server("kill")]
	public static void RespawnCMD()
	{
		(ConsoleSystem.Caller.Pawn as SCSPawn).Spawn();
	}

	[ConCmd.Server("noclip")]
	public static void ToggleNoclipping()
	{
		var player = ConsoleSystem.Caller.Pawn as SCSPawn;
		if ( player == null ) return;

		if ( player.Controller is not NoclipController )
			player.Controller = new NoclipController( player );
		else if (player.Controller is NoclipController)
			player.Controller = new StandardController( player );
	}

	public override void Spawn()
	{
		LifeState = LifeState.Alive;

		Controller = new StandardController( this );
		SetModel( "models/citizen/citizen.vmdl" );
		CreatePhysHull();
		MoveToSpawnpoint();

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
	}

	[ClientInput] public Vector3 InputDirection { get; protected set; }
	[ClientInput] public Angles ViewAngles { get; set; }

	bool setView;
	Angles setAngles;

	[ClientRpc]
	public void SetViewAngles(Angles angle)
	{
		setAngles = angle;
		setView = true;
	}

	public override void BuildInput()
	{
		if( setView )
		{
			ViewAngles = setAngles;
			setView = false;
		}

		InputDirection = Input.AnalogMove;

		var look = Input.AnalogLook;

		var viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -89, 89 );
		ViewAngles = viewAngles.Normal;
	}

	void SimulateAnimator()
	{
		var helper = new CitizenAnimationHelper( this );

		helper.WithLookAt( AimRay.Position + AimRay.Forward );
		helper.WithVelocity( Velocity );
		helper.WithWishVelocity( Controller.WishVelocity );

		helper.DuckLevel = Input.Down( InputButton.Duck ) ? 0.5f : 0.0f;
		
		helper.HoldType = CitizenAnimationHelper.HoldTypes.None;

		Rotation rotation = ViewAngles.ToRotation();
		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp( Rotation, idealRotation, Controller.WishVelocity.Length * Time.Delta * 0.05f );
	}

	public override void Simulate( IClient cl )
	{
		base.Simulate( cl );

		Controller?.Simulate();
		SimulateAnimator();
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );
	}

	public override void OnKilled()
	{
		LifeState = LifeState.Dead;
		Controller = null;

		EnableHitboxes = false;
		EnableAllCollisions = false;
		EnableLagCompensation = false;
	}

	public override void FrameSimulate( IClient cl )
	{
		base.FrameSimulate( cl );

		Controller?.FrameSimulate();

		Camera.Position = EyePosition;
		Camera.Rotation = ViewAngles.ToRotation();
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		Camera.FirstPersonViewer = this;
	}

	public TraceResult GetEyeTrace(float dist = 25.0f, float size = 1.0f)
	{
		var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * dist )
			.Ignore( this )
			.UseHitboxes( true )
			.Size( size )
			.Run();

		return tr;
	}
}
