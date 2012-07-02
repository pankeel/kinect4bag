/* File : example.i */
%module CGePhysics

%{
#include "CGePhysX.h"
%}

/* Let's just grab the original header file here */
%include "arrays_csharp.i"
%include "std_vector.i"



%apply int INPUT[]  { void* particles}
%apply int OUTPUT[]  { void* particles}
%apply int INPUT[]  { int* indices}
%apply int OUTPUT[]  { int* indices}

%include "CGePhysX.h"

%clear void* particles;
%clear void* particles;
%clear int* indices;
%clear int* indices;
%include "PxVec3Wrapper.h"



%template(Int_Vector) std::vector<int>;
%template(PxVec3_Vector) std::vector<PxVec3Wrapper>;
