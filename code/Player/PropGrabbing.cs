using Sandbox;
using Sandbox.Physics;
using System;
using System.Linq;

namespace SCS.Player;

public partial class SCSPawn
{
	private PhysicsBody holdBody;
	private FixedJoint holdJoint;
	public PhysicsBody HeldBody { get; private set; }
	public Rotation HeldRot { get; private set; }
	public ModelEntity HeldEntity { get; private set; }

	TimeSince timeSinceDrop;
	//TimeSince timeSinceGrabbed;
	public void SimulateGrabbing()
	{
		if ( !Game.IsServer )
			return;

		if ( Input.Pressed( InputButton.Use ))
		{
			if( HeldBody.IsValid() )
				GrabEnd();

			using ( Prediction.Off() )
			{
				if ( timeSinceDrop < 0.1f )
					return;

				var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 120 )
					.UseHitboxes()
					.Ignore( this, false )
					.Radius( 4.0f )
					.Run();

				if ( !tr.Entity.IsValid() )
					return;

				if ( !tr.Body.IsValid() )
					return;

				if ( tr.Entity.IsWorld )
					return;

				if ( tr.Entity.PhysicsGroup == null )
					return;

				if ( tr.Entity is not ModelEntity box )
					return;

				if ( tr.Entity is ModelEntity crystalBox )
				{
					GrabStart( crystalBox, tr.Body, EyePosition + EyeRotation.Forward * 120, EyeRotation );
				}
			}
		}

		if( HeldBody.IsValid() || HeldEntity.IsValid())
		{
			if(GroundEntity == HeldEntity)
			{
				GrabEnd();
			}

			if ( Input.Pressed( InputButton.PrimaryAttack ) && HeldBody.IsValid() )
			{
				timeSinceDrop = 0;
				HeldBody.ApplyImpulse( EyeRotation.Forward * (HeldBody.Mass * 250.0f) );
				GrabEnd();
			}
			else if ( Input.Pressed( InputButton.SecondaryAttack ) )
			{
				timeSinceDrop = 0;
				GrabEnd();
			}

			GrabMove( EyePosition, EyeRotation.Forward, EyeRotation);
		} 
		else
		{
			GrabEnd();
		}
	}

	private void GrabStart( ModelEntity box, PhysicsBody body, Vector3 grabPos, Rotation grabRot )
	{
		if ( !body.IsValid() )
			return;

		if ( body.PhysicsGroup == null )
			return;

		GrabEnd();

		HeldBody = body;
		HeldRot = grabRot.Inverse * HeldBody.Rotation;

		holdBody.Position = grabPos;
		holdBody.Rotation = HeldBody.Rotation;

		HeldBody.Sleeping = false;
		HeldBody.AutoSleep = false;

		holdJoint = PhysicsJoint.CreateFixed( holdBody, HeldBody.MassCenterPoint() );
		holdJoint.SpringLinear = new( 10.0f, 1.0f );
		holdJoint.SpringAngular = new( 1.0f, 1.0f );
		holdJoint.Strength = HeldBody.Mass * 1000.0f;

		HeldEntity = box;

		Client?.Pvs.Add( HeldEntity );
	}

	private void GrabEnd()
	{
		holdJoint?.Remove();
		holdJoint = null;

		if ( HeldBody.IsValid() )
		{
			HeldBody.AutoSleep = true;
		}

		if ( HeldEntity.IsValid() )
		{
			Client?.Pvs.Remove( HeldEntity );
		}

		HeldBody = null;
		HeldRot = Rotation.Identity;
		HeldEntity = null;
	}

	private void GrabMove( Vector3 startPos, Vector3 dir, Rotation rot )
	{
		if ( !HeldBody.IsValid() )
			return;

		var attachPos = HeldBody.FindClosestPoint( startPos );
		var holdDistance = 50.0f + attachPos.Distance( HeldBody.MassCenter );

		holdBody.Position = startPos + dir * holdDistance;
		holdBody.Rotation = rot * HeldRot;
	}
}
