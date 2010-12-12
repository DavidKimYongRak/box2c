// Box2C.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "Box2C.h"

BOX2C_API cb2world *b2world_constructor (cb2vec2 gravity, bool doSleep)
{
	return new b2World(gravity, doSleep);
}

BOX2C_API void b2world_destroy (cb2world *world)
{
	delete world;
}

struct cb2destructionlistener
{
	typedef void (*saygoodbye_joint) (cb2joint *joint);
	typedef void (*saygoodbye_fixture) (cb2fixture *joint);

	saygoodbye_joint saygoodbye_joint_callback;
	saygoodbye_fixture saygoodbye_fixture_callback;
};

class cb2DestructionListenerWrapper : public b2DestructionListener
{
public:
	cb2DestructionListenerWrapper (cb2destructionlistener listener) :
	  listener(listener)
	  {
	  }

	cb2destructionlistener listener;

	void SayGoodbye(b2Fixture* fixture)
	{
		listener.saygoodbye_fixture_callback(fixture);
	}

	void SayGoodbye(b2Joint* joint)
	{
		listener.saygoodbye_joint_callback(joint);
	}
};

BOX2C_API cb2DestructionListenerWrapper *cb2destructionlistener_create (cb2destructionlistener functions)
{
	return new cb2DestructionListenerWrapper(functions);
}

BOX2C_API void cb2destructionlistener_destroy (cb2DestructionListenerWrapper *listener)
{
	delete listener;
}

//SetDestructionListener
BOX2C_API void b2world_setdestructionlistener (cb2world *world, cb2DestructionListenerWrapper *listener)
{
	world->SetDestructionListener(listener);
}

//SetContactFilter
struct cb2contactfilter
{
	typedef bool (*shouldcollide) (cb2fixture *fixtureA, cb2fixture *fixtureB);

	shouldcollide shouldcollide_callback;
};

class cb2ContactFilterWrapper : public b2ContactFilter
{
public:
	cb2ContactFilterWrapper (cb2contactfilter listener) :
	  listener(listener)
	  {
	  }

	cb2contactfilter listener;

	bool ShouldCollide(b2Fixture* fixtureA, b2Fixture* fixtureB)
	{
		return listener.shouldcollide_callback(fixtureA, fixtureB);
	}
};

BOX2C_API cb2ContactFilterWrapper *cb2contactfilter_create (cb2contactfilter functions)
{
	return new cb2ContactFilterWrapper(functions);
}

BOX2C_API void cb2contactfilter_destroy (cb2ContactFilterWrapper *listener)
{
	delete listener;
}

BOX2C_API void b2world_setcontactfilter (cb2world *world, cb2ContactFilterWrapper *listener)
{
	world->SetContactFilter(listener);
}

//SetContactListener
struct cb2contactlistener
{
	typedef void (*beginendcontact) (cb2contact *contact);
	typedef void (*presolve) (cb2contact *contact, const cb2manifold *oldManifold);
	typedef void (*postsolve) (cb2contact *contact, const cb2contactimpulse *impulse);

	beginendcontact begincontact_callback;
	beginendcontact endcontact_callback;
	presolve presolve_callback;
	postsolve postsolve_callback;
};

class cb2ContactListenerWrapper : public b2ContactListener
{
public:
	cb2ContactListenerWrapper (cb2contactlistener listener) :
	  listener(listener)
	  {
	  }

	  cb2contactlistener listener;

	  void BeginContact(b2Contact* contact)
	  {
		  listener.begincontact_callback(contact);
	  }

	  void EndContact(b2Contact* contact) 
	  {
		  listener.endcontact_callback(contact);
	  }

	  void PreSolve(b2Contact* contact, const b2Manifold* oldManifold)
	  {
		  listener.presolve_callback(contact, oldManifold);
	  }

	  void PostSolve(b2Contact* contact, const b2ContactImpulse* impulse)
	  {
		  listener.postsolve_callback(contact, impulse);
	  }
};

BOX2C_API cb2ContactListenerWrapper *cb2contactlistener_create (cb2contactlistener functions)
{
	return new cb2ContactListenerWrapper(functions);
}

BOX2C_API void cb2contactlistener_destroy (cb2ContactListenerWrapper *listener)
{
	delete listener;
}

BOX2C_API void b2world_setcontactlistener (cb2world *world, cb2ContactListenerWrapper *listener)
{
	world->SetContactListener(listener);
}

//SetDebugDraw
struct cb2debugdraw
{
	typedef void (*drawpolygon) (const b2Vec2* vertices, int vertexCount, b2Color color);
	typedef void (*drawcircle) (b2Vec2 center, float radius, b2Color color);
	typedef void (*drawsolidcircle) (b2Vec2 center, float radius, b2Vec2 axis, b2Color color);
	typedef void (*drawsegment) (b2Vec2 p1, b2Vec2 p2, b2Color color);
	typedef void (*drawtransform) (b2Transform xf);

	drawpolygon drawpolygon_callback;
	drawpolygon drawsolidpolygon_callback;
	drawcircle drawcircle_callback;
	drawsolidcircle drawsolidcircle_callback;
	drawsegment drawsegment_callback;
	drawtransform drawtransform_callback;
};

class cb2DebugDrawWrapper : public b2DebugDraw
{
public:
	cb2DebugDrawWrapper (cb2debugdraw listenerPtr) :
	  listener(listenerPtr)
	{
	}

	cb2debugdraw listener;

	virtual void DrawPolygon(const b2Vec2* vertices, int32 vertexCount, const b2Color& color)
	{
		if (listener.drawpolygon_callback)
			listener.drawpolygon_callback(vertices, vertexCount, color);
	}

	virtual void DrawSolidPolygon(const b2Vec2* vertices, int32 vertexCount, const b2Color& color)
	{
		if (listener.drawsolidpolygon_callback)
			listener.drawsolidpolygon_callback(vertices, vertexCount, color);
	}

	virtual void DrawCircle(const b2Vec2& center, float32 radius, const b2Color& color)
	{
		if (listener.drawcircle_callback)
			listener.drawcircle_callback(center, radius, color);
	}

	virtual void DrawSolidCircle(const b2Vec2& center, float32 radius, const b2Vec2& axis, const b2Color& color)
	{
		if (listener.drawsolidcircle_callback)
			listener.drawsolidcircle_callback(center, radius, axis, color);
	}

	virtual void DrawSegment(const b2Vec2& p1, const b2Vec2& p2, const b2Color& color)
	{
		if (listener.drawsegment_callback)
			listener.drawsegment_callback(p1, p2, color);
	}

	virtual void DrawTransform(const b2Transform& xf)
	{
		if (listener.drawtransform_callback)
			listener.drawtransform_callback(xf);
	}
};

BOX2C_API cb2DebugDrawWrapper *cb2debugdraw_create (cb2debugdraw functions)
{
	return new cb2DebugDrawWrapper(functions);
}

BOX2C_API uint32 cb2debugdraw_getflags (cb2DebugDrawWrapper *listener)
{
	return listener->GetFlags();
}

BOX2C_API void cb2debugdraw_setflags (cb2DebugDrawWrapper *listener, uint32 flags)
{
	listener->SetFlags(flags);
}

