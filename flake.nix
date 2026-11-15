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
          libnotify
        ];

        DOTNET_ROOT = "${pkgs.dotnet-sdk_10}/share/dotnet";
        LD_LIBRARY_PATH = pkgs.lib.makeLibraryPath (
          with pkgs; [
            # X
            fontconfig
            libx11
            libice
            libsm
            libxcursor
            libxrandr
            libxi
            libxext
            libxfixes
            libglvnd
            mesa

            # Wayland
            wayland
            libxkbcommon
          ]
        );
      };

      packages.x86_64-linux.default = pkgs.callPackage ./nix/package.nix { };
    };
}
