import os
import subprocess
import shutil
import sys
from pathlib import Path


def find_msbuild():
    msbuild_path = shutil.which("msbuild")
    if msbuild_path:
        return msbuild_path
    vswhere_paths = [r"C:\Program Files\Microsoft Visual Studio\Installer\vswhere.exe", r"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe"]
    for vswhere in vswhere_paths:
        if Path(vswhere).exists():
            try:
                result = subprocess.run(
                    [vswhere, "-latest", "-requires", "Microsoft.Component.MSBuild", "-find", "MSBuild\\**\\Bin\\MSBuild.exe"],
                    capture_output=True,
                    text=True,
                    encoding="utf-8",
                    errors="ignore",
                )
                if result.returncode == 0:
                    lines = result.stdout.strip().splitlines()
                    if lines:
                        return lines[0]
            except:
                pass
    return None


def build_solution(solution_path, msbuild_path, configuration="Release"):
    print(f"[构建] {solution_path}")
    command = [msbuild_path, str(solution_path), f"/p:Configuration={configuration}", "/t:Rebuild", "/m"]
    result = subprocess.run(command, capture_output=True, text=True, encoding="utf-8", errors="replace")
    if result.returncode != 0:
        print(f"构建失败: {solution_path}")
        print(result.stderr)
        return False
    print(f"构建成功: {solution_path}")
    return True


def find_solution_files(root_directory):
    solution_files = []
    for directory_path, directory_names, file_names in os.walk(root_directory):
        if "obj" in directory_path or "bin" in directory_path:
            continue
        for file_name in file_names:
            if file_name.endswith(".sln"):
                solution_files.append(Path(directory_path) / file_name)
    return solution_files


def collect_artifacts(root_directory, output_directory):
    output_directory.mkdir(parents=True, exist_ok=True)
    collected_files = set()
    main_executable = root_directory / "Stroke" / "Stroke" / "bin" / "Release" / "Stroke.exe"
    configure_executable = root_directory / "Stroke" / "Stroke.Configure" / "bin" / "Release" / "Stroke.Configure.exe"
    base_library = root_directory / "Stroke" / "Base" / "bin" / "Release" / "Base.dll"
    for source_file in [main_executable, configure_executable, base_library]:
        if source_file.exists():
            destination_file = output_directory / source_file.name
            shutil.copy2(source_file, destination_file)
            print(f"复制: {source_file} -> {destination_file}")
            collected_files.add(source_file.name)
    for directory_path, _, file_names in os.walk(root_directory):
        if "bin" + os.sep + "Release" in directory_path and "obj" not in directory_path:
            for file_name in file_names:
                if file_name.endswith(".dll") and file_name not in collected_files:
                    source_file = Path(directory_path) / file_name
                    destination_file = output_directory / file_name
                    shutil.copy2(source_file, destination_file)
                    print(f"复制: {source_file} -> {destination_file}")
                    collected_files.add(file_name)
    print(f"\n共复制 {len(collected_files)} 个文件到 {output_directory}")


def main():
    if getattr(sys, "frozen", False):
        script_directory = Path(sys.argv[0]).parent
    else:
        script_directory = Path(__file__).parent.absolute()
    root_directory = script_directory.parent
    os.chdir(root_directory)
    print(f"根目录: {root_directory}")
    print(f"脚本目录: {script_directory}")
    msbuild = find_msbuild()
    if not msbuild:
        print("错误: 找不到 MSBuild.exe。")
        print("请安装 Visual Studio 或 .NET Framework Build Tools。")
        os.system("pause")
        sys.exit(1)
    print(f"使用 MSBuild: {msbuild}\n")
    main_solution = root_directory / "Stroke" / "Stroke.sln"
    if not main_solution.exists():
        print("错误: 找不到 Stroke.sln")
        os.system("pause")
        sys.exit(1)
    if not build_solution(main_solution, msbuild):
        os.system("pause")
        sys.exit(1)
    all_solutions = find_solution_files(root_directory)
    plugin_solutions = [solution for solution in all_solutions if solution != main_solution]
    for plugin_solution in plugin_solutions:
        build_solution(plugin_solution, msbuild)
    release_directory = root_directory / "Release"
    collect_artifacts(root_directory, release_directory)
    print("\n全部完成！")
    os.system("pause")


if __name__ == "__main__":
    main()
