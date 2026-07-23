# NuGet release procedure

This repository keeps package creation separate from the normal solution build. The
`Build solution` workflow only restores and builds. Package creation and publication
are available through the manually dispatched `Manual NuGet release` workflow.

## Trusted Publishing setup

Publishing uses NuGet Trusted Publishing (OIDC), not a long-lived NuGet API key.
Configure one Trusted Publishing policy on nuget.org with these values:

- Repository Owner: `mfakane`
- Repository: `rawinput-sharp`
- Workflow File: `release.yml`
- Environment: leave empty (this workflow does not declare a GitHub environment)

Then add exactly one repository secret:

- `NUGET_USER`: the nuget.org profile name that owns the package; do not use an email address

The workflow has `id-token: write` permission and calls `NuGet/login@v1` during a
`publish=true` run. NuGet exchanges the GitHub OIDC token for a short-lived
credential, which is used only by that job. No `NUGET_API_KEY` secret is required.

## Dry run

1. Open **Actions → Manual NuGet release → Run workflow**.
2. Set `version` to the exact `<Version>` in `RawInput.Sharp/RawInput.Sharp.csproj`.
3. Leave `publish` set to `false`.
4. Optionally provide Markdown release notes.

The dry run restores and builds the library, creates both `.nupkg` and `.snupkg`,
checks that the package contains `README.md`, a nuspec, library files, and PDB
symbols, and uploads the packages and validation log as an artifact. It does not
contact NuGet and does not create a GitHub Release.

## Publishing

Publishing is a human/PO-controlled operation:

1. Confirm the version, release notes, build evidence, and intended scope with the PO.
2. Confirm the nuget.org Trusted Publishing policy and repository secret `NUGET_USER` are configured as described above.
3. Dispatch the workflow with the exact project version and set `publish` to `true`.
4. The workflow refuses to publish if the requested version does not equal the
   project `<Version>` or if the corresponding `v<version>` tag already exists.
5. On success it pushes the `.nupkg` to `https://api.nuget.org/v3/index.json` using
   the short-lived Trusted Publishing credential and creates the matching GitHub
   Release/tag with the `.nupkg` and `.snupkg` attached.

The workflow never publishes from a push to `master` or from a normal pull request.
A failed publish can be investigated from the workflow logs; rerun only after
confirming whether NuGet accepted the package and whether a GitHub Release/tag was
created. `--skip-duplicate` makes an already accepted package harmless on retry,
but an existing Git tag intentionally blocks the workflow until the situation is
resolved.

## Package guarantees and limits

The package metadata includes the repository URL, package README, zlib license,
release-note pointer, and Source Link build settings. The symbols package is
published in `snupkg` format. CI verifies package structure and the presence of PDB
symbols. This does not prove compatibility with every physical HID device or every
Windows/.NET runtime combination.
