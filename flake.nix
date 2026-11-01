{
  description = "Kyanite";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs?ref=nixpkgs-unstable";
  };

  outputs =
    { nixpkgs, ... }:
    let
      pkgs = nixpkgs.legacyPackages.x86_64-linux;
    in
    {
      devShells.x86_64-linux.default = pkgs.mkShell {
        packages = with pkgs; [
          dotnet-sdk_10
          nuget-to-json
        ];

        DOTNET_ROOT = "${pkgs.dotnet-sdk_10}/share/dotnet";
        LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath (
          with pkgs;
          [
            fontconfig
            libx11
            libice
            libsm
          ]
        );
      };

      packages.x86_64-linux.default = pkgs.callPackage ./nix/package.nix { };
    };
}
