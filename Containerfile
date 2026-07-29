# ---------- Stage 1: build & publish ----------
# The SDK image has the full .NET toolchain (compiler, restore, publish).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire solution into the build stage.
COPY . .

# Publish the API in Release mode into /app/publish. "publish" restores
# NuGet packages, compiles the API + all its referenced projects, and
# writes the final DLLs out — all in one command.
RUN dotnet publish MeetingRoomBooking.API/MeetingRoomBooking.API.csproj -c Release -o /app/publish

# ---------- Stage 2: runtime ----------
# The aspnet image is runtime-only (no compiler/SDK) — much smaller.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Bring over ONLY the published output. The SDK and source stay behind.
COPY --from=build /app/publish .

# These runtime images listen on port 8080 by default. Document it.
EXPOSE 8080

# How to start the container: run the compiled entry DLL with the runtime.
ENTRYPOINT ["dotnet", "MeetingRoomBooking.API.dll"]