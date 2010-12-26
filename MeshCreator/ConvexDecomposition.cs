using System;
using System.Linq;
using System.Collections.Generic;
using Box2CS;

namespace MeshCreator
{
	namespace ConvexDecomposition
	{
		public class Triangle
		{
			public float[] x = new float[3];
			public float[] y = new float[3];

			public Triangle(float x1, float y1, float x2, float y2, float x3, float y3)
			{
				float dx1 = x2-x1;
				float dx2 = x3-x1;
				float dy1 = y2-y1;
				float dy2 = y3-y1;
				float cross = dx1*dy2-dx2*dy1;
				bool ccw = (cross>0);
				if (ccw)
				{
					x[0] = x1; x[1] = x2; x[2] = x3;
					y[0] = y1; y[1] = y2; y[2] = y3;
				}
				else
				{
					x[0] = x1; x[1] = x3; x[2] = x2;
					y[0] = y1; y[1] = y3; y[2] = y2;
				}

			}

			public bool IsInside(float _x, float _y)
			{
				if (_x < x[0] && _x < x[1] && _x < x[2]) return false;
				if (_x > x[0] && _x > x[1] && _x > x[2]) return false;
				if (_y < y[0] && _y < y[1] && _y < y[2]) return false;
				if (_y > y[0] && _y > y[1] && _y > y[2]) return false;

				float vx2 = _x-x[0]; float vy2 = _y-y[0];
				float vx1 = x[1]-x[0]; float vy1 = y[1]-y[0];
				float vx0 = x[2]-x[0]; float vy0 = y[2]-y[0];

				float dot00 = vx0*vx0+vy0*vy0;
				float dot01 = vx0*vx1+vy0*vy1;
				float dot02 = vx0*vx2+vy0*vy2;
				float dot11 = vx1*vx1+vy1*vy1;
				float dot12 = vx1*vx2+vy1*vy2;
				float invDenom = 1.0f / (dot00*dot11 - dot01*dot01);
				float u = (dot11*dot02 - dot01*dot12)*invDenom;
				float v = (dot00*dot12 - dot01*dot02)*invDenom;

				return ((u>=0)&&(v>=0)&&(u+v<=1));
			}

			public void Set(Triangle toMe)
			{
				x = toMe.x;
				y = toMe.y;
			}
		}

		/*int remainder(int x, int modulus);
		int TriangulatePolygon(float* xv, float* yv, int vNum, b2Triangle* results);
		bool IsEar(int i, float* xv, float* yv, int xvLength); //Not for external use
		int PolygonizeTriangles(b2Triangle* triangulated, int triangulatedLength, b2Polygon* polys, int polysLength);
		int DecomposeConvex(b2Polygon* p, b2Polygon* results, int maxPolys);
		void DecomposeConvexAndAddTo(b2Polygon* p, b2Body* bd, b2FixtureDef* prototype);
		b2Polygon ConvexHull(b2Vec2* v, int nVert);
		b2Polygon ConvexHull(float* cloudX, float* cloudY, int nVert);
		void ReversePolygon(float* x, float* y, int n);

		b2Polygon TraceEdge(b2Polygon* p); //For use with self-intersecting polygons, finds outline*/

		public class Polygon
		{
			public const int maxVerticesPerPolygon = 8;

			public List<float> x = new List<float>(); //vertex arrays
			public List<float> y = new List<float>();

			public float area;
			public bool areaIsSet = false;

			public int nVertices
			{
				get
				{
					return x.Count;
				}
			}

			public Polygon(List<float> _x, List<float> _y)
			{
				for (int i = 0; i < _x.Count; ++i)
				{
					x.Add(_x[i]);
					y.Add(_y[i]);
				}
			}
			public Polygon(List<Vec2> v)
			{
				for (int i = 0; i < v.Count; ++i)
				{
					x.Add(v[i].X);
					y.Add(v[i].Y);
				}
			}
			public Polygon()
			{
			}

			public float GetArea()
			{
				// TODO: fix up the areaIsSet caching so that it can be used
				//if (areaIsSet) return area;
				area = 0.0f;

				//First do wraparound
				area += x[nVertices-1]*y[0]-x[0]*y[nVertices-1];
				for (int i=0; i<nVertices-1; ++i)
				{
					area += x[i]*y[i+1]-x[i+1]*y[i];
				}
				area *= .5f;
				areaIsSet = true;
				return area;
			}

			public void MergeParallelEdges(float tolerance)
			{
				if (nVertices <= 3) return; //Can't do anything useful here to a triangle
				bool[] mergeMe = new bool[nVertices];
				int newNVertices = nVertices;
				for (int i = 0; i < nVertices; ++i)
				{
					int lower = (i == 0) ? (nVertices - 1) : (i - 1);
					int middle = i;
					int upper = (i == nVertices - 1) ? (0) : (i + 1);
					float dx0 = x[middle] - x[lower];
					float dy0 = y[middle] - y[lower];
					float dx1 = x[upper] - x[middle];
					float dy1 = y[upper] - y[middle];
					float norm0 = (float)Math.Sqrt(dx0*dx0+dy0*dy0);
					float norm1 = (float)Math.Sqrt(dx1*dx1+dy1*dy1);
					if (!(norm0 > 0.0f && norm1 > 0.0f) && newNVertices > 3)
					{
						//Merge identical points
						mergeMe[i] = true;
						--newNVertices;
					}
					dx0 /= norm0; dy0 /= norm0;
					dx1 /= norm1; dy1 /= norm1;
					float cross = dx0 * dy1 - dx1 * dy0;
					float dot = dx0 * dx1 + dy0 * dy1;
					if (Math.Abs(cross) < tolerance && dot > 0 && newNVertices > 3)
					{
						mergeMe[i] = true;
						--newNVertices;
					}
					else
					{
						mergeMe[i] = false;
					}
				}
				if (newNVertices == nVertices || newNVertices == 0)
					return;
				float[] newx = new float[newNVertices];
				float[] newy = new float[newNVertices];
				int currIndex = 0;
				for (int i=0; i < nVertices; ++i)
				{
					if (mergeMe[i] || newNVertices == 0 || currIndex == newNVertices) continue;
					if (!(currIndex < newNVertices))
						throw new Exception("ass");

					newx[currIndex] = x[i];
					newy[currIndex] = y[i];
					++currIndex;
				}
				x = newx.ToList<float>();
				y = newy.ToList<float>();
			}

			public List<Vec2> GetVertexVecs()
			{
				List<Vec2> outList = new List<Vec2>();
				for (int i = 0; i < nVertices; ++i)
					outList.Add(new Vec2(x[i], y[i]));
				return outList;

			}

			public Polygon(Triangle t)
			{
				for (int i = 0; i < 3; ++i)
				{
					x.Add(t.x[i]);
					y.Add(t.y[i]);
				}

			}

			public void Set(Polygon p)
			{
				x.Clear();
				y.Clear();

				for (int i = 0; i < p.nVertices; ++i)
				{
					x.Add(p.x[i]);
					y.Add(p.y[i]);
				}
				areaIsSet = false;

			}

