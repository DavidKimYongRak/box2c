using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public class PolygonShape : Shape
	{
		cb2polygonshapeportable _internalPolyShape;

		internal cb2polygonshapeportable InternalPolyShape
		{
			get { return _internalPolyShape; }
		}

		StructToPtrMarshaller<cb2polygonshapeportable> _internalstruct;
		internal override IntPtr Lock()
		{
			_internalPolyShape.m_shape = base.InternalShape;
			_internalstruct = new StructToPtrMarshaller<cb2polygonshapeportable>(_internalPolyShape);
			return _internalstruct.Pointer;
		}

		internal override void Unlock()
		{
			_internalstruct.Free();
			_internalstruct = null;
		}

		public PolygonShape()
		{
			_internalPolyShape = new cb2polygonshapeportable();
			InternalShape.m_radius = Box2DSettings.b2_polygonRadius;
			_internalPolyShape.m_shape = base.InternalShape;
			InternalShape.m_type = EShapeType.e_polygon;
		}

		public PolygonShape(Vec2[] vertices, Vec2 centroid) :
			this()
		{
			if (vertices.Length == 2)
				SetAsEdge(vertices[0], vertices[1]);
			else
				Vertices = vertices;
			Centroid = centroid;
		}

		public PolygonShape(params Vec2[] vertices) :
			this(vertices, Vec2.Empty)
		{
		}

		public PolygonShape(float width, float height, Vec2 centroid, float angle = 0.0f) :
			this()
		{
			SetAsBox(width, height, centroid, angle);
		}

		public PolygonShape(float width, float height, float angle = 0.0f) :
			this(width, height, Vec2.Empty, angle)
		{
		}

		internal PolygonShape(cb2polygonshapeportable portableShape)
		{
			_internalPolyShape = portableShape;
			InternalShape = portableShape.m_shape;
		}

		public override Shape Clone()
		{
			PolygonShape shape = new PolygonShape();
			shape.Centroid = Centroid;
			shape.Normals = Normals;
			shape.Radius = Radius;
			shape.Vertices = Vertices;
			return shape;
		}

		public void SetAsBox(float hx, float hy)
		{
			SetAsBox(hx, hy, Vec2.Empty, 0);
		}

		internal bool _autoSet = true;
		public void SetAsBox(float hx, float hy, Vec2 center, float angle)
		{
			var tempVertices = new Vec2[]
			{
				new Vec2(-hx, -hy),
				new Vec2(hx, -hy),
				new Vec2(hx, hy),
				new Vec2(-hx, hy)
			};

			var tempNormals = new Vec2[]
			{
				new Vec2(0.0f, -1.0f),
				new Vec2(1.0f, 0.0f),
				new Vec2(0.0f, 1.0f),
				new Vec2(-1.0f, 0.0f)
			};

			Transform xf = new Transform();
			xf.position = center;
			xf.R.Set(angle);

			// Transform vertices and normals.
			for (int i = 0; i < tempVertices.Length; ++i)
			{
				tempVertices[i] = xf * tempVertices[i];
				tempNormals[i] = xf.R * tempNormals[i];
			}

			Vertices = tempVertices;
			Normals = tempNormals;

			Centroid = center;
		}

		public void SetAsEdge(Vec2 v1, Vec2 v2)
		{
			Vertices = new Vec2[]
			{
				v1,
				v2
			};
			Centroid = 0.5f * (v1 + v2);

			var tempNormal = (v2 - v1).Cross(1.0f).Normalized();
			Normals = new Vec2[]
			{
				tempNormal,
				-tempNormal
			};
		}

		public Vec2 Centroid
		{
			get { return _internalPolyShape.m_centroid; }
			set { _internalPolyShape.m_centroid = value; }
		}

		public int VertexCount
		{
			get { return _internalPolyShape.m_vertexCount; }
			set { _internalPolyShape.m_vertexCount = value; }
		}

		Vec2 ComputeCentroid()
		{
			if (!(_internalPolyShape.m_vertexCount >= 2))
				throw new Exception();

			Vec2 c = Vec2.Empty;
			float area = 0.0f;

			if (_internalPolyShape.m_vertexCount == 2)
			{
				c = 0.5f * (_internalPolyShape.m_vertices[0] + _internalPolyShape.m_vertices[1]);
				return c;
			}

			// pRef is the reference point for forming triangles.
			// It's location doesn't change the result (except for rounding error).
			Vec2  pRef = Vec2.Empty;

			const float inv3 = 1.0f / 3.0f;

			for (int i = 0; i < _internalPolyShape.m_vertexCount; ++i)
			{
				// Triangle vertices.
				Vec2 p1 = pRef;
				Vec2 p2 = _internalPolyShape.m_vertices[i];
				Vec2 p3 = i + 1 < _internalPolyShape.m_vertexCount ? _internalPolyShape.m_vertices[i + 1] : 

_internalPolyShape.m_vertices[0];

				Vec2 e1 = p2 - p1;
				Vec2 e2 = p3 - p1;

				float D = e1.Cross(e2);

				float triangleArea = 0.5f * D;
				area += triangleArea;

				// Area weighted centroid
				c += triangleArea * inv3 * (p1 + p2 + p3);
			}

			// Centroid
			if (!(area > float.Epsilon))
				throw new Exception();
			c *= 1.0f / area;
			return c;
		}

		void Set()
		{
			if (!(2 <= VertexCount && VertexCount <= Box2DSettings.b2_maxPolygonVertices))
				throw new Exception();

			// Compute normals. Ensure the edges have non-zero length.
			for (int i = 0; i < VertexCount; ++i)
			{
				int i1 = i;
				int i2 = i + 1 < VertexCount ? i + 1 : 0;
				Vec2 edge = _internalPolyShape.m_vertices[i2] - _internalPolyShape.m_vertices[i1];
				if (!(edge.LengthSquared() > float.Epsilon * float.Epsilon))
					throw new Exception();
				_internalPolyShape.m_normals[i] = edge.Cross(1.0f);
				_internalPolyShape.m_normals[i].Normalize();
			}

#if DEBUG
			// Ensure the polygon is convex and the interior
			// is to the left of each edge.
			for (int i = 0; i < _internalPolyShape.m_vertexCount; ++i)
			{
				int i1 = i;
				int i2 = i + 1 < _internalPolyShape.m_vertexCount ? i + 1 : 0;
				Vec2 edge = _internalPolyShape.m_vertices[i2] - _internalPolyShape.m_vertices[i1];

				for (int j = 0; j < _internalPolyShape.m_vertexCount; ++j)
				{
					// Don't check vertices on the current edge.
					if (j == i1 || j == i2)
					{
						continue;
					}

					Vec2 r = _internalPolyShape.m_vertices[j] - _internalPolyShape.m_vertices[i1];

					// Your polygon is non-convex (it has an indentation) or
					// has colinear edges.
					float s = edge.Cross(r);
					if (!(s > 0.0f))
						throw new Exception();
				}
			}
#endif

			// Compute the polygon centroid.
			_internalPolyShape.m_centroid = ComputeCentroid();
		}

		public Vec2[] Vertices
		{
			get
			{
				Vec2[] verts = new Vec2[VertexCount];

				for (int i = 0; i < verts.Length; ++i)
					verts[i] = _internalPolyShape.m_vertices[i];

				return verts;
			}

			set
			{
				if (value.Length > 8)
					throw new IndexOutOfRangeException();

				for (int i = 0; i < value.Length; ++i)
					_internalPolyShape.m_vertices[i] = value[i];

				VertexCount = value.Length;

				if (_autoSet)
					Set();
			}
		}

		public Vec2[] Normals
		{
			get
			{
				Vec2[] verts = new Vec2[VertexCount];

				for (int i = 0; i < verts.Length; ++i)
					verts[i] = _internalPolyShape.m_normals[i];

				return verts;
			}

			set
			{
				if (value.Length > 8)
					throw new IndexOutOfRangeException();

				for (int i = 0; i < value.Length; ++i)
					_internalPolyShape.m_normals[i] = value[i];

				_internalPolyShape.m_vertexCount = value.Length;
			}
		}
	}
}
