/* File : example.i */
%module CGePhysics

%{
#include "CGePhysX.h"
%}
%include "arrays_csharp.i"

/* Let's just grab the original header file here */
%include "arrays_csharp.i"
%include "std_vector.i"



%apply float INPUT[]  { void* particles}
%apply float OUTPUT[]  { void* particles}
%apply int INPUT[]  { int* indices}
%apply int OUTPUT[]  { int* indices}
%apply float INPUT[]  { float* pSpherePos}
%apply float INPUT[]  { float* pSphereRadius}
%apply int INPUT[]  { int* pIndexPair}
%apply float INPUT[]  { float* normals}
%apply float OUTPUT[]  { float* normals}
%apply float INPUT[]  { float* textures}
%apply float OUTPUT[]  { float* textures}
%apply float INPUT[]  { float* clothOffset}
%apply float INPUT[]  { float* clothRotate}

%include "CGePhysX.h"

%clear void* particles;
%clear void* particles;
%clear int* indices;
%clear int* indices;
%include "PxVec3Wrapper.h"



%apply int INPUT[]  { int* sourceArray }
%apply int OUTPUT[] { int* particle }

%template(Int_Vector) std::vector<int>;
%template(PxVec3_Vector) std::vector<PxVec3Wrapper>;
