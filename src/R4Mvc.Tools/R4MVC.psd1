@{
    # Script module or binary module file associated with this manifest
    ModuleToProcess = 'R4MVC.psm1'

    # Version number of this module.
    ModuleVersion = '1.0.0'

    # ID used to uniquely identify this module
    GUID = 'cbe4b5b4-0663-4e78-a43e-33d60e1df7d7'

    # Author of this module
    Author = 'R4MVC'

    # Company or vendor of this module
    CompanyName = 'R4MVC'

    # Copyright statement for this module
    Copyright = '(c) 2017 All rights reserved.'

    # Description of the functionality provided by this module
    Description = 'R4MVC Code Generator'

    # Minimum version of the Windows PowerShell engine required by this module
    PowerShellVersion = '3.0'

    # Name of the Windows PowerShell host required by this module
    PowerShellHostName = 'Package Manager Host'

    # Minimum version of the Windows PowerShell host required by this module
    PowerShellHostVersion = '1.2'

    # Minimum version of the .NET Framework required by this module
    DotNetFrameworkVersion = '4.0'

    # Minimum version of the common language runtime (CLR) required by this module
    CLRVersion = ''

    # Processor architecture (None, X86, Amd64, IA64) required by this module
    ProcessorArchitecture = ''

    # Modules that must be imported into the global environment prior to importing this module
    RequiredModules = 'NuGet'

    # Assemblies that must be loaded prior to importing this module
    RequiredAssemblies = @()

    # Script files (.ps1) that are run in the caller's environment prior to importing this module
    ScriptsToProcess = @()

    # Type files (.ps1xml) to be loaded when importing this module
    TypesToProcess = @()

    # Format files (.ps1xml) to be loaded when importing this module
    FormatsToProcess = @()

    # Modules to import as nested modules of the module specified in ModuleToProcess
    NestedModules = @()

    # Functions to export from this module
    FunctionsToExport = (
        'Generate-R4MVC',
        'Remove-R4MVC'
    )

    # Cmdlets to export from this module
    CmdletsToExport = @()

    # Variables to export from this module
    VariablesToExport = @()

    # Aliases to export from this module
    AliasesToExport = @()

    # List of all modules packaged with this module
    ModuleList = @()

    # List of all files packaged with this module
    FileList = @()

    # Private data to pass to the module specified in ModuleToProcess
    PrivateData = ''
}

