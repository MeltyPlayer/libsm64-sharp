using Assimp;


namespace demo;

public class AssimpNormalSmoother {
  private record AssimpVertex(
      Vector3D Position
  );

  public void SmoothNormalsInScene(Scene scene) {
    foreach (var mesh in scene.Meshes) {
      this.SmoothNormalsInMesh(mesh);
    }
  }

  public void SmoothNormalsInMesh(Mesh mesh) {
    var vertexCount = mesh.VertexCount;

    var vertexToIndex = new Dictionary<AssimpVertex, int>();
    var indexMapper = new int[vertexCount];

    for (var i = 0; i < mesh.VertexCount; ++i) {
      var position = mesh.Vertices[i];

      var assimpVertex = new AssimpVertex(position);

      if (vertexToIndex.TryGetValue(assimpVertex, out var index)) {
        indexMapper[i] = index;
      } else {
        indexMapper[i] = i;
        vertexToIndex[assimpVertex] = i;
      }
    }

    var totalNormalByIndex = new Vector3D[vertexCount];

    foreach (var face in mesh.Faces) {
      var i1 = indexMapper[face.Indices[0]];
      var i2 = indexMapper[face.Indices[1]];
      var i3 = indexMapper[face.Indices[2]];

      var p1 = mesh.Vertices[i1];
      var p2 = mesh.Vertices[i2];
      var p3 = mesh.Vertices[i3];

      // calculate facet normal of the triangle using cross product;
      // both components are "normalized" against a common point chosen as the base
      var facetNormal =
          Vector3D.Cross(p2 - p1, p3 - p1); // p1 is the 'base' here

      // get the angle between the two other points for each point;
      // the starting point will be the 'base' and the two adjacent points will be normalized against it
      var a1 = AssimpNormalSmoother.AngleBetween_(p2 - p1, p3 - p1);
      var a2 = AssimpNormalSmoother.AngleBetween_(p3 - p2, p1 - p2);
      var a3 = AssimpNormalSmoother.AngleBetween_(p1 - p3, p2 - p3);

      // normalize the initial facet normals if you want to ignore surface area
      //if (!area_weighting) {
      facetNormal.Normalize();
      //}

      // store the weighted normal in an structured array
      totalNormalByIndex[i1] += facetNormal * a1;
      totalNormalByIndex[i2] += facetNormal * a2;
      totalNormalByIndex[i3] += facetNormal * a3;
    }

    for (var v = 0; v < vertexCount; v++) {
      var N = totalNormalByIndex[indexMapper[v]];
      N.Normalize();

      mesh.Normals[v] = N;
    }
  }

  private static float AngleBetween_(Vector3D v1, Vector3D v2)
    => MathF.Acos(Vector3D.Dot(v1, v2) / (v1.Length() * v2.Length()));
}