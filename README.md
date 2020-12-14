# The RIPper

## Description.
The RIPper is a set of web-based tools designed for the analyses of Repeat-Induced Point (RIP) mutations in genome sequences of Ascomycota.

## Environment.

The RIPper was built using Asp.Net Core with an AngularJs based front-end and uses session storage to keep a users files for a defined amount of time.
The RIPper has no account or database requirements.

## Building The RIPper.

1. Clone the repository.
2. Restore all required packages.
3. Build or publish the "TheRIPper.UI.NoDatabase" project.


## Running a built version of the RIPper

There are a few pre-built distributions of The RIPper available, download the zip file that correspondes to your OS and run the TheRIPPer executable.

[Windows 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/Latest-Windows-NoDb/Windows.zip)

[Linux 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/Latest-Linux-NoDb/Linux.zip)

[OSX 64bit](https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/Latest-OSX-NoDb/OSX.zip)

### Instructions for Debian based linux users

1.  apt update && apt install wget unzip -y
2.  wget https://github.com/TheRIPper-Fungi/TheRIPper/releases/download/Latest-Linux-NoDb/Linux.zip
3.  unzip Linux.zip
4.  cd Linux/
5.  chmod +x TheRIPper.UI.NoDatabase
6.  ./TheRIPper.UI.NoDatabase


## Licence

Full licence available [here](https://github.com/TheRIPper-Fungi/TheRIPPer/blob/master/LICENSE)

