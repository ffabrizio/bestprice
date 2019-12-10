$path = "C:\Data\BestPrice\"
$rulespath = $path + "contract-rules/"
$pricespath = $path + "contracted-prices/"
$apiEndpoint = "http://localhost:7071"
$ProgressPreference = "SilentlyContinue"

function PushRuleData {
 param( [PSObject]$file )
 Write-Host "Processing " ($file.Name)
 $val = $data = Get-Content -Path ($file.FullName) | ConvertFrom-Json
 
 Invoke-WebRequest -Uri ($apiEndpoint)/api/contract/update -Method POST -Body ($data | ConvertTo-Json).ToString() -ContentType "application/json" -UseBasicParsing | Out-Null
}

function PushPriceData {
 param( [PSObject]$file )
 Write-Host "Processing " ($file.Name)
 $data = Get-Content -Path ($file.FullName) | ConvertFrom-Json

 Invoke-WebRequest -Uri ($apiEndpoint)/api/customer/update -Method POST -Body ($data | ConvertTo-Json).ToString() -ContentType "application/json" -UseBasicParsing | Out-Null
}

Write-Host "Started processing rules... " (Get-Date)
Get-ChildItem -Path $newpath -Filter "*.json" | foreach { PushRuleData($_)}
Write-Host "Completed processing rules. " (Get-Date)

Write-Host "Started processing prices... " (Get-Date)
Get-ChildItem -Path $newpath -Filter "*.json" | foreach { PushPriceData($_)}
Write-Host "Completed processing prices. " (Get-Date)
