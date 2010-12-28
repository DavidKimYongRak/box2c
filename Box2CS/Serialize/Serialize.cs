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

		public Body DerivedBody
		{
			get;
			set;
		}

		public BodyDefSerialized(Body derivedBody, BodyDef body, List<int> fixtureIDs)
		{
			DerivedBody = derivedBody;
			Body = body;
			FixtureIDs = fixtureIDs;
		}
	}

	public class JointDefSerialized
	{
		public JointDef Joint
		{
			get;
			set;
		}

		public int BodyAIndex
		{
			get;
			set;
		}

		public int BodyBIndex
		{
			get;
			set;
		}

		public Joint DerivedJoint
		{
			get;
			set;
		}

		public JointDefSerialized(Joint derivedJoint, JointDef joint, int bodyA, int bodyB)
		{
			DerivedJoint = derivedJoint;
			Joint = joint;
			BodyAIndex = bodyA;
			BodyBIndex = bodyB;
		}
	}

	public interface IWorldSerializer
	{
		void Open(Stream stream, IWorldSerializationProvider provider);
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

		void BeginSerializingJoints();
		void EndSerializingJoints();

		void SerializeJoint(JointDefSerialized joint);
	}

	public interface IWorldSerializationProvider
	{
		IList<Shape> Shapes
		{
			get;
		}

		IList<FixtureDefSerialized> FixtureDefs
		{
			get;
		}

		int IndexOfFixture(FixtureDef def);

		IList<BodyDefSerialized> Bodies
		{
			get;
		}

		int IndexOfBody(BodyDef def);

		IList<JointDefSerialized> Joints
		{
			get;
		}
	}

	public interface IWorldDeserializer
	{
		void Deserialize(Stream stream);
	}

	public class WorldXmlSerializer : IWorldSerializer
	{
		XmlWriter writer;
		IWorldSerializationProvider _provider;

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

		void WriteElement(string name, int val)
		{
			writer.WriteElementString(name, val.ToString());
		}

		void WriteElement(string name, float val)
		{
			writer.WriteElementString(name, val.ToString());
		}

		void WriteElement(string name, double val)
		{
			writer.WriteElementString(name, val.ToString());
		}

		void WriteElement(string name, bool val)
		{
			writer.WriteElementString(name, val.ToString());
		}

		void WriteElement(string name, Vec2 vec)
		{
			writer.WriteStartElement(name);
			writer.WriteAttributeString("X", vec.X.ToString());
			writer.WriteAttributeString("Y", vec.Y.ToString());
			WriteEndElement();
		}

		public void Open(Stream stream, IWorldSerializationProvider provider)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			settings.OmitXmlDeclaration = true;

			writer = XmlWriter.Create(stream, settings);

			writer.WriteStartElement("World");

			_provider = provider;
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

					WriteElement("Position", circle.Position);
				}
				break;
			case ShapeType.Polygon:
				{
					PolygonShape poly = (PolygonShape)shape;

					writer.WriteStartElement("Vertices");
					foreach (var v in poly.Vertices)
						WriteElement("Vertex", v);
					WriteEndElement();

					WriteElement("Centroid", poly.Centroid);
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
				WriteElement("LinearVelocity", body.Body.LinearVelocity);

			if (body.Body.Position != defaultBodyDefData.Position)
				WriteElement("Position", body.Body.Position);

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

		public void BeginSerializingJoints()
		{
			writer.WriteStartElement("Joints");
		}

		public void EndSerializingJoints()
		{
			writer.WriteEndElement();
		}

		public void SerializeJoint(JointDefSerialized def)
		{
			writer.WriteStartElement("Joint");

			writer.WriteAttributeString("Type", def.Joint.JointType.ToString());

			WriteElement("BodyA", def.BodyAIndex);
			WriteElement("BodyB", def.BodyBIndex);

			WriteElement("CollideConnected", def.Joint.CollideConnected);

			if (def.Joint.UserData != null)
			{
				writer.WriteStartElement("UserData");
				WriteDynamicType(def.Joint.UserData.GetType(), def.Joint.UserData);
				WriteEndElement();
			}

			switch (def.Joint.JointType)
			{
			case JointType.Distance:
				{
					DistanceJointDef djd = (DistanceJointDef)def.Joint;

					WriteElement("DampingRatio", djd.DampingRatio);
					WriteElement("FrequencyHz", djd.FrequencyHz);
					WriteElement("Length", djd.Length);
					WriteElement("LocalAnchorA", djd.LocalAnchorA);
					WriteElement("LocalAnchorB", djd.LocalAnchorB);
				}
				break;
			case JointType.Friction:
				{
					FrictionJointDef fjd = (FrictionJointDef)def.Joint;

					WriteElement("LocalAnchorA", fjd.LocalAnchorA);
					WriteElement("LocalAnchorB", fjd.LocalAnchorB);
					WriteElement("MaxForce", fjd.MaxForce);
					WriteElement("MaxTorque", fjd.MaxTorque);
				}
				break;
			case JointType.Gear:
				{
					GearJointDef gjd = (GearJointDef)def.Joint;

					int jointA = -1, jointB = -1;

					for (int i = 0; i < _provider.Joints.Count; ++i)
					{
						if (gjd.JointA == _provider.Joints[i].DerivedJoint)
							jointA = i;

						if (gjd.JointB == _provider.Joints[i].DerivedJoint)
							jointB = i;

						if (jointA != -1 && jointB != -1)
							break;
					}

					WriteElement("JointA", jointA);
					WriteElement("JointB", jointB);
					WriteElement("Ratio", gjd.Ratio);
				}
				break;
			case JointType.Line:
				{
					LineJointDef ljd = (LineJointDef)def.Joint;

					WriteElement("EnableLimit", ljd.EnableLimit);
					WriteElement("EnableMotor", ljd.EnableMotor);
					WriteElement("LocalAnchorA", ljd.LocalAnchorA);
					WriteElement("LocalAnchorB", ljd.LocalAnchorB);
					WriteElement("LocalAxisA", ljd.LocalAxisA);
					WriteElement("LowerTranslation", ljd.LowerTranslation);
					WriteElement("MaxMotorForce", ljd.MaxMotorForce);
					WriteElement("MotorSpeed", ljd.MotorSpeed);
					WriteElement("UpperTranslation", ljd.UpperTranslation);
				}
				break;
			case JointType.Prismatic:
				{
					PrismaticJointDef pjd = (PrismaticJointDef)def.Joint;

					WriteElement("EnableLimit", pjd.EnableLimit);
					WriteElement("EnableMotor", pjd.EnableMotor);
					WriteElement("LocalAnchorA", pjd.LocalAnchorA);
					WriteElement("LocalAnchorB", pjd.LocalAnchorB);
					WriteElement("LocalAxisA", pjd.LocalAxis);
					WriteElement("LowerTranslation", pjd.LowerTranslation);
					WriteElement("MaxMotorForce", pjd.MaxMotorForce);
					WriteElement("MotorSpeed", pjd.MotorSpeed);
					WriteElement("UpperTranslation", pjd.UpperTranslation);
					WriteElement("ReferenceAngle", pjd.ReferenceAngle);
				}
				break;
			case JointType.Pulley:
				{
					PulleyJointDef pjd = (PulleyJointDef)def.Joint;

					WriteElement("GroundAnchorA", pjd.GroundAnchorA);
					WriteElement("GroundAnchorB", pjd.GroundAnchorB);
					WriteElement("LengthA", pjd.LengthA);
					WriteElement("LengthB", pjd.LengthB);
					WriteElement("LocalAnchorA", pjd.LocalAnchorA);
					WriteElement("LocalAnchorB", pjd.LocalAnchorB);
					WriteElement("MaxLengthA", pjd.MaxLengthA);
					WriteElement("MaxLengthB", pjd.MaxLengthB);
					WriteElement("Ratio", pjd.Ratio);
				}
				break;
			case JointType.Revolute:
				{
					RevoluteJointDef rjd = (RevoluteJointDef)def.Joint;

					WriteElement("EnableLimit", rjd.EnableLimit);
					WriteElement("EnableMotor", rjd.EnableMotor);
					WriteElement("LocalAnchorA", rjd.LocalAnchorA);
					WriteElement("LocalAnchorB", rjd.LocalAnchorB);
					WriteElement("LowerAngle", rjd.LowerAngle);
					WriteElement("MaxMotorTorque", rjd.MaxMotorTorque);
					WriteElement("MotorSpeed", rjd.MotorSpeed);
					WriteElement("ReferenceAngle", rjd.ReferenceAngle);
					WriteElement("UpperAngle", rjd.UpperAngle);
				}
				break;
			case JointType.Weld:
				{
				}
				break;
			default:
				throw new Exception();
			}

			writer.WriteEndElement();
		}
	}

	public class WorldXmlDeserializer : IWorldDeserializer, IWorldSerializationProvider
	{
		List<Shape> _shapes = new List<Shape>();
		List<FixtureDefSerialized> _fixtures = new List<FixtureDefSerialized>();
		List<BodyDefSerialized> _bodies = new List<BodyDefSerialized>();
		List<JointDefSerialized> _joints = new List<JointDefSerialized>();

		public IList<Shape> Shapes
		{
			get { return _shapes; }
		}

		public IList<FixtureDefSerialized> FixtureDefs
		{
			get { return _fixtures; }
		}

		public IList<BodyDefSerialized> Bodies
		{
			get { return _bodies; }
		}

		public IList<JointDefSerialized> Joints
		{
			get { return _joints; }
		}

		public int IndexOfFixture(FixtureDef def)
		{
			for (int i = 0; i < _fixtures.Count; ++i)
				if (_fixtures[i].Fixture == def)
					return i;

			return -1;
		}

		public int IndexOfBody(BodyDef def)
		{
			for (int i = 0; i < _bodies.Count; ++i)
				if (_bodies[i].Body == def)
					return i;

			return -1;
		}

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

							_fixtures.Add(new FixtureDefSerialized(fixture, -1));
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

							_bodies.Add(new BodyDefSerialized(null, body, fixtures));
						}
					}
					break;
				}
			}
		}
	}

	public class WorldSerializer : IWorldSerializationProvider
	{
		IWorldSerializer _serializer;

		List<Shape> _shapeDefinitions = new List<Shape>();
		List<FixtureDefSerialized> _fixtureDefinitions = new List<FixtureDefSerialized>();
		List<BodyDefSerialized> _bodyDefinitions = new List<BodyDefSerialized>();
		List<JointDefSerialized> _joints = new List<JointDefSerialized>();

		public IList<Shape> Shapes
		{
			get { return _shapeDefinitions; }
		}

		public IList<FixtureDefSerialized> FixtureDefs
		{
			get { return _fixtureDefinitions; }
		}

		public IList<BodyDefSerialized> Bodies
		{
			get { return _bodyDefinitions; }
		}

		public IList<JointDefSerialized> Joints
		{
			get { return _joints; }
		}

		public int IndexOfFixture(FixtureDef def)
		{
			for (int i = 0; i < _fixtureDefinitions.Count; ++i)
				if (_fixtureDefinitions[i].Fixture == def)
					return i;

			return -1;
		}

		public int IndexOfBody(BodyDef def)
		{
			for (int i = 0; i < _bodyDefinitions.Count; ++i)
				if (_bodyDefinitions[i].Body == def)
					return i;

			return -1;
		}

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

		static JointDef JointDefFromJoint(Joint j)
		{
			JointDef def;

			switch (j.JointType)
			{
			case JointType.Distance:
				{
					DistanceJoint dj = (DistanceJoint)j;
					DistanceJointDef djd = new DistanceJointDef();
					def = djd;

					djd.DampingRatio = dj.DampingRatio;
					djd.FrequencyHz = dj.Frequency;
					djd.LocalAnchorA = dj.AnchorA;
					djd.LocalAnchorB = dj.AnchorB;
					djd.Length = dj.Length;
				}
				break;
			case JointType.Friction:
				{
					FrictionJoint fj = (FrictionJoint)j;
					FrictionJointDef fjd = new FrictionJointDef();
					def = fjd;

					fjd.MaxForce = fj.MaxForce;
					fjd.MaxTorque = fj.MaxTorque;
				}
				break;
			case JointType.Gear:
				{
					GearJoint gj = (GearJoint)j;
					GearJointDef gjd = new GearJointDef();
					def = gjd;

					gjd.JointA = gj.JointA;
					gjd.JointB = gj.JointB;
					gjd.Ratio = gj.Ratio;
				}
				break;
			case JointType.Line:
				{
					/*LineJoint lj = (LineJoint)j;
					LineJointDef ljd = new LineJointDef();
					def = ljd;

					ljd.EnableLimit = lj.IsLimitEnabled;
					ljd.EnableMotor = lj.IsMotorEnabled;
					ljd.LocalAnchorA = lj.AnchorA;
					ljd.LocalAnchorB = lj.AnchorB;
					ljd.LocalAxisA = lj.axi*/
					throw new Exception();
				}
				break;
			}

			def.JointType = j.JointType;
			def.BodyA = dj.BodyA;
			def.BodyB = dj.BodyB;
			def.CollideConnected = dj.CollideConnected;

			return def;
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

				worldSerializer.AddBody(body, def, fixtures);
			}

			foreach (var joint in world.Joints)
				worldSerializer.AddJoint(joint, JointDefFromJoint(joint));

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

		public void AddBody(Body derivedBody, BodyDef body, List<FixtureDef> fixtures)
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

			_bodyDefinitions.Add(new BodyDefSerialized(derivedBody, body, fixtureIDs));
		}

		int IndexOfDerivedBody(Body b)
		{
			for (int i = 0; i < _bodyDefinitions.Count; ++i)
				if (_bodyDefinitions[i].DerivedBody == b)
					return i;

			return -1;
		}

		public void AddJoint(Joint derivedJoint, JointDef joint)
		{
			_joints.Add(new JointDefSerialized(derivedJoint, joint, IndexOfDerivedBody(joint.BodyA), IndexOfDerivedBody(joint.BodyB)));
		}

		public void Serialize(Stream stream)
		{
			_serializer.Open(stream, this);

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
