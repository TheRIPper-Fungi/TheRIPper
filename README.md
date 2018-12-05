# The RIPPer

## Description.
The RIPper is a set of web-based tools designed for analyses of Repeat-Induced Point (RIP) mutations in genome sequences of Ascomycota.

## Environment.

The RIPPer was built using Asp.Net Core with an AngularJs based front-end and Postgres running as the database, as Entity Framework was used for all database mapping, the backing database provider can be changed

## Building The RIPPer from source.

1. Clone the repository.
2. Restore all required packages.
3. Update database connection strings found in `TheRIPPer.Db\ApplicationDbContext.cs` and `TheRIPPer.AngularJs\appsettings.json` to match that of your database, as well as a unique JWT Auth key
4. Run the update-database command to build the initial database.

## Running a built version of the RIPPer

There are a few pre-built distributions of The RIPPer available, download the zip file that correspondes to your OS and run the TheRIPPer executable, this stand-alone version uses SQLLite and does not require Postgres on the machine.

[Windows 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/latest/The.RIPper.-.Windows.-.64-Bit.zip)

[Linux 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/latest-linux/The.RIPper.-.Linux.-.64-Bit.zip)

[OSX (Coming Soon)]()

## Licence

Full licence available here
https://github.com/TheRIPper-Fungi/TheRIPPer/blob/master/LICENSE