BOX2C_API void cb2debugdraw_destroy (cb2DebugDrawWrapper *listener)
{
	delete listener;
}

BOX2C_API void b2world_setdebugdraw (cb2world *world, cb2DebugDrawWrapper *listener)
{
	world->SetDebugDraw(listener);
}

BOX2C_API cb2body *b2world_createbody (cb2world *world, cb2bodydef *def)
{
	return world->CreateBody (def);
}

BOX2C_API void b2world_destroybody (cb2world *world, cb2body *body)
{
	world->DestroyBody(body);
}

BOX2C_API cb2joint *b2world_createjoint (cb2world *world, cb2jointdef *def)
{
	return world->CreateJoint(def);
}

BOX2C_API void b2world_destroyjoint (cb2world *world, cb2joint *joint)
{
	world->DestroyJoint(joint);
}

BOX2C_API void b2world_step (cb2world *world, float timeStep, int velocityIterations, int positionIterations)
{
	world->Step(timeStep, velocityIterations, positionIterations);
}

BOX2C_API void b2world_clearforces (cb2world *world)
{
	world->ClearForces();
}

BOX2C_API void b2world_drawdebugdata (cb2world *world)
{
	world->DrawDebugData();
}

// QueryAABB

// RayCast

BOX2C_API cb2body *b2world_getbodylist (cb2world *world)
{
	return world->GetBodyList();
}

BOX2C_API cb2joint *b2world_getjointlist (cb2world *world)
{
	return world->GetJointList();
}

BOX2C_API cb2contact *b2world_getcontactlist (cb2world *world)
{
	return world->GetContactList();
}

BOX2C_API void b2world_setwarmstarting (cb2world *world, bool flag)
{
	world->SetWarmStarting(flag);
}

BOX2C_API void b2world_setcontinuousphysics (cb2world *world, bool flag)
{
	world->SetContinuousPhysics(flag);
}

BOX2C_API int b2world_getproxycount (cb2world *world)
{
	return world->GetProxyCount();
}

BOX2C_API int b2world_getbodycount (cb2world *world)
{
	return world->GetBodyCount();
}

BOX2C_API int b2world_getjointcount (cb2world *world)
{
	return world->GetJointCount();
}

BOX2C_API int b2world_getcontactcount (cb2world *world)
{
	return world->GetContactCount();
}

BOX2C_API void b2world_setgravity (cb2world *world, cb2vec2 gravity)
{
	world->SetGravity(gravity);
}

BOX2C_API cb2vec2 b2world_getgravity (cb2world *world)
{
	return world->GetGravity();
}

BOX2C_API bool b2world_islocked (cb2world *world)
{
	return world->IsLocked();
}

BOX2C_API void b2world_setautoclearforces (cb2world *world, bool flag)
{
	world->SetAutoClearForces(flag);
}

BOX2C_API bool b2world_getautoclearforces (cb2world *world)
{
	return world->GetAutoClearForces();
}

struct cb2querycallback
{
	typedef bool (*reportfixture) (b2Fixture* fixture);

	reportfixture reportfixture_callback;
};

class cb2QueryCallbackWrapper : public b2QueryCallback
{
public:
	cb2QueryCallbackWrapper (cb2querycallback listener) :
	  listener(listener)
	  {
	  }

	cb2querycallback listener;

	bool ReportFixture(b2Fixture* fixture)
	{
		return listener.reportfixture_callback(fixture);
	}
};

BOX2C_API cb2QueryCallbackWrapper *cb2querycallback_create (cb2querycallback functions)
{
	return new cb2QueryCallbackWrapper(functions);
}

BOX2C_API void cb2querycallback_destroy (cb2QueryCallbackWrapper *listener)
{
	delete listener;
}

struct cb2raycastcallback
{
	typedef float (*reportfixture) (cb2fixture* fixture, cb2vec2 point,
									cb2vec2 normal, float fraction);

	reportfixture reportfixture_callback;
};

class cb2RayCastCallbackWrapper : public b2RayCastCallback
{
public:
	cb2RayCastCallbackWrapper (cb2raycastcallback listener) :
	  listener(listener)
	  {
	  }

	cb2raycastcallback listener;

	float ReportFixture(	b2Fixture* fixture, const b2Vec2& point,
									const b2Vec2& normal, float fraction)
	{
		return listener.reportfixture_callback(fixture, point, normal, fraction);
	}
};

BOX2C_API cb2RayCastCallbackWrapper *cb2raycastcallback_create (cb2raycastcallback functions)
{
	return new cb2RayCastCallbackWrapper(functions);
}

BOX2C_API void cb2raycastcallback_destroy (cb2RayCastCallbackWrapper *listener)
{
	delete listener;
}

void b2world_queryaabb (cb2world *world, cb2QueryCallbackWrapper *callback, cb2aabb aabb)
{
	world->QueryAABB(callback, aabb);
}

void b2world_raycast (cb2world *world, cb2RayCastCallbackWrapper *callback, cb2vec2 point1, cb2vec2 point2)
{
	world->RayCast(callback, point1, point2);
}

cb2bodydef *b2bodydef_constructor ()
{
	return new b2BodyDef;
}

void b2bodydef_destroy (cb2bodydef *body)
{
	delete body;
}

void b2bodydef_setuserdata (cb2bodydef *body, void *userData)
{
	body->userData = userData;
}

void *b2bodydef_getuserdata (cb2bodydef *body)
{
	return body->userData;
}

void b2bodydef_setposition (cb2bodydef *body, cb2vec2 pos)
{
	body->position = pos;
}
 
void b2bodydef_getposition (cb2bodydef *body, cb2vec2 *outPos)
{
	*outPos = body->position;
}

void b2bodydef_setangle (cb2bodydef *body, float angle)
{
	body->angle = angle;
}

float b2bodydef_getangle (cb2bodydef *body)
{
	return body->angle;
}

void b2bodydef_setlinearvelocity (cb2bodydef *body, cb2vec2 linearVelocity)
{
	body->linearVelocity = linearVelocity;
}

void b2bodydef_getlinearvelocity (cb2bodydef *body, cb2vec2 *outVel)
{
	*outVel = body->linearVelocity;
}

void b2bodydef_setlineardamping (cb2bodydef *body, float linearDamping)
{
	body->linearDamping = linearDamping;
}

float b2bodydef_getlineardamping (cb2bodydef *body)
{
	return body->linearDamping;
}

void b2bodydef_setangulardamping (cb2bodydef *body, float angulardamping)
{
	body->angularDamping = angulardamping;
}

float b2bodydef_getangulardamping (cb2bodydef *body)
{
	return body->angularDamping;
}

void b2bodydef_setallowsleep (cb2bodydef *body, bool allowSleep)
{
	body->allowSleep = allowSleep;
}

bool b2bodydef_getallowsleep (cb2bodydef *body)
{
	return body->allowSleep;
}

void b2bodydef_setawake (cb2bodydef *body, bool awake)
{
	body->awake = awake;
}

bool b2bodydef_getawake (cb2bodydef *body)
{
	return body->awake;
}

