Quick recap:
------------

The .NET API:
- was experimenting with .NET Core 3
- it is a showstopper for OData (so filtering is done manually)
- also used EF Core - where there is still a limitation in grouping expression, so the main select got more complex and not as efficient as it could be

The Angular app:
- had to use angular material library (and not KENDO UI - due licensing)
- the main layout contains some "extra" samples



Some tech. decisions:
---------------------

1) we will use ASP.NET Core Identity (and not auth0)

	reason: all actions, including authentication should be provided by our API
	extras: a sample is provided with auh0 (both postman and .NET api secured via auth0)

	Some thoughts on authentication
	-------------------------------

		It's usual to have some kind of 3rd party authentication / authorization managing app, like Auth0, Okta or Identity Server.
			Those support a lot of extra features, which is always worth to consider
			On the other side:
			- some data is then stored outside, our app still have to manage some link between their userID and internal user ID (sync some data on change)
			- or our app can use the management API to list user and change user's data
			
		The MealAuth0Api is an auth0 security implementation on the .NET side, with extra rules to get the roles into the access-token.
		It's just a showcase, would need CORS for real usage.

	Authorization
	-------------

		Basics (with roles) can be done in any system. For fine grained, i'd probably go into a custom one, where the easiest (DB) structure (role in role and CTE) should do the heavy work.
		Actually the system would use the concept of roles, but with an additional flags each role could have a type, rules could be defined as well, so actually via a simple table we could have user groups, roles (which would group permissions) etc.
		IMO such an approach is simple and flexible enough (i'd prefer over complicated solutions with dozens of tables and extra work on set-ups)...
		
		According sales rep. Okta should have such a nesting (but haven't had time to check it out yet).



3) the ANGULAR application

	for versioning (and version checks) i'd now use "semantic-release post" and make sure that version from packages.json get's copied into the version.json file
	...



Quick ref.
----------

install-package Microsoft.Extensions.Identity.Core
install-package Microsoft.Extensions.Identity.Stores
	
install-package Microsoft.EntityFrameworkCore.Tools
install-package Microsoft.EntityFrameworkCore.Design
install-package  Microsoft.EntityFrameworkCore.SqlServer

Scaffold-DbContext -Provider Microsoft.EntityFrameworkCore.SqlServer -Connection "Data Source=.\sqlexpress;Initial Catalog=Texas911;Integrated Security=True" -o Model

EntityFrameworkCore\Add-Migration v0.0.1
EntityFrameworkCore\Update-Database



Req. functions:
---------------

User:
- create account
- login
- manage user settings
- see list of meals
--- filtering date-time from to)
--- daily display (group) - day is green if sum of calories aren't above settings
- add/edit/delete meals

Manager:
- CRUD users (change user setting, ev. block user)

Admin:
- CRUD users
- CRUD records

