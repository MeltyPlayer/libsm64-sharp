using OpenTK;


namespace Quad64.src.Scripts {
  public class ModelBuilder {
    public class FinalMesh {
      public List<Vector3> vertices;
      public List<Vector2> texCoords;
      public List<Vector4> colors;
      public List<uint> indices;

      public ModelBuilderMaterial Material { get; set; }
    }

    public class TempMesh {
      public List<Vector3> vertices;
      public List<Vector2> texCoords;
      public List<Vector4> colors;
      public FinalMesh final;

      public ModelBuilderMaterial Material { get; set; }
    }

    public struct TextureInfo {
      public int wrapS, wrapT;
    }

    public bool processingTexture = false;

    public float currentScale = 1f;
    public int numTriangles = 0;

    public ModelBuilderMaterial? CurrentMaterial { get; set; }

    private List<TempMesh> TempMeshes = new List<TempMesh>();
    private FinalMesh finalMesh;

    public bool UsesFog { get; set; }
    public Color FogColor { get; set; }
    public List<uint> FogColor_romLocation = new List<uint>();

    private Vector3 offset = new Vector3(0, 0, 0);

    public HashSet<(Bitmap image, uint segmentAddr, TextureInfo info)>
        TextureData { get; } = new();

    public IEnumerable<Bitmap> TextureImages =>
        this.TextureData.Select(data => data.image);

    public Vector3 Offset {
      get { return offset; }
      set { offset = value; }
    }

    private FinalMesh newFinalMesh() {
      FinalMesh m = new FinalMesh();
      m.vertices = new List<Vector3>();
      m.texCoords = new List<Vector2>();
      m.colors = new List<Vector4>();
      m.indices = new List<uint>();
      return m;
    }

    private TempMesh newTempMesh() {
      TempMesh m = new TempMesh();
      m.vertices = new List<Vector3>();
      m.texCoords = new List<Vector2>();
      m.colors = new List<Vector4>();
      m.final = newFinalMesh();
      return m;
    }

    public TextureInfo newTexInfo(int wrapS, int wrapT) {
      TextureInfo info = new TextureInfo();
      info.wrapS = wrapS;
      info.wrapT = wrapT;
      return info;
    }

    public void AddGeometryMode(RspGeometryMode geometryMode) {
      var newMaterial =
          this.CurrentMaterial?.Clone() ?? new ModelBuilderMaterial();
      newMaterial.GeometryMode = geometryMode;

      this.TryToStartNewMesh(newMaterial);
    }

    public void AddTexture(Bitmap bmp, TextureInfo info, uint segmentAddress) {
      var newMaterial =
          this.CurrentMaterial?.Clone() ?? new ModelBuilderMaterial();
      newMaterial.Bitmap = bmp;
      newMaterial.SegmentAddress = segmentAddress;
      newMaterial.TextureInfo = info;

      this.TextureData.Add((bmp, segmentAddress, info));

      this.TryToStartNewMesh(newMaterial);
    }

    public class ModelBuilderMaterial {
      public ModelBuilderMaterial Clone() {
        return new ModelBuilderMaterial {
            Bitmap = Bitmap,
            TextureInfo = TextureInfo,
            SegmentAddress = SegmentAddress,
            GeometryMode = GeometryMode
        };
      }

      public Bitmap Bitmap { get; set; }
      public TextureInfo TextureInfo { get; set; }
      public uint SegmentAddress { get; set; }
      public RspGeometryMode GeometryMode { get; set; }
    }

    public void TryToStartNewMesh(ModelBuilderMaterial material) {
      var lastTempMesh = this.TempMeshes.LastOrDefault();
      if ((lastTempMesh?.vertices.Count ?? -1) == 0) {
        lastTempMesh!.Material = material;
      } else {
        var newMesh = newTempMesh();
        newMesh.Material = material;
        TempMeshes.Add(newMesh);
      }

      CurrentMaterial = material;
    }

