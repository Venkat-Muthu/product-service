# Product-Service
Things to know before cloning, building, running the project 

To clone:
git@github.com:Venkat-Muthu/product-service.git

To Build:
*. Open the solution in Visual Studio, build 

To browse APIs
*. VS -> Ctrl + F5 to Run the application
*. You will see a web page with Health check, Product APIs
*. You will notice Authorize button in the page, but auto-auth/single-sign on is not implemented.

In-Memory mongodb:
*. This project uses mongo2go in-memory mongodb database
*. Config change should allow to use MongoDb database Server
*. On startup, a json collection with 10 products are seeded to the in-memory database
*. Two products has Red Color (To get product by colour)
*. Azure App Registration Secret is not set due to plain text in repo, so can't use roles to Authorize
*. Authorize and Roles are commented in the ProductsController class for demo
	[Authorize], [Authorize(Roles = "User")] and [Authorize(Roles = "PowerUser")] should be uncommented

Unit and integration Tests:
*. AutoFixture, NSubstitute, xUnit are used. Custom Mapper is used to map domain with DTOs and Documents
*. Integration tests involved Domain, API and Application layers only, but can be extendable to Repository Layers also.
*. Tests are not reviewed due to time constraint.

Repo:
6 feature branches PRs created and merged into main

Architecture diagram:
*. Please take a look at DistributedMicroservicesArchitectureDiagram.txt or 
	DistributedMicroservicesArchitectureDiagram.jpg file in the root folder
