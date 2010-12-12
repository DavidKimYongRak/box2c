using System;
using System.Collections.Generic;
using System.IO;

namespace Box2CS
{
	/// <summary>
	/// A Box2CS exclusive.
	/// Mesh shapes are a saved collection of shapes
	/// for use as fixtures.
	/// </summary>
	public class MeshShape : IDisposable
	{
		List<Shape> _shapes = new List<Shape>();

		public List<Shape> Shapes
		{
			get { return _shapes; }
		}

		public MeshShape()
		{
		}

		public MeshShape(params Shape[] shapes)
		{
			foreach (var shape in shapes)
				_shapes.Add(shape.Clone());
		}

		public MeshShape(string fileName)
		{
			Load(fileName);
		}

		public MeshShape(Stream stream, bool binary)
		{
			if (binary)
				LoadBinary(stream);
			else
				LoadASCII(stream);
		}

		public void Load(string fileName)
		{
			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				if (fileName.EndsWith(".mesh"))
					LoadASCII(fs);
				else
					LoadBinary(fs);
			}
		}

		public void LoadBinary(Stream stream)
		{
			using (var reader = new BinaryReader(stream))
			{
				ushort count = reader.ReadUInt16();

				for (int i = 0; i < count; ++i)
				{
					EShapeType type = (EShapeType)reader.ReadByte();

					switch (type)
					{
					case EShapeType.e_circle:
						{
							CircleShape shape = new CircleShape();
							shape.Position = new Vec2(reader.ReadSingle(), reader.ReadSingle());
							shape.Radius = reader.ReadSingle();
							_shapes.Add(shape);
							continue;
						}
					case EShapeType.e_polygon:
						{
							PolygonShape shape = new PolygonShape();
							shape.Centroid = new Vec2(reader.ReadSingle(), reader.ReadSingle());
							shape.Radius = reader.ReadSingle();

							byte vertexCount = reader.ReadByte();

							Vec2[] vertices = new Vec2[vertexCount], normals = new Vec2[vertexCount];

							for (byte x = 0; x < vertexCount; ++x)
							{
								vertices[x] = new Vec2(reader.ReadSingle(), reader.ReadSingle());
								normals[x] = new Vec2(reader.ReadSingle(), reader.ReadSingle());
							}

							shape.Vertices = vertices;
							shape.Normals = normals;
							_shapes.Add(shape);
						}
						break;
					}
				}
			}
		}

		public void LoadASCII(Stream stream)
		{
		}

		public void Save(string fileName)
		{
			using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				if (fileName.EndsWith(".mesh"))
					SaveASCII(fs);
				else
					SaveBinary(fs);
			}
		}

		public void SaveBinary(Stream stream)
		{
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write((ushort)_shapes.Count);

				for (int i = 0; i < _shapes.Count; ++i)
				{
					var shape = _shapes[i];

					writer.Write((byte)shape.ShapeType);

					switch (shape.ShapeType)
					{
					case EShapeType.e_circle:
						{
							CircleShape cshape = (CircleShape)shape;
							writer.Write(cshape.Position.x);
							writer.Write(cshape.Position.y);
							writer.Write(cshape.Radius);
							continue;
						}
					case EShapeType.e_polygon:
						{
							PolygonShape cshape = (PolygonShape)shape;
							writer.Write(cshape.Centroid.x);
							writer.Write(cshape.Centroid.y);
							writer.Write(cshape.Radius);

							writer.Write((byte)cshape.VertexCount);

							for (byte x = 0; x < cshape.VertexCount; ++x)
							{
								writer.Write(cshape.Vertices[x].x);
								writer.Write(cshape.Vertices[x].y);
								writer.Write(cshape.Normals[x].x);
								writer.Write(cshape.Normals[x].y);
							}
						}
						break;
					}
				}
			}
		}

		public void SaveASCII(Stream stream)
		{
		}

		public Fixture[] AddToBody(Body body, float density)
		{
			Fixture[] fixtures = new Fixture[_shapes.Count];

			for (int i = 0; i < _shapes.Count; ++i)
				fixtures[i] = body.CreateFixture(_shapes[i], density);

			return fixtures;
		}

		#region IDisposable
		bool _disposed = false;
		public void Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
		}

		~MeshShape()
		{
			Dispose();
		}
		#endregion
	}
}
