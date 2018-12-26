# The RIPPer

## Description.
The RIPper is a set of web-based tools designed for analyses of Repeat-Induced Point (RIP) mutations in genome sequences of Ascomycota.

## Environment.

The RIPPer was built using Asp.Net Core with an AngularJs based front-end and Postgres running as the database, as Entity Framework was used for all database mapping, the backing database provider can be changed.

A version of The RIPper that has no database requirements has also been built, it uses session storage to keep users files

## Building The RIPPer with Database from source.

1. Clone the repository.
2. Restore all required packages.
3. Update database connection strings found in `TheRIPPer.Db\ApplicationDbContext.cs` and `TheRIPPer.AngularJs\appsettings.json` to match that of your database, as well as a unique JWT Auth key
4. Run the update-database command to build the initial database.


## Building The RIPPer with NO Database from source.
1. Clone the repository.
2. Restore all required packages.
3. Build/ or Publish the "TheRIPper.UI.NoDatabase" project

## Running a built version of the RIPPer

There are a few pre-built distributions of The RIPPer available, download the zip file that correspondes to your OS and run the TheRIPPer executable, this stand-alone version uses SQLLite and does not require Postgres on the machine.

There are also versions for each OS that require no database and use session storage, closing the application will result in the session being terminated and all files will be abandoned 

[Windows 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/latest/The.RIPper.-.Windows.-.64-Bit.zip)

[Linux 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/latest-linux/The.RIPper.-.Linux.-.64-Bit.zip)

[OSX 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/latest-osx/OSX.64bit.zip)

## Licence

Full licence available [here](https://github.com/TheRIPper-Fungi/TheRIPPer/blob/master/LICENSE)

