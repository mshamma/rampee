# Rampee
A Windows service for managing JMS consumers  

# Introduction

### Rampee bridges the gap between JMS and .Net.  

It is a Windows Service that manages JMS “consumers” (a.k.a. the client) of the JMS producer (a.k.a. the server).  More information on Java Message Service can be found here (http://www.oracle.com/technetwork/java/jms/index.html). 

It has been tested successfully with ApacheMQ (http://activemq.apache.org/) and uses the Apache NMS API (http://activemq.apache.org/nms/apachenms.html)

# Requirements

## Software 

.Net Framework version 4.6 (or higher)

## Network

TCP Port 61616: JMS is a TCP based service whose default port is 61616.   
TCP Port 1433: The Rampee database uses a Microsoft SQL server for storing messages and logging events.

# Operations

The service will need to run in the Windows Service Control Manager as follows.  
Full permissions granted to NT AUTHORITY\NETWORK SERVICE for C:\Path\to\install\folder.
Startup type set to “Automatic”
Log On set to “NT AUTHORITY\NETWORK SERVICE”

# Installation Procedure

1. Open an administrative command-line window
2. Execute "Rampee_Service.exe --install"
3. The service will prompt you for user credentials so that the install can complete. Please enter credentials with administrator access so that the install will complete.
4. Verify that the service "Rampee Service" is listed in the Service Control Manager MMC
5. Open the properties for the "Rampee Service"
6. Open the "Log on As" tab and remove your credentials from the "Logon As" property of the service.
7. Add "NT AUTHORITY\NETWORK Service" in the "Log on As" property of the service.
8. Start the service.

# Monitoring the Service

The Windows Service Control Manager should be sufficient for monitoring the Rampee service.  Additionally, log messages from the service will be written to two locations: 

1) a log file stored in the “logs” sub-folder of the installation file path 
and 
2) a table in the Microsoft SQL database.
