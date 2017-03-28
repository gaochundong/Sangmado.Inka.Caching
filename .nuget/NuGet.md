Commands
------------
nuget setApiKey xxx-xxx-xxxx-xxxx

nuget pack ..\Sangmado.Inka.Caching\Sangmado.Inka.Caching.csproj -IncludeReferencedProjects -Symbols -Build -Prop Configuration=Release -OutputDirectory ".\packages"

nuget push .\packages\Sangmado.Inka.Caching.1.0.0.0.nupkg

