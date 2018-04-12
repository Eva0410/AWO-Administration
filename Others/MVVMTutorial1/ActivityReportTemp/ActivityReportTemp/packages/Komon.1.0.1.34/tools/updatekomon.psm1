
function Update-Komon() {
    
	update-package Komon.PlatformService.NET40
    update-package Komon.PlatformService.NET45
    update-package Komon
    update-package Komon.DataAccess
    update-package Komon.Logic
    update-package Komon.Web
    update-package Komon.Core
    update-package Komon.DataAccess.Core
    update-package Komon.Logic.Core
    update-package Komon.Web.Core
    update-package Komon.Client
    update-package Komon.Presentation
    update-package Komon.Client.Core
    update-package Komon.Presentation.Core

}

# http://haacked.com/archive/2011/04/19/writing-a-nuget-package-that-adds-a-command-to-the.aspx/
# Import-Module "D:\src\Codigo\Komon.Framework\trunk\Komon.Framework-CodeFirst\Build\updatekomon.psm1"
Export-ModuleMember Update-Komon