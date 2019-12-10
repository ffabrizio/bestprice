$path = "C:\Data\BestPrice\"
$rulespath = $path + "contract-rules/"
$pricespath = $path + "contracted-prices/"
$rulesApiEndpoint = "http://localhost:7071/api/contract/update"
$pricesApiEndpoint = "http://localhost:7071/api/customer/update"
$ProgressPreference = "SilentlyContinue"

function PushRuleData {
 param( [PSObject]$file )
 Write-Host "Processing " ($file.Name)
 $data = Get-Content -Path ($file.FullName) | ConvertFrom-Json
 
 Invoke-WebRequest -Uri $rulesApiEndpoint -Method POST -Body ($data | ConvertTo-Json).ToString() -ContentType "application/json" -UseBasicParsing | Out-Null
}

function PushPriceData {
 param( [PSObject]$file )
 Write-Host "Processing " ($file.Name)
 $data = Get-Content -Path ($file.FullName) | ConvertFrom-Json

 Invoke-WebRequest -Uri $pricesApiEndpoint -Method POST -Body ($data | ConvertTo-Json).ToString() -ContentType "application/json" -UseBasicParsing | Out-Null
}

Write-Host "Started pushing rules... " (Get-Date)
Get-ChildItem -Path $rulespath -Filter "*.json" | foreach { PushRuleData($_)}
Write-Host "Completed pushing rules. " (Get-Date)

Write-Host "Started pushing prices... " (Get-Date)
Get-ChildItem -Path $pricespath -Filter "*.json" | foreach { PushPriceData($_)}
Write-Host "Completed pushing prices. " (Get-Date)
