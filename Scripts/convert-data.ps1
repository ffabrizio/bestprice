$path = "C:\Data\BestPrice\"
$rulespath = $path + "discount-rules.csv"
$contractspath = $path + "contract-types.csv"
$pricespath = $path + "contracted-prices.csv"
$mappedrulespath = $path + "contract-rules/"
$mappedpricespath = $path + "contracted-prices/"

$rulesSource = Get-Content $rulespath | Out-String | ConvertFrom-Csv
$contractsSource = Get-Content $contractspath | Out-String | ConvertFrom-Csv
$pricesSource = Get-Content $pricespath | Out-String | ConvertFrom-Csv

$Rules = @()
$Prices = @()

New-Item -ItemType Directory -Force -Path $mappedrulespath | Out-Null
New-Item -ItemType Directory -Force -Path $mappedpricespath | Out-Null

function CreatePrice {
 param( [object]$price )

 $validfrom = (Get-Date)
 $validto = (Get-Date).AddYears(1)

 if ($price.SBQQ__EffectiveDate__c -ne $null -and $price.SBQQ__EffectiveDate__c -ne "")
 {
    $validfrom = [datetime] $price.SBQQ__EffectiveDate__c
 }
 if ($price.SBQQ__ExpirationDate__c -ne $null -and $price.SBQQ__ExpirationDate__c -ne "")
 {
    $validto = [datetime] $price.SBQQ__ExpirationDate__c
 }

 $percentage = 0
 $discount = $price.SBQQ__Price__c
 if ($price.SBQQ__Discount__c-ne $null -and $price.SBQQ__Discount__c -ne "")
 {
  $percentage = 1
  $discount = $price.SBQQ__Discount__c
 }

 $props = @{
    customerNumber = $price."SBQQ__Account__r.AccountNumber"
    discountValue = [decimal] $discount
    sku = $price."SBQQ__Product__r.ProductCode"
    isPercentageValue = [bool] $percentage
    validFrom = $validfrom 
    validTo = $validto
 }

 $newprice = New-Object PsObject -Property $props
 $newprice
}

function CreateRule {
 param( [object]$rule )

 $validfrom = (Get-Date)
 $validto = (Get-Date).AddYears(1)
 $contract = $contractsSource | Where { $rule."Contract Type" -eq $_."Contract Type" } | Select -First 1
 if ($contract -ne $null -and $contract."Effective Date" -ne "")
 {
    $validfrom = [datetime] ($contract."Effective Date")
 }
 if ($contract -ne $null -and $contract."Expiration Date" -ne "")
 {
    $validto = [datetime] ($contract."Expiration Date")
 }

 $props = @{
    contractTypeId = $rule."Contract Type"
    discountValue = [decimal] $rule."Discount (%)"
    productGroup = $rule."Product Group"
    productLine = $rule."Product Line"
    productType = $rule."Item Type"
    validFrom = $validfrom
    validTo = $validto
 }
 
 $newrule = New-Object PsObject -Property $props
 $newrule
}

function SaveRuleFile {
 param( $group )
 $props = @{
    contractTypeId = $group.Name
    contractRules = $group.Group
 }

 $data = New-Object PsObject -Property $props
 $data | ConvertTo-Json | Set-Content ($path + "contract-rules/" + $group.Name + ".json")
}

function SavePricesFile {
 param( $group )

 $props = @{
    customerNumber = $group.Name
    contractedPrices = $group.Group
 }

 $data = New-Object PsObject -Property $props
 $data | ConvertTo-Json | Set-Content ($path + "contracted-prices/" + $group.Name + ".json")
}

Write-Host "Started processing rules... " (Get-Date)
$rulesSource | foreach { $Rules += CreateRule($_) }
Write-Host "Completed processing rules. " (Get-Date)

Write-Host "Started saving rule files... " (Get-Date)
$Rules | Group-Object -Property contractTypeId | foreach { SaveRuleFile($_) }
Write-Host "Completed saving rule files. " (Get-Date)

Write-Host "Started processing prices... " (Get-Date)
$pricesSource | foreach { $Prices += CreatePrice($_) }
Write-Host "Completed processing prices. " (Get-Date)

Write-Host "Started saving price files... " (Get-Date)
$Prices | Group-Object -Property customerNumber | foreach { SavePricesFile($_) }
Write-Host "Completed saving price files. " (Get-Date)
