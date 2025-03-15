@echo off
echo Publicando o aplicativo...
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:DebugType=None -p:DebugSymbols=false

echo Assinando o executavel...
cd "bin\Release\net9.0-windows\win-x64\publish"
"C:\Arquivos de programas (x86)\Windows Kits\bin\10.0.26100.0\x64\signtool.exe" sign /f "E:\Certificados\AkiraDev.pfx" /p "31082011" /tr http://timestamp.digicert.com /td sha256 /fd sha256 "OtimizadorDePastas.exe"

echo Processo concluido!
echo Executavel assinado esta em: bin\Release\net9.0-windows\win-x64\publish\OtimizadorDePastas.exe
pause