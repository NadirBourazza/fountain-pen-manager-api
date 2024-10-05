# Project name

This is a collection of three different API's:

1. User Manager - This enables the creation, updating, deletion and retrieval of user information

2. Catalogue Manager - This enables managment of the fountain pen catalogue, which th following API relies on.

3. Pen Collection Manager - This enables the managment of a users fountain pen collection. Enabling Creation, Deletion, Updating and Searching.

# Running notes

These API's are a collection of Azure Functions, which each run on dotnet 8.0 and utilise Entity Framework to interface with an SQLite database.

Please ensure in the root directory you have an empty "Database" directory, where the DB will be created. Additionally please ensure that each API starts up on non-colliding ports.

The azure-function-core-tools@4 may be required if you are having trouble with dotnet 8.0 or running Azure Functions.
