import argparse
from pathlib import Path
import shutil
import subprocess
import sys


SOURCE = Path(__file__).resolve().parent.parent
TARGET = Path("/mnt/softwares/stsmod/Deadcells-STS2")
FILES = (
    ".github/workflows/build.yml",
    ".gitignore",
    "Deadcells.csproj",
    "Deadcells.json",
    "export_presets.cfg",
    "global.json",
    "local.props.example",
    "packages.lock.json",
    "README.md",
    "tools/build.py",
    "tools/migrate_env01.py",
)


def migrate(target, apply):
    target = target.resolve()
    if target == SOURCE or not (target / ".git").exists():
        raise ValueError("Target must be a separate Git checkout.")
    status = subprocess.check_output(
        ["git", "-C", str(target), "status", "--porcelain"], text=True
    )
    if status.strip():
        raise ValueError("Target checkout has local changes; commit or preserve them first.")
    changed = []
    for name in FILES:
        source = SOURCE / name
        destination = target / name
        if not source.is_file():
            raise ValueError(f"Missing migration source: {source}")
        if not destination.resolve().is_relative_to(target):
            raise ValueError(f"Destination escapes target checkout: {destination}")
        if destination.is_file() and source.read_bytes() == destination.read_bytes():
            continue
        changed.append(name)
    print(f"Source: {SOURCE}")
    print(f"Target: {target}")
    for name in changed:
        print(f"{'COPY' if apply else 'WOULD COPY'} {name}")
    if apply:
        for name in changed:
            destination = target / name
            destination.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(SOURCE / name, destination)
        print(f"Migrated {len(changed)} files. Review git diff before committing.")
    else:
        print(f"{len(changed)} files would change. Add --apply to copy them.")


def main():
    parser = argparse.ArgumentParser(
        description="Migrate ENV-01 build files to the GitHub checkout; exclude local docs."
    )
    parser.add_argument("--target", type=Path, default=TARGET)
    parser.add_argument("--apply", action="store_true")
    args = parser.parse_args()
    try:
        migrate(args.target, args.apply)
    except (ValueError, OSError, subprocess.CalledProcessError) as error:
        print(f"Migration failed: {error}", file=sys.stderr)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
