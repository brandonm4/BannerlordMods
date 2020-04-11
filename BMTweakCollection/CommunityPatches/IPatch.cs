using TaleWorlds.Core;

namespace CommunityPatch {

  public interface IPatch {

    bool IsApplicable(Game game);

    void Apply(Game game);
    
    bool Applied { get; }

    void Reset();

  }

}