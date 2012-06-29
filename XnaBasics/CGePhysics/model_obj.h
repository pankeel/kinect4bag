// Copyright info of this file is left out for the assignment.

#if !defined(MODEL_OBJ_H)
#define MODEL_OBJ_H

#include <cstdio>
#include <map>
#include <string>
#include <vector>

//-----------------------------------------------------------------------------
// Alias|Wavefront OBJ file loader.
//
// This OBJ file loader contains the following restrictions:
// 1. Group information is ignored. Faces are grouped based on the material
//    that each face uses.
// 2. Object information is ignored. This loader will merge everything into a
//    single object.
// 3. The MTL file must be located in the same directory as the OBJ file. If
//    it isn't then the MTL file will fail to load and a default material is
//    used instead.
// 4. This loader triangulates all polygonal faces during importing.
//-----------------------------------------------------------------------------

class ModelOBJ
{
public:
    struct Material
    {
        float ambient[4];
        float diffuse[4];
        float specular[4];
        float shininess;        // [0 = min shininess, 1 = max shininess]
        float alpha;            // [0 = fully transparent, 1 = fully opaque]

        std::string name;
        std::string colorMapFilename;
        std::string bumpMapFilename;
    };

    struct Vertex
    {
        float position[3];
        float texCoord[2];
        float normal[3];
        float tangent[4];
        float bitangent[3];
    };

    struct Mesh
    {
        int startIndex;
        int triangleCount;
        const Material *pMaterial;
    };

	struct Edge
	{
		int startIndex;		// the smaller one
		int endIndex;		// the larger one
		bool operator<(const Edge& rhs) const;
	};

    ModelOBJ();
    ~ModelOBJ();

    void destroy();
    bool import(const char *pszFilename, bool rebuildNormals = false);
    void normalize(float scaleTo = 1.0f, bool center = true);
    void reverseWinding();

    // Getter methods.

    void getCenter(float &x, float &y, float &z) const;
    float getWidth() const;
    float getHeight() const;
    float getLength() const;
    float getRadius() const;

    const int *getIndexBuffer() const;
	const int *getIndexBufferAdj() const;
    int getIndexSize() const;

    const Material &getMaterial(int i) const;
    const Mesh &getMesh(int i) const;

    int getNumberOfIndices() const;
    int getNumberOfMaterials() const;
    int getNumberOfMeshes() const;
    int getNumberOfTriangles() const;
    int getNumberOfVertices() const;

    const std::string &getPath() const;

    const Vertex &getVertex(int i) const;
    const Vertex *getVertexBuffer() const;
    int getVertexSize() const;

    bool hasNormals() const;
    bool hasPositions() const;
    bool hasTangents() const;
    bool hasTextureCoords() const;

private:
	void addTriangleWithAdj(int index, int v0, int v1, int v2);
    void addTrianglePos(int index, int material,
        int v0, int v1, int v2);
    void addTrianglePosNormal(int index, int material,
        int v0, int v1, int v2,
        int vn0, int vn1, int vn2);
    void addTrianglePosTexCoord(int index, int material,
        int v0, int v1, int v2,
        int vt0, int vt1, int vt2);
    void addTrianglePosTexCoordNormal(int index, int material,
        int v0, int v1, int v2,
        int vt0, int vt1, int vt2,
        int vn0, int vn1, int vn2);
    int addVertex(int hash, const Vertex *pVertex);
    void bounds(float center[3], float &width, float &height,
        float &length, float &radius) const;
    void buildMeshes();
    void generateNormals();
    void generateTangents();
    void importGeometryFirstPass(FILE *pFile);
    void importGeometrySecondPass(FILE *pFile);
    bool importMaterials(const char *pszFilename);
    void scale(float scaleFactor, float offset[3]);
	void matchAdjVertex(int vert1Ind, int vert2Ind, int vert3IndBuf, int vert3OppPos);

    bool m_hasPositions;
    bool m_hasTextureCoords;
    bool m_hasNormals;
    bool m_hasTangents;

    int m_numberOfVertexCoords;
    int m_numberOfTextureCoords;
    int m_numberOfNormals;
    int m_numberOfTriangles;
    int m_numberOfMaterials;
    int m_numberOfMeshes;

    float m_center[3];
    float m_width;
    float m_height;
    float m_length;
    float m_radius;

    std::string m_directoryPath;

    std::vector<Mesh> m_meshes;
    std::vector<Material> m_materials;
    std::vector<Vertex> m_vertexBuffer;
    std::vector<int> m_indexBuffer;
	std::vector<int> m_indexBufferAdj;
    std::vector<int> m_attributeBuffer;
    std::vector<float> m_vertexCoords;
    std::vector<float> m_textureCoords;
    std::vector<float> m_normals;

    std::map<std::string, int> m_materialCache;
    std::map<int, std::vector<int> > m_vertexCache;
	std::map<Edge, int> m_edgeAdjCache;
};

//-----------------------------------------------------------------------------

inline void ModelOBJ::getCenter(float &x, float &y, float &z) const
{ x = m_center[0]; y = m_center[1]; z = m_center[2]; }

inline float ModelOBJ::getWidth() const
{ return m_width; }

inline float ModelOBJ::getHeight() const
{ return m_height; }

inline float ModelOBJ::getLength() const
{ return m_length; }

inline float ModelOBJ::getRadius() const
{ return m_radius; }

inline const int *ModelOBJ::getIndexBuffer() const
{ return &m_indexBuffer[0]; }

inline const int *ModelOBJ::getIndexBufferAdj() const
{ return &m_indexBufferAdj[0]; }

inline int ModelOBJ::getIndexSize() const
{ return static_cast<int>(sizeof(int)); }

inline const ModelOBJ::Material &ModelOBJ::getMaterial(int i) const
{ return m_materials[i]; }

inline const ModelOBJ::Mesh &ModelOBJ::getMesh(int i) const
{ return m_meshes[i]; }

inline int ModelOBJ::getNumberOfIndices() const
{ return m_numberOfTriangles * 3; }

inline int ModelOBJ::getNumberOfMaterials() const
{ return m_numberOfMaterials; }

inline int ModelOBJ::getNumberOfMeshes() const
{ return m_numberOfMeshes; }

inline int ModelOBJ::getNumberOfTriangles() const
{ return m_numberOfTriangles; }

inline int ModelOBJ::getNumberOfVertices() const
{ return static_cast<int>(m_vertexBuffer.size()); }

inline const std::string &ModelOBJ::getPath() const
{ return m_directoryPath; }

inline const ModelOBJ::Vertex &ModelOBJ::getVertex(int i) const
{ return m_vertexBuffer[i]; }

inline const ModelOBJ::Vertex *ModelOBJ::getVertexBuffer() const
{ return &m_vertexBuffer[0]; }

inline int ModelOBJ::getVertexSize() const
{ return static_cast<int>(sizeof(Vertex)); }

inline bool ModelOBJ::hasNormals() const
{ return m_hasNormals; }

inline bool ModelOBJ::hasPositions() const
{ return m_hasPositions; }

inline bool ModelOBJ::hasTangents() const
{ return m_hasTangents; }

inline bool ModelOBJ::hasTextureCoords() const
{ return m_hasTextureCoords; }

inline bool ModelOBJ::Edge::operator<(const Edge& rhs) const
{
	return (startIndex == rhs.startIndex) ? 
		endIndex < rhs.endIndex :
		startIndex < rhs.startIndex;
}


#endif