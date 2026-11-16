{
  lib,
  buildDotnetModule,
  dotnetCorePackages,
  makeWrapper,
  libnotify,
  wayland,
  libxkbcommon,
  makeDesktopItem,
  copyDesktopItems,
}:

buildDotnetModule (finalAttrs: {
  pname = "kyanite";
  version = "0.1.0";

  src = ../.;
  projectFile = "App/Kyanite.Desktop/Kyanite.Desktop.csproj";

  nugetDeps = ./deps.json;
  dotnet-sdk = dotnetCorePackages.sdk_10_0;
  dotnet-runtime = dotnetCorePackages.runtime_10_0;

  buildInputs = [
    wayland
    libxkbcommon
  ];

  nativeBuildInputs = [
    makeWrapper
    copyDesktopItems
  ];

  postInstall = ''
    for icon in App/Kyanite/Assets/Icons/*.png; do
      size="$(basename "$icon" .png)"
      install -Dm644 "$icon" "$out/share/icons/hicolor/$size/apps/kyanite.png"
    done

    install -Dm644 App/Kyanite/Assets/Icons/Scalable/Kyanite.svg "$out/share/icons/hicolor/scalable/apps/kyanite.svg"
  '';

  postFixup = ''
    makeWrapper $out/bin/Kyanite.Desktop $out/bin/kyanite \
      --prefix LD_LIBRARY_PATH : "${lib.makeLibraryPath [ wayland libxkbcommon ]}" \
      --prefix PATH : "${lib.makeBinPath [ libnotify ]}"
  '';

  desktopItems = [
    (makeDesktopItem {
      name = "kyanite";
      desktopName = "Kyanite";
      exec = "kyanite %U";
      icon = "kyanite";
      comment = "Simple work managment tool";
      categories = [
        "Office"
        "Utility"
        "TextEditor"
      ];
      startupWMClass = "Kyanite";
      mimeTypes = [
        "text/markdown"
      ];
    })
  ];

  meta = {
    description = "Simple work managment tool";
    changelog = "https://github.com/Qzername/Kyanite/releases/tag/${finalAttrs.version}v";
    license = lib.licenses.mit;
    maintainers = with lib.maintainers; [
      justkrysteq
    ];
    mainProgram = "kyanite";
    platforms = lib.platforms.linux ++ lib.platforms.darwin;
  };
})
