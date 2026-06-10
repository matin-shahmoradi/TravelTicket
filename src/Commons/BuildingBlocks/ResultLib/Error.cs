namespace BuildingBlocks
{
    public record struct Error
    {
        private Error(string message, int code, ErrorType errorType)
        {
            Message = message;
            Code = code;
            ErrorType = errorType;
        }
        public string Message { get; set; }
        public int Code { get; set; }
        public ErrorType ErrorType { get; set; }

        public static Error NotFoundError(
            string message = "Source not found!",
            int code = 404,
            ErrorType errorType = ErrorType.NOT_FOUND)
        {
            return new Error(message, code, errorType);
        }

        public static Error ValidationError(
            string message = "Validation error occured!",
            int code = 400,
            ErrorType errorType = ErrorType.VALIDATION_ERROR)
        {
            return new Error(message, code, errorType);
        }

        public static Error UnAuthorized(
            string message = "UnAuthorized error occured!",
            int code = 401,
            ErrorType errorType = ErrorType.UNAUTHORIZED_ERROR)
        {
            return new Error(message, code, errorType);
        }

        public static Error Forbidden(
            string message = "Aceess denied !",
            int code = 400,
            ErrorType errorType = ErrorType.FORBIDDEN_ERROR)
        {
            return new Error(message, code, errorType);
        }
        public static Error Conflict(
            string message = "Conflict error occured!",
            int code = 409,
            ErrorType errorType = ErrorType.CONFLICT_ERROR)
        {
            return new Error(message, code, errorType);
        }
        public static Error Internal_Server(
            string mesaage = "Internal server error occured!",
            int code = 500,
            ErrorType errorType = ErrorType.CUSTOM_ERROR)
        {
            return new Error(mesaage, code, errorType);
        }
        public static Error CustomError(
            string mesaage = "Something bad happened",
            int code = 500,
            ErrorType errorType = ErrorType.CUSTOM_ERROR)
        {
            return new Error(mesaage, code, errorType);
        }
    }
}