			public bool IsConvex()
			{
				bool isPositive = false;
				for (int i = 0; i < nVertices; ++i)
				{
					int lower = (i == 0) ? (nVertices - 1) : (i - 1);
					int middle = i;
					int upper = (i == nVertices - 1) ? (0) : (i + 1);
					float dx0 = x[middle] - x[lower];
					float dy0 = y[middle] - y[lower];
					float dx1 = x[upper] - x[middle];
					float dy1 = y[upper] - y[middle];
					float cross = dx0 * dy1 - dx1 * dy0;
					// Cross product should have same sign
					// for each vertex if poly is convex.
					bool newIsP = (cross >= 0) ? true : false;
					if (i == 0)
					{
						isPositive = newIsP;
					}
					else if (isPositive != newIsP)
					{
						return false;
					}
				}
				return true;

			}

			public bool IsCCW()
			{
				return (GetArea() > 0.0f);
			}

			public const bool ReportErrors = true;
			public bool IsUsable(bool printError = ReportErrors)
			{
				int error = -1;
				bool noError = true;
				if (nVertices < 3 || nVertices > maxVerticesPerPolygon) { noError = false; error = 0; }
				if (!IsConvex()) { noError = false; error = 1; }
				if (!IsSimple()) { noError = false; error = 2; }
				if (GetArea() < Statics.FLT_EPSILON) { noError = false; error = 3; }

				//Compute normals
				List<Vec2> normals = new List<Vec2>();
				List<Vec2> vertices = new List<Vec2>();
				for (int i = 0; i < nVertices; ++i)
				{
					vertices[i] = new Vec2(x[i], y[i]);
					int i1 = i;
					int i2 = i + 1 < nVertices ? i + 1 : 0;
					Vec2 edge = new Vec2(x[i2]-x[i1], y[i2]-y[i1]);
					normals[i] = edge.Cross(1.0f);
					normals[i].Normalize();
				}

				//Required side checks
				for (int i=0; i<nVertices; ++i)
				{
					int iminus = (i==0)?nVertices-1:i-1;
					//int iplus = (i==nVertices-1)?0:i+1;

					//Parallel sides check
					float cross = normals[iminus].Cross(normals[i]);
					cross = b2Math.b2Clamp(cross, -1.0f, 1.0f);
					float angle = (float)Math.Asin(cross);
					if (angle <= Box2DSettings.b2_angularSlop)
					{
						noError = false;
						error = 4;
						break;
					}

					//Too skinny check
					for (int j=0; j<nVertices; ++j)
					{
						if (j == i || j == (i + 1) % nVertices)
						{
							continue;
						}
						float s = normals[i].Dot(vertices[j] - vertices[i]);
						if (s >= -Box2DSettings.b2_angularSlop)
						{
							noError = false;
							error = 5;
						}
					}


					Vec2 centroid = Statics.PolyCentroid(vertices);
					Vec2 n1 = normals[iminus];
					Vec2 n2 = normals[i];
					Vec2 v = vertices[i] - centroid; ;

					Vec2 d = new Vec2(
					n1.Dot(v) - 0.04f,
					n2.Dot(v) - 0.04f);

					// Shifting the edge inward by b2_toiSlop should
					// not cause the plane to pass the centroid.
					if ((d.X < 0.0f)||(d.Y < 0.0f))
					{
						noError = false;
						error = 6;
					}

				}

				if (!noError && printError)
				{
					string exceptionStr = "Found invalid polygon, ";
					switch (error)
					{
					case 0:
						exceptionStr += "must have between 3 and "+Polygon.maxVerticesPerPolygon.ToString()+" vertices.\n";
						break;
					case 1:
						exceptionStr += "must be convex.\n";
						break;
					case 2:
						exceptionStr += "must be simple (cannot intersect itself).\n";
						break;
					case 3:
						exceptionStr += "area is too small.\n";
						break;
					case 4:
						exceptionStr += "sides are too close to parallel.\n";
						break;
					case 5:
						exceptionStr += "polygon is too thin.\n";
						break;
					case 6:
						exceptionStr += "core shape generation would move edge past centroid (too thin).\n";
						break;
					default:
						exceptionStr +="don't know why.\n";
						break;
					}
					throw new Exception(exceptionStr);
				}
				return noError;

			}

			public bool IsSimple()
			{
				for (int i=0; i<nVertices; ++i)
				{
					int iplus = (i+1 > nVertices-1)?0:i+1;
					Vec2 a1 = new Vec2(x[i], y[i]);
					Vec2 a2 = new Vec2(x[iplus], y[iplus]);
					for (int j=i+1; j<nVertices; ++j)
					{
						int jplus = (j+1 > nVertices-1)?0:j+1;
						Vec2 b1 = new Vec2(x[j], y[j]);
						Vec2 b2 = new Vec2(x[jplus], y[jplus]);
						if (Statics.intersect(a1, a2, b1, b2))
							return false;
					}
				}
				return true;

			}
			/*void AddTo(b2FixtureDef& pd))
			{
				if (nVertices < 3) return;
       
				b2Assert(nVertices <= b2_maxPolygonVertices);
       
				b2Vec2* vecs = GetVertexVecs();
				b2Vec2* vecsToAdd = new b2Vec2[nVertices];


				int offset = 0;
       
				b2PolygonShape *polyShape = new b2PolygonShape;
				int ind;
       
			for (int i = 0; i < nVertices; ++i) {
               
						//Omit identical neighbors (including wraparound)
						ind = i - offset;
						if (vecs[i].X==vecs[remainder(i+1,nVertices)].X &&
								vecs[i].Y==vecs[remainder(i+1,nVertices)].Y){
										offset++;
										continue;
						}
               
						vecsToAdd[ind] = vecs[i];
               
			}
       
				polyShape->Set((const b2Vec2*)vecsToAdd, ind+1);
				pd.shape = polyShape;
       
			delete[] vecs;
				delete[] vecsToAdd;

			}*/

			public Polygon Add(Triangle t)
			{
				//              float equalTol = .001f;
				// First, find vertices that connect
				int firstP = -1;
				int firstT = -1;
				int secondP = -1;
				int secondT = -1;
				for (int i = 0; i < nVertices; i++)
				{
					if (t.x[0] == x[i] && t.y[0] == y[i])
					{
						if (firstP == -1)
						{
							firstP = i;
							firstT = 0;
						}
						else
						{
							secondP = i;
							secondT = 0;
						}
					}
					else if (t.x[1] == x[i] && t.y[1] == y[i])
					{
						if (firstP == -1)
						{
							firstP = i;
							firstT = 1;
						}
						else
						{
							secondP = i;
							secondT = 1;
						}
					}
					else if (t.x[2] == x[i] && t.y[2] == y[i])
					{
						if (firstP == -1)
						{
							firstP = i;
							firstT = 2;
						}
						else
						{
							secondP = i;
							secondT = 2;
						}
					}
					else
					{
					}
				}
				// Fix ordering if first should be last vertex of poly
				if (firstP == 0 && secondP == nVertices - 1)
				{
					firstP = nVertices - 1;
					secondP = 0;
				}

				// Didn't find it
				if (secondP == -1)
					return null;

				// Find tip index on triangle
				int tipT = 0;
				if (tipT == firstT || tipT == secondT)
					tipT = 1;
				if (tipT == firstT || tipT == secondT)
					tipT = 2;

				float[] newx = new float[nVertices + 1];
				float[] newy = new float[nVertices + 1];
				int currOut = 0;
				for (int i = 0; i < nVertices; i++)
				{
					newx[currOut] = x[i];
					newy[currOut] = y[i];
					if (i == firstP)
					{
						++currOut;
						newx[currOut] = t.x[tipT];
						newy[currOut] = t.y[tipT];
					}
					++currOut;
				}

				return new Polygon(newx.ToList<float>(), newy.ToList<float>());
			}

