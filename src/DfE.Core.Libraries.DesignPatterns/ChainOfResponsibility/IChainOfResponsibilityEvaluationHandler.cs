namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility;

public interface IChainOfResponsibilityEvaluationHandler<in TIn> : IEvaluationHandler<TIn>
{
    bool CanHandle(TIn input);
}
