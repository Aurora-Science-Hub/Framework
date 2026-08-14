# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Fixed

- `AuroraScienceHub.Framework.AspNetCore`: `BackgroundJobBase` now executes the job once on application startup (after the random delay) instead of waiting for the first full period to elapse. A job without a configured period runs once and stops instead of failing to start.
