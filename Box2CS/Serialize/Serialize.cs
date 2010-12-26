using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Box2CS.Serialize
{
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
				ex = ex;
			}
		}
	}
}
