net stop ApacsAdapterService
taskkill /f /im ApacsAdapterService.exe
cd ..\bin\x86\Debug
copy ApacsAdapter.dll C:\Windows\SysWOW64\ /y
copy WSO2.dll C:\Windows\SysWOW64\ /y
copy ApacsAdapterService.exe C:\Windows\SysWOW64\ /y
cd C:\Windows\SysWOW64\
del ApacsAdapter.cfg /F /Q
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u ApacsAdapterService.exe
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe ApacsAdapterService.exe
net start ApacsAdapterService