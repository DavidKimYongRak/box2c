using System.Collections.Generic;

namespace Box2CS
{
	public static class Box2DSettings
	{
		public const int b2_maxManifoldPoints = 2;
		public const int b2_maxPolygonVertices = 8;
		public const float b2_linearSlop = 0.005f;
		public const float b2_polygonRadius = (2.0f * b2_linearSlop);
		public const string Box2CDLLName = "box2c.dll";
	}
}
