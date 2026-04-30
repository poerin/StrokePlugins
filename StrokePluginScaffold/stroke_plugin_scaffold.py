import os
import uuid
import locale


def get_system_language():
    """
    Detect system language; return 'zh' for Chinese, 'en' otherwise.
    """
    try:
        language, _ = locale.getdefaultlocale()
        if language and language.startswith("zh"):
            return "zh"
    except Exception:
        pass
    return "en"


def generate_guid(uppercase=True):
    guid_string = str(uuid.uuid4())
    if uppercase:
        guid_string = guid_string.upper()
    return "{" + guid_string + "}"


def write_file(file_path, content, add_bom=False):
    os.makedirs(os.path.dirname(file_path), exist_ok=True)
    content = content.replace("\r\n", "\n").replace("\n", "\r\n")
    encoding = "utf-8-sig" if add_bom else "utf-8"
    with open(file_path, "w", encoding=encoding, newline="") as f:
        f.write(content)


def generate_sln(plugin_name, project_guid, solution_guid, reference_stroke):
    type_guid = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
    stroke_guid = "{4BF7E1C7-8683-47A3-99FF-76ECEDA97BB4}"

    solution_content = "\nMicrosoft Visual Studio Solution File, Format Version 12.00\n"
    solution_content += "# Visual Studio Version 18\n"
    solution_content += "VisualStudioVersion = 18.5.11716.220 stable\n"
    solution_content += "MinimumVisualStudioVersion = 10.0.40219.1\n"
    solution_content += f'Project("{type_guid}") = "{plugin_name}", "{plugin_name}\\{plugin_name}.csproj", "{project_guid}"\n'
    solution_content += "EndProject\n"
    if reference_stroke:
        solution_content += f'Project("{type_guid}") = "Stroke", "..\\Stroke\\Stroke\\Stroke.csproj", "{stroke_guid}"\n'
        solution_content += "EndProject\n"
    solution_content += "Global\n"
    solution_content += "\tGlobalSection(SolutionConfigurationPlatforms) = preSolution\n"
    solution_content += "\t\tDebug|Any CPU = Debug|Any CPU\n"
    solution_content += "\t\tRelease|Any CPU = Release|Any CPU\n"
    solution_content += "\tEndGlobalSection\n"
    solution_content += "\tGlobalSection(ProjectConfigurationPlatforms) = postSolution\n"
    solution_content += f"\t\t{project_guid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\n"
    solution_content += f"\t\t{project_guid}.Debug|Any CPU.Build.0 = Debug|Any CPU\n"
    solution_content += f"\t\t{project_guid}.Release|Any CPU.ActiveCfg = Release|Any CPU\n"
    solution_content += f"\t\t{project_guid}.Release|Any CPU.Build.0 = Release|Any CPU\n"
    if reference_stroke:
        solution_content += f"\t\t{stroke_guid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU\n"
        solution_content += f"\t\t{stroke_guid}.Debug|Any CPU.Build.0 = Debug|Any CPU\n"
        solution_content += f"\t\t{stroke_guid}.Release|Any CPU.ActiveCfg = Release|Any CPU\n"
        solution_content += f"\t\t{stroke_guid}.Release|Any CPU.Build.0 = Release|Any CPU\n"
    solution_content += "\tEndGlobalSection\n"
    solution_content += "\tGlobalSection(SolutionProperties) = preSolution\n"
    solution_content += "\t\tHideSolutionNode = FALSE\n"
    solution_content += "\tEndGlobalSection\n"
    solution_content += "\tGlobalSection(ExtensibilityGlobals) = postSolution\n"
    solution_content += f"\t\tSolutionGuid = {solution_guid}\n"
    solution_content += "\tEndGlobalSection\n"
    solution_content += "EndGlobal\n"
    return solution_content


