using System.Collections.ObjectModel;

using Assimp;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

using Image = SixLabors.ImageSharp.Image;


namespace demo.mesh;

public class AssimpSceneData {
  public static AssimpSceneData Load(string path) {
    var basePath = Path.GetDirectoryName(path) ?? "";

    var assimpContext = new AssimpContext();
    var assimpScene = assimpContext.ImportFile(path);

    var texturesByFilePath =
        assimpScene
            .Materials
            .Select(
                assimpMaterial => {
                  var filePath = assimpMaterial.TextureDiffuse.FilePath;
                  if (filePath == null) {
                    return (assimpMaterial, null);
                  }

                  var texturePath = Path.Join(basePath, filePath);
                  var textureImage =
                      (Image<Rgba32>?) Image.Load<Rgba32>(texturePath);

                  return (assimpMaterial, textureImage);
                })
            .ToDictionary(
                filePathAndImage => filePathAndImage.assimpMaterial,
                filePathAndImage => filePathAndImage.textureImage);

    return new AssimpSceneData(assimpScene, texturesByFilePath);
  }

  private AssimpSceneData(
      Scene scene,
      IDictionary<Material, Image<Rgba32>?> texturesByFilePath) {
    this.Scene = scene;
    this.TexturesByMaterial =
        new ReadOnlyDictionary<Material, Image<Rgba32>?>(texturesByFilePath);
  }

  public Scene Scene { get; }

  public IReadOnlyDictionary<Material, Image<Rgba32>?> TexturesByMaterial {
    get;
  }
}