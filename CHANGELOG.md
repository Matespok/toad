
# Changelog
All notable changes to this project will be documented in this file.

## Small changes:
- Added ViewUser, which displays his threads
- Can now click on username, which will display users profile
- Edited class DbMethods, Post and User

## [0.1.1] - Apr-16-2026
- Migrated all database operations in DbMethods from synchronous to asynchronous (Task, await).
- Refactored DbMethods from a static utility class to instance-based methods to improve testability and lifetime management.
- Optimized connection handling: Connections are now opened asynchronously using await conn.OpenAsync().
- Updated PageModels (AddPost, FocusPost,...) to support the Post-Redirect-Get pattern with Task<IActionResult>.
### Changed
- Docker running :) ~~- not complete tho~~ - fully running in docker
- Small bugfixes
## [0.1.0] - Mar-13-2026
### Added
- Initial release (published to GitHub).
