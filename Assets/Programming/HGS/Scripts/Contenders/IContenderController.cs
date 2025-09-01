using System.Collections.Generic;

namespace SHG
{
  public interface IContenderController 
  {
    public Dictionary<IGroup, List<IContender>> Althetes { get; }
    public Team[] Teams { get; }
  }
}