			public Polygon(Polygon p)
			{
				area = p.area;
				areaIsSet = p.areaIsSet;
				for (int i = 0; i < p.nVertices; ++i)
				{
					x.Add(p.x[i]);
					y.Add(p.y[i]);
				}
			}
		};

		public class PolyNode
		{
			public const int MAX_CONNECTED = 32;
			public const float COLLAPSE_DIST_SQR = 1.192092896e-07F*1.192092896e-07F;//0.1f;//1000*FLT_EPSILON*1000*FLT_EPSILON;

			public Vec2 position = new Vec2();
			public PolyNode[] connected = new PolyNode[MAX_CONNECTED];
			public int nConnected = 0;
			public bool visited = false;

			public PolyNode(Vec2 pos)
			{
				position = pos;
			}

			public PolyNode()
			{
			}

			public void AddConnection(PolyNode toMe)
			{
				//.b2Assert(nConnected < MAX_CONNECTED);
				// Ignore duplicate additions
				for (int i=0; i<nConnected; ++i)
				{
					if (connected[i] == toMe)
						return;
				}
				connected[nConnected] = toMe;
				++nConnected;
			}

			public void RemoveConnection(PolyNode fromMe)
			{
				int foundIndex = -1;
				for (int i=0; i<nConnected; ++i)
				{
					if (fromMe == connected[i])
					{//.position == connected[i]->position){
						foundIndex = i;
						break;
					}
				}
				//b2Assert(isFound);
				--nConnected;
				//printf("nConnected: %d\n",nConnected);
				for (int i=foundIndex; i < nConnected; ++i)
				{
					connected[i] = connected[i+1];
				}

			}

			public void RemoveConnectionByIndex(int index)
			{
				--nConnected;
				//printf("New nConnected = %d\n",nConnected);
				for (int i=index; i < nConnected; ++i)
					connected[i] = connected[i+1];
			}

			public bool IsConnectedTo(PolyNode me)
			{
				bool isFound = false;
				for (int i=0; i<nConnected; ++i)
				{
					if (me == connected[i])
					{
						isFound = true;
						break;
					}
				}
				return isFound;

			}

			public PolyNode GetRightestConnection(PolyNode incoming)
			{
				if (nConnected == 0)
					throw new Exception("wat"); // This means the connection graph is inconsistent
				if (nConnected == 1)
				{
					//b2Assert(false);
					// Because of the possibility of collapsing nearby points,
					// we may end up with "spider legs" dangling off of a region.
					// The correct behavior here is to turn around.
					return incoming;
				}
				Vec2 inDir = position - incoming.position;
				float inLength = inDir.Normalize();
				//b2Assert(inLength > FLT_EPSILON);

				PolyNode result = null;
				for (int i=0; i<nConnected; ++i)
				{
					if (connected[i] == incoming)
						continue;

					Vec2 testDir = connected[i].position - position;
					float testLengthSqr = testDir.LengthSquared();
					testDir.Normalize();
					/*
					if (testLengthSqr < COLLAPSE_DIST_SQR) {
							printf("Problem with connection %d\n",i);
							printf("This node has %d connections\n",nConnected);
							printf("That one has %d\n",connected[i]->nConnected);
							if (this == connected[i]) printf("This points at itself.\n");
					}*/
					//b2Assert(testLengthSqr >= COLLAPSE_DIST_SQR);
					float myCos = inDir.Dot(testDir);
					float mySin = inDir.Cross(testDir);
					if (result != null)
					{
						Vec2 resultDir = result.position - position;
						resultDir.Normalize();
						float resCos = inDir.Dot(resultDir);
						float resSin = inDir.Cross(resultDir);
						if (Statics.IsRighter(mySin, myCos, resSin, resCos))
							result = connected[i];
					}
					else
						result = connected[i];
				}
				if (Polygon.ReportErrors && result == null)
				{
					//printf("nConnected = %d\n", nConnected);
					//for (int32 i=0; i<nConnected; ++i)
					//{
					//	printf("connected[%d] @ %d\n", i, 0);//(int)connected[i]);
					//}
					throw new Exception("wat");
				}
				//b2Assert(result);

				return result;
			}

			public PolyNode GetRightestConnection(ref Vec2 incomingDir)
			{
				Vec2 diff = position - incomingDir;
				PolyNode temp = new PolyNode(diff);
				PolyNode res = GetRightestConnection(temp);
				//b2Assert(res);
				return res;
			}
		};


		public static class Statics
		{
			//If you're using 1.4.3, b2_toiSlop won't exist, so set this equal to 0
			public const float toiSlop = 0.0f;
			public const float FLT_EPSILON = 1.192092896e-07F;
			public const float FLT_MAX = 3.4028235E38F;

			/*
			 * Check if the lines a0->a1 and b0->b1 cross.
			 * If they do, intersectionPoint will be filled
			 * with the point of crossing.
			 *
			 * Grazing lines should not return true.
			 */
			public static bool intersect(Vec2 a0, Vec2 a1,
									   Vec2 b0, Vec2 b1,
									   ref Vec2 intersectionPoint)
			{

				if (a0 == b0 || a0 == b1 || a1 == b0 || a1 == b1) return false;
				float x1 = a0.X; float y1 = a0.Y;
				float x2 = a1.X; float y2 = a1.Y;
				float x3 = b0.X; float y3 = b0.Y;
				float x4 = b1.X; float y4 = b1.Y;

				//AABB early exit
				if (Math.Max(x1, x2) < Math.Min(x3, x4) || Math.Max(x3, x4) < Math.Min(x1, x2)) return false;
				if (Math.Max(y1, y2) < Math.Min(y3, y4) || Math.Max(y3, y4) < Math.Min(y1, y2)) return false;

				float ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
				float ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
				float denom = (y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1);
				if (Math.Abs(denom) < FLT_EPSILON)
				{
					//Lines are too close to parallel to call
					return false;
				}
				ua /= denom;
				ub /= denom;

				if ((0 < ua) && (ua < 1) && (0 < ub) && (ub < 1))
				{
					//if (intersectionPoint){
					intersectionPoint.X = (x1 + ua * (x2 - x1));
					intersectionPoint.Y = (y1 + ua * (y2 - y1));
					//}
					//printf("%f, %f -> %f, %f crosses %f, %f -> %f, %f\n",x1,y1,x2,y2,x3,y3,x4,y4);
					return true;
				}

				return false;
			}

