#!/bin/bash

dotnet restore
dotnet build
dotnet run --project PetMateAPI