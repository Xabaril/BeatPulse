# Taken from psake https://github.com/psake/psake

<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

$tag = $(git tag -l --points-at HEAD)

if ( $tag -eq $null){
	$tag = "dev"
}

echo "building docker image with tag: $tag"

exec { & docker build . -f .\Dockerfile -t xabarilcoding/beatpulseui:$tag }

echo ""
echo "Created docker image beatpulse:$tag. You can execute this image using docker run"
echo ""
echo "Command Sample"
echo "docker run --name ui -p 5000:80 -e 'BeatPulse-UI:Liveness:0:Name=httpBasic' -e 'BeatPulse-UI:Liveness:0:Uri=http://www.google.es' -d beatpulseui:dev"
