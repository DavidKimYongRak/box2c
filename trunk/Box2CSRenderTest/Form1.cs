using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Collision;

using Tao.OpenGl;
using Tao.Platform.Windows;

namespace Box2DSharpRenderTest
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		class GDIDebugThing : b2DebugDraw
		{
			public static GDIDebugThing instance = new GDIDebugThing();

			public override void DrawPolygon(b2Vec2[] vertices, int vertexCount, b2Color color)
			{
				Gl.glColor3f(color.r, color.g, color.b);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].x, vertices[i].y);
				}
				Gl.glEnd();
			}

			public override void DrawSolidPolygon(b2Vec2[] vertices, int vertexCount, b2Color color)
			{
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
				Gl.glColor4f(0.5f * color.r, 0.5f * color.g, 0.5f * color.b, 0.5f);
				Gl.glBegin(Gl.GL_TRIANGLE_FAN);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].x, vertices[i].y);
				}
				Gl.glEnd();
				Gl.glDisable(Gl.GL_BLEND);

				Gl.glColor4f(color.r, color.g, color.b, 1.0f);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < vertexCount; ++i)
				{
					Gl.glVertex2f(vertices[i].x, vertices[i].y);
				}
				Gl.glEnd();
			}

			public override void DrawCircle(b2Vec2 center, float radius, b2Color color)
			{
				const float k_segments = 16.0f;
				const float k_increment = (float)(2.0f * System.Math.PI / k_segments);
				float theta = 0.0f;
				Gl.glColor3f(color.r, color.g, color.b);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < k_segments; ++i)
				{
					b2Vec2 v = center + radius * new b2Vec2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.x, v.y);
					theta += k_increment;
				}
				Gl.glEnd();
			}

			public override void DrawSolidCircle(b2Vec2 center, float radius, b2Vec2 axis, b2Color color)
			{
				const float k_segments = 16.0f;
				const float k_increment = (float)(2.0f * System.Math.PI / k_segments);
				float theta = 0.0f;
				Gl.glEnable(Gl.GL_BLEND);
				Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
				Gl.glColor4f(0.5f * color.r, 0.5f * color.g, 0.5f * color.b, 0.5f);
				Gl.glBegin(Gl.GL_TRIANGLE_FAN);
				for (int i = 0; i < k_segments; ++i)
				{
					b2Vec2 v = center + radius * new b2Vec2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.x, v.y);
					theta += k_increment;
				}
				Gl.glEnd();
				Gl.glDisable(Gl.GL_BLEND);

				theta = 0.0f;
				Gl.glColor4f(color.r, color.g, color.b, 1.0f);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				for (int i = 0; i < k_segments; ++i)
				{
					b2Vec2 v = center + radius * new b2Vec2((float)System.Math.Cos(theta), (float)System.Math.Sin(theta));
					Gl.glVertex2f(v.x, v.y);
					theta += k_increment;
				}
				Gl.glEnd();

				b2Vec2 p = center + radius * axis;
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(center.x, center.y);
				Gl.glVertex2f(p.x, p.y);
				Gl.glEnd();
			}

			public override void DrawSegment(b2Vec2 p1, b2Vec2 p2, b2Color color)
			{
				Gl.glColor3f(color.r, color.g, color.b);
				Gl.glBegin(Gl.GL_LINES);
				Gl.glVertex2f(p1.x, p1.y);
				Gl.glVertex2f(p2.x, p2.y);
				Gl.glEnd();
			}

			public override void DrawTransform(b2Transform xf)
			{
				b2Vec2 p1 = xf.position, p2;
				const float k_axisScale = 0.4f;
				Gl.glBegin(Gl.GL_LINES);
	
				Gl.glColor3f(1.0f, 0.0f, 0.0f);
				Gl.glVertex2f(p1.x, p1.y);
				p2 = p1 + k_axisScale * xf.R.col1;
				Gl.glVertex2f(p2.x, p2.y);

				Gl.glColor3f(0.0f, 1.0f, 0.0f);
				Gl.glVertex2f(p1.x, p1.y);
				p2 = p1 + k_axisScale * xf.R.col2;
				Gl.glVertex2f(p2.x, p2.y);

				Gl.glEnd();
			}

			public void DrawPoint(b2Vec2 p, float size, b2Color color)
			{
				Gl.glPointSize(size);
				Gl.glBegin(Gl.GL_POINTS);
				Gl.glColor3f(color.r, color.g, color.b);
				Gl.glVertex2f(p.x, p.y);
				Gl.glEnd();
				Gl.glPointSize(1.0f);
			}

			public void DrawAABB(b2AABB aabb, b2Color c)
			{
				Gl.glColor3f(c.r, c.g, c.b);
				Gl.glBegin(Gl.GL_LINE_LOOP);
				Gl.glVertex2f(aabb.lowerBound.x, aabb.lowerBound.y);
				Gl.glVertex2f(aabb.upperBound.x, aabb.lowerBound.y);
				Gl.glVertex2f(aabb.upperBound.x, aabb.upperBound.y);
				Gl.glVertex2f(aabb.lowerBound.x, aabb.upperBound.y);
				Gl.glEnd();
			}
		}

		b2Body groundBody;
		b2World world;

		void MakeBox(b2Vec2 pos, float w, float h)
		{
			// Define the dynamic body. We set its position and call the body factory.
			b2BodyDef bodyDef = new b2BodyDef();
			bodyDef.type = b2BodyType.b2_dynamicBody;
			bodyDef.position = pos;
			var body = world.CreateBody(bodyDef);

			// Define another box shape for our dynamic body.
			b2PolygonShape dynamicBox = new b2PolygonShape();
			dynamicBox.SetAsBox(w, h);

			// Define the dynamic body fixture.
			b2FixtureDef fixtureDef = new b2FixtureDef();
			fixtureDef.shape = dynamicBox;

			// Set the box density to be non-zero, so it will be dynamic.
			fixtureDef.density = 1.0f;

			// Override the default friction.
			fixtureDef.friction = 0.3f;

			// Add the shape to the body.
			body.CreateFixture(fixtureDef);
		}

		void MakeCircle(b2Vec2 pos, float radius)
		{
			// Define the dynamic body. We set its position and call the body factory.
			b2BodyDef bodyDef = new b2BodyDef();
			bodyDef.type = b2BodyType.b2_dynamicBody;
			bodyDef.position = pos;
			var body = world.CreateBody(bodyDef);

			// Define another box shape for our dynamic body.
			b2CircleShape dynamicBox = new b2CircleShape();
			dynamicBox.m_radius = radius;

			// Define the dynamic body fixture.
			b2FixtureDef fixtureDef = new b2FixtureDef();
			fixtureDef.shape = dynamicBox;

			// Set the box density to be non-zero, so it will be dynamic.
			fixtureDef.density = 1.0f;

			// Override the default friction.
			fixtureDef.friction = 0.3f;

			// Add the shape to the body.
			body.CreateFixture(fixtureDef);
		}

		SimpleOpenGlControl sogc;
		private void Form1_Load(object sender, EventArgs e)
		{
			sogc = new SimpleOpenGlControl();
			sogc.Dock = DockStyle.Fill;
			sogc.Paint += new PaintEventHandler(sogc_Paint);
			sogc.Resize += new EventHandler(sogc_Resize);
			Controls.Add(sogc);

			sogc.InitializeContexts();
			InitOpenGL(sogc.Size, 1, PointF.Empty);

			// Define the gravity vector.
			b2Vec2 gravity = new b2Vec2(0.0f, -10.0f);

			// Do we want to let bodies sleep?
			bool doSleep = true;

			// Construct a world object, which will hold and simulate the rigid bodies.
			world = new b2World(gravity, doSleep);
		//	world.SetWarmStarting(true);

			{
				b2BodyDef bd = new b2BodyDef();
				b2Body ground = world.CreateBody(bd);

				b2PolygonShape shape = new b2PolygonShape();
				shape.SetAsEdge(new b2Vec2(-40.0f, 0.0f), new b2Vec2(40.0f, 0.0f));
				ground.CreateFixture(shape, 0.0f);
			}

			{
				float a = 0.5f;
				b2CircleShape shape = new b2CircleShape();
				shape.m_radius = a;

				b2Vec2 x = new b2Vec2(-7.0f, 0.95f);
				b2Vec2 y;
				b2Vec2 deltaX = new b2Vec2(0, 1.25f);
				b2Vec2 deltaY = new b2Vec2(0, 1.25f);
				y= deltaY;

				for (int j = 0; j < 8; ++j)
				{
					b2BodyDef bd = new b2BodyDef();
					bd.type = b2BodyType.b2_dynamicBody;
					bd.position = y;
					b2Body body = world.CreateBody(bd);
					body.CreateFixture(shape, 5.0f);

					y += deltaY;
				}
			}

			GDIDebugThing.instance.SetFlags(EDebugFlags.e_shapeBit);
			world.SetDebugDraw(GDIDebugThing.instance);

			System.Timers.Timer timer = new System.Timers.Timer();
			timer.Interval = 85;
			timer.SynchronizingObject = this;
			timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
			timer.Start();
		}

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			// Prepare for simulation. Typically we use a time step of 1/60 of a
			// second (60Hz) and 10 iterations. This provides a high quality simulation
			// in most game scenarios.
			float timeStep = 1.0f / 60.0f;
			int velocityIterations = 8;
			int positionIterations = 4;

			// This is our little game loop.
			// Instruct the world to perform a single step of simulation.
			// It is generally best to keep the time step and iterations fixed.
			world.Step(timeStep, velocityIterations, positionIterations);

			sogc.Invalidate();
		}

		void sogc_Resize(object sender, EventArgs e)
		{
			InitOpenGL(sogc.Size, 1, PointF.Empty);
			sogc.Invalidate();
		}

		void sogc_Paint(object sender, PaintEventArgs e)
		{
			OpenGLDraw();
		}

		PointF offset;
		float zoom = 1;
		public void InitOpenGL(Size WidthHeight, float viewZoom, PointF ViewOffset)
		{
			offset = ViewOffset;
			zoom = viewZoom;

			int Width = WidthHeight.Width;
			int Height = WidthHeight.Height;
			Height = Height > 0 ? Height : 1;

			Gl.glDisable(Gl.GL_CULL_FACE);
			Gl.glClearColor(.5f, .5f, .5f, 1);

			float AspectRatio = (float)Width / (float)Height;
			Gl.glViewport(0, 0, Width, Height);
			Gl.glMatrixMode(Gl.GL_PROJECTION);

			Gl.glLoadIdentity();
			Gl.glOrtho(0, Width, Height, 0, -1, 1);

			Gl.glMatrixMode(Gl.GL_MODELVIEW);
		}

		public void OpenGLDraw()
		{
			Gl.glClear(Gl.GL_COLOR_BUFFER_BIT);
			Gl.glMatrixMode(Gl.GL_MODELVIEW);
			Gl.glLoadIdentity();

			Gl.glPushMatrix();

			Gl.glTranslatef(sogc.Width / 2, sogc.Height / 2, 0);
			Gl.glScalef(14, -14, 14);

			world.DrawDebugData();

			Gl.glPopMatrix();
		}
	}
}