			/*
			 * True if line from a0->a1 intersects b0->b1
			 */
			public static bool intersect(Vec2 a0, Vec2 a1,
									   Vec2 b0, Vec2 b1)
			{
				Vec2 myVec = new Vec2();
				return intersect(a0, a1, b0, b1, ref myVec);
			}

			/*
			 * Pulled from b2Shape.cpp, assertions removed
			 */
			public static Vec2 PolyCentroid(List<Vec2> vs)
			{
				Vec2 c = new Vec2(0.0f, 0.0f);
				float area = 0.0f;

				const float inv3 = 1.0f / 3.0f;
				Vec2 pRef = new Vec2(0.0f, 0.0f);
				for (int i = 0; i < vs.Count; ++i)
				{
					// Triangle vertices.
					Vec2 p1 = pRef;
					Vec2 p2 = vs[i];
					Vec2 p3 = i + 1 < vs.Count ? vs[i+1] : vs[0];

					Vec2 e1 = p2 - p1;
					Vec2 e2 = p3 - p1;

					float D = e1.Cross(e2);

					float triangleArea = 0.5f * D;
					area += triangleArea;

					// Area weighted centroid
					c += triangleArea * inv3 * (p1 + p2 + p3);
				}

				// Centroid
				c *= 1.0f / area;
				return c;
			}


			/**
			 * Finds and fixes "pinch points," points where two polygon
			 * vertices are at the same point.
			 *
			 * If a pinch point is found, pin is broken up into poutA and poutB
			 * and true is returned; otherwise, returns false.
			 *
			 * Mostly for internal use.
			 */
			public static bool ResolvePinchPoint(Polygon pin, ref Polygon poutA, ref Polygon poutB)
			{
				if (pin.nVertices < 3) return false;
				float tol = .001f;
				bool hasPinchPoint = false;
				int pinchIndexA = -1;
				int pinchIndexB = -1;
				for (int i=0; i<pin.nVertices; ++i)
				{
					for (int j=i+1; j<pin.nVertices; ++j)
					{
						//Don't worry about pinch points where the points
						//are actually just dupe neighbors
						if (Math.Abs(pin.x[i]-pin.x[j])<tol&&Math.Abs(pin.y[i]-pin.y[j])<tol&&j!=i+1)
						{
							pinchIndexA = i;
							pinchIndexB = j;
							//printf("pinch: %f, %f == %f, %f\n",pin.X[i],pin.Y[i],pin.X[j],pin.Y[j]);
							//printf("at indexes %d, %d\n",i,j);
							hasPinchPoint = true;
							break;
						}
					}
					if (hasPinchPoint) break;
				}
				if (hasPinchPoint)
				{
					//printf("Found pinch point\n");
					int sizeA = pinchIndexB - pinchIndexA;
					if (sizeA == pin.nVertices) return false;//has dupe points at wraparound, not a problem here
					float[] xA = new float[sizeA];
					float[] yA = new float[sizeA];
					for (int i=0; i < sizeA; ++i)
					{
						int ind = remainder(pinchIndexA+i, pin.nVertices);
						xA[i] = pin.x[ind];
						yA[i] = pin.y[ind];
					}
					poutA = new Polygon(xA.ToList<float>(), yA.ToList<float>());

					int sizeB = pin.nVertices - sizeA;
					float[] xB = new float[sizeB];
					float[] yB = new float[sizeB];
					for (int i=0; i<sizeB; ++i)
					{
						int ind = remainder(pinchIndexB+i, pin.nVertices);
						xB[i] = pin.x[ind];
						yB[i] = pin.y[ind];
					}
					poutB = new Polygon(xB.ToList<float>(), yB.ToList<float>());

					//printf("Size of a: %d, size of b: %d\n",sizeA,sizeB);
				}
				return hasPinchPoint;
			}

			/**
			 * Triangulates a polygon using simple ear-clipping algorithm. Returns
			 * size of Triangle array unless the polygon can't be triangulated.
			 * This should only happen if the polygon self-intersects,
			 * though it will not _always_ return null for a bad polygon - it is the
			 * caller's responsibility to check for self-intersection, and if it
			 * doesn't, it should at least check that the return value is non-null
			 * before using. You're warned!
				 *
				 * Triangles may be degenerate, especially if you have identical points
				 * in the input to the algorithm.  Check this before you use them.
			 *
			 * This is totally unoptimized, so for large polygons it should not be part
			 * of the simulation loop.
			 *
			 * Returns:
			 * -1 if algorithm fails (self-intersection most likely)
			 * 0 if there are not enough vertices to triangulate anything.
			 * Number of triangles if triangulation was successful.
			 *
			 * results will be filled with results - ear clipping always creates vNum - 2
			 * or fewer (due to pinch point polygon snipping), so allocate an array of
				 * this size.
			 */

			public static int TriangulatePolygon(List<float> xv, List<float> yv, List<Triangle> results)
			{
				if (xv.Count < 3)
					return 0;

				//Recurse and split on pinch points
				Polygon pA = new Polygon(), pB = new Polygon();
				Polygon pin = new Polygon(xv, yv);
				if (ResolvePinchPoint(pin, ref pA, ref pB))
				{
					List<Triangle> mergeA = new List<Triangle>();
					List<Triangle> mergeB = new List<Triangle>();
					int nA = TriangulatePolygon(pA.x, pA.y, mergeA);
					int nB = TriangulatePolygon(pB.x, pB.y, mergeB);
					if (nA==-1 || nB==-1)
						return -1;
					for (int i=0; i<nA; ++i)
						results.Add(mergeA[i]);
					for (int i=0; i<nB; ++i)
						results.Add(mergeB[i]);
					return (nA+nB);
				}

				int vNum = xv.Count;
				List<Triangle> buffer = new List<Triangle>();
				float[] xrem = new float[vNum];
				float[] yrem = new float[vNum];
				for (int i = 0; i < vNum; ++i)
				{
					xrem[i] = xv[i];
					yrem[i] = yv[i];
				}

				int xremLength = vNum;

				while (vNum > 3)
				{
					// Find an ear
					int earIndex = -1;
					//float earVolume = -1.0f;
					float earMaxMinCross = -10.0f;
					for (int i = 0; i < vNum; ++i)
					{
						if (IsEar(i, xrem, yrem))
						{
							int lower = remainder(i-1, vNum);
							int upper = remainder(i+1, vNum);
							Vec2 d1 = new Vec2(xrem[upper]-xrem[i], yrem[upper]-yrem[i]);
							Vec2 d2 = new Vec2(xrem[i]-xrem[lower], yrem[i]-yrem[lower]);
							Vec2 d3 = new Vec2(xrem[lower]-xrem[upper], yrem[lower]-yrem[upper]);

							d1.Normalize();
							d2.Normalize();
							d3.Normalize();
							float cross12 = Math.Abs(d1.Cross(d2));
							float cross23 = Math.Abs(d2.Cross(d3));
							float cross31 = Math.Abs(d3.Cross(d1));
							//Find the maximum minimum angle
							float minCross = Math.Min(cross12, Math.Min(cross23, cross31));
							if (minCross > earMaxMinCross)
							{
								earIndex = i;
								earMaxMinCross = minCross;
							}

							/*//This bit chooses the ear with greatest volume first
							float testVol = b2Abs( d1.X*d2.Y-d2.X*d1.Y );
							if (testVol > earVolume){
									earIndex = i;
									earVolume = testVol;
							}*/
						}
					}

					// If we still haven't found an ear, we're screwed.
					// Note: sometimes this is happening because the
					// remaining points are collinear.  Really these
					// should just be thrown out without halting triangulation.
					if (earIndex == -1)
					{
						if (Polygon.ReportErrors)
						{
							Polygon dump = new Polygon(xrem.ToList<float>(), yrem.ToList<float>());
							//printf("Couldn't find an ear, dumping remaining poly:\n");
							//printf("Please submit this dump to ewjordan at Box2d forums\n");
							throw new Exception("Couldn't find ear");
						}

						/*for (int i = 0; i < buffer.Count; i++)
							results.Add(buffer[i]);

						if (buffer.Count > 0)
							return buffer.Count;
						else
							return -1;*/
					}

					// Clip off the ear:
					// - remove the ear tip from the list


					--vNum;
					float[] newx = new float[vNum];
					float[] newy = new float[vNum];
					int currDest = 0;
					for (int i = 0; i < vNum; ++i)
					{
						if (currDest == earIndex) ++currDest;
						newx[i] = xrem[currDest];
						newy[i] = yrem[currDest];
						++currDest;
					}

					// - add the clipped triangle to the triangle list
					int under = (earIndex == 0) ? (vNum) : (earIndex - 1);
					int over = (earIndex == vNum) ? 0 : (earIndex + 1);
					Triangle toAdd = new Triangle(xrem[earIndex], yrem[earIndex], xrem[over], yrem[over], xrem[under], yrem[under]);
					buffer.Add(toAdd);

					// - replace the old list with the new one
					xrem = newx;
					yrem = newy;
				}

				Triangle ntoAdd = new Triangle(xrem[1], yrem[1], xrem[2], yrem[2],
																		  xrem[0], yrem[0]);
				buffer.Add(ntoAdd);

				if (!(buffer.Count == xremLength-2))
					throw new Exception("wat");

				for (int i = 0; i < buffer.Count; i++)
					results.Add(buffer[i]);

				return buffer.Count;
			}

