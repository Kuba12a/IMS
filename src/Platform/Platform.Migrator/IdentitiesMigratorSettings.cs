using Common.Types.Settings;

namespace Platform.Migrator;

public class IdentitiesMigratorSettings : IValidatable
{
    public PostgresSettings PostgresSettings { get; set; }

    public void Validate()
    {
        PostgresSettings.Validate();
    }
}
