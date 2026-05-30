# Contributing to EP.Tamin.NET

Thank you for your interest in contributing! Please follow these guidelines.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Any IDE that supports C# (Visual Studio, Rider, VS Code + C# Dev Kit)

## Build & Test

```bash
dotnet test EP.Tamin.NET.slnx
```

## Code Style

- The project uses `.editorconfig` for consistent formatting.
- All public APIs must have XML documentation (`/// <summary>`).
- Nullable reference types are enabled; avoid `!` suppression unless necessary.

## Pull Requests

1. Fork the repository and create a branch from `main`.
2. Add or update tests for any new or changed behaviour.
3. Ensure `dotnet test EP.Tamin.NET.slnx` passes with no failures.
4. Open a pull request with a clear description of the change.

## Reporting Issues

Please open a GitHub issue with a minimal reproduction and the full exception message.

## License

By contributing you agree that your contributions will be licensed under the same [MIT](LICENSE) license as the project.
