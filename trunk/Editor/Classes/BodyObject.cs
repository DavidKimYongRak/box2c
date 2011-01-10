using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Paril.Collections;
using Box2CS.Serialize;
using Box2CS;

namespace Editor
{
	public class BodyObject
	{
		public BodyDef Body
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		MassData _mass;
		public MassData Mass
		{
			get { return _mass; }
			set { _mass = value; }
		}

		bool _autoCalculate = true;

		public bool AutoMassRecalculate
		{
			get { return _autoCalculate; }
			set { _autoCalculate = value; }
		}

		NotifyList<FixtureDefSerialized> _fixtures = new NotifyList<FixtureDefSerialized>();
		public NotifyList<FixtureDefSerialized> Fixtures
		{
			get { return _fixtures; }
		}

		public IEnumerable<FixtureDef> OnlyFixtures
		{
			get
			{
				foreach (var x in _fixtures)
					yield return x.Fixture;
			}
		}

		void _fixtures_ObjectsRemoved(object sender, EventArgs e)
		{
			_mass = Body.ComputeMass(OnlyFixtures);
		}

		void _fixtures_ObjectsAdded(object sender, EventArgs e)
		{
			_mass = Body.ComputeMass(OnlyFixtures);
		}

		public BodyObject(WorldObject world, BodyDefSerialized x)
		{
			Body = x.Body;
			Name = x.Name;

			for (int i = 0; i < x.FixtureIDs.Count; ++i)
			{
				var fixture = world.Fixtures[x.FixtureIDs[i]];
				//fixture.Fixture.Shape = world.Shapes[world.Fixtures[x.FixtureIDs[i]].ShapeID].Shape;
				_fixtures.Add(fixture);
			}

			_mass = Body.ComputeMass(OnlyFixtures);

			_fixtures.ObjectsAdded += new EventHandler(_fixtures_ObjectsAdded);
			_fixtures.ObjectsRemoved += new EventHandler(_fixtures_ObjectsRemoved);
		}
	}
}
