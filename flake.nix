{
	description = "Kyanite";

	inputs = {
		nixpkgs.url = "github:nixos/nixpkgs?ref=nixpkgs-unstable";
	};

	outputs = { nixpkgs, ... }:
	let
		pkgs = nixpkgs.legacyPackages.x86_64-linux;
	in
	{
		devShells.x86_64-linux.default = pkgs.mkShell {
			buildInputs = with pkgs; [
				dotnet-sdk_10 dotnet-runtime_10 nuget-to-json
			];
		};

		packages.x86_64-linux.default = pkgs.callPackage ./nix/package.nix {};
	};
}
