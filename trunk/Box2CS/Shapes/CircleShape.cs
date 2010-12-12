using System;
using System.Runtime.InteropServices;

namespace Box2CS
{
	public class CircleShape : Shape
	{
		cb2circleshapeportable _internalCircleShape;

		internal cb2circleshapeportable InternalCircleShape
		{
			get { return _internalCircleShape; }
		}

		StructToPtrMarshaller<cb2circleshapeportable> _internalstruct;
		internal override IntPtr Lock()
		{
			_internalCircleShape.m_shape = base.InternalShape;
			_internalstruct = new StructToPtrMarshaller<cb2circleshapeportable>(_internalCircleShape);
			return _internalstruct.Pointer;
		}

		internal override void Unlock()
		{
			_internalstruct.Free();
			_internalstruct = null;
		}

		internal CircleShape(cb2circleshapeportable portableShape)
		{
			_internalCircleShape = portableShape;
			InternalShape = portableShape.m_shape;
		}

		public CircleShape(Vec2 position, float radius = 0.0f)
		{
			_internalCircleShape = new cb2circleshapeportable();
			_internalCircleShape.m_shape = base.InternalShape;
			InternalShape.m_type = EShapeType.e_circle;
			InternalShape.m_radius = radius;
			_internalCircleShape.m_p = position;
		}

		public CircleShape() :
			this(Vec2.Empty, 0)
		{
		}

		public CircleShape(float radius) :
			this(Vec2.Empty, radius)
		{
		}

		public override Shape Clone()
		{
			CircleShape shape = new CircleShape();
			shape.Position = Position;
			shape.Radius = Radius;
			return shape;
		}

		public Vec2 Position
		{
			get { return _internalCircleShape.m_p; }
			set { _internalCircleShape.m_p = value; }
		}
	}
}
