using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2CS;
using Box2CS.Serialize;

namespace Editor
{
	public class WorldObject
	{
		List<BodyNode> _bodies = new List<BodyNode>();
		List<FixtureNode> _fixtures = new List<FixtureNode>();
		//List<JointDefSerialized> _joints = new List<JointDefSerialized>();

		public List<BodyNode> Bodies
		{
			get { return _bodies; }
		}

		public List<FixtureNode> Fixtures
		{
			get { return _fixtures; }
		}

		//public List<JointDefSerialized> Joints
		//{
		//	get { return _joints; }
		//}

		public void Clear()
		{
			_bodies.Clear();
			_fixtures.Clear();
		}

		public void LoadFromFile(string fileName)
		{
			var deserializer = new WorldXmlDeserializer();
			using (System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
				deserializer.Deserialize(fs);

			for (int i = 0; i < deserializer.Shapes.Count; ++i)
			{
				var x = deserializer.Shapes[i];

				if (string.IsNullOrEmpty(x.Name))
					x.Name = "Shape "+i.ToString();

				//_shapes.Add(x);
			}

			for (int i = 0; i < deserializer.FixtureDefs.Count; ++i)
			{
				var x = deserializer.FixtureDefs[i];

				if (string.IsNullOrEmpty(x.Name))
					x.Name = "Fixture "+i.ToString();

				//_fixtures.Add(x);
			}

			for (int i = 0; i < deserializer.Bodies.Count; ++i)
			{
				var x = deserializer.Bodies[i];

				if (string.IsNullOrEmpty(x.Name))
					x.Name = "Body "+i.ToString();

				//_bodies.Add(new BodyObject(this, x));
			}

			for (int i = 0; i < deserializer.Joints.Count; ++i)
			{
				var x = deserializer.Joints[i];

				if (string.IsNullOrEmpty(x.Name))
					x.Name = "Joint "+i.ToString();

				//_joints.Add(x);
			}
		}
	}
}
