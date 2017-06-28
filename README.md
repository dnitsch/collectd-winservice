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
The counters can be defined in the App.Config file, see below. You can add remove these values based on the Perf Monitor counters available to you.
Default behaviour loops through all the instances of a counter i.e. in Web Service (iis) all websites will be looped through based, similarly all CPUs for Processor counter, 
you can limit the instances of the performance object by setting a specific value in the instanceName property of the Counters Element in the app.config.

```CONFIG
  <performanceCounters>
    <Counters>
      <add name="Web Service"
           counterName="Bytes Received/sec,Bytes Sent/sec,Connection Attempts/sec,Current Connections,Current Blocked Async I/O Requests,Current Connections,Total Options Requests,Total Put Requests,Total Post Requests,Total Get Requests,Total Delete Requests"/>
      <add name="Processor"
          counterName="% Processor Time,% User Time,% Privileged Time,Interrupts/sec,% DPC Time,% Interrupt Time,% Idle Time,% C1 Time"/>
      <add name="Memory"
          counterName="Page Faults/sec,Available Bytes,Committed Bytes"/>
      <add name="PhysicalDisk"
           counterName="Current Disk Queue Length,% Disk Time,Avg. Disk Queue Length,% Disk Read Time,Avg. Disk Read Queue,% Disk Write Time,Avg. Disk Write Queue"/>
      <add name="Process"
           counterName="Private Bytes,Virtual Bytes,Thread Count,% Processor Time,IO Write Operations/sec,IO Write Bytes/sec"
           instanceName="ServiceX,ServiceY,ServiceZ"/>
    </Counters>
  </performanceCounters>
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
 - working on exposing the counters and instance into the app.config for easier management [Done]
 - enabled debug mode in interactive mode, it can be tested at first by simply running the exe
    - most common problems will include missing counters on the target machine
