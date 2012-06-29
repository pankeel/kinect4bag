#include "CGePhysX.h"
#include "model_obj.h"
#include "vector3.h"
#include "bitmap.h"
#include "PsFastMemory.h"
#include "extensions/PxExtensionsAPI.h"
#include "PxTkStream.h"
#include <iostream>
using namespace physx;
void fatalError(const char * msg);
PxErrorCallback& getPhysxErrorCallback();

PxDefaultAllocator gDefaultAllocatorCallback;
PxSimulationFilterShader gDefaultFilterShader = PxDefaultSimulationFilterShader;

static PxVec3 gVirtualParticleWeights[] = 
{ 
    // center point
    PxVec3(1.0f / 3, 1.0f / 3, 1.0f / 3),

    // center of sub triangles
    PxVec3(2.0f / 3, 1.0f / 6, 1.0f / 6),
    PxVec3(1.0f / 6, 2.0f / 3, 1.0f / 6),
    PxVec3(1.0f / 6, 1.0f / 6, 2.0f / 3),

    // edge mid points
    PxVec3(1.0f / 2, 1.0f / 2, 0.0f),
    PxVec3(0.0f, 1.0f / 2, 1.0f / 2),
    PxVec3(1.0f / 2, 0.0f, 1.0f / 2),

    // further subdivision of mid points
    PxVec3(1.0f / 4, 3.0f / 4, 0.0f),
    PxVec3(3.0f / 4, 1.0f / 4, 0.0f),

    PxVec3(0.0f, 1.0f / 4, 3.0f / 4),
    PxVec3(0.0f, 3.0f / 4, 1.0f / 4),

    PxVec3(1.0f / 4, 0.0f, 3.0f / 4),
    PxVec3(3.0f / 4, 0.0f, 1.0f / 4)
};

void fatalError(const char * msg)
{
    cerr << msg << endl;
    exit(-1);
}

CGePhysX::CGePhysX(void)
    :mPvdConnection(NULL)
    ,mCharacterScale(1.0f)
{
    mClothCollisionData.setToDefault();
}


CGePhysX::~CGePhysX(void)
{
}


