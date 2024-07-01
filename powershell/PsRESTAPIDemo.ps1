$config = Get-Content .\config.json -Raw | ConvertFrom-Json
$scopeConfig = Get-Content .\scope.json -Raw | ConvertFrom-Json
$uriConfig = Get-Content .\uri.json -Raw | ConvertFrom-Json
$tenant = $config.tenant
$client_id = $config.client_id
$scope = $scopeConfig.arm
$client_secret = $config.client_secret
$grant_type = $config.grant_type
# get uri from token files
$uri = $uriConfig.azure_ad_get_access_token_oauth2.Replace('$tenant',$tenant) #"https://login.microsoftonline.com/$tenant/oauth2/v2.0/token"
$method = "POST"

$body = @{
    client_id = $client_id
    scope = $scope
    client_secret = $client_secret
    grant_type = $grant_type
}
try
{
    Write-Output "Attempt to acquire token from Tenant - $tenant +  for client ID: $client_id"

    $result = Invoke-WebRequest -URI $uri -Body $body -Method $method
    Write-Output "File is created in same folder with name -> access_token.json which contains access token."
    $resultContent = ConvertFrom-Json $result.Content
    Write-Output "Access Token:" $resultContent.access_token 
    $result.Content | Out-File .\access_token.json #| ConvertTo-Json 

}
catch
{
    Write-Error "Exception Message " $PSItem.Exception.Message
    Write-Error "Stack trace: " $PSItem.Exception.StackTrace
}

