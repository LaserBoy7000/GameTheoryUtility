using GameTheoryUtility.Logic.Game;

namespace GameTheoryUtility.Logic.Contexts;

public class PracticeContext(GameEnvironment environment) : EnvironmentContext
{
    public PracticeContext() : this(new())
    {
        Name = "Нова практика";
    }

    public GameEnvironment Game => environment;
}
