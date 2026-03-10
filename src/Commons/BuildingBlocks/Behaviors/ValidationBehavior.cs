namespace BuildingBlocks.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> :
        IPipelineBehavior<TRequest, TResponse>
         where TRequest : ICommand<TResponse>
         where TResponse : BaseResult
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures =
                validationResults
                .Where(v => v.Errors.Any())
                .SelectMany(e => e.Errors)
                .ToList();

            if (failures.Any())
            {
                var message = string.Join(" | ", failures.Select(x => x.ErrorMessage));

                var error = Error.ValidationError(message);

                return (TResponse)typeof(TResponse)
                .GetMethod("Failure")!
                .Invoke(null, new object[] { error })!;
            }
            return await next();
        }
    }
}

