﻿using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public sealed class PolygonShape : Shape
	{
		cb2polygonshapeportable _internalPolyShape;

		internal cb2polygonshapeportable InternalPolyShape
		{
			get { return _internalPolyShape; }
		}

		StructToPtrMarshaller _internalstruct;
		internal override IntPtr Lock()
		{
			_internalPolyShape.m_shape = base.InternalShape;
			_internalstruct = new StructToPtrMarshaller(_internalPolyShape);
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

		bool _autoReverse;

		public bool AutoReverse
		{
			get { return _autoReverse; }
			set { _autoReverse = value; }
		}

		public PolygonShape(Vec2[] vertices, Vec2 centroid, bool autoReverse = false) :
			this()
		{
			_autoReverse = autoReverse;
			if (vertices.Length == 2)
				SetAsEdge(vertices[0], vertices[1]);
			else
				Vertices = vertices;
			Centroid = centroid;
		}

		public PolygonShape(params Vec2[] vertices) :
			this(false, vertices)
		{
		}

		public PolygonShape(bool autoReverse, params Vec2[] vertices) :
			this(vertices, Vec2.Empty, autoReverse)
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
				throw new ArgumentOutOfRangeException("Vertice count");

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
				throw new Exception("Area of polygon is too small");
			c *= 1.0f / area;
			return c;
		}

		void Set()
		{
			if (!(2 <= VertexCount && VertexCount <= Box2DSettings.b2_maxPolygonVertices))
				throw new ArgumentOutOfRangeException("Vertices", "Vertice count is " + ((2 <= VertexCount) ? "less than 2" : "greater than "+Box2DSettings.b2_maxPolygonVertices.ToString()));

			// Compute normals. Ensure the edges have non-zero length.
			for (int i = 0; i < VertexCount; ++i)
			{
				int i1 = i;
				int i2 = i + 1 < VertexCount ? i + 1 : 0;
				Vec2 edge = _internalPolyShape.m_vertices[i2] - _internalPolyShape.m_vertices[i1];

				if (!(edge.LengthSquared() > float.Epsilon * float.Epsilon))
					throw new Exception("Edge has a close-to-zero length (vertices too close?)");

				_internalPolyShape.m_normals[i] = edge.Cross(1.0f);
				_internalPolyShape.m_normals[i].Normalize();
			}

			if (_autoReverse)
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
					{
						if (_autoReverse)
							ReverseOrder();
						else
							throw new Exception("Polygon is non-convex or has colinear edges");
						return;
					}
				}
			}

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
					throw new IndexOutOfRangeException("value");

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
					throw new IndexOutOfRangeException("value");

				for (int i = 0; i < value.Length; ++i)
					_internalPolyShape.m_normals[i] = value[i];

				_internalPolyShape.m_vertexCount = value.Length;
			}
		}

		public void ReverseOrder()
		{
			var list = new System.Collections.Generic.List<Vec2>(Vertices);
			list.Reverse();
			Vertices = list.ToArray();
		}

		public override void ComputeAABB(out AABB aabb, Transform xf)
		{
			Vec2 lower = xf * Vertices[0];
			Vec2 upper = lower;

			for (int i = 1; i < VertexCount; ++i)
			{
				Vec2 v = xf * Vertices[i];
				lower = Vec2.Min(lower, v);
				upper = Vec2.Max(upper, v);
			}

			Vec2 r = new Vec2(Radius, Radius);
			aabb = new AABB(lower - r,
							upper + r);
		}

		public override void ComputeMass(out MassData massData, float density)
		{
			if (!(VertexCount >= 2))
				throw new ArgumentOutOfRangeException("Vertice count less than 2");

			// A line segment has zero mass.
			if (VertexCount == 2)
			{
				massData = new MassData(0.0f,
										0.5f * (Vertices[0] + Vertices[1]),
										0.0f);
				return;
			}

			Vec2 center = Vec2.Empty;
			float area = 0.0f;
			float I = 0.0f;

			// pRef is the reference point for forming triangles.
			// It's location doesn't change the result (except for rounding error).
			Vec2 pRef = Vec2.Empty;
		#if NO
			// This code would put the reference point inside the polygon.
			for (int i = 0; i < VertexCount; ++i)
			{
				pRef += Vertices[i];
			}
			pRef *= 1.0f / count;
		#endif

			const float k_inv3 = 1.0f / 3.0f;

			for (int i = 0; i < VertexCount; ++i)
			{
				// Triangle vertices.
				Vec2 p1 = pRef;
				Vec2 p2 = Vertices[i];
				Vec2 p3 = i + 1 < VertexCount ? Vertices[i+1] : Vertices[0];

				Vec2 e1 = p2 - p1;
				Vec2 e2 = p3 - p1;

				float D = e1.Cross(e2);

				float triangleArea = 0.5f * D;
				area += triangleArea;

				// Area weighted centroid
				center += triangleArea * k_inv3 * (p1 + p2 + p3);

				float px = p1.x, py = p1.y;
				float ex1 = e1.x, ey1 = e1.y;
				float ex2 = e2.x, ey2 = e2.y;

				float intx2 = k_inv3 * (0.25f * (ex1*ex1 + ex2*ex1 + ex2*ex2) + (px*ex1 + px*ex2)) + 0.5f*px*px;
				float inty2 = k_inv3 * (0.25f * (ey1*ey1 + ey2*ey1 + ey2*ey2) + (py*ey1 + py*ey2)) + 0.5f*py*py;

				I += D * (intx2 + inty2);
			}

			if (!(area > float.Epsilon))
				throw new Exception("Area is too small");

			massData = new MassData(density * area,
				center * (1.0f / area),
				density * I);
		}
	}
}
