# Message Queue Console App Demo (.NET + RabbitMQ + Docker)

This project demonstrates a message queue system using two .NET console apps (SenderApp and ReceiverApp) that communicate via RabbitMQ.

<br>
<br>

## Getting Started

Requirements: 
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running
- Terminal open in root folder of solution

<br>
<br>

### Step 1: Build and Start the Containers

```
docker compose up --build -d
```
This will:
- Build SenderApp and ReceiverApp 
- Start a RabbitMQ container
- Start idle containers for the sender and receiver apps (won't run automatically)

<br>
<br>

### Step 2: Run Sender App Manually

```
docker exec -it senderapp dotnet SenderApp.dll
```
This will:
- Launch the SenderApp console
- Allow you to enter a name
- Close automatically once a name is entered

<br>
<br>

### Step 3: Run Receiver App Manually

```
docker exec -it receiverapp dotnet ReceiverApp.dll
```
This will:
- Launch the ReceiverApp console
- Receive message and ouput a custom greeting

<br>
<br>

### Stop Everything

```
docker compose down
```
This will stop and remove all containers