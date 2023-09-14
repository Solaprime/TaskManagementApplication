# Project SetUp
Clone the Code base from Repo
Create a Sql Seveer Db called "TaskManagementHangfireDb"  `HangFire` will write to these Db
Restore the NuggetPackage


# Design Decisons
These project was  built with scalability and loose coupling taken into considertion, the project is in three layers, the [Api Layer]
the [core layer] and  [Api.Test layer] . [Smtp]was used as a Mailing service, [HangFire] 
and [Asp.net BackgroundService] was the choice for background service,   [Sql Server] was the Db Use 



## Api.Test

The Unit test project was built with `XUnit version 2.4.1`

## Api

The Api project was built with `ASP.net Core 3.1` 
 `Swagger` was used for the API documentation,  `Serilog` was used for logging to File
 
## Core 

the Core project is in Five layers, the Application Layer,
the Domain layer and Infrastruture layer, Persitent Layer,
and Shared Layer

## Shared Layer
Shared layer is a class library that contains ReUSable Componet throughout the Application

## Persitent Layer
Persitent layer is a class library that contains Code that interact with the Db. `Sql Server` 
 was the Database Use `Ef Core` was the  `ORM`
 
 ## Infrastruture Layer 
Infrastruture layer is a class library that contains core application Infrastruture and Services like `Smtp`  
which was the mail Service use 

 ## Domain Layer 
Domain layer is a class library that contains the application Entities
which was the mail Service use 

 ## Application Layer 
Application layer is a class library that contains the application Contracts and Dependencies



