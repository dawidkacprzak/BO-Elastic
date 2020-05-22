[1mdiff --git a/BO.Elastic/BO.Elastic.sln b/BO.Elastic/BO.Elastic.sln[m
[1mindex bb2ccab..e71c4e3 100644[m
[1m--- a/BO.Elastic/BO.Elastic.sln[m
[1m+++ b/BO.Elastic/BO.Elastic.sln[m
[36m@@ -5,9 +5,9 @@[m [mVisualStudioVersion = 16.0.30011.22[m
 MinimumVisualStudioVersion = 10.0.40219.1[m
 Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BO.Elastic.Panel", "BO.Elastic.Panel\BO.Elastic.Panel.csproj", "{C6EB1A7E-A5E3-48BC-B25F-1AFC2DF39F71}"[m
 EndProject[m
[31m-Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BO.Elastic.ClusterWrapper", "BO.Elastic.ClusterWrapper\BO.Elastic.ClusterWrapper.csproj", "{BE025338-BCF2-4A43-A922-10A492365EC1}"[m
[32m+[m[32mProject("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "BO.Elastic.BLL", "BO.Elastic.BLL\BO.Elastic.BLL.csproj", "{3A9E7344-C9E1-4605-AB2B-48A6BA8F33BA}"[m
 EndProject[m
[31m-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "BO.Elastic.BLL", "BO.Elastic.BLL\BO.Elastic.BLL.csproj", "{3A9E7344-C9E1-4605-AB2B-48A6BA8F33BA}"[m
[32m+[m[32mProject("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "BO.Elastic.DAL", "BO.Elastic.DAL\BO.Elastic.DAL.csproj", "{FCB8C0CE-A57D-4539-9512-C5BC0258629E}"[m
 EndProject[m
 Global[m
 	GlobalSection(SolutionConfigurationPlatforms) = preSolution[m
[36m@@ -19,14 +19,14 @@[m [mGlobal[m
 		{C6EB1A7E-A5E3-48BC-B25F-1AFC2DF39F71}.Debug|Any CPU.Build.0 = Debug|Any CPU[m
 		{C6EB1A7E-A5E3-48BC-B25F-1AFC2DF39F71}.Release|Any CPU.ActiveCfg = Release|Any CPU[m
 		{C6EB1A7E-A5E3-48BC-B25F-1AFC2DF39F71}.Release|Any CPU.Build.0 = Release|Any CPU[m
[31m-		{BE025338-BCF2-4A43-A922-10A492365EC1}.Debug|Any CPU.ActiveCfg = Debug|Any CPU[m
[31m-		{BE025338-BCF2-4A43-A922-10A492365EC1}.Debug|Any CPU.Build.0 = Debug|Any CPU[m
[31m-		{BE025338-BCF2-4A43-A922-10A492365EC1}.Release|Any CPU.ActiveCfg = Release|Any CPU[m
[31m-		{BE025338-BCF2-4A43-A922-10A492365EC1}.Release|Any CPU.Build.0 = Release|Any CPU[m
 		{3A9E7344-C9E1-4605-AB2B-48A6BA8F33BA}.Debug|Any CPU.ActiveCfg = Debug|Any CPU[m
 		{3A9E7344-C9E1-4605-AB2B-48A6BA8F33BA}.Debug|Any CPU.Build.0 = Debug|Any CPU[m
 		{3A9E7344-C9E1-4605-AB2B-48A6BA8F33BA}.Release|Any CPU.ActiveCfg = Release|Any CPU[m
 		{3A9E7344-C9E1-4605-AB2B-48A6BA8F33BA}.Release|Any CPU.Build.0 = Release|Any CPU[m
[32m+[m		[32m{FCB8C0CE-A57D-4539-9512-C5BC0258629E}.Debug|Any CPU.ActiveCfg = Debug|Any CPU[m
[32m+[m		[32m{FCB8C0CE-A57D-4539-9512-C5BC0258629E}.Debug|Any CPU.Build.0 = Debug|Any CPU[m
[32m+[m		[32m{FCB8C0CE-A57D-4539-9512-C5BC0258629E}.Release|Any CPU.ActiveCfg = Release|Any CPU[m
[32m+[m		[32m{FCB8C0CE-A57D-4539-9512-C5BC0258629E}.Release|Any CPU.Build.0 = Release|Any CPU[m
 	EndGlobalSection[m
 	GlobalSection(SolutionProperties) = preSolution[m
 		HideSolutionNode = FALSE[m