void b2bodydef_setfixedrotation (cb2bodydef *body, bool fixedrotation)
{
	body->fixedRotation = fixedrotation;
}

bool b2bodydef_getfixedrotation (cb2bodydef *body)
{
	return body->fixedRotation;
}

void b2bodydef_setbullet (cb2bodydef *body, bool bullet)
{
	body->bullet = bullet;
}

bool b2bodydef_getbullet (cb2bodydef *body)
{
	return body->bullet;
}

void b2bodydef_setbodytype (cb2bodydef *body, int type)
{
	body->type = (b2BodyType)type;
}

int b2bodydef_getbodytype (cb2bodydef *body)
{
	return body->type;
}

void b2bodydef_setactive (cb2bodydef *body, bool active)
{
	body->active = active;
}

bool b2bodydef_getactive (cb2bodydef *body)
{
	return body->active;
}

void b2bodydef_setinertiascale (cb2bodydef *body, float inertiaScale)
{
	body->inertiaScale = inertiaScale;
}

float b2bodydef_getinertiascale (cb2bodydef *body)
{
	return body->inertiaScale;
}

cb2fixturedef *b2fixturedef_constructor ()
{
	return new cb2fixturedef;
}

void b2fixturedef_destroy (cb2fixturedef *fixture)
{
	delete fixture;
}

void b2fixturedef_setshape (cb2fixturedef *fixture, cb2shape *shape)
{
	fixture->shape = shape;
}

void *b2fixturedef_getuserdata (cb2fixturedef *fixture)
{
	return fixture->userData;
}

void b2fixturedef_setuserdata (cb2fixturedef *fixture, void *data)
{
	fixture->userData = data;
}

float b2fixturedef_getfriction (cb2fixturedef *fixture)
{
	return fixture->friction;
}

void b2fixturedef_setfriction (cb2fixturedef *fixture, float friction)
{
	fixture->friction = friction;
}

float b2fixturedef_getrestitution (cb2fixturedef *fixture)
{
	return fixture->restitution;
}

void b2fixturedef_setrestitution (cb2fixturedef *fixture, float restitution)
{
	fixture->restitution = restitution;
}

float b2fixturedef_getdensity (cb2fixturedef *fixture)
{
	return fixture->density;
}

void b2fixturedef_setdensity (cb2fixturedef *fixture, float density)
{
	fixture->density = density;
}

bool b2fixturedef_getissensor (cb2fixturedef *fixture)
{
	return fixture->isSensor;
}

void b2fixturedef_setissensor (cb2fixturedef *fixture, bool issensor)
{
	fixture->isSensor = issensor;
}

uint16 b2fixturedef_getfiltercategorybits (cb2fixturedef *fixture)
{
	return fixture->filter.categoryBits;
}

void b2fixturedef_setfiltercategorybits (cb2fixturedef *fixture, uint16 bits)
{
	fixture->filter.categoryBits = bits;
}

uint16 b2fixturedef_getfiltermaskbits (cb2fixturedef *fixture)
{
	return fixture->filter.maskBits;
}

void b2fixturedef_setfiltermaskbits (cb2fixturedef *fixture, uint16 bits)
{
	fixture->filter.maskBits = bits;
}

int16 b2fixturedef_getfiltergroupindex (cb2fixturedef *fixture)
{
	return fixture->filter.groupIndex;
}

void b2fixturedef_setfiltergroupindex (cb2fixturedef *fixture, int16 bits)
{
	fixture->filter.groupIndex = bits;
}

int b2shape_gettype (cb2shape *shape)
{
	return shape->GetType();
}

float b2shape_getradius (cb2shape *shape)
{
	return shape->m_radius;
}

void b2shape_setradius (cb2shape *shape, float radius)
{
	shape->m_radius = radius;
}

cb2circleshape *b2circleshape_constructor ()
{
	return new cb2circleshape;
}

void b2circleshape_destroy(cb2circleshape *circle)
{
	delete circle;
}

void b2circleshape_getposition (cb2circleshape *circle, cb2vec2 *outPos)
{
	*outPos = circle->m_p;
}

void b2circleshape_setposition (cb2circleshape *circle, cb2vec2 position)
{
	circle->m_p = position;
}

cb2polygonshape *b2polygonshape_constructor ()
{
	return new cb2polygonshape;
}

void b2polygonshape_destroy (cb2polygonshape *polygon)
{
	delete polygon;
}

void b2polygonshape_getcentroid (cb2polygonshape *polygon, cb2vec2 *outPos)
{
	*outPos = polygon->m_centroid;
}

void b2polygonshape_setcentroid (cb2polygonshape *polygon, cb2vec2 centroid)
{
	polygon->m_centroid = centroid;
}

int b2polygonshape_getvertexcount (cb2polygonshape *polygon)
{
	return polygon->m_vertexCount;
}

void b2polygonshape_setvertexcount (cb2polygonshape *polygon, int vertexCount)
{
	polygon->m_vertexCount = vertexCount;
}

void b2polygonshape_getvertex (cb2polygonshape *polygon, int index, cb2vec2 *outPos)
{
	*outPos = polygon->m_vertices[index];
}

void b2polygonshape_setvertex (cb2polygonshape *polygon, int vertex, cb2vec2 value)
{
	polygon->m_vertices[vertex] = value;
}

void b2polygonshape_getnormal (cb2polygonshape *polygon, int index, cb2vec2 *outPos)
{
	*outPos = polygon->m_normals[index];
}

void b2polygonshape_setnormal (cb2polygonshape *polygon, int vertex, cb2vec2 value)
{
	polygon->m_normals[vertex] = value;
}

void b2polygonshape_set (cb2polygonshape *polygon)
{
	polygon->Set(polygon->m_vertices, polygon->m_vertexCount);
}

int b2fixture_gettype (cb2fixture *fixture)
{
	return fixture->GetType();
}

void b2fixture_getshape (cb2fixture *fixture, cb2shapeportable *shape)
{
	switch (fixture->GetShape()->m_type)
	{
	case cb2shape::e_circle:
		{
			cb2circleshapeportable *circle = (cb2circleshapeportable*)shape;
			cb2circleshape *circleIn = (cb2circleshape*)fixture->GetShape();
			circle->m_p = circleIn->m_p;
			circle->m_shape.m_radius = circleIn->m_radius;
			circle->m_shape.m_type = circleIn->m_type;
		}
		break;
	case cb2shape::e_polygon:
		{
			cb2polygonshapeportable *poly = (cb2polygonshapeportable*)shape;
			cb2polygonshape *polyIn = (cb2polygonshape*)fixture->GetShape();
			poly->m_shape.m_radius = polyIn->m_radius;
			poly->m_shape.m_type = polyIn->m_type;
			poly->m_centroid = polyIn->m_centroid;
			poly->m_vertexCount = polyIn->m_vertexCount;
			memcpy (poly->m_vertices, polyIn->m_vertices, sizeof(polyIn->m_vertices));
			memcpy (poly->m_normals, polyIn->m_normals, sizeof(polyIn->m_normals));
		}
		break;
	}
}

void b2fixture_setsensor (cb2fixture *fixture, bool val)
{
	fixture->SetSensor(val);
}