    public void AddTempVertex(Vector3 pos, Vector2 uv, Vector4 color) {
      pos += offset;
      if (currentScale != 1f)
        pos *= currentScale;
      //Console.WriteLine("currentMaterial = " + currentMaterial + ", totalCount = " + textureImages.Count);
      if (CurrentMaterial?.Bitmap == null) {
        AddTexture(
            TextureFormats.createColorTexture(System.Drawing.Color.White),
            newTexInfo((int) OpenTK.Graphics.OpenGL.All.Repeat,
                       (int) OpenTK.Graphics.OpenGL.All.Repeat),
            0x00000000
        );
      }

      var lastMesh = this.TempMeshes.Last();
      lastMesh.vertices.Add(pos);
      lastMesh.texCoords.Add(uv);
      lastMesh.colors.Add(color);
    }
    /*
    private void AddFinalVertex(Vector3 pos, Vector2 uv, Vector4 color)
    {
        finalMesh.vertices.Add(pos);
        finalMesh.texCoords.Add(uv);
        finalMesh.colors.Add(color);
    }*/

    private int doesVertexAlreadyExist(int index,
                                       Vector3 pos,
                                       Vector2 uv,
                                       Vector4 col) {
      TempMesh tmp = TempMeshes[index];
      for (int i = 0; i < tmp.final.vertices.Count; i++) {
        Vector3 v = tmp.final.vertices[i];
        if (pos.X == v.X && pos.Y == v.Y && pos.Z == v.Z) {
          Vector2 t = tmp.final.texCoords[i];
          if (uv.X == t.X && uv.Y == t.Y) {
            Vector4 c = tmp.final.colors[i];
            if (col.X == c.X && col.Y == c.Y && col.Z == c.Z && col.W == c.W) {
              return i;
            }
          }
        }
      }
      return -1;
    }

    public void BuildData(ref List<Model3D.MeshData> meshes) {
      finalMesh = newFinalMesh();
      for (int t = 0; t < TempMeshes.Count; t++) {
        TempMesh temp = TempMeshes[t];

        uint indexCount = 0;
        Model3D.MeshData md = new Model3D.MeshData();

        var material = temp.Material;
        Bitmap bmp = material.Bitmap;
        md.texture = ContentPipe.LoadTexture(ref bmp);
        md.texture.TextureParamS = material.TextureInfo.wrapS;
        md.texture.TextureParamT = material.TextureInfo.wrapT;

        temp.final.Material = material;

        for (int i = 0; i < temp.vertices.Count; i++) {
          int vExists =
              doesVertexAlreadyExist(t, temp.vertices[i], temp.texCoords[i],
                                     temp.colors[i]);
          if (vExists < 0) {
            Vector2 texCoord = temp.texCoords[i];
            texCoord.X /= (float) bmp.Width * 32.0f;
            texCoord.Y /= (float) bmp.Height * 32.0f;
            temp.final.vertices.Add(temp.vertices[i]);
            temp.final.texCoords.Add(texCoord);
            temp.final.colors.Add(temp.colors[i]);
            temp.final.indices.Add(indexCount);
            indexCount++;
          } else {
            temp.final.indices.Add((uint) vExists);
          }
        }
        meshes.Add(md);
      }
    }

    public ModelBuilderMaterial GetMaterial(int i) {
      return TempMeshes[i].final.Material;
    }

    public Vector3[] getVertices(int i) {
      return TempMeshes[i].final.vertices.ToArray();
    }

    public Vector2[] getTexCoords(int i) {
      return TempMeshes[i].final.texCoords.ToArray();
    }

    public Vector4[] getColors(int i) {
      return TempMeshes[i].final.colors.ToArray();
    }

    public uint[] getIndices(int i) {
      return TempMeshes[i].final.indices.ToArray();
    }

    public bool TryToReuseLoadedTexture(uint segmentAddress) {
      int index = 0;
      foreach (var tempMesh in this.TempMeshes) {
        if (tempMesh.Material.SegmentAddress == segmentAddress) {
          var currentGeometryMode = tempMesh.Material.GeometryMode;

          var newMaterial = tempMesh.Material.Clone();
          newMaterial.GeometryMode = currentGeometryMode;

          TryToStartNewMesh(newMaterial);

          return true;
        }
      }
      return false;
    }
  }
}