# Millennium Eye first release implementation plan

> **For agentic workers:** Use superpowers:executing-plans for inline execution.

**Goal:** Publish and deploy Millennium Eye 0.1.0 with source and AMM support.
**Architecture:** Keep the repaired native UI reader, rename its managed
identity, build against locally generated IL2CPP references, and distribute
owned files through a validated package and the existing author catalog.
**Tech Stack:** C#/.NET 6, MelonLoader 0.7.2, Harmony, Tolk, SDL3, PowerShell,
GitHub CLI and AMM Author CLI 0.30.0.
**Spec:** ../specs/2026-10-13-first-release.md

## Global constraints

- Only ordinary player-visible information and native player-controlled actions.
- LGPL v3 and original BlindDuel attribution remain.
- Windows x64, Master Duel 2.8.0 / Steam build 24462996 is the initial target.
- Public downloads and catalog hashes must match exact verified package bytes.
- Game bindings, saves, settings, private logs and credentials stay local.

## Review focus

- Both reader DLLs present: refuse installation rather than duplicate hooks.
- Unknown loader or previous mod: retain its files and explain the conflict.
- Installation failure: restore replaced owned files from backups.
- Game update: preserve data and report unsupported build explicitly.
- Catalog changed concurrently: refresh and preserve all unrelated records.

## Task 1: Public source and provenance

- [x] Copy the repaired source and offline tests into an isolated new repository.
- [x] Rename managed identity to Millennium Eye 0.1.0 and add portable GamePath references.
- [x] Include original license, contributor notices, source provenance and native research summary.
- [x] Run the 21 reader checks and the full build; resolve current patch targets.

## Task 2: Installation and release packages

- [x] Provide verified speech/controller libraries, licenses and corresponding source.
- [x] Add read-only pre-install checks and standalone backup/rollback installation.
- [x] Test fresh installation, legacy replacement, refusal cases and failure recovery in temporary folders.
- [x] Build and validate standalone and AMM packages, plus checksums.
- [x] Back up current saves/settings and deploy exact package files; run a Steam startup check.

## Task 3: Publication

- [ ] Review source, package boundaries and migration behavior independently.
- [ ] Create buu420/MillenniumEye, push source and v0.1.0, and publish a GitHub prerelease.
- [ ] Verify anonymous downloads against the local assets.
- [ ] Add only the new Master Duel beta record to the refreshed buu420 catalog.
- [ ] Publish, verify the live index and directory submission, and record the evidence and limits.
