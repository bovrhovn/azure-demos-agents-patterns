# Tests

This directory contains all tests for the project.

## Structure

```
tests/
├── <pattern-name>/   # Tests mirroring the structure of src/
│   └── ...
└── README.md
```

## Running Tests

Add instructions for running the test suite here once the first tests are introduced. For example:

```bash
# Example — adjust to your toolchain
dotnet test
# or
pytest
```

## Guidelines

- Mirror the folder structure of [`src/`](../src/README.md) so tests are easy to locate.
- Write unit tests for individual components and integration tests for end-to-end agent scenarios.
- Aim for meaningful coverage of the agent patterns, not just line coverage.
