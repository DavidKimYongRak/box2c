using System;
using Tao.FreeGlut;
using Tao.OpenGl;
using Box2CS;

namespace Editor
{
	public class TestDebugDraw : DebugDraw
	{
		public static void DrawString(int x, int y, string str)
		{
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();
			int w = (int)Main.GLWindow.Width;
			int h = (int)Main.GLWindow.Height;
			Glu.gluOrtho2D(0, w, h, 0);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glPushMatrix();
			Gl.glLoadIdentity();

			Gl.glColor3f(0.9f, 0.6f, 0.6f);
			Gl.glRasterPos2i(x, y);

			foreach (var c in str)
				Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_12, c);

			Gl.glPopMatrix();
			Gl.glMatrixMode(Gl.GL_PROJECTION);
			Gl.glPopMatrix();
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
		}

		public static void DrawStringFollow(float x, float y, string str)
		{
			Gl.glColor3f(0.9f, 0.6f, 0.6f);
			Gl.glRasterPos2f(x, y);

			foreach (var c in str)
				Glut.glutBitmapCharacter(Glut.GLUT_BITMAP_HELVETICA_12, c);
		}

		public override void DrawCircle(Vec2 center, float radius, ColorF color)
		{
			float k_segments = (int)(1024.0f * (radius / 50));
			float k_increment = (float)(2.0f * Math.PI / k_segments);
			float theta = 0.0f;
			Gl.glColor3f(color.R, color.G, color.B);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < k_segments; ++i)
			{
				Vec2 v = center + radius * new Vec2((float)Math.Cos(theta), (float)Math.Sin(theta));
				Gl.glVertex2f(v.X, v.Y);
				theta += k_increment;
			}
			Gl.glEnd();
		}

		public override void DrawPolygon(Vec2[] vertices, int vertexCount, ColorF color)
		{
			Gl.glColor3f(color.R, color.G, color.B);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < vertexCount; ++i)
			{
				Gl.glVertex2f(vertices[i].X, vertices[i].Y);
			}
			Gl.glEnd();
		}

		public override void DrawSegment(Vec2 p1, Vec2 p2, ColorF color)
		{
			Gl.glColor3f(color.R, color.G, color.B);
			Gl.glBegin(Gl.GL_LINES);
			Gl.glVertex2f(p1.X, p1.Y);
			Gl.glVertex2f(p2.X, p2.Y);
			Gl.glEnd();
		}

		public override void DrawSolidCircle(Vec2 center, float radius, Vec2 axis, ColorF color)
		{
			float k_segments = (int)(1024.0f * (radius / 50));
			float k_increment = (float)(2.0f * Math.PI / k_segments);
			float theta = 0.0f;
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glColor4f(0.5f * color.R, 0.5f * color.G, 0.5f * color.B, 0.5f);
			Gl.glBegin(Gl.GL_TRIANGLE_FAN);
			for (int i = 0; i < k_segments; ++i)
			{
				Vec2 v = center + radius * new Vec2((float)Math.Cos(theta), (float)Math.Sin(theta));
				Gl.glVertex2f(v.X, v.Y);
				theta += k_increment;
			}
			Gl.glEnd();
			Gl.glDisable(Gl.GL_BLEND);

			theta = 0.0f;
			Gl.glColor4f(color.R, color.G, color.B, 1.0f);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < k_segments; ++i)
			{
				Vec2 v = center + radius * new Vec2((float)Math.Cos(theta), (float)Math.Sin(theta));
				Gl.glVertex2f(v.X, v.Y);
				theta += k_increment;
			}
			Gl.glEnd();

			Vec2 p = center + radius * axis;
			Gl.glBegin(Gl.GL_LINES);
			Gl.glVertex2f(center.X, center.Y);
			Gl.glVertex2f(p.X, p.Y);
			Gl.glEnd();
		}

		public override void DrawSolidPolygon(Vec2[] vertices, int vertexCount, ColorF color)
		{
			Gl.glEnable(Gl.GL_BLEND);
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
			Gl.glColor4f(0.5f * color.R, 0.5f * color.G, 0.5f * color.B, 0.5f);
			Gl.glBegin(Gl.GL_TRIANGLE_FAN);
			for (int i = 0; i < vertexCount; ++i)
			{
				Gl.glVertex2f(vertices[i].X, vertices[i].Y);
			}
			Gl.glEnd();
			Gl.glDisable(Gl.GL_BLEND);

			Gl.glColor4f(color.R, color.G, color.B, 1.0f);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			for (int i = 0; i < vertexCount; ++i)
			{
				Gl.glVertex2f(vertices[i].X, vertices[i].Y);
			}
			Gl.glEnd();
		}

		public override void DrawTransform(Transform xf)
		{
			Vec2 p1 = xf.Position, p2;
			const float k_axisScale = 0.4f;
			Gl.glBegin(Gl.GL_LINES);

			Gl.glColor3f(1.0f, 0.0f, 0.0f);
			Gl.glVertex2f(p1.X, p1.Y);
			p2 = p1 + k_axisScale * xf.R.Col1;
			Gl.glVertex2f(p2.X, p2.Y);

			Gl.glColor3f(0.0f, 1.0f, 0.0f);
			Gl.glVertex2f(p1.X, p1.Y);
			p2 = p1 + k_axisScale * xf.R.Col2;
			Gl.glVertex2f(p2.X, p2.Y);

			Gl.glEnd();
		}

		public void DrawPoint(Vec2 p, float size, ColorF color)
		{
			Gl.glPointSize(size);
			Gl.glBegin(Gl.GL_POINTS);
			Gl.glColor3f(color.R, color.G, color.B);
			Gl.glVertex2f(p.X, p.Y);
			Gl.glEnd();
			Gl.glPointSize(1.0f);
		}

		public void DrawAABB(AABB aabb, ColorF c)
		{
			Gl.glColor3f(c.R, c.G, c.B);
			Gl.glBegin(Gl.GL_LINE_LOOP);
			Gl.glVertex2f(aabb.LowerBound.X, aabb.LowerBound.Y);
			Gl.glVertex2f(aabb.UpperBound.X, aabb.LowerBound.Y);
			Gl.glVertex2f(aabb.UpperBound.X, aabb.UpperBound.Y);
			Gl.glVertex2f(aabb.LowerBound.X, aabb.UpperBound.Y);
			Gl.glEnd();
		}

		public void DrawShape(FixtureDef fixture, Transform xf, ColorF color)
		{
			switch (fixture.Shape.ShapeType)
			{
			case ShapeType.Circle:
				{
					CircleShape circle = (CircleShape)fixture.Shape;

					Vec2 center = (xf * circle.Position);
					float radius = circle.Radius;
					Vec2 axis = xf.R.Col1;

					DrawSolidCircle(center, radius, axis, color);
				}
				break;

			case ShapeType.Polygon:
				{
					PolygonShape poly = (PolygonShape)fixture.Shape;
					int vertexCount = poly.VertexCount;
					//b2Assert(vertexCount <= b2_maxPolygonVertices);
					Vec2[] vertices = new Vec2[Box2DSettings.b2_maxPolygonVertices];

					for (int i = 0; i < vertexCount; ++i)
					{
						vertices[i] = (xf * poly.Vertices[i]);
					}

					DrawSolidPolygon(vertices, vertexCount, color);
				}
				break;
			}
		}

		public void DrawArc(Vec2 pos, float radius, float startAngle, float endAngle)
		{
			Gl.glPushMatrix();
			Gl.glTranslatef(pos.X, pos.Y, 0);
			var xx = Glu.gluNewQuadric();
			Glu.gluQuadricDrawStyle(xx, Glu.GLU_SILHOUETTE);
			Glu.gluPartialDisk(xx, radius, radius, 24, 1, b2Math.Rad2Deg(startAngle), b2Math.Rad2Deg(endAngle - startAngle));
			Glu.gluDeleteQuadric(xx);
			Gl.glPopMatrix();
		}

		public void DrawJoint(Box2CS.Serialize.JointDefSerialized x, Vec2 p1, Vec2 p2, BodyDef bodyA, BodyDef bodyB)
		{
			Transform xf1 = new Transform(bodyA.Position, new Mat22(bodyA.Angle));
			Transform xf2 = new Transform(bodyB.Position, new Mat22(bodyB.Angle));
			Vec2 x1 = xf1.Position;
			Vec2 x2 = xf2.Position;

			p1 = xf1 * p1;
			p2 = xf2 * p2;
	
			ColorF color = new ColorF(0.5f, 0.8f, 0.8f);

			switch (x.Joint.JointType)
			{
			case JointType.Distance:
				DrawSegment(p1, p2, color);
				break;

			case JointType.Pulley:
				{
					PulleyJointDef pulley = (PulleyJointDef)x.Joint;
					Vec2 s1 = pulley.GroundAnchorA;
					Vec2 s2 = pulley.GroundAnchorB;
					DrawSegment(s1, p1, color);
					DrawSegment(s2, p2, color);
					DrawSegment(s1, s2, color);
				}
				break;

			case JointType.Revolute:
				{
					RevoluteJointDef rjd = (RevoluteJointDef)x.Joint;

					if (rjd.EnableLimit)
					{
						Vec2 startPos = p1;
						Vec2 sinCos = new Vec2(-(float)Math.Cos((rjd.UpperAngle + rjd.ReferenceAngle) - (float)Math.PI / 2), -(float)Math.Sin((rjd.UpperAngle + rjd.ReferenceAngle) - (float)Math.PI / 2));

						var end = startPos + (sinCos * 3);
						DrawSegment(startPos, end, new ColorF(0, 0.65f, 0.65f));

						sinCos = new Vec2(-(float)Math.Cos((rjd.LowerAngle + rjd.ReferenceAngle) - (float)Math.PI / 2), -(float)Math.Sin((rjd.LowerAngle + rjd.ReferenceAngle) - (float)Math.PI / 2));

						end = startPos + (sinCos * 3);
						DrawSegment(startPos, end, new ColorF(0, 0.65f, 0.65f));
						DrawArc(startPos, 3, (-rjd.LowerAngle - rjd.ReferenceAngle), (-rjd.UpperAngle - rjd.ReferenceAngle));
					}

					DrawCircle(p1, 0.75f, new ColorF(0, 0.65f, 0.65f));

					DrawSegment(x1, p1, color);
					DrawSegment(p1, p2, color);
					DrawSegment(x2, p2, color);
				}
				break;

			default:
			case JointType.Unknown:
				DrawSegment(x1, p1, color);
				DrawSegment(p1, p2, color);
				DrawSegment(x2, p2, color);
				break;
			}
		}
	}
}
