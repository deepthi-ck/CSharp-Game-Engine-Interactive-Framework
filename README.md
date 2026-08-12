# C# Game Engine / Interactive Framework

Minimal Blazor + ASP.NET Core + SignalR multiplayer game platform demonstrating FE/BE .NET version matrix compatibility, in-memory sessions, sync/broadcast, TTL, eviction, statistics, and shared quality tooling.

## Branches (exactly 12)

| Branch | FE | BE |
|--------|----|----|
| CSharp_FE6_BE8 | 6 | 8 |
| CSharp_FE6_BE9 | 6 | 9 |
| CSharp_FE6_BE10 | 6 | 10 |
| CSharp_FE8_BE6 | 8 | 6 |
| CSharp_FE8_BE9 | 8 | 9 |
| CSharp_FE8_BE10 | 8 | 10 |
| CSharp_FE9_BE6 | 9 | 6 |
| CSharp_FE9_BE8 | 9 | 8 |
| CSharp_FE9_BE10 | 9 | 10 |
| CSharp_FE10_BE6 | 10 | 6 |
| CSharp_FE10_BE8 | 10 | 8 |
| CSharp_FE10_BE9 | 10 | 9 |

Same-version branches are forbidden.

## Build

```bash
python build.py
# or
./build.sh
```

## Layout

- `frontend_csharp/` Blazor game dashboard client
- `backend_csharp/` Game API + SignalR hub + orchestration
- `shared/` DTOs / version / config models
- `engine/` In-memory store (canonical)
- `distribution/` Session routing + sync (canonical)
- `quality/` Shared Scenario 2 tools

Conceptual inspiration only: [BlazorGame](https://github.com/dneimke/BlazorGame).
