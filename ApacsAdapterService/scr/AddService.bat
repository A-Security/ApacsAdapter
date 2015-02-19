net stop ApacsAdapterService
cd ..\bin\x86\Release\
copy ApacsAdapter.dll C:\Windows\SysWOW64\ /y
copy ApacsAdapterService.exe C:\Windows\SysWOW64\ /y
cd C:\Windows\SysWOW64\
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u ApacsAdapterService.exe
C:\Windows\Microsoft.NET\Framework\v4.0.30319\installutil.exe ApacsAdapterService.exe
net start ApacsAdapterService