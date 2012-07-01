/* File : example.i */
%module CGePhysics

%{
#include "CGePhysX.h"

%}

/* Let's just grab the original header file here */
%include "std_vector.i"
%include "CGePhysX.h"
%include "PxVec3Wrapper.h"



%template(Int_Vector) std::vector<int>;
%template(PxVec3_Vector) std::vector<PxVec3Wrapper>;
