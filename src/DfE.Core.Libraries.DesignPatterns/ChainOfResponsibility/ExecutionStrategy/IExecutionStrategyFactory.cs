using DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Options;

namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.ExecutionStrategy;

internal interface IExecutionStrategyFactory<TIn, THandler> where THandler : IEvaluationHandler<TIn>
{
    IExecutionStrategy<TIn, THandler> Create(EvaluationOptions options);
}