			/**
				 * Turns a list of triangles into a list of convex polygons. Very simple
			 * method - start with a seed triangle, keep adding triangles to it until
			 * you can't add any more without making the polygon non-convex.
			 *
			 * Returns an integer telling how many polygons were created.  Will fill
			 * polys array up to polysLength entries, which may be smaller or larger
			 * than the return value.
			 *
			 * Takes O(N*P) where P is the number of resultant polygons, N is triangle
			 * count.
			 *
			 * The final polygon list will not necessarily be minimal, though in
			 * practice it works fairly well.
			 */
			public static int PolygonizeTriangles(List<Triangle> triangulated, List<Polygon> polys, int polysLength)
			{
				int polyIndex = 0;

				if (triangulated.Count <= 0)
					return 0;
				else
				{
					int[] covered = new int[triangulated.Count];
					for (int i = 0; i < triangulated.Count; ++i)
					{
						covered[i] = 0;
						//Check here for degenerate triangles
						if (((triangulated[i].x[0] == triangulated[i].x[1]) && (triangulated[i].y[0] == triangulated[i].y[1]))
												 || ((triangulated[i].x[1] == triangulated[i].x[2]) && (triangulated[i].y[1] == triangulated[i].y[2]))
												 || ((triangulated[i].x[0] == triangulated[i].x[2]) && (triangulated[i].y[0] == triangulated[i].y[2])))
						{
							covered[i] = 1;
						}
					}

					bool notDone = true;
					while (notDone)
					{
						int currTri = -1;
						for (int i = 0; i < triangulated.Count; ++i)
						{
							if (covered[i] != 0)
								continue;
							currTri = i;
							break;
						}
						if (currTri == -1)
						{
							notDone = false;
						}
						else
						{
							Polygon poly = new Polygon(triangulated[currTri]);
							covered[currTri] = 1;
							int index = 0;
							for (int i = 0; i < 2*triangulated.Count; ++i, ++index)
							{
								while (index >= triangulated.Count) index -= triangulated.Count;
								if (covered[index] != 0)
								{
									continue;
								}
								Polygon newP = poly.Add(triangulated[index]);
								if (newP == null)
									continue;

								if (newP.nVertices > Polygon.maxVerticesPerPolygon)
								{
									newP = null;
									continue;
								}
								if (newP.IsConvex())
								{ //Or should it be IsUsable?  Maybe re-write IsConvex to apply the angle threshold from Box2d
									poly.Set(newP);
									newP = null;
									covered[index] = 1;
								}
								else
									newP = null;

							}
							if (polyIndex < polysLength)
							{
								poly.MergeParallelEdges(Box2DSettings.b2_angularSlop);
								//If identical points are present, a triangle gets
								//borked by the MergeParallelEdges function, hence
								//the vertex number check
								if (poly.nVertices >= 3) polys.Add(poly);
								//else printf("Skipping corrupt poly\n");
							}
							if (poly.nVertices >= 3) polyIndex++; //Must be outside (polyIndex < polysLength) test
						}
						//printf("MEMCHECK: %d\n",_CrtCheckMemory());
					}
				}
				return polyIndex;
			}

			/**
				 * Checks if vertex i is the tip of an ear in polygon defined by xv[] and
			 * yv[].
				 *
				 * Assumes clockwise orientation of polygon...ick
			 */
			public static bool IsEar(int i, float[] xv, float[] yv)
			{
				float dx0, dy0, dx1, dy1;
				dx0 = dy0 = dx1 = dy1 = 0;
				if (i >= xv.Length || i < 0 || xv.Length < 3)
				{
					return false;
				}
				int upper = i + 1;
				int lower = i - 1;
				if (i == 0)
				{
					dx0 = xv[0] - xv[xv.Length - 1];
					dy0 = yv[0] - yv[xv.Length - 1];
					dx1 = xv[1] - xv[0];
					dy1 = yv[1] - yv[0];
					lower = xv.Length - 1;
				}
				else if (i == xv.Length - 1)
				{
					dx0 = xv[i] - xv[i - 1];
					dy0 = yv[i] - yv[i - 1];
					dx1 = xv[0] - xv[i];
					dy1 = yv[0] - yv[i];
					upper = 0;
				}
				else
				{
					dx0 = xv[i] - xv[i - 1];
					dy0 = yv[i] - yv[i - 1];
					dx1 = xv[i + 1] - xv[i];
					dy1 = yv[i + 1] - yv[i];
				}
				float cross = dx0 * dy1 - dx1 * dy0;
				if (cross > 0)
					return false;
				Triangle myTri = new Triangle(xv[i], yv[i], xv[upper], yv[upper],
																		  xv[lower], yv[lower]);
				for (int j = 0; j < xv.Length; ++j)
				{
					if (j == i || j == lower || j == upper)
						continue;
					if (myTri.IsInside(xv[j], yv[j]))
						return false;
				}
				return true;
			}

