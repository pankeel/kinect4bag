#pragma once
#include <iostream>
#include <vector>
#include "PxPhysicsAPI.h"
#include "PxPhysX.h"

using namespace std;


//#ifdef _DEBUG
#pragma comment(lib, "PhysX3CHECKED_x86.lib")
#pragma comment(lib, "PhysX3CommonCHECKED_x86.lib")
#pragma comment(lib, "PhysX3CookingCHECKED_x86.lib")
#pragma comment(lib, "PxTaskCHECKED.lib")
//#pragma comment(lib, "PhysX3ExtensionsDEBUG.lib")
//#else
//#pragma comment(lib, "PhysX3_x86.lib")
//#pragma comment(lib, "PhysX3Common_x86.lib")
//#pragma comment(lib, "PhysX3Cooking_x86.lib")
//#pragma comment(lib, "PxTask.lib")
#pragma comment(lib, "PhysX3Extensions.lib")
//#endif

#ifdef CGECLOTH_EXPORTS
#define CGEXPORT __declspec(dllexport) 
#define DllImport   __declspec( dllimport )
#define DllExport   __declspec( dllexport )

#else 
#define CGEXPORT __declspec(dllimport) 
#endif 





//class DllExport CGePhysX
class CGePhysX
{
public:
    CGePhysX(void);
    virtual ~CGePhysX(void);

public:
    void OnInit();

    void StepPhysX(float stepTime);

    void createCloth(const char* clothName, physx::PxReal clothScale = 1.0f, 
        physx::PxVec3 clothOffset = physx::PxVec3(0,0,0), physx::PxQuat clothRotate = physx::PxQuat::createIdentity());

    bool addCollisionSpheres(vector<physx::PxVec3>& positions, vector<physx::PxReal>& radius, vector<physx::PxU32>& indexPair);

    void updateCollisionSpheres(vector<physx::PxVec3>& positions);

    bool getClothIndices(physx::PxU32*& indices, physx::PxU32& nIndices);

    bool getClothParticles(physx::PxVec3* particles);

    void Destroy();

protected:
    bool createMeshFromObj(
        const char* path, physx::PxReal scale, const physx::PxQuat* rot, const physx::PxVec3* offset, 
        vector<physx::PxVec3>& vertexBuffer, vector<physx::PxU32>& primitiveBuffer, 
        vector<physx::PxReal>* textureBuffer, physx::PxClothMeshDesc &meshDesc);

    physx::PxCloth* createClothFromMeshDesc(physx::PxClothMeshDesc &meshDesc, const physx::PxTransform &pose, 
        const physx::PxVec3& gravityDir = physx::PxVec3(0,-1,0), const physx::PxReal* uv = 0, 
        const char *textureFile = 0, const physx::PxVec3& color = physx::PxVec3(0.5f, 0.5f, 0.5f));

    void fillClothMeshDesc( vector<physx::PxVec3> &vertexBuffer, vector<physx::PxU32>& primitiveBuffer,
        physx::PxClothMeshDesc &meshDesc);

    physx::PxClothFabric* createFabric(physx::PxPhysics &physics, physx::PxCooking &cooking, 
        const physx::PxClothMeshDesc &desc, const physx::PxVec3& gravityDir);

    bool createDefaultParticles(const physx::PxClothMeshDesc& meshDesc, 
        physx::PxClothParticle* clothParticles, physx::PxReal massPerParticle = 1.0);

    bool createVirtualParticles(physx::PxCloth& cloth, physx::PxClothMeshDesc& meshDesc, int numSamples);

    bool getVertsFromCloth(physx::PxVec3* verts, const physx::PxCloth& cloth);

protected:
    physx::PxFoundation*                           mFoundation;
    physx::PxPhysics*                              mPhysics;
    physx::PxCooking*								mCooking;
    physx::PxScene*								mScene;
    physx::PxMaterial*								mMaterial;
    PVD::PvdConnection*                     mPvdConnection;
    bool                                    mUseFullPvdConnection;

public:
    physx::PxCloth* mCloth; 
    vector<physx::PxU32> mClothIndices;
    vector<physx::PxVec3> mClothVertices;
    physx::PxClothCollisionData mClothCollisionData;
    vector<physx::PxClothCollisionSphere> mClothCollisionSpheres;
    vector<physx::PxU32> mClothCollisionSpheresIndexPair;
    vector<physx::PxU32> mClothCollisionIndexPair;
    physx::PxReal mCharacterScale;

public:
    physx::PxPhysics* getPhysics() { return mPhysics;}
    physx::PxScene* getScene() { return mScene;}
    physx::PxCooking* getCooking() { return mCooking;}
};
