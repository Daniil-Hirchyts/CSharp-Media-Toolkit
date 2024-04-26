# Historical application and dependencies

`MediaToolsApp` retained the earlier .NET Framework 4.8 WPF interface, including its media tabs, MVVM examples and original project configuration.

The main .NET 6 application was kept in `src/MediaToolsApplication`; this historical project was excluded from the root solution's build group.

The original NuGet package files were preserved in `packages`, including their native libraries and package notices. The legacy project retained its original framework target and dependency declarations.
