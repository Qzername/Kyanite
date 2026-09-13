{
	lib,
	buildDotnetModule,
	dotnetCorePackages,
}:

buildDotnetModule (finalAttrs: {
	pname = "kyanite";
	version = "0.1.0";

	src = ../.;
	projectFile = "App/Kyanite.Desktop/Kyanite.Desktop.csproj";

	nugetDeps = ./deps.json;
	dotnet-sdk = dotnetCorePackages.sdk_10_0;
	dotnet-runtime = dotnetCorePackages.runtime_10_0;

	meta = {
		description = "Simple work managment tool";
		changelog = "https://github.com/Qzername/Kyanite/releases/tag/${finalAttrs.version}v";
		license = lib.licenses.mit;
		maintainers = with lib.maintainers; [
			justkrysteq
		];
		mainProgram = "Kyanite.Desktop";
		platforms = lib.platforms.all;
	};
})
