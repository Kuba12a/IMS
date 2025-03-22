DOTNET_ENVIRONMENT ?= Development
MIGRATOR_STARTUP_PROJECT ?= Platform.Migrator
MIGRATIONS_PROJECT ?= Platform.Migrator
SERVICE_TEST_DIR ?= Platform.Tests

# ------------------------------------------------ DATABASE --------------------------------------------------------
regenerate-drop-migrate: regenerate-initial-migration drop-database migrate-latest

drop-migrate-seed: drop-database migrate-latest seed

regenerate-initial-migration:
	cd ${MIGRATIONS_PROJECT} && \
	rm -rf Migrations && \
    DOTNET_ENVIRONMENT=${DOTNET_ENVIRONMENT} MIGRATIONS_ASSEMBLY=${MIGRATIONS_PROJECT} dotnet ef migrations add InitialMigration \
    	--context WriteDbContext \
		--startup-project ./${MIGRATOR_STARTUP_PROJECT}.csproj \
    	--output-dir Migrations

add-new-migration:
	echo "Migration name: " && read MIGRATION_NAME && \
	cd ${MIGRATIONS_PROJECT} && \
	DOTNET_ENVIRONMENT=${DOTNET_ENVIRONMENT} MIGRATIONS_ASSEMBLY=${MIGRATIONS_PROJECT} MIGRATIONS_GENERATION=true dotnet ef migrations add $$MIGRATION_NAME \
		--context WriteDbContext \
		--startup-project ../${MIGRATOR_STARTUP_PROJECT}/${MIGRATOR_STARTUP_PROJECT}.csproj	\
		--output-dir Migrations

remove-last-migration:
	cd ${MIGRATIONS_PROJECT} && \
	DOTNET_ENVIRONMENT=${DOTNET_ENVIRONMENT} MIGRATIONS_ASSEMBLY=${MIGRATIONS_PROJECT} MIGRATIONS_GENERATION=true dotnet ef migrations remove \
		--context WriteDbContext \
		--startup-project ./${MIGRATOR_STARTUP_PROJECT}.csproj

drop-database:
	cd ${MIGRATOR_STARTUP_PROJECT} && DOTNET_ENVIRONMENT=Development dotnet ef database drop --context WriteDbContext -f

migrate-latest:
	cd ${MIGRATOR_STARTUP_PROJECT} && DOTNET_ENVIRONMENT=${DOTNET_ENVIRONMENT} MIGRATIONS_ASSEMBLY=${MIGRATIONS_PROJECT} dotnet run migrator



.PHONY: rollback
rollback:
	cd ${MIGRATOR_STARTUP_PROJECT} && \
	echo "Migration name to rollback to: " && read MIGRATION_NAME && \
	DOTNET_ENVIRONMENT=Development dotnet ef database update $$MIGRATION_NAME

# -------------------------- LOCAL TOOLS -------------------------------
restore-local-tools:
	cd ${MIGRATOR_STARTUP_PROJECT} && dotnet tool restore
