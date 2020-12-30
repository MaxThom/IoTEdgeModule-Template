[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

[![Build Status](https://dev.azure.com/MaxThom/MasterThesis/_apis/build/status/ModuleTelemetry?branchName=main)](https://dev.azure.com/MaxThom/MasterThesis/_build/latest?definitionId=1&branchName=main)

# Introduction 
This is a template project in C# to build IoT applications using the Azure IotHub and Azure IotEdge framework.
It provides a startup with default console logging, configuration files, dependency injection and more.
The template also has basic unit test for every project.

The project has a temporising service not to overwhelm the Azure IotHub and maximise message size. It provides default methods for IotHub communication and connections. Random data are generated in the Broker project to mimic real-life scenarios.

The template provides Docker files for many OS and architecture:
 - linux-amd64
 - windows-amd64
 - linux-arm32
 - linux-arm64

Finally, an azure-pipeline.yaml is configured to run the unit test, push to DockerHub repository and deploy on Azure IotHub.
Every project runs with .Net 5.0

# Getting Started
Install the tools :
- https://marketplace.visualstudio.com/items?itemName=vsc-iot.vs16iotedgetools
- https://github.com/Azure/iotedgehubdev

Documentation :
- https://docs.microsoft.com/en-us/azure/iot-edge/?view=iotedge-2018-06