bool b2fixture_getsensor (cb2fixture *fixture)
{
	return fixture->IsSensor();
}

void b2fixture_setfilterdata (cb2fixture *fixture, cb2filter filter)
{
	fixture->SetFilterData(filter);
}

void b2fixture_getfilterdata (cb2fixture *fixture, cb2filter *outFilter)
{
	*outFilter = fixture->GetFilterData();
}

cb2body *b2fixture_getbody (cb2fixture *fixture)
{
	return fixture->GetBody();
}

cb2fixture *b2fixture_getnext (cb2fixture *fixture)
{
	return fixture->GetNext();
}

void b2fixture_setuserdata (cb2fixture *fixture, void *data)
{
	fixture->SetUserData(data);
}

void *b2fixture_getuserdata (cb2fixture *fixture)
{
	return fixture->GetUserData();
}

bool b2fixture_testpoint (cb2fixture *fixture, cb2vec2 point)
{
	return fixture->TestPoint(point);
}

bool b2fixture_raycast (cb2fixture *fixture, cb2raycastoutput *output, cb2raycastinput input)
{
	return fixture->RayCast(output, input);
}

void b2fixture_getmassdata (cb2fixture *fixture, cb2massdata *data)
{
	fixture->GetMassData(data);
}

float b2fixture_getdensity (cb2fixture *fixture)
{
	return fixture->GetDensity();
}

void b2fixture_setdensity (cb2fixture *fixture, float val)
{
	fixture->SetDensity(val);
}

float b2fixture_getfriction (cb2fixture *fixture)
{
	return fixture->GetFriction();
}

void b2fixture_setfriction (cb2fixture *fixture, float val)
{
	fixture->SetFriction(val);
}

float b2fixture_getrestitution (cb2fixture *fixture)
{
	return fixture->GetRestitution();
}

void b2fixture_setrestitution (cb2fixture *fixture, float val)
{
	fixture->SetRestitution(val);
}

void b2fixture_getaabb (cb2fixture *fixture, cb2aabb *outAABB)
{
	*outAABB = fixture->GetAABB();
}

cb2fixture *b2body_createfixture (cb2body *body, cb2fixturedefportable *fixtureDef)
{
	cb2fixturedef tempDef;
	tempDef.userData = fixtureDef->userData;
	tempDef.friction = fixtureDef->friction;
	tempDef.restitution = fixtureDef->restitution;
	tempDef.density = fixtureDef->density;
	tempDef.isSensor = fixtureDef->isSensor;
	tempDef.filter = fixtureDef->filter;

	switch (fixtureDef->shape->m_type)
	{
	case b2Shape::e_circle:
		{
			b2CircleShape circleShape;
			circleShape.m_radius = fixtureDef->shape->m_radius;
			circleShape.m_type = fixtureDef->shape->m_type;
			circleShape.m_p = ((cb2circleshapeportable*)fixtureDef->shape)->m_p;
			tempDef.shape = &circleShape;
			return body->CreateFixture(&tempDef);
		}
	case b2Shape::e_polygon:
		{
			b2PolygonShape polyShape;
			polyShape.m_radius = fixtureDef->shape->m_radius;
			polyShape.m_type = fixtureDef->shape->m_type;
			cb2polygonshapeportable *poly = ((cb2polygonshapeportable*)fixtureDef->shape);
			polyShape.m_centroid = poly->m_centroid;
			polyShape.m_vertexCount = poly->m_vertexCount;
			memcpy (polyShape.m_normals, poly->m_normals, sizeof(polyShape.m_normals));
			memcpy (polyShape.m_vertices, poly->m_vertices, sizeof(polyShape.m_vertices));
			tempDef.shape = &polyShape;
			return body->CreateFixture(&tempDef);
		}
	default:
		return NULL;
	}
}

cb2fixture *b2body_createfixturefromshape (cb2body *body, cb2shapeportable *shape, float density)
{
	switch (shape->m_type)
	{
	case b2Shape::e_circle:
		{
			b2CircleShape circleShape;
			circleShape.m_radius = shape->m_radius;
			circleShape.m_type = shape->m_type;
			circleShape.m_p = ((cb2circleshapeportable*)shape)->m_p;
			return body->CreateFixture(&circleShape, density);
		}
	case b2Shape::e_polygon:
		{
			b2PolygonShape polyShape;
			polyShape.m_radius = shape->m_radius;
			polyShape.m_type = shape->m_type;
			cb2polygonshapeportable *poly = ((cb2polygonshapeportable*)shape);
			polyShape.m_centroid = poly->m_centroid;
			polyShape.m_vertexCount = poly->m_vertexCount;
			memcpy (polyShape.m_normals, poly->m_normals, sizeof(polyShape.m_normals));
			memcpy (polyShape.m_vertices, poly->m_vertices, sizeof(polyShape.m_vertices));

			return body->CreateFixture(&polyShape, density);
		}
	default:
		return NULL;
	}
}

cb2body *b2body_getnext (cb2body *body)
{
	return body->GetNext();
}

void b2body_destroyfixture (cb2body *body, cb2fixture *fixture)
{
	body->DestroyFixture(fixture);
}

void b2body_settransform (cb2body *body, cb2vec2 pos, float angle)
{
	body->SetTransform(pos, angle);
}

void b2body_getposition (cb2body *body, cb2vec2 *outVec)
{
	*outVec = body->GetPosition();
}

float b2body_getangle (cb2body *body)
{
	return body->GetAngle();
}

void b2body_getworldcenter (cb2body *body, cb2vec2 *outVec)
{
	*outVec = body->GetWorldCenter();
}

void b2body_getlocalcenter (cb2body *body, cb2vec2 *outVec)
{
	*outVec = body->GetLocalCenter();
}

void b2body_setlinearvelocity (cb2body *body, cb2vec2 v)
{
	body->SetLinearVelocity(v);
}

void b2body_getlinearvelocity (cb2body *body, cb2vec2 *outVec)
{
	*outVec = body->GetLinearVelocity();
}

void b2body_setangularvelocity (cb2body *body, float v)
{
	body->SetAngularVelocity(v);
}

float b2body_getangularvelocity (cb2body *body)
{
	return body->GetAngularVelocity();
}

void b2body_applyforce (cb2body *body, cb2vec2 force, cb2vec2 point)
{
	body->ApplyForce(force, point);
}

void b2body_applytorque (cb2body *body, float torque)
{
	body->ApplyTorque(torque);
}

void b2body_applylinearimpulse (cb2body *body, cb2vec2 force, cb2vec2 point)
{
	body->ApplyLinearImpulse(force, point);
}

void b2body_applyangularimpulse (cb2body *body, float impulse)
{
	body->ApplyAngularImpulse(impulse);
}

float b2body_getmass (cb2body *body)
{
	return body->GetMass();
}

float b2body_getinertia (cb2body *body)
{
	return body->GetInertia();
}

void b2body_getmassdata (cb2body *body, cb2massdata *data)
{
	body->GetMassData(data);
}

void b2body_setmassdata (cb2body *body, cb2massdata *data)
{
	body->SetMassData(data);
}