			public static void ReversePolygon(Polygon p)
			{
				ReversePolygon(p.x, p.y);
			}

			public static void ReversePolygon(List<float> x, List<float> y)
			{
				int n = x.Count;
				if (n == 1)
					return;
				int low = 0;
				int high = n - 1;
				while (low < high)
				{
					float buffer = x[low];
					x[low] = x[high];
					x[high] = buffer;
					buffer = y[low];
					y[low] = y[high];
					y[high] = buffer;
					++low;
					--high;
				}
			}


			/**
				 * Decomposes a non-convex polygon into a number of convex polygons, up
			 * to maxPolys (remaining pieces are thrown out, but the total number
				 * is returned, so the return value can be greater than maxPolys).
			 *
			 * Each resulting polygon will have no more than maxVerticesPerPolygon
			 * vertices (set to b2MaxPolyVertices by default, though you can change
				 * this).
			 *
			 * Returns -1 if operation fails (usually due to self-intersection of
				 * polygon).
			 */
			public static int DecomposeConvex(Polygon p, List<Polygon> results, int maxPolys)
			{
				List<Triangle> triangulated;
				Triangulize(p, out triangulated);

				if (triangulated == null)
					return -1;
				
				return PolygonizeTriangles(triangulated, results, maxPolys);
			}

			public static int Triangulize(Polygon p, out List<Triangle> triangulated)
			{
				triangulated = null;

				if (p.nVertices < 3)
					return 0;

				triangulated = new List<Triangle>();
				int nTri;
				if (p.IsCCW())
				{
					//printf("It is ccw \n");
					Polygon tempP = new Polygon();
					tempP.Set(p);
					ReversePolygon(tempP.x, tempP.y);
					nTri = TriangulatePolygon(tempP.x, tempP.y, triangulated);
					//                      ReversePolygon(p->x, p->y, p->nVertices); //reset orientation
				}
				else
				{
					//printf("It is not ccw \n");
					nTri = TriangulatePolygon(p.x, p.y, triangulated);
				}
				if (nTri < 1)
				{
					//Still no luck?  Oh well...
					return -1;
				}
				return nTri;
			}

			/**
				 * Decomposes a polygon into convex polygons and adds all pieces to a b2BodyDef
			 * using a prototype b2PolyDef. All fields of the prototype are used for every
			 * shape except the vertices (friction, restitution, density, etc).
			 *
			 * If you want finer control, you'll have to add everything by hand.
			 *
			 * This is the simplest method to add a complicated polygon to a body.
				 *
				 * Until Box2D's b2BodyDef behavior changes, this method returns a pointer to
				 * a heap-allocated array of b2PolyDefs, which must be deleted by the user
				 * after the b2BodyDef is added to the world.
			 */
			/*	public static void DecomposeConvexAndAddTo(b2Polygon* p, b2Body* bd, b2FixtureDef* prototype) {

						if (p->nVertices < 3) return;
						b2Polygon* decomposed = new b2Polygon[p->nVertices - 2]; //maximum number of polys
						int nPolys = DecomposeConvex(p, decomposed, p->nVertices - 2);
				//              printf("npolys: %d",nPolys);
								b2FixtureDef* pdarray = new b2FixtureDef[2*p->nVertices];//extra space in case of splits
								int extra = 0;
						for (int i = 0; i < nPolys; ++i) {
							b2FixtureDef* toAdd = &pdarray[i+extra];
										 *toAdd = *prototype;
										 //decomposed[i].print();
										b2Polygon curr = decomposed[i];
										//TODO ewjordan: move this triangle handling to a better place so that
										//it happens even if this convenience function is not called.
										if (curr.nVertices == 3){
														//Check here for near-parallel edges, since we can't
														//handle this in merge routine
														for (int j=0; j<3; ++j){
																int lower = (j == 0) ? (curr.nVertices - 1) : (j - 1);
																int middle = j;
																int upper = (j == curr.nVertices - 1) ? (0) : (j + 1);
																float dx0 = curr.X[middle] - curr.X[lower]; float dy0 = curr.Y[middle] - curr.Y[lower];
																float dx1 = curr.X[upper] - curr.X[middle]; float dy1 = curr.Y[upper] - curr.Y[middle];
																float norm0 = sqrtf(dx0*dx0+dy0*dy0); float norm1 = sqrtf(dx1*dx1+dy1*dy1);
																if ( !(norm0 > 0.0f && norm1 > 0.0f) ) {
																		//Identical points, don't do anything!
																		goto Skip;
																}
																dx0 /= norm0; dy0 /= norm0;
																dx1 /= norm1; dy1 /= norm1;
																float cross = dx0 * dy1 - dx1 * dy0;
																float dot = dx0*dx1 + dy0*dy1;
																if (fabs(cross) < b2_angularSlop && dot > 0) {
																		//Angle too close, split the triangle across from this point.
																		//This is guaranteed to result in two triangles that satify
																		//the tolerance (one of the angles is 90 degrees)
																		float dx2 = curr.X[lower] - curr.X[upper]; float dy2 = curr.Y[lower] - curr.Y[upper];
																		float norm2 = sqrtf(dx2*dx2+dy2*dy2);
																		if (norm2 == 0.0f) {
																				goto Skip;
																		}
																		dx2 /= norm2; dy2 /= norm2;
																		float thisArea = curr.GetArea();
																		float thisHeight = 2.0f * thisArea / norm2;
																		float buffer2 = dx2;
																		dx2 = dy2; dy2 = -buffer2;
																		//Make two new polygons
																		//printf("dx2: %f, dy2: %f, thisHeight: %f, middle: %d\n",dx2,dy2,thisHeight,middle);
																		float newX1[3] = { curr.X[middle]+dx2*thisHeight, curr.X[lower], curr.X[middle] };
																		float newY1[3] = { curr.Y[middle]+dy2*thisHeight, curr.Y[lower], curr.Y[middle] };
																		float newX2[3] = { newX1[0], curr.X[middle], curr.X[upper] };
																		float newY2[3] = { newY1[0], curr.Y[middle], curr.Y[upper] };
																		b2Polygon p1(newX1,newY1,3);
																		b2Polygon p2(newX2,newY2,3);
																		if (p1.IsUsable()){
																				p1.AddTo(*toAdd);
                                                               
                                                               
																				bd->CreateFixture(toAdd);
																				++extra;
																		} else if (B2_POLYGON_REPORT_ERRORS){
																				printf("Didn't add unusable polygon.  Dumping vertices:\n");
																				p1.print();
																		}
																		if (p2.IsUsable()){
																				p2.AddTo(pdarray[i+extra]);
                                                               
																				bd->CreateFixture(toAdd);
																		} else if (B2_POLYGON_REPORT_ERRORS){
																				printf("Didn't add unusable polygon.  Dumping vertices:\n");
																				p2.print();
																		}
																		goto Skip;
																}
														}

										}
										if (decomposed[i].IsUsable()){
												decomposed[i].AddTo(*toAdd);
                               
												bd->CreateFixture((const b2FixtureDef*)toAdd);
										} else if (B2_POLYGON_REPORT_ERRORS){
												printf("Didn't add unusable polygon.  Dumping vertices:\n");
												decomposed[i].print();
										}
				Skip:
										;
						}
								delete[] pdarray;
						delete[] decomposed;
								return;// pdarray; //needs to be deleted after body is created
				}*/