void CGePhysX::OnInit()
{
    //Recording memory allocations is necessary if you want to 
    //use the memory facilities in PVD effectively.  Since PVD isn't necessarily connected
    //right away, we add a mechanism that records all outstanding memory allocations and
    //forwards them to PVD when it does connect.

    //This certainly has a performance and memory profile effect and thus should be used
    //only in non-production builds.
    bool recordMemoryAllocations = true;
    const bool useCustomTrackingAllocator = true;

    PxAllocatorCallback* allocator = &gDefaultAllocatorCallback;

    //if(useCustomTrackingAllocator)		
    //    allocator = getSampleAllocator();		//optional override that will track memory allocations

    mFoundation = PxCreateFoundation(PX_PHYSICS_VERSION, *allocator, getPhysxErrorCallback());
    if(!mFoundation)
        fatalError("PxCreateFoundation failed!");

    mPhysics = PxCreatePhysics(PX_PHYSICS_VERSION, *mFoundation, PxTolerancesScale(), recordMemoryAllocations);
    if(!mPhysics)
        fatalError("PxCreatePhysics failed!");

    if(!PxInitExtensions(*mPhysics))
        fatalError("PxInitExtensions failed!");

    mCooking = PxCreateCooking(PX_PHYSICS_VERSION, *mFoundation, PxCookingParams());
    if(!mCooking)
        fatalError("PxCreateCooking failed!");

    if(!PxInitExtensions(*mPhysics))
        cerr<< "PxInitExtensions failed!" <<endl;
    PxVisualDebuggerExt::createConnection(mPhysics->getPvdConnectionManager(), "127.0.0.1", 5425, 100);
    
    // check if PvdConnection manager is available on this platform
    //if(mPhysics->getPvdConnectionManager() != NULL)
    //{
    //    // setup connection parameters
    //    const char*     pvd_host_ip = "127.0.0.1";  // IP of the PC which is running PVD
    //    int             port        = 5425;         // TCP port to connect to, where PVD is listening
    //    unsigned int    timeout     = 100;          // timeout in milliseconds to wait for PVD to respond,
    //    // consoles and remote PCs need a higher timeout.
    //    PxVisualDebuggerConnectionFlags connectionFlags = PxVisualDebuggerExt::getAllConnectionFlags();

    //    // and now try to connect
    //    mPvdConnection = PxVisualDebuggerExt::createConnection(mPhysics->getPvdConnectionManager(),
    //        pvd_host_ip, port, timeout, connectionFlags);

    //}


    //Create the scene
    PxSceneDesc sceneDesc(mPhysics->getTolerancesScale());
    sceneDesc.gravity=PxVec3(0.0f, -9.8f, 0.0f);
    //sceneDesc.gravity=PxVec3(0.0f, 0.f, 0.0f);

    if(!sceneDesc.cpuDispatcher) {
        PxDefaultCpuDispatcher* mCpuDispatcher = PxDefaultCpuDispatcherCreate(1);
        if(!mCpuDispatcher)
            cerr<<"PxDefaultCpuDispatcherCreate failed!"<<endl;
        sceneDesc.cpuDispatcher = mCpuDispatcher;
    } 
    if(!sceneDesc.filterShader)
        sceneDesc.filterShader  = gDefaultFilterShader;


    mScene = mPhysics->createScene(sceneDesc);
    if (!mScene)
        cerr<<"createScene failed!"<<endl;

    mScene->setVisualizationParameter(PxVisualizationParameter::eSCALE,				 1.0);
    mScene->setVisualizationParameter(PxVisualizationParameter::eCOLLISION_SHAPES,	1.0f);
    //gScene->setVisualizationParameter(PxVisualizationParameter::eDEFORMABLE_MESH, 1.0f);
    //gScene->setVisualizationParameter(PxVisualizationParameter::eDEFORMABLE_SELFCOLLISIONS, 1.0f);
    //gScene->setVisualizationParameter(PxVisualizationParameter::eCOLLISION_DYNAMIC, 1.0f);
    //gScene->setVisualizationParameter(PxVisualizationParameter::eDEFORMABLE_SHAPES, 1.0f);

    PxMaterial* mMaterial = mPhysics->createMaterial(0.5,0.5,0.5);

    //Create actors 
    //1) Create ground plane
    //PxReal d = 0.0f;	 
    //PxTransform pose = PxTransform(PxVec3(0.0f, 0, 0.0f),PxQuat(PxHalfPi, PxVec3(0.0f, 0.0f, 1.0f)));

    //PxRigidStatic* plane = mPhysics->createRigidStatic(pose);
    //if (!plane)
    //    cerr<<"create plane failed!"<<endl;

    //PxShape* shape = plane->createShape(PxPlaneGeometry(), *mMaterial);
    //if (!shape)
    //    cerr<<"create shape failed!"<<endl;
    //mScene->addActor(*plane);


    ////2) Create cube	 
    //PxReal density = 1.0f;
    //PxTransform transform(PxVec3(0.0f, 5.0, 0.0f), PxQuat::createIdentity());
    //PxVec3 dimensions(0.5,0.5,0.5);
    //PxBoxGeometry geometry(dimensions);    

    //PxRigidDynamic *actor = PxCreateDynamic(*mPhysics, transform, geometry, *mMaterial, density);
    //if (!actor)
    //    cerr<<"create actor failed!"<<endl;
    //actor->setAngularDamping(0.75);
    //actor->setLinearVelocity(PxVec3(0,0,0)); 
    //mScene->addActor(*actor);
    //boxes.push_back(actor);
}

