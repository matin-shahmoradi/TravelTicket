using Serilog.Core;
using Serilog.Events;
using System.Diagnostics.CodeAnalysis;

namespace BuildingBlocks.Serilog
{
    public sealed class SensitiveDataDestructuringPolicy : IDestructuringPolicy
    {
        private static readonly HashSet<string> SensitivePropertyNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Password",
            "ConfirmPassword",
            "Token",
            "AccessToken",
            "RefreshToken",
            "Key",
            "SecretKey",
            "CardNumber"
        };
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, [NotNull] out LogEventPropertyValue? result)
        {
            var props = value.GetType().GetProperties();
            var valueType = value.GetType();
            var logEventProperties = new List<LogEventProperty>();

            foreach (var prop in props)
            {
                var isSensitive = SensitivePropertyNames.Contains(prop.Name);

                var logValue = isSensitive
                    ? new ScalarValue("***REDACTED***")
                    : propertyValueFactory.CreatePropertyValue(prop.GetValue(value), true);

                logEventProperties.Add(new LogEventProperty(prop.Name, logValue));
            }

            result = new StructureValue(logEventProperties, valueType.Name);
            return true;
        }
    }
}
