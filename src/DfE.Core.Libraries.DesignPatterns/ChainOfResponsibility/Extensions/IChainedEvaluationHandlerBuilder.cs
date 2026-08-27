namespace DfE.Core.Libraries.DesignPatterns.ChainOfResponsibility.Extensions;

public interface IChainedEvaluationHandlerBuilder<TIn>
{
    IChainedEvaluationHandlerBuilder<TIn> AddScoped<THandler>() where THandler : BaseEvaluationHandler<TIn>;
    IChainedEvaluationHandlerBuilder<TIn> AddScoped<THandler>(Func<IServiceProvider, THandler> factory) where THandler : BaseEvaluationHandler<TIn>;
    IChainedEvaluationHandlerBuilder<TIn> AddTransient<THandler>() where THandler : BaseEvaluationHandler<TIn>;
    IChainedEvaluationHandlerBuilder<TIn> AddTransient<THandler>(Func<IServiceProvider, THandler> factory) where THandler : BaseEvaluationHandler<TIn>;
    IChainedEvaluationHandlerBuilder<TIn> AddSingleton<THandler>() where THandler : BaseEvaluationHandler<TIn>;
    IChainedEvaluationHandlerBuilder<TIn> AddSingleton<THandler>(Func<IServiceProvider, THandler> factory) where THandler : BaseEvaluationHandler<TIn>;
}
