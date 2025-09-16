using System.ComponentModel;


namespace Quad64.src.LevelInfo;

public class CustomSortedCategoryAttribute : CategoryAttribute {
  private const char NonPrintableChar = '\t';

  public CustomSortedCategoryAttribute(string category,
                                       ushort categoryPos,
                                       ushort totalCategories)
      : base(category.PadLeft(
                 category.Length + (totalCategories - categoryPos),
                 NonPrintableChar)) { }
}