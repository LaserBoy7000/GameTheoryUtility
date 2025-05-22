using System.Text.Json.Serialization;

namespace GameTheoryUtility.Logic.Contexts;

[JsonDerivedType(typeof(PracticeContext), "practice")]
[JsonDerivedType(typeof(GenerationContext), "generation")]
public interface IEnvironmentContext
{
    public string? Name { get; }
}