			/**
				 * Find the convex hull of a point cloud using "Gift-wrap" algorithm - start
			 * with an extremal point, and walk around the outside edge by testing
			 * angles.
			 *
			 * Runs in O(N*S) time where S is number of sides of resulting polygon.
			 * Worst case: point cloud is all vertices of convex polygon -> O(N^2).
			 *
			 * There may be faster algorithms to do this, should you need one -
			 * this is just the simplest. You can get O(N log N) expected time if you
			 * try, I think, and O(N) if you restrict inputs to simple polygons.
			 *
			 * Returns null if number of vertices passed is less than 3.
			 *
				 * Results should be passed through convex decomposition afterwards
				 * to ensure that each shape has few enough points to be used in Box2d.
				 *
			 * FIXME?: May be buggy with colinear points on hull. Couldn't find a test
			 * case that resulted in wrong behavior. If one turns up, the solution is to
			 * supplement angle check with an equality resolver that always picks the
			 * longer edge. I think the current solution is working, though it sometimes
			 * creates an extra edge along a line.
			 */

			public static Polygon ConvexHull(List<Vec2> v)
			{
				float[] cloudX = new float[v.Count];
				float[] cloudY = new float[v.Count];
				for (int i = 0; i < v.Count; ++i)
				{
					cloudX[i] = v[i].X;
					cloudY[i] = v[i].Y;
				}
				return ConvexHull(cloudX, cloudY);
			}

			public static Polygon ConvexHull(float[] cloudX, float[] cloudY)
			{
				int nVert = cloudX.Length;
				int[] edgeList = new int[nVert];
				int numEdges = 0;

				float minY = Statics.FLT_MAX;
				int minYIndex = nVert;
				for (int i = 0; i < nVert; ++i)
				{
					if (cloudY[i] < minY)
					{
						minY = cloudY[i];
						minYIndex = i;
					}
				}

				int startIndex = minYIndex;
				int winIndex = -1;
				float dx = -1.0f;
				float dy = 0.0f;
				while (winIndex != minYIndex)
				{
					float newdx = 0.0f;
					float newdy = 0.0f;
					float maxDot = -2.0f;
					float nrm;
					for (int i = 0; i < nVert; ++i)
					{
						if (i == startIndex)
							continue;
						newdx = cloudX[i] - cloudX[startIndex];
						newdy = cloudY[i] - cloudY[startIndex];
						nrm = (float)Math.Sqrt(newdx * newdx + newdy * newdy);
						nrm = (nrm == 0.0f) ? 1.0f : nrm;
						newdx /= nrm;
						newdy /= nrm;

						//Cross and dot products act as proxy for angle
						//without requiring inverse trig.
						//FIXED: don't need cross test
						//float newCross = newdx * dy - newdy * dx;
						float newDot = newdx * dx + newdy * dy;
						if (newDot > maxDot)
						{//newCross >= 0.0f && newDot > maxDot) {
							maxDot = newDot;
							winIndex = i;
						}
					}
					edgeList[numEdges++] = winIndex;
					dx = cloudX[winIndex] - cloudX[startIndex];
					dy = cloudY[winIndex] - cloudY[startIndex];
					nrm = (float)Math.Sqrt(dx * dx + dy * dy);
					nrm = (nrm == 0.0f) ? 1.0f : nrm;
					dx /= nrm;
					dy /= nrm;
					startIndex = winIndex;
				}

				float[] xres = new float[numEdges];
				float[] yres = new float[numEdges];
				for (int i = 0; i < numEdges; i++)
				{
					xres[i] = cloudX[edgeList[i]];
					yres[i] = cloudY[edgeList[i]];
					//("%f, %f\n",xres[i],yres[i]);
				}

				Polygon returnVal = new Polygon(xres.ToList<float>(), yres.ToList<float>());


				returnVal.MergeParallelEdges(Box2DSettings.b2_angularSlop);
				return returnVal;
			}


			/*
			 * Given sines and cosines, tells if A's angle is less than B's on -Pi, Pi
			 * (in other words, is A "righter" than B)
			 */
			public static bool IsRighter(float sinA, float cosA, float sinB, float cosB)
			{
				if (sinA < 0)
				{
					if (sinB > 0 || cosA <= cosB) return true;
					else return false;
				}
				else
				{
					if (sinB < 0 || cosA <= cosB) return false;
					else return true;
				}
			}

			//Fix for obnoxious behavior for the % operator for negative numbers...
			public static int remainder(int x, int modulus)
			{
				int rem = x % modulus;
				while (rem < 0)
				{
					rem += modulus;
				}
				return rem;
			}

			/*
			Method:
			Start at vertex with minimum y (pick maximum x one if there are two).  
			We aim our "lastDir" vector at (1.0, 0)
			We look at the two rays going off from our start vertex, and follow whichever
			has the smallest angle (in -Pi -> Pi) wrt lastDir ("rightest" turn)

			Loop until we hit starting vertex:

			We add our current vertex to the list.
			We check the seg from current vertex to next vertex for intersections
			  - if no intersections, follow to next vertex and continue
			  - if intersections, pick one with minimum distance
				- if more than one, pick one with "rightest" next point (two possibilities for each)

			*/

