using System.Text.Json.Serialization;
using System.Windows.Controls;

namespace GameTheoryUtility.Logic.Contexts;

[JsonDerivedType(typeof(PracticeContext), "practice")]
[JsonDerivedType(typeof(GenerationContext), "generation")]
public abstract partial class EnvironmentContext : IEnvironmentContext
{
    public string? Name { get; set; }

    public event Action<IEnvironmentContext>? Closed;
    public Page? Visual { get; set; }

    public static EnvironmentContext OpenAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public void Close()
    {
        Closed?.Invoke(this);
    }
}
