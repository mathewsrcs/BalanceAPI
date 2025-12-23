# BalanceAPI

A REST API for managing bank account operations built with .NET 9.0 and ASP.NET Core.

## What It Does

Manage account balances with three simple operations:
- *Deposit* - Add money to an account
- *Withdraw* - Remove money from an account
- *Transfer* - Move money between accounts

## Technologies

- .NET 9.0
- ASP.NET Core
- xUnit (testing)
- In-memory data storage

## Getting Started

1. Install [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

2. Run the API:
```bash
cd src/BalanceApi.Api
dotnet run
```

3. Access at: http://localhost:5140

## API Endpoints

### GET /balance
Get account balance.
```bash
curl "http://localhost:5140/balance?account_id=100"
```

### POST /event
Process a financial event.

*Deposit:*
```json
{
  "type": "deposit",
  "destination": "100",
  "amount": 10
}
```

*Withdraw:*
```json
{
  "type": "withdraw",
  "origin": "100",
  "amount": 5
}
```

*Transfer:*
```json
{
  "type": "transfer",
  "origin": "100",
  "destination": "200",
  "amount": 15
}
```

### POST /reset
Clear all data.

## Running Tests

```bash
dotnet test
```

## Architecture

- Clean architecture with layered separation
- Repository pattern for data access
- Dependency injection
- Comprehensive unit testing