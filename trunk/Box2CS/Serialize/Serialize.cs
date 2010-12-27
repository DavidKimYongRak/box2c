using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Box2CS.Serialize
{
	public class FixtureDefSerialized
	{
		public FixtureDef Fixture
		{
			get;
			set;
		}

		public int ShapeID
		{
			get;
			set;
		}

		public FixtureDefSerialized(FixtureDef fixture, int shapeID)
		{
			Fixture = fixture;
			ShapeID = shapeID;
		}
	}

	public class BodyDefSerialized
	{
		public BodyDef Body
		{
			get;
			set;
		}

		public List<int> FixtureIDs
		{
			get;
			set;
		}

		public BodyDefSerialized(BodyDef body, List<int> fixtureIDs)
		{
			Body = body;
			FixtureIDs = fixtureIDs;
		}
	}

	public interface IWorldSerializer
	{
		void Open(Stream stream);
		void Close();

		void BeginSerializingShapes();
		void EndSerializingShapes();

		void BeginSerializingFixtures();
		void EndSerializingFixtures();

		void BeginSerializingBodies();
		void EndSerializingBodies();

		void SerializeShape(Shape shape);
		void SerializeFixture(FixtureDefSerialized fixture);
		void SerializeBody(BodyDefSerialized body);
	}

	public interface IWorldDeserializer
	{
		IList<Shape> Shapes
		{
			get;
		}

		IList<FixtureDef> FixtureDefs
		{
			get;
		}

		IList<BodyDefSerialized> Bodies
		{
			get;
		}

		void Deserialize(Stream stream);
	}

	public class WorldXmlSerializer : IWorldSerializer
	{
		XmlWriter writer;

		void WriteEndElement()
		{
			writer.WriteEndElement();
		}

		void WriteSimpleType(Type type, object val)
		{
			var serializer = new XmlSerializerFactory().CreateSerializer(type);
			XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
			xmlnsEmpty.Add("", "");
			serializer.Serialize(writer, val, xmlnsEmpty);
		}

		void WriteDynamicType(Type type, object val)
		{
			writer.WriteElementString("Type", type.FullName);

			writer.WriteStartElement("Value");
			var serializer = new XmlSerializerFactory().CreateSerializer(type);
			XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
			xmlnsEmpty.Add("", "");
			serializer.Serialize(writer, val, xmlnsEmpty);
			writer.WriteEndElement();
		}

		void WriteVector(string name, Vec2 vec)
		{
			writer.WriteStartElement(name);
			writer.WriteAttributeString("X", vec.X.ToString());
			writer.WriteAttributeString("Y", vec.Y.ToString());
			WriteEndElement();
		}

		public void Open(Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			settings.OmitXmlDeclaration = true;

			writer = XmlWriter.Create(stream, settings);

			writer.WriteStartElement("World");
		}

		public void Close()
		{
			WriteEndElement();

			writer.Flush();
			writer.Close();
			writer = null;
		}

		public void BeginSerializingShapes()
		{
			writer.WriteStartElement("Shapes");
		}

		public void EndSerializingShapes()
		{
			WriteEndElement();
		}

		public void BeginSerializingFixtures()
		{
			writer.WriteStartElement("Fixtures");
		}

		public void EndSerializingFixtures()
		{
			WriteEndElement();
		}

		public void BeginSerializingBodies()
		{
			writer.WriteStartElement("Bodies");
		}

		public void EndSerializingBodies()
		{
			WriteEndElement();
		}

		public void SerializeShape(Shape shape)
		{
			writer.WriteStartElement("Shape");
			writer.WriteAttributeString("Type", shape.ShapeType.ToString());

			switch (shape.ShapeType)
			{
			case ShapeType.Circle:
				{
					CircleShape circle = (CircleShape)shape;

					writer.WriteElementString("Radius", circle.Radius.ToString());

					WriteVector("Position", circle.Position);
				}
				break;
			case ShapeType.Polygon:
				{
					PolygonShape poly = (PolygonShape)shape;

					writer.WriteStartElement("Vertices");
					foreach (var v in poly.Vertices)
						WriteVector("Vertex", v);
					WriteEndElement();

					WriteVector("Centroid", poly.Centroid);
				}
				break;
			default:
				throw new Exception();
			}

			WriteEndElement();
		}

		static FixtureDef defaultFixtureDefData = new FixtureDef();

		public void SerializeFixture(FixtureDefSerialized fixture)
		{
			writer.WriteStartElement("Fixture");

			writer.WriteElementString("Shape", fixture.ShapeID.ToString());

			if (fixture.Fixture.Density != defaultFixtureDefData.Density)
				writer.WriteElementString("Density", fixture.Fixture.Density.ToString());

			if (fixture.Fixture.Filter != defaultFixtureDefData.Filter)
				WriteSimpleType(typeof(FilterData), fixture.Fixture.Filter);

			if (fixture.Fixture.Friction != defaultFixtureDefData.Friction)
				writer.WriteElementString("Friction", fixture.Fixture.Friction.ToString());

			if (fixture.Fixture.IsSensor != defaultFixtureDefData.IsSensor)
				writer.WriteElementString("IsSensor", fixture.Fixture.IsSensor.ToString());

			if (fixture.Fixture.Restitution != defaultFixtureDefData.Restitution)
				writer.WriteElementString("Restitution", fixture.Fixture.Restitution.ToString());

			if (fixture.Fixture.UserData != null)
			{
				writer.WriteStartElement("UserData");
				WriteDynamicType(fixture.Fixture.UserData.GetType(), fixture.Fixture.UserData);
				WriteEndElement();
			}

			WriteEndElement();
		}

		static BodyDef defaultBodyDefData = new BodyDef();

		public void SerializeBody(BodyDefSerialized body)
		{
			writer.WriteStartElement("Body");
			writer.WriteAttributeString("Type", body.Body.BodyType.ToString());

			if (body.Body.Active != defaultBodyDefData.Active)
				writer.WriteElementString("Active", body.Body.Active.ToString());

			if (body.Body.AllowSleep != defaultBodyDefData.AllowSleep)
				writer.WriteElementString("AllowSleep", body.Body.AllowSleep.ToString());

			if (body.Body.Angle != defaultBodyDefData.Angle)
				writer.WriteElementString("Angle", body.Body.Angle.ToString());

			if (body.Body.AngularDamping != defaultBodyDefData.AngularDamping)
				writer.WriteElementString("AngularDamping", body.Body.AngularDamping.ToString());

			if (body.Body.AngularVelocity != defaultBodyDefData.AngularVelocity)
				writer.WriteElementString("AngularVelocity", body.Body.AngularVelocity.ToString());

			if (body.Body.Awake != defaultBodyDefData.Awake)
				writer.WriteElementString("Awake", body.Body.Awake.ToString());

			if (body.Body.Bullet != defaultBodyDefData.Bullet)
				writer.WriteElementString("Bullet", body.Body.Bullet.ToString());

			if (body.Body.FixedRotation != defaultBodyDefData.FixedRotation)
				writer.WriteElementString("FixedRotation", body.Body.FixedRotation.ToString());

			if (body.Body.InertiaScale != defaultBodyDefData.InertiaScale)
				writer.WriteElementString("InertiaScale", body.Body.InertiaScale.ToString());

			if (body.Body.LinearDamping != defaultBodyDefData.LinearDamping)
				writer.WriteElementString("LinearDamping", body.Body.LinearDamping.ToString());

			if (body.Body.LinearVelocity != defaultBodyDefData.LinearVelocity)
				WriteVector("LinearVelocity", body.Body.LinearVelocity);

			if (body.Body.Position != defaultBodyDefData.Position)
				WriteVector("Position", body.Body.Position);

			if (body.Body.UserData != null)
			{
				writer.WriteStartElement("UserData");
				WriteDynamicType(body.Body.UserData.GetType(), body.Body.UserData);
				WriteEndElement();
			}

			writer.WriteStartElement("Fixtures");
			foreach (var fixture in body.FixtureIDs)
				writer.WriteElementString("ID", fixture.ToString());
			WriteEndElement();

			WriteEndElement();
		}
	}

	public class WorldXmlDeserializer : IWorldDeserializer
	{
		List<Shape> _shapes = new List<Shape>();
		List<FixtureDef> _fixtures = new List<FixtureDef>();
		List<BodyDefSerialized> _bodies = new List<BodyDefSerialized>();

		public IList<Shape> Shapes
		{
			get { return _shapes; }
		}

		public IList<FixtureDef> FixtureDefs
		{
			get { return _fixtures; }
		}

		public IList<BodyDefSerialized> Bodies
		{
			get { return _bodies; }
		}

		//XmlReader reader;

		Vec2 ReadVector(XmlNode node)
		{
			return new Vec2(
							float.Parse(node.Attributes["X"].Value),
							float.Parse(node.Attributes["Y"].Value));
		}

		object ReadSimpleType(XmlNode node, Type type, bool outer)
		{
			try
			{
				if (type == null)
					return ReadSimpleType(node.LastChild, Type.GetType(node.FirstChild.FirstChild.Value), outer);

				var serializer = new XmlSerializer(type);
				XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
				xmlnsEmpty.Add("", "");

				using (MemoryStream stream = new MemoryStream())
				{
					StreamWriter writer = new StreamWriter(stream);
					{
						writer.Write((outer) ? node.OuterXml : node.InnerXml);
						writer.Flush();
						stream.Position = 0;
					}
					XmlReaderSettings settings = new XmlReaderSettings();
					settings.ConformanceLevel = ConformanceLevel.Fragment;

					return serializer.Deserialize(XmlReader.Create(stream, settings));
				}
			}
			catch (Exception e)
			{
				e = e;
				return null;
			}
		}

		public void Deserialize(Stream stream)
		{
			XmlDocument document = new XmlDocument();
			document.Load(stream);

			if (document.FirstChild.Name.ToLower() != "world")
				throw new Exception();

			foreach (XmlNode main in document.FirstChild)
			{
				switch (main.Name.ToLower())
				{
				case "shapes":
					{
						foreach (XmlNode n in main)
						{
							if (n.Name.ToLower() != "shape")
								throw new Exception();

							ShapeType type = (ShapeType)Enum.Parse(typeof(ShapeType), n.Attributes[0].Value, true);

							switch (type)
							{
							case ShapeType.Circle:
								{
									CircleShape shape = new CircleShape();

									foreach (XmlNode sn in n)
									{
										switch (sn.Name.ToLower())
										{
										case "radius":
											shape.Radius = float.Parse(sn.FirstChild.Value);
											break;
										case "position":
											shape.Position = ReadVector(sn);
											break;
										default:
											throw new Exception();
										}
									}

									_shapes.Add(shape);
								}
								break;
							case ShapeType.Polygon:
								{
									PolygonShape shape = new PolygonShape();

									foreach (XmlNode sn in n)
									{
										switch (sn.Name.ToLower())
										{
										case "vertices":
											{
												List<Vec2> verts = new List<Vec2>();

												foreach (XmlNode vert in sn)
													verts.Add(ReadVector(vert));

												shape.Vertices = verts.ToArray();
											}
											break;
										case "centroid":
											shape.Centroid = ReadVector(sn);
											break;
										}
									}

									_shapes.Add(shape);
								}
								break;
							}
						}
					}
					break;
				case "fixtures":
					{
						foreach (XmlNode n in main)
						{
							FixtureDef fixture = new FixtureDef();

							if (n.Name.ToLower() != "fixture")
								throw new Exception();

							foreach (XmlNode sn in n)
							{
								switch (sn.Name.ToLower())
								{
								case "shape":
									fixture.Shape = _shapes[int.Parse(sn.FirstChild.Value)]; // FIXME ordering
									break;
								case "density":
									fixture.Density = float.Parse(sn.FirstChild.Value);
									break;
								case "filterdata":
									fixture.Filter = (FilterData)ReadSimpleType(sn, typeof(FilterData), true);
									break;
								case "friction":
									fixture.Friction = float.Parse(sn.FirstChild.Value);
									break;
								case "issensor":
									fixture.IsSensor = bool.Parse(sn.FirstChild.Value);
									break;
								case "restitution":
									fixture.Restitution = float.Parse(sn.FirstChild.Value);
									break;
								case "userdata":
									fixture.UserData = ReadSimpleType(sn, null, false);
									break;
								}
							}

							_fixtures.Add(fixture);
						}
					}
					break;
				case "bodies":
					{
						foreach (XmlNode n in main)
						{
							BodyDef body = new BodyDef();

							if (n.Name.ToLower() != "body")
								throw new Exception();

							body.BodyType = (BodyType)Enum.Parse(typeof(BodyType), n.Attributes[0].Value, true);
							List<int> fixtures = new List<int>();

							foreach (XmlNode sn in n)
							{
								switch (sn.Name.ToLower())
								{
								case "active":
									body.Active = bool.Parse(sn.FirstChild.Value);
									break;
								case "allowsleep":
									body.AllowSleep = bool.Parse(sn.FirstChild.Value);
									break;
								case "angle":
									body.Angle = float.Parse(sn.FirstChild.Value);
									break;
								case "angulardamping":
									body.AngularDamping = float.Parse(sn.FirstChild.Value);
									break;
								case "angularvelocity":
									body.AngularVelocity = float.Parse(sn.FirstChild.Value);
									break;
								case "awake":
									body.Awake = bool.Parse(sn.FirstChild.Value);
									break;
								case "bullet":
									body.Bullet = bool.Parse(sn.FirstChild.Value);
									break;
								case "fixedrotation":
									body.FixedRotation = bool.Parse(sn.FirstChild.Value);
									break;
								case "inertiascale":
									body.InertiaScale = float.Parse(sn.FirstChild.Value);
									break;
								case "lineardamping":
									body.LinearDamping = float.Parse(sn.FirstChild.Value);
									break;
								case "linearvelocity":
									body.LinearVelocity = ReadVector(sn);
									break;
								case "position":
									body.Position = ReadVector(sn);
									break;
								case "userdata":
									body.UserData = ReadSimpleType(sn, null, false);
									break;
								case "fixtures":
									{
										foreach (XmlNode v in sn)
											fixtures.Add(int.Parse(v.FirstChild.Value));
										break;
									}
								}
							}

							_bodies.Add(new BodyDefSerialized(body, fixtures));
						}
					}
					break;
				}
			}
		}
	}

	public class WorldSerializer
	{
		IWorldSerializer _serializer;

		List<Shape> _shapeDefinitions = new List<Shape>();
		List<FixtureDefSerialized> _fixtureDefinitions = new List<FixtureDefSerialized>();
		List<BodyDefSerialized> _bodyDefinitions = new List<BodyDefSerialized>();

		public WorldSerializer(IWorldSerializer serializer)
		{
			_serializer = serializer;
		}

		protected int FixtureIDFromFixture(FixtureDef def)
		{
			for (int i = 0; i < _fixtureDefinitions.Count; ++i)
			{
				if (_fixtureDefinitions[i].Fixture.CompareWith(def))
					return i;
			}

			throw new KeyNotFoundException();
		}

		public static WorldSerializer SerializeWorld(World world, IWorldSerializer serializer)
		{
			WorldSerializer worldSerializer = new WorldSerializer(serializer);

			foreach (var body in world.Bodies)
			{
				BodyDef def = new BodyDef();

				def.Active = body.IsActive;
				def.AllowSleep = body.IsSleepingAllowed;
				def.Angle = body.Angle;
				def.AngularDamping = body.AngularDamping;
				def.AngularVelocity = body.AngularVelocity;
				def.Awake = body.IsAwake;
				def.BodyType = body.BodyType;
				def.Bullet = body.IsBullet;
				def.FixedRotation = body.IsFixedRotation;
				//def.InertiaScale
				def.LinearDamping = body.LinearDamping;
				def.LinearVelocity = body.LinearVelocity;
				def.Position = body.Position;
				def.UserData = body.UserData;

				List<FixtureDef> fixtures = new List<FixtureDef>();

				foreach (var f in body.Fixtures)
				{
					FixtureDef fixDef = new FixtureDef();
					fixDef.Density = f.Density;
					fixDef.Filter = f.FilterData;
					fixDef.Friction = f.Friction;
					fixDef.IsSensor = f.IsSensor;
					fixDef.Restitution = f.Restitution;
					fixDef.Shape = f.Shape;
					fixDef.UserData = f.UserData;

					fixtures.Add(fixDef);
				}

				worldSerializer.AddBody(def, fixtures);
			}

			return worldSerializer;
		}

		public void AddShape(Shape shape)
		{
			foreach (var s in _shapeDefinitions)
			{
				// shape already exists
				if (s.CompareWith(shape))
					return;
			}

			_shapeDefinitions.Add(shape);
		}

		public void AddFixture(FixtureDef fixture)
		{
			bool containsShape = false;
			int shapeID = -1;

			// see if we need to add the shape
			for (int i = 0; i < _shapeDefinitions.Count; ++i)
			{
				if (fixture.Shape == _shapeDefinitions[i])
				{
					containsShape = true;
					shapeID = i;
					break;
				}
			}

			// No, so let's add it
			if (!containsShape)
			{
				AddShape(fixture.Shape);
				shapeID = _shapeDefinitions.Count - 1;
			}

			_fixtureDefinitions.Add(new FixtureDefSerialized(fixture, shapeID));
		}

		public void AddBody(BodyDef body, List<FixtureDef> fixtures)
		{
			List<int> fixtureIDs = new List<int>();

			// See if we need to add any of the fixtures
			foreach (var f in fixtures)
			{
				try
				{
					fixtureIDs.Add(FixtureIDFromFixture(f));
				}
				catch (KeyNotFoundException)
				{
					AddFixture(f);
					fixtureIDs.Add(_fixtureDefinitions.Count - 1);
				}
			}

			_bodyDefinitions.Add(new BodyDefSerialized(body, fixtureIDs));
		}

		public void Serialize(Stream stream)
		{
			_serializer.Open(stream);

			_serializer.BeginSerializingShapes();
			foreach (var s in _shapeDefinitions)
				_serializer.SerializeShape(s);
			_serializer.EndSerializingShapes();

			_serializer.BeginSerializingFixtures();
			foreach (var f in _fixtureDefinitions)
				_serializer.SerializeFixture(f);
			_serializer.EndSerializingFixtures();

			_serializer.BeginSerializingBodies();
			foreach (var b in _bodyDefinitions)
				_serializer.SerializeBody(b);
			_serializer.EndSerializingBodies();

			_serializer.Close();
		}
	}

	public class Serializer
	{
		List<BodyDef> _bodyDefinitions = new List<BodyDef>();
		List<Shape> _shapeDefinitions = new List<Shape>();
		List<FixtureDef> _fixtureDefinitions = new List<FixtureDef>();

		public List<BodyDef> BodyDefs
		{
			get { return _bodyDefinitions; }
		}

		public List<Shape> Shapes
		{
			get { return _shapeDefinitions; }
		}

		public List<FixtureDef> FixtureDefs
		{
			get { return _fixtureDefinitions; }
		}

		public Serializer()
		{
		}

		public void Save(string fileName)
		{
			try
			{
				using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
				{
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;

					using (XmlWriter writer = XmlWriter.Create(fs, settings))
					{
						writer.WriteStartElement("Bodies");

						foreach (var b in _bodyDefinitions)
						{
							writer.WriteStartElement("BodyDef");

							writer.WriteElementString("BodyType", b.BodyType.ToString());

							writer.WriteEndElement();
						}

						writer.WriteEndElement();
					}
				}
			}
			catch (Exception ex)
			{
				fileName = ex.Message;
			}
		}
	}
}
