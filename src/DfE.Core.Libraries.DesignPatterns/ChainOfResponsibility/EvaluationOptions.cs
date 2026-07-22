using System.ComponentModel;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public sealed class EvaluationOptions
{
    public EvaluationMode Mode { get; set; } = EvaluationMode.ChainOfResponsibility;
}

public enum EvaluationMode
{
    [Description("First handleable handler processes the input")]
    ChainOfResponsibility = 0,
}
