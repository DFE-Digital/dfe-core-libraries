namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;

public interface IChainedHandlerBuilder<TIn>
{
    IChainedHandlerBuilder<TIn> AddScoped<THandler>() where THandler : BaseEvaluationHandler<TIn>;

    IChainedHandlerBuilder<TIn> AddTransient<THandler>() where THandler : BaseEvaluationHandler<TIn>;

    IChainedHandlerBuilder<TIn> AddSingleton<THandler>() where THandler : BaseEvaluationHandler<TIn>;
}
