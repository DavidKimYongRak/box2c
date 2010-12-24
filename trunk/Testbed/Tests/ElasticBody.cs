using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;

namespace Testbed.Tests
{
	public class ElasticBody : Test
	{
		Body[] bodies = new Body[TotalBodies];
		Body  m_ground;

		public static string Name
		{
			get { return "ElasticBody"; }
		}

		public override void BeginContact(Contact contact)
		{
		}

		public override void EndContact(Contact contact)
		{
		}

		public override void PostSolve(Contact contact, ContactImpulse impulse)
		{
		}

		const int BodyCountX = 4;
		const int BodyCountY = 4;
		const int TotalBodies = (BodyCountX * BodyCountY);
		const float width = 0.55f;
		const float height = 0.55f;

		/// Main...
		public ElasticBody()
		{
			Program.MainForm.ViewZoom = 0.25f;

			/// Bottom static body
			{ 
				PolygonShape sd = new PolygonShape();
				sd.SetAsBox(50.0f, 2.0f);
				BodyDef bd = new BodyDef();
				bd.Position = new Vec2(-1.0f, -7.5f);

				m_ground = m_world.CreateBody(bd);
				m_ground.CreateFixture(new FixtureDef(sd, 0.0f, 0.1f, 0.1f));
			}

			/// "Elastic body" 64 bodies - something like a lin. elastic compound
			/// connected via dynamic forces (springs) 
			{
				PolygonShape sd = new PolygonShape();
				sd.SetAsBox(width, height);
				
				FixtureDef sdf = new FixtureDef();
				sdf.Density    = 1.5f;
				sdf.Friction   = 0.01f;
				sdf.Filter = new FilterData(FilterData.Default.CategoryBits, FilterData.Default.MaskBits, -1);
				sdf.Shape = sd;
				Vec2 startpoint = new Vec2(30.0f, 20.0f);
				BodyDef    bd = new BodyDef();
				bd.BodyType = EBodyType.b2_dynamicBody;
				bd.Bullet = false;
  	 			//bd.AllowSleep = false;
				for (int i = 0; i < BodyCountY; ++i) 
				{
					for (int j = 0; j < BodyCountX; ++j) 
					{
						bd.Position = new Vec2(j*(width*2), 2.51f + (height*2) * i);
						bd.Position  += startpoint;
						Body body  = m_world.CreateBody(bd);
						bodies[BodyCountX*i+j] = body;
						body.CreateFixture(sdf);
					}
				}
			}
		}
		///  Apply dynamic forces (springs) and check elevator state
		public override void  Step()
		{
			base.Step();

			for (int i=0; i<BodyCountX; ++i)
			{
				for (int j=0; j<BodyCountY; ++j)
				{
					Vec2 zero = new Vec2(0.0f,0.0f);
					Vec2 down = new Vec2(0.0f, -0.50f);
					Vec2 up = new Vec2(0.0f, 0.50f);
					Vec2 right = new Vec2(0.50f, 0.0f);
					Vec2 left = new Vec2(-0.50f, 0.0f);
					int ind = i*BodyCountX+j;
					int indr = ind+1;
					int indd = ind+BodyCountY;
					float spring = 500.0f;
					float damp = 5.0f;
					if (j<BodyCountX-1)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[indr]),zero,spring, damp, 1.0f);
						AddSpringForce((bodies[ind]), right, (bodies[indr]), left, 0.50f*spring, damp, 0.0f);
					}
					if (i<BodyCountY-1)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[indd]),zero,spring, damp, 1.0f);
						AddSpringForce((bodies[ind]), up, (bodies[indd]), down, 0.50f*spring, damp, 0.0f);
					}
					int inddr = indd + 1;
					int inddl = indd - 1;
					float drdist = (float)Math.Sqrt(2.0f);
					if (i < BodyCountY-1 && j < BodyCountX-1)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[inddr]),zero,spring, damp, drdist);
					}
					if (i < BodyCountY-1 && j > 0)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[inddl]),zero,spring, damp, drdist);
					}

					indr = ind+2;
					indd = ind+BodyCountX*2;
					if (j<BodyCountX-2)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[indr]),zero,spring, damp, 2.0f);
					}
					if (i<BodyCountY-2)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[indd]),zero,spring,damp,2.0f);
					}

					inddr = indd + 2;
					inddl = indd - 2;
					drdist = (float)Math.Sqrt(2.0f)*2.0f;
					if (i < BodyCountY-2 && j < BodyCountX-2)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[inddr]),zero,spring, damp, drdist);
					}
					if (i < BodyCountY-2 && j > 1)
					{
						AddSpringForce((bodies[ind]),zero,(bodies[inddl]),zero,spring, damp, drdist);
					}
				}
			}
		}
	   /// Add a spring force
	   void AddSpringForce(Body bA, Vec2 localA, Body bB, Vec2 localB, float k, float friction, float desiredDist)
	   {
			Vec2 pA = bA.GetWorldPoint(localA);
			Vec2 pB = bB.GetWorldPoint(localB);
			Vec2 diff = pB - pA;
			//Find velocities of attach points
			Vec2 vA = bA.LinearVelocity - (bA.GetWorldVector(localA).Cross(bA.AngularVelocity));
			Vec2 vB = bB.LinearVelocity - (bB.GetWorldVector(localB).Cross(bB.AngularVelocity));
			Vec2 vdiff = vB-vA;
			float dx = diff.Normalize(); //normalizes diff and puts length into dx
			float vrel = vdiff.x*diff.x + vdiff.y*diff.y;
			float forceMag = -k*(dx-desiredDist) - friction*vrel;
			diff *= forceMag; // diff *= forceMag
			bB.ApplyForce(diff, bA.GetWorldPoint(localA));
			diff *= -1.0f;
			bA.ApplyForce(diff, bB.GetWorldPoint(localB));
		}

	   public override void Draw()
	   {
		   base.Draw();

		   List<Vec2> points = new List<Vec2>();

		   for (int i = 0; i < BodyCountX; ++i)
		   {
			   // m_debugDraw.DrawPoint(bodies[i].WorldCenter, 2, new ColorF(1, 0, 0));
			   points.Add(bodies[i].WorldCenter);
		   }
		   for (int i = 1; i < BodyCountY; ++i)
		   {
			   // m_debugDraw.DrawPoint(bodies[8 * i + 7].WorldCenter, 2, new ColorF(1, 0, 0));
			   points.Add(bodies[BodyCountX * i + (BodyCountX - 1)].WorldCenter);
		   }
		   for (int i = BodyCountX - 1; i >= 0; --i)
		   {
			   //m_debugDraw.DrawPoint(bodies[(64 - 8) + i].WorldCenter, 2, new ColorF(1, 0, 0));
			   points.Add(bodies[(TotalBodies - BodyCountX) + i].WorldCenter);
		   }

		   for (int i = BodyCountY - 2; i >= 1; --i)
		   {
			   //m_debugDraw.DrawPoint(bodies[8 * i].WorldCenter, 2, new ColorF(1, 0, 0));
			   points.Add(bodies[BodyCountX * i].WorldCenter);
		   }

		   for (int i = 0; i < points.Count; ++i)
		   {
			   int p1 = i;
			   int p2 = (p1 == points.Count - 1) ? 0 : p1 + 1;

			   m_debugDraw.DrawSegment(points[p1], points[p2], new ColorF(1, 0, 0));
		   }
	   }
	};
}