void b2body_resetmassdata (cb2body *body)
{
	body->ResetMassData();
}

void b2body_getworldpoint (cb2body *body, cb2vec2 localPoint, cb2vec2 *outPoint)
{
	*outPoint = body->GetWorldPoint(localPoint);
}

void b2body_getworldvector (cb2body *body, cb2vec2 localPoint, cb2vec2 *outPos)
{
	*outPos = body->GetWorldVector(localPoint);
}

void b2body_getlocalpoint (cb2body *body, cb2vec2 localPoint, cb2vec2 *outPos)
{
	*outPos = body->GetLocalPoint(localPoint);
}

void b2body_getlocalvector (cb2body *body, cb2vec2 localPoint, cb2vec2 *outPos)
{
	*outPos = body->GetLocalVector(localPoint);
}

void b2body_getlinearvelocityfromworldvector (cb2body *body, cb2vec2 localPoint, cb2vec2 *outPos)
{
	*outPos = body->GetLinearVelocityFromWorldPoint(localPoint);
}

void b2body_getlinearvelocityfromlocalvector (cb2body *body, cb2vec2 localPoint, cb2vec2 *outPos)
{
	*outPos = body->GetLinearVelocityFromLocalPoint(localPoint);
}

float b2body_getlineardamping (cb2body *body)
{
	return body->GetLinearDamping();
}

void b2body_setlineardamping (cb2body *body, float damping)
{
	body->SetLinearDamping(damping);
}

float b2body_getangulardamping (cb2body *body)
{
	return body->GetAngularDamping();
}

void b2body_setangulardamping (cb2body *body, float damping)
{
	return body->SetAngularDamping(damping);
}

void b2body_settype (cb2body *body, int type)
{
	body->SetType((b2BodyType)type);
}

int b2body_gettype (cb2body *body)
{
	return body->GetType();
}

void b2body_setbullet (cb2body *body, bool bullet)
{
	body->SetBullet (bullet);
}

bool b2body_getbullet (cb2body *body)
{
	return body->IsBullet();
}

void b2body_setissleepingallowed (cb2body *body, bool bullet)
{
	body->SetSleepingAllowed(bullet);
}

bool b2body_getissleepingallowed (cb2body *body)
{
	return body->IsSleepingAllowed();
}

void b2body_setawake (cb2body *body, bool bullet)
{
	body->SetAwake(bullet);
}

bool b2body_getawake (cb2body *body)
{
	return body->IsAwake();
}

void b2body_setactive (cb2body *body, bool bullet)
{
	body->SetActive(bullet);
}

bool b2body_getactive (cb2body *body)
{
	return body->IsActive();
}

void b2body_setfixedrotation (cb2body *body, bool bullet)
{
	body->SetFixedRotation(bullet);
}

bool b2body_getfixedrotation (cb2body *body)
{
	return body->IsFixedRotation();
}

cb2fixture *b2body_getfixturelist (cb2body *body)
{
	return body->GetFixtureList();
}

cb2jointedge *b2body_getjointlist (cb2body *body)
{
	return body->GetJointList();
}

cb2contactedge *b2body_getcontactlist (cb2body *body)
{
	return body->GetContactList();
}

void *b2body_getuserdata (cb2body *body)
{
	return body->GetUserData();
}

void b2body_setuserdata (cb2body *body, void *data)
{
	body->SetUserData(data);
}

cb2world *b2body_getworld (cb2body *body)
{
	return body->GetWorld();
}


cb2body *b2jointedge_getother (cb2jointedge *edge)
{
	return edge->other;
}

cb2joint *b2jointedge_getjoint (cb2jointedge *edge)
{
	return edge->joint;
}

cb2jointedge *b2jointedge_getprev (cb2jointedge *edge)
{
	return edge->prev;
}

cb2jointedge *b2jointedge_getnext (cb2jointedge *edge)
{
	return edge->next;
}

cb2body *b2contactedge_getother (cb2contactedge *edge)
{
	return edge->other;
}

cb2contact *b2contactedge_getcontact (cb2contactedge *edge)
{
	return edge->contact;
}

cb2contactedge *b2contactedge_getprev (cb2contactedge *edge)
{
	return edge->prev;
}

cb2contactedge *b2contactedge_getnext (cb2contactedge *edge)
{
	return edge->next;
}


cb2manifold *b2contact_getmanifold (cb2contact *contact)
{
	return contact->GetManifold();
}

void b2contact_getworldmanifold (cb2contact *contact, cb2worldmanifold *worldManifold)
{
	contact->GetWorldManifold(worldManifold);
}

bool b2contact_istouching (cb2contact *contact)
{
	return contact->IsTouching();
}

void b2contact_setenabled (cb2contact *contact, bool flag)
{
	contact->SetEnabled(flag);
}

bool b2contact_getenabled (cb2contact *contact)
{
	return contact->IsEnabled();
}

cb2contact *b2contact_getnext (cb2contact *contact)
{
	return contact->GetNext();
}

cb2fixture *b2contact_getfixturea (cb2contact *contact)
{
	return contact->GetFixtureA();
}

cb2fixture *b2contact_getfixtureb (cb2contact *contact)
{
	return contact->GetFixtureB();
}

void b2contact_evaluate (cb2contact *contact, cb2manifold *outManifold, cb2transform xfA, cb2transform xfB)
{
	contact->Evaluate(outManifold, xfA, xfB);
}


// joint defs

int b2jointdef_gettype (cb2jointdef *jointdef)
{
	return jointdef->type;
}

void *b2jointdef_getuserdata (cb2jointdef *jointdef)
{
	return jointdef->userData;
}

void b2jointdef_setuserdata (cb2jointdef *jointdef, void *data)
{
	jointdef->userData = data;
}

cb2body *b2jointdef_getbodya (cb2jointdef *jointdef)
{
	return jointdef->bodyA;
}

void b2jointdef_setbodya (cb2jointdef *jointdef, cb2body *body)
{
	jointdef->bodyA = body;
}

cb2body *b2jointdef_getbodyb (cb2jointdef *jointdef)
{
	return jointdef->bodyB;
}

void b2jointdef_setbodyb (cb2jointdef *jointdef, cb2body *body)
{
	jointdef->bodyB = body;
}

bool b2jointdef_getcollideconnected (cb2jointdef *jointdef)
{
	return jointdef->collideConnected;
}

void b2jointdef_setcollideconnected (cb2jointdef *jointdef, bool flag)
{
	jointdef->collideConnected = flag;
}

QUICK_CONSTRUCTOR_DESTROYER(
	b2revolutejointdef_constructor,
	b2revolutejointdef_destroy,
	cb2revolutejointdef);

QUICK_GET_SETTER_POINTER(
	b2revolutejointdef_getlocalanchora, 
	b2revolutejointdef_setlocalanchora,
	cb2revolutejointdef,
	cb2vec2,
	localAnchorA);

QUICK_GET_SETTER_POINTER(
	b2revolutejointdef_getlocalanchorb, 
	b2revolutejointdef_setlocalanchorb,
	cb2revolutejointdef,
	cb2vec2,
	localAnchorB);

