{
  description = "Entorno de desarrollo para iperia-backend";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = import nixpkgs {
          inherit system;
          config.allowUnfree = true;
        };
      in
      {
        devShells.default = pkgs.mkShell {
          buildInputs = with pkgs; [
            (dotnetCorePackages.combinePackages [
              dotnetCorePackages.sdk_8_0
            ])
            gh
            awscli2
            zip
            postgresql_17
          ];

          shellHook = ''
            echo "Bienvenido al entorno iperia-backend"

            if command -v dotnet &> /dev/null; then
              echo ".NET Version: $(dotnet --version)"
            fi

            if command -v gh &> /dev/null; then
              echo "GitHub CLI Version: $(gh --version | head -n1)"
            fi

            if command -v aws &> /dev/null; then
              echo "AWS CLI Version: $(aws --version)"
            fi

            if command -v psql &> /dev/null; then
              echo "psql Version: $(psql --version)"
            fi

            echo "--------------------------------------------------------"
            echo "Sistema detectado: ${system}"
            echo "--------------------------------------------------------"
          '';
        };
      }
    );
}