			public static Polygon TraceEdge(Polygon p)
			{
				List<PolyNode> nodes = new List<PolyNode>();//overkill, but sufficient (order of mag. is right)

				//Add base nodes (raw outline)
				for (int i=0; i < p.nVertices; ++i)
				{
					Vec2 pos = new Vec2(p.x[i], p.y[i]);
					nodes.Add(new PolyNode());
					nodes[i].position = pos;
					int iplus = (i==p.nVertices-1)?0:i+1;
					int iminus = (i==0)?p.nVertices-1:i-1;
					nodes[i].AddConnection(nodes[iplus]);
					nodes[i].AddConnection(nodes[iminus]);
				}

				//Process intersection nodes - horribly inefficient
				bool dirty = true;
				int counter = 0;
				while (dirty)
				{
					dirty = false;
					for (int i=0; i < nodes.Count; ++i)
					{
						for (int j=0; j < nodes[i].nConnected; ++j)
						{
							for (int k=0; k < nodes.Count; ++k)
							{
								if (k==i || nodes[k] == nodes[i].connected[j]) continue;
								for (int l=0; l < nodes[k].nConnected; ++l)
								{

									if (nodes[k].connected[l] == nodes[i].connected[j] ||
																 nodes[k].connected[l] == nodes[i]) continue;
									//Check intersection
									Vec2 intersectPt = new Vec2();
									//if (counter > 100) printf("checking intersection: %d, %d, %d, %d\n",i,j,k,l);
									bool crosses = intersect(nodes[i].position, nodes[i].connected[j].position,
																					 nodes[k].position, nodes[k].connected[l].position,
																					 ref intersectPt);
									if (crosses)
									{
										/*if (counter > 100) {
												printf("Found crossing at %f, %f\n",intersectPt.X, intersectPt.Y);
												printf("Locations: %f,%f - %f,%f | %f,%f - %f,%f\n",
																				nodes[i].position.X, nodes[i].position.Y,
																				nodes[i].connected[j]->position.X, nodes[i].connected[j]->position.Y,
																				nodes[k].position.X,nodes[k].position.Y,
																				nodes[k].connected[l]->position.X,nodes[k].connected[l]->position.Y);
												printf("Memory addresses: %d, %d, %d, %d\n",(int)&nodes[i],(int)nodes[i].connected[j],(int)&nodes[k],(int)nodes[k].connected[l]);
										}*/
										dirty = true;
										//Destroy and re-hook connections at crossing point
										PolyNode connj = nodes[i].connected[j];
										PolyNode connl = nodes[k].connected[l];
										nodes[i].connected[j].RemoveConnection(nodes[i]);
										nodes[i].RemoveConnection(connj);
										nodes[k].connected[l].RemoveConnection(nodes[k]);
										nodes[k].RemoveConnection(connl);
										int nNodes = nodes.Count;
										nodes.Add(new PolyNode());
										nodes[nNodes] = new PolyNode(intersectPt);
										nodes[nNodes].AddConnection(nodes[i]);
										nodes[i].AddConnection(nodes[nNodes]);
										nodes[nNodes].AddConnection(nodes[k]);
										nodes[k].AddConnection(nodes[nNodes]);
										nodes[nNodes].AddConnection(connj);
										connj.AddConnection(nodes[nNodes]);
										nodes[nNodes].AddConnection(connl);
										connl.AddConnection(nodes[nNodes]);
										++nNodes;
										goto SkipOut;
									}
								}
							}
						}
					}
					SkipOut:
					++counter;
					//if (counter > 100) printf("Counter: %d\n",counter);
				}

				/*
				// Debugging: check for connection consistency
				for (int i=0; i<nNodes; ++i) {
						int nConn = nodes[i].nConnected;
						for (int j=0; j<nConn; ++j) {
								if (nodes[i].connected[j]->nConnected == 0) b2Assert(false);
								b2PolyNode* connect = nodes[i].connected[j];
								bool found = false;
								for (int k=0; k<connect->nConnected; ++k) {
										if (connect->connected[k] == &nodes[i]) found = true;
								}
								b2Assert(found);
						}
				}*/

				//Collapse duplicate points
				bool foundDupe = true;
				int nActive = nodes.Count;
				while (foundDupe)
				{
					foundDupe = false;
					for (int i=0; i < nodes.Count; ++i)
					{
						if (nodes[i].nConnected == 0) continue;
						for (int j=i+1; j < nodes.Count; ++j)
						{
							if (nodes[j].nConnected == 0) continue;
							Vec2 diff = nodes[i].position - nodes[j].position;
							if (diff.LengthSquared() <= PolyNode.COLLAPSE_DIST_SQR)
							{
								if (nActive <= 3) return new Polygon();
								//printf("Found dupe, %d left\n",nActive);
								--nActive;
								foundDupe = true;
								PolyNode inode = nodes[i];
								PolyNode jnode = nodes[j];
								//Move all of j's connections to i, and orphan j
								int njConn = jnode.nConnected;
								for (int k=0; k < njConn; ++k)
								{
									PolyNode knode = jnode.connected[k];
									//b2Assert(knode != jnode);
									if (knode != inode)
									{
										inode.AddConnection(knode);
										knode.AddConnection(inode);
									}
									knode.RemoveConnection(jnode);
									//printf("knode %d on node %d now has %d connections\n",k,j,knode->nConnected);
									//printf("Found duplicate point.\n");
								}
								//printf("Orphaning node at address %d\n",(int)jnode);
								//for (int k=0; k<njConn; ++k) {
								//      if (jnode->connected[k]->IsConnectedTo(*jnode)) printf("Problem!!!\n");
								//}
								/*
								for (int k=0; k < njConn; ++k){
										jnode->RemoveConnectionByIndex(k);
								}*/
								jnode.nConnected = 0;
							}
						}
					}
				}

				/*
				// Debugging: check for connection consistency
				for (int i=0; i<nNodes; ++i) {
						int nConn = nodes[i].nConnected;
						printf("Node %d has %d connections\n",i,nConn);
						for (int j=0; j<nConn; ++j) {
								if (nodes[i].connected[j]->nConnected == 0) {
										printf("Problem with node %d connection at address %d\n",i,(int)(nodes[i].connected[j]));
										b2Assert(false);
								}
								b2PolyNode* connect = nodes[i].connected[j];
								bool found = false;
								for (int k=0; k<connect->nConnected; ++k) {
										if (connect->connected[k] == &nodes[i]) found = true;
								}
								if (!found) printf("Connection %d (of %d) on node %d (of %d) did not have reciprocal connection.\n",j,nConn,i,nNodes);
								b2Assert(found);
						}
				}//*/


				//Now walk the edge of the list

				//Find node with minimum y value (max x if equal)
				float minY = FLT_MAX;
				float maxX = -FLT_MAX;
				int minYIndex = -1;
				for (int i = 0; i < nodes.Count; ++i)
				{
					if (nodes[i].position.Y < minY && nodes[i].nConnected > 1)
					{
						minY = nodes[i].position.Y;
						minYIndex = i;
						maxX = nodes[i].position.X;
					}
					else if (nodes[i].position.Y == minY && nodes[i].position.X > maxX && nodes[i].nConnected > 1)
					{
						minYIndex = i;
						maxX = nodes[i].position.X;
					}
				}

				Vec2 origDir = new Vec2(1.0f, 0.0f);
				List<Vec2> resultVecs = new List<Vec2>();// nodes may be visited more than once, unfortunately - change to growable array!
				PolyNode currentNode = nodes[minYIndex];
				PolyNode startNode = currentNode;
				//b2Assert(currentNode->nConnected > 0);
				PolyNode nextNode = currentNode.GetRightestConnection(ref origDir);
				if (nextNode == null) goto CleanUp; // Borked, clean up our mess and return
				resultVecs[0] = startNode.position;
				while (nextNode != startNode)
				{
					resultVecs.Add(nextNode.position);
					PolyNode oldNode = currentNode;
					currentNode = nextNode;
					//printf("Old node connections = %d; address %d\n",oldNode->nConnected, (int)oldNode);
					//printf("Current node connections = %d; address %d\n",currentNode->nConnected, (int)currentNode);
					nextNode = currentNode.GetRightestConnection(oldNode);
					if (nextNode == null) goto CleanUp; // There was a problem, so jump out of the loop and use whatever garbage we've generated so far
					//printf("nextNode address: %d\n",(int)nextNode);
				}

				CleanUp:

				float[] xres = new float[resultVecs.Count];
				float[] yres = new float[resultVecs.Count];
				for (int i=0; i<resultVecs.Count; ++i)
				{
					xres[i] = resultVecs[i].X;
					yres[i] = resultVecs[i].Y;
				}
				return new Polygon(xres.ToList<float>(), yres.ToList<float>());
			}

		}
	}
}