QUICK_GET_SETTER(
	b2revolutejointdef_getreferenceangle, 
	b2revolutejointdef_setreferenceangle,
	cb2revolutejointdef,
	float,
	referenceAngle);

QUICK_GET_SETTER(
	b2revolutejointdef_getenablelimit, 
	b2revolutejointdef_setenablelimit,
	cb2revolutejointdef,
	bool,
	enableLimit);

QUICK_GET_SETTER(
	b2revolutejointdef_getlowerangle, 
	b2revolutejointdef_setlowerangle,
	cb2revolutejointdef,
	float,
	lowerAngle);

QUICK_GET_SETTER(
	b2revolutejointdef_getupperangle, 
	b2revolutejointdef_setupperangle,
	cb2revolutejointdef,
	float,
	upperAngle);

QUICK_GET_SETTER(
	b2revolutejointdef_getenablemotor, 
	b2revolutejointdef_setenablemotor,
	cb2revolutejointdef,
	bool,
	enableMotor);

QUICK_GET_SETTER(
	b2revolutejointdef_getmotorspeed, 
	b2revolutejointdef_setmotorspeed,
	cb2revolutejointdef,
	float,
	motorSpeed);

QUICK_GET_SETTER(
	b2revolutejointdef_getmaxmotortorque, 
	b2revolutejointdef_setmaxmotortorque,
	cb2revolutejointdef,
	float,
	maxMotorTorque);


QUICK_CONSTRUCTOR_DESTROYER(
	b2distancejointdef_constructor,
	b2distancejointdef_destroy,
	cb2distancejointdef);

QUICK_GET_SETTER_POINTER(
	b2distancejointdef_getlocalanchora, 
	b2distancejointdef_setlocalanchora,
	cb2distancejointdef,
	cb2vec2,
	localAnchorA);

QUICK_GET_SETTER_POINTER(
	b2distancejointdef_getlocalanchorb, 
	b2distancejointdef_setlocalanchorb,
	cb2distancejointdef,
	cb2vec2,
	localAnchorB);

QUICK_GET_SETTER(
	b2distancejointdef_getlength, 
	b2distancejointdef_setlength,
	cb2distancejointdef,
	float,
	length);

QUICK_GET_SETTER(
	b2distancejointdef_getfrequency, 
	b2distancejointdef_setfrequency,
	cb2distancejointdef,
	float,
	frequencyHz);

QUICK_GET_SETTER(
	b2distancejointdef_getdampingratio, 
	b2distancejointdef_setdampingratio,
	cb2distancejointdef,
	float,
	dampingRatio);


QUICK_CONSTRUCTOR_DESTROYER(
	b2frictionjointdef_constructor,
	b2frictionjointdef_destroy,
	cb2frictionjointdef);

QUICK_GET_SETTER_POINTER(
	b2frictionjointdef_getlocalanchora, 
	b2frictionjointdef_setlocalanchora,
	cb2frictionjointdef,
	cb2vec2,
	localAnchorA);

QUICK_GET_SETTER_POINTER(
	b2frictionjointdef_getlocalanchorb, 
	b2frictionjointdef_setlocalanchorb,
	cb2frictionjointdef,
	cb2vec2,
	localAnchorB);

QUICK_GET_SETTER(
	b2frictionjointdef_getmaxforce, 
	b2frictionjointdef_setmaxforce,
	cb2frictionjointdef,
	float,
	maxForce);

QUICK_GET_SETTER(
	b2frictionjointdef_getmaxtorque, 
	b2frictionjointdef_setmaxtorque,
	cb2frictionjointdef,
	float,
	maxTorque);


QUICK_CONSTRUCTOR_DESTROYER(
	b2gearjointdef_constructor,
	b2gearjointdef_destroy,
	cb2gearjointdef);

QUICK_GET_SETTER(
	b2gearjointdef_getjoint1, 
	b2gearjointdef_setjoint1,
	cb2gearjointdef,
	cb2joint*,
	joint1);

QUICK_GET_SETTER(
	b2gearjointdef_getjoint2, 
	b2gearjointdef_setjoint2,
	cb2gearjointdef,
	cb2joint*,
	joint2);

QUICK_GET_SETTER(
	b2gearjointdef_getratio, 
	b2gearjointdef_setratio,
	cb2gearjointdef,
	float,
	ratio);


QUICK_CONSTRUCTOR_DESTROYER(
	b2linejointdef_constructor,
	b2linejointdef_destroy,
	cb2linejointdef);

QUICK_GET_SETTER_POINTER(
	b2linejointdef_getlocalanchora, 
	b2linejointdef_setlocalanchora,
	cb2linejointdef,
	cb2vec2,
	localAnchorA);

QUICK_GET_SETTER_POINTER(
	b2linejointdef_getlocalanchorb, 
	b2linejointdef_setlocalanchorb,
	cb2linejointdef,
	cb2vec2,
	localAnchorB);

QUICK_GET_SETTER_POINTER(
	b2linejointdef_getlocalaxisa, 
	b2linejointdef_setlocalaxisa,
	cb2linejointdef,
	cb2vec2,
	localAxisA);

QUICK_GET_SETTER(
	b2linejointdef_getenablelimit, 
	b2linejointdef_setenablelimit,
	cb2linejointdef,
	bool,
	enableLimit);

QUICK_GET_SETTER(
	b2linejointdef_getlowertranslation, 
	b2linejointdef_setlowertranslation,
	cb2linejointdef,
	float,
	lowerTranslation);

QUICK_GET_SETTER(
	b2linejointdef_getuppertranslation, 
	b2linejointdef_setuppertranslation,
	cb2linejointdef,
	float,
	upperTranslation);

QUICK_GET_SETTER(
	b2linejointdef_getenablemotor, 
	b2linejointdef_setenablemotor,
	cb2linejointdef,
	bool,
	enableMotor);

QUICK_GET_SETTER(
	b2linejointdef_getmaxmotorforce, 
	b2linejointdef_setmaxmotorforce,
	cb2linejointdef,
	float,
	maxMotorForce);

QUICK_GET_SETTER(
	b2linejointdef_getmotorspeed, 
	b2linejointdef_setmotorspeed,
	cb2linejointdef,
	float,
	motorSpeed);


QUICK_CONSTRUCTOR_DESTROYER(
	b2mousejointdef_constructor,
	b2mousejointdef_destroy,
	cb2mousejointdef);

QUICK_GET_SETTER_POINTER(
	b2mousejointdef_gettarget, 
	b2mousejointdef_settarget,
	cb2mousejointdef,
	cb2vec2,
	target);

QUICK_GET_SETTER(
	b2mousejointdef_getmaxforce, 
	b2mousejointdef_setmaxforce,
	cb2mousejointdef,
	float,
	maxForce);

QUICK_GET_SETTER(
	b2mousejointdef_getfrequency, 
	b2mousejointdef_setfrequency,
	cb2mousejointdef,
	float,
	frequencyHz);

QUICK_GET_SETTER(
	b2mousejointdef_getdampingratio, 
	b2mousejointdef_setdampingratio,
	cb2mousejointdef,
	float,
	dampingRatio);