void CGePhysX::StepPhysX(float stepTime)
{
    mScene->simulate(stepTime);        

    //...perform useful work here using previous frame's state data        
    while(!mScene->fetchResults() )     
    {
        // do something useful        
    }
}
void CGePhysX::createCloth( const char* clothName, PxReal clothScale, 
    PxVec3 clothOffset, PxQuat clothRotate)
{
    // compute root transform and positions of all the bones
    PxTransform rootPose(PxVec3(0,0,0), PxQuat::createIdentity());
    //vector<PxVec3> positions;	
    //vector<PxU32> indexPairs;
    ////mCharacter.getFramePose(rootPose, positions, indexPairs);

    //// convert bones to collision capsules
    //vector<PxClothCollisionSphere> spheres;	
    //spheres.resize(positions.size());
    //for (PxU32 i = 0; i < positions.size(); i++)
    //{
    //    spheres[i].pos = positions[i];
    //    //spheres[i].radius = gCharacterScale * gSphereRadius[i];
    //}

    //PxClothCollisionData collisionData;
    //collisionData.numSpheres = static_cast<PxU32>(positions.size());
    //collisionData.spheres = &spheres[0];
    //collisionData.numPairs = static_cast<PxU32>(indexPairs.size()) / 2; // number of capsules
    //collisionData.pairIndexBuffer = &indexPairs[0];

    PxClothMeshDesc meshDesc;
    meshDesc.setToDefault();
    vector<PxReal> uvs;

    //PxReal clothScale = gCharacterScale * 0.3f;
    createMeshFromObj(clothName, clothScale, &clothRotate, &clothOffset, 
        mClothVertices, mClothIndices, &uvs, meshDesc);

    if (!meshDesc.isValid()) 
		fatalError("Could not load cloth obj\n");
    // create the cloth
    //PxCloth& cloth = *createClothFromMeshDesc( \
        meshDesc, rootPose, &collisionData, PxVec3(0,-1,0),\
        &uvs[0], "dummy_cape_d.bmp", PxVec3(0.5f, 0.5f, 0.5f));
    mCloth = createClothFromMeshDesc( \
        meshDesc, rootPose, PxVec3(0,-1,0),\
        &uvs[0], "dummy_cape_d.bmp", PxVec3(0.5f, 0.5f, 0.5f));
}

bool CGePhysX::createMeshFromObj( const char* path, PxReal scale, const PxQuat* rot, 
    const PxVec3* offset, vector<PxVec3>& vertexBuffer, vector<PxU32>& primitiveBuffer, 
    vector<PxReal>* textureBuffer, PxClothMeshDesc &meshDesc )
{
    bool textured = (textureBuffer != 0);

    ModelOBJ obj;
    obj.import(path, false);
    if (obj.getNumberOfVertices() <= 0)
        return false;

    PxVec3 myOffset(0.0f);
    PxQuat myRot = PxQuat::createIdentity();

    if (offset)
        myOffset = *offset;

    if (rot)
        myRot = *rot;

    int numVertices	= obj.getNumberOfVertices();
    int numPrimitives = obj.getNumberOfTriangles();

    printf("Obj nVert: %d\n", numVertices);
    printf("Obj nPrim: %d\n", numPrimitives);

    vertexBuffer.resize(numVertices);
    primitiveBuffer.resize(numPrimitives*3);

    const ModelOBJ::Vertex* pVert = obj.getVertexBuffer();
    PxVec3 *vDest = (PxVec3*)&vertexBuffer[0];
    for (int i = 0; i < numVertices; i++, vDest++, pVert++) 
    {
        PxVec3 posSrc(pVert->position[0], pVert->position[1], pVert->position[2]);
        *vDest = scale * myRot.rotate(posSrc) + myOffset;

        /*PxVec3 *vSrc2 = (PxVec3*)&vertexBuffer[0];
        for (int j = 0; j < i; ++j, ++vSrc2)
        {
            if ((posSrc - *vSrc2).magnitude() < 0.0001f)
            {
                printf("%d: %f, %f, %f \n", i, posSrc.x, posSrc.y, posSrc.z);
                printf("%d: %f, %f, %f \n", j, vSrc2->x, vSrc2->y, vSrc2->z);
            }
        }*/
    }

    physx::shdfnd::fastMemcpy((PxU32*)&primitiveBuffer[0], 
        (PxU32*)obj.getIndexBuffer(), sizeof(PxU32)*numPrimitives*3);

    if (textured)
    {
        int numTexCoords = numVertices * 2;
        textureBuffer->resize(numTexCoords);
        const ModelOBJ::Vertex* pVert = obj.getVertexBuffer();
        for (int i = 0; i < numVertices; i++, pVert++) 
        {
            (*textureBuffer)[2*i] = (PxReal)pVert->texCoord[0];
            (*textureBuffer)[2*i+1] = (PxReal)pVert->texCoord[1];
        }
        //physx::shdfnd::fastMemcpy((PxReal*)textureBuffer->begin(), wo.mTexCoords, \
            sizeof(PxReal) * numTexCoords);
    }

    // fill the physx cloth mesh descriptor 
    fillClothMeshDesc( vertexBuffer, primitiveBuffer, meshDesc);

    return true;
}

