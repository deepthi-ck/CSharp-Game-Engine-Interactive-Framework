# Shared Quality Tools

All Scenario 2 tools live under `quality/`.

- `config/` shared configuration
- `scripts/` runners (invoked by `build.py`)
- `reports/` centralized outputs

Tools interconnect: Coverlet/AltCover coverage artifacts feed the composite quality summary; static/mutation/audit reports are merged into the final gate.
