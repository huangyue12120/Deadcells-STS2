# Deadcells Mod for Slay the Spire 2

## 构建基线

当前构建基线为游戏 **v0.111.1**（commit `cc1f9462`）：.NET SDK **9.0.321**、Godot **4.5.1 .NET**、NuGet **Alchyr.Sts2.BaseLib 3.4.8**。依赖由 `global.json` 与 `packages.lock.json` 固定；MIG-01 已完成目标程序集编译适配，游戏内回归仍需在安装 BaseLib 3.4.8 的运行环境中验证。

## GitHub Actions

本地只需准备游戏引用包；后续使用 Windows runner 编译和导出，无需本机安装 .NET 或 Godot。

1. 用 Python 3.10+ 在项目根目录准备引用包：

   ```powershell
   python tools/build.py prepare-references --data-dir "D:/Steam/steamapps/common/Slay the Spire 2/data_sts2_windows_x86_64"
   ```

   输出 `.build/sts2-references.zip` 及 SHA256。ZIP 根目录直接包含 `sts2.dll`、`0Harmony.dll`、`Steamworks.NET.dll`、`release_info.json`，版本必须是 `v0.111.1`。

2. 把引用包存放于自己控制的 HTTPS 下载地址。不要把游戏程序集提交到公共仓库或发布附件。

3. 在仓库 Settings → Secrets and variables → Actions 配置：

   | 类型 | 名称 | 内容 |
   | --- | --- | --- |
   | Secret | `STS2_REFERENCES_URL` | runner 可访问的引用 ZIP 的 HTTPS 直链 |
   | Variable | `STS2_REFERENCES_SHA256` | 同一 ZIP 的 64 位 SHA256 |

4. 进入 Actions → Build Deadcells → Run workflow。构建会校验下载、锁定依赖、编译 DLL、用现有 Windows Spine 扩展导入资源，并导出 PCK。

5. 下载 `Deadcells` artifact，解开 GitHub 的外层下载包即可取得 `Deadcells.zip`：

   ```text
   Deadcells.zip
   └── Deadcells/
       ├── Deadcells.dll
       ├── Deadcells.pck
       └── Deadcells.json
   ```

推送 `vX.Y.Z` 标签会使用标签版本构建，并创建带 ZIP 附件的 Release 草稿。完成游戏加载检查后再发布草稿。此工作流不上传 Steam 工坊。

PCK 使用 `msil` 预设，不包含项目里的 Spine 本机库；游戏运行时提供 Spine 扩展。C# 脚本保留资源路径占位而不发布源码。资源导出成功后仍需在目标游戏与匹配的 BaseLib 运行时中检查角色、文本及动画。

## 本地 Windows 构建

安装上述 .NET SDK、Godot .NET 和 Python。复制 `local.props.example` 为 `local.props`，修改游戏路径：

```powershell
dotnet restore Deadcells.csproj --locked-mode
dotnet build Deadcells.csproj -c Release --no-restore
```

完整打包先准备引用 ZIP，再运行：

```powershell
Expand-Archive .build/sts2-references.zip .build/references -Force
python tools/build.py package --references .build/references --godot "D:/Tools/Godot_v4.5.1-stable_mono_win64/Godot_v4.5.1-stable_mono_win64_console.exe"
```

输出为 `dist/Deadcells.zip`。可用 `--version 0.2.1` 覆盖本次打包版本。默认 `CopyToGame=false`，不会自动复制到游戏目录。

## 从开发副本更新此仓库

在 `/mnt/softwares/stsmod/STS2/deadcells` 运行：

```bash
python3 tools/migrate_env01.py
python3 tools/migrate_env01.py --apply
```

脚本默认预览，只复制 ENV-01 文件清单，目标为 `/mnt/softwares/stsmod/Deadcells-STS2`，且要求目标工作区干净。它不迁移本地 `docs/`、`AGENTS.md`、游戏引用、本机配置、缓存、玩法或美术资源，也不自动提交推送。复制后在目标仓库检查 `git diff`，再提交。

## 参考

- [SlayTheSpire2ModdingTutorials](https://github.com/GlitchedReme/SlayTheSpire2ModdingTutorials)
- [BaseLib Wiki](https://alchyr.github.io/BaseLib-Wiki/)