def generate_csproj(plugin_name, project_guid, reference_stroke):
    stroke_guid_lowercase = "{4bf7e1c7-8683-47a3-99ff-76eceda97bb4}"
    project_content = '<?xml version="1.0" encoding="utf-8"?>\n'
    project_content += '<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">\n'
    project_content += '  <Import Project="$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props" Condition="Exists(\'$(MSBuildExtensionsPath)\\$(MSBuildToolsVersion)\\Microsoft.Common.props\')" />\n'
    project_content += "  <PropertyGroup>\n"
    project_content += "    <Configuration Condition=\" '$(Configuration)' == '' \">Debug</Configuration>\n"
    project_content += "    <Platform Condition=\" '$(Platform)' == '' \">AnyCPU</Platform>\n"
    project_content += f"    <ProjectGuid>{project_guid}</ProjectGuid>\n"
    project_content += "    <OutputType>Library</OutputType>\n"
    project_content += "    <AppDesignerFolder>Properties</AppDesignerFolder>\n"
    project_content += "    <RootNamespace>Stroke</RootNamespace>\n"
    project_content += f"    <AssemblyName>Stroke.{plugin_name}</AssemblyName>\n"
    project_content += "    <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>\n"
    project_content += "    <FileAlignment>512</FileAlignment>\n"
    project_content += "    <Deterministic>true</Deterministic>\n"
    project_content += "  </PropertyGroup>\n"
    project_content += "  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' \">\n"
    project_content += "    <DebugSymbols>true</DebugSymbols>\n"
    project_content += "    <DebugType>full</DebugType>\n"
    project_content += "    <Optimize>false</Optimize>\n"
    project_content += "    <OutputPath>bin\\Debug\\</OutputPath>\n"
    project_content += "    <DefineConstants>DEBUG;TRACE</DefineConstants>\n"
    project_content += "    <ErrorReport>prompt</ErrorReport>\n"
    project_content += "    <WarningLevel>4</WarningLevel>\n"
    project_content += "  </PropertyGroup>\n"
    project_content += "  <PropertyGroup Condition=\" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' \">\n"
    project_content += "    <DebugType>none</DebugType>\n"
    project_content += "    <Optimize>true</Optimize>\n"
    project_content += "    <OutputPath>bin\\Release\\</OutputPath>\n"
    project_content += "    <DefineConstants>TRACE</DefineConstants>\n"
    project_content += "    <ErrorReport>prompt</ErrorReport>\n"
    project_content += "    <WarningLevel>4</WarningLevel>\n"
    project_content += "  </PropertyGroup>\n"
    project_content += "  <ItemGroup>\n"
    if reference_stroke:
        project_content += f'    <ProjectReference Include="..\\..\\Stroke\\Stroke\\Stroke.csproj">\n'
        project_content += f"      <Project>{stroke_guid_lowercase}</Project>\n"
        project_content += f"      <Name>Stroke</Name>\n"
        project_content += f"    </ProjectReference>\n"
    project_content += '    <Reference Include="System" />\n'
    project_content += '    <Reference Include="System.Core" />\n'
    project_content += '    <Reference Include="System.Drawing" />\n'
    project_content += '    <Reference Include="System.Windows.Forms" />\n'
    project_content += "  </ItemGroup>\n"
    project_content += "  <ItemGroup>\n"
    project_content += f'    <Compile Include="{plugin_name}.cs" />\n'
    project_content += '    <Compile Include="Properties\\AssemblyInfo.cs" />\n'
    project_content += "  </ItemGroup>\n"
    project_content += '  <Import Project="$(MSBuildToolsPath)\\Microsoft.CSharp.targets" />\n'
    project_content += "</Project>\n"
    return project_content


def generate_assembly_info(plugin_name, company, copyright, project_guid):
    guid_lowercase = project_guid.lower()
    return f"""using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Stroke.{plugin_name}")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("{company}")]
[assembly: AssemblyProduct("Stroke.{plugin_name}")]
[assembly: AssemblyCopyright("Copyright © {copyright}")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("{guid_lowercase.strip('{}')}")]
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
"""


def generate_main_cs(plugin_name):
    return f"""using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stroke
{{
    public static class {plugin_name}
    {{
        // TODO: Implement plugin functionality.
    }}
}}
"""


