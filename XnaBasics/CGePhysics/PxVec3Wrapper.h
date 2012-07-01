
class PxVec3Wrapper{
public:
	double x,y,z;

	PxVec3Wrapper():x(0),y(0),z(0){}
	PxVec3Wrapper(double nx, double ny,double nz):x(nx), y(ny), z(nz) {
	}
	~PxVec3Wrapper(){}
};