PxClothFabric* CGePhysX::createFabric(PxPhysics &physics, PxCooking &cooking, 
    const PxClothMeshDesc &desc, const PxVec3& gravityDir)
{
    // In this example, we cook the fabric on the fly through a memory stream
    // Note that we can also use a file stream and pre-cook the mesh to save the cooking time
    PxToolkit::MemoryOutputStream wb;
    PX_ASSERT(desc.isValid());

    // cook the fabric data into memory buffer (cooking time operation)
    if (!cooking.cookClothFabric(desc, gravityDir, wb))
        return 0;

    // read fabric from memory stream (runtime operation)
    PxToolkit::MemoryInputData rb(wb.getData(), wb.getSize());
    return physics.createClothFabric(rb);
}

////////////////////////////////////////////////////////////////////////////////
bool CGePhysX::createDefaultParticles(const PxClothMeshDesc& meshDesc, 
    PxClothParticle* clothParticles, PxReal massPerParticle)
{
    const PxVec3* srcPoints = reinterpret_cast<const PxVec3*>(meshDesc.points.data);
    for (PxU32 i = 0; i < meshDesc.points.count; i++)
    {
        clothParticles[i].pos = srcPoints[i]; 
        clothParticles[i].invWeight = 1.0f / massPerParticle;
    }
    return true;
}