QUICK_CONSTRUCTOR_DESTROYER(
	b2prismaticjointdef_constructor,
	b2prismaticjointdef_destroy,
	cb2prismaticjointdef);

QUICK_GET_SETTER_POINTER(
	b2prismaticjointdef_getlocalanchora, 
	b2prismaticjointdef_setlocalanchora,
	cb2prismaticjointdef,
	cb2vec2,
	localAnchorA);

QUICK_GET_SETTER_POINTER(
	b2prismaticjointdef_getlocalaxis1, 
	b2prismaticjointdef_setlocalaxis1,
	cb2prismaticjointdef,
	cb2vec2,
	localAxis1);

QUICK_GET_SETTER_POINTER(
	b2prismaticjointdef_getlocalanchorb, 
	b2prismaticjointdef_setlocalanchorb,
	cb2prismaticjointdef,
	cb2vec2,
	localAnchorB);

QUICK_GET_SETTER(
	b2prismaticjointdef_getreferenceangle, 
	b2prismaticjointdef_setreferenceangle,
	cb2prismaticjointdef,
	float,
	referenceAngle);

QUICK_GET_SETTER(
	b2prismaticjointdef_getenablelimit, 
	b2prismaticjointdef_setenablelimit,
	cb2prismaticjointdef,
	bool,
	enableLimit);

QUICK_GET_SETTER(
	b2prismaticjointdef_getlowertranslation, 
	b2prismaticjointdef_setlowertranslation,
	cb2prismaticjointdef,
	float,
	lowerTranslation);

QUICK_GET_SETTER(
	b2prismaticjointdef_getuppertranslation, 
	b2prismaticjointdef_setuppertranslation,
	cb2prismaticjointdef,
	float,
	upperTranslation);

QUICK_GET_SETTER(
	b2prismaticjointdef_getenablemotor, 
	b2prismaticjointdef_setenablemotor,
	cb2prismaticjointdef,
	bool,
	enableMotor);

QUICK_GET_SETTER(
	b2prismaticjointdef_getmaxmotorforce, 
	b2prismaticjointdef_setmaxmotorforce,
	cb2prismaticjointdef,
	float,
	maxMotorForce);

QUICK_GET_SETTER(
	b2prismaticjointdef_getmotorspeed, 
	b2prismaticjointdef_setmotorspeed,
	cb2prismaticjointdef,
	float,
	motorSpeed);


QUICK_CONSTRUCTOR_DESTROYER(
	b2pulleyjointdef_constructor,
	b2pulleyjointdef_destroy,
	cb2pulleyjointdef);

QUICK_GET_SETTER_POINTER(
	b2pulleyjointdef_getgroundanchora, 
	b2pulleyjointdef_setgroundanchora,
	cb2pulleyjointdef,
	cb2vec2,
	groundAnchorA);

QUICK_GET_SETTER_POINTER(
	b2pulleyjointdef_getgroundanchorb, 
	b2pulleyjointdef_setgroundanchorb,
	cb2pulleyjointdef,
	cb2vec2,
	groundAnchorB);

QUICK_GET_SETTER_POINTER(
	b2pulleyjointdef_getlocalanchora, 
	b2pulleyjointdef_setlocalanchora,
	cb2pulleyjointdef,
	cb2vec2,
	localAnchorA);

QUICK_GET_SETTER_POINTER(
	b2pulleyjointdef_getlocalanchorb, 
	b2pulleyjointdef_setlocalanchorb,
	cb2pulleyjointdef,
	cb2vec2,
	localAnchorB);

QUICK_GET_SETTER(
	b2pulleyjointdef_getlengtha, 
	b2pulleyjointdef_setlengtha,
	cb2pulleyjointdef,
	float,
	lengthA);

QUICK_GET_SETTER(
	b2pulleyjointdef_getmaxlengtha, 
	b2pulleyjointdef_setmaxlengtha,
	cb2pulleyjointdef,
	float,
	maxLengthA);

QUICK_GET_SETTER(
	b2pulleyjointdef_getlengthb, 
	b2pulleyjointdef_setlengthb,
	cb2pulleyjointdef,
	float,
	lengthB);

QUICK_GET_SETTER(
	b2pulleyjointdef_getmaxlengthb, 
	b2pulleyjointdef_setmaxlengthb,
	cb2pulleyjointdef,
	float,
	maxLengthB);

QUICK_GET_SETTER(
	b2pulleyjointdef_getratio, 
	b2pulleyjointdef_setratio,
	cb2pulleyjointdef,
	float,
	ratio);


QUICK_CONSTRUCTOR_DESTROYER(
	b2weldjointdef_constructor,
	b2weldjointdef_destroy,
	cb2weldjointdef);

QUICK_GET_SETTER_POINTER(
	b2weldjointdef_getlocalanchora, 
	b2weldjointdef_setlocalanchora,
	cb2weldjointdef,
	cb2vec2,
	localAnchorA);

QUICK_GET_SETTER_POINTER(
	b2weldjointdef_getlocalanchorb, 
	b2weldjointdef_setlocalanchorb,
	cb2weldjointdef,
	cb2vec2,
	localAnchorB);

QUICK_GET_SETTER(
	b2weldjointdef_getreferenceangle, 
	b2weldjointdef_setreferenceangle,
	cb2weldjointdef,
	float,
	referenceAngle);


QUICK_GETTER(
	b2joint_gettype,
	cb2joint,
	int,
	GetType());

QUICK_GETTER(
	b2joint_getbodya,
	cb2joint,
	cb2body*,
	GetBodyA());

QUICK_GETTER(
	b2joint_getbodyb,
	cb2joint,
	cb2body*,
	GetBodyB());

QUICK_GETTER_PTR(
	b2joint_getanchora,
	cb2joint,
	cb2vec2,
	GetAnchorA());

QUICK_GETTER_PTR(
	b2joint_getanchorb,
	cb2joint,
	cb2vec2,
	GetAnchorB());

void b2joint_getreactionforce (cb2joint *joint, float inv_dt, cb2vec2 *outVar)
{
	*outVar = joint->GetReactionForce(inv_dt);
}

float b2joint_getreactiontorque (cb2joint *joint, float inv_dt)
{
	return joint->GetReactionTorque(inv_dt);
}

QUICK_GETTER(
	b2joint_getnext,
	cb2joint,
	cb2joint*,
	GetNext());

QUICK_GET_SETTER_FUNC(
	b2joint_getuserdata,
	b2joint_setuserdata,
	cb2joint,
	void*,
	GetUserData,
	SetUserData);

QUICK_GETTER(
	b2joint_getisactive,
	cb2joint,
	bool,
	IsActive());


QUICK_GET_SETTER_FUNC(
	b2gearjoint_getratio,
	b2gearjoint_setratio,
	cb2gearjoint,
	float,
	GetRatio,
	SetRatio);


QUICK_GET_SETTER_FUNC(
	b2distancejoint_getratio,
	b2distancejoint_setratio,
	cb2distancejoint,
	float,
	GetLength,
	SetLength);

QUICK_GET_SETTER_FUNC(
	b2distancejoint_getfrequency,
	b2distancejoint_setfrequency,
	cb2distancejoint,
	float,
	GetFrequency,
	SetFrequency);

