$path = "C:\Data\BestPrice\"
$rulespath = $path + "discount-rules.csv"
$contractspath = $path + "contract-types.csv"
$pricespath = $path + "contracted-prices.csv"

$rulesSource = Get-Content $rulespath | Out-String | ConvertFrom-Csv
$contractsSource = Get-Content $contractspath | Out-String | ConvertFrom-Csv
$pricesSource = Get-Content $pricespath | Out-String | ConvertFrom-Csv

$Rules = @()
$Prices = @()

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

 $props = @{
    customerNumber = $price."SBQQ__Account__r.AccountNumber"
    discountValue = [decimal] $price.SBQQ__Discount__c
    sku = $price."SBQQ__Product__r.ProductCode"
    isPercentageValue = [bool] 1
    validFrom = $validfrom 
    validTo = $validto
 }

 $newprice = New-Object PsObject -Property $props
 $newprice
}

function CreateRule {
 param( [object]$rule )

 if ($rule."Item Type") {
    $attrname = "ProductType"
    $attrvalue = $rule."Item Type"
 }
 if ($rule."Product Line") {
    $attrname = "ProductLine"
    $attrvalue = $rule."Product Line"
 }
 if ($rule."Product Group") {
    $attrname = "ProductGroup"
    $attrvalue = $rule."Product Group"
 }

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
    attributeName = $attrname
    attributeValue = $attrvalue
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