PxCloth* CGePhysX::createClothFromMeshDesc( PxClothMeshDesc &meshDesc, const PxTransform &pose, const PxVec3& gravityDir /*= PxVec3(0,0,-1)*/, const PxReal* uv /*= 0*/, const char *textureFile /*= 0*/, const PxVec3& color /*= PxVec3(0.5f, 0.5f, 0.5f)*/ )
{
    PxClothFabric* clothFabric = createFabric(*mPhysics, *mCooking, meshDesc, gravityDir);
    if (!clothFabric)
        return 0;

    // create cloth particle to set initial position and inverse mass (constraint)
    vector<PxClothParticle> clothParticles(meshDesc.points.count);
    createDefaultParticles(meshDesc, &clothParticles[0]);

    // flags to set GPU solver, CCD, etc.
    PxClothFlags flags;
    flags=PxClothFlag::eSWEPT_CONTACT;

    // create the cloth actor
    PxCloth* cloth = mPhysics->createCloth(pose, *clothFabric, &clothParticles[0], 
        mClothCollisionData, flags);

    cloth->setSolverFrequency(60.0f); // don't know how to get target simulation frequency, just hardcode for now

    // damp global particle velocity to 90% every 0.1 seconds
    //cloth->setDampingCoefficient(0.1f); // damp local particle velocity
    cloth->setDampingCoefficient(1.f); // damp local particle velocity
    cloth->setDragCoefficient(0.1f); // transfer frame velocity

    // reduce effect of local frame acceleration
    cloth->setInertiaScale(0.3f);

    PX_ASSERT(cloth);	

    // add this cloth into the scene
    mScene->addActor(*cloth);

    // create render material
    //RenderMaterial* clothMaterial = createRenderMaterialFromTextureFile(textureFile);

    // create the render object in sample framework
    //createRenderObjectsFromCloth(*cloth, meshDesc, clothMaterial, uv, true, color, scale);
    const bool useVirtualParticles = true;
    const bool useSweptContact = true;
    const bool useCustomConfig = true;

    // virtual particles
    if (useVirtualParticles)
        createVirtualParticles(*cloth, meshDesc, 4);

    // ccd
    cloth->setClothFlag(PxClothFlag::eSWEPT_CONTACT, useSweptContact);

    // use GPU or not
#if PX_SUPPORT_GPU_PHYSX
    cloth->setClothFlag(PxClothFlag::eGPU, true);
#endif

    // custom fiber configuration
    if (useCustomConfig)
    {
        PxClothPhaseSolverConfig config;

        config = cloth->getPhaseSolverConfig(PxClothFabricPhaseType::eSTRETCHING);
        config.solverType = PxClothPhaseSolverConfig::eSTIFF;
        config.stiffness = 1.0f;
        cloth->setPhaseSolverConfig(PxClothFabricPhaseType::eSTRETCHING, config);

        config = cloth->getPhaseSolverConfig(PxClothFabricPhaseType::eSTRETCHING_HORIZONTAL);
        config.solverType = PxClothPhaseSolverConfig::eFAST;
        config.stiffness = 1.0f;
        cloth->setPhaseSolverConfig(PxClothFabricPhaseType::eSTRETCHING_HORIZONTAL, config);

        config = cloth->getPhaseSolverConfig(PxClothFabricPhaseType::eSHEARING);
        config.solverType = PxClothPhaseSolverConfig::eFAST;
        config.stiffness = 0.75f;
        cloth->setPhaseSolverConfig(PxClothFabricPhaseType::eSHEARING, config);

        config = cloth->getPhaseSolverConfig(PxClothFabricPhaseType::eBENDING_ANGLE);
        config.solverType = PxClothPhaseSolverConfig::eBENDING;
        config.stiffness = 0.5f;
        cloth->setPhaseSolverConfig(PxClothFabricPhaseType::eBENDING_ANGLE, config);
    }

    return cloth;
}

bool CGePhysX::createVirtualParticles(PxCloth& cloth, PxClothMeshDesc& meshDesc, int numSamples)
{
    if(!numSamples)
        return false;

    PxU32 numFaces = meshDesc.triangles.count;
    PxU8* triangles = (PxU8*)meshDesc.triangles.data;

    PxU32 numParticles = numFaces * numSamples;
    vector<PxU32> virtualParticleIndices;
    virtualParticleIndices.reserve(4 * numParticles);

    for (PxU32 i = 0; i < numFaces; i++)
    {
        for (int s = 0; s < numSamples; ++s)
        {
            PxU32 v0, v1, v2;

            if (meshDesc.flags & PxMeshFlag::e16_BIT_INDICES)
            {
                PxU16* triangle = (PxU16*)triangles;
                v0 = triangle[0];
                v1 = triangle[1];
                v2 = triangle[2];
            }
            else
            {
                PxU32* triangle = (PxU32*)triangles;
                v0 = triangle[0];
                v1 = triangle[1];
                v2 = triangle[2];
            }

            virtualParticleIndices.push_back(v0);
            virtualParticleIndices.push_back(v1);
            virtualParticleIndices.push_back(v2);
            virtualParticleIndices.push_back(s);
        }
        triangles += meshDesc.triangles.stride;
    }

    cloth.setVirtualParticles(numParticles, &virtualParticleIndices[0], numSamples, gVirtualParticleWeights);

    return true;
}

