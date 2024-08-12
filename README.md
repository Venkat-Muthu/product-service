# Product-Service
## Things to know before cloning, building, running the project<br>
<br>

### Repo Url:<br>
git clone git@github.com:Venkat-Muthu/product-service.git<br>
<br>

### Build:<br>
- Open the solution in Visual Studio, build <br>
<br>

### browse APIs<br>
- VS -> Ctrl + F5 to Run the application<br>
- You will see a web page with Health check, Product APIs<br>
- You will notice Authorize button in the page, but auto-auth/single-sign on is not implemented.<br>
<br>

### In-Memory mongodb:<br>
- This project uses mongo2go in-memory mongodb database<br>
- Config change should allow to use MongoDb database Server<br>
- On startup, a json collection with 10 products are seeded to the in-memory database<br>

### Data
- Two products has Red Color (To get product by colour)<br>

### Authorisation
- Azure App Registration Secret is not set due to plain text in repo, so can't use roles to Authorize<br>
- Authorize and Roles are commented in the ProductsController class for demo<br>
	[Authorize], [Authorize(Roles = "User")] and [Authorize(Roles = "PowerUser")] should be uncommented<br>
<br>
 
### Unit and integration Tests:<br>
- AutoFixture, NSubstitute, xUnit are used. Custom Mapper is used to map domain with DTOs and Documents<br>
- Integration tests involved Domain, API and Application layers only, but can be extendable to Repository Layers also.<br>
- Tests are not reviewed due to time constraint.<br>

### Branching:<br>
- 6 feature branches PRs created and merged into main<br>
<br>

### Architecture diagram:<br>
- Please take a look at DistributedMicroservicesArchitectureDiagram.txt or 
	DistributedMicroservicesArchitectureDiagram.jpg file in the root folder<br>
