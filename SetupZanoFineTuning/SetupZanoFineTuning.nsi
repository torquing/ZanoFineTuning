!define PRODUCT_NAME "ZanoFineTuning"

Name "${PRODUCT_NAME}"
OutFile "SetupZanoFineTuning.exe"
Icon "Setup.ico"
BrandingText " "
SilentInstall silent

Section

  SetOutPath "$TEMP\${PRODUCT_NAME}"

  File "vc_redist.x86.exe"	; VC 2015 runtime for libZano
  File "setup.exe"		; .NET 4.5.1 for ZanoFineTuning
  File "setup.msi"		; ZanoFineTuning itself

  ExecWait "$TEMP\${PRODUCT_NAME}\vc_redist.x86.exe /install /passive /norestart"
  ExecWait "$TEMP\${PRODUCT_NAME}\setup.exe"
  ExecWait "$TEMP\${PRODUCT_NAME}\setup.msi"
  RMDir /r /REBOOTOK "$TEMP\${PRODUCT_NAME}"

SectionEnd
