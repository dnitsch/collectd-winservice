# collectd-winservice
Configurable windows service based on the collectd module for linux. Aimed to be used with collectd's write_http plugin alongside your unix based infrastructure and to create uniformity across platforms.

## prerequisites
This version of the service is designed to be used with an http interface that accepts the standard collectd format
```JSON
[ {    
    "values": 10020,
    "dstypes": [
      "value"
    ],
    "dsnames": [
      "gauge"
    ],
    "time": "2016-01-01T15:12:11.00012Z",
    "interval": 10,
    "host": "LAPTOP-ME-CODE",
    "plugin": "iis",
    "plugin_instance": "demo",
    "type": "gauge",
    "type_instance": "Bytes Sent/sec"
  }, 
  ...
]
```

## Usage 
The counters can be defined in the Resource properties file for now, you can add remove these values based on the Perf Monitor counters available to you.
Default behaviour loops through all the instances of the counter i.e. in Web Service (iis) all websites will be looped through based, similarly all CPUs for Processor counter
Properties.Resources.ExternalConstants

```JSON
{
  "counters": [
    {
      "name": "Web Service",
      "counterName": [
        "Bytes Received/sec",
        "Bytes Sent/sec",
        "Connection Attempts/sec",
        "Current Connections",
        "Current Blocked Async I/O Requests",
        "Current Connections",
        "Total Options Requests",
        "Total Put Requests",
        "Total Post Requests",
        "Total Get Requests",
        "Total Delete Requests"
      ]
    },
    {
      "name": "Processor",
      "counterName": [
        "% Processor Time",
        "% User Time",
        "% Privileged Time",
        "Interrupts/sec",
        "% DPC Time",
        "% Interrupt Time",
        "% Idle Time",
        "% C1 Time"
      ]
    },
    {
      "name": "PhysicalDisk",
      "counterName": [
        "Current Disk Queue Length",
        "% Disk Time",
        "Avg. Disk Queue Length",
        "% Disk Read Time",
        "Avg. Disk Read Queue",
        "% Disk Write Time",
        "Avg. Disk Write Queue"
      ]
    },
    {
      "name": "Memory",
      "counterName": [
        ""
      ]
    }
  ]
}
```

This has been designed and tested to work and with Elasticsearch (5.x) type TSDB, but can be used with any TSDB that accepts HTTP POST as inserts. 

## Installation
 - Identify which Counters you want to collect and adjust the resources.ExternalConstants
 - restore RestSharp pacakge from nuget
 - Adjust Model.Response.ResponseBody class as per your expected output of your http service
 - build project (or use latest inside bin folder)
 - recommended service installer is [NSSM]("http://nssm.cc") 

## Launch


```powershell
$nssmLocation = $args[0] ## "C:/$nssm-exe-folder-location$/"
$serviceLocation = $args[1] ## "C:/$collectdSrv-lcation$/collectdSrv.exe"
$argsLocal = @($args[2]) ## interval at which to poll 
$serviceName = $args[3]  ## e.g. "collectd-win"

Set-Location $nssmLocation 
.\nssm.exe install $serviceName $serviceLocation $argsLocal
.\nssm.exe start $serviceName
```

### next updates
 - working on exposing the counters and instance into the app.config for easier management