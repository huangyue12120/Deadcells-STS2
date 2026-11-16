import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import sys
import tempfile
from zipfile import ZIP_DEFLATED, ZipFile


ROOT = Path(__file__).resolve().parent.parent
REFERENCE_FILES = ("sts2.dll", "0Harmony.dll", "Steamworks.NET.dll")
GAME_VERSION = "v0.107.1"


def require_file(path):
    if not path.is_file() or path.stat().st_size == 0:
        raise ValueError(f"Missing or empty file: {path}")


def run(arguments, environment=None):
    subprocess.run(
        [str(argument) for argument in arguments], cwd=ROOT, check=True, env=environment
    )


def read_game_info(path):
    require_file(path)
    info = json.loads(path.read_text(encoding="utf-8-sig"))
    if info.get("version") != GAME_VERSION:
        raise ValueError(
            f"Expected game {GAME_VERSION}, got {info.get('version')!r}. "
            "Migrate the mod before changing the build baseline; see README.md."
        )
    return info


def prepare_references(args):
    data_dir = Path(args.data_dir).resolve()
    for name in REFERENCE_FILES:
        require_file(data_dir / name)
    release_info = data_dir.parent / "release_info.json"
    info = read_game_info(release_info)
    destination = Path(args.output).resolve()
    destination.parent.mkdir(parents=True, exist_ok=True)
    with ZipFile(destination, "w", ZIP_DEFLATED) as bundle:
        for name in REFERENCE_FILES:
            bundle.write(data_dir / name, name)
        bundle.write(release_info, "release_info.json")
    print(f"Reference bundle for {info['version']}: {destination}")
    print(f"SHA256: {hashlib.sha256(destination.read_bytes()).hexdigest()}")


def build(args):
    references = Path(args.references).resolve()
    for name in REFERENCE_FILES:
        require_file(references / name)
    info = read_game_info(references / "release_info.json")
    manifest = json.loads((ROOT / "Deadcells.json").read_text(encoding="utf-8"))
    if manifest["id"] != "Deadcells" or not manifest["has_dll"] or not manifest["has_pck"]:
        raise ValueError("Manifest must declare Deadcells with both DLL and PCK.")
    if manifest.get("min_game_version") != GAME_VERSION.removeprefix("v"):
        raise ValueError("Manifest min_game_version must match the build baseline.")
    lock = json.loads((ROOT / "packages.lock.json").read_text(encoding="utf-8"))
    baselib_version = lock["dependencies"]["net9.0"]["Alchyr.Sts2.BaseLib"]["resolved"]
    if {"id": "BaseLib", "min_version": baselib_version} not in manifest["dependencies"]:
        raise ValueError("Manifest BaseLib dependency must match packages.lock.json.")
    if args.version:
        manifest["version"] = args.version
    if not re.fullmatch(r"\d+\.\d+\.\d+(?:-[0-9A-Za-z.-]+)?", manifest["version"]):
        raise ValueError("Version must use X.Y.Z or X.Y.Z-prerelease syntax.")
    godot = Path(args.godot).resolve()
    require_file(godot)
    require_file(ROOT / "windows" / "libspine_godot.windows.editor.x86_64.dll")
    godot_version = subprocess.check_output(
        [str(godot), "--version"], cwd=ROOT, text=True
    ).strip()
    if not godot_version.startswith("4.5.1.stable.mono."):
        raise ValueError(f"Expected Godot 4.5.1 .NET, got: {godot_version}")
    output = Path(args.output).resolve()
    output.mkdir(parents=True, exist_ok=True)
    archive = output / "Deadcells.zip"
    archive.unlink(missing_ok=True)
    properties = [f"-p:Sts2DataDir={references}", "-p:CopyToGame=false"]
    environment = os.environ.copy()
    environment.update(
        Sts2DataDir=str(references), CopyToGame="false",
        RestoreLockedMode="true", Version=manifest["version"],
    )
    print(
        f"Building against game {info['version']}; BaseLib {baselib_version}; "
        f"Godot {godot_version}", flush=True,
    )
    with tempfile.TemporaryDirectory(prefix="deadcells-build-") as temporary:
        stage = Path(temporary)
        compiled = stage / "compiled"
        mod_dir = stage / "Deadcells"
        mod_dir.mkdir()
        run(["dotnet", "restore", "Deadcells.csproj", "--locked-mode", *properties], environment)
        run([
            "dotnet", "build", "Deadcells.csproj", "-c", "Release",
            "--no-restore", "--output", compiled, *properties,
            f"-p:Version={manifest['version']}",
        ], environment)
        require_file(compiled / "Deadcells.dll")
        run([godot, "--headless", "--path", ROOT, "--editor", "--import"], environment)
        run([
            godot, "--headless", "--path", ROOT, "--export-pack",
            "Windows Desktop", mod_dir / "Deadcells.pck",
        ], environment)
        require_file(mod_dir / "Deadcells.pck")
        with (mod_dir / "Deadcells.pck").open("rb") as stream:
            if stream.read(4) != b"GDPC":
                raise ValueError("Export did not produce a Godot PCK.")
        shutil.copy2(compiled / "Deadcells.dll", mod_dir / "Deadcells.dll")
        (mod_dir / "Deadcells.json").write_text(
            json.dumps(manifest, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
        )
        temporary_archive = stage / "Deadcells.zip"
        with ZipFile(temporary_archive, "w", ZIP_DEFLATED) as bundle:
            for suffix in ("dll", "pck", "json"):
                name = f"Deadcells.{suffix}"
                bundle.write(mod_dir / name, f"Deadcells/{name}")
        with ZipFile(temporary_archive) as bundle:
            expected = {f"Deadcells/Deadcells.{suffix}" for suffix in ("dll", "pck", "json")}
            if set(bundle.namelist()) != expected or bundle.testzip() is not None:
                raise ValueError("Package structure or CRC validation failed.")
        shutil.copy2(temporary_archive, archive)
    print(f"Package validated: {archive}")


def main():
    parser = argparse.ArgumentParser(description="Build and package the Deadcells mod.")
    commands = parser.add_subparsers(dest="command", required=True)
    prepare = commands.add_parser("prepare-references", help="Prepare CI game references.")
    prepare.add_argument("--data-dir", required=True)
    prepare.add_argument("--output", default=".build/sts2-references.zip")
    prepare.set_defaults(action=prepare_references)
    package = commands.add_parser("package", help="Compile DLL, export PCK and validate ZIP.")
    package.add_argument("--references", required=True)
    package.add_argument("--godot", required=True)
    package.add_argument("--output", default="dist")
    package.add_argument("--version")
    package.set_defaults(action=build)
    args = parser.parse_args()
    try:
        args.action(args)
    except (ValueError, OSError, subprocess.CalledProcessError) as error:
        print(f"Build failed: {error}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
