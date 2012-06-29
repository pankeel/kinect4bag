/* File : example.i */
%module CGePhysics

%{
#include "CGePhysX.h"
#include "example.h"
%}

/* Let's just grab the original header file here */
%include "CGePhysX.h"
%include "example.h"