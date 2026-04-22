namespace BuildingBlocks.Behaviors
{
    public class ExceptionLoggingBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IRequest<TResponse>
        where TResponse : notnull
    {
        private readonly Logger<ExceptionLoggingBehavior<TRequest, TResponse>> logger;
        public ExceptionLoggingBehavior(Logger<ExceptionLoggingBehavior<TRequest, TResponse>> logger)
        {
            this.logger = logger;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception exception)
            {
                logger.LogError("Error : {exception}",exception);
                return await next();
            }
        }
    }
}
 