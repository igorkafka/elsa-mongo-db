using System.ComponentModel.DataAnnotations;

namespace ElsaServer.Configuration
{
    public class MongoDbSettings : IValidatableObject
    {
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Required]
        public string DatabaseName { get; set; } = "elsa-workflows";

        public string CollectionPrefix { get; set; } = "Elsa";

        public int? ConnectionTimeoutSeconds { get; set; } = 30;

        public int? MaxConnectionPoolSize { get; set; } = 100;

        public int? MinConnectionPoolSize { get; set; } = 10;

        public bool? RetryReads { get; set; } = true;

        public bool? RetryWrites { get; set; } = true;

        public string? WriteConcern { get; set; }

        public string? ReadPreference { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate connection string format
            if (!string.IsNullOrEmpty(ConnectionString))
            {
                if (!ConnectionString.StartsWith("mongodb://") &&
                    !ConnectionString.StartsWith("mongodb+srv://"))
                {
                    yield return new ValidationResult(
                        "ConnectionString must start with mongodb:// or mongodb+srv://",
                        new[] { nameof(ConnectionString) });
                }
            }

            // Validate connection pool sizes
            if (MaxConnectionPoolSize.HasValue && MinConnectionPoolSize.HasValue)
            {
                if (MinConnectionPoolSize > MaxConnectionPoolSize)
                {
                    yield return new ValidationResult(
                        "MinConnectionPoolSize cannot be greater than MaxConnectionPoolSize",
                        new[] { nameof(MinConnectionPoolSize), nameof(MaxConnectionPoolSize) });
                }
            }

            // Validate database name
            if (!string.IsNullOrEmpty(DatabaseName))
            {
                if (DatabaseName.Length > 64)
                {
                    yield return new ValidationResult(
                        "DatabaseName cannot exceed 64 characters",
                        new[] { nameof(DatabaseName) });
                }

                if (DatabaseName.Contains(' ') || DatabaseName.Contains('\\') || DatabaseName.Contains('/'))
                {
                    yield return new ValidationResult(
                        "DatabaseName cannot contain spaces, backslashes, or forward slashes",
                        new[] { nameof(DatabaseName) });
                }
            }

            // Validate WriteConcern
            if (!string.IsNullOrEmpty(WriteConcern))
            {
                var validWriteConcerns = new[] {
                "Unacknowledged", "Acknowledged", "Majority", "W2", "W3", "W4", "W5"
            };

                if (!validWriteConcerns.Contains(WriteConcern))
                {
                    yield return new ValidationResult(
                        $"WriteConcern must be one of: {string.Join(", ", validWriteConcerns)}",
                        new[] { nameof(WriteConcern) });
                }
            }

            // Validate ReadPreference
            if (!string.IsNullOrEmpty(ReadPreference))
            {
                var validReadPreferences = new[] {
                "Primary", "PrimaryPreferred", "Secondary", "SecondaryPreferred", "Nearest"
            };

                if (!validReadPreferences.Contains(ReadPreference))
                {
                    yield return new ValidationResult(
                        $"ReadPreference must be one of: {string.Join(", ", validReadPreferences)}",
                        new[] { nameof(ReadPreference) });
                }
            }
        }
    }
}
