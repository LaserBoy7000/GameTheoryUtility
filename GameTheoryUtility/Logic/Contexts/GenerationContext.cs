using GameTheoryUtility.Logic.Game;

namespace GameTheoryUtility.Logic.Contexts;

public class GenerationContext : EnvironmentContext
{
    public GenerationContext()
    {
        Name = "Нові завдання";
    }

    public List<GameEnvironment> Templates { get; init; } = [];
    public List<GameEnvironment> Instances { get; init; } = [];
}
