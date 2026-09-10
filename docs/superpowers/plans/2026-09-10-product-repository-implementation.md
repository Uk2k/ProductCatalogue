# Product Repository Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Move product handlers behind an `IProductRepository` abstraction.

**Architecture:** Keep EF Core in `Data/ProductRepository.cs`; inject `IProductRepository` into all product handlers and register it in the persistence composition extension. Preserve the existing vertical-slice endpoints and contracts.

**Tech Stack:** .NET 8, EF Core SQL Server, xUnit, Testcontainers.

**Spec:** `docs/superpowers/specs/2026-09-10-product-repository-design.md`

## Tasks

- [x] Add `IProductRepository` and the EF Core `ProductRepository` implementation.
- [x] Register the repository as scoped with `AppDbContext`.
- [x] Update create, get, list, update, and delete handlers to depend on the abstraction.
- [x] Update handler tests and add repository lookup/order tests.
- [x] Document the repository boundary in the README.
- [x] Run the complete unit and integration test suites.