# SIG # Begin signature block
# MIIYdgYJKoZIhvcNAQcCoIIYZzCCGGMCAQExCzAJBgUrDgMCGgUAMGkGCisGAQQB
# gjcCAQSgWzBZMDQGCisGAQQBgjcCAR4wJgIDAQAABBAfzDtgWUsITrck0sYpfvNR
# AgEAAgEAAgEAAgEAAgEAMCEwCQYFKw4DAhoFAAQUREZQJZkVo4hetDIr94dfOoqu
# iF6gghOTMIIGUDCCBDigAwIBAgIVAP2WxDxi4+uCQF7bIqeXC1zr5BuMMA0GCSqG
# SIb3DQEBDQUAMH8xCzAJBgNVBAYTAkdCMQ8wDQYDVQQHDAZMb25kb24xFTATBgNV
# BAoMDEZsZXhMYWJzIEx0ZDElMCMGA1UEAwwcRmxleExhYnMgTHRkIENvZGUgU2ln
# bmluZyBDQTEhMB8GCSqGSIb3DQEJARYSY2VydHNAZmxleGxhYnMub3JnMB4XDTE5
# MDUxMjE5MzkzMVoXDTI0MDUxMDE5MzkzMVowejELMAkGA1UEBhMCR0IxDzANBgNV
# BAcMBkxvbmRvbjEXMBUGA1UECgwOQXJ0aW9tIENoaWxhcnUxHTAbBgNVBAMMFEFy
# dGlvbSBDaGlsYXJ1IChPVEcpMSIwIAYJKoZIhvcNAQkBFhNhcnRpb21AZmxleGxh
# YnMub3JnMIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuDp15tQi25Xp
# fDjhOAlC8yfxiI3PseaGzqmy+MzlBV4P2ENqWbYG9VXpvV4V8EFK7fxPzFmXO7dn
# Y1VWJ0HWMW3BYE8MbwPm6ZHC1n8O5/Psy6Tf/CKGgDbgGvL+zMVq8g14ahzRvS6D
# lUB9WjNuPDl99LUzG6AtRKc00fnNPh6P9XZFzAeO5OxnpwniEplIty2v8Kr0wZYP
# EqQh0N2IaX/bTUwmlqxZ5brKuMjIthUUGg3ztxiO+7kR8JI13CaEMalG0jSuihIx
# vWHlGkS5cgIbdvJ+1xV3PXme8CUAIX5iXLr1bsDpTRTxZ/Jsn0yitlEf8P3fsNym
# qjR2y/Ny4QIDAQABo4IBxjCCAcIwCQYDVR0TBAIwADAdBgNVHQ4EFgQUN36Dakra
# eVfvin9U2YgxiO0+StYwga8GA1UdIwSBpzCBpIAUKf5vHrsOXeDk6UkHJHrTeOVu
# x/ChdqR0MHIxCzAJBgNVBAYTAkdCMQ8wDQYDVQQHDAZMb25kb24xFTATBgNVBAoM
# DEZsZXhMYWJzIEx0ZDEYMBYGA1UEAwwPRmxleExhYnMgTHRkIENBMSEwHwYJKoZI
# hvcNAQkBFhJjZXJ0c0BmbGV4bGFicy5vcmeCFD/osud09MGcg31IreV1KCwyLkIp
# MA4GA1UdDwEB/wQEAwIHgDArBgNVHSUEJDAiBggrBgEFBQcDAwYKKwYBBAGCNwIB
# FQYKKwYBBAGCNwIBFjA0BgNVHR8ELTArMCmgJ6AlhiNodHRwczovL2NhLmZsZXhs
# YWJzLm9yZy9jb2RlU2lnLmNybDBxBggrBgEFBQcBAQRlMGMwLwYIKwYBBQUHMAKG
# I2h0dHBzOi8vY2EuZmxleGxhYnMub3JnL2NvZGVTaWcuY3J0MDAGCCsGAQUFBzAB
# hiRodHRwczovL2NhLmZsZXhsYWJzLm9yZy9vY3NwL2NvZGVTaWcwDQYJKoZIhvcN
# AQENBQADggIBAIWM+fFua2o2Y0pA5Xjiepy5hmpWAleBCtWl3qHXODi/jX5pOBJk
# 8ZsJnEh3mlYnNURoo5tnqoNsZ5Z3oLjs3xpxMPDxnq/7isbAWcd3KB1ObRcyMOhV
# g1BDAzyNgYH4RMRUKqJjKFpXyhuUTibjnmHSZp7hlDQB4joXPRs7x7fIgwbHv2tf
# RlAxA1FT0IcWiKkZAGqwhCC62c+OMubnpCLnyDlKLPKwM5GBTJFZYOa/tbt+dj5m
# qt+V+6VsTOqsEf7edTQQWzTzJvV9Y3/qGuHjZNGiF7rTI7/5dlI9NVa/+5NSIjrJ
# /5dKFkDAOP6NiXf8uCFxD4etJGSKdxOH6M9uA53leE2NDCvMZjrKB7ziGPxzfjuD
# 6VE1wB/n9ZKEItxaE70HHZhYVH3qEuinlhBuU3Z01lyvGlEaIry1S8poLNKHy3QR
# kV9BsFIkH2IZJMGRTo7JSoWmbbbjVJ1PPAPjGGEb0lzxZzGAOQ/eJOUS8Enf2IhO
# aJozVrTQfSDQQlcq0GQems05yYJutZBSWY8XpwDKCDGqVGnBmDnl0s2RtEBjVZF3
# Wu+ny5TK0quz8vKZ6y9obobtMfyvRmcXpt2a5P4JMoUXLrtq8QFfLYyX6XFzPQrS
# ncwgYMqUzvXm//vXd+aDQoX0sdrmCLGr96slSDC472n4qIuQ1uIyghqvMIIGajCC
# BVKgAwIBAgIQAwGaAjr/WLFr1tXq5hfwZjANBgkqhkiG9w0BAQUFADBiMQswCQYD
# VQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3d3cuZGln
# aWNlcnQuY29tMSEwHwYDVQQDExhEaWdpQ2VydCBBc3N1cmVkIElEIENBLTEwHhcN
# MTQxMDIyMDAwMDAwWhcNMjQxMDIyMDAwMDAwWjBHMQswCQYDVQQGEwJVUzERMA8G
# A1UEChMIRGlnaUNlcnQxJTAjBgNVBAMTHERpZ2lDZXJ0IFRpbWVzdGFtcCBSZXNw
# b25kZXIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCjZF38fLPggjXg
# 4PbGKuZJdTvMbuBTqZ8fZFnmfGt/a4ydVfiS457VWmNbAklQ2YPOb2bu3cuF6V+l
# +dSHdIhEOxnJ5fWRn8YUOawk6qhLLJGJzF4o9GS2ULf1ErNzlgpno75hn67z/RJ4
# dQ6mWxT9RSOOhkRVfRiGBYxVh3lIRvfKDo2n3k5f4qi2LVkCYYhhchhoubh87ubn
# NC8xd4EwH7s2AY3vJ+P3mvBMMWSN4+v6GYeofs/sjAw2W3rBerh4x8kGLkYQyI3o
# BGDbvHN0+k7Y/qpA8bLOcEaD6dpAoVk62RUJV5lWMJPzyWHM0AjMa+xiQpGsAsDv
# pPCJEY93AgMBAAGjggM1MIIDMTAOBgNVHQ8BAf8EBAMCB4AwDAYDVR0TAQH/BAIw
# ADAWBgNVHSUBAf8EDDAKBggrBgEFBQcDCDCCAb8GA1UdIASCAbYwggGyMIIBoQYJ
# YIZIAYb9bAcBMIIBkjAoBggrBgEFBQcCARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQu
# Y29tL0NQUzCCAWQGCCsGAQUFBwICMIIBVh6CAVIAQQBuAHkAIAB1AHMAZQAgAG8A
# ZgAgAHQAaABpAHMAIABDAGUAcgB0AGkAZgBpAGMAYQB0AGUAIABjAG8AbgBzAHQA
# aQB0AHUAdABlAHMAIABhAGMAYwBlAHAAdABhAG4AYwBlACAAbwBmACAAdABoAGUA
# IABEAGkAZwBpAEMAZQByAHQAIABDAFAALwBDAFAAUwAgAGEAbgBkACAAdABoAGUA
# IABSAGUAbAB5AGkAbgBnACAAUABhAHIAdAB5ACAAQQBnAHIAZQBlAG0AZQBuAHQA
# IAB3AGgAaQBjAGgAIABsAGkAbQBpAHQAIABsAGkAYQBiAGkAbABpAHQAeQAgAGEA
# bgBkACAAYQByAGUAIABpAG4AYwBvAHIAcABvAHIAYQB0AGUAZAAgAGgAZQByAGUA
# aQBuACAAYgB5ACAAcgBlAGYAZQByAGUAbgBjAGUALjALBglghkgBhv1sAxUwHwYD
# VR0jBBgwFoAUFQASKxOYspkH7R7for5XDStnAs0wHQYDVR0OBBYEFGFaTSS2STKd
# Sip5GoNL9B6Jwcp9MH0GA1UdHwR2MHQwOKA2oDSGMmh0dHA6Ly9jcmwzLmRpZ2lj
# ZXJ0LmNvbS9EaWdpQ2VydEFzc3VyZWRJRENBLTEuY3JsMDigNqA0hjJodHRwOi8v
# Y3JsNC5kaWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURDQS0xLmNybDB3Bggr
# BgEFBQcBAQRrMGkwJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0LmNv
# bTBBBggrBgEFBQcwAoY1aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0RpZ2lD
# ZXJ0QXNzdXJlZElEQ0EtMS5jcnQwDQYJKoZIhvcNAQEFBQADggEBAJ0lfhszTbIm
# gVybhs4jIA+Ah+WI//+x1GosMe06FxlxF82pG7xaFjkAneNshORaQPveBgGMN/qb
# sZ0kfv4gpFetW7easGAm6mlXIV00Lx9xsIOUGQVrNZAQoHuXx/Y/5+IRQaa9Ytnw
# Jz04HShvOlIJ8OxwYtNiS7Dgc6aSwNOOMdgv420XEwbu5AO2FKvzj0OncZ0h3RTK
# FV2SQdr5D4HRmXQNJsQOfxu19aDxxncGKBXp2JPlVRbwuwqrHNtcSCdmyKOLChzl
# ldquxC5ZoGHd2vNtomHpigtt7BIYvfdVVEADkitrwlHCCkivsNRu4PQUCjob4489
# yq9qjXvc2EQwggbNMIIFtaADAgECAhAG/fkDlgOt6gAK6z8nu7obMA0GCSqGSIb3
# DQEBBQUAMGUxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAX
# BgNVBAsTEHd3dy5kaWdpY2VydC5jb20xJDAiBgNVBAMTG0RpZ2lDZXJ0IEFzc3Vy
# ZWQgSUQgUm9vdCBDQTAeFw0wNjExMTAwMDAwMDBaFw0yMTExMTAwMDAwMDBaMGIx
# CzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3
# dy5kaWdpY2VydC5jb20xITAfBgNVBAMTGERpZ2lDZXJ0IEFzc3VyZWQgSUQgQ0Et
# MTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAOiCLZn5ysJClaWAc0Bw
# 0p5WVFypxNJBBo/JM/xNRZFcgZ/tLJz4FlnfnrUkFcKYubR3SdyJxArar8tea+2t
# sHEx6886QAxGTZPsi3o2CAOrDDT+GEmC/sfHMUiAfB6iD5IOUMnGh+s2P9gww/+m
# 9/uizW9zI/6sVgWQ8DIhFonGcIj5BZd9o8dD3QLoOz3tsUGj7T++25VIxO4es/K8
# DCuZ0MZdEkKB4YNugnM/JksUkK5ZZgrEjb7SzgaurYRvSISbT0C58Uzyr5j79s5A
# XVz2qPEvr+yJIvJrGGWxwXOt1/HYzx4KdFxCuGh+t9V3CidWfA9ipD8yFGCV/QcE
# ogkCAwEAAaOCA3owggN2MA4GA1UdDwEB/wQEAwIBhjA7BgNVHSUENDAyBggrBgEF
# BQcDAQYIKwYBBQUHAwIGCCsGAQUFBwMDBggrBgEFBQcDBAYIKwYBBQUHAwgwggHS
# BgNVHSAEggHJMIIBxTCCAbQGCmCGSAGG/WwAAQQwggGkMDoGCCsGAQUFBwIBFi5o
# dHRwOi8vd3d3LmRpZ2ljZXJ0LmNvbS9zc2wtY3BzLXJlcG9zaXRvcnkuaHRtMIIB
# ZAYIKwYBBQUHAgIwggFWHoIBUgBBAG4AeQAgAHUAcwBlACAAbwBmACAAdABoAGkA
# cwAgAEMAZQByAHQAaQBmAGkAYwBhAHQAZQAgAGMAbwBuAHMAdABpAHQAdQB0AGUA
# cwAgAGEAYwBjAGUAcAB0AGEAbgBjAGUAIABvAGYAIAB0AGgAZQAgAEQAaQBnAGkA
# QwBlAHIAdAAgAEMAUAAvAEMAUABTACAAYQBuAGQAIAB0AGgAZQAgAFIAZQBsAHkA
# aQBuAGcAIABQAGEAcgB0AHkAIABBAGcAcgBlAGUAbQBlAG4AdAAgAHcAaABpAGMA
# aAAgAGwAaQBtAGkAdAAgAGwAaQBhAGIAaQBsAGkAdAB5ACAAYQBuAGQAIABhAHIA
# ZQAgAGkAbgBjAG8AcgBwAG8AcgBhAHQAZQBkACAAaABlAHIAZQBpAG4AIABiAHkA
# IAByAGUAZgBlAHIAZQBuAGMAZQAuMAsGCWCGSAGG/WwDFTASBgNVHRMBAf8ECDAG
# AQH/AgEAMHkGCCsGAQUFBwEBBG0wazAkBggrBgEFBQcwAYYYaHR0cDovL29jc3Au
# ZGlnaWNlcnQuY29tMEMGCCsGAQUFBzAChjdodHRwOi8vY2FjZXJ0cy5kaWdpY2Vy
# dC5jb20vRGlnaUNlcnRBc3N1cmVkSURSb290Q0EuY3J0MIGBBgNVHR8EejB4MDqg
# OKA2hjRodHRwOi8vY3JsMy5kaWdpY2VydC5jb20vRGlnaUNlcnRBc3N1cmVkSURS
# b290Q0EuY3JsMDqgOKA2hjRodHRwOi8vY3JsNC5kaWdpY2VydC5jb20vRGlnaUNl
# cnRBc3N1cmVkSURSb290Q0EuY3JsMB0GA1UdDgQWBBQVABIrE5iymQftHt+ivlcN
# K2cCzTAfBgNVHSMEGDAWgBRF66Kv9JLLgjEtUYunpyGd823IDzANBgkqhkiG9w0B
# AQUFAAOCAQEARlA+ybcoJKc4HbZbKa9Sz1LpMUerVlx71Q0LQbPv7HUfdDjyslxh
# opyVw1Dkgrkj0bo6hnKtOHisdV0XFzRyR4WUVtHruzaEd8wkpfMEGVWp5+Pnq2LN
# +4stkMLA0rWUvV5PsQXSDj0aqRRbpoYxYqioM+SbOafE9c4deHaUJXPkKqvPnHZL
# 7V/CSxbkS3BMAIke/MV5vEwSV/5f4R68Al2o/vsHOE8Nxl2RuQ9nRc3Wg+3nkg2N
# sWmMT/tZ4CMP0qquAHzunEIOz5HXJ7cW7g/DvXwKoO4sCFWFIrjrGBpN/CohrUkx
# g0eVd3HcsRtLSxwQnHcUwZ1PL1qVCCkQJjGCBE0wggRJAgEBMIGYMH8xCzAJBgNV
# BAYTAkdCMQ8wDQYDVQQHDAZMb25kb24xFTATBgNVBAoMDEZsZXhMYWJzIEx0ZDEl
# MCMGA1UEAwwcRmxleExhYnMgTHRkIENvZGUgU2lnbmluZyBDQTEhMB8GCSqGSIb3
# DQEJARYSY2VydHNAZmxleGxhYnMub3JnAhUA/ZbEPGLj64JAXtsip5cLXOvkG4ww
# CQYFKw4DAhoFAKB4MBgGCisGAQQBgjcCAQwxCjAIoAKAAKECgAAwGQYJKoZIhvcN
# AQkDMQwGCisGAQQBgjcCAQQwHAYKKwYBBAGCNwIBCzEOMAwGCisGAQQBgjcCARYw
# IwYJKoZIhvcNAQkEMRYEFKF0yo8RTtThRS/ntetM7yNhpSB/MA0GCSqGSIb3DQEB
# AQUABIIBAA3iWfA845jvDVl4ITAM1fKpF+PilFFodNOpoPRLlUDRLIy1TADL+9CN
# JGvUJMZtpPmI8+OuCL9DOXjbtICVXk2XV1MKMLDjNhIxFEW+aSf0xXkOJVeSwHo/
# u/GW2KlVCvI8RYsr1fgXfWy9gDd7HcI2hkeH19KzgxHY2QuP/QgAqNeViosUzuOk
# K/vPyZb6vdlOGU/evtKNIiCZUGMQv4tXC8cCfJVI6JaXEpxbyFf1+u+W/8FqFLPz
# nkssmNM8Ka8Fhwz8MvIQ3KzGZPNnRTRnGI7queP+ZBxIR7Ji8ViDbgmk3nsfuSRi
# 0Ltq7qAj7UApAWsvoAiWISJzdshRg0GhggIPMIICCwYJKoZIhvcNAQkGMYIB/DCC
# AfgCAQEwdjBiMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5jMRkw
# FwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMSEwHwYDVQQDExhEaWdpQ2VydCBBc3N1
# cmVkIElEIENBLTECEAMBmgI6/1ixa9bV6uYX8GYwCQYFKw4DAhoFAKBdMBgGCSqG
# SIb3DQEJAzELBgkqhkiG9w0BBwEwHAYJKoZIhvcNAQkFMQ8XDTIwMDExODIzNDQ1
# NVowIwYJKoZIhvcNAQkEMRYEFK5hXR/E7dpOOJ4agUAlvjtVhd+6MA0GCSqGSIb3
# DQEBAQUABIIBABLAz2pEW1v1lUzw56zMlFdLR04qOtwFdLkJYpNEW3LiTHk+4Fwq
# a3AB7ZYM9/kFIqtNRsc0gm8vG9J9GHFn/Drhtfk4YuKrBn9AwlgtsuNFHet8hxIo
# j1MSiWu/Bw/sGn36ok9YgH6XYQjvWIGEBhWW6esd9zGZB3T6QlqCbZ0P0ZQaGRds
# HY0AQjKjoUE25CiB6OhXwkK0P4T/UQE4kR7QynfR2SmRRKFG/vzIW8H6vw0+bCmV
# vONVfjdPaU/hnqlGSevgonFzSBmb01ls3xYr8kN9Qzc8Xc8FNU1QKi917W8HKOPj
# RmMly3AROKIJOosaDYhS6IRDeQr+BWchmT8=
# SIG # End signature block
