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

		public int JointAIndex
		{
			get;
			set;
		}

		public int JointBIndex
		{
			get;
			set;
		}

		public JointDefSerialized(JointDef joint, int jointA, int jointB)
		{
			Joint = joint;
			JointAIndex = jointA;
			JointBIndex = jointB;
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
			/*writer.WriteStartElement(name);
			writer.WriteAttributeString("X", vec.X.ToString());
			writer.WriteAttributeString("Y", vec.Y.ToString());
			WriteEndElement();*/
			writer.WriteElementString(name, vec.X.ToString() + " " + vec.Y.ToString());
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

		static DistanceJointDef _defaultDistanceJoint = new DistanceJointDef();
		static FrictionJointDef _defaultFrictionJoint = new FrictionJointDef();
		static LineJointDef _defaultLineJoint = new LineJointDef();
		static PrismaticJointDef _defaultPrismaticJoint = new PrismaticJointDef();
		static PulleyJointDef _defaultPulleyJoint = new PulleyJointDef();
		static RevoluteJointDef _defaultRevoluteJoint = new RevoluteJointDef();
		static WeldJointDef _defaultWeldJoint = new WeldJointDef();

		public void SerializeJoint(JointDefSerialized def)
		{
			writer.WriteStartElement("Joint");

			writer.WriteAttributeString("Type", def.Joint.JointType.ToString());

			WriteElement("JointA", def.JointAIndex);
			WriteElement("JointB", def.JointBIndex);

			if (def.Joint.CollideConnected != false)
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

					if (djd.DampingRatio != _defaultDistanceJoint.DampingRatio)
						WriteElement("DampingRatio", djd.DampingRatio);
					if (djd.FrequencyHz != _defaultDistanceJoint.FrequencyHz)
						WriteElement("FrequencyHz", djd.FrequencyHz);
					if (djd.Length != _defaultDistanceJoint.Length)
						WriteElement("Length", djd.Length);
					if (djd.LocalAnchorA != _defaultDistanceJoint.LocalAnchorA)
						WriteElement("LocalAnchorA", djd.LocalAnchorA);
					if (djd.LocalAnchorB != _defaultDistanceJoint.LocalAnchorB)
						WriteElement("LocalAnchorB", djd.LocalAnchorB);
				}
				break;
			case JointType.Friction:
				{
					FrictionJointDef fjd = (FrictionJointDef)def.Joint;

					if (fjd.LocalAnchorA != _defaultFrictionJoint.LocalAnchorA)
						WriteElement("LocalAnchorA", fjd.LocalAnchorA);
					if (fjd.LocalAnchorB != _defaultFrictionJoint.LocalAnchorB)
						WriteElement("LocalAnchorB", fjd.LocalAnchorB);
					if (fjd.MaxForce != _defaultFrictionJoint.MaxForce)
						WriteElement("MaxForce", fjd.MaxForce);
					if (fjd.MaxTorque != _defaultFrictionJoint.MaxTorque)
						WriteElement("MaxTorque", fjd.MaxTorque);
				}
				break;
			case JointType.Gear:
				/*	GearJointDef gjd = (GearJointDef)def.Joint;

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
					WriteElement("Ratio", gjd.Ratio);*/
					throw new Exception("Gear joint not supported by serialization");
			case JointType.Line:
				{
					LineJointDef ljd = (LineJointDef)def.Joint;

					if (ljd.EnableLimit != _defaultLineJoint.EnableLimit)
						WriteElement("EnableLimit", ljd.EnableLimit);
					if (ljd.EnableMotor != _defaultLineJoint.EnableMotor)
						WriteElement("EnableMotor", ljd.EnableMotor);
					if (ljd.LocalAnchorA != _defaultLineJoint.LocalAnchorA)
						WriteElement("LocalAnchorA", ljd.LocalAnchorA);
					if (ljd.LocalAnchorB != _defaultLineJoint.LocalAnchorB)
						WriteElement("LocalAnchorB", ljd.LocalAnchorB);
					if (ljd.LocalAxisA != _defaultLineJoint.LocalAxisA)
						WriteElement("LocalAxisA", ljd.LocalAxisA);
					if (ljd.LowerTranslation != _defaultLineJoint.LowerTranslation)
						WriteElement("LowerTranslation", ljd.LowerTranslation);
					if (ljd.MaxMotorForce != _defaultLineJoint.MaxMotorForce)
						WriteElement("MaxMotorForce", ljd.MaxMotorForce);
					if (ljd.MotorSpeed != _defaultLineJoint.MotorSpeed)
						WriteElement("MotorSpeed", ljd.MotorSpeed);
					if (ljd.UpperTranslation != _defaultLineJoint.UpperTranslation)
						WriteElement("UpperTranslation", ljd.UpperTranslation);
				}
				break;
			case JointType.Prismatic:
				{
					PrismaticJointDef pjd = (PrismaticJointDef)def.Joint;

					if (pjd.EnableLimit != _defaultPrismaticJoint.EnableLimit)
						WriteElement("EnableLimit", pjd.EnableLimit);
					if (pjd.EnableMotor != _defaultPrismaticJoint.EnableMotor)
						WriteElement("EnableMotor", pjd.EnableMotor);
					if (pjd.LocalAnchorA != _defaultPrismaticJoint.LocalAnchorA)
						WriteElement("LocalAnchorA", pjd.LocalAnchorA);
					if (pjd.LocalAnchorB != _defaultPrismaticJoint.LocalAnchorB)
						WriteElement("LocalAnchorB", pjd.LocalAnchorB);
					if (pjd.LocalAxis != _defaultPrismaticJoint.LocalAxis)
						WriteElement("LocalAxisA", pjd.LocalAxis);
					if (pjd.LowerTranslation != _defaultPrismaticJoint.LowerTranslation)
						WriteElement("LowerTranslation", pjd.LowerTranslation);
					if (pjd.MaxMotorForce != _defaultPrismaticJoint.MaxMotorForce)
						WriteElement("MaxMotorForce", pjd.MaxMotorForce);
					if (pjd.MotorSpeed != _defaultPrismaticJoint.MotorSpeed)
						WriteElement("MotorSpeed", pjd.MotorSpeed);
					if (pjd.UpperTranslation != _defaultPrismaticJoint.UpperTranslation)
						WriteElement("UpperTranslation", pjd.UpperTranslation);
					if (pjd.ReferenceAngle != _defaultPrismaticJoint.ReferenceAngle)
						WriteElement("ReferenceAngle", pjd.ReferenceAngle);
				}
				break;
			case JointType.Pulley:
				{
					PulleyJointDef pjd = (PulleyJointDef)def.Joint;

					if (pjd.GroundAnchorA != _defaultPulleyJoint.GroundAnchorA)
						WriteElement("GroundAnchorA", pjd.GroundAnchorA);
					if (pjd.GroundAnchorB != _defaultPulleyJoint.GroundAnchorB)
						WriteElement("GroundAnchorB", pjd.GroundAnchorB);
					if (pjd.LengthA != _defaultPulleyJoint.LengthA)
						WriteElement("LengthA", pjd.LengthA);
					if (pjd.LengthB != _defaultPulleyJoint.LengthB)
						WriteElement("LengthB", pjd.LengthB);
					if (pjd.LocalAnchorA != _defaultPulleyJoint.LocalAnchorA)
						WriteElement("LocalAnchorA", pjd.LocalAnchorA);
					if (pjd.LocalAnchorB != _defaultPulleyJoint.LocalAnchorB)
						WriteElement("LocalAnchorB", pjd.LocalAnchorB);
					if (pjd.MaxLengthA != _defaultPulleyJoint.MaxLengthA)
						WriteElement("MaxLengthA", pjd.MaxLengthA);
					if (pjd.MaxLengthB != _defaultPulleyJoint.MaxLengthB)
						WriteElement("MaxLengthB", pjd.MaxLengthB);
					if (pjd.Ratio != _defaultPulleyJoint.Ratio)
						WriteElement("Ratio", pjd.Ratio);
				}
				break;
			case JointType.Revolute:
				{
					RevoluteJointDef rjd = (RevoluteJointDef)def.Joint;

					if (rjd.EnableLimit != _defaultRevoluteJoint.EnableLimit)
						WriteElement("EnableLimit", rjd.EnableLimit);
					if (rjd.EnableMotor != _defaultRevoluteJoint.EnableMotor)
						WriteElement("EnableMotor", rjd.EnableMotor);
					if (rjd.LocalAnchorA != _defaultRevoluteJoint.LocalAnchorA)
						WriteElement("LocalAnchorA", rjd.LocalAnchorA);
					if (rjd.LocalAnchorB != _defaultRevoluteJoint.LocalAnchorB)
						WriteElement("LocalAnchorB", rjd.LocalAnchorB);
					if (rjd.LowerAngle != _defaultRevoluteJoint.LowerAngle)
						WriteElement("LowerAngle", rjd.LowerAngle);
					if (rjd.MaxMotorTorque != _defaultRevoluteJoint.MaxMotorTorque)
						WriteElement("MaxMotorTorque", rjd.MaxMotorTorque);
					if (rjd.MotorSpeed != _defaultRevoluteJoint.MotorSpeed)
						WriteElement("MotorSpeed", rjd.MotorSpeed);
					if (rjd.ReferenceAngle != _defaultRevoluteJoint.ReferenceAngle)
						WriteElement("ReferenceAngle", rjd.ReferenceAngle);
					if (rjd.UpperAngle != _defaultRevoluteJoint.UpperAngle)
						WriteElement("UpperAngle", rjd.UpperAngle);
				}
				break;
			case JointType.Weld:
				{
					WeldJointDef wjd = (WeldJointDef)def.Joint;

					if (wjd.LocalAnchorA != _defaultWeldJoint.LocalAnchorA)
						WriteElement("LocalAnchorA", wjd.LocalAnchorA);
					if (wjd.LocalAnchorB != _defaultWeldJoint.LocalAnchorB)
						WriteElement("LocalAnchorB", wjd.LocalAnchorB);
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
			return Vec2.Parse(node.FirstChild.Value);
		}

		object ReadSimpleType(XmlNode node, Type type, bool outer)
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
				case "joints":
					{
						foreach (XmlNode n in main)
						{
							JointDef mainDef = null;

							if (n.Name.ToLower() != "joint")
								throw new Exception();

							JointType type = (JointType)Enum.Parse(typeof(JointType), n.Attributes[0].Value, true);

							int jointA = -1, jointB = -1;
							bool collideConnected = false;
							object userData = null;

							switch (type)
							{
							case JointType.Distance:
								mainDef = new DistanceJointDef();
								break;
							case JointType.Friction:
								mainDef = new FrictionJointDef();
								break;
							case JointType.Line:
								mainDef = new LineJointDef();
								break;
							case JointType.Prismatic:
								mainDef = new PrismaticJointDef();
								break;
							case JointType.Pulley:
								mainDef = new PulleyJointDef();
								break;
							case JointType.Revolute:
								mainDef = new RevoluteJointDef();
								break;
							case JointType.Weld:
								mainDef = new WeldJointDef();
								break;
							default:
								throw new Exception("Invalid or unsupported joint");
							}

							foreach (XmlNode sn in n)
							{
								// check for specific nodes
								switch (type)
								{
								case JointType.Distance:
									{
										switch (sn.Name.ToLower())
										{
										case "dampingratio":
											((DistanceJointDef)mainDef).DampingRatio = float.Parse(sn.FirstChild.Value);
											break;
										case "frequencyhz":
											((DistanceJointDef)mainDef).FrequencyHz = float.Parse(sn.FirstChild.Value);
											break;
										case "length":
											((DistanceJointDef)mainDef).Length = float.Parse(sn.FirstChild.Value);
											break;
										case "localanchora":
											((DistanceJointDef)mainDef).LocalAnchorA = ReadVector(sn);
											break;
										case "localanchorb":
											((DistanceJointDef)mainDef).LocalAnchorB = ReadVector(sn);
											break;
										}
									}
									break;
								case JointType.Friction:
									{
										switch (sn.Name.ToLower())
										{
										case "localanchora":
											((FrictionJointDef)mainDef).LocalAnchorA = ReadVector(sn);
											break;
										case "localanchorb":
											((FrictionJointDef)mainDef).LocalAnchorB = ReadVector(sn);
											break;
										case "maxforce":
											((FrictionJointDef)mainDef).MaxForce = float.Parse(sn.FirstChild.Value);
											break;
										case "maxtorque":
											((FrictionJointDef)mainDef).MaxTorque = float.Parse(sn.FirstChild.Value);
											break;
										}
									}
									break;
								case JointType.Line:
									{
										switch (sn.Name.ToLower())
										{
										case "enablelimit":
											((LineJointDef)mainDef).EnableLimit = bool.Parse(sn.FirstChild.Value);
											break;
										case "enablemotor":
											((LineJointDef)mainDef).EnableMotor = bool.Parse(sn.FirstChild.Value);
											break;
										case "localanchora":
											((LineJointDef)mainDef).LocalAnchorA = ReadVector(sn);
											break;
										case "localanchorb":
											((LineJointDef)mainDef).LocalAnchorB = ReadVector(sn);
											break;
										case "localaxisa":
											((LineJointDef)mainDef).LocalAxisA = ReadVector(sn);
											break;
										case "maxmotorforce":
											((LineJointDef)mainDef).MaxMotorForce = float.Parse(sn.FirstChild.Value);
											break;
										case "motorspeed":
											((LineJointDef)mainDef).MotorSpeed = float.Parse(sn.FirstChild.Value);
											break;
										case "lowertranslation":
											((LineJointDef)mainDef).LowerTranslation = float.Parse(sn.FirstChild.Value);
											break;
										case "uppertranslation":
											((LineJointDef)mainDef).UpperTranslation = float.Parse(sn.FirstChild.Value);
											break;
										}
									}
									break;
								case JointType.Prismatic:
									{
										switch (sn.Name.ToLower())
										{
										case "enablelimit":
											((PrismaticJointDef)mainDef).EnableLimit = bool.Parse(sn.FirstChild.Value);
											break;
										case "enablemotor":
											((PrismaticJointDef)mainDef).EnableMotor = bool.Parse(sn.FirstChild.Value);
											break;
										case "localanchora":
											((PrismaticJointDef)mainDef).LocalAnchorA = ReadVector(sn);
											break;
										case "localanchorb":
											((PrismaticJointDef)mainDef).LocalAnchorB = ReadVector(sn);
											break;
										case "localaxisa":
											((PrismaticJointDef)mainDef).LocalAxis = ReadVector(sn);
											break;
										case "maxmotorforce":
											((PrismaticJointDef)mainDef).MaxMotorForce = float.Parse(sn.FirstChild.Value);
											break;
										case "motorspeed":
											((PrismaticJointDef)mainDef).MotorSpeed = float.Parse(sn.FirstChild.Value);
											break;
										case "lowertranslation":
											((PrismaticJointDef)mainDef).LowerTranslation = float.Parse(sn.FirstChild.Value);
											break;
										case "uppertranslation":
											((PrismaticJointDef)mainDef).UpperTranslation = float.Parse(sn.FirstChild.Value);
											break;
										case "referenceangle":
											((PrismaticJointDef)mainDef).ReferenceAngle = float.Parse(sn.FirstChild.Value);
											break;
										}
									}
									break;
								case JointType.Pulley:
									{
										switch (sn.Name.ToLower())
										{
										case "groundanchora":
											((PulleyJointDef)mainDef).GroundAnchorA = ReadVector(sn);
											break;
										case "groundanchorb":
											((PulleyJointDef)mainDef).GroundAnchorB = ReadVector(sn);
											break;
										case "lengtha":
											((PulleyJointDef)mainDef).LengthA = float.Parse(sn.FirstChild.Value);
											break;
										case "lengthb":
											((PulleyJointDef)mainDef).LengthB = float.Parse(sn.FirstChild.Value);
											break;
										case "localanchora":
											((PulleyJointDef)mainDef).LocalAnchorA = ReadVector(sn);
											break;
										case "localanchorb":
											((PulleyJointDef)mainDef).LocalAnchorB = ReadVector(sn);
											break;
										case "maxlengtha":
											((PulleyJointDef)mainDef).MaxLengthA = float.Parse(sn.FirstChild.Value);
											break;
										case "maxlengthb":
											((PulleyJointDef)mainDef).MaxLengthB = float.Parse(sn.FirstChild.Value);
											break;
										case "ratio":
											((PulleyJointDef)mainDef).Ratio = float.Parse(sn.FirstChild.Value);
											break;
										}
									}
									break;
								case JointType.Revolute:
									{
										switch (sn.Name.ToLower())
										{
										case "enablelimit":
											((RevoluteJointDef)mainDef).EnableLimit = bool.Parse(sn.FirstChild.Value);
											break;
										case "enablemotor":
											((RevoluteJointDef)mainDef).EnableMotor = bool.Parse(sn.FirstChild.Value);
											break;
										case "localanchora":
											((RevoluteJointDef)mainDef).LocalAnchorA = ReadVector(sn);
											break;
										case "localanchorb":
											((RevoluteJointDef)mainDef).LocalAnchorB = ReadVector(sn);
											break;
										case "maxmotortorque":
											((RevoluteJointDef)mainDef).MaxMotorTorque = float.Parse(sn.FirstChild.Value);
											break;
										case "motorspeed":
											((RevoluteJointDef)mainDef).MotorSpeed = float.Parse(sn.FirstChild.Value);
											break;
										case "lowerangle":
											((RevoluteJointDef)mainDef).LowerAngle = float.Parse(sn.FirstChild.Value);
											break;
										case "upperangle":
											((RevoluteJointDef)mainDef).UpperAngle = float.Parse(sn.FirstChild.Value);
											break;
										case "referenceangle":
											((RevoluteJointDef)mainDef).ReferenceAngle = float.Parse(sn.FirstChild.Value);
											break;
										}
									}
									break;
								case JointType.Weld:
									{
										switch (sn.Name.ToLower())
										{
										case "localanchora":
											((WeldJointDef)mainDef).LocalAnchorA = ReadVector(sn);
											break;
										case "localanchorb":
											((WeldJointDef)mainDef).LocalAnchorB = ReadVector(sn);
											break;
										}
									}
									break;
								case JointType.Gear:
									throw new Exception("Gear joint is unsupported");
								}

								switch (sn.Name.ToLower())
								{
								case "jointa":
									jointA = int.Parse(sn.FirstChild.Value);
									break;
								case "jointb":
									jointB = int.Parse(sn.FirstChild.Value);
									break;
								case "collideconnected":
									collideConnected = bool.Parse(sn.FirstChild.Value);
									break;
								case "userdata":
									userData = ReadSimpleType(sn, null, false);
									break;
								}
							}

							mainDef.CollideConnected = collideConnected;
							mainDef.UserData = userData;
							_joints.Add(new JointDefSerialized(mainDef, jointA, jointB));
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

		public void AddJoint(JointDef joint)
		{
			_joints.Add(new JointDefSerialized(joint, IndexOfDerivedBody(joint.BodyA), IndexOfDerivedBody(joint.BodyB)));
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

			_serializer.BeginSerializingJoints();
			foreach (var j in _joints)
				_serializer.SerializeJoint(j);
			_serializer.EndSerializingJoints();

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