def generate_readme(plugin_name, is_chinese=False):
    if is_chinese:
        return f"# Stroke.{plugin_name}\n\n## 许可证\n\n基于 MIT 许可证开源。\n"
    else:
        return f"# Stroke.{plugin_name}\n\n## License\n\nDistributed under the MIT License.\n"


def main():
    language = get_system_language()

    messages = {
        "title": {"zh": "=== Stroke 插件项目生成器 ===\n", "en": "=== Stroke Plugin Project Generator ===\n"},
        "plugin_name_prompt": {"zh": "插件名称（不含 'Stroke.' 前缀，例如 TrayIcon）：", "en": "Plugin name (without 'Stroke.' prefix, e.g. TrayIcon): "},
        "plugin_name_empty": {"zh": "插件名称不能为空。", "en": "Plugin name cannot be empty."},
        "reference_stroke_prompt": {"zh": "是否引用 Stroke 项目？(y/n)：", "en": "Reference Stroke project? (y/n): "},
        "company_prompt": {"zh": "公司名称：", "en": "Company name: "},
        "copyright_prompt": {"zh": "版权所有者（不含 'Copyright ©'，例如 Poerin）：", "en": "Copyright holder (without 'Copyright ©', e.g. Poerin): "},
        "company_copyright_required": {"zh": "公司名称和版权所有者不能为空。", "en": "Company and Copyright are required."},
        "generation_success": {"zh": "\n项目生成成功！", "en": "\nProject generated successfully!"},
        "location_prefix": {"zh": "位置：", "en": "Location: "},
        "files_created": {"zh": "已创建文件：", "en": "Files created:"},
    }

    print(messages["title"][language])

    plugin_name = input(messages["plugin_name_prompt"][language]).strip()
    if not plugin_name:
        print(messages["plugin_name_empty"][language])
        return

    reference_choice = input(messages["reference_stroke_prompt"][language]).strip().lower()
    reference_stroke = reference_choice == "y"

    company = input(messages["company_prompt"][language]).strip()
    copyright = input(messages["copyright_prompt"][language]).strip()
    if not company or not copyright:
        print(messages["company_copyright_required"][language])
        return

    project_guid = generate_guid(uppercase=True)
    solution_guid = generate_guid(uppercase=True)

    output_directory = f"Stroke.{plugin_name}"
    project_directory = os.path.join(output_directory, plugin_name)
    properties_directory = os.path.join(project_directory, "Properties")

    solution_content = generate_sln(plugin_name, project_guid, solution_guid, reference_stroke)
    project_content = generate_csproj(plugin_name, project_guid, reference_stroke)
    assembly_content = generate_assembly_info(plugin_name, company, copyright, project_guid)
    csharp_content = generate_main_cs(plugin_name)
    readme_english = generate_readme(plugin_name, is_chinese=False)
    readme_chinese = generate_readme(plugin_name, is_chinese=True)

    write_file(os.path.join(output_directory, f"{plugin_name}.sln"), solution_content, add_bom=True)
    write_file(os.path.join(project_directory, f"{plugin_name}.csproj"), project_content, add_bom=True)
    write_file(os.path.join(properties_directory, "AssemblyInfo.cs"), assembly_content, add_bom=True)
    write_file(os.path.join(project_directory, f"{plugin_name}.cs"), csharp_content, add_bom=True)
    write_file(os.path.join(output_directory, "README.md"), readme_english, add_bom=False)
    write_file(os.path.join(output_directory, "README_CN.md"), readme_chinese, add_bom=False)

    print(messages["generation_success"][language])
    print(f"{messages['location_prefix'][language]}{os.path.abspath(output_directory)}")
    print(messages["files_created"][language])
    print(f"  {output_directory}/{plugin_name}.sln")
    print(f"  {output_directory}/README.md")
    print(f"  {output_directory}/README_CN.md")
    print(f"  {output_directory}/{plugin_name}/{plugin_name}.csproj")
    print(f"  {output_directory}/{plugin_name}/{plugin_name}.cs")
    print(f"  {output_directory}/{plugin_name}/Properties/AssemblyInfo.cs")


if __name__ == "__main__":
    main()
