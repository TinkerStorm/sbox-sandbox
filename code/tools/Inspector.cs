using System.Linq;

namespace Sandbox.Tools
{
	[Library( "tool_inspector", Title = "Inspector", Description = "Inspects entities", Group = "construction" )]
	public partial class InspectorTool : BaseTool
	{
		[Net] private Entity Selected { get; set; } = null;

		public override void Activate()
		{
			base.Activate();
			if (Host.IsServer)
			{
				// Selected = null;
			}
		}

		private Particles CreateParticles() => Particles.Create( "particles/physgun_freeze.vpcf" );

		public override void Simulate()
		{
			if ( !Host.IsServer )
				return;

			using ( Prediction.Off() )
			{
				if ( !Input.Pressed( InputButton.PrimaryAttack ) && !Input.Pressed( InputButton.SecondaryAttack ) )
					return;

				var tr = DoTrace();

				if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld )
					return;

				CreateHitEffects( tr.EndPosition );
				CreateParticles().SetPosition( 0, tr.Entity.Position );

				if ( Input.Pressed(InputButton.SecondaryAttack) )
				{
					if ( Selected != tr.Entity ) return;

					Selected = null;

					return;
				}

				// if ( tr.Entity is Player )
				//	return;

				// tr.Entity.Delete();
				Selected = tr.Entity;
			}
		}

		[Event.Frame]
		public override void OnFrame()
		{
			base.OnFrame();
			if (Selected != null)
			{
				if (!Selected.IsValid()) {
					Selected = null;
					return;
				}

				DebugOverlay.Text( Selected.Name, Selected.Position );

				DebugOverlay.ScreenText( Selected.Name, 3 );
				DebugOverlay.ScreenText( $"Position: {Selected.Position}", 4 );
				DebugOverlay.ScreenText( $"Rotation: {Selected.Rotation}", 5 );
			}
		}
	}
}