QUICK_GET_SETTER_FUNC(
	b2distancejoint_getdampingratio,
	b2distancejoint_setdampingratio,
	cb2distancejoint,
	float,
	GetDampingRatio,
	SetDampingRatio);


QUICK_GET_SETTER_FUNC(
	b2frictionjoint_getmaxforce,
	b2frictionjoint_setmaxforce,
	cb2frictionjoint,
	float,
	GetMaxForce,
	SetMaxForce);

QUICK_GET_SETTER_FUNC(
	b2frictionjoint_getmaxtorque,
	b2frictionjoint_setmaxtorque,
	cb2frictionjoint,
	float,
	GetMaxTorque,
	SetMaxTorque);


QUICK_GETTER(
	b2pulleyjoint_getlength1,
	cb2pulleyjoint,
	float,
	GetLength1());

QUICK_GETTER(
	b2pulleyjoint_getlength2,
	cb2pulleyjoint,
	float,
	GetLength2());

QUICK_GETTER(
	b2pulleyjoint_getratio,
	cb2pulleyjoint,
	float,
	GetRatio());


QUICK_GET_SETTER_FUNC_PTR(
	b2mousejoint_gettarget,
	b2mousejoint_settarget,
	cb2mousejoint,
	cb2vec2,
	GetTarget,
	SetTarget);

QUICK_GET_SETTER_FUNC(
	b2mousejoint_getmaxforce,
	b2mousejoint_setmaxforce,
	cb2mousejoint,
	float,
	GetMaxForce,
	SetMaxForce);

QUICK_GET_SETTER_FUNC(
	b2mousejoint_getfrequency,
	b2mousejoint_setfrequency,
	cb2mousejoint,
	float,
	GetFrequency,
	SetFrequency);

QUICK_GET_SETTER_FUNC(
	b2mousejoint_getdampingratio,
	b2mousejoint_setdampingratio,
	cb2mousejoint,
	float,
	GetDampingRatio,
	SetDampingRatio);


QUICK_GETTER(
	b2linejoint_getjointtranslation,
	cb2linejoint,
	float,
	GetJointTranslation());

QUICK_GETTER(
	b2linejoint_getjointspeed,
	cb2linejoint,
	float,
	GetJointSpeed());

QUICK_GET_SETTER_FUNC(
	b2linejoint_getenablelimit,
	b2linejoint_setenablelimit,
	cb2linejoint,
	bool,
	IsLimitEnabled,
	EnableLimit);

QUICK_GETTER(
	b2linejoint_getlowerlimit,
	cb2linejoint,
	float,
	GetLowerLimit());

QUICK_GETTER(
	b2linejoint_getupperlimit,
	cb2linejoint,
	float,
	GetUpperLimit());

void b2linejoint_setlimits (cb2linejoint *joint, float lower, float upper)
{
	joint->SetLimits(lower, upper);
}

QUICK_GET_SETTER_FUNC(
	b2linejoint_getenablemotor,
	b2linejoint_setenablemotor,
	cb2linejoint,
	bool,
	IsMotorEnabled,
	EnableMotor);

QUICK_GET_SETTER_FUNC(
	b2linejoint_getmotorspeed,
	b2linejoint_setmotorspeed,
	cb2linejoint,
	float,
	GetMotorSpeed,
	SetMotorSpeed);

QUICK_SETTER_FUNC(
	b2linejoint_setmaxmotorforce,
	cb2linejoint,
	float,
	SetMaxMotorForce);

QUICK_GETTER(
	b2linejoint_getmotorforce,
	cb2linejoint,
	float,
	GetMotorForce());


QUICK_GETTER(
	b2revolutejoint_getjointangle,
	cb2revolutejoint,
	float,
	GetJointAngle());

QUICK_GETTER(
	b2revolutejoint_getjointspeed,
	cb2revolutejoint,
	float,
	GetJointSpeed());

QUICK_GET_SETTER_FUNC(
	b2revolutejoint_getenablelimit,
	b2revolutejoint_setenablelimit,
	cb2revolutejoint,
	bool,
	IsLimitEnabled,
	EnableLimit);

QUICK_GETTER(
	b2revolutejoint_getlowerlimit,
	cb2revolutejoint,
	float,
	GetLowerLimit());

QUICK_GETTER(
	b2revolutejoint_getupperlimit,
	cb2revolutejoint,
	float,
	GetUpperLimit());

void b2revolutejoint_setlimits (cb2revolutejoint *joint, float lower, float upper)
{
	joint->SetLimits(lower, upper);
}

QUICK_GET_SETTER_FUNC(
	b2revolutejoint_getenablemotor,
	b2revolutejoint_setenablemotor,
	cb2revolutejoint,
	bool,
	IsMotorEnabled,
	EnableMotor);

QUICK_GET_SETTER_FUNC(
	b2revolutejoint_getmotorspeed,
	b2revolutejoint_setmotorspeed,
	cb2revolutejoint,
	float,
	GetMotorSpeed,
	SetMotorSpeed);

QUICK_SETTER_FUNC(
	b2revolutejoint_setmaxmotortorque,
	cb2revolutejoint,
	float,
	SetMaxMotorTorque);

QUICK_GETTER(
	b2revolutejoint_getmotortorque,
	cb2revolutejoint,
	float,
	GetMotorTorque());


QUICK_GETTER(
	b2prismaticjoint_getjointtranslation,
	cb2prismaticjoint,
	float,
	GetJointTranslation());

QUICK_GETTER(
	b2prismaticjoint_getjointspeed,
	cb2prismaticjoint,
	float,
	GetJointSpeed());

QUICK_GET_SETTER_FUNC(
	b2prismaticjoint_getenablelimit,
	b2prismaticjoint_setenablelimit,
	cb2prismaticjoint,
	bool,
	IsLimitEnabled,
	EnableLimit);

QUICK_GETTER(
	b2prismaticjoint_getlowerlimit,
	cb2prismaticjoint,
	float,
	GetLowerLimit());

QUICK_GETTER(
	b2prismaticjoint_getupperlimit,
	cb2prismaticjoint,
	float,
	GetUpperLimit());

void b2prismaticjoint_setlimits (cb2prismaticjoint *joint, float lower, float upper)
{
	joint->SetLimits(lower, upper);
}

QUICK_GET_SETTER_FUNC(
	b2prismaticjoint_getenablemotor,
	b2prismaticjoint_setenablemotor,
	cb2prismaticjoint,
	bool,
	IsMotorEnabled,
	EnableMotor);

QUICK_GET_SETTER_FUNC(
	b2prismaticjoint_getmotorspeed,
	b2prismaticjoint_setmotorspeed,
	cb2prismaticjoint,
	float,
	GetMotorSpeed,
	SetMotorSpeed);

QUICK_SETTER_FUNC(
	b2prismaticjoint_setmaxmotorforce,
	cb2prismaticjoint,
	float,
	SetMaxMotorForce);

QUICK_GETTER(
	b2prismaticjoint_getmotorforce,
	cb2prismaticjoint,
	float,
	GetMotorForce());

void b2version_get (cb2version *outVersion)
{
	*outVersion = b2_version;
}