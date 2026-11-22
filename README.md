# Resume Maker

## Test environment and command-line workflow

The commands below are independent of Visual Studio or another IDE. Run them from the repository root.

### Tool versions

| Tool | Version |
|---|---:|
| .NET SDK | 10.0.401 |
| Target framework | net10.0 |
| Microsoft.NET.Test.Sdk | 17.12.0 |
| xUnit | 2.9.2 |
| xUnit Visual Studio runner | 2.8.2 |
| coverlet.collector | 6.0.4 |
| ReportGenerator | 5.4.3 |

The project was developed and verified on Windows. The Linux commands use the same repository and .NET SDK version; only the shell syntax differs.

## Windows (PowerShell)

### Prepare the environment

Install the .NET 10 SDK, then verify it:

```powershell
dotnet --version
dotnet --info
```

Clone and enter the repository:

```powershell
git clone https://github.com/tohaf17/Resume-project.git
Set-Location Resume-project
```

Restore dependencies:

```powershell
dotnet restore "Resume Maker.slnx"
```

### Build and run tests

```powershell
dotnet build "Resume Maker.slnx" --configuration Release --no-restore
dotnet test "Resume Maker.slnx" --configuration Release --no-build
```

Run one test independently:

```powershell
dotnet test "ResumeMaker.Tests/ResumeMaker.Tests.csproj" --filter "FullyQualifiedName~LandingServiceTests.GetAsync_ShouldReturnLandingContentWithTemplatesCta"
```

### Measure coverage

Install the pinned HTML report tool once:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.4.3
```

Collect Cobertura coverage and generate the HTML report:

```powershell
Remove-Item -Recurse -Force TestResults -ErrorAction SilentlyContinue
dotnet test "Resume Maker.slnx" --collect:"XPlat Code Coverage" --results-directory TestResults
reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:coverage-report" "-reporttypes:Html"
Start-Process "coverage-report/index.html"
```

## Linux (Bash)

### Prepare the environment

Install the .NET 10 SDK using the package manager recommended for the distribution, then verify it:

```bash
dotnet --version
dotnet --info
```

Clone and enter the repository:

```bash
git clone https://github.com/tohaf17/Resume-project.git
cd Resume-project
```

Restore dependencies:

```bash
dotnet restore "Resume Maker.slnx"
```

### Build and run tests

```bash
dotnet build "Resume Maker.slnx" --configuration Release --no-restore
dotnet test "Resume Maker.slnx" --configuration Release --no-build
```

Run one test independently:

```bash
dotnet test "ResumeMaker.Tests/ResumeMaker.Tests.csproj" --filter "FullyQualifiedName~LandingServiceTests.GetAsync_ShouldReturnLandingContentWithTemplatesCta"
```

### Measure coverage

Install the pinned HTML report tool once:

```bash
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.4.3
```

Collect Cobertura coverage and generate the HTML report:

```bash
rm -rf TestResults
dotnet test "Resume Maker.slnx" --collect:"XPlat Code Coverage" --results-directory TestResults
reportgenerator \
  "-reports:TestResults/**/coverage.cobertura.xml" \
  "-targetdir:coverage-report" \
  "-reporttypes:Html"
xdg-open coverage-report/index.html
```

## Reproducibility notes

- Run all commands from the repository root.
- Do not use absolute paths in tests or scripts.
- Use UTC for date and time assertions.
- Do not commit `bin/`, `obj/`, `TestResults/`, or `coverage-report/`.
- The test project is `ResumeMaker.Tests/ResumeMaker.Tests.csproj`.