////////////////////////////////////////////////////////////////////////////////
// fill cloth mesh descriptor from vertices and primitives
void CGePhysX::fillClothMeshDesc(
    vector<PxVec3> &vertexBuffer, 
    vector<PxU32>& primitiveBuffer,
    PxClothMeshDesc &meshDesc)
{
    int numVertices = vertexBuffer.size();
    int numTriangles = primitiveBuffer.size() / 3;

    // convert vertex array to PxBoundedData (desc.points)
    meshDesc.points.data = &vertexBuffer[0];
    meshDesc.points.count = static_cast<PxU32>(numVertices);
    meshDesc.points.stride = sizeof(PxVec3);

    // convert face index array to PxBoundedData (desc.triangles)
    meshDesc.triangles.data = &primitiveBuffer[0];
    meshDesc.triangles.count = static_cast<PxU32>(numTriangles); 
    meshDesc.triangles.stride = sizeof(PxU32) * 3; // <- stride per triangle
}

void CGePhysX::Destroy()
{
    // TODO: 
    mPhysics->release();

    // remember to release the connection by manual in the end
    if (mPvdConnection)
        mPvdConnection->release();
}

bool CGePhysX::addCollisionSpheres( vector<PxVec3>& positions, 
    vector<PxReal>& radius, vector<PxU32>& indexPair )
{
    mClothCollisionSpheres.resize(positions.size());
    for (PxU32 i = 0; i < positions.size(); i++)
    {
        mClothCollisionSpheres[i].pos = positions[i];
        mClothCollisionSpheres[i].radius = mCharacterScale * radius[i];
    }
    mClothCollisionData.numSpheres = static_cast<PxU32>(positions.size());
    if (mClothCollisionData.numSpheres)
        mClothCollisionData.spheres = &mClothCollisionSpheres[0];
    mClothCollisionSpheresIndexPair.assign(indexPair.begin(), indexPair.end());
    mClothCollisionData.numPairs = static_cast<PxU32>(mClothCollisionSpheresIndexPair.size()) / 2; // number of capsules
    if(mClothCollisionData.numPairs)
        mClothCollisionData.pairIndexBuffer = &mClothCollisionSpheresIndexPair[0];

    return true;
}

void CGePhysX::updateCollisionSpheres( vector<PxVec3>& positions )
{
    // set collision sphere positions
    mClothCollisionSpheres.resize(mCloth->getNbCollisionSpheres());
    //vector<PxU32> indexpairs(mCloth->getNbCollisionSpherePairs());
    //vector<PxClothCollisionPlane> planes(mCloth->getNbCollisionPlanes());
    //vector<PxU32> convexMasks(mCloth->getNbCollisionConvexes());

    mCloth->getCollisionData(&mClothCollisionSpheres[0], NULL, NULL, NULL);
    for (PxU32 i = 0; i < positions.size(); i++)
        mClothCollisionSpheres[i].pos = positions[i];
    mCloth->setCollisionSpheres(&mClothCollisionSpheres[0]);
}

// extract current cloth particle position from PxCloth
// verts is supposed to have been pre-allocated to be at least size of particle array
bool CGePhysX::getVertsFromCloth( PxVec3* verts, const PxCloth& cloth )
{
    PxClothReadData* readData = cloth.lockClothReadData();
    if (!readData)
        return false;

    // copy vertex positions
    PxU32 numVerts = cloth.getNbParticles();
    for (PxU32 i = 0; i < numVerts; ++i)
        verts[i] = readData->particles[i].pos;

    readData->unlock();

    return true;
}

bool CGePhysX::getClothIndices( PxU32*& indices, PxU32& nIndices )
{
    nIndices = mClothIndices.size();
    indices = &mClothIndices[0];
    return true;
}

bool CGePhysX::getClothParticles( PxVec3* particles )
{
    return getVertsFromCloth(particles, *mCloth);
}

// ----------
PxErrorCallback& getPhysxErrorCallback()
{
    static PxDefaultErrorCallback gDefaultErrorCallback;
    return gDefaultErrorCallback;
}